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
    /// Interaction logic for AdminHome.xaml
    /// </summary>
    public partial class AdminHome : Page
    {
        public AdminHome()
        {
            InitializeComponent();
        }
        private void AddEmployee_Click(object sender, RoutedEventArgs e)
        {
            AddEmployee addEmployee = new AddEmployee("add");
            addEmployee.ShowDialog();
        }

        private void RemoveEmployee_Click(object sender, RoutedEventArgs e)
        {
            AddEmployee removeEmployee = new AddEmployee("remove");
            removeEmployee.ShowDialog();
        }

        private void ViewEmployee_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.admin.frameContent.Navigate(new ViewAllEmployeeInfo());
        }

        private void AdminLogin_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.admin.frameContent.Navigate(new AdminLogins());
        }

        private void Writeups_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.admin.frameContent.Navigate(new Writeups());
        }

        private void Disclaimer_Click(object sender, RoutedEventArgs e)
        {
            LegalDisclaimer legalDisclaimer = new LegalDisclaimer();
            legalDisclaimer.ShowDialog();
        }
    }
}
