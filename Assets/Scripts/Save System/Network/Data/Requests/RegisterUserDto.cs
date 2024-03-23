using Newtonsoft.Json;
using System;

namespace Ford.WebApi.Data
{
    public class RegisterUserDto
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [JsonConverter(typeof(DateConverter))]
        public DateTime? BirthDate { get; set; }
    }
}
