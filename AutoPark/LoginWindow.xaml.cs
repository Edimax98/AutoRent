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
using System.Windows.Shapes;
using System.Security.Cryptography;
using System.Data.SqlClient;

namespace AutoPark
{
    public partial class LoginWindow : Window
    {
        private MainWindow window;
        SqlConnection connection = new SqlConnection(@"Server=EDIMAX;Database=AutoPark;Trusted_Connection=Yes;");
        public LoginWindow()
        {
            InitializeComponent();
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

        private string getPasswordFromDB(string login)
        {
            string str = "";
            connectToDataBase();

            string getPass = "SELECT hash FROM admins where login = @login";
            SqlCommand commandBrands = new SqlCommand(getPass, connection);
            commandBrands.Parameters.AddWithValue("@login", login);
            using (SqlDataReader reader = commandBrands.ExecuteReader())
            {
                while (reader.Read())
                {
                    str = reader["hash"].ToString();
                }
            }
            connection.Close();
            return str;
        }

        private const int SaltValueSize = 4;
        private static string[] HashingAlgorithms = new string[] { "SHA256", "MD5" };

        private bool VerifyHashedPassword(string password, string profilePassword)
        {
            int saltLength = SaltValueSize * UnicodeEncoding.CharSize;

            if (string.IsNullOrEmpty(profilePassword) ||
                string.IsNullOrEmpty(password) ||
                profilePassword.Length < saltLength)
            {
                return false;
            }

            string saltValue = profilePassword.Substring(0, saltLength);

            foreach (string hashingAlgorithmName in HashingAlgorithms)
            {
                HashAlgorithm hash = HashAlgorithm.Create(hashingAlgorithmName);
                string hashedPassword = Hash(password);
                if (profilePassword.Equals(hashedPassword, StringComparison.Ordinal))
                    return true;
            }
            return false;
        }

        private string Hash(string str)
        {
            using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
            {
                UTF8Encoding utf8 = new UTF8Encoding();
                byte[] data = md5.ComputeHash(utf8.GetBytes(str));
                return Convert.ToBase64String(data);
            }
        }

        private void LogInButton_Click(object sender, RoutedEventArgs e)
        {
          
            if (VerifyHashedPassword(PasswordTextBox.Password, getPasswordFromDB(LoginTextBox.Text)))
            {
                foreach (Window window in Application.Current.Windows)
                {
                    if (window.GetType() == typeof(MainWindow))
                    {
                        (window as MainWindow).AddNewThingPopUp.Visibility = Visibility.Visible;
                        (window as MainWindow).CarTableCard.Visibility = Visibility.Visible;
                        (window as MainWindow).ClientTableCard.Visibility = Visibility.Visible;
                        (window as MainWindow).BookingTableCard.Visibility = Visibility.Visible;
                    }
                }
                this.Close();
            } else
            {
                var messageQueue = LoginSnackBar.MessageQueue;
                var message = "Неверные данные";
                messageQueue.Enqueue(message);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window.GetType() == typeof(MainWindow))
                {
                    (window as MainWindow).AddNewThingPopUp.Visibility = Visibility.Collapsed;
                    (window as MainWindow).CarTableCard.Visibility = Visibility.Collapsed;
                    (window as MainWindow).ClientTableCard.Visibility = Visibility.Collapsed;
                    (window as MainWindow).BookingTableCard.Visibility = Visibility.Collapsed;
                }
            }

            this.Close();
        }
    }
}
