using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PerPush.Api.Models
{
    public class PaperAddDto
    {


        [Required]
        [StringLength(512, MinimumLength = 1)]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        [StringLength(512, MinimumLength = 1)]
        public string Lable { get; set; }

        public bool Auth { get; set; } = true;

    }
}
