using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics.Eventing.Reader;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Security.Policy;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Xml.Linq;
using TCSimplified2;

namespace RibbonWin
{
    public class TableDataAccessClass
    {
        //private string connectionString = "Data Source=104.181.51.61\\SQLEXPRESS;Initial Catalog=Sneakers;Integrated Security=True";
        private string connectionString = "Data Source=104.181.51.61;Initial Catalog=TCSimplified;Persist Security Info=True;User ID=ils;Password=foll2164";
        private SqlConnection connection;


        public ObservableCollection<DataTable> dataCollection;
        public DataTable dt;

        public List<string> columns = new List<string>();
        public Dictionary<string, string> prevcolumnvalue = new Dictionary<string, string>();
        public string TableName;

        public string PrimeKey;
        public string PrimeKeyValue;
        public string UpdateSet;
        public string SchemaName;
        public string Table;
        public string query = "";

        //private Window window;

        public TableDataAccessClass(string tablename, string primekey)
        {
            PrimeKey = primekey;

            //window = w;

            SchemaName = "";
            Table = "";

            connection = new SqlConnection(connectionString);
            LoadTable(tablename);


        }

        public bool LoadTable(string tablename)
        {

            TableName = tablename;
            connection.Open();



            Table = tablename;

            string[] values = TableName.Split('.');

            if (values.Length == 2)
            {
                SchemaName = values[0].Trim();
                Table = values[1].Trim();
            }

            GetColumnsFromTable();


            if (connection.State == System.Data.ConnectionState.Open)
                connection.Close();

            return true;

        }
        public void GetColumnsFromSQL()
        {
            columns.Clear();
        }
        public void GetColumnsFromTable()
        {
            columns.Clear();

            if (connection.State != System.Data.ConnectionState.Open)
                connection.Open();

            var schemaTable = connection.GetSchema("Columns");
            foreach (DataRow row in schemaTable.Rows)
            {

                if (SchemaName == "")
                {
                    if (row["TABLE_NAME"].ToString().ToLower() == Table.ToLower())
                    {
                        columns.Add(row["COLUMN_NAME"].ToString());
                    }

                }
                else
                {

                    if (row["TABLE_NAME"].ToString().ToLower() == Table.ToLower() && row["TABLE_SCHEMA"].ToString().ToLower() == SchemaName.ToLower())
                    {
                        columns.Add(row["COLUMN_NAME"].ToString());
                    }

                }
            }

            if (connection.State == System.Data.ConnectionState.Open)
                connection.Close();
        }

        public void LoadDataGrid(DataGrid dgCustomerSearch, string[] argColumns, int[] argWidths, string[] argHeaders)
        {
            //Example Usage in Code
            //string[] aColumns = { "customer_id", "last_name", "first_name", "phone", "street", "city", "state", "zip_code" };
            //int[] aWidths = { 40, 150, 150, 150, 150, 225, 225, 225 };
            //string[] aHeaders = { "#", "Last Name", "First Name", "Phone", "Street", "City", "State", "Zip" };

            //dataCollection.LoadDataGrid(dgCustomerSearch, aColumns, aWidths, aHeaders);

            if (dgCustomerSearch.Columns.Count == 0)
            {

                DataGridTextColumn column;

                for (int i = 0; i < argColumns.Length; i++)
                {
                    column = new DataGridTextColumn
                    {
                        Header = argHeaders[i],
                        Binding = new Binding(argColumns[i]),
                        Width = argWidths[i],

                    };

                    if (i == 0)
                        column.Visibility = Visibility.Collapsed;

                    dgCustomerSearch.Columns.Add(column);

                }

                dgCustomerSearch.AutoGenerateColumns = false;
            }

        }


        public bool CheckForChanges()
        {

            bool blnchanged = false;

            for (int i = 0; i < dt.Columns.Count; i++)
            {

                if (prevcolumnvalue.ContainsKey(dt.Columns[i].ColumnName.ToString()))
                {

                    string prev = prevcolumnvalue[dt.Columns[i].ColumnName.ToString()].ToString();

                    if (prev.ToString() != dt.Rows[0].ItemArray[i].ToString())
                    {


                        blnchanged = true;


                    }
                }

            }


            if (blnchanged)
            {
                MessageBoxResult result = MessageBox.Show("Changes have been made.  Would you like to Save?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    UpdateRow();
                }


            }



            return blnchanged;

        }

        public void AddNewRecord()
        {
            App.Current.Properties[PrimeKey] = "-1";
            PrimeKeyValue = null;
            //dt.Rows.Add(PrimeKey);

        }
        public bool UpdateRow()
        {


            //Check for New Record and Insert
            if (prevcolumnvalue.Count == 0 || PrimeKeyValue is null || PrimeKeyValue == "-1" || PrimeKeyValue == "")
            {
                return InsertRow();


            }



            bool blnupdate = false;
            UpdateSet = "";
            for (int i = 0; i < dt.Columns.Count; i++)
            {

                if (prevcolumnvalue.ContainsKey(dt.Columns[i].ColumnName.ToString()))
                {

                    string prev = prevcolumnvalue[dt.Columns[i].ColumnName.ToString()].ToString();

                    if (prev.ToString() != dt.Rows[0].ItemArray[i].ToString())
                    {

                        prevcolumnvalue[dt.Columns[i].ColumnName.ToString()] = dt.Rows[0].ItemArray[i].ToString();

                        string updateValue = dt.Rows[0].ItemArray[i].ToString();
                        if (updateValue.IndexOf("'") > -1)
                        {
                            updateValue = updateValue.Replace("'", "''");
                        }

                        if (UpdateSet == "")
                            UpdateSet = dt.Columns[i].ColumnName.ToString() + "='" + updateValue + "'";
                        else
                            UpdateSet = UpdateSet + "," + dt.Columns[i].ColumnName.ToString() + "='" + updateValue + "'";

                        blnupdate = true;


                    }
                }

            }
            if (blnupdate)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "UPDATE " + TableName + " SET " + UpdateSet + " WHERE " + PrimeKey + " = '" + PrimeKeyValue + "'";

                    //MessageBox.Show(query);

                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        //command.Parameters.AddWithValue("@value1", "new_value");
                        //command.Parameters.AddWithValue("@value2", "old_value");
                        try { command.ExecuteNonQuery(); }
                        catch
                        {
                            //MessageBox.Show("Record could not be updated. You are missing information.");
                            MessageBox.Show("Error: Record could not be updated. Please check your fields and try again.");
                            return false;

                        }
                    }
                    if (connection.State == System.Data.ConnectionState.Open)
                        connection.Close();
                }
            }



            MessageBox.Show("Changes saved.");
            return true;

        }

        public bool InsertRow()
        {

            bool blnupdate = false;
            string InsertColumns = "";
            string InsertValues = "";
            for (int i = 0; i < dt.Columns.Count; i++)
            {

                if (prevcolumnvalue.ContainsKey(dt.Columns[i].ColumnName.ToString()) == false)
                {

                    prevcolumnvalue.Add(dt.Columns[i].ColumnName.ToString(), dt.Rows[0].ItemArray[i].ToString());

                }
                else
                {
                    prevcolumnvalue[dt.Columns[i].ColumnName.ToString()] = dt.Rows[0].ItemArray[i].ToString();
                }


                if (dt.Rows[0].ItemArray[i].ToString() != "" && dt.Columns[i].ColumnName.ToString().ToLower() != PrimeKey.ToLower())
                {



                    string updateValue = dt.Rows[0].ItemArray[i].ToString();
                    if (updateValue.IndexOf("'") > -1)
                    {
                        updateValue = updateValue.Replace("'", "''");
                    }


                    if (InsertColumns == "")
                    {
                        InsertValues = "'" + updateValue + "'";
                        InsertColumns = dt.Columns[i].ColumnName.ToString();
                    }
                    else
                    {
                        InsertValues = InsertValues + ",'" + updateValue + "'";
                        InsertColumns = InsertColumns + "," + dt.Columns[i].ColumnName.ToString();
                    }

                    blnupdate = true;


                }


            }
            if (blnupdate)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "INSERT INTO " + TableName + " (" + InsertColumns + ") VALUES (" + InsertValues + ")";

                    //MessageBox.Show(query);

                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        //command.Parameters.AddWithValue("@value1", "new_value");
                        //command.Parameters.AddWithValue("@value2", "old_value");

                        try { command.ExecuteNonQuery(); }
                        catch
                        {
                            MessageBox.Show("Record Can Not Be Added. You are Missing Information!");
                            return false;

                        }
                    }
                    if (connection.State == System.Data.ConnectionState.Open)
                        connection.Close();
                }
            }

            //MessageBox.Show("Changes saved.");
            return true;

        }
        public string GetColumnByNameForLastRecord(string columnName)
        {
            return dt.Rows[dt.Rows.Count - 1][columnName].ToString();
        }
        public string GetColumnByName(string columnName)
        {
            return dt.Rows[0][columnName].ToString();
        }
        public string[] GetAllDataInColumnByName(string columnName)
        {
            string[] strings = new string[dt.Rows.Count];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                strings[i] = dt.Rows[i][columnName].ToString();
            }
            return strings;
        }

        public void SetColumnByName(string columnName, string value)
        {
            dt.Rows[0][columnName] = value;
        }


        public int GetColumnIntegarByName(string columnName)
        {
            return (int)dt.Rows[0][columnName];
        }

        public bool GetColumnBooleanByName(string columnName)
        {
            return (bool)dt.Rows[0][columnName];
        }

        public string GetColumnByIndex(int columnNumber)
        {
            return dt.Rows[0][columnNumber].ToString();
        }

        public bool FoundRows()
        {
            if (dt.Rows.Count == 0)
            {
                return false;
            }
            return true;
        }

        public bool ExecuteSQL(string SQL)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = SQL;

                //MessageBox.Show(query);

                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    //command.Parameters.AddWithValue("@value1", "new_value");
                    //command.Parameters.AddWithValue("@value2", "old_value");

                    try { command.ExecuteNonQuery(); }
                    catch
                    {
                        return false;
                    }




                }
                if (connection.State == System.Data.ConnectionState.Open)
                    connection.Close();
            }
            return true;
        }

        public bool DeleteRow()
        {

            bool blnupdate = false;

            MessageBoxResult result = MessageBox.Show("Are you sure you want to remove this employee?\nALL TIMECARD AND PAY DATA WILL BE LOST.", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                blnupdate = true;
            }


            if (blnupdate && PrimeKeyValue != "")
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "DELETE FROM " + TableName + " WHERE " + PrimeKey + " = '" + PrimeKeyValue + "'";

                    //MessageBox.Show(query);

                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        //command.Parameters.AddWithValue("@value1", "new_value");
                        //command.Parameters.AddWithValue("@value2", "old_value");

                        try { command.ExecuteNonQuery(); }
                        catch
                        {
                            MessageBox.Show("Record could not be deleted.");
                            return false;

                        }




                    }
                    if (connection.State == System.Data.ConnectionState.Open)
                        connection.Close();
                }
                //MessageBox.Show("Record removed.");
                return true;
            }

            return false;

        }

        public bool LoadDataTableFromSQL(string argSQL = null)
        {

            try
            {





                connection.Open();

                if (argSQL != null)
                {
                    query = argSQL;
                }



                dt = new DataTable(TableName);

                dataCollection = new ObservableCollection<DataTable>();

                using (SqlCommand command = new SqlCommand(query, connection))
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    columns.Clear();
                    int fieldCount = reader.FieldCount;
                    for (int i = 0; i < fieldCount; i++)
                    {
                        columns.Add(reader.GetName(i));

                    }

                    for (int i = 0; i < columns.Count; i++)
                    {
                        dt.Columns.Add(columns[i], typeof(string));
                    }

                    bool found = false;

                    while (reader.Read())
                    {
                        found = true;
                        DataRow row1 = dt.NewRow();
                        prevcolumnvalue.Clear();

                        for (int i = 0; i < columns.Count; i++)
                        {
                            if (ColumnExists(reader, columns[i]))
                            {
                                if (PrimeKey.ToLower() == columns[i].ToLower())
                                {
                                    PrimeKeyValue = reader[columns[i]].ToString();
                                }

                                row1[columns[i]] = reader[columns[i]].ToString();
                                prevcolumnvalue.Add(columns[i], row1[columns[i]].ToString());
                            }


                        }

                        dt.Rows.Add(row1);

                    }

                    if (found == false)
                    {

                        DataRow row1 = dt.NewRow();
                        prevcolumnvalue.Clear();

                        for (int i = 0; i < columns.Count; i++)
                        {



                            prevcolumnvalue.Add(columns[i], row1[columns[i]].ToString());



                        }

                        dt.Rows.Add(row1);





                    }



                    if (dt.Rows.Count == 0)
                    {

                        DataRow row1 = dt.NewRow();
                        dt.Rows.Add(row1);


                    }



                }
            }
            catch (Exception ex)
            {
                // Handle exceptions or log errors here
            }
            finally
            {
                if (connection.State == System.Data.ConnectionState.Open)
                    connection.Close();
            }

            return true;
        }

        private bool ColumnExists(SqlDataReader reader, string columnName)
        {
            var schemaTable = reader.GetSchemaTable();
            foreach (DataRow row in schemaTable.Rows)
            {
                if (row["ColumnName"].ToString() == columnName)
                {
                    return true;
                }
            }
            return false;
        }
        public void RefreshTable()
        {
            LoadDataTableFromSQL();

        }
        public bool LoadDataTable(string argFilter)
        {

            try
            {



                string query = "";

                connection.Open();


                query = "SELECT * FROM " + TableName + " WHERE " + argFilter;

                dt = new DataTable(TableName);


                for (int i = 0; i < columns.Count; i++)
                {
                    dt.Columns.Add(columns[i], typeof(string));
                }


                dataCollection = new ObservableCollection<DataTable>();


                using (SqlCommand command = new SqlCommand(query, connection))
                using (SqlDataReader reader = command.ExecuteReader())
                {



                    while (reader.Read())
                    {

                        DataRow row1 = dt.NewRow();
                        for (int i = 0; i < columns.Count; i++)
                        {
                            row1[columns[i]] = reader[columns[i]].ToString();
                        }
                        dt.Rows.Add(row1);

                    }



                }
            }
            catch (Exception ex)
            {
                // Handle exceptions or log errors here
            }
            finally
            {
                if (connection.State == System.Data.ConnectionState.Open)
                    connection.Close();
            }

            return true;
        }

        public string getStoreCondition()
        {

            if (TableName.ToUpper() == "CUSTOMER")
            {
                return " AND STORE = '" + Globals.StoreNo + "' ";
            }

            return "";
        }



        public string GetColumnByRowID(string id, string columnName)
        {
            DataRow[] foundRows = dt.Select(PrimeKey + " = " + id);

            // Check if any rows were found
            if (foundRows.Length > 0)
            {
                // Access the first found row (in this example, there should be only one)
                DataRow row = foundRows[0];


                return (string)row[columnName];

            }


            return "";
        }



    }
}






