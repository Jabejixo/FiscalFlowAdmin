using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using FiscalFlowAdmin.Converter;
using FiscalFlowAdmin.Database;
using FiscalFlowAdmin.Database.Repositories;
using FiscalFlowAdmin.Helpers;
using FiscalFlowAdmin.Model;
using FiscalFlowAdmin.Model.Attributes;
using FiscalFlowAdmin.ViewModel;

using MaterialDesignThemes.Wpf;
using DataGridTextColumn = System.Windows.Controls.DataGridTextColumn;

namespace FiscalFlowAdmin.View.Controls;

    /// <summary>
    /// Логика взаимодействия для EntityManagementView.xaml
    /// </summary>
    public partial class EntityManagementView : UserControl
    {
        private readonly DataManager _dataManager;

        public EntityManagementView()
        {
            InitializeComponent();
            _dataManager = new DataManager();
            this.Loaded += EntityManagementView_Loaded;
            this.Unloaded += EntityManagementView_Unloaded;
        }

        /// <summary>
        /// Обработчик события загрузки UserControl.
        /// </summary>
        private async void EntityManagementView_Loaded(object sender, RoutedEventArgs e)
        {
            // Проверяем, что DataContext является BaseViewModel<T>
            var viewModelType = DataContext?.GetType();
            if (viewModelType == null || !viewModelType.IsGenericType || viewModelType.GetGenericTypeDefinition() != typeof(BaseViewModel<>))
                return;

            // Получаем тип модели T
            var modelType = viewModelType.GetGenericArguments()[0];

            // Получаем свойство FormFields через reflection
            var formFieldsProperty = viewModelType.GetProperty("FormFields");
            if (formFieldsProperty == null)
                return;

            // Получаем значение FormFields
            var formFields = formFieldsProperty.GetValue(DataContext) as IEnumerable<FormField>;
            if (formFields == null)
                return;

            // Генерируем форму
            await GenerateFormFields(formFields, modelType, viewModelType);

            // Генерируем DataGrid колонки
            GenerateDataGridColumns(DataGrid, DataContext, modelType, viewModelType);

            // Подписываемся на событие PropertyChanged ViewModel
            if (DataContext is INotifyPropertyChanged notifyPropertyChanged)
            {
                notifyPropertyChanged.PropertyChanged += ViewModel_PropertyChanged;
            }
        }

        /// <summary>
        /// Обработчик события выгрузки UserControl.
        /// </summary>
        private void EntityManagementView_Unloaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is INotifyPropertyChanged notifyPropertyChanged)
            {
                notifyPropertyChanged.PropertyChanged -= ViewModel_PropertyChanged;
            }
            _dataManager.Dispose();
        }

        /// <summary>
        /// Обработчик события изменения свойства ViewModel.
        /// </summary>
        private async void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CurrentItem")
            {
                // Обновляем DataContext для FormStackPanel
                FormStackPanel.DataContext = GetCurrentItem();
                await Task.CompletedTask; // Для сохранения асинхронности метода
            }
        }

        /// <summary>
        /// Генерирует контролы формы на основе FormFields.
        /// </summary>
        private async Task GenerateFormFields(IEnumerable<FormField> formFields, Type modelType, Type viewModelType)
        {
            FormStackPanel.Children.Clear();

            // Устанавливаем DataContext для FormStackPanel равным CurrentItem
            FormStackPanel.DataContext = GetCurrentItem();

            foreach (var formField in formFields.OrderBy(f => f.Order))
            {
                var propertyInfo = modelType.GetProperty(formField.PropertyName);

                // Проверяем наличие атрибута FormIgnore или DataGridIgnore
                if (propertyInfo.GetCustomAttribute<FormIgnoreAttribute>() != null ||
                    propertyInfo.GetCustomAttribute<DataGridIgnoreAttribute>() != null)
                    continue;

                // Пропускаем поле Id
                if (propertyInfo.Name == nameof(Base.Id))
                    continue;

                // Создаём Label для поля
                var label = new TextBlock
                {
                    Text = formField.DisplayName + (propertyInfo.GetCustomAttribute<RequiredAttribute>() != null ? " *" : ""),
                    ToolTip = propertyInfo.GetCustomAttribute<TooltipAttribute>()?.Text,
                    Margin = new Thickness(0, 5, 0, 0), // Добавляем отступ сверху
                    TextWrapping = TextWrapping.Wrap,
                    FontWeight = FontWeights.Bold
                };
                FormStackPanel.Children.Add(label);

                Control control = null;

                var foreignKeyAttribute = propertyInfo.GetCustomAttribute<ForeignKeyAttribute>();
                if (foreignKeyAttribute != null)
                {
                    // Используем ComboBox для связанных сущностей
                    control = await CreateComboBoxAsync(propertyInfo, formField, foreignKeyAttribute);
                }
                else if (formField.PropertyType == typeof(string))
                {
                    control = CreateStringControl(propertyInfo, formField);
                }
                else if (IsNumericType(formField.PropertyType))
                {
                    control = CreateNumericControl(propertyInfo, formField);
                }
                else if (formField.PropertyType == typeof(bool) || formField.PropertyType == typeof(bool?))
                {
                    control = CreateCheckBox(propertyInfo, formField);
                }
                else if (IsDateType(formField.PropertyType))
                {
                    control = CreateDateControl(propertyInfo, formField);
                }

                // Добавляем контрол в форму, если он создан
                if (control != null)
                {
                    FormStackPanel.Children.Add(control);
                }
            }

            // Добавляем кнопки действий
            AddActionButtons();

            // Запускаем анимацию появления формы
            AnimateFormAppearance();
        }

        /// <summary>
        /// Создаёт ComboBox для связанных сущностей на основе атрибута ForeignKey.
        /// </summary>
        private async Task<Control> CreateComboBoxAsync(PropertyInfo propertyInfo, FormField formField, ForeignKeyAttribute foreignKeyAttribute)
        {
            var comboBox = new ComboBox
            {
                IsEnabled = !(propertyInfo.GetCustomAttribute<ReadOnlyAttribute>()?.IsReadOnly ?? false),
                ToolTip = propertyInfo.GetCustomAttribute<TooltipAttribute>()?.Text,
                Style = (Style)FindResource("ComboBoxValidationStyle"),
                Margin = new Thickness(0, 5, 0, 5), // Добавляем отступы для лучшего UI
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Center
            };

            // Получаем имя свойства внешнего ключа (например, UserId)
            string foreignKeyPropertyName = foreignKeyAttribute.Name; // Убедитесь, что ForeignKeyAttribute имеет свойство Name

            // Получаем тип связанной сущности (например, User)
            var relatedEntityType = propertyInfo.PropertyType;
            PropertyInfo? repositoryProperty;

            repositoryProperty = typeof(DataManager).GetProperty(relatedEntityType.Name.EndsWith('y')
                ? $"{relatedEntityType.Name.Remove(relatedEntityType.Name.Length - 1)}ies"
                : $"{relatedEntityType.Name}s");

            // Получаем соответствующий репозиторий из DataManager
            if (repositoryProperty == null)
            {
                Debug.WriteLine($"Репозиторий для типа '{relatedEntityType.Name}' не найден в DataManager.");
                return comboBox;
            }

            var relatedRepository = repositoryProperty.GetValue(_dataManager);
            if (relatedRepository == null)
            {
                Debug.WriteLine($"Репозиторий для типа '{relatedEntityType.Name}' имеет значение null.");
                return comboBox;
            }

            // Используем reflection для вызова метода GetAllWithRelatedDataAsync с CancellationToken
            var getAllMethod = relatedRepository.GetType().GetMethod("GetAllWithRelatedDataAsync");
            if (getAllMethod == null)
            {
                Debug.WriteLine($"Метод 'GetAllWithRelatedDataAsync' не найден в репозитории '{relatedRepository.GetType().Name}'.");
                return comboBox;
            }

            try
            {
                // Вызываем метод асинхронно
                var task = (Task)getAllMethod.Invoke(relatedRepository, new object[] { CancellationToken.None });
                await task.ConfigureAwait(false);

                // Извлекаем результат из Task<T> через рефлексию
                var resultProperty = task.GetType().GetProperty("Result");
                var items = (IEnumerable)resultProperty.GetValue(task);

                Debug.WriteLine($"Получено {items?.Cast<object>().Count() ?? 0} элементов из репозитория '{relatedEntityType.Name}'.");

                if (items == null || !items.Cast<object>().Any())
                {
                    Debug.WriteLine($"Нет данных для ComboBox '{propertyInfo.Name}'.");
                }

                // Получаем имя свойства для отображения из атрибута DisplayMemberPath, если он есть
                var displayMemberPathAttr = propertyInfo.GetCustomAttribute<DisplayMemberPathAttribute>();
                var displayMemberPath = displayMemberPathAttr?.PropertyName ?? "Name"; // По умолчанию "Name"

                Debug.WriteLine($"DisplayMemberPath: {displayMemberPath}");

                // Обновляем UI в UI-потоке
                await Dispatcher.InvokeAsync(() =>
                {
                    comboBox.ItemsSource = items;
                    comboBox.DisplayMemberPath = displayMemberPath;
                    comboBox.SelectedValuePath = "Id"; // Предполагается, что у связанных сущностей есть свойство Id

                    // Привязка SelectedValue к свойству внешнего ключа через CurrentItem
                    var binding = new Binding(foreignKeyPropertyName)
                    {
                        Mode = BindingMode.TwoWay,
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                        ValidatesOnDataErrors = true,
                        ValidatesOnNotifyDataErrors = true,
                        NotifyOnValidationError = true
                    };
                    comboBox.SetBinding(ComboBox.SelectedValueProperty, binding);

                    Debug.WriteLine($"ComboBox '{propertyInfo.Name}' настроен с {items?.Cast<object>().Count() ?? 0} элементами.");
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка при вызове 'GetAllWithRelatedDataAsync': {ex.Message}");
                // Дополнительная обработка или уведомление пользователя
            }

            return comboBox;
        }

        /// <summary>
        /// Создаёт TextBox для строковых свойств с водяным знаком.
        /// </summary>
        private Control CreateStringControl(PropertyInfo propertyInfo, FormField formField)
        {
            var placeholderAttr = propertyInfo.GetCustomAttribute<PlaceholderAttribute>();
            string hint = placeholderAttr?.Text ?? string.Empty;

            var textBox = new TextBox
            {
                IsReadOnly = propertyInfo.GetCustomAttribute<ReadOnlyAttribute>()?.IsReadOnly ?? false,
                ToolTip = propertyInfo.GetCustomAttribute<TooltipAttribute>()?.Text,
                Style = (Style)FindResource("TextBoxValidationStyle"),
                Margin = new Thickness(0, 5, 0, 5), // Добавляем отступы
                Tag = hint,
                HorizontalAlignment = HorizontalAlignment.Stretch
            };

            var binding = new Binding(formField.PropertyName)
            {
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                ValidatesOnDataErrors = true,
                ValidatesOnNotifyDataErrors = true,
                NotifyOnValidationError = true,
                ValidatesOnExceptions = true
            };
            textBox.SetBinding(TextBox.TextProperty, binding);

            return textBox;
        }

        /// <summary>
        /// Создаёт TextBox для числовых свойств с водяным знаком.
        /// </summary>
        private Control CreateNumericControl(PropertyInfo propertyInfo, FormField formField)
        {
            var placeholderAttr = propertyInfo.GetCustomAttribute<PlaceholderAttribute>();
            string hint = placeholderAttr?.Text ?? string.Empty;

            var textBox = new TextBox
            {
                IsReadOnly = propertyInfo.GetCustomAttribute<ReadOnlyAttribute>()?.IsReadOnly ?? false,
                ToolTip = propertyInfo.GetCustomAttribute<TooltipAttribute>()?.Text,
                Style = (Style)FindResource("TextBoxValidationStyle"),
                Margin = new Thickness(0, 5, 0, 5), // Добавляем отступы
                Tag = hint,
                HorizontalAlignment = HorizontalAlignment.Stretch
            };

            var binding = new Binding(formField.PropertyName)
            {
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                ValidatesOnDataErrors = true,
                ValidatesOnNotifyDataErrors = true,
                NotifyOnValidationError = true,
                StringFormat = propertyInfo.GetCustomAttribute<DisplayFormatAttribute>()?.DataFormatString
            };
            textBox.SetBinding(TextBox.TextProperty, binding);

            return textBox;
        }

        /// <summary>
        /// Создаёт CheckBox для булевых свойств.
        /// </summary>
        private Control CreateCheckBox(PropertyInfo propertyInfo, FormField formField)
        {
            var checkBox = new CheckBox
            {
                IsEnabled = !(propertyInfo.GetCustomAttribute<ReadOnlyAttribute>()?.IsReadOnly ?? false),
                ToolTip = propertyInfo.GetCustomAttribute<TooltipAttribute>()?.Text,
                Style = (Style)FindResource("CheckBoxValidationStyle"),
                Margin = new Thickness(0, 5, 0, 5), // Добавляем отступы
                VerticalAlignment = VerticalAlignment.Center
            };

            var binding = new Binding(formField.PropertyName)
            {
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                ValidatesOnDataErrors = true,
                ValidatesOnNotifyDataErrors = true,
                NotifyOnValidationError = true,
                Converter = propertyInfo.PropertyType == typeof(bool?) ? (IValueConverter)FindResource("NullableBoolToBoolConverter") : null
            };
            checkBox.SetBinding(CheckBox.IsCheckedProperty, binding);

            return checkBox;
        }

        /// <summary>
        /// Создаёт DatePicker для свойств типа DateTime или DateOnly с водяным знаком.
        /// </summary>
        private Control CreateDateControl(PropertyInfo propertyInfo, FormField formField)
        {
            var placeholderAttr = propertyInfo.GetCustomAttribute<PlaceholderAttribute>();
            string hint = placeholderAttr?.Text ?? string.Empty;

            var datePicker = new DatePicker
            {
                IsEnabled = !(propertyInfo.GetCustomAttribute<ReadOnlyAttribute>()?.IsReadOnly ?? false),
                ToolTip = propertyInfo.GetCustomAttribute<TooltipAttribute>()?.Text,
                Style = (Style)FindResource("DatePickerValidationStyle"),
                Margin = new Thickness(0, 5, 0, 5), // Добавляем отступы
                Tag = hint,
                HorizontalAlignment = HorizontalAlignment.Stretch
            };

            var binding = new Binding(formField.PropertyName)
            {
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                ValidatesOnDataErrors = true,
                ValidatesOnNotifyDataErrors = true,
                NotifyOnValidationError = true,
                Converter = propertyInfo.PropertyType == typeof(DateOnly?) ? (IValueConverter)FindResource("DateOnlyToDateTimeConverter") : null,
                StringFormat = propertyInfo.GetCustomAttribute<DisplayFormatAttribute>()?.DataFormatString
            };
            datePicker.SetBinding(DatePicker.SelectedDateProperty, binding);

            return datePicker;
        }

        /// <summary>
        /// Определяет, является ли тип числовым.
        /// </summary>
        private bool IsNumericType(Type type)
        {
            return type == typeof(int) || type == typeof(double) || type == typeof(decimal) ||
                   type == typeof(long) || type == typeof(float) || type == typeof(short) ||
                   type == typeof(uint) || type == typeof(ulong) || type == typeof(ushort);
        }

        /// <summary>
        /// Определяет, является ли тип датой.
        /// </summary>
        private bool IsDateType(Type type)
        {
            return type == typeof(DateTime) || type == typeof(DateTime?) ||
                   type == typeof(DateOnly) || type == typeof(DateOnly?);
        }

        /// <summary>
        /// Добавляет кнопки действий (Создать, Обновить, Удалить) в форму с иконками и анимациями.
        /// </summary>
        private void AddActionButtons()
        {
            var viewModelType = DataContext?.GetType();
            if (viewModelType == null || !viewModelType.IsGenericType || viewModelType.GetGenericTypeDefinition() != typeof(BaseViewModel<>))
                return;

            var viewModel = DataContext;

            var buttonPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(10)
            };

            var createCommand = viewModelType.GetProperty("CreateCommand")?.GetValue(viewModel) as ICommand;
            var updateCommand = viewModelType.GetProperty("UpdateCommand")?.GetValue(viewModel) as ICommand;
            var deleteCommand = viewModelType.GetProperty("DeleteCommand")?.GetValue(viewModel) as ICommand;

            // Кнопка "Создать"
            if (createCommand != null)
            {
                var createButton = new Button
                {
                    Command = createCommand,
                    Margin = new Thickness(5),
                    Padding = new Thickness(10, 5, 10, 5),
                    Style = (Style)FindResource("MaterialDesignRaisedButton")
                };

                // Создаём StackPanel для иконки и текста
                var createContent = new StackPanel
                {
                    Orientation = Orientation.Horizontal
                };

                var createIcon = new PackIcon
                {
                    Kind = PackIconKind.ContentSave, // Иконка сохранения
                    Width = 16,
                    Height = 16,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(0, 0, 5, 0)
                };

                var createText = new TextBlock
                {
                    Text = "Создать",
                    VerticalAlignment = VerticalAlignment.Center
                };

                createContent.Children.Add(createIcon);
                createContent.Children.Add(createText);
                createButton.Content = createContent;

                // Добавление анимации наведения
                AddButtonHoverAnimation(createButton);

                buttonPanel.Children.Add(createButton);
            }

            // Кнопка "Обновить"
            if (updateCommand != null)
            {
                var updateButton = new Button
                {
                    Command = updateCommand,
                    Margin = new Thickness(5),
                    Padding = new Thickness(10, 5, 10, 5),
                    Style = (Style)FindResource("MaterialDesignRaisedButton")
                };

                var updateContent = new StackPanel
                {
                    Orientation = Orientation.Horizontal
                };

                var updateIcon = new PackIcon
                {
                    Kind = PackIconKind.ContentSaveEdit, // Иконка редактирования сохранения
                    Width = 16,
                    Height = 16,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(0, 0, 5, 0)
                };

                var updateText = new TextBlock
                {
                    Text = "Обновить",
                    VerticalAlignment = VerticalAlignment.Center
                };

                updateContent.Children.Add(updateIcon);
                updateContent.Children.Add(updateText);
                updateButton.Content = updateContent;

                // Добавление анимации наведения
                AddButtonHoverAnimation(updateButton);

                buttonPanel.Children.Add(updateButton);
            }

            // Кнопка "Удалить"
            if (deleteCommand != null)
            {
                var deleteButton = new Button
                {
                    Command = deleteCommand,
                    Margin = new Thickness(5),
                    Padding = new Thickness(10, 5, 10, 5),
                    Style = (Style)FindResource("MaterialDesignRaisedButton")
                };

                var deleteContent = new StackPanel
                {
                    Orientation = Orientation.Horizontal
                };

                var deleteIcon = new PackIcon
                {
                    Kind = PackIconKind.Delete, // Иконка удаления
                    Width = 16,
                    Height = 16,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(0, 0, 5, 0)
                };

                var deleteText = new TextBlock
                {
                    Text = "Удалить",
                    VerticalAlignment = VerticalAlignment.Center
                };

                deleteContent.Children.Add(deleteIcon);
                deleteContent.Children.Add(deleteText);
                deleteButton.Content = deleteContent;

                // Добавление анимации наведения
                AddButtonHoverAnimation(deleteButton);

                buttonPanel.Children.Add(deleteButton);
            }

            FormStackPanel.Children.Add(buttonPanel);
        }

        /// <summary>
        /// Добавляет анимацию при наведении курсора на кнопку.
        /// </summary>
        /// <param name="button">Кнопка, к которой добавляется анимация.</param>
        private void AddButtonHoverAnimation(Button button)
        {
            // Создаем анимацию увеличения масштаба
            var scaleUp = new DoubleAnimation
            {
                To = 1.05,
                Duration = TimeSpan.FromMilliseconds(200)
            };

            var scaleDown = new DoubleAnimation
            {
                To = 1.0,
                Duration = TimeSpan.FromMilliseconds(200)
            };

            // Создаем Storyboard для MouseEnter
            var storyboardEnter = new Storyboard();
            storyboardEnter.Children.Add(scaleUp);
            Storyboard.SetTarget(scaleUp, button);
            Storyboard.SetTargetProperty(scaleUp, new PropertyPath("RenderTransform.(ScaleTransform.ScaleX)"));

            var storyboardEnterY = new Storyboard();
            storyboardEnterY.Children.Add(scaleUp);
            Storyboard.SetTarget(scaleUp, button);
            Storyboard.SetTargetProperty(scaleUp, new PropertyPath("RenderTransform.(ScaleTransform.ScaleY)"));

            // Создаем Storyboard для MouseLeave
            var storyboardLeave = new Storyboard();
            storyboardLeave.Children.Add(scaleDown);
            Storyboard.SetTarget(scaleDown, button);
            Storyboard.SetTargetProperty(scaleDown, new PropertyPath("RenderTransform.(ScaleTransform.ScaleX)"));

            var storyboardLeaveY = new Storyboard();
            storyboardLeaveY.Children.Add(scaleDown);
            Storyboard.SetTarget(scaleDown, button);
            Storyboard.SetTargetProperty(scaleDown, new PropertyPath("RenderTransform.(ScaleTransform.ScaleY)"));

            // Устанавливаем RenderTransform, если не установлен
            if (!(button.RenderTransform is ScaleTransform))
            {
                button.RenderTransform = new ScaleTransform(1.0, 1.0);
                button.RenderTransformOrigin = new Point(0.5, 0.5);
            }

            // Создаем и добавляем EventTrigger для MouseEnter
            var triggerEnter = new EventTrigger(Button.MouseEnterEvent);
            triggerEnter.Actions.Add(new BeginStoryboard { Storyboard = storyboardEnter });
            triggerEnter.Actions.Add(new BeginStoryboard { Storyboard = storyboardEnterY });
            button.Triggers.Add(triggerEnter);

            // Создаем и добавляем EventTrigger для MouseLeave
            var triggerLeave = new EventTrigger(Button.MouseLeaveEvent);
            triggerLeave.Actions.Add(new BeginStoryboard { Storyboard = storyboardLeave });
            triggerLeave.Actions.Add(new BeginStoryboard { Storyboard = storyboardLeaveY });
            button.Triggers.Add(triggerLeave);
        }

        /// <summary>
        /// Генерирует столбцы DataGrid на основе свойств модели.
        /// </summary>
        private void GenerateDataGridColumns(DataGrid dataGrid, object viewModel, Type modelType, Type viewModelType)
        {
            dataGrid.Columns.Clear();

            var properties = modelType.GetProperties();

            // Сортируем свойства по атрибуту Order
            var orderedProperties = properties.OrderBy(p => p.GetCustomAttribute<OrderAttribute>()?.Order ?? int.MaxValue);

            foreach (var property in orderedProperties)
            {
                // Проверяем наличие атрибута DataGridIgnore
                if (property.GetCustomAttribute<DataGridIgnoreAttribute>() != null)
                    continue;

                // Получаем имя столбца из атрибутов Display или DisplayName
                var displayAttribute = property.GetCustomAttribute<DisplayAttribute>();
                var displayNameAttribute = property.GetCustomAttribute<DisplayNameAttribute>();
                var header = displayAttribute?.Name ?? displayNameAttribute?.DisplayName ?? property.Name;

                DataGridColumn column = null;

                // Проверяем, является ли свойство навигационным (связанной сущностью)
                if (typeof(Base).IsAssignableFrom(property.PropertyType))
                {
                    // Получаем имя свойства для отображения из атрибута DisplayMemberPath
                    var displayMemberPathAttr = property.GetCustomAttribute<DisplayMemberPathAttribute>();
                    var displayMemberPath = displayMemberPathAttr?.PropertyName ?? "Name"; // По умолчанию "Name"

                    // Создаём столбец с привязкой к связанному свойству
                    column = new DataGridTextColumn
                    {
                        Header = header,
                        Binding = new Binding($"{property.Name}.{displayMemberPath}")
                        {
                            StringFormat = property.GetCustomAttribute<DisplayFormatAttribute>()?.DataFormatString
                        }
                    };
                }
                else if (typeof(IEnumerable).IsAssignableFrom(property.PropertyType) && property.PropertyType != typeof(string))
                {
                    // Обработка коллекций связанных сущностей
                    var collectionAttr = property.GetCustomAttribute<CollectionAttribute>();
                    var displayMember = collectionAttr?.DisplayMember ?? "Name"; // По умолчанию "Name"

                    // Создаём конвертер для отображения коллекции
                    var converter = new CollectionToStringConverter
                    {
                        DisplayMember = displayMember
                    };

                    column = new DataGridTextColumn
                    {
                        Header = header,
                        Binding = new Binding(property.Name) { Converter = converter }
                    };
                }
                else
                {
                    // Обычное свойство
                    column = new DataGridTextColumn
                    {
                        Header = header,
                        Binding = new Binding(property.Name)
                        {
                            StringFormat = property.GetCustomAttribute<DisplayFormatAttribute>()?.DataFormatString
                        }
                    };
                }

                if (column != null)
                {
                    dataGrid.Columns.Add(column);
                }
            }
        }

        /// <summary>
        /// Возвращает текущий элемент (CurrentItem) из DataContext.
        /// </summary>
        private Base? GetCurrentItem()
        {
            var viewModel = DataContext;
            if (viewModel == null)
                return null;

            var currentItemProperty = viewModel.GetType().GetProperty("CurrentItem");
            if (currentItemProperty == null)
                return null;

            return currentItemProperty.GetValue(viewModel) as Base;
        }

        /// <summary>
        /// Анимирует появление формы (Fade In).
        /// </summary>
        private void AnimateFormAppearance()
        {
            var storyboard = new Storyboard();

            var fadeIn = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = new Duration(TimeSpan.FromSeconds(0.5))
            };
            Storyboard.SetTarget(fadeIn, FormStackPanel);
            Storyboard.SetTargetProperty(fadeIn, new PropertyPath(UIElement.OpacityProperty));

            storyboard.Children.Add(fadeIn);
            storyboard.Begin();
        }
    }
