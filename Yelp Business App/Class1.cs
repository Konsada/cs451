using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Windows.Forms;
using System.Data;

namespace Yelp_Business_App
{
    class MySql_Connection
    {
        private MySqlConnection connection; // used to open and close connection the database
        public string server;
        public string database;
        public string uid;
        public string password;


        //Constructor calls initialize function that establishes database connection
        public MySql_Connection()
        {
            try
            {
                Initialize();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
                // handle exception here
            }
        }

        // Initialize Connection - sets all connection parameters, creates connection string and calls MySql_Connection constructor
        private void Initialize()
        {
            server = "127.0.0.1";
            database = "milestone2db";
            uid = "root";
            password = "password";
            // server=127.0.0.1;user id=root;password=Kahle$$0217;persistsecurityinfo=True;database=milestone1db
            string connectionString = "server=" + server + ";" + "user id=" + uid + ";" + "password=" + password + ";" + "persistsecurityinfo=True;" + "database=" + database + ";";
            connection = new MySqlConnection(connectionString);
        }

        public void NewConnection(string newserv, string newdb, string newuid, string newpw)
        {
            server = newserv;
            database = newdb;
            uid = newuid;
            password = newpw;
            string connectionString = "server=" + server + ";" + "user id=" + uid + ";" + "password=" + password + ";" + "persistsecurityinfo=True;" + "database=" + database + ";";
            connection = new MySqlConnection(connectionString);
        }
        //open connection to DB
        private bool OpenConnection()
        {
            try
            {
                connection.Open();
                return true;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
                if (ex.Number == 0)
                {
                    return false;//Can't Connect to server
                }
                else if (ex.Number == 1045)
                {
                    return false;//Invalid Username/Password
                }
                //handle other exceptions as well
                return false;
            }
        }

        //close connection
        public bool CloseConnection()
        {
            try
            {
                connection.Close();
                return true;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
                // handle exception here
            }
            return false;
        }
        public List<String> SQLTABLEExec(string querySTR)
        {
            List<String> qResult = new List<String>();

            if (OpenConnection() == true)
            {
                DataTable schemaTable = new DataTable();
                Dictionary<String, bool> attDict = new Dictionary<string, bool>();
                MySqlCommand cmd = new MySqlCommand(querySTR, connection);
                MySqlDataReader dataReader = cmd.ExecuteReader();

                schemaTable = dataReader.GetSchemaTable();
                foreach (DataRow row in schemaTable.Rows)
                {
                    attDict.Add(row[0].ToString(), false);
                }
                while (dataReader.Read())
                {
                    for (int i = 0; i < attDict.Keys.Count; i++)
                    {
                        if (!dataReader.IsDBNull(i))
                        {
                            String str = dataReader.GetString(attDict.Keys.ElementAt(i));
                            if (null != str && str != "0")
                            {
                                attDict[attDict.Keys.ElementAt(i)] = true;
                            }
                        }
                    }
                }
                foreach (String str in attDict.Keys)
                {
                    if (attDict[str])
                    {
                        qResult.Add(str);
                    }
                }
                dataReader.Close();
                CloseConnection();
                //qResult = attDict.Keys.ToList<String>();
            }
            return qResult;
        }

        // Execute SELECT Query - return single attribute

        public List<String> SQLSELECTExec(string querySTR, string column_name)
        {
            List<String> qResult = new List<String>();

            if (this.OpenConnection() == true) //open the connection
            {
                MySqlCommand cmd = new MySqlCommand(querySTR, connection);
                MySqlDataReader dataReader = cmd.ExecuteReader();
                while (dataReader.Read())
                {
                    qResult.Add(dataReader.GetString(column_name));
                }
                //close the reader
                dataReader.Close();
                //close the connection
                CloseConnection();
            }
            return qResult;
        }
        public BindingSource SQLDATATABLEExec(string querySTR)
        {
            if(OpenConnection() == true)
            {
                MySqlDataAdapter dataAdapter = new MySqlDataAdapter();
                dataAdapter.SelectCommand = new MySqlCommand(querySTR, connection);
                dataAdapter.SelectCommand.CommandTimeout = 180;
                DataTable table = new DataTable();
                try
                {
                    dataAdapter.Fill(table);
                }
                catch(Exception e)
                {
                    MessageBox.Show(e.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                

                BindingSource bSource = new BindingSource();
                bSource.DataSource = table;

                //dataAdapter.Dispose();
                CloseConnection();
                return bSource;
            }
            return null;
        }
        public int SQLCOUNTExec(string querySTR)
        {
            int qResult = 0;
            if (this.OpenConnection() == true)
            {
                MySqlCommand cmd = new MySqlCommand(querySTR, connection);
                MySqlDataReader dataReader = cmd.ExecuteReader();
                while (dataReader.Read())
                {
                    qResult = dataReader.GetInt32(0);
                }
                dataReader.Close();
                this.CloseConnection();
            }
            return qResult;
        }
        public double SQLAVGExec(string querySTR)
        {
            double qResult = 0;
            if (this.OpenConnection() == true)
            {
                MySqlCommand cmd = new MySqlCommand(querySTR, connection);
                MySqlDataReader dataReader = cmd.ExecuteReader();
                while (dataReader.Read())
                {
                    qResult = dataReader.GetDouble(0);
                }
                dataReader.Close();
                this.CloseConnection();
            }
            return qResult;
        }

        public String QueryZipcode(string column_name, string zip)
        {
            string qstr = "SELECT " + column_name + " FROM demographics WHERE zipcode = " + "'" + zip + "';";

            List<String> qResult = SQLSELECTExec(qstr, column_name);

            return qResult[0];
        }

        public Dictionary<string, double> QueryState(string state)
        {
            string qstr = "SELECT AVG(under18years), AVG(18_to_24years), AVG(25_to_44years), AVG(45_to_64years), AVG(65_and_over) FROM demographics WHERE state_code = " + "'" + state + "';";
            Dictionary<string, double> qResult = new Dictionary<string, double>()
            {
                {"AVG(under18years)", 0.0 },
                {"AVG(18_to_24years)", 0.0 },
                {"AVG(25_to_44years)", 0.0 },
                {"AVG(45_to_64years)", 0.0 },
                {"AVG(65_and_over)", 0.0 }
            };
            if (this.OpenConnection() == true)
            {
                MySqlCommand cmd = new MySqlCommand(qstr, connection);
                MySqlDataReader dataReader = cmd.ExecuteReader();
                int i = 0;
                while (dataReader.Read())
                {
                    if (!dataReader.IsDBNull(i))
                    {
                        for (int j = 0; j < qResult.Keys.Count; j++)
                        {
                            string column_name = dataReader.GetName(j);
                            double temp = 0.0;
                            double.TryParse(dataReader.GetString(column_name), out temp);
                            qResult[qResult.Keys.ElementAt(j)] = temp;
                        }

                    }
                    i++;
                }
                dataReader.Close();
                this.CloseConnection();
            }

            return qResult;
        }

        public Dictionary<string, double> QueryCity(string state, string city)
        {
            string qstr = "SELECT AVG(under18years), AVG(18_to_24years), AVG(25_to_44years), AVG(45_to_64years), AVG(65_and_over) FROM demographics WHERE state_code = " + "'" + state + "' AND city = " + "'" + city + "';";
            Dictionary<string, double> qResult = new Dictionary<string, double>()
            {
                {"AVG(under18years)", 0.0 },
                {"AVG(18_to_24years)", 0.0 },
                {"AVG(25_to_44years)", 0.0 },
                {"AVG(45_to_64years)", 0.0 },
                {"AVG(65_and_over)", 0.0 }
            };
            if (this.OpenConnection() == true)
            {
                MySqlCommand cmd = new MySqlCommand(qstr, connection);
                MySqlDataReader dataReader = cmd.ExecuteReader();
                int i = 0;
                while (dataReader.Read())
                {
                    if (!dataReader.IsDBNull(i))
                    {
                        for (int j = 0; j < qResult.Keys.Count; j++)
                        {
                            string column_name = dataReader.GetName(j);
                            double temp = 0.0;
                            double.TryParse(dataReader.GetString(column_name), out temp);
                            qResult[qResult.Keys.ElementAt(j)] = temp;
                        }
                    }
                    i++;
                }
                dataReader.Close();
                this.CloseConnection();
            }
            return qResult;
        }
        public List<string> QueryBusinessSearch(string qStr)
        {
            List<string> qResult = new List<string>();
            if (this.OpenConnection() == true)
            {
                MySqlCommand cmd = new MySqlCommand(qStr, connection);
                MySqlDataReader dataReader = cmd.ExecuteReader();
                while (dataReader.Read())
                {
                    for(int i = 0; i < dataReader.FieldCount; i++)
                    {
                        qResult.Add(dataReader[i].ToString());
                    }
                }
                dataReader.Close();
                this.CloseConnection();
            }
            return qResult;

        }
        public List<String> SQLSELECTExec(string querySTR)
        {
            List<String> qResult = new List<string>();
            if(OpenConnection() == true)
            {
                MySqlCommand cmd = new MySqlCommand(querySTR, connection);
                MySqlDataReader dataReader = cmd.ExecuteReader();
                while (dataReader.Read())
                {
                    qResult.Add(dataReader.GetString(0));
                }

                dataReader.Close();
                CloseConnection();
            }
            return qResult;
        }
    }
}
