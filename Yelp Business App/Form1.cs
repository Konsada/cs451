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

namespace Yelp_Business_App
{
    public partial class Form1 : Form
    {
        MySql_Connection mydb;
        List<string> categories;

        public Form1()
        {
            //init();
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            mydb = new MySql_Connection();
            UpdateStateComboBoxes();
            intitCategories();
            initControls();
        }
        void init()
        {
            businessInit();
            //reviewInit();
            //userInit();
        }
        void initControls()
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

        }
        protected void intitCategories()
        {
            string qStr = "SELECT DISTINCT name FROM categories";
            categories = mydb.SQLSELECTExec(qStr, "name");
            foreach (string s in categories)
            {
                zipcodeBusinessSearchListBox.Items.Add(s);
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
                        }
                        else
                        {
                            sb.Append(biz.writeBizValue());
                            //biz.hashAttributes(atts);
                            ab.Append(biz.ab);
                            cb.Append(biz.cb);
                        }
                        count++;
                    }
                    sb.Append(";\r\n");
                    cb.Append(";\r\n");

                    outfile.Write(sb);
                    outfile.Write(cb);
                    outfile.Write(ab);
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
                        if (count % 3000 == 0)
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
                using (System.IO.StreamWriter outfile = new System.IO.StreamWriter(dataDir + "user.sql"))
                {
                    StringBuilder sb = new StringBuilder();
                    contents = jsonFile.ReadLine();
                    while ((contents = jsonFile.ReadLine()) != null)
                    {
                        User uzr = JsonConvert.DeserializeObject<User>(contents);

                        if (count % 5000 == 0)
                        {
                            if (count > 0)
                            {
                                sb.Append(";\r\n");
                            }
                            sb.Append(uzr.writeUzr());
                        }
                        else
                        {
                            sb.Append(uzr.writeUzrValue());
                        }
                        count++;
                    }
                    sb.Append(";\r\n");
                    outfile.Write(sb);
                }
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            string qstr = "SELECT distinct state_code FROM demographics ORDER BY state_code;";
            //execute query
            List<String> qResult = mydb.SQLSELECTExec(qstr, "maincategory");

            //copy query results to listBox1
            for (int i = 0; i < qResult.Count; i++)
            {
                cityListBox.Items.Add(qResult[i]);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

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
                mydb = new MySql_Connection();
                mydb.server = serverName;
                mydb.database = databaseName;
                mydb.uid = userName;
                mydb.password = passwordName;
                UpdateStateComboBoxes();
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
            for(int i = 0; i < qResult.Count; i++)
            {
                stateBusinessSearchComboBox.Items.Add(qResult[i]);
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

            string qstr = "SELECT city FROM demographics WHERE state_code = " + "'" + stateComboBox.SelectedItem.ToString() + "'" + "GROUP BY city;";

            List<String> qResult = mydb.SQLSELECTExec(qstr, "city");

            for (int i = 0; i < qResult.Count; i++)
            {
                cityListBox.Items.Add(qResult[i]);
            }

            qstr = "SELECT SUM(population) FROM demographics where state_code = " + "'" + stateComboBox.SelectedItem.ToString() + "'";

            string qPopulationResult = mydb.SQLCOUNTExec(qstr).ToString("N0");
            statePopulationTextBox.Text = qPopulationResult;

            qstr = "SELECT AVG(avg_income) FROM demographics WHERE state_code = " + "'" + stateComboBox.SelectedItem.ToString() + "'";
            string qAverageIncomeResult = mydb.SQLAVGExec(qstr).ToString("C2");
            stateAverageIncomeTextBox.Text = qAverageIncomeResult;

            qstr = "SELECT AVG(median_age) FROM demographics WHERE state_code = " + "'" + stateComboBox.SelectedItem.ToString() + "'";
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

            string qstr = "SELECT zipcode FROM demographics WHERE city = " + "'" + cityListBox.SelectedItem.ToString() + "' AND state_code = " + "'" +
                stateComboBox.SelectedItem.ToString() + "'" + "GROUP BY zipcode;";

            List<String> qResult = mydb.SQLSELECTExec(qstr, "zipcode");

            for (int i = 0; i < qResult.Count; i++)
            {
                zipcodeListBox.Items.Add(qResult[i]);
            }

            qstr = "SELECT SUM(population) FROM demographics where state_code = " + "'" + stateComboBox.SelectedItem.ToString() + "' AND city = " + "'" + cityListBox.SelectedItem.ToString() + "'";

            string qPopulationResult = mydb.SQLCOUNTExec(qstr).ToString("N0");
            cityPopulationTextBox.Text = qPopulationResult;

            qstr = "SELECT AVG(avg_income) FROM demographics WHERE state_code = " + "'" + stateComboBox.SelectedItem.ToString() + "' AND city = " + "'" + cityListBox.SelectedItem.ToString() + "'";
            string qAverageIncomeResult = mydb.SQLAVGExec(qstr).ToString("C2");
            cityAverageIncomeTextBox.Text = qAverageIncomeResult;

            qstr = "SELECT AVG(median_age) FROM demographics WHERE state_code = " + "'" + stateComboBox.SelectedItem.ToString() + "' AND city = " + "'" + cityListBox.SelectedItem.ToString() + "'";
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
            attributesListBox.Items.Clear();
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
                    attributesListBox.Items.Add(str);
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
                        zipcodeNumberOfBusinessesDataGridView.Rows[i].HeaderCell.Value = s.Substring(14);
                        qResult = mydb.QueryBusinessSearch(qZipcodeStr + s);
                        int j = 2;
                        foreach (string t in qResult)
                        {
                            zipcodeNumberOfBusinessesDataGridView.Rows[i].Cells[j].Value = t;
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
                        int j = 2;
                        foreach (string t in qResult)
                        {
                            cityNumberOfBusinessesDataGridView.Rows[i].Cells[j].Value = t;
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
                        int j = 2;
                        foreach (string t in qResult)
                        {
                            stateNumberOfBusinessDataGridView.Rows[i].Cells[j].Value = t;
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
            foreach(DataGridViewRow r in businessSearchResultsDataGridView.Rows)
            {
                businessSearchResultsDataGridView.Rows.Remove(r);
            }

        }

        private void updateBusinessSearchButton_Click(object sender, EventArgs e)
        {
            string qstr = "SELECT name, city, state, zipcode, stars, review_count FROM businesses WHERE";
            if(stateBusinessSearchComboBox.SelectedItem != null)
            {
                qstr += " AND state = '" + stateBusinessSearchComboBox.SelectedItem.ToString() + "'";
            }
            if(cityBusinessSearchListBox.SelectedItem != null)
            {
                qstr += " AND city = '" + cityBusinessSearchListBox.SelectedItem.ToString() + "'";
            }
            if(zipcodeBusinessSearchListBox.SelectedItem != null)
            {
                qstr += " AND zipcode = '" + zipcodeBusinessSearchListBox.SelectedItem.ToString() + "'";
            }
            if(categoryQueryBusinessSearchListBox.Items.Count > 1)
            {
                qstr += " AND (c.name = '" + categoryQueryBusinessSearchListBox.Items[0].ToString() + "'";
                for(int i = 1; i < categoryQueryBusinessSearchListBox.Items.Count; i++)
                {
                    qstr += " OR c.name = '" + categoryQueryBusinessSearchListBox.Items[i].ToString() + "'";
                }
                qstr += ")";
            }
            else
            {
                qstr += " AND c.name = '" + categoryQueryBusinessSearchListBox.Items[0].ToString() + "'";
            }
        }
    }
}

