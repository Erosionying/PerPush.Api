using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PerPush.Api.Models
{
    public class UserUpdateDto
    {
        [Required]
        [StringLength(64, MinimumLength = 1)]
        public string NickName { get; set; }

        [Required]
        [MaxLength(64)]
        //Account user name
        public string Email { get; set; }

        [StringLength(128, MinimumLength = 1)]
        public string SchoolName { get; set; }

        [MaxLength(256)]
        public string Bio { get; set; }
    }
}
