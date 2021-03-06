﻿using Newtonsoft.Json;
using PerPush.Api.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PerPush.Api.Models
{
    public class LoginRequestDto
    {
        [Required]
        [JsonProperty("username")]
        public string UserName { get; set; }

        [NoSpace]
        [Required]
        [JsonProperty("password")]
        public string Password { get; set; }


    }
}
