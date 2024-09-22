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
    /// Логика взаимодействия для AddEditPage.xaml
    /// </summary>
    public partial class AddEditPage : Page
    {
        public string FlagAddorEdit = "default";
        public Data.Product _currentProduct = new Data.Product();
        public AddEditPage(Data.Product product)
        {
            InitializeComponent();
            

            if (product != null) {
                
                _currentProduct =product;
                FlagAddorEdit = "edit";
            }
            else
            {
                FlagAddorEdit = "add";
            }
            DataContext = _currentProduct;
            Init();
        }

        public void Init()
        {
            try {
                CategoryComboBox.ItemsSource = Data.Pet_shopEntities.GetContext().ProductCategory.ToList();
                if (FlagAddorEdit == "add")
                {
                    IdTextBox.Visibility = Visibility.Hidden;
                    IdLabel.Visibility = Visibility.Hidden;
                    CategoryComboBox.SelectedItem = null;
                    CountTextBox.Text = string.Empty;
                    UnitTextBox.Text = string.Empty;
                    NameTextBox.Text = string.Empty;
                    CostTextBox.Text = string.Empty;
                    SupplierTextBox.Text = string.Empty;
                    DescriptionTextBox.Text = string.Empty;

                    //Product кинуть заглушку
                }
                else if (FlagAddorEdit == "edit")
                {
                    IdTextBox.Visibility = Visibility.Visible;
                    IdLabel.Visibility = Visibility.Visible;

                    CategoryComboBox.SelectedItem = null;
                    CountTextBox.Text = _currentProduct.ProductQuantityInStock.ToString();
                    UnitTextBox.Text = _currentProduct.Units.Name;
                    NameTextBox.Text = _currentProduct.ProductName1.NameName;
                    CostTextBox.Text = _currentProduct.ProductCost.ToString();
                    SupplierTextBox.Text = _currentProduct.ProductSupplier1.SupplierName;
                    DescriptionTextBox.Text = _currentProduct.ProductDescription;

                    IdTextBox.Text = Data.Pet_shopEntities.GetContext().Product.Max(d => d.Id + 1).ToString();// вопрос про id
                    CategoryComboBox.SelectedItem = Data.Pet_shopEntities.GetContext().ProductCategory.Where(d => d.CategoryId == _currentProduct.ProductCategory).FirstOrDefault();
                    //Product через xaml binding
                }
            } 
            
            catch
            {

            }
        }
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
