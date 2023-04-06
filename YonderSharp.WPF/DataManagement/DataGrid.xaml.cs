using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using YonderSharp.Attributes;
using YonderSharp.WPF.Helper;
using YonderSharp.WPF.Helper.CustomDialogs;

namespace YonderSharp.WPF.DataManagement
{
    /// <summary>
    /// Interaction logic for DataGrid.xaml
    /// </summary>
    public partial class DataGrid : UserControl
    {
        DataGridVM _vm;

        public DataGrid()
        {
            InitializeComponent();
        }

        public DataGrid(IDataGridSource dataSource)
        {
            InitializeComponent();
            SetSource(dataSource);
            EntryList.SelectionChanged += (s, e) => EntryList.ScrollIntoView(EntryList.SelectedItem);
            dataSource.GetIDPropertyInfo();
        }

        private void GenerateFields(Grid grid, Type itemType, Tuple<string, Type>[] items, string baseBindingPath)
        {

            if (itemType == typeof(Type))
            {
                return;
            }

            //Maybe TODO: V2.0: Config objects that tell how to generate a line for even more flexibility....
            grid.RowDefinitions.Clear();
            grid.Children.Clear();

            var handledPositions = new List<int>();
            for (int i = 0; i < items.Length; i++)
            {
                PropertyInfo currentPropertyInfo = itemType.GetProperties().Where(x => x.Name == items[i].Item1).First();
                PrimaryKey primaryKeyAttribute = (PrimaryKey)currentPropertyInfo.GetCustomAttribute(typeof(PrimaryKey));
                if (primaryKeyAttribute != null)
                {
                    GenerateRow(grid, itemType, items[i], $"{baseBindingPath}.{items[i].Item1}");
                    handledPositions.Add(i);
                }
            }

            for (int i = 0; i < items.Length; i++)
            {
                if (handledPositions.Contains(i))
                {
                    continue;
                }

                PropertyInfo currentPropertyInfo = itemType.GetProperties().Where(x => x.Name == items[i].Item1).First();
                Title titleAttribute = (Title)currentPropertyInfo.GetCustomAttribute(typeof(Title));
                if (titleAttribute != null)
                {
                    GenerateRow(grid, itemType, items[i], $"{baseBindingPath}.{items[i].Item1}");
                    handledPositions.Add(i);
                }
            }

            string itemBindingPath;
            for (int i = 0; i < items.Length; i++)
            {
                if (handledPositions.Contains(i))
                {
                    continue;
                }

                itemBindingPath = $"{baseBindingPath}.{items[i].Item1}";
                //simple datatype
                if (CanBeHandledAsSimpleEntry(items[i].Item2))
                {
                    GenerateRow(grid, itemType, items[i], itemBindingPath);
                }
                //List of Class 
                else if (items[i].Item2.GenericTypeArguments?.Length > 0 && items[i].Item2.GetInterfaces().Any(x => x == typeof(IEnumerable)))
                {
                    GenerateComplexList(grid, itemType, items[i], itemBindingPath);
                }
                //Class property
                else
                {
                    GenerateComplexRow(grid, itemType, items[i], itemBindingPath);
                }
            }
        }

        private void GenerateComplexList(Grid parentGrid, Type itemType, Tuple<string, Type> tuple, string itemBindingPath)
        {
            if (itemType == typeof(Type))
            {
                return;
            }

            var margin = new Thickness(2, 2, 2, 2);
            parentGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

            //Add Label
            Label label = new Label();
            label.Content = _vm.DataSource.GetLabel(tuple.Item1);

            int parentRow = parentGrid.RowDefinitions.Count - 1;

            Grid.SetRow(label, parentRow);
            Grid.SetColumn(label, 0);
            parentGrid.Children.Add(label);

            //create expander
            Expander expander = new Expander();
            expander.Margin = margin;
            Grid.SetRow(expander, parentRow);
            Grid.SetColumn(expander, 1);
            parentGrid.Children.Add(expander);
        }


        private void GenerateComplexRow(Grid parentGrid, Type itemType, Tuple<string, Type> tuple, string bindingPath)
        {
            if (itemType == typeof(Type) || tuple.Item2 == typeof(Type))
            {
                return;
            }

            var margin = new Thickness(2, 2, 2, 2);
            parentGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

            //Add Label
            Label label = new Label();
            label.Content = _vm.DataSource.GetLabel(tuple.Item1);

            int parentRow = parentGrid.RowDefinitions.Count - 1;

            Grid.SetRow(label, parentRow);
            Grid.SetColumn(label, 0);
            parentGrid.Children.Add(label);

            //create new Grid
            Grid childGrid = new Grid();
            childGrid.Margin = margin;
            childGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = parentGrid.ColumnDefinitions[0].Width });
            childGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = parentGrid.ColumnDefinitions[1].Width });

            Grid.SetRow(childGrid, parentRow);
            Grid.SetColumn(childGrid, 1);
            parentGrid.Children.Add(childGrid);

            //call GenerateFields() with new grid to create the fields
            var subTypes = _vm.GetFields(tuple.Item2);
            GenerateFields(childGrid, tuple.Item2, subTypes, bindingPath);

        }

        private bool CanBeHandledAsSimpleEntry(Type item)
        {
            bool result = false;

            if (item.GenericTypeArguments?.Length > 0 && item.GetInterfaces().Any(x => x == typeof(IEnumerable)))
            {
                return CanBeHandledAsSimpleEntry(item.GenericTypeArguments[0]);
            }
            else
            {

                result = item.IsEnum || !item.IsClass || IsHashsetEnum(item) || item.Name.Equals("String", StringComparison.OrdinalIgnoreCase);

                if (!result)
                {
                    result = item.GenericTypeArguments?.Length > 0 && !item.GenericTypeArguments[0].IsClass;
                }
            }

            return result;
        }

        /// <param name="itemType">The datatype of this grid</param>
        /// <param name="item">Property of the row</param>
        /// <param name="row">0 based index of row to create</param>
        private void GenerateRow(Grid grid, Type itemType, Tuple<string, Type> item, string bindingPath)
        {
            if (itemType == typeof(Type))
            {
                return;
            }

            var margin = new Thickness(2, 2, 2, 2);
            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

            //Add Label
            Label label = new Label();
            label.Content = _vm.DataSource.GetLabel(item.Item1);

            int row = grid.RowDefinitions.Count - 1;


            Grid.SetRow(label, row);
            Grid.SetColumn(label, 0);
            grid.Children.Add(label);

            UIElement contentElement = null;
            PropertyInfo currentPropertyInfo = itemType.GetProperties().Where(x => x.Name == item.Item1).First();
            ForeignKey currentPropertyFkAttribute = (ForeignKey)currentPropertyInfo.GetCustomAttribute(typeof(ForeignKey));
            if (currentPropertyFkAttribute == null)
            {
                //Add content element
                if (IsHashsetEnum(item.Item2))
                {
                    ListView lView = new ListView();
                    lView.Margin = margin;
                    lView.Height = 150;


                    #region binding

                    Binding bind = new Binding(bindingPath);
                    bind.Mode = BindingMode.TwoWay;
                    bind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                    bind.Source = _vm;

                    lView.SetBinding(ListView.ItemsSourceProperty, bind);
                    #endregion binding

                    #region contextmenu
                    ContextMenu contextMenu = new ContextMenu();

                    MenuItem addNewItem = new MenuItem();
                    addNewItem.Header = Properties.Resources.Resources.Add;
                    addNewItem.Command = new RelayCommand(param =>
                    {
                        Array values = Enum.GetValues(item.Item2.GenericTypeArguments[0]);
                        object[] addableItems = new object[values.Length];
                        //dunno why System.Array ain't support by linq #sadface
                        for (int i = 0; i < values.Length; i++)
                        {
                            addableItems[i] = values.GetValue(i);
                        }


                        ComboboxDialog dialog = new ComboboxDialog(addableItems, item.Item1);
                        if (dialog.ShowDialog(Application.Current.MainWindow) == true)
                        {
                            var toAdd = values.GetValue(dialog.SelectedIndex);

                            dynamic entryList = _vm.SelectedItem.GetType().GetProperty(item.Item1).GetValue(_vm.SelectedItem, null);

                            MethodInfo method = entryList.GetType().GetMethod("Add");
                            object result = method.Invoke(entryList, new object[] { toAdd });

                            RefreshItemList(lView, item);
                        }
                    });

                    MenuItem removeItem = new MenuItem();
                    removeItem.Header = Properties.Resources.Resources.Remove;
                    removeItem.Command = new RelayCommand(param =>
                    {
                        if (lView.SelectedIndex == -1)
                        {
                            return;
                        }

                        //dynamic is expected to be IList<?>
                        dynamic entryList = currentPropertyInfo.GetValue(_vm.SelectedItem);
                        MethodInfo method = entryList.GetType().GetMethod("Remove");
                        method.Invoke(entryList, new object[] { lView.SelectedItem });

                        RefreshItemList(lView, item);
                    });

                    contextMenu.Items.Add(addNewItem);
                    contextMenu.Items.Add(removeItem);

                    lView.ContextMenu = contextMenu;
                    #endregion contextmenu
                    contentElement = lView;

                }
                else if (item.Item2.IsEnum)
                {
                    ComboBox cBox = new ComboBox();
                    cBox.ItemsSource = Enum.GetValues(item.Item2);

                    //TODO: if the value of the current item is 0 (= no value has been selected yet), the combobox doesn't clear
                    //      on change of the selected item
                    #region binding
                    Binding bind = new Binding(bindingPath);
                    bind.Mode = BindingMode.TwoWay;
                    bind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;

                    bind.Source = _vm;

                    cBox.SetBinding(ComboBox.SelectedItemProperty, bind);
                    #endregion binding

                    cBox.Margin = margin;
                    contentElement = cBox;
                }
                else if (item.Item2 == typeof(bool))
                {
                    CheckBox box = new CheckBox();
                    box.VerticalAlignment = VerticalAlignment.Center;
                    Binding bind = new Binding(bindingPath);
                    bind.Source = _vm;
                    box.SetBinding(CheckBox.IsCheckedProperty, bind);

                    contentElement = box;
                }
                else if (IsList(currentPropertyInfo))
                {
                    ListView lView = new ListView();
                    lView.Margin = margin;
                    lView.Height = 150;


                    #region binding

                    Binding bind = new Binding(bindingPath);
                    bind.Mode = BindingMode.TwoWay;
                    bind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                    bind.Source = _vm;

                    lView.SetBinding(ListView.ItemsSourceProperty, bind);
                    #endregion binding

                    #region contextmenu
                    ContextMenu contextMenu = new ContextMenu();

                    MenuItem addNewItem = new MenuItem();
                    addNewItem.Header = Properties.Resources.Resources.Add;
                    addNewItem.Command = new RelayCommand(param =>
                    {
                        var inputBoxDialog = new InputBoxDialog("MasterData", "Wert:");
                        inputBoxDialog.ShowDialog();
                        if (!(inputBoxDialog.DialogResult.HasValue && inputBoxDialog.DialogResult.Value))
                        {
                            return;
                        }

                        dynamic entryList = _vm.SelectedItem.GetType().GetProperty(item.Item1).GetValue(_vm.SelectedItem, null);

                        MethodInfo method = entryList.GetType().GetMethod("Add");
                        object result = method.Invoke(entryList, new object[] { inputBoxDialog.Answer });

                        RefreshItemList(lView, item);
                    });

                    MenuItem removeItem = new MenuItem();
                    removeItem.Header = Properties.Resources.Resources.Remove;
                    removeItem.Command = new RelayCommand(param =>
                    {
                        if (lView.SelectedIndex == -1)
                        {
                            return;
                        }

                        //dynamic is expected to be IList<?>
                        dynamic entryList = currentPropertyInfo.GetValue(_vm.SelectedItem);
                        MethodInfo method = entryList.GetType().GetMethod("Remove");
                        method.Invoke(entryList, new object[] { lView.SelectedItem });

                        RefreshItemList(lView, item);
                    });

                    contextMenu.Items.Add(addNewItem);
                    contextMenu.Items.Add(removeItem);

                    lView.ContextMenu = contextMenu;
                    #endregion contextmenu
                    contentElement = lView;
                }
                else
                {
                    TextBox box = new TextBox();

                    box.AcceptsReturn = true;
                    box.TextWrapping = TextWrapping.Wrap;

                    box.VerticalContentAlignment = VerticalAlignment.Center;
                    Binding bind = new Binding(bindingPath);
                    bind.Source = _vm;
                    bind.Mode = BindingMode.TwoWay;
                    bind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                    box.SetBinding(TextBox.TextProperty, bind);

                    box.Margin = margin;

                    contentElement = box;
                }

                if (_vm.IsPrimaryKeyDisabled && currentPropertyInfo.GetCustomAttribute(typeof(PrimaryKey)) != null)
                {
                    contentElement.IsEnabled = false;
                }
            }
            else if (!IsList(currentPropertyInfo))
            {
                //TODO: Update on new FK Source change (new item, removed item)
                ComboBox cBox = new ComboBox();

                cBox.ItemsSource = DataGridSourceManager.GetSource(currentPropertyFkAttribute.TargetClass).GetObservable();



                #region binding
                PropertyInfo fkTitleProperty = currentPropertyFkAttribute.TargetClass.GetProperties().First(x => x.GetCustomAttribute<Title>() != null);
                cBox.DisplayMemberPath = fkTitleProperty.Name;

                Binding bind = new Binding(bindingPath);
                bind.Mode = BindingMode.TwoWay;
                bind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;

                bind.Converter = new ForeignKeyConverter(DataGridSourceManager.GetSource(currentPropertyFkAttribute.TargetClass));
                bind.Source = _vm;

                cBox.SetBinding(ComboBox.SelectedItemProperty, bind);
                #endregion binding

                cBox.Margin = margin;
                contentElement = cBox;
            }
            else if (IsList(currentPropertyInfo))
            {
                ListView lView = new ListView();
                lView.Margin = margin;
                lView.Height = 150;


                #region binding
                PropertyInfo fkTitleProperty = currentPropertyFkAttribute.TargetClass.GetProperties().First(x => x.GetCustomAttribute<Title>() != null);
                lView.DisplayMemberPath = fkTitleProperty.Name;

                Binding bind = new Binding(bindingPath);
                bind.Mode = BindingMode.TwoWay;
                bind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;

                bind.Converter = new ForeignKeyConverter(DataGridSourceManager.GetSource(currentPropertyFkAttribute.TargetClass));
                bind.Source = _vm;

                lView.SetBinding(ListView.ItemsSourceProperty, bind);
                #endregion binding


                #region contextmenu
                ContextMenu contextMenu = new ContextMenu();

                MenuItem addNewItem = new MenuItem();
                addNewItem.Header = Properties.Resources.Resources.Add;
                addNewItem.Command = new RelayCommand(param =>
                {
                    var dataSource = DataGridSourceManager.GetSource(currentPropertyInfo.GetCustomAttribute<ForeignKey>().TargetClass);
                    ForeignKeyConverter converter = new ForeignKeyConverter(dataSource);

                    IList entryList = (IList)currentPropertyInfo.GetValue(_vm.SelectedItem);
                    object[] addableItems = dataSource.GetAllItems().Where(x => !entryList.Contains(converter.GetId(x))).ToArray();

                    if (addableItems?.Length > 0)
                    {
                        PropertyInfo fkTitleProperty = currentPropertyFkAttribute.TargetClass.GetProperties().First(x => x.GetCustomAttribute<Title>() != null);
                        string[] titles = addableItems.Select(x => fkTitleProperty.GetValue(x).ToString()).ToArray();

                        ComboboxDialog dialog = new ComboboxDialog(titles, item.Item1);
                        if (dialog.ShowDialog(Application.Current.MainWindow) == true)
                        {
                            object toAdd = addableItems[dialog.SelectedIndex];

                            //don't add the object itself, add it's PK to the list
                            entryList.Add(converter.ConvertBack(toAdd, toAdd.GetType(), null, null));
                        }
                    }

                    _vm.OnPropertyChanged("SelectedItem");
                });

                MenuItem removeItem = new MenuItem();
                removeItem.Header = Properties.Resources.Resources.Remove;
                removeItem.Command = new RelayCommand(param =>
                {
                    if (lView.SelectedIndex == -1)
                    {
                        return;
                    }

                    //dynamic is expected to be IList<?>
                    dynamic entryList = currentPropertyInfo.GetValue(_vm.SelectedItem);
                    entryList.RemoveAt(lView.SelectedIndex);

                    _vm.OnPropertyChanged("SelectedItem");
                });

                contextMenu.Items.Add(addNewItem);
                contextMenu.Items.Add(removeItem);

                lView.ContextMenu = contextMenu;
                #endregion contextmenu
                contentElement = lView;
            }

            if (contentElement == null)
            {
                throw new Exception("You somehow forgot to set the current element Ü");
            }

            contentElement.IsEnabled &= !_vm.IsReadOnlyMode;


            Grid.SetRow(contentElement, row);
            Grid.SetColumn(contentElement, 1);

            grid.Children.Add(contentElement);

            if (_vm.DataSource.IsFieldPartOfListText(item.Item1))
            {
                contentElement.LostFocus += RefreshList;
            }
        }

        private void RefreshItemList(ListView lView, Tuple<string, Type> item)
        {
            //Normally the ItemSource would need to implement INotifyCollectionChanged
            //but I would prefer not to force this upon the user...
            ICollectionView view = CollectionViewSource.GetDefaultView(lView.ItemsSource);
            view.Refresh();
        }


        private bool IsHashsetEnum(Type type)
        {
            return type.Name.Contains("Hashset", StringComparison.OrdinalIgnoreCase) && type?.GenericTypeArguments?.Length == 1 && type.GenericTypeArguments[0].IsEnum;
        }

        private Type[] listTypes = new[] { typeof(List<>), typeof(IList<>) };
        private bool IsList(PropertyInfo fkPropertyInfo)
        {
            return fkPropertyInfo.PropertyType.IsGenericType && listTypes.Any(x => listTypes.Contains(x));
        }

        private void RefreshList(object sender, RoutedEventArgs e)
        {
            _vm.UpdateList();
        }

        public void SetSource(DataGridVM vm)
        {
            _vm = vm ?? throw new ArgumentNullException(nameof(vm));
            DataContext = _vm;

            GenerateFields(ContentGrid, _vm.DataSource.GetTypeOfObjects(), _vm.GetFields(), "SelectedItem");

            //verify that the ID is avaiable
            var id = vm.DataSource.GetIDPropertyInfo();
            if (id == null && _vm.DataSource.GetConfiguration().IsPrimaryKeyRequired)
            {
                throw new Exception("ID not identified!");
            }
        }

        public void SetSource(IDataGridSource source, string saveButtonLabel = "")
        {
            var vm = new DataGridVM(source, ScrollToChangedEntry);
            if (!string.IsNullOrEmpty(saveButtonLabel))
            {
                vm.SaveButtonLabel = saveButtonLabel;
            }

            DataGridSourceManager.RegisterDataSource(source);

            SetSource(vm);
        }

        public void SelectItem(object item)
        {
            _vm.SelectedItem = item;
        }

        private void ScrollToChangedEntry()
        {
            ContentScroller.ScrollToTop();
            EntryList.ScrollIntoView(EntryList.SelectedItem);
        }
    }
}
