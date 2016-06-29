using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yelp_Business_App
{
    class Demographics
    {
        /*
        zipcode					int(11) 		PRIMARY KEY,
        state					char(20),
        state_code				char(2),
        city					varchar(75),
        population				int(11),
        under18years			decimal(4,2)	CHECK (under18 <= 100.0),
        18_to_24years			decimal(4,2)	CHECK (18to24 <= 100.0),
        25_to_44				decimal(4,2)	CHECK (25to44 <= 100.0),
        45_to_64				decimal(4,2)	CHECK (45to64 <= 100.0),
        65_and_over				decimal(4,2)	CHECK (65andover <= 100.0),
        median_age				int(11),
        percentage_of_females	decimal(4,2),
        num_employee			int(11),
        annual_payroll			decimal(9,2),
        avg_income				decimal(9,2)
        */
        public int zipcode;
        public string state;
        public string state_code;
        public string city;
        public int population;
        public double under18;
        public double _18to24;
        public double _25to44;
        public double _45to64;
        public double _65andover;
        public int median_age;
        public double percentage_of_females;
        public int num_employee;
        public double annual_payroll;
        public double avg_income;

    }
}
