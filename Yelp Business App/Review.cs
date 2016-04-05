using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection;

namespace Yelp_Business_App
{
    class Review
    {
        public Dictionary<string, int> votes { get; set; }
        [JsonProperty("user_id")]
        public string uid { get; set; }
        [JsonProperty("review_id")]
        public string rid { get; set; }
        public int stars { get; set; }
        public string date { get; set; }
        public string text { get; set; }
        public string type { get; set; }
        [JsonProperty("business_id")]
        public string bid { get; set; }

        public StringBuilder writeRev()
        {
            StringBuilder sb = new StringBuilder();
            StringBuilder insertsb = new StringBuilder();
            StringBuilder valuesb = new StringBuilder("VALUES (");
            StringBuilder temp = new StringBuilder();

            sb.Append("INSERT INTO Reviews (");

            //temp.Append(text);
            //temp.Replace("\"","\\\"");
            //text = temp.ToString();
            text = System.Text.RegularExpressions.Regex.Replace(text, @"[\000\010\011\012\015\032\042\047\134\140]", "\\$0");


            foreach (PropertyInfo p in typeof(Review).GetProperties())
            {
                if (p.Name == "votes")
                {
                    if (this.votes != null)
                    {
                        foreach (string k in votes.Keys)
                        {
                            insertsb.Append(", " + k);
                            valuesb.Append(", " + votes[k]);
                        }
                    }
                }
                else if (p.PropertyType == typeof(string))
                {
                    insertsb.Append(", " + p.Name);
                    valuesb.Append(", \"" + p.GetValue(this) + "\"");
                }
                else
                {
                    insertsb.Append(", " + p.Name);
                    valuesb.Append(", " + p.GetValue(this));
                }
            }
            insertsb.Append(") ");
            valuesb.Append(")");
            valuesb.Remove(8, 1);
            sb.Append(insertsb);
            sb.Append(valuesb);

            sb.Remove(21, 1);
            return sb;
        }
        public StringBuilder writeRevValue()
        {
            StringBuilder sb = new StringBuilder();
            StringBuilder valuesb = new StringBuilder(",\r\n (");
            StringBuilder temp = new StringBuilder();

            text = System.Text.RegularExpressions.Regex.Replace(text, @"[\000\010\011\012\015\032\042\047\134\140]", "\\$0");

            foreach (PropertyInfo p in typeof(Review).GetProperties())
            {
                if (p.Name == "votes")
                {
                    if (this.votes != null)
                    {
                        foreach (string k in votes.Keys)
                        {
                            valuesb.Append(", " + votes[k]);
                        }
                    }
                }
                else if (p.PropertyType == typeof(string))
                {
                    valuesb.Append(", \"" + p.GetValue(this) + "\"");
                }
                else
                {
                    valuesb.Append(", " + p.GetValue(this));
                }
            }
            valuesb.Remove(5, 1);
            sb.Append(valuesb);
            sb.Append(")");

            return sb;
        }

    }

}
