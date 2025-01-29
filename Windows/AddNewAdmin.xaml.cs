using RibbonWin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
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

namespace TCSimplified2.Windows
{
    /// <summary>
    /// Interaction logic for AddNewAdmin.xaml
    /// </summary>
    public partial class AddNewAdmin : Window
    {
        public AddNewAdmin()
        {
            InitializeComponent();
        }
        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try { int.Parse(tbPIN.Text); }
            catch
            {
                MessageBox.Show("Error: Invalid PIN!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (tbUser.Text.Length == 0 || tbPass.Password.Length == 0 || tbPIN.Text.Length == 0 || Utility.IsInjection(tbUser.Text) || Utility.IsInjection(tbPass.Password))
            {
                MessageBox.Show("Error: Some required fields are missing or there are invalid characters in the input.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return; 
            }
            if (tbPIN.Text.Length > 10)
            {
                tbPIN.Text = tbPIN.Text.Substring(0, 10);
            }

            string encryptpassword = Utility.Encrypt(tbPass.Password);
            string strSQL = "insert into admins (username, password, admin_pin) " +
                    "values ('" + tbUser.Text + "', '" + encryptpassword + "', '" + tbPIN.Text + "')";
            TableDataAccessClass dataCollection = new TableDataAccessClass("admins", "admin_id");
            dataCollection.ExecuteSQL(strSQL);
            MessageBox.Show("Admin saved.");
            Close();
        }
    }
}

