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
    [Route("api/home")]
    [ApiController]
    public class HomeController:ControllerBase
    {
        private readonly IPaperService paperService;
        private readonly IMapper mapper;

        public HomeController(IPaperService paperService, IMapper mapper)
        {
            this.paperService = paperService ?? 
                throw new ArgumentNullException(nameof(paperService));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PaperDto>>> GetPapers()
        {
            var papers = await paperService.GetPapersAsync();

            var papersDto = mapper.Map<IEnumerable<PaperDto>>(papers);

            return Ok(papersDto);
        }
    }
}
