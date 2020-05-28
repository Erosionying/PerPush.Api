using PerPush.Api.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PerPush.Api.Models
{
    public class UserRegisteredDto
    {

        [Required]
        [StringLength(64, MinimumLength = 1)]
        public string NickName { get; set; }

        [StringLength(64, MinimumLength = 1)]
        public string FirstName { get; set; }

        [StringLength(64, MinimumLength = 1)]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(64)]
        [NoSpace]
        //Account user name
        public string Email { get; set; }

        [NoSpace]
        [DataType(DataType.Password)]
        //Account password
        [Required, Display(Name = "password")]
        [StringLength(32, MinimumLength = 6,ErrorMessage = "The minimum length of the {0} is {2}")]
        public string password { get; set; }


        [StringLength(128, MinimumLength = 1)]
        public string SchoolName { get; set; }

        [MaxLength(256)]
        public string Bio { get; set; }
    }
}
