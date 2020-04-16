using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PerPush.Api.Models
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string NickName { get; set; }

        public string Name { get; set; }
        //Account user name
        public string Email { get; set; }

        public string SchoolName { get; set; }

        public string Bio { get; set; }

    }
}
