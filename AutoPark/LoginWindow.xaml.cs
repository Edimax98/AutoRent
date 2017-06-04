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
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        SqlConnection connection = new SqlConnection(@"Server=EDIMAX;Database=AutoPark;Trusted_Connection=Yes;");
        private LoginWindow lgWindow;
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

            // Strip the salt value off the front of the stored password.
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
          
            if (VerifyHashedPassword(PasswordTextBox.Text, getPasswordFromDB(LoginTextBox.Text)))
            {
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
            this.Close();
        }
    }
}
