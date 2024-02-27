using System;

namespace Ford.WebApi.Data
{
    public class AccountDto
    {
        public string UserId { get; set; }
        public string Login { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string Country { get; set; }
        public DateTime? BirthDate { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime? LastUpdatedDate { get; set; }
    }
}
