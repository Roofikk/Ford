using Newtonsoft.Json;
using System;

namespace Ford.WebApi.Data
{
    public class AccountDto : UserDto
    {
        public string Email { get; set; }
        public DateTime LastUpdatedDate { get; set; }
    }
}
