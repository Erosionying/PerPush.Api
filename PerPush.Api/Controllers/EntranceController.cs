using Microsoft.AspNetCore.Mvc;
using PerPush.Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PerPush.Api.Controllers
{
    [ApiController]
    [Route("api")]
    public class EntranceController : ControllerBase
    {
        [Route("Entrance",Name = (nameof(Entrance)))]
        public ActionResult<IEnumerable<LinkDto>> Entrance()
        {
            var retunLinkDto = CreateHomeLink();

            return Ok(retunLinkDto);
        }

        private IEnumerable<LinkDto> CreateHomeLink()
        {
            List<LinkDto> links = new List<LinkDto>();

            links.Add(new LinkDto(Url.Link(nameof(HomeController.GetBriefPapers), new { }),
                    "perPushHome",
                    "GET"));

            links.Add(new LinkDto(Url.Link(nameof(Entrance), new { }),
                    "self",
                    "GET"));

            return links;
        }

    }
    
}
