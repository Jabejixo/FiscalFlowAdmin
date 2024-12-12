using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using FiscalFlowAdmin.Database.Repositories;
using FiscalFlowAdmin.Database.Repositories.Authentication;
using FiscalFlowAdmin.Helpers;
using FiscalFlowAdmin.Model;
using FiscalFlowAdmin.Model.Attributes;
using FiscalFlowAdmin.ViewModel.Commands;
using ValidationResult = System.ComponentModel.DataAnnotations.ValidationResult;

namespace FiscalFlowAdmin.ViewModel
{
    public sealed class BaseViewModel<T> : ViewModelBase, INotifyPropertyChanged where T : Base, new()
    {
        private IRepository<T> Repository { get; }

        private ObservableCollection<T>? _items;
        public ObservableCollection<T> Items
        {
            get => _items ??= new ObservableCollection<T>();
            set { _items = value; OnPropertyChanged(); }
        }

        private ObservableCollection<T>? _filteredItems;
        public ObservableCollection<T> FilteredItems
        {
            get => _filteredItems ??= new ObservableCollection<T>();
            set { _filteredItems = value; OnPropertyChanged(); }
        }

        private long _currentItemId = 0;

        private T? _selectedItem;
        public T? SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (_selectedItem != value)
                {
                    _selectedItem = value;
                    OnPropertyChanged(nameof(SelectedItem));
                    if (value != null)
                    {
                        CurrentItem = CopyItem(value, copyId: false); // Не копировать Id для редактирования
                    }
                    else
                    {
                        CurrentItem = new T();
                    }
                    RaiseCommandCanExecuteChanged();
                }
            }
        }

        private T? _currentItem;
        public T? CurrentItem
        {
            get => _currentItem;
            set
            {
                if (_currentItem != value)
                {
                    if (_currentItem != null)
                    {
                        _currentItem.PropertyChanged -= OnCurrentItemPropertyChanged;
                        _currentItem.ErrorsChanged -= OnCurrentItemErrorsChanged;
                    }

                    _currentItem = value;
                    OnPropertyChanged(nameof(CurrentItem));

                    if (_currentItem != null)
                    {
                        _currentItem.PropertyChanged += OnCurrentItemPropertyChanged;
                        _currentItem.ErrorsChanged += OnCurrentItemErrorsChanged;
                        _currentItem.ValidateAllProperties(); // Валидируем все свойства изначально
                    }

                    RaiseCommandCanExecuteChanged();
                }
            }
        }

        private void OnCurrentItemPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            // Инициируем повторную валидацию изменённого свойства
            _currentItem?.ValidateProperty(e.PropertyName, _currentItem.GetType().GetProperty(e.PropertyName)?.GetValue(_currentItem));
            RaiseCommandCanExecuteChanged();
        }

        private void OnCurrentItemErrorsChanged(object? sender, DataErrorsChangedEventArgs e)
        {
            RaiseCommandCanExecuteChanged();
        }

        // Определение команд
        public ICommand CreateCommand { get; set; }
        public ICommand UpdateCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand ClearSearchCommand { get; set; }
        public ICommand ApplyFilterCommand { get; set; }
        public ICommand ClearFilterCommand { get; set; }

        public ObservableCollection<FormField> FormFields { get; set; }

        // Сортировка и фильтрация
        private string _searchQuery = string.Empty;
        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                if (_searchQuery != value)
                {
                    _searchQuery = value;
                    OnPropertyChanged();
                    ApplyFilteringSortingSearching();
                }
            }
        }

        private string _sortColumn = string.Empty;
        public string SortColumn
        {
            get => _sortColumn;
            set
            {
                if (_sortColumn != value)
                {
                    _sortColumn = value;
                    OnPropertyChanged();
                    ApplyFilteringSortingSearching();
                }
            }
        }

        private bool _sortAscending = true;
        public bool SortAscending
        {
            get => _sortAscending;
            set
            {
                if (_sortAscending != value)
                {
                    _sortAscending = value;
                    OnPropertyChanged();
                    ApplyFilteringSortingSearching();
                }
            }
        }

        public Dictionary<string, string> FilterCriteria { get; set; } = new();

        private string _selectedFilterProperty = string.Empty;
        public string SelectedFilterProperty
        {
            get => _selectedFilterProperty;
            set
            {
                if (_selectedFilterProperty != value)
                {
                    _selectedFilterProperty = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _withRelatedData;

        private string _filterValue = string.Empty;
        public string FilterValue
        {
            get => _filterValue;
            set
            {
                if (_filterValue != value)
                {
                    _filterValue = value;
                    OnPropertyChanged();
                }
            }
        }

        public List<string> SortableProperties { get; set; }
        public List<string> FilterableProperties { get; set; }

        // Конструктор
        public BaseViewModel(IRepository<T> repository, bool withRelatedData = true)
        {
            Repository = repository;

            CreateCommand = new RelayCommand(async (_) => await CreateAsync(), (_) => CanCreate());
            UpdateCommand = new RelayCommand(async (_) => await UpdateAsync(), (_) => CanUpdate());
            DeleteCommand = new RelayCommand(async (_) => await DeleteAsync(), (_) => CanDelete());
            ClearSearchCommand = new RelayCommand(_ => ClearSearch(), _ => !string.IsNullOrEmpty(SearchQuery));
            ApplyFilterCommand = new RelayCommand(_ => ApplyFilter(), _ => CanApplyFilter());
            ClearFilterCommand = new RelayCommand(_ => ClearFilter(), _ => FilterCriteria.Count > 0);

            // Инициализация коллекций
            Items = new ObservableCollection<T>();
            FilteredItems = new ObservableCollection<T>();
            _withRelatedData = withRelatedData;
            // Загрузка данных асинхронно
            _ = LoadDataAsync();

            GenerateFormFields();

            SortableProperties = GetSortableProperties();
            FilterableProperties = GetFilterableProperties();

            // Инициализация CurrentItem как нового объекта для создания
            CurrentItem = new T();
        }

        /// <summary>
        /// Генерирует FormFields на основе свойств модели.
        /// </summary>
        private void GenerateFormFields()
        {
            FormFields = new ObservableCollection<FormField>();
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var property in properties)
            {
                if (property.GetCustomAttribute<FormIgnoreAttribute>() != null)
                    continue;

                var displayAttribute = property.GetCustomAttribute<DisplayAttribute>();
                var displayNameAttribute = property.GetCustomAttribute<DisplayNameAttribute>();
                var displayName = displayAttribute?.Name ?? displayNameAttribute?.DisplayName ?? property.Name;

                var orderAttr = property.GetCustomAttribute<OrderAttribute>();
                var order = orderAttr?.Order ?? int.MaxValue;

                var formField = new FormField()
                {
                    PropertyName = property.Name,
                    PropertyType = property.PropertyType,
                    DisplayName = displayName,
                    Order = order,
                    ValidationAttributes = property.GetCustomAttributes<ValidationAttribute>()
                };

                FormFields.Add(formField);
            }
        }

        /// <summary>
        /// Загружает данные из репозитория.
        /// </summary>
        private async Task LoadDataAsync()
        {
            IEnumerable<T> data;
            if (!_withRelatedData)
            {
                data = await Repository.GetAllAsync();
            }
            else
            {
                data = await Repository.GetAllWithRelatedDataAsync();
            }

            Items = new ObservableCollection<T>(data);
            ApplyFilteringSortingSearching();
        }

        // Методы команд
        private async Task CreateAsync()
        {
            if (CurrentItem != null)
            {
                CurrentItem.ValidateAllProperties();
                if (CurrentItem.HasErrors)
                {
                    MessageBox.Show("Пожалуйста, исправьте ошибки перед созданием.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                bool itemIsUser = false;
                bool success = false;
                FilterableProperties.ForEach(item =>
                {
                    if (item == "Password")
                    {
                        itemIsUser = true;
                        return;
                    }
                });
                if (itemIsUser)
                {
                    var repo = Repository as UserRepository;
                    if (repo != null)
                    {
                        CurrentItem.Id = 0; // Убедиться, что Id = 0 для автоинкремента
                        success = await repo.AddAsync(CurrentItem as User);
                    }
                }
                else
                {
                    CurrentItem.Id = 0; // Убедиться, что Id = 0 для автоинкремента
                    success = await Repository.AddAsync(CurrentItem);
                }
                if (success)
                {
                    await LoadDataAsync();
                    SelectedItem = null;
                    CurrentItem = new T(); // Сброс CurrentItem для нового ввода
                }
                else
                {
                    MessageBox.Show("Не удалось добавить сущность.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private bool CanCreate() => CurrentItem != null && !CurrentItem.HasErrors;

        private async Task UpdateAsync()
        {
            if (CurrentItem != null)
            {
                CurrentItem.ValidateAllProperties();
                if (CurrentItem.HasErrors)
                {
                    MessageBox.Show("Пожалуйста, исправьте ошибки перед обновлением.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                CurrentItem.Id = _currentItemId;
                var success = await Repository.UpdateAsync(CurrentItem);
                if (success)
                {
                    await LoadDataAsync();
                    SelectedItem = null;
                    CurrentItem = new T(); // Сброс CurrentItem после обновления
                }
                else
                {
                    MessageBox.Show("Не удалось обновить сущность.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private bool CanUpdate() => SelectedItem != null && CurrentItem != null && !CurrentItem.HasErrors;

        private async Task DeleteAsync()
        {
            if (SelectedItem != null)
            {
                var confirmResult = MessageBox.Show($"Вы уверены, что хотите удалить сущность с ID {SelectedItem.Id}?",
                                                    "Подтверждение удаления",
                                                    MessageBoxButton.YesNo,
                                                    MessageBoxImage.Warning);
                if (confirmResult == MessageBoxResult.Yes)
                {
                    var success = await Repository.DeleteAsync(SelectedItem.Id);
                    if (success)
                    {
                        await LoadDataAsync();
                        SelectedItem = null;
                        CurrentItem = new T(); // Сброс CurrentItem после удаления
                    }
                    else
                    {
                        MessageBox.Show("Не удалось удалить сущность.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private bool CanDelete() => SelectedItem != null;

        /// <summary>
        /// Возвращает список свойств, доступных для сортировки.
        /// </summary>
        private List<string> GetSortableProperties()
        {
            return typeof(T).GetProperties()
                            .Where(p => p.PropertyType.IsPrimitive || p.PropertyType == typeof(string) || p.PropertyType == typeof(DateTime) || p.PropertyType == typeof(DateOnly))
                            .Select(p => p.Name)
                            .ToList();
        }

        /// <summary>
        /// Возвращает список свойств, доступных для фильтрации.
        /// </summary>
        private List<string> GetFilterableProperties()
        {
            return typeof(T).GetProperties()
                            .Where(p => p.PropertyType == typeof(string) || p.PropertyType.IsPrimitive || p.PropertyType == typeof(DateTime) || p.PropertyType == typeof(DateOnly))
                            .Select(p => p.Name)
                            .ToList();
        }

        /// <summary>
        /// Применяет фильтрацию, сортировку и поиск к данным.
        /// </summary>
        private void ApplyFilteringSortingSearching()
        {
            IEnumerable<T> query = Items;

            // Применяем фильтрацию
            foreach (var criteria in FilterCriteria)
            {
                var propertyInfo = typeof(T).GetProperty(criteria.Key);
                if (propertyInfo != null)
                {
                    query = query.Where(item =>
                    {
                        var value = propertyInfo.GetValue(item);
                        if (value != null)
                        {
                            return value.ToString().IndexOf(criteria.Value, StringComparison.OrdinalIgnoreCase) >= 0;
                        }
                        return false;
                    });
                }
            }

            // Применяем поиск
            if (!string.IsNullOrEmpty(SearchQuery))
            {
                var stringProperties = typeof(T).GetProperties().Where(p => p.PropertyType == typeof(string));
                query = query.Where(item =>
                {
                    foreach (var property in stringProperties)
                    {
                        var value = property.GetValue(item) as string;
                        if (!string.IsNullOrEmpty(value) && value.IndexOf(SearchQuery, StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            return true;
                        }
                    }
                    return false;
                });
            }

            // Применяем сортировку
            if (!string.IsNullOrEmpty(SortColumn))
            {
                var propertyInfo = typeof(T).GetProperty(SortColumn);
                if (propertyInfo != null)
                {
                    query = SortAscending ? query.OrderBy(item => propertyInfo.GetValue(item)) : query.OrderByDescending(item => propertyInfo.GetValue(item));
                }
            }

            FilteredItems = new ObservableCollection<T>(query);
        }

        private void ClearSearch()
        {
            SearchQuery = string.Empty;
            ApplyFilteringSortingSearching();
        }

        private void ClearFilter()
        {
            FilterCriteria.Clear();
            ApplyFilteringSortingSearching();
        }

        private bool CanApplyFilter()
        {
            return !string.IsNullOrEmpty(SelectedFilterProperty) && !string.IsNullOrEmpty(FilterValue);
        }

        private void ApplyFilter()
        {
            if (!string.IsNullOrEmpty(SelectedFilterProperty) && !string.IsNullOrEmpty(FilterValue))
            {
                FilterCriteria[SelectedFilterProperty] = FilterValue;
                ApplyFilteringSortingSearching();
            }
        }

        /// <summary>
        /// Копирует свойства из исходного объекта в новый объект.
        /// </summary>
        private T? CopyItem(T? source, bool copyId = true)
        {
            if (source == null)
                return null;

            T? target = new T();

            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead && p.CanWrite);

            foreach (var property in properties)
            {
                if (!copyId && property.Name == nameof(Base.Id))
                {
                    _currentItemId = (long)(property.GetValue(source) ?? 0);
                    property.SetValue(target, 0L); // Установить Id = 0 для автоинкремента
                }
                else
                {
                    var value = property.GetValue(source);
                    property.SetValue(target, value);
                }
            }

            return target;
        }

        /// <summary>
        /// Поднимает событие изменения возможности выполнения команд.
        /// </summary>
        private void RaiseCommandCanExecuteChanged()
        {
            (CreateCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (UpdateCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (DeleteCommand as RelayCommand)?.RaiseCanExecuteChanged();
        }
    }
}
