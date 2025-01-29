using RibbonWin;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
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

namespace TCSimplified2.Windows
{
    /// <summary>
    /// Interaction logic for AdminPIN.xaml
    /// </summary>
    public partial class AdminPIN : Window
    {
        bool pinFound;
        public AdminPIN()
        {
            InitializeComponent();
            admin_warning.Text = "* This action requires an admin PIN number to continue.\n  Please enter your PIN.";
            tbPIN.Focus();
            
        }

        public static bool ShowPIN()
        {
            AdminPIN adminPIN = new AdminPIN();
            adminPIN.ShowDialog();
            if (adminPIN.FoundPIN())
                return true;
            return false;
        }

        private void Num_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            string temp = new string(tbPIN.Text.Where(c => Char.IsDigit(c)).ToArray());
            tbPIN.Text = temp;
            if (button.Name == "back")
            {
                if (tbPIN.Text.Length > 0)
                    tbPIN.Text = tbPIN.Text.Substring(0, tbPIN.Text.Length-1);
            }
            else
                tbPIN.Text += button.Content;

            tbPIN.Focus();
            tbPIN.SelectionStart = tbPIN.Text.Length;
        }

        private void enter_Click(object sender, RoutedEventArgs e)
        {
            TableDataAccessClass checkPin = new TableDataAccessClass("admins", "admin_id");
            string strSQL = "select admin_id, admin_pin from admins";
            checkPin.LoadDataTableFromSQL(strSQL);
            string[] pins = checkPin.GetAllDataInColumnByName("admin_pin");
            pinFound = false;
            for (int i = 0; i < pins.Length; i++)
            {
                if (pins[i] == tbPIN.Text)
                {
                    pinFound = true;
                    break;
                }
            }
            if (!pinFound)
            {
                MessageBox.Show("Incorrect PIN!");
                return;
            }

            this.Close();

            /*
            switch (newWindow)
            {
                case "Admin":
                    break;
                default:
                    MessageBox.Show("Error: Window '" + newWindow + "' could not be found.");
                    break;
            }
            */
        }

        public bool FoundPIN()
        {
            if (pinFound)
                return true;
            return false;
        }

        private void tbPIN_TextChanged(object sender, TextChangedEventArgs e)
        {
            string temp = new string(tbPIN.Text.Where(c => Char.IsDigit(c)).ToArray());
            tbPIN.Text = temp;
            tbPIN.SelectionStart = tbPIN.Text.Length;
            if (tbPIN.Text.Length > 10)
            {
                tbPIN.Text = tbPIN.Text.Substring(0, 10);
            }
        }
        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
