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
    [Route("api/home")]
    [ApiController]
    public class HomeController:ControllerBase
    {
        private readonly IPaperService paperService;
        private readonly IMapper mapper;

        public HomeController(IPaperService paperService, IMapper mapper)
        {
            this.paperService = paperService ?? throw new ArgumentNullException(nameof(paperService));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        [HttpGet]
        [HttpHead]
        public async Task<ActionResult<IEnumerable<PaperDto>>> GetPapers([FromQuery] PaperDtoParameters paperDtoParameters)
        {
            var papers = await paperService.GetPapersAsync(paperDtoParameters);

            var papersDto = mapper.Map<IEnumerable<PaperDto>>(papers);

            return Ok(papersDto);
        }
    }
}
