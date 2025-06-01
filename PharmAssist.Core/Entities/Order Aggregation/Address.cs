using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmAssist.Core.Entities.Order_Aggregation
{
	public class Address
	{
        public Address()
        {
				
        }
        public Address(string firstName, string lastName, string city, string country, string street)
		{
			FirstName = firstName;
			LastName = lastName;
			City = city;
			Country = country;
			Street = street;
		}

		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string City { get; set; }
		public string Country { get; set; }
		public string Street { get; set; }
	}
}
