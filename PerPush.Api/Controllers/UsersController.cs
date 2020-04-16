using AutoMapper;
using Microsoft.AspNetCore.Authorization;
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
    [Route("api/user/{userId}")]
    [ApiController]
    [Authorize]
    public class UsersController:ControllerBase
    {
        private readonly IUserService userService;
        private readonly IMapper mapper;

        public UsersController(IUserService userService, IMapper mapper)
        {
            this.userService = userService ?? throw new ArgumentNullException(nameof(userService));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        [HttpGet("pubpapers",Name = nameof(GetPublicPapersForUser))]
        public async Task<ActionResult<IEnumerable<PaperDto>>> GetPublicPapersForUser(Guid userId)
        {
            if (!await userService.UserExistsAsync(userId))
            {
                return NotFound();
            }

            var papers = await userService.GetUserPublicPaperAsync(userId);

            var paperDtos = mapper.Map<IEnumerable<PaperDto>>(papers);

            return Ok(paperDtos);

        }
        [HttpGet("pripapers",Name = nameof(GetPrivatePapersForUser))]
        public async Task<ActionResult<IEnumerable<PaperDto>>> GetPrivatePapersForUser(Guid userId)
        {
            if(! await userService.UserExistsAsync(userId))
            {
                return NotFound();
            }

            var papers = await userService.GetUserPrivatePapersAsync(userId);

            var paperDtos = mapper.Map<IEnumerable<PaperDto>>(papers);

            return Ok(paperDtos);
        }
        [HttpGet("center")]
        public async Task<ActionResult<UserDto>> GetUserInfo([FromRoute]Guid userId)
        {
            var user = await userService.GetUserInfoAsync(userId);

            var userDto = mapper.Map<UserDto>(user);

            return Ok(userDto);
        }
        
        [HttpPost("paper")]
        public async Task<ActionResult<PaperDto>> CreatePaper([FromRoute]Guid userId,PaperAddDto paperAddDto)
        {
            if(! await userService.UserExistsAsync(userId))
            {
                return NotFound();
            }

            var paper = mapper.Map<Paper>(paperAddDto);
            userService.AddPaper(userId, paper);
            await userService.SaveAsync();

            var returnDto = mapper.Map<PaperDto>(paper);

            return CreatedAtRoute(nameof(GetPublicPapersForUser), new { userId,paperId = returnDto.Id }, returnDto);

        }
        //[HttpPost]
        //public async Task<ActionResult<>>
    }
}
