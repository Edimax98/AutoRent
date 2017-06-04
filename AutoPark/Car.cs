//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Drawing;
//using System.Windows;
//using System.Runtime.InteropServices;
//using System.Windows.Controls;
//using System.Windows.Media.Imaging;

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
using System.Data.SqlClient;
using System.Data;
using System.Windows.Markup;
using MaterialDesignThemes;
using System.Xml;
using System.IO;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.Drawing;

namespace AutoPark
{
    class Car
    {
        private int price;
        private string brand;
        private string specification;
        private string model;
        private BitmapImage image;
        private string number;
        private bool reserved;
        private string clas;
        private int discount;

        public Car(int price, string brand, string specification, string model, BitmapImage image, string number, bool reserved, string clas, int discount)
        {
            this.price = price;
            this.brand = brand;
            this.specification = specification;
            this.model = model;
            this.image = image;
            this.number = number;
            this.reserved = reserved;
            this.clas = clas;
            this.discount = discount;
        }

        public void CarButton_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            MessageBox.Show(getModel());
        }

        public void BookCarButton_Click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show(getDiscount());

            foreach (Window window in Application.Current.Windows)
            {
                if (window.GetType() == typeof(MainWindow))
                {
                    (window as MainWindow).BookingCarName.Text  = (window as MainWindow).BookingCarName.Text + " , " + getModel();
                    (window as MainWindow).finalPrice.Text = (window as MainWindow).carPriceForBook.ToString();
                    (window as MainWindow).MainTabControl.SelectedItem = (window as MainWindow).inputCarClientTabItem;
                    (window as MainWindow).carDiscount = getDiscount(); 
                    (window as MainWindow).AddCarCard.Visibility = Visibility.Collapsed;
                    (window as MainWindow).AddClientCard.Visibility = Visibility.Collapsed;
                    (window as MainWindow).BookCarCard.Visibility = Visibility.Visible;
                }
            }
        }

        public int getDiscount()
        {
            return this.discount;
        }

        public int getPrice()
        {
            return this.price;
        }

        public string getBrand()
        {
            return this.brand;
        }

        public string getSpecification()
        {
            return this.specification;
        }

        public string getModel()
        {
            return this.model;
        }

        public BitmapImage getImage() {
            return this.image;
        }

        public string getNumber()
        {
            return this.number;
        }

        public bool isReserved()
        {
            return this.reserved;
        }

        public string getClass()
        {
            return this.clas;
        }

    }
}
