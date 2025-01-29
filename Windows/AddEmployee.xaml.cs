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
using System.Windows.Shapes;

namespace TCSimplified2.Windows
{
    /// <summary>
    /// Interaction logic for AddEmployee.xaml
    /// </summary>
    public partial class AddEmployee : Window
    {
        TableDataAccessClass dataCollection;
        string addOrRemove;
        public AddEmployee(string addOrRemove)
        {
            InitializeComponent();
            this.addOrRemove = addOrRemove;
            InitializeTask();
        }

        private void InitializeTask()
        {
            string strSQL;
            switch (addOrRemove) 
            {
                case "add":
                    Title.Text = "Add Employee";
                    MainText.Text = "Add Employee:";

                    dataCollection = new TableDataAccessClass("employees", "employee_id");
                    strSQL = "select employee_id, first_name, last_name from employees";
                    dataCollection.LoadDataTableFromSQL(strSQL);
                    break;
                case "remove":
                    Title.Text = "Remove Employee";
                    MainText.Text = "Remove Employee:";
                    Cancel.Background = (Brush)FindResource("PrimaryGreenColor");
                    Save.Background = (Brush)FindResource("PrimaryRedColor");
                    Save.Content = "Remove";
                    Save.IsEnabled = false;
                    FirstLabel.Visibility = Visibility.Collapsed;
                    LastLabel.Visibility = Visibility.Collapsed;
                    EmployeeFirst.Visibility = Visibility.Collapsed;
                    EmployeeLast.Visibility = Visibility.Collapsed;
                    EmployeeSelect.Visibility = Visibility.Visible;

                    PopulateEmployees();

                    break;
            }
        }

        private void PopulateEmployees()
        {
            dataCollection = new TableDataAccessClass("employees", "employee_id");

            string strSQL = "select employee_id, first_name, last_name, first_name+' '+last_name as name from employees order by first_name";
            dataCollection.LoadDataTableFromSQL(strSQL);

            EmployeeSelect.ItemsSource = null;
            EmployeeSelect.Items.Clear();
            EmployeeSelect.ItemsSource = dataCollection.dt.DefaultView;

            // Load Timecards

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
            EmployeeFirst.Text = EmployeeFirst.Text.Replace(" ", "");
            EmployeeLast.Text = EmployeeLast.Text.Replace(" ", "");
            if (addOrRemove == "add")
            {
                if (dataCollection.GetAllDataInColumnByName("first_name").Contains(EmployeeFirst.Text) && dataCollection.GetAllDataInColumnByName("last_name").Contains(EmployeeLast.Text))
                {
                    MessageBox.Show("An employee with the same name already exists.");
                }
                else if (EmployeeFirst.Text == "" || EmployeeLast.Text == "")
                {
                    MessageBox.Show("Some fields are missing.");
                }
                else
                {
                    string strSQL = "insert into employees (first_name, last_name, hourly_pay) " +
                            "values ('" + EmployeeFirst.Text + "', '" + EmployeeLast.Text + "', '0.00')";
                    dataCollection.ExecuteSQL(strSQL);
                    Close();
                }
            }
            else
            {
                MessageBoxResult result = MessageBox.Show("Are you sure you want to remove this employee?\nALL TIMECARD AND PAY DATA WILL BE LOST.", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    string strSQL = "delete from employees where employee_id = '" + App.Current.Properties["employee_id"] + "'";
                    dataCollection.ExecuteSQL(strSQL);

                    dataCollection = new TableDataAccessClass("shift_info", "timecard_id");
                    strSQL = "delete from shift_info where employee_id = '" + App.Current.Properties["employee_id"] + "'";
                    dataCollection.ExecuteSQL(strSQL);
                    Close();
                }
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            App.Current.Properties["employee_id"] = cb.SelectedValue;
            Save.IsEnabled = true;
        }
    }
}
