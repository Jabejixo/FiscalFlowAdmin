   M S B A M L   `   `   `  ����J  FFiscalFlowAdmin, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullM+clr-namespace:FiscalFlowAdmin.View.ControlsFiscalFlowAdmin.View.Controls  5      E'clr-namespace:FiscalFlowAdmin.ViewModelFiscalFlowAdmin.ViewModel  5      E'clr-namespace:FiscalFlowAdmin.ConverterFiscalFlowAdmin.Converter  5      8    2FiscalFlowAdmin.View.Controls.EntityManagementView   5      W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      5      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      =local+clr-namespace:FiscalFlowAdmin.View.Controls     5      6vm'clr-namespace:FiscalFlowAdmin.ViewModel     5      =	converter'clr-namespace:FiscalFlowAdmin.Converter     5      _ [MaterialDesignThemes.Wpf, Version=5.1.1.0, Culture=neutral, PublicKeyToken=df2a72020bd7962aFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  5	      >mdIcons0http://materialdesigninxaml.net/winfx/xaml/icons  5
      -   5          >�Name$  
EntityView��5       � 	Resources 5         ValidationErrorTemplate �� Path �� RelativeSource �� Mode �� AncestorType;   5FiscalFlowAdmin.Converter.NullableBoolToBoolConverter  NullableBoolToBoolConverter;   5FiscalFlowAdmin.Converter.DateOnlyToDateTimeConverter  DateOnlyToDateTimeConverter;   5FiscalFlowAdmin.Converter.CollectionToStringConverter  CollectionToStringConverter  TextBoxValidationStyle �� 
TargetType �� Property �� Value)  #MaterialDesignThemes.Wpf.HintAssist.  (MaterialDesignThemes.Wpf.TextFieldAssist	 �� Triggers
 P� Property P� Value  ComboBoxValidationStyle  DatePickerValidationStyle(  "System.Windows.Controls.DatePicker  CheckBoxValidationStyle��%�  &	        &	 r    &	 �    &	 �    &	 �    7   &	 �    7   &	     7   &	 �    7   �� 5   
   .R�5      �� .+�5      ]� $
��True��6   .+�5      �� 
���  ��$��*��61   $��Center=�6:   $��0,0,5,0q�6U   �� 5      5      �� 5      
���  ��$��12(�6-   $��25,0,0,0q�5      ��5      �� $8 2AdornedElement.(Validation.Errors)[0].ErrorContent� �� $ FindAncestor=� Adorner5      5      5        5   
   60     5   
   60     5    
   60   �� 5#   
   " ��6/   .�5$      ��  �@=�ErrorTemplate8   69   �� 5%       � ��$ 0,5,0,5q�6'   �� 5&       �@ Hint 6?   �� *Tag+ �� *Self+�� 5'       �@ HasClearButton$
 True��6N   	 5(      P� 5)      
 �@=�HasError$
 True��69   . �5*      ��  �@�ToolTip 5+      ��  �� *Self+$) #(Validation.Errors)[0].ErrorContent��� 5-       � ��
 �  ��64   �� 5.       � ��$ 1q�68   5/      51      �� 53   
   " ��60   .�54      ��  �@=�ErrorTemplate8   69   �� 55       � ��$ 0,5,0,5q�6'   	 56      P� 57      
 �@=�HasError$
 True��69   . �58      ��  �@�ToolTip 59      ��  �� *Self+$) #(Validation.Errors)[0].ErrorContent��� 5;       � ��
 �  ��64   �� 5<       � ��$ 1q�68   5=      5?      �� 5A   
   "  62   .�5B      ��  �@=�ErrorTemplate8   69   �� 5C       � ��$ 0,5,0,5q�6'   �� 5D       �@ Hint 6?   �� *Tag+ �� *Self+	 5E      P� 5F      
 �@=�HasError$
 True��69   . �5G      ��  �@�ToolTip 5H      ��  �� *Self+$) #(Validation.Errors)[0].ErrorContent��� 5J       � ��
 �  ��64   �� 5K       � ��$ 1q�68   5L      5N      �� 5P   
   " ��60   .�5Q      ��  �@=�ErrorTemplate8   69   �� 5R       � ��$ 0,5,0,5q�6'   	 5S      P� 5T      
 �@=�HasError$
 True��69   . �5U      ��  �@�ToolTip 5V      ��  �� *Self+$) #(Validation.Errors)[0].ErrorContent��� 5X       � ��
 �  ��64   �� 5Y       � ��$ 1q�68   5Z      5\      5]      .��5_      �  � RowDefinitions 5`   
   �� 5a      $
��Auto �6   �� 5b      $��* �6    � ColumnDefinitions 5d   
   �� 5e      $��3* �6   �� 5f      $��2* �6   .+�5j   
   ��  �� Orientation$ 
Horizontal=�6   $��10q�6.   $��0��6:   $��2��6G   $��Stretch=�5k      $	��Top=�63   .+�5m      �� $	��200��6   $��0,0,10,0q�5o      #��[ 5p      
 � Tag$ Поиск...��5q      ��5n      �� *SearchQuery+ �� UpdateSourceTrigger$ PropertyChanged=�5q      �� 5r      $��0,0,10,0q�5s      ��5r      �� *ClearSearchCommand+.��5t      �� $ 
Horizontal=�6   .+�5u      '  !MaterialDesignThemes.Wpf.PackIcon     Kind$ Refresh=�6.   $��16��6=   $��16��6H   $��0,0,5,0q�6T   �� 5v      $!��Очистить поиск��6    $��Center=�66   5w      5x      �� 5{      $"��Сортировать по:��6   $��Center=�5|      $��0,0,5,0q�5}      $
��Wrap=�5~      �� 5      $	��150��5�      $��0,0,10,0q�5�      #��[ 5�      ��5      �� *SortableProperties+ �� SelectedItem 5�      �� *
SortColumn+5�      �� 5�      $!��По возрастанию��6   $��Center=�5�      $��0,0,10,0q�5�      #��[ 5�       d� 	IsChecked 5�      �� *SortAscending+5�      �� 5�      $��Фильтр:��6   $��Center=�5�      $��0,0,5,0q�5�      $
��Wrap=�5�      �� 5�      $	��150��5�      $��0,0,5,0q�5�      #��[ 5�      ��5�      �� *FilterableProperties+ 5�      �� *SelectedFilterProperty+5�      �� 5�      $	��150��6   $��0,0,5,0q�5�      #��[ 5�      $2 ,Значение для фильтрации��5�      ��5�      �� *FilterValue+$ PropertyChanged=�5�      �� 5�      $��0,0,5,0q�5�      ��5�      �� *ApplyFilterCommand+.��5�      �� $ 
Horizontal=�6   .+�5�        $ Filter=�6.   $��16��6<   $��16��6G   $��0,0,5,0q�6S   �� 5�      $%��Применить фильтр��6    $��Center=�68   5�      5�      �� 5�      ��6   �� *ClearFilterCommand+.��5�      �� $ 
Horizontal=�6   .+�5�        $ FilterRemove=�6.   $��16��6B   $��16��6M   $��0,0,5,0q�6Y   �� 5�      $#��Сбросить фильтр��6    $��Center=�67   5�      5�      5�      &   System.Windows.Controls.DataGrid  5�   
   -   5          Name$ DataGrid��5�      $��1��6%   $��0��62      AutoGenerateColumns$ False��5�         CanUserAddRows$ False��5�         
IsReadOnly$
 True��5�         EnableRowVirtualization$
 True��5�      !   EnableColumnVirtualization$
 True��5�      $��10q�5�      $��Stretch=�5�      $��Stretch=�5�      ��5�      �� *FilteredItems+ 5�      �� *SelectedItem+ �� Mode$ TwoWay=�5�      �� 5�   
   $��1��6   $��1��6$   $
��Auto=�64   .��5�      �� -   5         ��Name$ FormStackPanel��5�      $��10q�61    J� Opacity$ 0Q�6=   5�      5�      5�      5�      