using RibbonWin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;
using TCSimplified2.Windows;

namespace TCSimplified2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TableDataAccessClass dataCollectionEmployees;
        TableDataAccessClass dataCollectionSelectedEmployee;
        TableDataAccessClass dataCollectionTimecards;
        bool dropDownInitiallyOpened = false;

        public static Admin admin;

        public MainWindow()
        {
            InitializeComponent();

            InitialLogin loginWindow = new InitialLogin();
            loginWindow.Owner = null;
            loginWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            loginWindow.ShowDialog();
            if (Globals.UserID == "" && loginWindow.cbBypass.IsChecked == false)
            {
                Close();
            }

            PopulateEmployees();

            Date.Content = "Date: " + DateTime.Now.ToString("MM/dd/yyyy") + ".";
            Time.Content = "Current Time: " + DateTime.Now.ToString("hh:mm:ss tt") + ".";
            CurrentPunch.Text = "Current Punch: ";

            DispatcherTimer LiveTime = new DispatcherTimer();
            LiveTime.Interval = TimeSpan.FromSeconds(1);
            LiveTime.Tick += timer_Tick;
            LiveTime.Start();
        }

        private void PopulateEmployees()
        {
            dataCollectionEmployees = new TableDataAccessClass("employees", "employee_id");

            string strSQL = "select employee_id, first_name, last_name, first_name+' '+last_name as name, hourly_pay from employees order by first_name";
            dataCollectionEmployees.LoadDataTableFromSQL(strSQL);

            Employees.ItemsSource = null;
            Employees.Items.Clear();
            Employees.ItemsSource = dataCollectionEmployees.dt.DefaultView;

            // Load Timecards

        }

        void timer_Tick(object sender, EventArgs e)
        {
            Date.Content = "Date: " + DateTime.Now.ToString("MM/dd/yyyy") + ".";
            Time.Content = "Current Time: " + DateTime.Now.ToString("hh:mm:ss tt") + ".";
        }
        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void btnRestore_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Normal)
                WindowState = WindowState.Maximized;
            else
                WindowState = WindowState.Normal;
        }
        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ClockIn.IsEnabled = true;
            ClockOut.IsEnabled = true;
            ComboBox cb = sender as ComboBox;
            App.Current.Properties["employee_id"] = cb.SelectedValue;

            TableDataAccessClass temp = new TableDataAccessClass("employees", "employee_id");
            string strSQLtemp = "select employee_id, hourly_pay from employees where employee_id = '" + App.Current.Properties["employee_id"] + "'";
            temp.LoadDataTableFromSQL(strSQLtemp);
            App.Current.Properties["pay_rate"] = temp.GetColumnByName("hourly_pay");

            dataCollectionSelectedEmployee = new TableDataAccessClass("shift_info", "timecard_id");

            string strSQL = "select top 1 timecard_id, employee_id, clock_status, datetime, shift_length from shift_info where employee_id = '" + App.Current.Properties["employee_id"] + "' order by timecard_id desc";
            dataCollectionSelectedEmployee.LoadDataTableFromSQL(strSQL);
            if (dataCollectionSelectedEmployee.GetColumnByName("timecard_id") == "")
            {
                string strSQL2 = "insert into shift_info (employee_id, clock_status, datetime, shift_length) " +
                    "values (" + App.Current.Properties["employee_id"] + ", 'OUT', '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', '0.00')";
                dataCollectionSelectedEmployee.ExecuteSQL(strSQL2);
                dataCollectionSelectedEmployee.LoadDataTableFromSQL(strSQL);
            }

            CurrentPunch.Text = "Current Punch: " + dataCollectionSelectedEmployee.GetColumnByName("clock_status") + ", " + dataCollectionSelectedEmployee.GetColumnByName("datetime");
            if (dataCollectionSelectedEmployee.GetColumnByName("clock_status") == "OUT")
            {
                ClockIn.IsEnabled = true;
                ClockOut.IsEnabled = false;
            }
            else
            {
                ClockIn.IsEnabled = false;
                ClockOut.IsEnabled = true;
            }
        }

        private void ClockInOut_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            string strSQL = "";

            if (!EmployeePIN.ShowPIN())
                return;

            switch (button.Name)
            {
                case "ClockIn":
                    strSQL = "insert into shift_info (employee_id, clock_status, datetime, shift_length, pay_rate) " +
                    "values (" + App.Current.Properties["employee_id"] + ", 'IN', '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', null, " + App.Current.Properties["pay_rate"] + ")";

                    ClockIn.IsEnabled = false;
                    ClockOut.IsEnabled = true;
                    break;
                case "ClockOut":
                    string startTime = dataCollectionSelectedEmployee.GetColumnByName("datetime");
                    string endTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    TimeSpan duration = DateTime.Parse(endTime).Subtract(DateTime.Parse(startTime));

                    strSQL = "insert into shift_info (employee_id, clock_status, datetime, shift_length) " +
                    "values (" + App.Current.Properties["employee_id"] + ", 'OUT', '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', '" + duration.TotalHours + "')";

                    ClockIn.IsEnabled = true;
                    ClockOut.IsEnabled = false;
                    break;
            }
            dataCollectionSelectedEmployee.ExecuteSQL(strSQL);
            UpdateTimecard();
        }

        private void UpdateTimecard()
        {
            dataCollectionSelectedEmployee = new TableDataAccessClass("shift_info", "timecard_id");

            string strSQL = "select timecard_id, employee_id, clock_status, datetime from shift_info where employee_id = '" + App.Current.Properties["employee_id"] + "' order by timecard_id desc";
            dataCollectionSelectedEmployee.LoadDataTableFromSQL(strSQL);
            CurrentPunch.Text = "Current Punch: " + dataCollectionSelectedEmployee.GetColumnByName("clock_status") + ", " + dataCollectionSelectedEmployee.GetColumnByName("datetime");
        }

        private void Admin_Click(object sender, RoutedEventArgs e)
        {
            if (!AdminPIN.ShowPIN())
                return;

            admin = new Admin();
            admin.ShowDialog();

            PopulateEmployees();
            CurrentPunch.Text = "Current Punch: ";
            Employees.Text = "Select Employee...";
            ClockIn.IsEnabled = false;
            ClockOut.IsEnabled = false;
        }

        private void Employees_DropDownOpened(object sender, EventArgs e)
        {
            if (!dropDownInitiallyOpened)
            {
                Employees.Text = "";
                dropDownInitiallyOpened = true;
            }
        }
    }
}
