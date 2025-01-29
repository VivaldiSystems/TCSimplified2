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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TCSimplified2.Windows
{
    /// <summary>
    /// Interaction logic for EditShift.xaml
    /// </summary>
    public partial class EditShift : Window
    {
        DateTime date;
        TableDataAccessClass dataCollection;

        DateTime timeDT;
        string[] splitTime;

        public EditShift()
        {
            InitializeComponent();
            dataCollection = new TableDataAccessClass("shift_info", "timecard_id");
            string strSQL = "select top 1 timecard_id, employee_id, clock_status, datetime, shift_length from shift_info where timecard_id = '" + App.Current.Properties["timecard_id"] + "'";
            dataCollection.LoadDataTableFromSQL(strSQL);
            this.DataContext = dataCollection.dt;

            timeDT = DateTime.Parse(dataCollection.GetColumnByName("datetime"));
            splitTime = timeDT.ToString("t").Split(':', ' ');

            TimeHrTB.Text = splitTime[0];
            TimeMinTB.Text = splitTime[1];
            AMPM.SelectedValue = splitTime[2];

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
            int hours;
            int minutes;
            try
            {
                hours = int.Parse(TimeHrTB.Text);
                minutes = int.Parse(TimeMinTB.Text);
            }
            catch
            {
                MessageBox.Show("Error: Invalid time.");
                return;
            }
            if (AMPM.SelectedValue.ToString() == "PM" && hours != 12) 
            {
                hours += 12;
            }
            date = date.AddHours(hours);
            date = date.AddMinutes(minutes);
            MessageBox.Show(date.ToString());

            /* Check if the edited shift time is after the previous time and before the next time. 
             * If it it outside of the range, admin must create a new shift, not edit the existing one. */
            //string previousTime = dataCollection.GetColumnByName("datetime");
            //string endTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            //TimeSpan duration = DateTime.Parse(endTime).Subtract(DateTime.Parse(startTime));


            Object[] fields = { DatePick, TimeHrTB, TimeMinTB, AMPM };
            for (int i = 0; i < fields.Length; i++)
            {
                //if (fields[i].ToString() == null || fields[i].ToString() == "")
            }

            if (DatePick.SelectedDate == null || TimeHrTB.Text == "" || TimeMinTB.Text == "")
            {

            }

            // ADD VALIDATION BEFORE UNCOMMENTING

            /*
            string strSQL = "update shift_info set datetime = '" + date + "' where timecard_id = '" + App.Current.Properties["timecard_id"] + "'";
            dataCollection.ExecuteSQL(strSQL);
            Close();
            */

        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //ComboBox cb = sender as ComboBox;
            //App.Current.Properties["clock_status"] = cb.SelectedValue;

        }

        private void DatePick_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            try { date = DatePick.SelectedDate.Value.Date; }
            catch
            {
                DatePick.SelectedDate = timeDT;
            }
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
                case "TimeHrTB":
                    if (int.Parse(textBox.Text) > 12)
                    {
                        textBox.Text = "12";
                        textBox.SelectionStart = textBox.Text.Length;
                    }
                    break;
                case "TimeMinTB":
                    if (int.Parse(textBox.Text) > 59)
                    {
                        textBox.Text = "59";
                        textBox.SelectionStart = textBox.Text.Length;
                    }
                    break;
            }
        }


        private void InOutButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            switch (button.Name)
            {
                case "IN":
                    StatusTB.Text = "IN";
                    break;
                case "OUT":
                    StatusTB.Text = "OUT";
                    break;
            }
        }

        private void TimeHrTB_LostFocus(object sender, RoutedEventArgs e)
        {

        }

        private void TimeTBs_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (textBox.Text.Length == 0)
            {
                if (textBox.Name == "TimeHrTB")
                    textBox.Text = splitTime[0];
                else
                    textBox.Text = splitTime[1];
            }
        }
    }
}
