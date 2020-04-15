using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PerPush.Api.Entities
{
    public class User
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        [StringLength(64, MinimumLength = 1)]
        public string NickName { get; set; }

        [StringLength(64, MinimumLength = 1)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(64)]
        //Account user name
        public string Email { get; set; }
        //Account password
        [Required]
        [StringLength(32, MinimumLength = 6)]
        public string password { get; set; }

        [StringLength(64, MinimumLength = 1)]
        public string LastName { get; set; }

        [StringLength(128, MinimumLength = 1)]
        public string SchoolName { get; set; }

        [MaxLength(256)]
        public string Bio { get; set; }
        public DateTimeOffset AccountCreateTime { get; } = DateTimeOffset.Now;

        public IEnumerable<Paper> Papers { get; set; }
    }
}
