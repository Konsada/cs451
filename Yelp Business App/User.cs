using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Yelp_Business_App
{
    class User
    {
        public string yelping_since { get; set; }
        public Dictionary<string, int> votes { get; set; }
        public int review_count { get; set; }
        public string name { get; set; }
        [JsonProperty("user_id")]
        public string uid { get; set; }
        public List<string> friends { get; set; }
        public int fans { get; set; }
        public float average_stars { get; set; }
        public string type { get; set; }
        public Compliments compliments { get; set; }
        public List<int> elite { get; set; }
        public class Compliments
        {
            public int profile { get; set; }
            public int cute { get; set; }
            [JsonProperty("funny")]
            public int cfunny { get; set; }
            public int plain { get; set; }
            public int list { get; set; }
            public int writer { get; set; }
            public int note { get; set; }
            public int photos { get; set; }
            public int hot { get; set; }
            [JsonProperty("cool")]
            public int ccool { get; set; }
            public int more { get; set; }

        }
        public StringBuilder fb { get; set; } = new StringBuilder();
        public StringBuilder writeUzr()
        {
            StringBuilder sb = new StringBuilder();
            StringBuilder insertsb = new StringBuilder();
            StringBuilder valuesb = new StringBuilder("VALUES (");
            StringBuilder insertfb = new StringBuilder();
            StringBuilder valuefb = new StringBuilder("VALUES ");

            sb.Append("INSERT INTO Users (");
            fb.Append("INSERT INTO Friends (uid, fid) ");

            foreach (PropertyInfo p in typeof(User).GetProperties())
            {
                if (p.Name == "votes")
                {
                    if (votes != null)
                    {
                        foreach (string k in votes.Keys)
                        {
                            insertsb.Append(", " + k);
                            valuesb.Append(", " + votes[k]);
                        }
                    }
                }
                else if (p.Name == "friends")
                {
                    foreach(string s in friends)
                    {
                        if (s.Equals(friends[0]))
                        {
                            valuefb.Append("\r\n(" + "\"" + uid + "\"" + "," + "\"" + s + "\"" + ")");
                        }
                        else
                        {
                            valuefb.Append(",\r\n(" + "\"" + uid + "\"" + "," + "\"" + s + "\"" + ")");
                        }
                    }
                }
                else if (p.Name == "compliments")
                {
                    foreach (PropertyInfo prop in typeof(Compliments).GetProperties())
                    {
                        insertsb.Append(", " + prop.Name);

                        valuesb.Append(", " + prop.GetValue(this.compliments));
                    }

                }
                else if (p.Name == "elite")
                {

                }
                else if (p.Name == "fb")
                {

                }
                else
                {
                    insertsb.Append(", " + p.Name);

                    if (p.PropertyType == typeof(string))
                    {
                        valuesb.Append(", \"" + p.GetValue(this) + "\"");
                    }
                    else
                        valuesb.Append(", " + p.GetValue(this));
                }
            }
            insertsb.Append(") ");
            valuesb.Append(")");
            valuesb.Remove(8, 1);
            sb.Append(insertsb);
            sb.Append(valuesb);

            sb.Remove(19, 1);
            fb.Append(insertfb.Append(valuefb));

            return sb;
        }
        public StringBuilder writeUzrValue()
        {
            StringBuilder sb = new StringBuilder();
            StringBuilder valuesb = new StringBuilder(",\r\n (");
            StringBuilder valuefb = new StringBuilder();

            foreach (PropertyInfo p in typeof(User).GetProperties())
            {
                if (p.Name == "votes")
                {
                    if (votes != null)
                    {
                        foreach (string k in votes.Keys)
                        {
                            valuesb.Append(", " + votes[k]);
                        }
                    }
                }
                else if (p.Name == "friends")
                {
                    foreach (string s in friends)
                    {
                        if (s.Equals(friends[0]))
                        {
                            valuefb.Append(",\r\n(" + "\"" + uid + "\"" + "," + "\"" + s + "\"" + ")"); // comma necessary?
                                
                        }
                        else
                        {
                            valuefb.Append(",\r\n(" + "\"" + uid + "\"" + "," + "\"" + s + "\"" + ")");
                        }
                    }
                }
                else if (p.Name == "compliments")
                {
                    foreach (PropertyInfo prop in typeof(Compliments).GetProperties())
                    {
                        valuesb.Append(", " + prop.GetValue(this.compliments));
                    }
                }
                else if (p.Name == "elite")
                {

                }
                else if (p.Name == "fb")
                {

                }
                else
                {
                    if (p.PropertyType == typeof(string))
                    {
                        valuesb.Append(", \"" + p.GetValue(this) + "\"");
                    }
                    else
                        valuesb.Append(", " + p.GetValue(this));
                }
            }
            valuesb.Remove(5, 1);
            sb.Append(valuesb);
            sb.Append(")");
            fb.Append(valuefb);
            return sb;
        }

    }
}
