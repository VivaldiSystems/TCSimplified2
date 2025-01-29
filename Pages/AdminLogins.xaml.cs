using RibbonWin;
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
using TCSimplified2.Windows;

namespace TCSimplified2.Pages
{
    /// <summary>
    /// Interaction logic for AdminLogins.xaml
    /// </summary>
    public partial class AdminLogins : Page
    {
        TableDataAccessClass dataCollection;
        public AdminLogins()
        {
            InitializeComponent();
            InitializeAdmins();
        }

        private void InitializeAdmins()
        {
            dataCollection = new TableDataAccessClass("admin", "admin_id");

            string strSQL = "select admin_id, admin_pin, username from admins order by username";
            dataCollection.LoadDataTableFromSQL(strSQL);

            AdminSelect.ItemsSource = null;
            AdminSelect.Items.Clear();
            AdminSelect.ItemsSource = dataCollection.dt.DefaultView;
            AdminSelect.Text = "Select Admin...";
            spPIN.IsEnabled = false;
            spResetPrompt.IsEnabled = false;
            RemoveBtn.IsEnabled = false;
        }

        private void PopulateAllFields()
        {
            dataCollection = new TableDataAccessClass("admins", "admin_id");

            string strSQL = "select admin_id, admin_pin, username from admins where admin_id = '" + App.Current.Properties["admin_id"] + "'";

            dataCollection.LoadDataTableFromSQL(strSQL);

            this.DataContext = dataCollection.dt;
        }

        private void Admin_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            App.Current.Properties["admin_id"] = cb.SelectedValue;
            PopulateAllFields();
            spPIN.IsEnabled = true;
            spResetPrompt.IsEnabled = true;
            RemoveBtn.IsEnabled = true;
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.admin.frameContent.Navigate(new AdminHome());
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try { if (PIN.Text != "") { int.Parse(PIN.Text); } }
            catch
            {
                MessageBox.Show("Could not save: Invalid PIN.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (PIN.Text.Length < 4)
            {
                MessageBox.Show("Could not save: PIN must be at least 4 characters long.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else
            {
                if (dataCollection.UpdateRow())
                {
                    PopulateAllFields();
                }
            }
        }

        private void AddAdmin_Click(object sender, RoutedEventArgs e)
        {
            AddNewAdmin addNewAdmin = new AddNewAdmin();
            addNewAdmin.ShowDialog();
            InitializeAdmins();
            InitializeAdmins();
        }

        private void ResetPassword_Click(object sender, RoutedEventArgs e)
        {
            if (spReset.Visibility == Visibility.Hidden)
                spReset.Visibility = Visibility.Visible;
            else
                spReset.Visibility = Visibility.Hidden;
        }

        private void SavePassword_Click(object sender, RoutedEventArgs e)
        {
            if (Utility.IsInjection(PasswordBox.Password))
            {
                MessageBox.Show("Invalid values found in the input!");
                return;
            }

            string encryptpassword = Utility.Encrypt(PasswordBox.Password);

            string strSQL = "UPDATE admins SET password = '" + encryptpassword + "' WHERE admin_id = '" + App.Current.Properties["admin_id"] + "'";
            dataCollection.ExecuteSQL(strSQL);
            spReset.Visibility = Visibility.Hidden;
        }

        private void RemoveAdmin_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you would like to remove this admin?", "Are you sure?", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                string strSQL = "delete from admins where admin_id = '" + App.Current.Properties["admin_id"] + "'";
                dataCollection.ExecuteSQL(strSQL);
                InitializeAdmins();
                InitializeAdmins();
            }
        }
    }
}
