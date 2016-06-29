using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

//NOTES FOR ADDITIONAL FUNCTIONALITY: 
// 1. User select business.json, user.json, review.json, etc.
// 2. Create DBs into connected server user pics
//    2a.  Check if DBs already exist
//    2b.  Insert all info into DBs
//    2c.  Create Update/Trigger functions for db 
// 3. Add entries to db through app


namespace Yelp_Business_App
{
    public partial class Form1 : Form
    {
        MySql_Connection mydb;
        List<string> categories;
        Dictionary<string, string> attributeDict = new Dictionary<string, string>();
        string localDir = ".\\..\\..\\yelp\\";

        public Form1()
        {
            init();
            mydb = new MySql_Connection("localhost", "root", "password");
            databaseInit("db");
            tablesInit();
            mydb.demographicsTableName = "demographics";
            demographicInit();
            populateTables();
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            UpdateStateComboBoxes();
            initUsernames();
            intitCategories();
            initControls();
        }
        void init()
        {
            //businessInit();
            //reviewInit();
            //userInit();
        }
        protected void initControls()
        {
            businessSearchResultsDataGridView.AutoResizeColumns();
            businessSearchResultsDataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            businessSearchResultsDataGridView.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomCenter;
            businessSearchResultsDataGridView.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.False;
            stateDataGridView.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomCenter;
            stateDataGridView.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.False;
            cityDataGridView.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomCenter;
            cityDataGridView.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.False;
            zipcodeDataGridView.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomCenter;
            zipcodeDataGridView.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.False;
            zipcodeDataGridView.AutoResizeColumns();
            zipcodeDataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            zipcodeDataGridView.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            /****** INIT BUSINESS SEARCH TAB *****/
            if (minRatingComboBox.Items.Count < 5 || maxRatingComboBox.Items.Count < 5)
            {
                minRatingComboBox.Items.Clear();
                maxRatingComboBox.Items.Clear();

                for (int i = 0; i <= 5; i++)
                {
                    minRatingComboBox.Items.Add(i);
                    maxRatingComboBox.Items.Add(i);
                }
            }
            if (minReviewsComboBox.Items.Count == 0 && maxReviewsComboBox.Items.Count == 0)
            {
                minReviewsComboBox.Items.Add(0);
                minReviewsComboBox.Items.Add(5);
                minReviewsComboBox.Items.Add(10);
                minReviewsComboBox.Items.Add(50);
                minReviewsComboBox.Items.Add(100);
                minReviewsComboBox.Items.Add(500);
                minReviewsComboBox.Items.Add("1000 +");
                maxReviewsComboBox.Items.Add(0);
                maxReviewsComboBox.Items.Add(5);
                maxReviewsComboBox.Items.Add(10);
                maxReviewsComboBox.Items.Add(50);
                maxReviewsComboBox.Items.Add(100);
                maxReviewsComboBox.Items.Add(500);
                maxReviewsComboBox.Items.Add("1000 +");
            }
            if (!(attributeTreeView.Nodes.Count > 0))
            {
                string qstr = "SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'attributes';";
                attributesList attributeTree = new attributesList();
                foreach (string s in attributeTree.list)
                {
                    attributeTreeView.Nodes.Add(s);
                }
            }
            List<string> query = mydb.SQLSELECTExec("SELECT DISTINCT uid FROM Users;");
            foreach (string s in query)
                username.Items.Add(s);
            hour.Hide();
        }
        protected void intitCategories()
        {
            string qStr = "SELECT DISTINCT name FROM categories";
            categories = mydb.SQLSELECTExec(qStr, "name");
            foreach (string s in categories)
            {
                categoriesBusinessSearchListBox.Items.Add(s);
                categoriesListBox.Items.Add(s);
            }
        }
        protected void businessInit()
        {
            String dataDir = ".\\..\\..\\yelp\\";
            //int count = 0;
            string contents;
            using (System.IO.StreamReader jsonFile = new System.IO.StreamReader(dataDir + "yelp_business.json"))
            {
                using (System.IO.StreamWriter outfile = new System.IO.StreamWriter(dataDir + "business.sql"))
                {
                    StringBuilder sb = new StringBuilder();
                    StringBuilder ab = new StringBuilder();
                    StringBuilder cb = new StringBuilder();
                    StringBuilder hb = new StringBuilder();
                    int count = 0;

                    while ((contents = jsonFile.ReadLine()) != null)
                    {
                        Business biz = JsonConvert.DeserializeObject<Business>(contents);
                        //HashSet <string> atts = new HashSet<string>();
                        if (count % 15000 == 0)
                        {
                            if (count > 0)
                            {
                                sb.Append(";\r\n");
                                cb.Append(";\r\n");
                                hb.Append(";\r\n");
                                outfile.Write(sb);
                                outfile.Write(cb);
                                sb.Clear();
                                cb.Clear();
                                //atts.Clear();
                            }
                            sb.Append(biz.writeBiz());
                            //biz.hashAttributes(atts);
                            ab.Append(biz.ab);
                            cb.Append(biz.cb);
                            hb.Append(biz.hb);
                        }
                        else
                        {
                            sb.Append(biz.writeBizValue());
                            //biz.hashAttributes(atts);
                            ab.Append(biz.ab);
                            cb.Append(biz.cb);
                            hb.Append(biz.hb);
                        }
                        count++;
                    }
                    sb.Append(";\r\n");
                    cb.Append(";\r\n");
                    hb.Append(";\r\n");

                    outfile.Write(sb);
                    outfile.Write(cb);
                    outfile.Write(ab);
                    outfile.Write(hb);
                }
            }
        }
        protected void reviewInit()
        {
            String dataDir = ".\\..\\..\\yelp\\";
            int count = 0;
            string contents;
            using (System.IO.StreamReader jsonFile = new System.IO.StreamReader(dataDir + "yelp_review.json"))
            {
                using (System.IO.StreamWriter outfile = new System.IO.StreamWriter(dataDir + "review.sql"))
                {
                    StringBuilder sb = new StringBuilder();

                    while ((contents = jsonFile.ReadLine()) != null)
                    {
                        Review rev = JsonConvert.DeserializeObject<Review>(contents);
                        if (count % 30000 == 0)
                        {
                            if (count > 0)
                            {
                                sb.Append(";\r\n");
                                outfile.Write(sb);
                                sb.Clear();
                            }
                            sb.Append(rev.writeRev());
                        }
                        else
                        {
                            sb.Append(rev.writeRevValue());
                        }
                        count++;
                    }
                    sb.Append(";\r\n");
                    outfile.Write(sb);
                }
            }
        }
        protected void userInit()
        {
            String dataDir = ".\\..\\..\\yelp\\";
            int count = 0;
            string contents;
            using (System.IO.StreamReader jsonFile = new System.IO.StreamReader(dataDir + "yelp_user.json"))
            {
                StringBuilder fb = new StringBuilder();

                using (System.IO.StreamWriter outfile = new System.IO.StreamWriter(dataDir + "user.sql"))
                {
                    StringBuilder sb = new StringBuilder();
                    System.IO.StreamWriter friendfile = new System.IO.StreamWriter(dataDir + "friend.sql");
                    contents = jsonFile.ReadLine();
                    while ((contents = jsonFile.ReadLine()) != null)
                    {
                        User uzr = JsonConvert.DeserializeObject<User>(contents);

                        if (count % 500000 == 0)
                        {
                            if (count > 0)
                            {
                                sb.Append(";\r\n");
                                fb.Append(";\r\n");
                            }
                            sb.Append(uzr.writeUzr());
                            fb.Append(uzr.fb);
                        }
                        else
                        {
                            sb.Append(uzr.writeUzrValue());
                            fb.Append(uzr.fb);
                        }
                        count++;
                    }
                    sb.Append(";\r\n");
                    fb.Append(";\r\n");
                    outfile.Write(sb);
                    friendfile.Write(fb);
                    friendfile.Close();
                }
            }
        }
        protected void demographicsFileInit()
        {
            List<Demographics> dems = new List<Demographics>();
            string demographicCreatorFileName = "demographics.csv";
            string demographicDir = "./../../yelp/" + demographicCreatorFileName;
            using(var reader = new StreamReader(File.OpenRead(demographicDir)))
            {
                int i = 0;
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (i > 0)
                    {
                        Demographics dem = new Demographics();
                        var values = line.Split(',');

                        int.TryParse(values[0], out dem.zipcode);
                        dem.state = values[1];
                        dem.state_code = values[2];
                        dem.city = values[3];
                        int.TryParse(values[4], out dem.population);
                        double.TryParse(values[5], out dem.under18);
                        double.TryParse(values[6], out dem._18to24);
                        double.TryParse(values[7], out dem._25to44);
                        double.TryParse(values[8], out dem._45to64);
                        double.TryParse(values[9], out dem._65andover);
                        int.TryParse(values[10], out dem.median_age);
                        double.TryParse(values[11], out dem.percentage_of_females);
                        int.TryParse(values[12], out dem.num_employee);
                        double.TryParse(values[13], out dem.annual_payroll);
                        double.TryParse(values[14], out dem.avg_income);

                        dems.Add(dem);
                    }
                    i++;
                }
            }
            using(StreamWriter outfile = new StreamWriter("./../../yelp/dem.sql"))
            {
                outfile.WriteLine("INSERT INTO " + mydb.demographicsTableName + " (zipcode, state, state_code, city, population, under18, 18to24, 25to44, 45to64, 65andover, median_age, percentage_of_females, num_employee, annual_payroll, avg_income) VALUES ");
                foreach(Demographics dem in dems)
                {
                    if(dem != dems[dems.Count-1])
                    {
                        outfile.WriteLine("(" + dem.zipcode + ",\'" + dem.state + "\',\'" + dem.state_code + "\',\'" + dem.city + "\'," + dem.population.ToString() + "," +
                            dem.under18.ToString() + "," + dem._18to24.ToString() + "," + dem._25to44.ToString() + "," + dem._45to64.ToString() + "," +
                            dem._65andover.ToString() + "," + dem.median_age.ToString() + "," + dem.percentage_of_females.ToString() + "," + dem.num_employee.ToString() + "," +
                            dem.annual_payroll.ToString() + "," + dem.avg_income.ToString() + "),\n");
                    }
                    else
                    {
                        outfile.WriteLine("(" + dem.zipcode + ",\'" + dem.state + "\',\'" + dem.state_code + "\',\'" + dem.city + "\'," + dem.population.ToString() + "," +
                            dem.under18.ToString() + "," + dem._18to24.ToString() + "," + dem._25to44.ToString() + "," + dem._45to64.ToString() + "," +
                            dem._65andover.ToString() + "," + dem.median_age.ToString() + "," + dem.percentage_of_females.ToString() + "," + dem.num_employee.ToString() + "," +
                            dem.annual_payroll.ToString() + "," + dem.avg_income.ToString() + ");");
                    }
                }
            }
        }
        protected void tablesInit() // MYSQL syntax error
        {
            string tableCreatorFileName = "Tables.sql";
            string tableDir = "./../../yelp/" + tableCreatorFileName;
            string str = File.ReadAllText(tableDir);
            //string str = @"source C:/Users/keons_000/Documents/Visual Studio 2015/Projects/Yelp Business App/Yelp Business App/yelp/Tables.sql;";

            mydb.SQLNonQueryExec(str);
            //string demographicsFileName = "dem.sql";
            //string demographicsDir = "./../../yelp/" + demographicsFileName;
            //string permission = "--secure-file-priv=" + Path.GetFullPath("./../../yelp").Replace("\\","/");
            //mydb.SQLNonQueryExec(permission);
            //str = @"source C:/Users/keons_000/Documents/Visual Studio 2015/Projects/Yelp Business App/Yelp Business App/yelp/dem.sql";
            //str = File.ReadAllText(demographicsDir);
            //str = "source " + Path.GetFullPath(demographicsDir);
            // file path is too long! Max: 99
            // cannot end "source filename" with ';'
            //str = @"source C:/Downloads/dem.sql;";
            
            //mydb.SQLNonQueryExec(str);

        }
        protected void databaseInit(string newdatabase)
        {
            mydb.SQLNonQueryExec("DROP DATABASE IF EXISTS " + newdatabase);
            mydb.SQLNonQueryExec("CREATE DATABASE IF NOT EXISTS " + newdatabase);
            mydb.NewConnection(mydb.server, newdatabase, mydb.uid, mydb.password);
            mydb.SQLNonQueryExec("USE " + newdatabase);
        }
        /// <summary>
        /// Might need to do this manually, calling SQL infile comand takes wayyyy too long
        /// </summary>
        protected void demographicInit()
        {
            demographicsFileInit();
            string demographicFileName = "dem.sql";
            string demographicFileLocation = "./../../yelp/" + demographicFileName;
            //StringBuilder str = new StringBuilder();
            //str.Append(File.ReadAllText(demographicFileLocation));
            //string str = "SOURCE " + demographicFileLocation;
            //mydb.SQLNonQueryExec(str.ToString());

            mydb.SQLSourceFile(demographicFileLocation);

            //string absolutePath = Path.GetFullPath(demographicFileLocation);
            /*
            mydb.SQLNonQueryExec("LOAD DATA LOCAL INFILE '" + absolutePath + "' \nINTO TABLE " + mydb.database + "." + mydb.demographicsTableName + 
                " \nFIELDS TERMINATED BY ',' \nENCLOSED BY '\"' \nLINES TERMINATED BY \'\\r\\n\' \nIGNORE 1 LINES " +
                "\n(zipcode, state, state_code, city, population, under18years, 18_to_24years, 25_to_44years, 45_to_64years, " + 
                "65_and_over, median_age, percentage_of_females, num_employee, annual_payroll, avg_income)", 999999);
            */
        }
        protected void populateTables()
        {
            populateBusinessesTable();
            populateUsersTable();
            populateReviewsTable();
            populateFriendTable();
        }
        protected void populateBusinessesTable()
        {
            string str = localDir + "business.sql";
            mydb.SQLSourceFile(str, 9999);
        }
        protected void populateUsersTable()
        {
            string str = localDir + "user.sql";
            mydb.SQLSourceFile(str, 9999);
        }
        protected void populateReviewsTable()
        {
            string str = localDir + "review.sql";
            mydb.SQLSourceFile(str, 9999);
        }
        protected void populateFriendTable()
        {
            string str = localDir + "friend.sql";
            mydb.SQLSourceFile(str, 9999);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            string qstr = "SELECT distinct state_code FROM " + mydb.demographicsTableName + " ORDER BY state_code;";
            //execute query
            List<String> qResult = mydb.SQLSELECTExec(qstr, "maincategory");

            //copy query results to listBox1
            for (int i = 0; i < qResult.Count; i++)
            {
                cityListBox.Items.Add(qResult[i]);
            }
        }

        private void newServerMenuItem_Click(object sender, EventArgs e)
        {
            Form serverInfo = new Form();

            Label serverLabel = new Label();
            Label databaseLabel = new Label();
            Label uidLabel = new Label();
            Label pwLabel = new Label();

            TextBox serverBox = new TextBox();
            TextBox databaseBox = new TextBox();
            TextBox uidBox = new TextBox();
            TextBox pwBox = new TextBox();


            Button buttonOK = new Button();
            Button buttonCancel = new Button();


            serverLabel.Text = "Server:";
            databaseLabel.Text = "Database:";
            uidLabel.Text = "Username:";
            pwLabel.Text = "Password:";

            serverBox.Text = mydb.server;
            databaseBox.Text = mydb.database;
            uidBox.Text = mydb.uid;
            pwBox.Text = mydb.password;

            buttonOK.Text = "OK";
            buttonCancel.Text = "Cancel";
            buttonOK.DialogResult = DialogResult.OK;
            buttonCancel.DialogResult = DialogResult.Cancel;
            buttonOK.Click += (s, eArgs) => UpdateServerEvent(s, eArgs, serverBox.Text, databaseBox.Text, uidBox.Text, pwBox.Text);

            serverLabel.AutoSize = true;
            databaseLabel.AutoSize = true;
            uidLabel.AutoSize = true;
            pwLabel.AutoSize = true;
            buttonOK.AutoSize = true;
            buttonCancel.AutoSize = true;

            serverInfo.ClientSize = new Size(250, 200);
            serverInfo.Controls.AddRange(new Control[] { serverLabel, databaseLabel, uidLabel, pwLabel, serverBox, databaseBox, uidBox, pwBox, buttonOK, buttonCancel });
            serverInfo.AcceptButton = buttonOK;
            serverInfo.CancelButton = buttonCancel;

            serverLabel.SetBounds(10, 20, 30, 10);
            databaseLabel.SetBounds(10, 40, 30, 10);
            uidLabel.SetBounds(10, 60, 30, 10);
            pwLabel.SetBounds(10, 80, 30, 10);
            serverBox.SetBounds(100, 20, 100, 10);
            databaseBox.SetBounds(100, 40, 100, 10);
            uidBox.SetBounds(100, 60, 100, 10);
            pwBox.SetBounds(100, 80, 100, 10);

            buttonOK.SetBounds(100, 100, 50, 20);
            buttonCancel.SetBounds(150, 100, 50, 20);
            serverInfo.ShowDialog(this);
        }

        protected void UpdateServerEvent(object sender, EventArgs e, string serverName, string databaseName, string userName, string passwordName)
        {
            if (mydb.server != serverName || mydb.database != databaseName || mydb.uid != userName || mydb.password != passwordName)
            {
                try
                {
                    mydb.NewConnection(serverName, userName, passwordName);
                    mydb.database = databaseName;
                    //databaseInit(databaseName);
                    //tablesInit();
                    //demographicInit();
                    //populateTables();
                    intitCategories();
                    initControls();
                    UpdateStateComboBoxes();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Unable to connect to " + databaseName + " on " + serverName + " with username: " + userName + " password: " + passwordName);
                }
            }
        }
        protected void UpdateStateComboBoxes()
        {
            stateComboBox.Items.Clear();
            string qstr = "SELECT state_code FROM demographics GROUP BY state_code;";
            //execute query
            List<String> qResult = mydb.SQLSELECTExec(qstr, "state_code");

            //copy query results to listBox1
            for (int i = 0; i < qResult.Count; i++)
            {
                stateComboBox.Items.Add(qResult[i]);
            }
            for (int i = 0; i < qResult.Count; i++)
            {
                stateBusinessSearchComboBox.Items.Add(qResult[i]);
            }
        }
        protected void initUsernames()
        {
            string qstr = "SELECT DISTINCT uid FROM friends";
            List<String> qResult = mydb.SQLSELECTExec(qstr, "uid");
            foreach(string user in qResult)
            {
                username.Items.Add(user);
            }
        }
        private void comboBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            cityListBox.Items.Clear();
            zipcodeListBox.Items.Clear();

            zipcodePopulationTextBox.Clear();
            zipcodeAverageIncomeTextBox.Clear();
            zipcodeMedianAgeTextBox.Clear();
            zipcodeDataGridView.Rows.Clear();
            statePopulationTextBox.Clear();
            stateAverageIncomeTextBox.Clear();
            stateMedianAgeTextBox.Clear();
            stateDataGridView.Rows.Clear();
            cityPopulationTextBox.Clear();
            cityAverageIncomeTextBox.Clear();
            cityMedianAgeTextBox.Clear();
            cityDataGridView.Rows.Clear();

            string qstr = "SELECT city FROM " + mydb.demographicsTableName + " WHERE state_code = " + "'" + stateComboBox.SelectedItem.ToString() + "'" + "GROUP BY city;";

            List<String> qResult = mydb.SQLSELECTExec(qstr, "city");

            for (int i = 0; i < qResult.Count; i++)
            {
                cityListBox.Items.Add(qResult[i]);
            }

            qstr = "SELECT SUM(population) FROM " + mydb.demographicsTableName + " where state_code = " + "'" + stateComboBox.SelectedItem.ToString() + "'";

            string qPopulationResult = mydb.SQLCOUNTExec(qstr).ToString("N0");
            statePopulationTextBox.Text = qPopulationResult;

            qstr = "SELECT AVG(avg_income) FROM " + mydb.demographicsTableName + " WHERE state_code = " + "'" + stateComboBox.SelectedItem.ToString() + "'";
            string qAverageIncomeResult = mydb.SQLAVGExec(qstr).ToString("C2");
            stateAverageIncomeTextBox.Text = qAverageIncomeResult;

            qstr = "SELECT AVG(median_age) FROM " + mydb.demographicsTableName + " WHERE state_code = " + "'" + stateComboBox.SelectedItem.ToString() + "'";
            string qAverageMedianAgeResult = mydb.SQLAVGExec(qstr).ToString("N0");
            stateMedianAgeTextBox.Text = qAverageMedianAgeResult;

            stateDataGridView.Rows.Add(5);
            stateDataGridView.Rows[0].HeaderCell.Value = "Under 18 years";
            stateDataGridView.Rows[1].HeaderCell.Value = "18 to 24 years";
            stateDataGridView.Rows[2].HeaderCell.Value = "25 to 44 years";
            stateDataGridView.Rows[3].HeaderCell.Value = "45 to 64 years";
            stateDataGridView.Rows[4].HeaderCell.Value = "65 and over";

            Dictionary<string, double> qStateResults = mydb.QueryState(stateComboBox.SelectedItem.ToString());
            int j = 0;
            foreach (string s in qStateResults.Keys)
            {
                //stateDataGridView.Rows.Add(1);
                //stateDataGridView.Rows[j].HeaderCell.Value = s;
                stateDataGridView.Rows[j].Cells[0].Value = qStateResults[s].ToString("N2");
                j++;
            }
            stateDataGridView.AutoResizeRowHeadersWidth(DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders);

            initControls();
            //qstr = "SELECT "
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            zipcodeListBox.Items.Clear();
            cityPopulationTextBox.Clear();
            cityAverageIncomeTextBox.Clear();
            cityMedianAgeTextBox.Clear();
            zipcodePopulationTextBox.Clear();
            zipcodeAverageIncomeTextBox.Clear();
            zipcodeMedianAgeTextBox.Clear();
            zipcodeDataGridView.Rows.Clear();

            string qstr = "SELECT zipcode FROM " + mydb.demographicsTableName + " WHERE city = " + "'" + cityListBox.SelectedItem.ToString() + "' AND state_code = " + "'" +
                stateComboBox.SelectedItem.ToString() + "'" + "GROUP BY zipcode;";

            List<String> qResult = mydb.SQLSELECTExec(qstr, "zipcode");

            for (int i = 0; i < qResult.Count; i++)
            {
                zipcodeListBox.Items.Add(qResult[i]);
            }

            qstr = "SELECT SUM(population) FROM " + mydb.demographicsTableName + " where state_code = " + "'" + stateComboBox.SelectedItem.ToString() + "' AND city = " + "'" + cityListBox.SelectedItem.ToString() + "'";

            string qPopulationResult = mydb.SQLCOUNTExec(qstr).ToString("N0");
            cityPopulationTextBox.Text = qPopulationResult;

            qstr = "SELECT AVG(avg_income) FROM " + mydb.demographicsTableName + " WHERE state_code = " + "'" + stateComboBox.SelectedItem.ToString() + "' AND city = " + "'" + cityListBox.SelectedItem.ToString() + "'";
            string qAverageIncomeResult = mydb.SQLAVGExec(qstr).ToString("C2");
            cityAverageIncomeTextBox.Text = qAverageIncomeResult;

            qstr = "SELECT AVG(median_age) FROM " + mydb.demographicsTableName + " WHERE state_code = " + "'" + stateComboBox.SelectedItem.ToString() + "' AND city = " + "'" + cityListBox.SelectedItem.ToString() + "'";
            string qAverageMedianAgeResult = mydb.SQLAVGExec(qstr).ToString("N0");
            cityMedianAgeTextBox.Text = qAverageMedianAgeResult;

            cityDataGridView.Rows.Add(5);
            cityDataGridView.Rows[0].HeaderCell.Value = "Under 18 years";
            cityDataGridView.Rows[1].HeaderCell.Value = "18 to 24 years";
            cityDataGridView.Rows[2].HeaderCell.Value = "25 to 44 years";
            cityDataGridView.Rows[3].HeaderCell.Value = "45 to 64 years";
            cityDataGridView.Rows[4].HeaderCell.Value = "65 and over";

            Dictionary<string, double> qCityResults = mydb.QueryCity(stateComboBox.SelectedItem.ToString(), cityListBox.SelectedItem.ToString());
            int j = 0;
            foreach (string s in qCityResults.Keys)
            {
                //stateDataGridView.Rows.Add(1);
                //stateDataGridView.Rows[j].HeaderCell.Value = s;
                cityDataGridView.Rows[j].Cells[0].Value = qCityResults[s].ToString("N2");
                j++;
            }
            cityDataGridView.AutoResizeRowHeadersWidth(DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders);

            initControls();

        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            zipcodePopulationTextBox.Clear();
            zipcodeAverageIncomeTextBox.Clear();
            zipcodeMedianAgeTextBox.Clear();
            zipcodeDataGridView.Rows.Clear();

            if (zipcodeListBox.SelectedItems.Count > 0)
            {
                string zip = zipcodeListBox.SelectedItem.ToString();
                double avgInc = 0.0;
                int temp = 0;
                int.TryParse(mydb.QueryZipcode("population", zip), out temp);
                zipcodePopulationTextBox.Text = temp.ToString("N0");
                zipcodeAverageIncomeTextBox.Text = mydb.QueryZipcode("avg_income", zip);
                double.TryParse(zipcodeAverageIncomeTextBox.Text, out avgInc);
                zipcodeAverageIncomeTextBox.Text = avgInc.ToString("C2");
                zipcodeMedianAgeTextBox.Text = mydb.QueryZipcode("median_age", zip);
                zipcodeDataGridView.Rows.Add(5);
                zipcodeDataGridView.Rows[0].HeaderCell.Value = "Under 18 years";
                zipcodeDataGridView.Rows[1].HeaderCell.Value = "18 to 24 years";
                zipcodeDataGridView.Rows[2].HeaderCell.Value = "25 to 44 years";
                zipcodeDataGridView.Rows[3].HeaderCell.Value = "45 to 64 years";
                zipcodeDataGridView.Rows[4].HeaderCell.Value = "65 and over";
                zipcodeDataGridView.Rows[0].Cells[0].Value = mydb.QueryZipcode("under18years", zip);
                zipcodeDataGridView.Rows[1].Cells[0].Value = mydb.QueryZipcode("18_to_24years", zip);
                zipcodeDataGridView.Rows[2].Cells[0].Value = mydb.QueryZipcode("25_to_44years", zip);
                zipcodeDataGridView.Rows[3].Cells[0].Value = mydb.QueryZipcode("45_to_64years", zip);
                zipcodeDataGridView.Rows[4].Cells[0].Value = mydb.QueryZipcode("65_and_over", zip);
                zipcodeDataGridView.AutoResizeRowHeadersWidth(DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders);
            }
        }

        private void listBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            attributeTreeView.Nodes.Clear();
            if (zipcodeBusinessSearchListBox.SelectedItems.Count > 0)
            {
                StringBuilder qstr = new StringBuilder();

                if (zipcodeBusinessSearchListBox.SelectedItems.Count > 1)
                {
                    foreach (object item in zipcodeBusinessSearchListBox.SelectedItems)
                    {
                        if (qstr.Length == 0)
                        {
                            //qstr.Append("SELECT * FROM attributes WHERE attributes.bid IN ( SELECT categories.bid FROM categories WHERE categories.name = '"
                            //    + item.ToString() + "'");
                            qstr.Append("SELECT a.*,c.name FROM attributes AS a JOIN categories AS c ON a.bid = c.bid HAVING c.name = '" + item.ToString() + "'");
                        }
                        else
                        {
                            qstr.Append(" OR '" + item + "'");
                        }
                    }
                    //qstr.Append(";");
                }
                else
                {
                    //qstr.Append("SELECT * FROM attributes WHERE attributes.bid IN ( SELECT categories.bid FROM categories WHERE categories.name = '"
                    //    + listBox3.SelectedItem.ToString() + "');\r\n");
                    qstr.Append("SELECT a.*,c.name FROM attributes AS a JOIN categories AS c ON a.bid = c.bid HAVING c.name = '" + zipcodeBusinessSearchListBox.SelectedItem.ToString() + "'");
                }
                //List<String> qResult = mydb.SQLSELECTExec(qstr.ToString(), "*");
                List<String> qResult = mydb.SQLTABLEExec(qstr.ToString());
                foreach (String str in qResult)
                {
                    attributeTreeView.Nodes.Add(str);
                }
            }
            initControls();
        }

        private void addCategoryButton_Click(object sender, EventArgs e)
        {
            foreach (string s in categoriesListBox.SelectedItems)
            {
                if (!categoryQueryListBox.Items.Contains(s))
                    categoryQueryListBox.Items.Add(s);
            }
        }

        private void listBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (attributeQueryListBox.SelectedItem != null && attributeQueryListBox.Items.Count > 0)
            {
                selectValueComboBox.Items.Clear();
                /*string qstr = "SELECT DISTINCT " + attributeQueryListBox.SelectedItem.ToString() + " FROM ac";

                List<string> qresult = mydb.SQLSELECTExec(qstr);

                foreach(string s in qresult)
                {
                    selectValueComboBox.Items.Add(s);
                }
                */
                selectValueComboBox.Items.Add(attributeDict[attributeQueryListBox.SelectedItem.ToString()]);
                selectValueComboBox.SelectedIndex = 0;
                selectValueComboBox.Items.Clear();
                selectValueComboBox.SelectedIndex = -1;
                selectValueComboBox.Text = "";
                selectValueComboBox.Refresh();

            }
        }


        private void updateButton_Click(object sender, EventArgs e)
        {
            stateDataGridView.Rows.Clear();
            cityDataGridView.Rows.Clear();
            zipcodeDataGridView.Rows.Clear();
            string qStateStr = null, qCityStr = null, qZipcodeStr = null, qStr = null;
            List<string> qStrList = new List<string>();
            List<string> qResult = new List<string>();
            int i = 0;

            if (categoryQueryListBox.Items.Count > 0)
            {
                if (stateComboBox.SelectedItem != null)
                {
                    qStateStr = "SELECT COUNT(b.bid), AVG(stars), AVG(review_count) FROM businesses b, categories c WHERE b.bid = c.bid AND b.state = '" +
                        stateComboBox.SelectedItem + "'";

                    if (cityListBox.SelectedItems.Count > 0)
                    {
                        qCityStr = "SELECT COUNT(b.bid), AVG(stars), AVG(review_count) FROM businesses b, categories c WHERE b.bid = c.bid AND b.state = '" +
                                 stateComboBox.SelectedItem + "' AND b.city = '" + cityListBox.SelectedItem + "'";

                        if (zipcodeListBox.SelectedItems.Count > 0)
                        {
                            qZipcodeStr = "SELECT COUNT(b.bid), AVG(stars), AVG(review_count) FROM businesses b, categories c WHERE b.bid = c.bid AND b.state = '" +
                                 stateComboBox.SelectedItem + "' AND b.city = '" + cityListBox.SelectedItem + "' AND b.zipcode = '" + zipcodeListBox.SelectedItem + "'";
                        }
                    }
                }
                // add each category to qStrList to be querried against for each category
                foreach (string s in categoryQueryListBox.Items)
                {
                    qStr = " AND c.name = '" + s + "'";
                    qStrList.Add(qStr);
                }

                if (qZipcodeStr != null)
                {
                    i = 0;
                    // Query db for every category individually
                    foreach (string s in qStrList)
                    {
                        zipcodeNumberOfBusinessesDataGridView.Rows.Add();
                        zipcodeNumberOfBusinessesDataGridView.Rows[i].HeaderCell.Value = s.Substring(15);
                        qResult = mydb.QueryBusinessSearch(qZipcodeStr + s);
                        int j = 0;
                        foreach (string t in qResult)
                        {
                            zipcodeNumberOfBusinessesDataGridView.Rows[i].Cells[j++].Value = t;
                        }
                        i++;
                    }
                    zipcodeNumberOfBusinessesDataGridView.AutoResizeRowHeadersWidth(DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders);
                }

                if (qCityStr != null)
                {
                    i = 0;
                    // Query db for every category individually
                    foreach (string s in qStrList)
                    {
                        cityNumberOfBusinessesDataGridView.Rows.Add();
                        cityNumberOfBusinessesDataGridView.Rows[i].HeaderCell.Value = s.Substring(14);
                        qResult = mydb.QueryBusinessSearch(qCityStr + s);
                        int j = 0;
                        foreach (string t in qResult)
                        {
                            cityNumberOfBusinessesDataGridView.Rows[i].Cells[j++].Value = t;
                        }
                        i++;
                    }
                    cityNumberOfBusinessesDataGridView.AutoResizeRowHeadersWidth(DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders);
                }

                if (qStateStr != null)
                {
                    i = 0;
                    // Query db for every category individually
                    foreach (string s in qStrList)
                    {
                        stateNumberOfBusinessDataGridView.Rows.Add();
                        stateNumberOfBusinessDataGridView.Rows[i].HeaderCell.Value = s.Substring(14);
                        qResult = mydb.QueryBusinessSearch(qStateStr + s);
                        int j = 0;
                        foreach (string t in qResult)
                        {
                            stateNumberOfBusinessDataGridView.Rows[i].Cells[j++].Value = t;
                        }
                        i++;
                    }
                    stateNumberOfBusinessDataGridView.AutoResizeRowHeadersWidth(DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders);
                }
            }
        }

        private void stateBusinessSearchComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            cityBusinessSearchListBox.Items.Clear();
            zipcodeBusinessSearchListBox.Items.Clear();

            string qstr = "SELECT city FROM demographics WHERE state_code = " + "'" + stateBusinessSearchComboBox.SelectedItem.ToString() + "'" + "GROUP BY city;";

            List<String> qResult = mydb.SQLSELECTExec(qstr, "city");

            for (int i = 0; i < qResult.Count; i++)
            {
                cityBusinessSearchListBox.Items.Add(qResult[i]);
            }

            businessSearchResultsDataGridView.AutoResizeRowHeadersWidth(DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders);

            initControls();
        }

        private void cityBusinessSearchListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            zipcodeBusinessSearchListBox.Items.Clear();
            if (businessSearchResultsDataGridView.Rows.Count > 0)
            {
                //foreach (DataGridViewRow r in businessSearchResultsDataGridView.Rows)
                //{
                //    businessSearchResultsDataGridView.Rows.Remove(r);
                //}
                businessSearchResultsDataGridView.Rows.Clear();
            }
            string qstr = "SELECT zipcode FROM demographics WHERE state_code = '" + stateBusinessSearchComboBox.SelectedItem.ToString() + "'" + " AND city = '" + cityBusinessSearchListBox.SelectedItem.ToString() + "' GROUP BY zipcode";
            List<string> qResult = mydb.SQLSELECTExec(qstr, "zipcode");
            foreach (string s in qResult)
            {
                zipcodeBusinessSearchListBox.Items.Add(s);
            }
        }

        private void updateBusinessSearchButton_Click(object sender, EventArgs e)
        {
            //foreach (DataGridViewRow r in businessSearchResultsDataGridView.Rows)
            //{
            //    if (!r.IsNewRow)
            //        businessSearchResultsDataGridView.Rows.Remove(r);
            //}
            if (businessSearchResultsDataGridView.Rows.Count > 0)
            {
                businessSearchResultsDataGridView.DataSource = null;
                businessSearchResultsDataGridView.Refresh();
            }
            string qstr = "SELECT bid, name, city, state, zipcode, stars, review_count FROM bac WHERE";
            //string qstr = "SELECT b.name, b.city, b.state, b.zipcode, b.stars, b.review_count FROM businesses b, categories c WHERE";
            if (stateBusinessSearchComboBox.SelectedItem != null)
            {
                qstr += " state = '" + stateBusinessSearchComboBox.SelectedItem.ToString() + "'";
            }
            if (cityBusinessSearchListBox.SelectedItem != null)
            {
                qstr += " AND city = '" + cityBusinessSearchListBox.SelectedItem.ToString() + "'";
            }
            if (zipcodeBusinessSearchListBox.SelectedItem != null)
            {
                qstr += " AND zipcode = '" + zipcodeBusinessSearchListBox.SelectedItem.ToString() + "'";
            }
            if (minRatingComboBox.SelectedIndex > -1)
            {
                qstr += " AND stars >= " + minRatingComboBox.SelectedItem;
            }
            if (maxRatingComboBox.SelectedIndex > -1)
            {
                qstr += " AND stars <= " + maxRatingComboBox.SelectedItem;
            }
            if (minReviewsComboBox.SelectedIndex > -1)
            {
                if (minReviewsComboBox.SelectedIndex == 6)
                {
                    qstr += " AND review_count >= 1000";
                }
                else
                {
                    qstr += " AND review_count >= " + minReviewsComboBox.SelectedItem;
                }
            }
            if (maxReviewsComboBox.SelectedIndex > -1)
            {
                if (maxReviewsComboBox.SelectedIndex == 6)
                {
                    qstr += " AND review_count <= 1000";
                }
                else
                {
                    qstr += " AND review_count <= " + maxReviewsComboBox.SelectedItem;
                }
            }
            if (attributeQueryListBox.Items.Count > 0)
            {
                foreach (string s in attributeQueryListBox.Items)
                {
                    qstr += " AND " + s + " = " + attributeDict[s];
                }
            }
            //if(categoryQueryBusinessSearchListBox.Items.Count > 1)
            //{
            //    qstr += " GROUP BY bid HAVING COUNT(*) >=" + categoryQueryBusinessSearchListBox.Items.Count;
            //}
            //qstr += " GROUP BY b.name";
            /************** MANY CAT **********************/
            if (categoryQueryBusinessSearchListBox.Items.Count > 1)
            {
                string[] catViews = new string[categoryQueryBusinessSearchListBox.Items.Count];
                string[] catViewQuery = new string[categoryQueryBusinessSearchListBox.Items.Count];
                string newQStr = "SELECT DISTINCT * FROM ";
                for(int i = 0; i < categoryQueryBusinessSearchListBox.Items.Count; i++)
                {
                    catViews[i] += categoryQueryBusinessSearchListBox.Items[i] + "view";
                    catViewQuery[i] += "CREATE OR REPLACE VIEW " + catViews[i] + " AS " + qstr + " AND category = '" + categoryQueryBusinessSearchListBox.Items[i] + "'";
                    mydb.SQLNonQueryExec(catViewQuery[i]);

                    newQStr += catViews[i];  // add list item

                    if(i < (catViews.Count() - 1)) // check if last item in list
                    {
                        newQStr += " WHERE bid IN (SELECT bid FROM ";
                    }
                }
                // CREATE HOURSVIEW
                if (day.CheckedItems.Count > 0)
                {
                    string ocQStr = "SELECT * FROM Hours WHERE day=";

                    if (day.CheckedItems.Count > 1)
                    {
                        ocQStr += "(";
                        foreach (string s in day.CheckedItems)
                        {
                            if(s.Equals(day.CheckedItems[0])) // first item to add
                            {
                                ocQStr += s;
                            }
                            else
                            {
                                ocQStr += " OR " + s;
                            }

                        }
                        ocQStr += ")";
                    }
                    else // only one day selected
                    {
                        ocQStr += day.CheckedItems[0];
                    }
                    // ADD Hour Restrictions
                    if(hour.SelectedItems.Count > 0)
                    {
                        ocQStr += " AND open <= " + hour.SelectedItems[0] + " AND close >= " + hour.SelectedItems[hour.SelectedItems.Count];
                    }

                    string hourViewQuery = "CREATE OR REPLACE VIEW hourView AS " + ocQStr + ";";
                    mydb.SQLNonQueryExec(hourViewQuery); // create hourView
                    string hourViewReturnQuery = "SELECT DISTINCT bid FROM hourView;"; // get list of bid for businesses that are open during specified hours


                }

                // CLOSE PARENTHESES
                for (int i = 0; i < catViews.Count() - 1; i++)
                {
                    newQStr += ")";
                }
                newQStr += ";";
                qstr = newQStr;

                // SELECT bid FROM businesses WHERE bid IN (SELECT bid FROM bac WHERE b.bid = c.bid AND category = cat1
                // AND SELECT bid FROM bac WHERE b.bid = c.bid AND category = cat2 ... etc)

                //qstr += " IN (category = '" + categoryQueryBusinessSearchListBox.Items[0].ToString() + "'";
                //for (int i = 1; i < categoryQueryBusinessSearchListBox.Items.Count; i++)
                //{
                //    qstr += " AND category = '" + categoryQueryBusinessSearchListBox.Items[i].ToString() + "'";
                //}
                //qstr += ")";

                //qstr += " AND category IN ('" + categoryQueryBusinessSearchListBox.Items[0] + "'";
                //for (int i = 1; i < categoryQueryBusinessSearchListBox.Items.Count; i++)
                //{
                //    qstr += ",'" + categoryQueryBusinessSearchListBox.Items[i].ToString() + "'";
                //}
                //qstr += ")";
            }
            /************* ONE CAT ****************************/
            else if (categoryQueryBusinessSearchListBox.Items.Count < 2)
            {
                if(categoryQueryBusinessSearchListBox.Items.Count > 0)
                    qstr += " AND category = '" + categoryQueryBusinessSearchListBox.Items[0].ToString() + "'";
            }

            if (friendsGo.Checked)
            {
                // check to see if friends have written a review of this business
            }
            mydb.SQLNonQueryExec("CREATE OR REPLACE VIEW testView AS " + qstr);
            try
            {
                businessSearchResultsDataGridView.DataSource = mydb.SQLDATATABLEExec(qstr);
                foreach (DataGridViewRow r in businessSearchResultsDataGridView.Rows)
                {
                    r.HeaderCell.Value = String.Format("{0}", r.Index + 1);
                }
                businessSearchResultsDataGridView.Columns[0].Visible = false;
                businessSearchResultsDataGridView.Columns[1].HeaderText = "Business Name";
                businessSearchResultsDataGridView.Columns[2].HeaderText = "City";
                businessSearchResultsDataGridView.Columns[3].HeaderText = "State";
                businessSearchResultsDataGridView.Columns[4].HeaderText = "Zipcode";
                businessSearchResultsDataGridView.Columns[5].HeaderText = "# of Stars";
                businessSearchResultsDataGridView.Columns[6].HeaderText = "# of Reviews";

                for (int i = 0; i < businessSearchResultsDataGridView.Columns.Count; i++)
                {
                    businessSearchResultsDataGridView.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }

                businessSearchResultsDataGridView.AutoResizeRowHeadersWidth(DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders);
                businessSearchResultsDataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

                // Clean up views from database
                if (categoryQueryBusinessSearchListBox.Items.Count > 1)
                {
                    string[] catViews = new string[categoryQueryBusinessSearchListBox.Items.Count];
                    string[] catViewQuery = new string[categoryQueryBusinessSearchListBox.Items.Count];
                    for (int i = 0; i < categoryQueryBusinessSearchListBox.Items.Count; i++)
                    {
                        catViews[i] += categoryQueryBusinessSearchListBox.Items[i] + "view";
                        catViewQuery[i] += "DROP VIEW " + catViews[i];
                        mydb.SQLNonQueryExec(catViewQuery[i]);
                    }
                }
                if(day.SelectedItems.Count > 0)
                {
                    mydb.SQLNonQueryExec("DROP VIEW hourView");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void addCategoryBusinessSearchButton_Click(object sender, EventArgs e)
        {
            foreach (string s in categoriesBusinessSearchListBox.SelectedItems)
            {
                if (!categoryQueryBusinessSearchListBox.Items.Contains(s))
                    categoryQueryBusinessSearchListBox.Items.Add(s);
            }
        }

        private void businessSearchResultsDataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            string qstr = "SELECT r.date, r.stars, r.text, r.uid, u.name, r.useful FROM reviews r, users u WHERE r.uid = u.uid";

            if (businessSearchResultsDataGridView.SelectedRows.Count > 1)
            {
                DataGridViewRow r = businessSearchResultsDataGridView.SelectedRows[0];

                qstr += " AND ( bid = '" + r.Cells[0].Value.ToString() + "'";

                for (int i = 1; i < businessSearchResultsDataGridView.SelectedRows.Count; i++)
                {
                    qstr += " OR bid = '" + businessSearchResultsDataGridView.SelectedRows[i].Cells[0].Value.ToString() + "'";
                }
                qstr += ")";
            }
            else
            {
                qstr += " AND bid = '" + businessSearchResultsDataGridView.SelectedRows[0].Cells[0].Value.ToString() + "'";
            }

            DataGridView revTable = new DataGridView();

            revTable.DataSource = mydb.SQLDATATABLEExec(qstr);

            Form revCtrl = new Form();

            revCtrl.Controls.Add(revTable);
            revTable.Visible = true;
            revCtrl.Visible = false;

            revCtrl.AutoSize = true;
            revCtrl.Width = 580;
            revCtrl.Height = 900;
            revTable.Dock = DockStyle.Fill;

            revCtrl.StartPosition = FormStartPosition.CenterScreen;

            revCtrl.ShowDialog();
        }

        private void attributeTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            selectValueComboBox.Text = "";
            selectValueComboBox.Items.Clear();

            string qstr = "SELECT DISTINCT ";

            if (attributeTreeView.SelectedNode.Text == "Smoking")
            {
                qstr += "Smoking ";
            }
            else if (attributeTreeView.SelectedNode.Text == "WiFi")
            {
                qstr += "WiFi ";
            }
            else if (attributeTreeView.SelectedNode.Text == "Noise_level")
            {
                qstr += "Noise_level ";
            }
            else if (attributeTreeView.SelectedNode.Text == "Attire")
            {
                qstr += "Attire ";
            }
            else if (attributeTreeView.SelectedNode.Text == "Alcohol")
            {
                qstr += "Alcohol ";
            }
            else
            {
                selectValueComboBox.Items.Add(true);
                selectValueComboBox.Items.Add(false);
                return;
            }
            qstr += "FROM attributes;";

            List<string> qResult = mydb.SQLSELECTExec(qstr);
            foreach (string s in qResult)
            {
                selectValueComboBox.Items.Add(s);
            }
        }

        private void addAttributeButton_Click(object sender, EventArgs e)
        {
            if (attributeTreeView.SelectedNode != null && selectValueComboBox.SelectedItem != null && !attributeQueryListBox.Items.Contains(attributeTreeView.SelectedNode.Text))
            {
                attributeDict.Add(attributeTreeView.SelectedNode.Text, selectValueComboBox.SelectedItem.ToString());
                attributeQueryListBox.Items.Add(attributeTreeView.SelectedNode.Text);
                selectValueComboBox.Items.Clear();
                selectValueComboBox.SelectedIndex = -1;
                selectValueComboBox.Text = "";
                selectValueComboBox.Refresh();
            }
        }

        private void removeAttributeButton_Click(object sender, EventArgs e)
        {
            if (attributeQueryListBox.SelectedItem != null && attributeQueryListBox.Items.Count > 0)
            {
                attributeDict.Remove(attributeQueryListBox.SelectedItem.ToString());
                attributeQueryListBox.Items.Remove(attributeQueryListBox.SelectedItem);
            }
        }

        private void newDatabaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void businessesjsonToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "JSON Files (.json)|*.json|Text Files (.txt)|*.txt";
                ofd.FilterIndex = 0;
            }
        }

        private void removeCategoryButton_Click(object sender, EventArgs e)
        {
            if (categoryQueryBusinessSearchListBox.SelectedItems.Count > 0)
            {
                categoryQueryBusinessSearchListBox.Items.Remove(categoryQueryBusinessSearchListBox.SelectedItem);
            }
        }

        private void username_SelectedIndexChanged(object sender, EventArgs e)
        {
            string uid = username.SelectedItem.ToString();
            List<string> query = mydb.SQLSELECTExec("SELECT DISTINCT fid FROM Friends WHERE uid = \'" + uid + "\'");
            foreach (string s in query)
                friends.Items.Add(s);
        }

        private void day_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(day.CheckedItems.Count > 0)
            {
                hour.Show();
            }
            else
            {
                hour.Hide();
            }
        }
    }
}

