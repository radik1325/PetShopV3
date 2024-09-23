using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PetShop.Pages
{
    /// <summary>
    /// Логика взаимодействия для ViewProductPage.xaml
    /// </summary>
    public partial class ViewProductPage : Page
    {
        public ViewProductPage()
        {
            InitializeComponent();
            Init();
           

        }
        public void Init()
        {

            ProductListView.ItemsSource = Data.Pet_shopEntities.GetContext().Product.ToList();

            var manufactList = Data.Pet_shopEntities.GetContext().ProductManufacture.ToList();
            manufactList.Insert(0, new Data.ProductManufacture { ManufactureName = "Все производители" });
            ManufacturedComboBox.ItemsSource = manufactList;
            ManufacturedComboBox.SelectedIndex = 0;

            if (Classes.Manager.CurrentUser != null) {
                FIOLabel.Visibility = Visibility.Visible;
                FIOLabel.Content = $"{Classes.Manager.CurrentUser.UserSurname} " + $"{Classes.Manager.CurrentUser.UserSurname} " +
                    $"{Classes.Manager.CurrentUser.UserPatronymic}";
            }
            else
            {
                FIOLabel.Visibility = Visibility.Hidden;
            }
            CountOfLabel.Content = $"{Data.Pet_shopEntities.GetContext().Product.Count()}/" +
                $"{Data.Pet_shopEntities.GetContext().Product.Count()}";
        }
        public List<Data.Product> _CurrentProduct = Data.Pet_shopEntities.GetContext().Product.ToList();

        public void Update()
        {
            try
            {
                _CurrentProduct = Data.Pet_shopEntities.GetContext().Product.ToList();

                _CurrentProduct = (from item in Data.Pet_shopEntities.GetContext().Product
                                   where item.ProductName1.NameName.ToLower().Contains(SearchTextBox.Text) ||
                                   item.ProductDescription.ToLower().Contains(SearchTextBox.Text) ||
                                   item.ProductManufacture.ManufactureName.ToLower().Contains(SearchTextBox.Text) ||
                                   item.ProductCost.ToString().Contains(SearchTextBox.Text) ||
                                   item.ProductQuantityInStock.ToString().Contains(SearchTextBox.Text)
                                   select item).ToList();
                if (SourtUpRadioButton.IsChecked == true)
                {
                    _CurrentProduct = _CurrentProduct.OrderBy(d => d.ProductCost).ToList();
                }
                if (SourtDownRadioButton.IsChecked == true)
                {
                    _CurrentProduct = _CurrentProduct.OrderByDescending(d => d.ProductCost).ToList();
                }
                var selected = ManufacturedComboBox.SelectedItem as Data.ProductManufacture;
                if (selected != null && selected.ManufactureName != "Все производители")
                {
                    _CurrentProduct = _CurrentProduct.Where(d => d.ProductManufacture.ManufactureId == selected.ManufactureId).ToList();
                }
                CountOfLabel.Content = $"{_CurrentProduct.Count}/{Data.Pet_shopEntities.GetContext().Product.Count()}";
                ProductListView.ItemsSource = _CurrentProduct;
            }
            catch
            {

            }
        }
        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Update();
        }

        private void SourtUpRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            Update();
        }

        private void SourtDownRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            Update();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
               // var selected = (sender as Button).DataContext as Data.Product;
                    //var orderProduct = Data.Pet_shopEntities.GetContext().OrderProduct
                    //.Where(d => d.ProductArticleNumber == selected.Id).Tolist();
            }
            catch
            {

            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (Classes.Manager.MainFrame.CanGoBack)
            {
                if(Classes.Manager.CurrentUser != null)
                {
                    Classes.Manager.CurrentUser = null;
                }
                Classes.Manager.MainFrame.GoBack();
            }
        }

        private void ManufacturedComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Update();
        }
    }
}
