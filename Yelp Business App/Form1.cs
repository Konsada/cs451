using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using parse_yelp;
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

        public Form1()
        {
            init();
            InitializeComponent();
            mydb = new MySql_Connection();
            UpdateComboBox1();
        }
        void init()
        {
            //businessInit();
            //reviewInit();
            //userInit();
            //intitCategories();
        }
        protected void intitCategories()
        {
            Categories c = new Categories();
            foreach(string s in c.mainCategories)
            {
                listBox3.Items.Add(s);
            }
        }
        protected void businessInit()
        {
            String dataDir = ".\\..\\..\\yelp\\";

            List<Business> bizList = new List<Business>();
            string contents;
            using (System.IO.StreamReader jsonFile = new System.IO.StreamReader(dataDir + "yelp_business.json"))
            {
                using (System.IO.StreamWriter outfile = new System.IO.StreamWriter(dataDir + "business.sql"))
                {
                    while ((contents = jsonFile.ReadLine()) != null)
                    {
                        StringBuilder sb = new StringBuilder();
                        var jObj = JObject.Parse(contents);
                        Business biz = JsonConvert.DeserializeObject<Business>(contents);
                        sb.Append(biz.writeBiz());
                        outfile.Write(sb);
                    }
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
                            if(count > 0)
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
                            if(count > 0)
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
            string qstr = "SELECT distinct state_code FROM censusdata ORDER BY state_code;";
            //execute query
            List<String> qResult = mydb.SQLSELECTExec(qstr, "maincategory");

            //copy query results to listBox1
            for (int i = 0; i < qResult.Count; i++)
            {
                listBox1.Items.Add(qResult[i]);
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
                UpdateComboBox1();
            }
        }
        protected void UpdateComboBox1()
        {
            comboBox1.Items.Clear();
            string qstr = "SELECT state_code FROM censusdata GROUP BY state_code;";
            //execute query
            List<String> qResult = mydb.SQLSELECTExec(qstr, "state_code");

            //copy query results to listBox1
            for (int i = 0; i < qResult.Count; i++)
            {
                comboBox1.Items.Add(qResult[i]);
            }
        }

        private void comboBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            listBox2.Items.Clear();
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            dataTable.Rows.Clear();
            string qstr = "SELECT city FROM censusdata WHERE state_code = " + "'" + comboBox1.SelectedItem.ToString() + "'" + "GROUP BY city;";

            List<String> qResult = mydb.SQLSELECTExec(qstr, "city");

            for (int i = 0; i < qResult.Count; i++)
            {
                listBox1.Items.Add(qResult[i]);
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox2.Items.Clear();
            string qstr = "SELECT zipcode FROM censusdata WHERE city = " + "'" + listBox1.SelectedItem.ToString() + "' AND state_code = " + "'" +
                comboBox1.SelectedItem.ToString() + "'" + "GROUP BY zipcode;";

            List<String> qResult = mydb.SQLSELECTExec(qstr, "zipcode");

            for (int i = 0; i < qResult.Count; i++)
            {
                listBox2.Items.Add(qResult[i]);
            }
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            dataTable.Rows.Clear();

            string zip = listBox2.SelectedItem.ToString();

            textBox1.Text = mydb.QueryZipcode("population", zip);
            textBox2.Text = mydb.QueryZipcode("avg_income", zip);
            textBox3.Text = mydb.QueryZipcode("median_age", zip);
            dataTable.Rows.Add(5);
            dataTable.Rows[0].HeaderCell.Value = "Under 18 years";
            dataTable.Rows[1].HeaderCell.Value = "18 to 24 years";
            dataTable.Rows[2].HeaderCell.Value = "25 to 44 years";
            dataTable.Rows[3].HeaderCell.Value = "45 to 64 years";
            dataTable.Rows[4].HeaderCell.Value = "65 and over";
            dataTable.Rows[0].Cells[0].Value = mydb.QueryZipcode("under18years", zip);
            dataTable.Rows[1].Cells[0].Value = mydb.QueryZipcode("18_to_24years", zip);
            dataTable.Rows[2].Cells[0].Value = mydb.QueryZipcode("25_to_44years", zip);
            dataTable.Rows[3].Cells[0].Value = mydb.QueryZipcode("45_to_64years", zip);
            dataTable.Rows[4].Cells[0].Value = mydb.QueryZipcode("65_and_over", zip);
        }
    }
}
