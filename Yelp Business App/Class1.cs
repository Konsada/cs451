using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Windows.Forms;

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
            }catch(MySqlException ex)
            {
                MessageBox.Show(ex.Message);
                // handle exception here
            }
        }

        // Initialize Connection - sets all connection parameters, creates connection string and calls MySql_Connection constructor
        private void Initialize()
        {
            server = "127.0.0.1";
            database = "milestone1db";
            uid = "root";
            password = "Kahle$$0217";
            // server=127.0.0.1;user id=root;password=Kahle$$0217;persistsecurityinfo=True;database=milestone1db
            string connectionString = "server=" + server + ";" + "user id=" + uid + ";" + "password=" + password + ";" + "persistsecurityinfo=True;"+ "database=" + database + ";";
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
            catch(MySqlException ex)
            {
                MessageBox.Show(ex.Message);
                // handle exception here
            }
            return false;
        }

        // Execute SELECT Query - return single attribute

        public List<String> SQLSELECTExec(string querySTR, string column_name)
        {
            List<String> qResult = new List<String>();
            
            if(this.OpenConnection() == true) //open the connection
            {
                MySqlCommand cmd = new MySqlCommand(querySTR, connection);
                MySqlDataReader dataReader = cmd.ExecuteReader();
                while(dataReader.Read())
                {
                    qResult.Add(dataReader.GetString(column_name));
                }
                //close the reader
                dataReader.Close();
                //close the connection
                this.CloseConnection();
            }
            return qResult;
        }

        public String QueryZipcode(string column_name, string zip)
        {
            string qstr = "SELECT " + column_name + " FROM censusdata WHERE zipcode = " + "'" + zip + "';";

            List<String> qResult = SQLSELECTExec(qstr, column_name);

            return qResult[0];
        }
    }
}
