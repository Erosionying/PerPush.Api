﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using PerPush.Api.Attributes;
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
    [Route("api/user/{userId}")]
    [ApiController]
    [LimitPerMin]
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
        //Get all public articles under the user name
        [HttpGet("pubpapers", Name = nameof(GetUserPublicPapers))]
        public async Task<ActionResult<IEnumerable<PaperBriefDetailDto>>> GetUserPublicPapers(
            Guid userId,
            [FromQuery] PaperDtoParameters parameters)
        {
            if (!await userService.UserExistsAsync(userId))
            {
                return NotFound();
            }
            
            var papers = await userService.GetUserPublicPaperAsync(userId, parameters);

            if(papers == null)
            {
                return NoContent();
            }
            var previousLink = papers.HasPrevious ?
                CreatePapersResourceUri(nameof(GetUserPublicPapers), parameters, ResourceUriType.PreviousPage) : null;
            var nextLink = papers.HasNext ?
                CreatePapersResourceUri(nameof(GetUserPublicPapers), parameters, ResourceUriType.NextPage) : null;

            var paginationMetaData = new
            {
                currentPage = papers.CurrentPage,
                pageSize = papers.PageSize,
                totalPages = papers.TotalPages,
                totalCount = papers.TotalCount,
                previousLink,
                nextLink
            };

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetaData, new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            }));

            var paperDtos = mapper.Map<IEnumerable<PaperBriefDetailDto>>(papers);
            var shapeData = paperDtos.ShapeDate(parameters.fields);

            var links = CreateLinkForUserHome(nameof(GetUserPublicPapers), parameters, papers.HasPrevious, papers.HasNext);

            var shapedWithUserPaper = shapeData.Select(u =>
            {
                var paperDic = u as IDictionary<string, object>;
                var paperLinks = CreateLinkForUserHome(nameof(GetUserPublicPapers), (Guid)paperDic["UserId"], (Guid)paperDic["Id"], null);

                paperDic.Add("links", paperLinks);

                return paperDic;
            });

            var collectionPaper = new
            {
                value = shapedWithUserPaper,
                links
            };

            return Ok(collectionPaper);
        }
        //Use resource ID collection to query public resources
        [HttpGet("papers/{paperIds}",Name = nameof(GetPublicPapersForUserList))]
        public async Task<ActionResult<IEnumerable<PaperDto>>> GetPublicPapersForUserList(Guid userId,
            [FromRoute]
            [ModelBinder(BinderType = typeof(ArrayModelBinder))]
            IEnumerable<Guid> paperIds,
            [FromQuery] PaperDtoParameters parameters = null)
        {
            if (!await userService.UserExistsAsync(userId))
            {
                return NotFound();
            }
            if(paperIds == null)
            {
                var papers = await userService.GetUserPublicPaperAsync(userId, parameters);

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
        //Get all private articles under the user name
        [HttpGet("pripapers", Name = nameof(GetUserPrivatePapers))]
        public async Task<ActionResult<IEnumerable<PaperBriefDetailDto>>> GetUserPrivatePapers(Guid userId,
            [FromQuery] PaperDtoParameters parameters)
        {
            if (!await userService.UserExistsAsync(userId))
            {
                return NotFound();
            }
            
            var papers = await userService.GetUserPrivatePapersAsync(userId, parameters);

            if(papers == null)
            {
                return NoContent();
            }

            var paperDtos = mapper.Map<IEnumerable<PaperBriefDetailDto>>(papers);

            return Ok(paperDtos);
            
        }
        //Use resource ID collection to query private resources
        [HttpGet("pripapers/{paperIds}", Name = nameof(GetPrivatePapersForUserList))]
        public async Task<ActionResult<IEnumerable<PaperDto>>> GetPrivatePapersForUserList(Guid userId,
            [FromRoute]
            [ModelBinder(BinderType = typeof(ArrayModelBinder))]
            IEnumerable<Guid> paperIds,
            [FromQuery] PaperDtoParameters parameters = null)
        {
            if(!await userService.UserExistsAsync(userId))
            {
                return NotFound();
            }
            if(paperIds == null)
            {
                var papers = await userService.GetUserPrivatePapersAsync(userId, parameters);

                var paperDtos = mapper.Map<IEnumerable<PaperBriefDetailDto>>(papers);

                return Ok(paperDtos);
            }

            var entities = await userService.GetUserPrivatePapersAsync(userId, paperIds);

            if(entities.Count() != paperIds.Count())
            {
                return NotFound();
            }

            var returnDto = mapper.Map<IEnumerable<PaperBriefDetailDto>>(entities);
            return Ok(returnDto);
            
        }
        //Get User Profile Information
        [HttpGet("center")]
        public async Task<ActionResult<UserDto>> GetUserInfo([FromRoute]Guid userId)
        {
            var user = await userService.GetUserInfoAsync(userId);

            var userDto = mapper.Map<UserDto>(user);

            return Ok(userDto);
        }
        //Use PatchMethod Update user profile information
        [HttpPatch("center")]
        public async Task<ActionResult<UserDto>> PartiallyUpdateUserInfo(
            [FromRoute] Guid userId
            ,JsonPatchDocument<UserUpdateDto> patchDocument)
        {
            
            var userEntity = await userService.GetUserInfoAsync(userId);

            if(userEntity == null)
            {
                return NotFound();
            }

            var userPatchDto = mapper.Map<UserUpdateDto>(userEntity);
            patchDocument.ApplyTo(userPatchDto, ModelState);

            if(!TryValidateModel(userPatchDto))
            {
                return ValidationProblem(ModelState);
            }

            mapper.Map(userPatchDto, userEntity);
            userService.UpdateUserInfo(userEntity);
            await userService.SaveAsync();


            var returnDto = mapper.Map<UserDto>(userEntity);
            return Ok(returnDto);

        }

        // Create the paper
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
                return CreatedAtRoute(nameof(GetUserPublicPapers), new { userId }, returnDto);
            }
            return CreatedAtRoute(nameof(GetUserPrivatePapers), new { userId }, returnDto);

        }

        //Update paper
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
        //User Patch Method Update Paper Detail
        [HttpPatch("paper/{paperId}",Name = nameof(PartiallyUpdatePaper))]
        public async Task<ActionResult<PaperDto>> PartiallyUpdatePaper(
            Guid userId, 
            Guid paperId, 
            JsonPatchDocument<PaperUpdateDto> patchDocument)
        {
            if(!await userService.UserExistsAsync(userId))
            {
                return NotFound();
            }
            var paperEntity = await userService.GetPaperAsync(userId, paperId);
            if(paperEntity == null)
            {
                return NotFound();
            }

            var paperPatchDto =  mapper.Map<PaperUpdateDto>(paperEntity);

            patchDocument.ApplyTo(paperPatchDto, ModelState);

            if(!TryValidateModel(paperPatchDto))
            {
                return ValidationProblem(ModelState);
            }

            mapper.Map(paperPatchDto, paperEntity);
            userService.UpdatePaper(paperEntity);
            await userService.SaveAsync();

            var returnDto = mapper.Map<PaperDto>(paperEntity);
            return Ok(returnDto);
        }
        //Remove Paper
        [HttpDelete("paper/{paperId}",Name = nameof(DeletePaper))]
        public async Task<IActionResult> DeletePaper(Guid userId, Guid paperId)
        {
            if(! await userService.UserExistsAsync(userId))
            {
                return NotFound();
            }
            var paperEntity = await userService.GetPaperAsync(userId, paperId);
            if (paperEntity == null)
            {
                return NotFound();
            }

            userService.DeletePaper(paperEntity);
            await userService.SaveAsync();

            return NoContent();
        }
        //Get Options
        [HttpOptions(Name = nameof(GetOptions))]
        public IActionResult GetOptions()
        {
            Response.Headers.Add("Allow","GET,POST,OPTIONS,PUT,PATCH");
            return Ok();
        }
        //Model verification
        public override ActionResult ValidationProblem(
            ModelStateDictionary modelStateDictionary)
        {
            var options = HttpContext.RequestServices
                .GetRequiredService<IOptions<ApiBehaviorOptions>>();

            return (ActionResult)options.Value.InvalidModelStateResponseFactory(ControllerContext);
        }
        private string CreatePapersResourceUri(string routeName,PaperDtoParameters parameters, ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return Url.Link( routeName , new
                    {
                        orderBy = parameters.OrderBy,
                        pageNumber = parameters.PageNumber - 1,
                        pageSize = parameters.PageSize,
                        title = parameters.Title,
                        lable = parameters.Lable,
                        searchTerm = parameters.SearchTerm,
                    });
                case ResourceUriType.NextPage:
                    return Url.Link( routeName, new
                    {
                        orderBy = parameters.OrderBy,
                        pageNumber = parameters.PageNumber + 1,
                        pageSize = parameters.PageSize,
                        title = parameters.Title,
                        lable = parameters.Lable,
                        searchTerm = parameters.SearchTerm,
                    });
                default:
                    return Url.Link( routeName, new
                    {
                        orderBy = parameters.OrderBy,
                        pageNumber = parameters.PageNumber,
                        pageSize = parameters.PageSize,
                        title = parameters.Title,
                        lable = parameters.Lable,
                        searchTerm = parameters.SearchTerm,
                    });

            }
        }
        private IEnumerable<LinkDto> CreateLinkForUserHome(
            string routeName, 
            Guid userId, 
            Guid paperId, 
            string fields)
        {
            List<LinkDto> links = new List<LinkDto>();
            if (string.IsNullOrWhiteSpace(fields))
            {
                links.Add(new LinkDto(Url.Link(routeName, new { userId, paperId }),
                    "self",
                    "GET"));
            }
            else
            {
                links.Add(new LinkDto(Url.Link(routeName, new { userId, paperId, fields }),
                    "self",
                    "GET"));
            }
            
            links.Add(new LinkDto(Url.Link(nameof(DeletePaper), new { userId, paperId }),
                "delete_self",
                "DELETE"));

            return links;
        }
        private IEnumerable<LinkDto> CreateLinkForUserHome(string routeName,PaperDtoParameters parameters, bool hasPrevious, bool hasNext)
        {
            List<LinkDto> links = new List<LinkDto>();
            links.Add(new LinkDto(CreatePapersResourceUri( routeName, parameters, ResourceUriType.CurrentPage),
                "self",
                "GET"));

            if (hasPrevious)
            {
                links.Add(new LinkDto(CreatePapersResourceUri(routeName, parameters, ResourceUriType.PreviousPage),
                    "perviousPage",
                    "GET"));
            }

            if (hasNext)
            {
                links.Add(new LinkDto(CreatePapersResourceUri(routeName, parameters, ResourceUriType.NextPage),
                    "nextPage",
                    "GET"));
            }

            return links;
        }

    }
}
