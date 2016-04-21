using System;
using System.Management;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Reflection;


namespace Yelp_Business_App
{
    public class Business
    {
        public string business_id { get; set; }
        public string full_address { get; set; }
        public Hours hours { get; set; }
        public bool open { get; set; }
        public List<string> categories { get; set; }
        public string city { get; set; }
        public int review_count { get; set; }
        public string name { get; set; }
        public List<string> neighborhoods { get; set; }
        public float longitude { get; set; }
        public string state { get; set; }
        public float stars { get; set; }
        public float latitude { get; set; }
        public Attributes attributes { get; set; }
        //public List<string> aTable { get; set; }
        public string type { get; set; }
        public StringBuilder ab { get; set; } = new StringBuilder();
        public StringBuilder cb { get; set; } = new StringBuilder();
        public StringBuilder writeBiz()
        {
            StringBuilder insertsb = new StringBuilder();
            StringBuilder valuesb = new StringBuilder();
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO businesses (");
            insertsb.Append("bid");
            valuesb.Append("\"" + business_id + "\"");
            //sb.Append("bid, name, city, state_code, zipcode, open) VALUES (" + business_id + ", " + name + ", ");
            //sb.Append(city + ", " + state + ", " + full_address.Substring(full_address.Length-5) + ", " + open + ");\r\n");

            // DYNAMIC STRING BUILDING <S>
            // BUILD insertsb and valuesb then combine after both are formed
            foreach (PropertyInfo p in typeof(Business).GetProperties())
            {
                if (p.Name == "categories") // appends categories to sb
                {
                    StringBuilder insertcb = new StringBuilder("INSERT INTO categories (name, bid) \r\nVALUES ");
                    StringBuilder valuecb = new StringBuilder();
                    foreach (string c in categories)
                    {
                        if (valuecb.Length > 0)
                        {
                            valuecb.Append(",\r\n");
                        }
                        valuecb.Append("(\"" + c + "\", " + "\"" + business_id + "\"" + ")");
                    }
                    cb.Append(insertcb.ToString() + valuecb.ToString());
                }
                else if (p.Name == "ab" || p.Name == "cb" || p.Name == "aTable")
                {

                }
                else if (p.Name == "business_id")
                {

                }
                else if (p.Name == "neighborhoods")
                {

                }
                else if (p.Name == "attributes")
                {
                    StringBuilder insertab = new StringBuilder("INSERT INTO attributes (bid");
                    StringBuilder valueab = new StringBuilder("VALUES (\"" + business_id + "\"");
                    foreach (PropertyInfo q in typeof(Attributes).GetProperties())
                    {
                        if (q.Name == "Music")
                        {
                            if (attributes.Music != null)
                            {
                                foreach (string k in this.attributes.Music.Keys)
                                {
                                    insertab.Append(", " + k);
                                    valueab.Append(", " + attributes.Music[k]);
                                    //aTable.Add(k);
                                }
                            }
                        }
                        else if (q.Name == "Ambience")
                        {
                            if (attributes.Ambience != null)
                            {
                                foreach (string k in this.attributes.Ambience.Keys)
                                {
                                    insertab.Append(", " + k);
                                    valueab.Append(", " + attributes.Ambience[k]);
                                    //aTable.Add(k);
                                }
                            }
                        }
                        else if (q.Name == "Goodfor")
                        {
                            if (attributes.Goodfor != null)
                            {
                                foreach (string k in this.attributes.Goodfor.Keys)
                                {
                                    insertab.Append(", " + k);
                                    valueab.Append(", " + attributes.Goodfor[k]);
                                    //aTable.Add(k);
                                }
                            }
                        }
                        else if (q.Name == "Parking")
                        {
                            if (attributes.Parking != null)
                            {
                                foreach (string k in attributes.Parking.Keys)
                                {
                                    insertab.Append(", " + k);
                                    valueab.Append(", " + attributes.Parking[k]);
                                    //aTable.Add(k);
                                }
                            }
                        }
                        else
                        {
                            insertab.Append(", " + q.Name);
                            valueab.Append(", ");
                            if (q.PropertyType == typeof(string))
                            {
                                valueab.Append("\"" + q.GetValue(this.attributes) + "\"");
                            }
                            else
                                valueab.Append(q.GetValue(this.attributes));
                            //valuesb.Append(this.attributes.GetType().GetProperty(q.ToString()).GetValue(this.attributes, null));
                            //aTable.Add(q.Name);
                        }
                    }
                    insertab.Append(") ");
                    ab.Append(insertab);
                    ab.Append(valueab + ");\r\n");
                }

                else if (p.Name == "hours")
                {
                    ///Worry about later, maybe make a table with bid and hours
                    /*
                        foreach (PropertyInfo q in typeof(Hours).GetProperties())
                        {
                            PropertyInfo[] props = q.GetType().GetProperties();
                            string dayName = q.Name.Substring(1);
                            foreach(var prop in props)
                            {
                                insertsb.Append(", " + prop.Name);
                                valuesb.Append(", ");
                                valuesb.Append(this.GetType().GetProperty(prop.ToString()).GetValue(this));

                            }
                        }
                      */
                }
                else
                {
                    insertsb.Append(", " + p.Name);
                    valuesb.Append(", ");
                    if (p.PropertyType == typeof(string))
                    {
                        string text = (string)p.GetValue(this);
                        text = text.Replace("\n", ", ");
                        text = text.Replace("\"", "");
                        valuesb.Append("\"" + text + "\"");
                    }
                    else
                        valuesb.Append(p.GetValue(this));
                    //aTable.Add(p.Name);
                    if (p.Name == "full_address")
                    {
                        insertsb.Append(", zipcode");
                        valuesb.Append(", \"" + p.GetValue(this).ToString().Substring(p.GetValue(this).ToString().Length-5) + "\"");
                    }
                }
            }
            sb.Append(insertsb + ") \r\nVALUES (" + valuesb + ")");

            /// BRUTE FORCE
            /*
                sb.Append("INSERT INTO attributes (bid, Takeout, Drivethru, Dessert, Latenight, Lunch, Dinner, Brunch, Breakfast, Caters, Noise_level, " +
                "Takes_reservations, Delivery, Romantic, Intimate, Classy, Hipster, Divey, Touristy, Trendy, Upscale, Casual, Garage, Street, " +
                "Validated, Lot, Valet, Has_tv, Outdoor_seating, Attire, Alcohol, Waiter_service, Accept_credit, Good_for_kids, Good_for_groups, Price_range)"
                + " VALUES (" + business_id + "," + this.attributes.Takeout + "," + this.attributes.DriveThru + "," + this.attributes.Goodfor["dessert"] + "," 
                + this.attributes.Goodfor["latenight"] + "," + this.attributes.Goodfor["lunch"] + "," + this.attributes.Goodfor["dinner"] + "," + 
                this.attributes.Goodfor["brunch"] + "," + this.attributes.Goodfor["breakfast"] + "," + this.attributes.Caters + "," + this.attributes.Noise_level
                 + "," + this.attributes.Takes_reservations + "," + this.attributes.Delivery + "," + this.attributes.Ambience["romantic"] + "," + this.attributes.Ambience["intimate"]
                  + "," + this.attributes.Ambience["classy"] + "," + this.attributes.Ambience["hipster"] + "," + this.attributes.Ambience["divey"] + "," + 
                  this.attributes.Ambience["touristy"] + "," + this.attributes.Ambience["trendy"] + "," + this.attributes.Ambience["upscale"] + "," + 
                  this.attributes.Ambience["casual"] + "," + this.attributes.Parking["garage"] + "," + this.attributes.Parking["street"] + "," + this.attributes.Parking["validated"]
                   + "," + this.attributes.Parking["lot"] + "," + this.attributes.Parking["valet"] + "," + this.attributes.Has_tv + "," + this.attributes.Outdoor_seating + "," + 
                   this.attributes.Attire + "," + this.attributes.Alcohol + "," + this.attributes.Waiter_service + "," + this.attributes.Accept_credit_cards
                    + "," + this.attributes.Good_for_kids + "," + this.attributes.Good_for_groups + "," + this.attributes.Price_range);
            */
            return sb;
        }
        public StringBuilder writeBizValue()
        {
            StringBuilder valuesb = new StringBuilder();
            StringBuilder sb = new StringBuilder(",\r\n(");

            //sb.Append("bid, name, city, state_code, zipcode, open) VALUES (" + business_id + ", " + name + ", ");
            //sb.Append(city + ", " + state + ", " + full_address.Substring(full_address.Length-5) + ", " + open + ");\r\n");

            // DYNAMIC STRING BUILDING <S>
            // BUILD insertsb and valuesb then combine after both are formed
            foreach (PropertyInfo p in typeof(Business).GetProperties())
            {
                if (p.Name == "categories") // appends categories to sb
                {
                    if (categories.Count > 0)
                    {
                        foreach (string c in categories)
                        {
                            cb.Append(",\r\n(\"" + c + "\", " + "\"" + business_id + "\"" + ")");
                        }
                    }
                }
                else if (p.Name == "ab" || p.Name == "cb" || p.Name == "aTable")
                {

                }
                else if (p.Name == "business_id")
                {
                    valuesb.Append("\"" + business_id + "\"");
                }
                else if (p.Name == "neighborhoods")
                {

                }
                else if (p.Name == "attributes")
                {
                    StringBuilder insertab = new StringBuilder("INSERT INTO attributes(bid");
                    StringBuilder valueab = new StringBuilder("VALUES (\"" + business_id + "\"");
                    foreach (PropertyInfo q in typeof(Attributes).GetProperties())
                    {
                        if (q.Name == "Music")
                        {
                            if (attributes.Music != null)
                            {
                                foreach (string k in this.attributes.Music.Keys)
                                {
                                    insertab.Append(", " + k);
                                    valueab.Append(", " + attributes.Music[k]);
                                    //aTable.Add(k);
                                }
                            }
                        }
                        else if (q.Name == "Ambience")
                        {
                            if (attributes.Ambience != null)
                            {
                                foreach (string k in this.attributes.Ambience.Keys)
                                {
                                    insertab.Append(", " + k);
                                    valueab.Append(", " + attributes.Ambience[k]);
                                    //aTable.Add(k);
                                }
                            }
                        }
                        else if (q.Name == "Goodfor")
                        {
                            if (attributes.Goodfor != null)
                            {
                                foreach (string k in this.attributes.Goodfor.Keys)
                                {
                                    insertab.Append(", " + k);
                                    valueab.Append(", " + attributes.Goodfor[k]);
                                    //aTable.Add(k);
                                }
                            }
                        }
                        else if (q.Name == "Parking")
                        {
                            if (attributes.Parking != null)
                            {
                                foreach (string k in attributes.Parking.Keys)
                                {
                                    insertab.Append(", " + k);
                                    valueab.Append(", " + attributes.Parking[k]);
                                    //aTable.Add(k);
                                }
                            }
                        }
                        else
                        {
                            insertab.Append(", " + q.Name);
                            valueab.Append(", ");
                            if (q.PropertyType == typeof(string))
                            {
                                valueab.Append("\"" + q.GetValue(this.attributes) + "\"");
                            }
                            else
                                valueab.Append(q.GetValue(this.attributes));
                            //valuesb.Append(this.attributes.GetType().GetProperty(q.ToString()).GetValue(this.attributes, null));
                            //aTable.Add(q.Name);
                        }
                    }
                    insertab.Append(") ");
                    ab.Append(insertab);
                    ab.Append(valueab + ");\r\n");
                }

                else if (p.Name == "hours")
                {
                    ///Worry about later, maybe make a table with bid and hours
                    /*
                        foreach (PropertyInfo q in typeof(Hours).GetProperties())
                        {
                            PropertyInfo[] props = q.GetType().GetProperties();
                            string dayName = q.Name.Substring(1);
                            foreach(var prop in props)
                            {
                                insertsb.Append(", " + prop.Name);
                                valuesb.Append(", ");
                                valuesb.Append(this.GetType().GetProperty(prop.ToString()).GetValue(this));

                            }
                        }
                      */
                }
                else
                {
                    //insertsb.Append(", " + p.Name);
                    valuesb.Append(", ");
                    if (p.PropertyType == typeof(string))
                    {
                        string text = (string)p.GetValue(this);
                        text = text.Replace("\n",", ");
                        text = text.Replace("\"", "");
                        valuesb.Append("\"" + text + "\"");
                    }
                    else
                        valuesb.Append(p.GetValue(this));
                    //aTable.Add(p.Name);
                    if (p.Name == "full_address")
                    {
                        valuesb.Append(", \"" + p.GetValue(this).ToString().Substring(p.GetValue(this).ToString().Length - 5) + "\"");
                    }

                }

            }
            //sb.Append(insertsb + ") VALUES (" + valuesb + ");\r\n");

            /// BRUTE FORCE
            /*
                sb.Append("INSERT INTO attributes (bid, Takeout, Drivethru, Dessert, Latenight, Lunch, Dinner, Brunch, Breakfast, Caters, Noise_level, " +
                "Takes_reservations, Delivery, Romantic, Intimate, Classy, Hipster, Divey, Touristy, Trendy, Upscale, Casual, Garage, Street, " +
                "Validated, Lot, Valet, Has_tv, Outdoor_seating, Attire, Alcohol, Waiter_service, Accept_credit, Good_for_kids, Good_for_groups, Price_range)"
                + " VALUES (" + business_id + "," + this.attributes.Takeout + "," + this.attributes.DriveThru + "," + this.attributes.Goodfor["dessert"] + "," 
                + this.attributes.Goodfor["latenight"] + "," + this.attributes.Goodfor["lunch"] + "," + this.attributes.Goodfor["dinner"] + "," + 
                this.attributes.Goodfor["brunch"] + "," + this.attributes.Goodfor["breakfast"] + "," + this.attributes.Caters + "," + this.attributes.Noise_level
                 + "," + this.attributes.Takes_reservations + "," + this.attributes.Delivery + "," + this.attributes.Ambience["romantic"] + "," + this.attributes.Ambience["intimate"]
                  + "," + this.attributes.Ambience["classy"] + "," + this.attributes.Ambience["hipster"] + "," + this.attributes.Ambience["divey"] + "," + 
                  this.attributes.Ambience["touristy"] + "," + this.attributes.Ambience["trendy"] + "," + this.attributes.Ambience["upscale"] + "," + 
                  this.attributes.Ambience["casual"] + "," + this.attributes.Parking["garage"] + "," + this.attributes.Parking["street"] + "," + this.attributes.Parking["validated"]
                   + "," + this.attributes.Parking["lot"] + "," + this.attributes.Parking["valet"] + "," + this.attributes.Has_tv + "," + this.attributes.Outdoor_seating + "," + 
                   this.attributes.Attire + "," + this.attributes.Alcohol + "," + this.attributes.Waiter_service + "," + this.attributes.Accept_credit_cards
                    + "," + this.attributes.Good_for_kids + "," + this.attributes.Good_for_groups + "," + this.attributes.Price_range);
            */
            sb.Append(valuesb + ")");
            return sb;
        }
        //public HashSet<string> hashAttributes(HashSet<string> oldHash)
        //{
        //    foreach(string s in aTable)
        //    {
        //        oldHash.Add(s);
        //    }
        //    return oldHash;
        //}
        //    public string writeAttInsert(HashSet<string> list)
        //    {
        //        StringBuilder line = new StringBuilder("INSERT INTO attributes (\"");
        //        string first = list.First();

        //        foreach(string s in list)
        //        {
        //            if (s != first)
        //                line.Append(",");
        //            line.Append("\"" + s + "\"");
        //        }
        //        return line.ToString();
        //    }
    }
    public class Hoods
    {
        public string hoods { get; set; }
    }

    public class Attributes
    {
        public string Alcohol { get; set; }
        [JsonProperty("Noise Level")]
        public string Noise_level { get; set; }
        public Dictionary<string, bool> Music { get; set; }
        public string Attire { get; set; }
        public Dictionary<string, bool> Ambience { get; set; }
        [JsonProperty("Good for Kids")]
        public bool Good_for_kids { get; set; }
        [JsonProperty("Good for Groups")]
        public bool Good_for_groups { get; set; }
        [JsonProperty("Wheelchair Accessible")]
        public bool Wheelchair_accessible { get; set; }
        [JsonProperty("Good For Dancing")]
        public bool Good_for_dancing { get; set; }
        public bool Delivery { get; set; }
        [JsonProperty("Dogs Allowed")]
        public bool Dogs_allowed { get; set; }
        [JsonProperty("Coath Check")]
        public bool Coat_check { get; set; }
        public string Smoking { get; set; }
        [JsonProperty("Accepts Credit Cards")]
        public bool Accept_credit_cards { get; set; }
        [JsonProperty("Take-out")]
        public bool Takeout { get; set; }
        [JsonProperty("Drive-Thru")]
        public bool DriveThru { get; set; }
        [JsonProperty("Good For")]
        public Dictionary<string, bool> Goodfor { get; set; }
        public bool Caters { get; set; }
        [JsonProperty("Price Range")]
        public short Price_range { get; set; }
        [JsonProperty("Has TV")]
        public bool Has_tv { get; set; }
        [JsonProperty("Outdoor Seating")]
        public bool Outdoor_seating { get; set; }
        [JsonProperty("Takes Reservations")]
        public bool Takes_reservations { get; set; }
        [JsonProperty("Waiter Services")]
        public bool Waiter_service { get; set; }
        [JsonProperty("Wi-Fi")]
        public string WiFi { get; set; }
        public Dictionary<string, bool> Parking { get; set; }
        [JsonProperty("Happy Hour")]
        public bool Happy_hour { get; set; }
    }
    public class Hours
    {
        public _Sunday Sunday { get; set; }
        public _Monday Monday { get; set; }
        public _Tuesday Tuesday { get; set; }
        public _Wednesday Wednesday { get; set; }
        public _Thursday Thursday { get; set; }
        public _Friday Friday { get; set; }
        public _Saturday Saturday { get; set; }
    }
    public class Day
    {
        public string close { get; set; }
        public string open { get; set; }
    }
    public class _Friday : Day
    {

    }
    public class _Tuesday : Day
    {

    }
    public class _Thursday : Day
    {

    }
    public class _Wednesday : Day
    {

    }
    public class _Monday : Day
    {

    }
    public class _Saturday : Day
    {

    }
    public class _Sunday : Day
    {

    }

}

