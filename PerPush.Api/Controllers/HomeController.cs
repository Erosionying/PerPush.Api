using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PerPush.Api.DtoParameters;
using PerPush.Api.Helpers;
using PerPush.Api.Models;
using PerPush.Api.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
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
        [HttpGet(Name = nameof(GetBriefPapers))]
        [HttpHead]
        public async Task<ActionResult<IEnumerable<PaperDto>>> GetBriefPapers([FromQuery] PaperDtoParameters paperDtoParameters)
        {
            var papers = await paperService.GetPapersAsync(paperDtoParameters);

            var previousLink = papers.HasPrevious ? 
                CreatePapersResourceUri(paperDtoParameters, ResourceUriType.PreviousPage) : null;
            var nextLink = papers.HasNext ?
                CreatePapersResourceUri(paperDtoParameters, ResourceUriType.NextPage) : null;

            var paginationMetaData = new
            {
                currentPage = papers.CurrentPage,
                pageSize = papers.PageSize,
                totalPages = papers.TotalPages,
                totalCount = papers.TotalCount,
                previousLink,
                nextLink
            };

            Response.Headers.Add("X-Pagination",JsonSerializer.Serialize(paginationMetaData, new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            }));

            var papersDto = mapper.Map<IEnumerable<PaperBriefDetailDto>>(papers);

            return Ok(papersDto);
        }
        [HttpGet("{userId}/paper/{paperId}")]
        public async Task<ActionResult<PaperDto>> GetPaper(Guid userId, Guid paperId)
        {
            var paperEntity = await paperService.GetPublicPaperAsync(userId, paperId);
            if(paperEntity == null)
            {
                return NotFound();
            }

            var returnDto = mapper.Map<PaperDto>(paperEntity);

            return Ok(returnDto);
        }

        private string CreatePapersResourceUri(PaperDtoParameters parameters,ResourceUriType type)
        {
            switch(type)
            {
                case ResourceUriType.PreviousPage:
                    return Url.Link(nameof(GetBriefPapers), new
                    {
                        pageNumber = parameters.PageNumber - 1,
                        pageSize = parameters.PageSize,
                        title = parameters.Title,
                        lable = parameters.Lable,
                        searchTerm = parameters.SearchTerm,
                    });
                case ResourceUriType.NextPage:
                    return Url.Link(nameof(GetBriefPapers), new
                    {
                        pageNumber = parameters.PageNumber + 1,
                        pageSize = parameters.PageSize,
                        title = parameters.Title,
                        lable = parameters.Lable,
                        searchTerm = parameters.SearchTerm,
                    });
                default:
                    return Url.Link(nameof(GetBriefPapers), new
                    {
                        pageNumber = parameters.PageNumber + 1,
                        pageSize = parameters.PageSize,
                        title = parameters.Title,
                        lable = parameters.Lable,
                        searchTerm = parameters.SearchTerm,
                    });

            }
        }
    }
}
