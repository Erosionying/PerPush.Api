using System;
using System.ComponentModel.DataAnnotations;

namespace PerPush.Api.Entities
{
    public class Paper
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required]
        [StringLength(512, MinimumLength = 1)]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        [StringLength(512, MinimumLength = 1)]
        public string Lable { get; set; }

        public bool Auth { get; set; } = true;
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTimeOffset StartTime { get; set; } = DateTimeOffset.Now;
        /// <summary>
        /// 参观人数
        /// </summary>
        public int Visitors { get; set; } = 0;
        /// <summary>
        /// 喜欢的人数
        /// </summary>
        public int Likes { get; set; } = 0;
        public User Author { get; set; }
    }
}
