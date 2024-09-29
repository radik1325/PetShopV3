using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Drawing;

namespace PetShop.Pages
{
    /// <summary>
    /// Логика взаимодействия для AddEditPage.xaml
    /// </summary>
    public partial class AddEditPage : Page
    {
        public byte[] imageBytes =null;
        public string FlagAddorEdit = "default";
        public Data.Product _currentProduct = new Data.Product();
        public AddEditPage(Data.Product product)
        {
            InitializeComponent();


            if (product != null)
            {

                _currentProduct = product;
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
            try
            {
                CategoryComboBox.ItemsSource = Data.Pet_shopEntities.GetContext().ProductCategory.ToList();
                if (FlagAddorEdit == "add")
                {
                    IdTextBox.Visibility = Visibility.Visible;
                    IdLabel.Visibility = Visibility.Visible;
                    IdTextBox.Text = Data.Pet_shopEntities.GetContext().Product.Max(d => d.Id + 1).ToString();// вопрос про id
                    
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
                    
                    IdTextBox.Visibility = Visibility.Hidden;
                    IdLabel.Visibility = Visibility.Hidden;
                    CategoryComboBox.SelectedItem = null;
                    CountTextBox.Text = _currentProduct.ProductQuantityInStock.ToString();
                    UnitTextBox.Text = _currentProduct.Units.Name;
                    NameTextBox.Text = _currentProduct.ProductName1.NameName;
                    CostTextBox.Text = _currentProduct.ProductCost.ToString();
                    SupplierTextBox.Text = _currentProduct.ProductSupplier1.SupplierName;
                    DescriptionTextBox.Text = _currentProduct.ProductDescription;

                    
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
            Classes.Manager.MainFrame.Navigate(new Pages.AdminLkPage());
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                StringBuilder errors = new StringBuilder();
                if (CategoryComboBox.SelectedItem == null)
                {
                    errors.AppendLine("Выберите категорию");
                }
                if (String.IsNullOrEmpty(CountTextBox.Text))
                {
                    errors.AppendLine("Заполните кол-во");
                }
                else
                {
                    var countTry = Int32.TryParse(CountTextBox.Text, out var reseltcount);
                    if (!countTry)
                    {
                        errors.AppendLine("Количество целое число");
                    }
                    else
                    {
                        if (reseltcount < 0)
                        {
                            errors.AppendLine("Количество не может быть отрицательной");
                        }
                    }
                }
                if (String.IsNullOrEmpty(UnitTextBox.Text))
                {
                    errors.AppendLine("Заполните ед. измер.");
                }
                if (String.IsNullOrEmpty(NameTextBox.Text))
                {
                    errors.AppendLine("Заполните наименование");
                }
                if (String.IsNullOrEmpty(CostTextBox.Text))
                {
                    errors.AppendLine("Заполните стоимость");
                }
                else
                {
                    var costry = Decimal.TryParse(CostTextBox.Text, out var resultCost);
                    if (!costry)
                    {
                        errors.AppendLine("Стоимость дробное число");
                    }
                    else
                    {
                        if (resultCost < 0)
                        {
                            errors.AppendLine("Стоимость не может быть отрицательной");
                        }
                        else
                        {
                            string text = CostTextBox.Text;
                            string[] parts = text.Split(new char[] { ',' });
                            if (parts.Length > 1)
                            {
                                int decimalPlaces = parts[1].Length;
                                if (decimalPlaces > 2)
                                {
                                    errors.AppendLine("У стоимости должно быть два знака после заяпятой");
                                }
                            }
                        }
                    }
                }
                if (String.IsNullOrEmpty(SupplierTextBox.Text))
                {
                    errors.AppendLine("Заполните поставщика");
                }
                if (String.IsNullOrEmpty(DescriptionTextBox.Text))
                {
                    errors.AppendLine("Заполните описание");
                }

                //Проверка на фото реализована в самом диалоге

                if (errors.Length > 0)
                {
                    MessageBox.Show(errors.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                
            }

            var selectedCategory = CategoryComboBox.SelectedItem as Data.ProductCategory;
            _currentProduct.ProductCategory = selectedCategory.CategoryId;//проверка подключенной таблицы таблицы
            _currentProduct.ProductQuantityInStock = Convert.ToInt32(CountTextBox.Text);
            _currentProduct.ProductCost =Convert.ToDecimal(CostTextBox.Text);
            _currentProduct.ProductDescription = DescriptionTextBox.Text;
            
            
            
            var searchUnit = (from item in Data.Pet_shopEntities.GetContext().Units
                              where item.Name == UnitTextBox.Text
                              select item).FirstOrDefault();
            if(searchUnit != null)
            {
                _currentProduct.IdUnits = searchUnit.id;

            }
            else
            {
                Data.Units new_units = new Data.Units()
                {
                    Name = UnitTextBox.Text
                };
                Data.Pet_shopEntities.GetContext().Units.Add(new_units);
                Data.Pet_shopEntities.GetContext().SaveChanges();
                _currentProduct.IdUnits = new_units.id;
            }

            

            var searchName = (from item in Data.Pet_shopEntities.GetContext().ProductName
                              where item.NameName == NameTextBox.Text
                              select item).FirstOrDefault();
            if (searchName != null)
            {
                _currentProduct.ProductName = searchName.NameId;

            }
            else
            {
                Data.ProductName new_name = new Data.ProductName()
                {
                    NameName = NameTextBox.Text
                };
                Data.Pet_shopEntities.GetContext().ProductName.Add(new_name);
                Data.Pet_shopEntities.GetContext().SaveChanges();
                _currentProduct.ProductName = new_name.NameId;
            }


            var searchSupplier = (from item in Data.Pet_shopEntities.GetContext().ProductSupplier
                              where item.SupplierName == SupplierTextBox.Text
                              select item).FirstOrDefault();
            if (searchSupplier != null)
            {
                _currentProduct.ProductSupplier = searchSupplier.SupplierId;

            }
            else
            {
                Data.ProductSupplier new_supplier = new Data.ProductSupplier()
                {
                    SupplierName = SupplierTextBox.Text
                };
                Data.Pet_shopEntities.GetContext().ProductSupplier.Add(new_supplier);
                Data.Pet_shopEntities.GetContext().SaveChanges();
                _currentProduct.ProductSupplier = new_supplier.SupplierId;
            }


            if (imageBytes != null)
            {
                _currentProduct.ProductPhoto = imageBytes;
                imageBytes = null;
            }

            if (FlagAddorEdit == "add")
            {
                var searchManufact = (from item in Data.Pet_shopEntities.GetContext().ProductManufacture
                                        where item.ManufactureName == "Производитель не указан"
                                      select item).FirstOrDefault();
                _currentProduct.ProductManufacturer = searchManufact.ManufactureId;
                Data.Pet_shopEntities.GetContext().Product.Add(_currentProduct);
                Data.Pet_shopEntities.GetContext().SaveChanges();
                MessageBox.Show("Успешно добавлено","Успех",MessageBoxButton.OK,MessageBoxImage.Information);
                Classes.Manager.MainFrame.Navigate(new Pages.AdminLkPage());
                
            }

            if (FlagAddorEdit == "edit")
            {
                try
                {
                    Data.Pet_shopEntities.GetContext().SaveChanges();
                    MessageBox.Show("Успешно добавлено", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    Classes.Manager.MainFrame.Navigate(new Pages.AdminLkPage());

                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }

        }

        public void ProductImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Images|*.jpg;*.jpeg;*.png;";
            if (openFileDialog.ShowDialog() == true)
            {
                
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(openFileDialog.FileName);
                bitmap.EndInit();

                if (bitmap.PixelWidth == 300 && bitmap.PixelHeight == 200)
                {
                    ProductImage.Source = bitmap;
                    imageBytes = File.ReadAllBytes(openFileDialog.FileName);
                }
                else
                {
                    MessageBox.Show("Размер изображения должен быть 300x200 пикселей.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }




            }
        }
    }
}
