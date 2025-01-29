using RibbonWin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
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
    /// Interaction logic for InitialLogin.xaml
    /// </summary>
    public partial class InitialLogin : Window
    {
        TableDataAccessClass serverCollection;
        public InitialLogin()
        {
            InitializeComponent();

            string macAddress = Utility.Encrypt(GetDefaultMacAddress());
            IniFile MyIni = new IniFile("tcsimplified.ini");
            if (false )  //macAddress != MyIni.GetValue("SECURITY", "MACADDRESS"))
            {
                MessageBox.Show("Your device is not registered for this software!");
                Close();
                return;
            }

            username.Text = MyIni.GetValue("SECURITY", "EMAILLOGIN");
            if (username.Text == "")
                username.Focus();
            else
            {
                password.Focus();
                cbSave.IsChecked = true;
            }
            cbBypass.IsChecked = true;
        }

        private string GetDefaultMacAddress()
        {
            Dictionary<string, long> macAddresses = new Dictionary<string, long>();
            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (nic.OperationalStatus == OperationalStatus.Up)
                    macAddresses[nic.GetPhysicalAddress().ToString()] = nic.GetIPStatistics().BytesSent + nic.GetIPStatistics().BytesReceived;
            }
            long maxValue = 0;
            string mac = "";
            foreach (KeyValuePair<string, long> pair in macAddresses)
            {
                if (pair.Value > maxValue)
                {
                    mac = pair.Key;
                    maxValue = pair.Value;
                }
            }
            return mac;
        }

        private void LoginClick(object sender, RoutedEventArgs e)
        {

            if (Utility.IsInjection(username.Text) || Utility.IsInjection(password.Password))
            {
                MessageBox.Show("Invalid values found in the input!");
                return;
            }

            
            if (cbSave.IsChecked == true)
            {
                IniFile MyIni = new IniFile("tcsimplified.ini");
                MyIni.SetValue("SECURITY", "EMAILLOGIN", username.Text);
                MyIni.Save();
            }
            else
            {
                IniFile MyIni = new IniFile("tcsimplified.ini");
                MyIni.SetValue("SECURITY", "EMAILLOGIN", "");
                MyIni.Save();
            }
            

            serverCollection = new TableDataAccessClass("admins", "admin_id");

            string encryptpassword = Utility.Encrypt(password.Password);

            string strSQL = "SELECT TOP 1 admin_id, username from admins WHERE username = '" + username.Text + "' AND password = '" + encryptpassword + "'";

            //add these fields to the sales.staffs if needed
            serverCollection.LoadDataTableFromSQL(strSQL);

            try
            {
                if (serverCollection.dt.Rows[0]["username"].ToString() == "" && cbBypass.IsChecked == false) // serverCollection.FoundRows() == false     
                {
                    MessageBox.Show("Username or password is invalid. Please try again.");
                    return;
                }
            }
            catch
            {
                MessageBox.Show("No admin logins detected. Contact support.");
                return;
            }

            Globals.UserID = serverCollection.GetColumnByName("admin_id");
            this.Close();

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

        }
    }
}
