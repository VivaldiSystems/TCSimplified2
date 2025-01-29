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

namespace TCSimplified2.Pages
{
    /// <summary>
    /// Interaction logic for Writeups.xaml
    /// </summary>
    public partial class Writeups : Page
    {
        public Writeups()
        {
            InitializeComponent();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.admin.frameContent.Navigate(new AdminHome());
        }
    }
}
