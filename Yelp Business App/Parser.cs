/*WSU EECS CptS 451*/
/*Instructor: Sakire Arslan Ay*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace parse_yelp
{
    class Parser
    {
        //initialize the input/output data directory. Currently set to execution folder. 
        public static String dataDir = ".\\..\\..\\yelp\\";
        public static void initParser()
        {
            List<Business> bizList = new List<Business>();
            string contents;
            using (System.IO.StreamReader jsonFile = new System.IO.StreamReader(dataDir + "biz.sql"))
            {
                while (jsonFile.ReadLine() != null)
                {

                    contents = jsonFile.ToString();
                    var jObj = JObject.Parse(contents);
                    Business biz = JsonConvert.DeserializeObject<Business>(contents);
                    string output = JsonConvert.SerializeObject(biz);
                    Console.WriteLine(output);
                }
                jsonFile.Close();
            }
            //JSONParser my_parser = new JSONParser();

            ////Parse yelp_business.json 
            //my_parser.parseJSONFile(dataDir + "yelp_business.json", dataDir + "business.sql");

            ////Parse yelp_review.json 
            //my_parser.parseJSONFile(dataDir + "yelp_review.json", dataDir + "review.sql");

            ////Parse yelp_user.json 
            //my_parser.parseJSONFile(dataDir + "yelp_user.json", dataDir + "user.sql");

        }
    }
}
