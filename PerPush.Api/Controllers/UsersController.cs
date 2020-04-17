using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using PerPush.Api.Entities;
using PerPush.Api.Helpers;
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

        [HttpGet("pubpapers/{paperIds}",Name = nameof(GetPublicPapersForUser))]
        public async Task<ActionResult<IEnumerable<PaperDto>>> GetPublicPapersForUser(Guid userId,
            [FromRoute]
            [ModelBinder(BinderType = typeof(ArrayModelBinder))]
            IEnumerable<Guid> paperIds)
        {
            if (!await userService.UserExistsAsync(userId))
            {
                return NotFound();
            }
            if(paperIds == null)
            {
                var papers = await userService.GetUserPublicPaperAsync(userId);

                var paperDtos = mapper.Map<IEnumerable<PaperDto>>(papers);

                return Ok(paperDtos);
            }
            var entities = await userService.GetUserPublicPaperAsync(userId, paperIds);

            if(entities.Count() != paperIds.Count())
            {
                return NotFound();
            }

            var returnDto = mapper.Map<IEnumerable<PaperDto>>(entities);

            return Ok(returnDto);
            

        }
        [HttpGet("pripapers/{paperIds}",Name = nameof(GetPrivatePapersForUser))]
        public async Task<ActionResult<IEnumerable<PaperDto>>> GetPrivatePapersForUser(Guid userId,
            [FromRoute]
            [ModelBinder(BinderType = typeof(ArrayModelBinder))]
            IEnumerable<Guid> paperIds)
        {
            if(!await userService.UserExistsAsync(userId))
            {
                return NotFound();
            }
            if(paperIds == null)
            {
                var papers = await userService.GetUserPrivatePapersAsync(userId);

                var paperDtos = mapper.Map<IEnumerable<PaperDto>>(papers);

                return Ok(paperDtos);
            }

            var entities = await userService.GetUserPrivatePapersAsync(userId, paperIds);

            if(entities.Count() != paperIds.Count())
            {
                return NotFound();
            }

            var returnDto = mapper.Map<IEnumerable<PaperDto>>(entities);
            return Ok(returnDto);
            
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

            if(paper.Auth == true)
            {
                return CreatedAtRoute(nameof(GetPublicPapersForUser), new { userId, paperId = returnDto.Id }, returnDto);
            }
            return CreatedAtRoute(nameof(GetPrivatePapersForUser), new { userId, paperId = returnDto.Id }, returnDto);

        }
        [HttpPut("paper/{paperId}")]
        public async Task<ActionResult<PaperDto>> UpdatePaper(
            [FromRoute]Guid userId,
            [FromRoute]Guid paperId,
            PaperUpdateDto paper)
        {
            if(!await userService.UserExistsAsync(userId))
            {
                return NotFound();
            }

            var paperEntity = await userService.GetPaperAsync(userId,paperId);

            if(paperEntity == null)
            {
                return NotFound();
            }

            mapper.Map(paper, paperEntity);
            userService.UpdatePaper(paperEntity);
            await userService.SaveAsync();

            var returnDto = mapper.Map<PaperDto>(paperEntity);

            return Ok(returnDto);
        }
        [HttpOptions]
        public IActionResult GetOptions()
        {
            Response.Headers.Add("Allow","GET,POST,OPTIONS,PUT");
            return Ok();
        }
    }
}
