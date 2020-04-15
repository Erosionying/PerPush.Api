using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PerPush.Api.Models;
using PerPush.Api.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PerPush.Api.Controllers
{
    [Route("api/user/{userId}/papers")]
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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PaperDto>>> GetPapersForUser(Guid userId)
        {
            if(!await userService.UserExistsAsync(userId))
            {
                return NotFound();
            }

            var papers = await userService.GetUserPublicPaperAsync(userId);

            var paperDtos =  mapper.Map<IEnumerable<PaperDto>>(papers);

            return Ok(paperDtos);

        }
        [HttpGet("{paperId}")]
        public async Task<ActionResult<IEnumerable<PaperDto>>> GetPapersForUser(Guid userId, Guid paperId)
        {
            if (!await userService.UserExistsAsync(userId))
            {
                return NotFound();
            }

            var paper = await userService.GetPaperAsync(userId,paperId);
            if(paper == null)
            {
                return NotFound();
            }


            var paperDto = mapper.Map<PaperDto>(paper);

            return Ok(paperDto);

        }
    }
}
