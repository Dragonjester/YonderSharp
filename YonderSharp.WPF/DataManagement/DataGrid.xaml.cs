using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Printing;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Navigation;
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

        private void GenerateFields(Type itemType, Tuple<string, Type>[] items)
        {
            //Maybe TODO: V2.0: Config objects that tell how to generate a line for even more flexibility....
            ContentGrid.RowDefinitions.Clear();
            ContentGrid.Children.Clear();

            var handledPositions = new List<int>();

            for (int i = 0; i < items.Length; i++)
            {
                PropertyInfo currentPropertyInfo = itemType.GetProperties().Where(x => x.Name == items[i].Item1).First();
                PrimaryKey primaryKeyAttribute = (PrimaryKey)currentPropertyInfo.GetCustomAttribute(typeof(PrimaryKey));
                if (primaryKeyAttribute != null)
                {
                    GenerateRow(itemType, items[i]);
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
                    GenerateRow(itemType, items[i]);
                    handledPositions.Add(i);
                }
            }

            for (int i = 0; i < items.Length; i++)
            {
                if (handledPositions.Contains(i))
                {
                    continue;
                }

                GenerateRow(itemType, items[i]);
            }
        }

        /// <param name="itemType">The datatype of this grid</param>
        /// <param name="item">Property of the row</param>
        /// <param name="row">0 based index of row to create</param>
        private void GenerateRow(Type itemType, Tuple<string, Type> item)
        {
            var margin = new Thickness(2, 2, 2, 2);
            ContentGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

            //Add Label
            Label label = new Label();
            label.Content = _vm.DataSource.GetLabel(item.Item1);

            int row = ContentGrid.RowDefinitions.Count - 1;


            Grid.SetRow(label, row);
            Grid.SetColumn(label, 0);
            ContentGrid.Children.Add(label);

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

                    Binding bind = new Binding($"SelectedItem.{item.Item1}");
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
                    Binding bind = new Binding($"SelectedItem.{item.Item1}");
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
                    Binding bind = new Binding($"SelectedItem.{item.Item1}");
                    bind.Source = _vm;
                    box.SetBinding(CheckBox.IsCheckedProperty, bind);

                    contentElement = box;
                }
                else
                {
                    TextBox box = new TextBox();
                    box.VerticalContentAlignment = VerticalAlignment.Center;
                    Binding bind = new Binding($"SelectedItem.{item.Item1}");
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

                cBox.ItemsSource = DataGridSourceManager.GetSource(currentPropertyFkAttribute.TargetClass).GetAllItems();

                #region binding
                PropertyInfo fkTitleProperty = currentPropertyFkAttribute.TargetClass.GetProperties().First(x => x.GetCustomAttribute<Title>() != null);
                cBox.DisplayMemberPath = fkTitleProperty.Name;

                Binding bind = new Binding($"SelectedItem.{item.Item1}");
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

                Binding bind = new Binding($"SelectedItem.{item.Item1}");
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

            Grid.SetRow(contentElement, row);
            Grid.SetColumn(contentElement, 1);

            ContentGrid.Children.Add(contentElement);

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

        private bool IsList(PropertyInfo fkPropertyInfo)
        {
            return fkPropertyInfo.PropertyType.IsGenericType && (fkPropertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(List<>));
        }

        private void RefreshList(object sender, RoutedEventArgs e)
        {
            _vm.UpdateList();
        }

        public void SetSource(IDataGridSource source)
        {
            _vm = new DataGridVM(source, ScrollToChangedEntry);
            DataContext = _vm;
            GenerateFields(_vm.DataSource.GetTypeOfObjects(), _vm.GetFields());

            //verify that the ID is avaiable
            var id = source.GetIDPropertyInfo();
            if (id == null)
            {
                throw new Exception("ID not identified!");
            }
        }

        private void ScrollToChangedEntry()
        {
            ContentScroller.ScrollToTop();
            EntryList.ScrollIntoView(EntryList.SelectedItem);
        }
    }
}
