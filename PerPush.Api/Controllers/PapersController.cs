using AutoMapper;
using Microsoft.AspNetCore.Mvc;
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

        public PapersController(IMapper mapper, IPaperService paperService)
        {
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.paperService = paperService ?? throw new ArgumentNullException(nameof(paperService));
        }
    }
}
