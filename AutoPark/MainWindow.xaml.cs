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
using System.Data.SqlClient;
using System.Data;
using System.Windows.Markup;
using MaterialDesignThemes;
using System.Xml;
using System.IO;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.Drawing;
using MaterialDesignColors;

namespace AutoPark
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public int carDiscount;
        public int carPriceForBook;
        List<MaterialDesignThemes.Wpf.Card> cardsList = new List<MaterialDesignThemes.Wpf.Card>();
        List carList = new List();
        Style headerHidden = new Style();
        SqlConnection connection = new SqlConnection(@"Server=EDIMAX;Database=AutoPark;Trusted_Connection=Yes;");
        BitmapImage noImgBtm = new BitmapImage(new Uri(@"C:\Users\Даниил\Desktop\noCar.png"));
        System.Windows.Controls.Image imgForNewCard = new System.Windows.Controls.Image();
        byte[] byteArray;
        private SqlDataAdapter CarsDa = new SqlDataAdapter();
        private DataSet changes = new DataSet();
        private DataSet CarsDs = new DataSet();
        private DataSet ClientDs = new DataSet();
        private SqlDataAdapter ClientDa = new SqlDataAdapter();
        private DataSet BookingDs = new DataSet();
        private SqlDataAdapter BookingDa = new SqlDataAdapter();
        public bool isAdmin = false;

        public MainWindow()
        {
            InitializeComponent();
            this.Title = "AutoRent";
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        private void connectToDataBase()
        {
            try
            {
                connection.Open();
            }
            catch
            {
                MessageBox.Show("Error with connecting to data base");
            }
        }

        private void window_loaded(object sender, RoutedEventArgs e)
        {
            LoginWindow lw = new LoginWindow();
            lw.Show();
            loadCar();
            fillInFields();
            fillUpDataGrids();
            ChangeImgButton.IsEnabled = false;
            connection.Close();
            hideControls();
        }

        private void hideControls()
        {
            AddCarCard.Visibility = Visibility.Collapsed;
            BookCarCard.Visibility = Visibility.Collapsed;
            AddClientCard.Visibility = Visibility.Collapsed;
            AddNewThingPopUp.Visibility = Visibility.Collapsed;
            CarTableCard.Visibility = Visibility.Collapsed;
            BookingTableCard.Visibility = Visibility.Collapsed;
            ClientTableCard.Visibility = Visibility.Collapsed;
        }

        private void fillInFields()
        {
            connectToDataBase();
            string getBrands = "SELECT * from Brand";
            string getAutoparks = "Select address from Autopark";
            string getTarif = "SELECT * FROM Tariffs";
            SqlCommand commandBrands = new SqlCommand(getBrands, connection);
            using (SqlDataReader reader = commandBrands.ExecuteReader())
            {
                while (reader.Read())
                {
                    searchAutoParkByBrandComboBox.Items.Add(reader["brand"]);
                    CarBrandComboBox.Items.Add(reader["brand"]);
                }
            }

            SqlCommand commandAutoParks = new SqlCommand(getAutoparks, connection);
            using (SqlDataReader reader = commandAutoParks.ExecuteReader())
            {
                while (reader.Read())
                {
                    CarAutoParkComboBox.Items.Add(reader["address"]);
                    AutoparkForReturningComboBox.Items.Add(reader["address"]);
                }
            }

            SqlCommand commandTariffs = new SqlCommand(getTarif, connection);
            using (SqlDataReader reader = commandTariffs.ExecuteReader())
            {
                while (reader.Read())
                {
                    TarifCombobox.Items.Add(reader["type"]);
                }
            }
            connection.Close();

            CarTransmissionComboBox.Items.Add("4MT");
            CarTransmissionComboBox.Items.Add("5MT");
            CarTransmissionComboBox.Items.Add("6MT");
            CarTransmissionComboBox.Items.Add("5AT");
            CarTransmissionComboBox.Items.Add("6AT");
            CarTransmissionComboBox.Items.Add("7AT");
            CarTransmissionComboBox.Items.Add("8AT");
        }


        private void Window_Activated(object sender, EventArgs e)
        {
            
        }

        private void MenuToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            //string insertCar = "SELECT Autopark.longitude, Autopark.latitude, Autopark.autopark_id FROM Brand INNER JOIN Cars ON Brand.brand = Cars.brand INNER JOIN Autopark ON Cars.autopark_id = Autopark.autopark_id WHERE(Brand.brand = @brand)";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //            string insertCar = "INSERT into Cars VALUES(@staffName, @userID, @idDepartment)";

            //using (SqlCommand querySaveStaff = new SqlCommand(insertCar))
            //{
            //    //querySaveStaff.Connection = connection;
            //    //querySaveStaff.Parameters.Add("@staffName", SqlDbType.VarChar, 30).Value = name;
            //    //connection.Open();
            //}

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //SqlConnection connection = new SqlConnection(@"Server=EDIMAX;Database=AutoPark;Trusted_Connection=Yes;");
            connection.Open();

            string searchCar = "SELECT Autopark.longitude, Autopark.latitude, Autopark.autopark_id FROM Brand INNER JOIN Cars ON Brand.brand = Cars.brand INNER JOIN Autopark ON Cars.autopark_id = Autopark.autopark_id WHERE (Brand.brand = @brand)";


            StringBuilder queryAddress = new StringBuilder();
            queryAddress.Append("http://maps.google.com/maps?q=");

            SqlCommand command = new SqlCommand(searchCar, connection);
            command.Parameters.AddWithValue("@brand", searchAutoParkByBrandComboBox.Text);
            using (SqlDataReader reader = command.ExecuteReader())
            {
                queryAddress.Append(searchAutoParkByBrandComboBox.Text);
            }
            connection.Close();
            webBrowser.Navigate(queryAddress.ToString());
        }

        private void NewCar_Click(object sender, RoutedEventArgs e)
        {
            AddClientCard.Visibility = Visibility.Collapsed;
            AddCarCard.Visibility = Visibility.Visible;
            BookCarCard.Visibility = Visibility.Collapsed;
            MainTabControl.SelectedItem = inputCarClientTabItem;   
        }

        private void reloadCards()
        {
            foreach (var card in cardsList)
            {
                wrapPanelCars.Children.Remove(card);
            }
            loadCar();
        }

        private void AddCar_Click(object sender, RoutedEventArgs e)
        { 
            StreamReader mysr = new StreamReader(@"C:\Users\Даниил\Documents\Visual Studio 2015\Projects\AutoPark\AutoPark\ResourcesStyles.xaml");
            ResourceDictionary rd = XamlReader.Load(mysr.BaseStream) as ResourceDictionary;
            Style style = rd["style"] as Style;
            System.Windows.Controls.Image noImg = new System.Windows.Controls.Image();
            StringBuilder strbld = new StringBuilder();
            Button btn = new Button();

            strbld.Append(@"<materialDesign:Card
                            xmlns:materialDesign = 'http://materialdesigninxaml.net/winfx/xaml/themes'
                            xmlns ='http://schemas.microsoft.com/winfx/2006/xaml/presentation'
                            xmlns:x ='http://schemas.microsoft.com/winfx/2006/xaml'>");
            strbld.Append(@"<Grid><Grid.RowDefinitions><RowDefinition Height='140'/>
                            <RowDefinition Height = 'auto'/><RowDefinition Height = '*'/><RowDefinition Height = '*'/><RowDefinition Height = '*'/></Grid.RowDefinitions>
                            <Image Source='Resources/noCar.png' Height='140' Width='220' Stretch='UniformToFill'/>
                            <Button Grid.Row='0' x:Name='btn' Style='{ StaticResource MaterialDesignFloatingActionMiniAccentButton}' HorizontalAlignment = 'Right' VerticalAlignment = 'Bottom' Margin = '0 0 20 -20'><materialDesign:PackIcon Kind = 'Car'/></Button>
                            <StackPanel Grid.Row='1' Margin='8 30 8 0'><TextBlock Style = '{StaticResource MaterialDesignBody2TextBlock}' x:Name = 'textblock' VerticalAlignment = 'Center'/></StackPanel>                            
                            <Separator Grid.Row='2' Style='{ StaticResource MaterialDesignDarkSeparator}' Margin='8 0 8 0'/>
                            <Grid Grid.Row = '3'><Grid.ColumnDefinitions><ColumnDefinition Width = 'auto'/><ColumnDefinition/></Grid.ColumnDefinitions>
                            <materialDesign:PackIcon Kind = 'AvTimer' VerticalAlignment = 'Top' Margin = '8 5 4 4'/>
                            <ListBox Grid.Column = '1' Style = '{ StaticResource MaterialDesignToolToggleFlatListBox}' VerticalAlignment = 'Top' SelectedIndex = '0' Margin = '4 0 8 4' >
                            <ListBox.Resources><Style TargetType = '{x:Type ListBoxItem}' BasedOn = '{StaticResource MaterialDesignToolToggleListBoxItem}'><Setter Property = 'Padding' Value = '4 6 4 6'/></Style></ListBox.Resources>
                            <ListBoxItem><TextBlock Text = '1 день'/></ListBoxItem>
                            <ListBoxItem><TextBlock Text = '7 дней'/></ListBoxItem>
                            <ListBoxItem><TextBlock Text = '30 дней'/></ListBoxItem></ListBox></Grid>
                            <Grid Grid.Row='5'><Grid.ColumnDefinitions><ColumnDefinition Width = 'auto'/><ColumnDefinition Width = 'auto'/><ColumnDefinition Width = 'auto'/></Grid.ColumnDefinitions>
                            <Button Grid.Column='0' Style='{ StaticResource MaterialDesignFlatButton}' HorizontalAlignment='Left' VerticalAlignment='Top' Margin='8 0 4 8'>RESERVE</Button>
                            <TextBlock Grid.Column = '1' Style = '{StaticResource MaterialDesignBody2TextBlock}' VerticalAlignment = 'Top' HorizontalAlignment = 'Left' Margin = '25,5,4,8' FontSize = '16'/>
                            <TextBlock Grid.Column = '2' Style = '{StaticResource MaterialDesignBody2TextBlock}' VerticalAlignment = 'Top' HorizontalAlignment = 'Left' Margin = '2,4,4,8' FontSize = '16' Text = 'руб.'/></Grid></Grid></materialDesign:Card>");


            MaterialDesignThemes.Wpf.Card crd = (MaterialDesignThemes.Wpf.Card)XamlReader.Parse(strbld.ToString());
            Grid gridFirst = (crd.Content as Grid);
            Grid gridSecond;
            StackPanel panel = new StackPanel();
            bool firstGrFound = false;
            Button bookBtn = new Button();
            TextBlock priceTextBlock = new TextBlock();
            TextBlock nameTextBlock = new TextBlock();
            System.Windows.Controls.Image carImg = new System.Windows.Controls.Image();
            BitmapImage carImgBit = loadPicture(byteArray);
          
            Car car = new Car(Convert.ToInt32(CarPriceTextBox.Text), CarBrandComboBox.Text, CarSpecTextBox.Text, CarModelTextBox.Text + " " + CarTransmissionComboBox.Text, carImgBit, CarNumberTextBox.Text, false, CarClassTextBox.Text, Convert.ToInt32(CarDiscountTextBox.Text));

            wrapPanelCars.Children.Add(crd);
            crd.Style = style;
            cardsList.Add(crd);

            foreach (var item in gridFirst.Children)
            {
                if (item is Button)
                {
                    (item as Button).Click += car.CarButton_Click;
                }

                if (item is System.Windows.Controls.Image)
                {
                    if (byteArray == null)
                    {
                        (item as System.Windows.Controls.Image).Source = noImgBtm;

                    }
                    else
                    {
                        (item as System.Windows.Controls.Image).Source = carImgBit;
                    }
                }

                if (item is StackPanel)
                {
                    panel = item as StackPanel;
                    nameTextBlock = panel.Children[0] as TextBlock;
                    nameTextBlock.Text = CarModelTextBox.Text + " " + CarTransmissionComboBox.Text;
                }

                if (item is Grid && !firstGrFound)
                {
                    gridSecond = (item as Grid);
                    foreach (var j in gridSecond.Children)
                    {

                        if (j is TextBlock)
                        {
                            priceTextBlock = (j as TextBlock);
                            if (priceTextBlock.Text == "")
                            {
                                priceTextBlock.Text = CarPriceTextBox.Text;
                            }
                        }

                        if (j is Button)
                        {
                            (j as Button).Click += car.BookCarButton_Click;
                        }
                    }
                }
            }

            connectToDataBase();
            int autopark_id = 0;

            string retrieveAutoparkKey = "SELECT autopark_id FROM Autopark WHERE address = @address";
            SqlCommand commandAutoParkkey = new SqlCommand(retrieveAutoparkKey, connection);
            commandAutoParkkey.Parameters.AddWithValue("@address", CarAutoParkComboBox.Text);
            using (SqlDataReader reader = commandAutoParkkey.ExecuteReader())
            {
                while (reader.Read())
                {
                    autopark_id = Convert.ToInt32(reader["autopark_id"]);
                }
            }

            string addCar = "INSERT INTO Cars VALUES(@car_number,@brand,@spec,@class,@autopark,@model,@reserved,@discount,@pic,@price)";

            SqlCommand commandAddCar = new SqlCommand(addCar, connection);

            try {

                if (byteArray == null)
                {
                    commandAddCar.Parameters.AddWithValue("@price", Convert.ToInt32(CarPriceTextBox.Text));
                    commandAddCar.Parameters.AddWithValue("@brand", CarBrandComboBox.Text);
                    commandAddCar.Parameters.AddWithValue("@car_number", CarNumberTextBox.Text);
                    commandAddCar.Parameters.AddWithValue("@spec", CarSpecTextBox.Text);
                    commandAddCar.Parameters.AddWithValue("@class", CarClassTextBox.Text);
                    commandAddCar.Parameters.AddWithValue("@autopark", autopark_id.ToString());
                    commandAddCar.Parameters.AddWithValue("@model", CarModelTextBox.Text +" "+ CarTransmissionComboBox.Text);
                    commandAddCar.Parameters.AddWithValue("@reserved", "1");
                    commandAddCar.Parameters.AddWithValue("@discount", CarDiscountTextBox.Text);
                    commandAddCar.Parameters.AddWithValue("@pic", System.Data.SqlTypes.SqlBinary.Null);
                    commandAddCar.ExecuteNonQuery();
                } else
                {
                    commandAddCar.Parameters.AddWithValue("@price", Convert.ToInt32(CarPriceTextBox.Text));
                    commandAddCar.Parameters.AddWithValue("@brand", CarBrandComboBox.Text);
                    commandAddCar.Parameters.AddWithValue("@car_number", CarNumberTextBox.Text);
                    commandAddCar.Parameters.AddWithValue("@spec", CarSpecTextBox.Text);
                    commandAddCar.Parameters.AddWithValue("@class", CarClassTextBox.Text);
                    commandAddCar.Parameters.AddWithValue("@autopark", autopark_id.ToString());
                    commandAddCar.Parameters.AddWithValue("@model", CarModelTextBox.Text + " " + CarTransmissionComboBox.Text);
                    commandAddCar.Parameters.AddWithValue("@reserved", "1");
                    commandAddCar.Parameters.AddWithValue("@discount", CarDiscountTextBox.Text);
                    commandAddCar.Parameters.AddWithValue("@pic", byteArray);
                    commandAddCar.ExecuteNonQuery();
                }
            } catch(Exception ex)
            {
                MessageBox.Show("Havent managed to execute query");
                throw ex;
            } finally
            {
                MainTabControl.SelectedItem = CarTab;
                CarAutoParkComboBox.Text = "";
                CarBrandComboBox.Text = "";
                CarClassTextBox.Text = "";
                CarDiscountTextBox.Text = "";
                CarModelTextBox.Text = "";
                CarPriceTextBox.Text = "";
                CarSpecTextBox.Text = "";
                CarNumberTextBox.Text = "";
                var messageQueue = CarAddedSnackBar.MessageQueue;
                var message = "Машина успешно добавлена";

                messageQueue.Enqueue(message);
            }
            
            connection.Close();
            fillUpDataGrids();
            AddCarCard.Visibility = Visibility.Collapsed;
        }

        private byte[] ImageToByteArray(System.Windows.Controls.Image img)
        {
            byte[] arr;

            FileStream fs = new FileStream(img.Source.ToString(), FileMode.Open);
            arr = new byte[fs.Length];
            fs.Read(arr, 0, Convert.ToInt32(fs.Length));

            return arr;
        }


        private void AddImgButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Title = "Choose Image";
            dlg.Filter = "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg";
            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                FileStream fs = new FileStream(dlg.FileName,FileMode.Open);
                byteArray = new byte[fs.Length];
                fs.Read(byteArray, 0, Convert.ToInt32(fs.Length));

                BitmapImage b = new BitmapImage();
                b.StreamSource = new MemoryStream(byteArray);
                imgForNewCard.Source = b;
                ChangeImgButton.IsEnabled = true;
            }
        }

        private void ChangeImgButton_Click(object sender, RoutedEventArgs e)
        {

        }

        //private byte[] SavePicture(BitmapImage bitImg)
        //{
        //    System.Windows.Controls.Image img = new System.Windows.Controls.Image();
        //    img.Source = bitImg;
        //    System.Drawing.Image imgToSave = null;
        //    if (img != null)
        //    {
        //        MemoryStream msToConvert = new MemoryStream();
        //        System.Windows.Media.Imaging.BmpBitmapEncoder bbe = new BmpBitmapEncoder();
        //        bbe.Frames.Add(BitmapFrame.Create(new Uri(img.Source.ToString(), UriKind.RelativeOrAbsolute)));
        //        bbe.Save(msToConvert);
        //        imgToSave = System.Drawing.Image.FromStream(msToConvert);
        //    }
        //    byte[] arr = null;

        //    if (imgChosen)
        //    {
        //            MemoryStream ms = new MemoryStream();
        //            imgToSave.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
        //            /*   ms.Position= 0;
        //               BitmapImage ix = new BitmapImage();
        //               ix.BeginInit();
        //               ix.CacheOption = BitmapCacheOption.OnLoad;
        //               ix.StreamSource = ms;
        //               ix.EndInit();*/
        //            //LadaImg.Source = ix;

        //            //////////////
        //            //imgToSave.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
        //            arr = ms.GetBuffer();
        //            ms.Close();
        //    }
        //     return arr;
        // }

        private BitmapImage loadPicture(byte[] arr)
        {
            BitmapImage ix = new BitmapImage();

            if (arr == null)
            {
                return ix = null;    
            }

            MemoryStream ms = new MemoryStream(arr);

            ix.BeginInit();
            ix.CacheOption = BitmapCacheOption.OnLoad;
            ix.StreamSource = ms;
            ix.EndInit();

            return ix;
        }

        private void loadCar()
        {
            int price = 0;
            string brand = "";
            string specification = "";
            string model = "";
            BitmapImage image = null;
            string number = "";
            bool reserved = false;
            string clas = "";
            int discount = 0;

            connectToDataBase();
            string loadCars = "SELECT * FROM Cars";

            SqlCommand commandBrands = new SqlCommand(loadCars, connection);
            using (SqlDataReader reader = commandBrands.ExecuteReader())
            {
                while (reader.Read())
                {
                    price = Convert.ToInt32(reader["price"]);
                    brand = reader["brand"].ToString();
                    specification = reader["specification"].ToString();
                    model = reader["model"].ToString();

                    if (reader["reserved"] != null)
                    {
                        reserved = true;
                    }
                    else {
                        reserved = false;
                    }
                    number = reader["car_number"].ToString();
                    clas = reader["class"].ToString();
                    discount = Convert.ToInt32(reader["discount"]);
                    image = loadPicture(reader["image"] as byte[]);

                    StreamReader mysr = new StreamReader(@"C:\Users\Даниил\Documents\Visual Studio 2015\Projects\AutoPark\AutoPark\ResourcesStyles.xaml");
                    ResourceDictionary rd = XamlReader.Load(mysr.BaseStream) as ResourceDictionary;
                    Style style = rd["style"] as Style;

                    StringBuilder strbld = new StringBuilder();
                    Button btn = new Button();

                    strbld.Append(@"<materialDesign:Card
                            xmlns:materialDesign = 'http://materialdesigninxaml.net/winfx/xaml/themes'
                            xmlns ='http://schemas.microsoft.com/winfx/2006/xaml/presentation'
                            xmlns:x ='http://schemas.microsoft.com/winfx/2006/xaml'>");
                    strbld.Append(@"<Grid><Grid.RowDefinitions><RowDefinition Height='140'/>
                            <RowDefinition Height = 'auto'/><RowDefinition Height = '*'/><RowDefinition Height = '*'/><RowDefinition Height = '*'/></Grid.RowDefinitions>
                            <Image Source='Resources/noCar.png' Height='140' Width='220' Stretch='UniformToFill'/>
                            <Button Grid.Row='0' x:Name='btn' Style='{ StaticResource MaterialDesignFloatingActionMiniAccentButton}' HorizontalAlignment = 'Right' VerticalAlignment = 'Bottom' Margin = '0 0 20 -20'><materialDesign:PackIcon Kind = 'Car'/></Button>
                            <StackPanel Grid.Row='1' Margin='8 30 8 0'><TextBlock Style = '{StaticResource MaterialDesignBody2TextBlock}' x:Name = 'textblock' VerticalAlignment = 'Center'/></StackPanel>                            
                            <Separator Grid.Row='2' Style='{ StaticResource MaterialDesignDarkSeparator}' Margin='8 0 8 0'/>
                            <Grid Grid.Row = '3'><Grid.ColumnDefinitions><ColumnDefinition Width = 'auto'/><ColumnDefinition/></Grid.ColumnDefinitions>
                            <materialDesign:PackIcon Kind = 'AvTimer' VerticalAlignment = 'Top' Margin = '8 5 4 4'/>
                            <ListBox Grid.Column = '1' Style = '{ StaticResource MaterialDesignToolToggleFlatListBox}' VerticalAlignment = 'Top' SelectedIndex = '0' Margin = '4 0 8 4' >
                            <ListBox.Resources><Style TargetType = '{x:Type ListBoxItem}' BasedOn = '{StaticResource MaterialDesignToolToggleListBoxItem}'><Setter Property = 'Padding' Value = '4 6 4 6'/></Style></ListBox.Resources>
                            <ListBoxItem><TextBlock Text = '1 день'/></ListBoxItem>
                            <ListBoxItem><TextBlock Text = '7 дней'/></ListBoxItem>
                            <ListBoxItem><TextBlock Text = '30 дней'/></ListBoxItem></ListBox></Grid>
                            <Grid Grid.Row='5'><Grid.ColumnDefinitions><ColumnDefinition Width = 'auto'/><ColumnDefinition Width = 'auto'/><ColumnDefinition Width = 'auto'/></Grid.ColumnDefinitions>
                            <Button Grid.Column='0' Style='{ StaticResource MaterialDesignFlatButton}' HorizontalAlignment='Left' VerticalAlignment='Top' Margin='8 0 4 8'>RESERVE</Button>
                            <TextBlock Grid.Column = '1' Style = '{StaticResource MaterialDesignBody2TextBlock}' VerticalAlignment = 'Top' HorizontalAlignment = 'Left' Margin = '25,5,4,8' FontSize = '16'/>
                            <TextBlock Grid.Column = '2' Style = '{StaticResource MaterialDesignBody2TextBlock}' VerticalAlignment = 'Top' HorizontalAlignment = 'Left' Margin = '2,4,4,8' FontSize = '16' Text = 'руб.'/></Grid></Grid></materialDesign:Card>");


                    BitmapImage img = new BitmapImage(new Uri(@"C:\Users\Даниил\Desktop\noCar.png"));
                    MaterialDesignThemes.Wpf.Card crd = (MaterialDesignThemes.Wpf.Card)XamlReader.Parse(strbld.ToString());
                    Grid gridFirst = (crd.Content as Grid);
                    Grid gridSecond;
                    StackPanel panel = new StackPanel();
                    bool firstGrFound = false;

                    TextBlock priceTextBlock = new TextBlock();
                    TextBlock nameTextBlock = new TextBlock();
                    System.Windows.Controls.Image carImg = new System.Windows.Controls.Image();
                    Car car = new Car(price, brand, specification, model, image, number, reserved, clas, discount);

                    wrapPanelCars.Children.Add(crd);
                    crd.Style = style;
                    cardsList.Add(crd);

                    foreach (var item in gridFirst.Children)
                    {
                        if (item is Button)
                        {
                            (item as Button).Click += car.CarButton_Click;
                        }

                        if (item is System.Windows.Controls.Image)
                        {
                            carImg = (item as System.Windows.Controls.Image);
                            if (image == null)
                            {
                                carImg.Source = img;
                            }
                            else
                            {
                                carImg.Source = image;
                            }
                        }

                        if (item is StackPanel)
                        {
                            panel = item as StackPanel;
                            nameTextBlock = panel.Children[0] as TextBlock;
                            nameTextBlock.Text = model;
                        }

                        if (item is Grid && !firstGrFound)
                        {
                            gridSecond = (item as Grid);
                            foreach (var j in gridSecond.Children)
                            {

                                if (j is TextBlock)
                                {
                                    priceTextBlock = (j as TextBlock);
                                    if (priceTextBlock.Text == "")
                                    {
                                        priceTextBlock.Text = price.ToString();
                                        carPriceForBook = price;
                                    }
                                }
                                if (j is Button)
                                {
                                    (j as Button).Click += car.BookCarButton_Click;
                                }
                            }
                        }
                    }
                }
            }
            connection.Close();
        }

        private void fillUpDataGrids()
        {
            connectToDataBase();
            string select = "SELECT * FROM Cars";

            SqlCommand cmdSel = new SqlCommand(select, connection);
            CarsDa.SelectCommand = cmdSel;           
            CarsDa.Fill(CarsDs, "Cars");
            CarsDs.AcceptChanges();
            CarsDataGrid.DataContext = CarsDs.DefaultViewManager;

            string selectClinets = "SELECT * FROM Clients";
            SqlCommand ClientCmd = new SqlCommand(selectClinets, connection);
            ClientDa.SelectCommand = new SqlCommand(selectClinets, connection);
            ClientDa.Fill(ClientDs, "Clients");
            ClientsDataGrid.DataContext = ClientDs.DefaultViewManager;
        
            string selectBooking = "SELECT * FROM Booking";
            SqlCommand BookingCmd = new SqlCommand(selectBooking, connection);
            BookingDa.SelectCommand = new SqlCommand(selectBooking, connection);
            BookingDa.Fill(BookingDs, "Booking");
            BookingDataGrid.DataContext = BookingDs.DefaultViewManager;

            connection.Close();
        }

        private void BookCar()
        {
            connectToDataBase();
            int firstPrice = Convert.ToInt32(finalPrice.Text);
            int tariffPrice = 0;
            int tariff_id = 0;
            // ВЫтаскиваю ключ тарифа в который вернут машину
            string retrieveTariffKey = "SELECT tariff_id,price FROM Tariffs WHERE type = @type";
            SqlCommand commandTariffkey = new SqlCommand(retrieveTariffKey, connection);
            commandTariffkey.Parameters.AddWithValue("@type", TarifCombobox.Text);
            using (SqlDataReader reader = commandTariffkey.ExecuteReader())
            {
                while (reader.Read())
                {
                    tariff_id = Convert.ToInt32(reader["tariff_id"]);
                    tariffPrice = Convert.ToInt32(reader["price"]);
                }
            }

            string bookCar = "INSERT INTO Booking VALUES(@client,@tariff,@date_start,@date_end,@price)";
            SqlCommand commandBookCar = new SqlCommand(bookCar, connection);

            try
            {
                commandBookCar.Parameters.AddWithValue("@client", Convert.ToInt32(BookingClient_idTextBlock.Text));
                commandBookCar.Parameters.AddWithValue("@tariff", tariff_id.ToString());
                commandBookCar.Parameters.AddWithValue("@date_start", DateStart.Text);
                commandBookCar.Parameters.AddWithValue("@date_end", DateEnd.Text);
                commandBookCar.Parameters.AddWithValue("@price", Convert.ToInt32(finalPrice.Text));
                commandBookCar.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Havent manage to execute query");
                throw ex;
            }
            finally
            {
                MainTabControl.SelectedItem = CarTab;
                var messageQueue = CarBookedSnackBar.MessageQueue;
                var message = "Машина успешно забронирована";
                messageQueue.Enqueue(message);
            }
            connection.Close();
            fillUpDataGrids();
            BookCarCard.Visibility = Visibility.Collapsed;
        }

        private void bookCarButton_Click(object sender, RoutedEventArgs e)
        {
            BookCar();
        }

        private void StartDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            int firstPrice = Convert.ToInt32(finalPrice.Text);
            int TotalPrice = 0;
            int countOfDays;
            int tariffPrice = 0;

            if (!DateEnd.SelectedDate.HasValue)
            {
                return;
            }
            else {

                DateTime start = DateStart.SelectedDate.Value.Date;
                DateTime finish = DateEnd.SelectedDate.Value.Date;
                TimeSpan difference = finish.Subtract(start);
                countOfDays = Convert.ToInt32(difference.TotalDays);
                firstPrice = (Convert.ToInt32(finalPrice.Text) + tariffPrice) * countOfDays;

                if (countOfDays > 1 && countOfDays < 7)
                {
                    TotalPrice = firstPrice - firstPrice * (carDiscount / 100);
                }

                if (countOfDays > 7 && countOfDays < 30)
                {
                    TotalPrice = firstPrice - firstPrice * ((carDiscount + carDiscount) / 100);
                }
                finalPrice.Text = TotalPrice.ToString();
            }
        }

        private void EndDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            int firstPrice = Convert.ToInt32(finalPrice.Text);
            int TotalPrice = 0;
            int countOfDays;
            int tariffPrice = 0;

            if (!DateStart.SelectedDate.HasValue)
            {
                return;
            }
            else {

                DateTime start = DateStart.SelectedDate.Value.Date;
                DateTime finish = DateEnd.SelectedDate.Value.Date;
                TimeSpan difference = finish.Subtract(start);
                countOfDays = Convert.ToInt32(difference.TotalDays);
                firstPrice = (Convert.ToInt32(finalPrice.Text) + tariffPrice) * countOfDays;

                if (countOfDays > 1 && countOfDays < 7)
                {
                    TotalPrice = firstPrice - firstPrice * (carDiscount / 100);
                }

                if (countOfDays > 7 && countOfDays < 30)
                {
                    TotalPrice = firstPrice - firstPrice * ((carDiscount + carDiscount) / 100);
                }
                finalPrice.Text = TotalPrice.ToString();
            }
        }

        private void CheckForNumberInput_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (!Char.IsDigit((char)KeyInterop.VirtualKeyFromKey(e.Key)) & e.Key != Key.Back | e.Key == Key.Space)
            {
                e.Handled = true;
                MessageBox.Show("I only accept numbers, sorry. :(", "This textbox says...");
                //ClientPhoneTextBox
            }
        }

        private void DetectChange_scrollChaned(object sender, ScrollChangedEventArgs e)
        {
            var verticalOffset = ScrollCarsScrollViewer.VerticalOffset;
            var maxVerticalOffset = ScrollCarsScrollViewer.ScrollableHeight; 
            if (maxVerticalOffset < 0 ||
                verticalOffset == maxVerticalOffset)
            {
                // Scrolled to bottom
                //rect.Fill = new SolidColorBrush(Colors.Red);
               // AddNewCarButton.Margin = new Thickness(4, 4, 4, maxVerticalOffset - System.Windows.SystemParameters.PrimaryScreenHeight);
            }
            else
            {
                // Not scrolled to bottom
             //   rect.Fill = new SolidColorBrush(Colors.Yellow);
            }
        }

        private void AddClientButton_Click(object sender, RoutedEventArgs e)
        {
            connectToDataBase();

            string addClient = "INSERT INTO Clients VALUES(@name,@surname,@patronymic,@phone)";
            SqlCommand commandAddClient = new SqlCommand(addClient, connection);

            try
            {
                if (ClientPatrTextBox.Text == "" && ClientPhoneTextBox.Text != "")
                {
                    commandAddClient.Parameters.AddWithValue("@name", ClientNameTextBox.Text);
                    commandAddClient.Parameters.AddWithValue("@surname", ClientSurnameTextBox.Text);
                    commandAddClient.Parameters.AddWithValue("@patronymic", System.Data.SqlTypes.SqlString.Null);
                    commandAddClient.Parameters.AddWithValue("@phone", ClientPhoneTextBox.Text);
                    commandAddClient.ExecuteNonQuery();
                }

                if (ClientPatrTextBox.Text != "" && ClientPhoneTextBox.Text == "")
                {

                    commandAddClient.Parameters.AddWithValue("@name", ClientNameTextBox.Text);
                    commandAddClient.Parameters.AddWithValue("@surname", ClientSurnameTextBox.Text);
                    commandAddClient.Parameters.AddWithValue("@patronymic", ClientPatrTextBox.Text);
                    commandAddClient.Parameters.AddWithValue("@phone", System.Data.SqlTypes.SqlString.Null);
                    commandAddClient.ExecuteNonQuery();
                }

                if (ClientPatrTextBox.Text == "" && ClientPhoneTextBox.Text == "")
                {
                    commandAddClient.Parameters.AddWithValue("@name", ClientNameTextBox.Text);
                    commandAddClient.Parameters.AddWithValue("@surname", ClientSurnameTextBox.Text);
                    commandAddClient.Parameters.AddWithValue("@patronymic", System.Data.SqlTypes.SqlString.Null);
                    commandAddClient.Parameters.AddWithValue("@phone", System.Data.SqlTypes.SqlString.Null);
                    commandAddClient.ExecuteNonQuery();
                }
                if (ClientPatrTextBox.Text != "" && ClientPhoneTextBox.Text != "")
                {
                    commandAddClient.Parameters.AddWithValue("@name", ClientNameTextBox.Text);
                    commandAddClient.Parameters.AddWithValue("@surname", ClientSurnameTextBox.Text);
                    commandAddClient.Parameters.AddWithValue("@patronymic", ClientPatrTextBox.Text);
                    commandAddClient.Parameters.AddWithValue("@phone", ClientPhoneTextBox.Text);
                    commandAddClient.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex);
                throw ex;
            }
            finally
            {
                MainTabControl.SelectedItem = CarTab;
                var messageQueue = ClientAddedSnackBar.MessageQueue;
                var message = "Клиент успешно добавлен";

                messageQueue.Enqueue(message);
            }
            connection.Close();
            fillUpDataGrids();
            AddCarCard.Visibility = Visibility.Collapsed;
        }

        private void NewClientButton_Click(object sender, RoutedEventArgs e)
        {
            BookCarCard.Visibility = Visibility.Collapsed;
            AddCarCard.Visibility = Visibility.Collapsed;
            AddClientCard.Visibility = Visibility.Visible;
            MainTabControl.SelectedItem = inputCarClientTabItem;
        }

        private void DeleteCarButton_Click(object sender, RoutedEventArgs e)
        {
            SqlCommand cmd = new SqlCommand();
            
            if (CarsDataGrid.SelectedItem != null)
            {
              //  CarsDataGrid.Items.Remove(CarsDataGrid.ItemsSource);
            }
                //  DataGridViewRow dr = dataGridView1.Rows[i];
                // if (dr.Selected == true)
                //{
                //CarsDataGrid.Items.Remove(item);
                ////   try
                //   {
                //       connectToDataBase();
                //       cmd.CommandText = "Delete from Car where car_number" + i + "";
                //       cmd.ExecuteNonQuery();
                //       connection.Close();
                //   }
                //   catch (Exception ex)
                //   {
                //       MessageBox.Show(ex.ToString());
                //   }
        //}
        }

        private void ChangeCarButton_Click(object sender, RoutedEventArgs e)
        {
            DataSet changes = CarsDs.GetChanges();
            if (changes != null)
            {
                SqlCommandBuilder builder = new SqlCommandBuilder(CarsDa);
                CarsDa.UpdateCommand = builder.GetUpdateCommand();

                try
                {
                    CarsDa.Update(CarsDs, "Cars");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    var messageQueue = CarChangedSnackBar.MessageQueue;
                    var message = "Информация о машине успешно изменена";
                    messageQueue.Enqueue(message);
                    reloadCards();
                }
            }
        }

        private void DeleteClientButton_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void ChangeClientButton_Click(object sender, RoutedEventArgs e)
        {
            DataSet changes = ClientDs.GetChanges();
            if (changes != null)
            {
                SqlCommandBuilder builder = new SqlCommandBuilder(ClientDa);
                ClientDa.UpdateCommand = builder.GetUpdateCommand();

                try
                {
                    ClientDa.Update(ClientDs, "Clients");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    var messageQueue = ClientChangedSnackBar.MessageQueue;
                    var message = "Информация о клиенте успешно изменена";
                    messageQueue.Enqueue(message);
                }
            }
        }

        private void DeleteBookingButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ChangeBookingButton_Click(object sender, RoutedEventArgs e)
        {
            DataSet changes = BookingDs.GetChanges();
            if (changes != null)
            {
                SqlCommandBuilder builder = new SqlCommandBuilder(BookingDa);
                BookingDa.UpdateCommand = builder.GetUpdateCommand();

                try
                {
                    BookingDa.Update(BookingDs, "Booking");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    var messageQueue = BookingChangedSnackBar.MessageQueue;
                    var message = "Информация о бронировании успешно изменена";
                    messageQueue.Enqueue(message);
                }
            }
        }
    }
}
