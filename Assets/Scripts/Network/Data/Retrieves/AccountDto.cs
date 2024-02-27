using System;
using System.Collections.Generic;
using System.Net;

namespace Ford.WebApi.Data
{
    public class AccountDto
    {
        public string Id { get; set; }
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
        public DateTime? LastUpdateDate { get; set; }
        public HttpStatusCode StatusCode { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Uri { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public List<string> Errors { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}
