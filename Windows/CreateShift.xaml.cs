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
    /// Interaction logic for CreateShift.xaml
    /// </summary>
    public partial class CreateShift : Window
    {
        TableDataAccessClass dataCollection;
        DateTime inDate;
        DateTime outDate;
        DateTime today;
        public CreateShift()
        {
            InitializeComponent();
            today = DateTime.Now;
            inDate = today;
            outDate = today;

            InDatePick.SelectedDate = today;
            OutDatePick.SelectedDate = today;
            InTimeHrTB.Text = "12";
            InTimeMinTB.Text = "00";
            OutTimeHrTB.Text = "12";
            OutTimeMinTB.Text = "01";
            InAMPM.SelectedValue = "AM";
            OutAMPM.SelectedValue = "AM";


            dataCollection = new TableDataAccessClass("shift_info", "timecard_id");
            string strSQL = "select timecard_id, employee_id, clock_status, datetime, shift_length, hourly_pay from shift_info where employee_id = '" + App.Current.Properties["employee_id"] + "' order by datetime desc";
            dataCollection.LoadDataTableFromSQL(strSQL);

            TableDataAccessClass temp = new TableDataAccessClass("employees", "employee_id");
            string strSQLtemp = "select employee_id, hourly_pay from employees where employee_id = '" + App.Current.Properties["employee_id"] + "'";
            temp.LoadDataTableFromSQL(strSQLtemp);
            tbPayRate.Text = temp.GetColumnByName("hourly_pay");
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

        private void TimeTBs_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            string temp = new string(textBox.Text.Where(c => Char.IsDigit(c)).ToArray());
            textBox.Text = temp;
            //int value;
            if (textBox.Text.Length > 2)
            {
                textBox.Text = textBox.Text.Substring(0, 2);
                textBox.SelectionStart = textBox.Text.Length;
            }
            try { int.Parse(textBox.Text); } catch { return; }

            switch (textBox.Name)
            {
                case "InTimeHrTB":
                case "OutTimeHrTB":
                    if (int.Parse(textBox.Text) > 12)
                    {
                        textBox.Text = "12";
                        textBox.SelectionStart = textBox.Text.Length;
                    }
                    break;
                case "InTimeMinTB":
                case "OutTimeMinTB":
                    if (int.Parse(textBox.Text) > 59)
                    {
                        textBox.Text = "59";
                        textBox.SelectionStart = textBox.Text.Length;
                    }
                    break;
            }
        }
        private void TimeTBs_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (textBox.Text.Length == 0)
            {
                if (textBox.Name == "InTimeHrTB" || textBox.Name == "OutTimeHrTB")
                    textBox.Text = "12";
                else if (textBox.Name == "InTimeMinTB")
                    textBox.Text = "00";
                else
                    textBox.Text = "01";
            }
        }

        private void DatePick_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            DatePicker datePicker = (DatePicker)sender;

            try
            {
                if (datePicker.Name == "InDatePick")
                    inDate = InDatePick.SelectedDate.Value.Date;
                else
                    outDate = OutDatePick.SelectedDate.Value.Date;
            }
            catch
            {
                if (datePicker.Name == "InDatePick")
                    InDatePick.SelectedDate = today;
                else
                    OutDatePick.SelectedDate = today;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            int inHours;
            int inMinutes;
            int outHours;
            int outMinutes;
            double payRate;

            DateTime originalInDate;
            DateTime originalOutDate;

            TableDataAccessClass temp = new TableDataAccessClass("employees", "employee_id");
            string strSQLtemp = "select employee_id, hourly_pay from employees where employee_id = '" + App.Current.Properties["employee_id"] + "'";
            temp.LoadDataTableFromSQL(strSQLtemp);
            App.Current.Properties["pay_rate"] = temp.GetColumnByName("hourly_pay");

            try
            {
                inHours = int.Parse(InTimeHrTB.Text);
                inMinutes = int.Parse(InTimeMinTB.Text);
                outHours = int.Parse(OutTimeHrTB.Text);
                outMinutes = int.Parse(OutTimeMinTB.Text);
                payRate = double.Parse(tbPayRate.Text);

                originalInDate = inDate;
                originalOutDate = outDate;
            }
            catch
            {
                MessageBox.Show("Error: Invalid value(s).");
                return;
            }
            if (InAMPM.SelectedValue.ToString() == "PM" && inHours != 12)
            {
                inHours += 12;
            }
            else if (InAMPM.SelectedValue.ToString() == "AM" && inHours == 12)
            {
                inHours = 0;
            }
            if (OutAMPM.SelectedValue.ToString() == "PM" && outHours != 12)
            {
                outHours += 12;
            }
            else if (OutAMPM.SelectedValue.ToString() == "AM" && outHours == 12)
            {
                outHours = 0;
            }
            inDate = inDate.AddHours(inHours);
            inDate = inDate.AddMinutes(inMinutes);
            outDate = outDate.AddHours(outHours);
            outDate = outDate.AddMinutes(outMinutes);
            //MessageBox.Show(inDate.ToString() + "\n" + outDate.ToString());

            TimeSpan duration = outDate.Subtract(inDate);
            if (duration.TotalMilliseconds < 0)
            {
                MessageBox.Show("Error: Clock out time is before clock in time.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                inDate = originalInDate;
                outDate = originalOutDate;
                return;
            }

            TableDataAccessClass dataCollectiontemp = new TableDataAccessClass("shift_info", "timecard_id");
            string strSQL = "SELECT COUNT(*) as count FROM shift_info WHERE datetime BETWEEN '" + inDate + "' AND '" + outDate + "';";
            dataCollectiontemp.LoadDataTableFromSQL(strSQL);
            if (dataCollectiontemp.GetColumnByName("count") != "0") 
            {
                MessageBox.Show("Error: New shift overlaps with an existing shift.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                inDate = originalInDate;
                outDate = originalOutDate;
                return;
            }

            strSQL = "insert into shift_info (employee_id, clock_status, datetime, shift_length, pay_rate) " +
                    "values (" + App.Current.Properties["employee_id"] + ", 'IN', '" + inDate + "', null, " + payRate + ")";
            dataCollection.ExecuteSQL(strSQL);

            strSQL = "insert into shift_info (employee_id, clock_status, datetime, shift_length) " +
                    "values (" + App.Current.Properties["employee_id"] + ", 'OUT', '" + outDate + "', '" + duration.TotalHours + "')";
            dataCollection.ExecuteSQL(strSQL);

            Close();
        }

    }
}
