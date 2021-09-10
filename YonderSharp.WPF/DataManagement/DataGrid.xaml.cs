using DeltahedronUI.DataManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using YonderSharp.Attributes;
using YonderSharp.WPF.Helper;
using YonderSharp.WPF.Helper.CustomDialogs;

namespace YonderSharp.WPF.DataManagement {
    /// <summary>
    /// Interaction logic for DataGrid.xaml
    /// </summary>
    public partial class DataGrid : UserControl {
        DataGridVM _vm;

        public DataGrid() {
            InitializeComponent();
        }

        public DataGrid(IDataGridSource dataSource) {
            InitializeComponent();
            SetSource(dataSource);
            EntryList.SelectionChanged += (s, e) => EntryList.ScrollIntoView(EntryList.SelectedItem);
            dataSource.GetIDPropertyInfo();
        }

        private void GenerateFields(Type itemType, Tuple<string, Type>[] items) {
            //Maybe TODO: V2.0: Config objects that tell how to generate a line for even more flexibility....

            ContentGrid.RowDefinitions.Clear();
            ContentGrid.Children.Clear();
            var margin = new Thickness(2, 2, 2, 2);

            for(int i = 0; i < items.Length; i++) {
                var item = items[i];
                ContentGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

                //Add Label
                Label label = new Label();
                label.Content = _vm.DataSource.GetLabel(item.Item1);
                Grid.SetRow(label, i);
                Grid.SetColumn(label, 0);
                ContentGrid.Children.Add(label);

                UIElement contentElement = null;
                PropertyInfo fkPropertyInfo = itemType.GetProperties().Where(x => x.Name == item.Item1).First();
                ForeignKey fkProperty = (ForeignKey)fkPropertyInfo.GetCustomAttribute(typeof(ForeignKey));
                if(fkProperty == null) {
                    //Add content element
                    if(item.Item2 == typeof(bool)) {
                        CheckBox box = new CheckBox();
                        box.VerticalAlignment = VerticalAlignment.Center;
                        Binding bind = new Binding($"SelectedItem.{item.Item1}");
                        bind.Source = _vm;
                        box.SetBinding(CheckBox.IsCheckedProperty, bind);

                        contentElement = box;
                    } else {
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

                    if(_vm.IsPrimaryKeyDisabled && fkPropertyInfo.GetCustomAttribute(typeof(PrimaryKey)) != null) {
                        contentElement.IsEnabled = false;
                    }
                } else if(!IsList(fkPropertyInfo)) {
                    //TODO: Update on new FK Source change (new item, removed item)
                    ComboBox cBox = new ComboBox();

                    cBox.ItemsSource = DataGridSourceManager.GetSource(fkProperty.TargetClass).GetAllItems();

                    #region binding
                    PropertyInfo fkTitleProperty = fkProperty.TargetClass.GetProperties().First(x => x.GetCustomAttribute<Title>() != null);
                    cBox.DisplayMemberPath = fkTitleProperty.Name;

                    Binding bind = new Binding($"SelectedItem.{item.Item1}");
                    bind.Mode = BindingMode.TwoWay;
                    bind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;

                    bind.Converter = new ForeignKeyConverter(DataGridSourceManager.GetSource(fkProperty.TargetClass));
                    bind.Source = _vm;

                    cBox.SetBinding(ComboBox.SelectedItemProperty, bind);
                    #endregion binding

                    cBox.Margin = margin;
                    contentElement = cBox;
                } else if(IsList(fkPropertyInfo)) {
                    ListView lView = new ListView();
                    lView.Margin = margin;
                    lView.Height = 150;


                    #region binding
                    PropertyInfo fkTitleProperty = fkProperty.TargetClass.GetProperties().First(x => x.GetCustomAttribute<Title>() != null);
                    lView.DisplayMemberPath = fkTitleProperty.Name;

                    Binding bind = new Binding($"SelectedItem.{item.Item1}");
                    bind.Mode = BindingMode.TwoWay;
                    bind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;

                    bind.Converter = new ForeignKeyConverter(DataGridSourceManager.GetSource(fkProperty.TargetClass));
                    bind.Source = _vm;

                    lView.SetBinding(ListView.ItemsSourceProperty, bind);
                    #endregion binding


                    #region contextmenu
                    ContextMenu contextMenu = new ContextMenu();

                    MenuItem addNewItem = new MenuItem();
                    addNewItem.Header = "Hinzufügen"; //TODO: Translation
                    addNewItem.Command = new RelayCommand(param => {
                        var dataSource = DataGridSourceManager.GetSource(fkPropertyInfo.GetCustomAttribute<ForeignKey>().TargetClass);
                        ForeignKeyConverter converter = new ForeignKeyConverter(dataSource);

                        IList entryList = (IList)fkPropertyInfo.GetValue(_vm.SelectedItem);
                        object[] addableItems = dataSource.GetAllItems().Where(x => !entryList.Contains(converter.GetId(x))).ToArray();

                        if(addableItems?.Length > 0) {
                            PropertyInfo fkTitleProperty = fkProperty.TargetClass.GetProperties().First(x => x.GetCustomAttribute<Title>() != null);
                            string[] titles = addableItems.Select(x => fkTitleProperty.GetValue(x).ToString()).ToArray();

                            ComboboxDialog dialog = new ComboboxDialog(titles);
                            if(dialog.ShowDialogInCenterOfCurrent()) {
                                object toAdd = addableItems[dialog.SelectedIndex];

                                //don't add the object itself, add it's PK to the list
                                entryList.Add(converter.ConvertBack(toAdd, toAdd.GetType(), null, null));

                            }
                        }

                        _vm.OnPropertyChanged("SelectedItem");
                    });

                    MenuItem removeItem = new MenuItem();
                    removeItem.Header = "Entfernen"; //TODO: Translation
                    removeItem.Command = new RelayCommand(param => {
                        if(lView.SelectedIndex == -1) {
                            return;
                        }

                        //dynamic is expected to be IList<?>
                        dynamic entryList = fkPropertyInfo.GetValue(_vm.SelectedItem);
                        entryList.RemoveAt(lView.SelectedIndex);

                        _vm.OnPropertyChanged("SelectedItem");
                    });

                    contextMenu.Items.Add(addNewItem);
                    contextMenu.Items.Add(removeItem);

                    lView.ContextMenu = contextMenu;
                    #endregion contextmenu
                    contentElement = lView;
                }

                if(contentElement == null) {
                    throw new Exception("You somehow forgot to set the current element Ü");
                }

                Grid.SetRow(contentElement, i);
                Grid.SetColumn(contentElement, 1);
                ContentGrid.Children.Add(contentElement);

                if(_vm.DataSource.IsFieldPartOfListText(item.Item1)) {
                    contentElement.LostFocus += RefreshList;
                }
            }
        }

        private bool IsList(PropertyInfo fkPropertyInfo) {
            return fkPropertyInfo.PropertyType.IsGenericType && (fkPropertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(List<>));
        }

        private void RefreshList(object sender, RoutedEventArgs e) {
            _vm.UpdateList();
        }

        public void SetSource(IDataGridSource source) {
            _vm = new DataGridVM(source);
            DataContext = _vm;
            GenerateFields(_vm.DataSource.GetTypeOfObjects(), _vm.GetFields());

            //verify that the ID is avaiable
            var id = source.GetIDPropertyInfo();
            if(id == null) {
                throw new Exception("ID not identified!");
            }
        }
    }
}
