using System.ComponentModel.DataAnnotations;

namespace PerPush.Api.Models
{
    public abstract class PaperCUDto
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
