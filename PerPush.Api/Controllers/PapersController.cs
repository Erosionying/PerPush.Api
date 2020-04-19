using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PerPush.Api.DtoParameters;
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
    public class PapersController:ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IPaperService paperService;
        private readonly IUserService userService;

        public PapersController(IMapper mapper, IPaperService paperService, IUserService userService)
        {
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.paperService = paperService ?? throw new ArgumentNullException(nameof(paperService));
            this.userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }
        
        [HttpGet("papers")]
        public async Task<ActionResult<IEnumerable<PaperBriefDetailDto>>> GetPublicPapersForUser(
            Guid userId,
            [FromQuery]PaperDtoParameters parameters)
        {
            if (!await userService.UserExistsAsync(userId))
            {
                return NotFound();
            }

            var papers = await userService.GetUserPublicPaperAsync(userId, parameters);

            var paperDtos = mapper.Map<IEnumerable<PaperBriefDetailDto>>(papers);

            return Ok(paperDtos);

        }
        [HttpGet("paper/{paperId}")]
        public async Task<ActionResult<IEnumerable<PaperDto>>> GetPaperForUser(Guid userId, Guid paperId)
        {
            if (!await userService.UserExistsAsync(userId))
            {
                return NotFound();
            }

            var paper = await userService.GetPaperAsync(userId, paperId);
            if (paper == null)
            {
                return NotFound();
            }

            var paperDto = mapper.Map<PaperDto>(paper);

            return Ok(paperDto);

        }
        [HttpGet]
        public async Task<ActionResult<UserInfoDto>> GetAuthorInfo(Guid userId)
        {
            if (!await userService.UserExistsAsync(userId))
            {
                return NotFound();
            }

            var userInfo = await userService.GetUserInfoAsync(userId);
            var Author = mapper.Map<UserInfoDto>(userInfo);

            return Ok(Author);

        }
        
    }
}
