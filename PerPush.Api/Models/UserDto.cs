using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PerPush.Api.Models
{
    public class UserDto
    {
        public string NickName { get; set; }

        public string FirstName { get; set; }
        //Account user name
        public string Email { get; set; }

        public string LastName { get; set; }

        public string SchoolName { get; set; }

        public string Bio { get; set; }

    }
}
