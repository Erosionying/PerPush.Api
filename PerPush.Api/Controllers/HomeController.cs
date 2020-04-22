using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PerPush.Api.DtoParameters;
using PerPush.Api.Entities;
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
        private readonly IPropertyMappingService propertyMappingService;
        private readonly IPropertyCheckerService propertyCheckerService;

        public HomeController(IPaperService paperService, 
            IMapper mapper, 
            IPropertyMappingService propertyMappingService,
            IPropertyCheckerService propertyCheckerService)
        {
            this.paperService = paperService ?? throw new ArgumentNullException(nameof(paperService));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.propertyMappingService = propertyMappingService ??
                throw new ArgumentNullException(nameof(propertyMappingService));
            this.propertyCheckerService = propertyCheckerService ?? 
                throw new ArgumentNullException(nameof(propertyCheckerService));
        }
        //Home Inforamtion
        [HttpGet(Name = nameof(GetBriefPapers))]
        [HttpHead]
        public async Task<IActionResult> GetBriefPapers([FromQuery] PaperDtoParameters paperDtoParameters)
        {
            if(!propertyMappingService.ValidMappingExists<PaperDto, Paper>(paperDtoParameters.OrderBy))
            {
                return BadRequest();
            }
            if(!propertyCheckerService.TypeHasProperties<PaperDto>(paperDtoParameters.fields))
            {
                return BadRequest();
            }
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

            return Ok(papersDto.ShapeDate(paperDtoParameters.fields));
        }
        // Brower Paper 
        [HttpGet("{userId}/paper/{paperId}",Name = nameof(GetPaper))]
        public async Task<IActionResult> GetPaper(Guid userId, Guid paperId, [FromQuery] string fields)
        {
            if (!propertyCheckerService.TypeHasProperties<PaperDto>(fields))
            {
                return BadRequest();
            }
            var paperEntity = await paperService.GetPublicPaperAsync(userId, paperId);
            if(paperEntity == null)
            {
                return NotFound();
            }
            var links = CreateLinkForHome(userId, paperId, fields);
            var linkedDic = mapper.Map<PaperDto>(paperEntity).ShapeData(fields) as IDictionary<string, object>;

            linkedDic.Add("links", links);

            return Ok(linkedDic);
        }

        private string CreatePapersResourceUri(PaperDtoParameters parameters,ResourceUriType type)
        {
            switch(type)
            {
                case ResourceUriType.PreviousPage:
                    return Url.Link(nameof(GetBriefPapers), new
                    {
                        orderBy = parameters.OrderBy,
                        pageNumber = parameters.PageNumber - 1,
                        pageSize = parameters.PageSize,
                        title = parameters.Title,
                        lable = parameters.Lable,
                        searchTerm = parameters.SearchTerm,
                    });
                case ResourceUriType.NextPage:
                    return Url.Link(nameof(GetBriefPapers), new
                    {
                        orderBy = parameters.OrderBy,
                        pageNumber = parameters.PageNumber + 1,
                        pageSize = parameters.PageSize,
                        title = parameters.Title,
                        lable = parameters.Lable,
                        searchTerm = parameters.SearchTerm,
                    });
                default:
                    return Url.Link(nameof(GetBriefPapers), new
                    {
                        orderBy = parameters.OrderBy,
                        pageNumber = parameters.PageNumber + 1,
                        pageSize = parameters.PageSize,
                        title = parameters.Title,
                        lable = parameters.Lable,
                        searchTerm = parameters.SearchTerm,
                    });

            }
        }
        private IEnumerable<LinkDto> CreateLinkForHome(Guid userId, Guid paperId , string fields)
        {
            List<LinkDto> links = new List<LinkDto>();
            if(string.IsNullOrWhiteSpace(fields))
            {
                links.Add(new LinkDto(Url.Link(nameof(GetPaper), new { userId, paperId}),
                    "self",
                    "GET"));
            }else
            {
                links.Add(new LinkDto(Url.Link(nameof(GetPaper), new { userId, paperId, fields}),
                    "self",
                    "GET"));
            }

            return links;
        }
    }
}
