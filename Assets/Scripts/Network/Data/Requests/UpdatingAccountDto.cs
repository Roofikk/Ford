
using System;

namespace Ford.WebApi.Data
{
    public class UpdatingAccountDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string Country { get; set; }
        public DateTime? BirthDate { get; set; }
    }
}
