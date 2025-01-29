using RibbonWin;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using TCSimplified2.Windows;

namespace TCSimplified2.Pages
{
    /// <summary>
    /// Interaction logic for ViewAllEmployeeInfo.xaml
    /// </summary>
    public partial class ViewAllEmployeeInfo : Page
    {
        TableDataAccessClass dataCollection;
        TableDataAccessClass dataCollectionTimecard;
        TableDataAccessClass dataCollectionAllShifts;
        TextBox[] allFields;
        string storedPayText;

        DateTime startShiftDate;
        DateTime endShiftDate;
        bool alreadyCustom = false;


        public ViewAllEmployeeInfo()
        {
            InitializeComponent();
            allFields = new TextBox[] { first_name, last_name, phone, email, address, apt, city, state, zip, hourly_pay, tbPIN };
            PopulateEmployees();
            DisableAll();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.admin.frameContent.Navigate(new AdminHome());
        }

        private void PopulateEmployees()
        {
            dataCollection = new TableDataAccessClass("employees", "employee_id");

            string strSQL = "select employee_id, first_name, last_name, first_name+' '+last_name as name from employees order by first_name";
            dataCollection.LoadDataTableFromSQL(strSQL);

            EmployeeSelect.ItemsSource = null;
            EmployeeSelect.Items.Clear();
            EmployeeSelect.ItemsSource = dataCollection.dt.DefaultView;
        }

        private void PopulateAllFields()
        {
            dataCollection = new TableDataAccessClass("employees", "employee_id");

            string strSQL = "select employee_id, first_name, last_name, employee_pin, phone, email, address, apt, city, state, zip, hourly_pay " +
                "from employees where employee_id = '" + App.Current.Properties["employee_id"] + "'";

            dataCollection.LoadDataTableFromSQL(strSQL);

            this.DataContext = dataCollection.dt;
        }

        private void PopulateAllShifts()
        {
            dataCollectionAllShifts = new TableDataAccessClass("shift_info", "timecard_id");
            if (!alreadyCustom)
            {
                endShiftDate = DateTime.Now.AddDays(1);
            }
            bool alltime = false;
            CustomRange.Visibility = Visibility.Hidden;
            switch (cbFilter.SelectedValue)
            {
                case "Past Week":
                    alreadyCustom = false;
                    startShiftDate = DateTime.Now.AddDays(-7);
                    break;
                case "Past 2 Weeks":
                    alreadyCustom = false;
                    startShiftDate = DateTime.Now.AddDays(-14);
                    break;
                case "Past Month":
                    alreadyCustom = false;
                    startShiftDate = DateTime.Now.AddMonths(-1);
                    break;
                case "Past Year":
                    alreadyCustom = false;
                    startShiftDate = DateTime.Now.AddYears(-1);
                    break;
                case "Past 2 Years":
                    alreadyCustom = false;
                    startShiftDate = DateTime.Now.AddYears(-2);
                    break;
                case "All Time":
                    alreadyCustom = false;
                    alltime = true;
                    break;
                case "Custom Range...":
                    if (!alreadyCustom)
                    {
                        alreadyCustom = true;
                        startShiftDate = DateTime.Now.AddMonths(-1);
                        endShiftDate = DateTime.Now;
                        dpStart.SelectedDate = startShiftDate;
                        dpEnd.SelectedDate = endShiftDate;
                    }
                    /*
                    if (dpStart.SelectedDate == null || dpEnd.SelectedDate == null)
                    {
                        MessageBox.Show("test");
                        return;
                        //startShiftDate = DateTime.Now;
                        //endShiftDate = DateTime.Now;
                    }
                    */
                    CustomRange.Visibility = Visibility.Visible;
                    //startShiftDate = dpStart.SelectedDate.Value.Date;
                    //endShiftDate = dpEnd.SelectedDate.Value.Date;
                    alreadyCustom = true;
                    break;
            }
            
            string strSQL = "";
            if (alltime)
                strSQL = "select timecard_id, employee_id, clock_status, datetime, shift_length, pay_rate from shift_info where employee_id = '" + App.Current.Properties["employee_id"] + "' order by datetime desc";
            else
                strSQL = "select timecard_id, employee_id, clock_status, datetime, shift_length, pay_rate from shift_info where datetime >= '" + startShiftDate + "' and datetime < '" + endShiftDate + "' and employee_id = '" + App.Current.Properties["employee_id"] + "' order by datetime desc";

            dataCollectionAllShifts.LoadDataTableFromSQL(strSQL);

            dgShifts.ItemsSource = dataCollectionAllShifts.dt.DefaultView;

            dgShifts.IsEnabled = true;
            if (dataCollectionAllShifts.GetColumnByName("clock_status") == "")
            {
                dgShifts.IsEnabled = false;
            }

            RemovePunch.IsEnabled = false;
            cbFilter.IsEnabled = true;
        }


        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            App.Current.Properties["employee_id"] = cb.SelectedValue;
            storedPayText = hourly_pay.Text;

            dataCollectionTimecard = new TableDataAccessClass("shift_info", "timecard_id");


            string strSQL = "select top 1 timecard_id, employee_id, clock_status, datetime, shift_length from shift_info where employee_id = '" + App.Current.Properties["employee_id"] + "' order by datetime desc";
            dataCollectionTimecard.LoadDataTableFromSQL(strSQL);
            if (dataCollectionTimecard.GetColumnByName("timecard_id") == "")
            {
                string strSQL2 = "insert into shift_info (employee_id, clock_status, datetime, shift_length) " +
                    "values (" + App.Current.Properties["employee_id"] + ", 'OUT', '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', '0.00')";
                dataCollectionTimecard.ExecuteSQL(strSQL2);
                dataCollectionTimecard.LoadDataTableFromSQL(strSQL);
            }
            //CurrentPunch.Text = "Current Punch: " + dataCollectionTimecard.GetColumnByName("clock_status") + ", " + dataCollectionTimecard.GetColumnByName("datetime");

            CreatePunch.IsEnabled = true;
            cbFilter.SelectedValue = "Past Month";
            PopulateAllFields();
            PopulateAllShifts();
            EnableAll();
        }
        private void EnableAll()
        {
            for (int i = 0; i < allFields.Length; i++)
            {
                allFields[i].IsEnabled = true;
            }
            Save.IsEnabled = true;
        }
        private void DisableAll()
        {
            for (int i = 0; i < allFields.Length; i++)
            {
                allFields[i].IsEnabled = false;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try { if (hourly_pay.Text != "") { double.Parse(hourly_pay.Text); } }
            catch
            {
                MessageBox.Show("Could not save: Invalid pay rate.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }


            if (hourly_pay.Text == "")
            {
                MessageBoxResult result = MessageBox.Show("WARNING: Saving without a pay rate will cause a default pay rate of $0/hr.\nAre you sure you want to continue?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    if (dataCollection.UpdateRow())
                    {
                        PopulateAllFields();
                    }
                }
            }
            else
            {
                if (dataCollection.UpdateRow())
                {

                    PopulateAllFields();
                }
            }
            
        }


        private void hourly_pay_TextChanged(object sender, TextChangedEventArgs e)
        {
            hourly_pay.Text = hourly_pay.Text.Replace(" ", "");
        }

        private void hourly_pay_LostFocus(object sender, RoutedEventArgs e)
        {
            if (hourly_pay.Text == "")
            {
                hourly_pay.Text = "0.00";
            }
        }

        private void dgShifts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //PunchBtns.IsEnabled = true;
            RemovePunch.IsEnabled = true;
            var cellInfo = dgShifts.SelectedCells[0];
            if (cellInfo != null && cellInfo.IsValid)
            {
                var value = ((System.Data.DataRowView)cellInfo.Item).Row.ItemArray[0].ToString();

                App.Current.Properties["timecard_id"] = value;
            }
            else
            {
                App.Current.Properties["timecard_id"] = "";
            }
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            CreateShift createShift = new CreateShift();
            createShift.ShowDialog();
            PopulateAllShifts();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            EditShift editShift = new EditShift();
            editShift.ShowDialog();
            PopulateAllShifts();
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            TableDataAccessClass tempDataCollection = new TableDataAccessClass("shift_info", "timecard_id");

            int index = int.Parse(App.Current.Properties["timecard_id"].ToString());
            string strSQL = "select timecard_id, clock_status, datetime, shift_length from shift_info where employee_id = '" + App.Current.Properties["employee_id"] + "' order by timecard_id";  //string strSQL = "select timecard_id, clock_status from shift_info where employee_id = '" + App.Current.Properties["employee_id"] + "' and timecard_id between '" + (index-1) + "' and '" + (index+1) +  "' order by timecard_id";
            tempDataCollection.LoadDataTableFromSQL(strSQL);

            /* For Confirmation Message Later */
            TableDataAccessClass employeeNameForMsg = new TableDataAccessClass("employees", "employee_id");
            string employeeStrSQL = "select first_name+' '+last_name as name from employees where employee_id = '" + App.Current.Properties["employee_id"] + "'";
            employeeNameForMsg.LoadDataTableFromSQL(employeeStrSQL);

            string msg = "Are you sure you want to remove this shift?\nTHIS IS A PERMANENT ACTION. Shift info will be lost.\n" +
                "Employee: " + employeeNameForMsg.GetColumnByName("name") + "\n\n";
            /*--------------------------------*/


            for (int i = 0; i < tempDataCollection.dt.Rows.Count; i++)
            {
                if (tempDataCollection.dt.Rows[i]["timecard_id"].ToString() == index.ToString())
                {
                    index = i;
                    break;
                }
            }
            string rowStatus = tempDataCollection.dt.Rows[index]["clock_status"].ToString();
            string rowBelowStatus = "";
            string rowAboveStatus = "";
            if (index < tempDataCollection.dt.Rows.Count-1)
                rowBelowStatus = tempDataCollection.dt.Rows[index + 1]["clock_status"].ToString();
            if (index > 0)
                rowAboveStatus = tempDataCollection.dt.Rows[index - 1]["clock_status"].ToString();

            if (rowBelowStatus == "" && rowAboveStatus == "") // if selected row is the only row
            {
                MessageBoxResult result = MessageBox.Show("Deleting this shift will cause it to be replaced by a default OUT shift.\nDo you want to continue?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.No) { return; }
                strSQL = "delete from shift_info where employee_id = '" + App.Current.Properties["employee_id"] + "' and timecard_id = '" + tempDataCollection.dt.Rows[index]["timecard_id"] + "' " + // delete only row exactly at index and replace with default OUT clock at the current time (as currently done when creating a new employee).
                    "insert into shift_info (employee_id, clock_status, datetime, shift_length) " +
                    "values (" + App.Current.Properties["employee_id"] + ", 'OUT', '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', '0.00')";
                msg += "OUT: " + tempDataCollection.dt.Rows[index]["datetime"] + "\n" +
                    "Shift Length: " + tempDataCollection.dt.Rows[index]["shift_length"] + " hrs.\n";
            }
            else if (rowBelowStatus != "" && rowAboveStatus == "") // if selected row is the top row
            {
                if (rowStatus == "IN")
                {
                    // delete row at index and index + 1
                    strSQL = "delete from shift_info where employee_id = '" + App.Current.Properties["employee_id"] + "' and timecard_id between '" + tempDataCollection.dt.Rows[index]["timecard_id"] + "' and '" + tempDataCollection.dt.Rows[index + 1]["timecard_id"] + "'";
                    msg += "IN: " + tempDataCollection.dt.Rows[index]["datetime"] + "\n" +
                           "OUT: " + tempDataCollection.dt.Rows[index + 1]["datetime"] + "\n" +
                           "Shift Length: " + tempDataCollection.dt.Rows[index + 1]["shift_length"] + " hrs.\n";
                }
                else
                {
                    // delete row only at index
                    strSQL = "delete from shift_info where employee_id = '" + App.Current.Properties["employee_id"] + "' and timecard_id = '" + tempDataCollection.dt.Rows[index]["timecard_id"] + "'";
                    msg += "OUT: " + tempDataCollection.dt.Rows[index]["datetime"] + "\n" +
                        "Shift Length: " + tempDataCollection.dt.Rows[index]["shift_length"] + " hrs.\n";
                }
            }
            else if (rowBelowStatus == "" && rowAboveStatus != "") // if selected row is the bottom row
            {
                if (rowStatus == "IN")
                {
                    // delete row only at index
                    strSQL = "delete from shift_info where employee_id = '" + App.Current.Properties["employee_id"] + "' and timecard_id = '" + tempDataCollection.dt.Rows[index]["timecard_id"] + "'";
                    msg += "IN: " + tempDataCollection.dt.Rows[index]["datetime"];
                }
                else
                {
                    // delete row at index and index - 1
                    strSQL = "delete from shift_info where employee_id = '" + App.Current.Properties["employee_id"] + "' and timecard_id between '" + tempDataCollection.dt.Rows[index - 1]["timecard_id"] + "' and '" + tempDataCollection.dt.Rows[index]["timecard_id"] + "'";
                    msg += "IN: " + tempDataCollection.dt.Rows[index - 1]["datetime"] + "\n" +
                           "OUT: " + tempDataCollection.dt.Rows[index]["datetime"] + "\n" +
                           "Shift Length: " + tempDataCollection.dt.Rows[index]["shift_length"] + " hrs.\n";
                }
            }
            else // if selected row is in between two rows
            {
                if (rowStatus == "IN")
                {
                    // delete row at index and index + 1
                    strSQL = "delete from shift_info where employee_id = '" + App.Current.Properties["employee_id"] + "' and timecard_id between '" + tempDataCollection.dt.Rows[index]["timecard_id"] + "' and '" + tempDataCollection.dt.Rows[index + 1]["timecard_id"] + "'";
                    msg += "IN: " + tempDataCollection.dt.Rows[index]["datetime"] + "\n" +
                           "OUT: " + tempDataCollection.dt.Rows[index + 1]["datetime"] + "\n" +
                           "Shift Length: " + tempDataCollection.dt.Rows[index + 1]["shift_length"] + " hrs.\n";
                }
                else
                {
                    // delete row at index and index - 1
                    strSQL = "delete from shift_info where employee_id = '" + App.Current.Properties["employee_id"] + "' and timecard_id between '" + tempDataCollection.dt.Rows[index - 1]["timecard_id"] + "' and '" + tempDataCollection.dt.Rows[index]["timecard_id"] + "'";
                    msg += "IN: " + tempDataCollection.dt.Rows[index - 1]["datetime"] + "\n" +
                           "OUT: " + tempDataCollection.dt.Rows[index]["datetime"] + "\n" +
                           "Shift Length: " + tempDataCollection.dt.Rows[index]["shift_length"] + " hrs.\n";
                }
            }

            MessageBoxResult finalResult = MessageBox.Show(msg, "Are you sure?", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (finalResult == MessageBoxResult.Yes)
            {
                tempDataCollection.ExecuteSQL(strSQL);
                PopulateAllShifts();
            }
        }

        private void loginInfo_Click(object sender, RoutedEventArgs e)
        {
            // if (!AdminPIN.ShowPIN())
            //     return;
        }

        private void cbFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PopulateAllShifts();
        }

        private void RangeSelect_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dpEnd.SelectedDate != null && dpStart.SelectedDate != null)
            {
                TimeSpan duration = dpEnd.SelectedDate.Value.Date.Subtract(dpStart.SelectedDate.Value.Date);
                if (duration.TotalMilliseconds > 0)
                {
                    startShiftDate = dpStart.SelectedDate.Value.Date;
                    endShiftDate = dpEnd.SelectedDate.Value.Date.AddDays(1);
                }
                else
                {
                    MessageBox.Show("Error: End date is before start date.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    dpStart.SelectedDate = startShiftDate;
                    dpEnd.SelectedDate = endShiftDate;
                    return;
                }
            }
            else
            {
                dgShifts.IsEnabled = false;
            }
            PopulateAllShifts();
        }
    }
}