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
using System;
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
        public int calculatedDics;
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
            foreach (Window window in Application.Current.Windows)
            {
                if (window.GetType() == typeof(MainWindow))
                {
                    (window as MainWindow).carNumberInfo.Text = getNumber();
                    (window as MainWindow).CarNameInfo.Text = getModel();
                    (window as MainWindow).carClassInfo.Text = getClass();
                    (window as MainWindow).moreCarInfo.Text = getSpecification();
                    (window as MainWindow).fillListBox();
                    (window as MainWindow).MainTabControl.SelectedItem = (window as MainWindow).InfoTab;
                }
            }
        }

        public void BookCarButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window.GetType() == typeof(MainWindow))
                {
                    if ((window as MainWindow).MainTabControl.Items.Contains((window as MainWindow).inputCarClientTabItem))
                    {
                        if ((window as MainWindow).AddCarCard.Visibility == Visibility.Visible || (window as MainWindow).AddClientCard.Visibility == Visibility.Visible)
                        {
                            if (MessageBox.Show("Вы не завершили предыдущий ввод. Хотите продолжить?", "Предупреждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                            {
                                (window as MainWindow).MainTabControl.SelectedItem = (window as MainWindow).inputCarClientTabItem;
                                (window as MainWindow).BookCarCard.Visibility = Visibility.Visible;
                                (window as MainWindow).AddCarCard.Visibility = Visibility.Collapsed;
                                (window as MainWindow).AddClientCard.Visibility = Visibility.Collapsed;
                                (window as MainWindow).BookingCarName.Text = (window as MainWindow).BookingCarName.Text + " " + getModel();
                                (window as MainWindow).finalPrice.Text = this.getPrice().ToString();
                                (window as MainWindow).MainTabControl.SelectedItem = (window as MainWindow).inputCarClientTabItem;
                                (window as MainWindow).carDiscount = getDiscount();
                            }
                            else
                            {
                                return;
                            }
                        }
                        return;
                    }
                    (window as MainWindow).MainTabControl.SelectedItem = (window as MainWindow).inputCarClientTabItem;
                    (window as MainWindow).MainTabControl.Items.Add((window as MainWindow).inputCarClientTabItem);
                    (window as MainWindow).BookCarCard.Visibility = Visibility.Visible;
                    (window as MainWindow).AddCarCard.Visibility = Visibility.Collapsed;
                    (window as MainWindow).AddClientCard.Visibility = Visibility.Collapsed;
                }
            }
        }

        private int days;

        public void listBox_selectedItemChange(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ListBox).SelectedItem != null)
            {
                string str = (((sender as ListBox).SelectedItem as ListBoxItem).Content as TextBlock).Text;
                char c = str[0];
                days = (int)Char.GetNumericValue(str[0]);
                foreach (var item in (((sender as ListBox).Parent as Grid).Parent as Grid).Children)
                {
                    if (item is Grid)
                    {
                        foreach (var jtem in (item as Grid).Children)
                        {
                            if (jtem is TextBlock)
                            {
                                if ((jtem as TextBlock).Text != "руб.")
                                {
                                    (jtem as TextBlock).Text = calculateDiscount(days, this.discount, this.price).ToString();
                                }
                            }
                        }
                    }
                }
            }
        }

        private int calculateDiscount(int days, int discount, int price)
        {
            if (days == 7)
            {
                return Convert.ToInt32(price - price * (discount / 100.0));
            }

            if (days == 3)
            {
                return Convert.ToInt32((price - price * ((discount + discount) / 100.0)));
            }

            if (days == 1)
            {
                return price;
            }
            return 0;
        }

        public int getCalculatedPrice()
        {
            return this.calculateDiscount(days, this.discount, this.price);
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
