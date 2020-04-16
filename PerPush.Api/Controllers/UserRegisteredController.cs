using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PerPush.Api.Entities;
using PerPush.Api.Models;
using PerPush.Api.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PerPush.Api.Controllers
{
    [Route("api/Account")]
    [ApiController]
    public class UserRegisteredController:ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IUserService userService;

        public UserRegisteredController(IMapper mapper, IUserService userService)
        {
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        [HttpPost]
        public async Task<ActionResult<UserDto>> RegisteredAccount([FromBody] UserRegisteredDto userRegisteredDto)
        {
            var user = mapper.Map<User>(userRegisteredDto);

            var result = await userService.RegisteredUser(user);
            var returnDto = mapper.Map<UserDto>(user);

            if (result == true)
            {
                await userService.SaveAsync();

                return Created("api/Account", returnDto);
            }

            return BadRequest(returnDto);
            
        }
    }
}
