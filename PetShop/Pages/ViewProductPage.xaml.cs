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
            ProductListView.ItemsSource = Data.Pet_shopEntities.GetContext().Product.ToList();
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void SourtUpRadioButton_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void SourtDownRadioButton_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
