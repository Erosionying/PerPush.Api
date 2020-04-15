using PerPush.Api.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PerPush.Api.Models
{
    public class PaperDto
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Lable { get; set; }

        //public bool Auth { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public int Visitors { get; set; }
        public int Likes { get; set; }
        public string Author { get; set; }
    }
}
