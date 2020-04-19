using Microsoft.EntityFrameworkCore;
using PerPush.Api.Data;
using PerPush.Api.DtoParameters;
using PerPush.Api.Entities;
using PerPush.Api.Helpers;
using PerPush.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PerPush.Api.Services
{
    public class PaperService : IPaperService
    {
        private readonly PerPushDbContext context;
        private readonly IPropertyMappingService propertyMappingService;

        public PaperService(PerPushDbContext context, IPropertyMappingService propertyMappingService)
        {
            this.context = context ?? 
                throw new ArgumentNullException(nameof(context));
            this.propertyMappingService = propertyMappingService ?? 
                throw new ArgumentNullException(nameof(propertyMappingService));
        }
        public async Task<PagedList<Paper>> GetPapersAsync(PaperDtoParameters paperDtoParameters)
        {
            if(paperDtoParameters == null)
            {
                throw new ArgumentNullException(nameof(paperDtoParameters));
            }
            var items = context.papers as IQueryable<Paper>;

            if (!string.IsNullOrWhiteSpace(paperDtoParameters.Title))
            {
                paperDtoParameters.Title = paperDtoParameters.Title.Trim();
                items = items.Where(x => x.Title.Contains(paperDtoParameters.Title));
            }

            if (!string.IsNullOrWhiteSpace(paperDtoParameters.Lable))
            {
                paperDtoParameters.Lable = paperDtoParameters.Lable.Trim();
                items = items.Where(x => x.Auth == true && x.Lable.Contains(paperDtoParameters.Lable));

            }

            if (!string.IsNullOrWhiteSpace(paperDtoParameters.SearchTerm))
            {
                paperDtoParameters.SearchTerm = paperDtoParameters.SearchTerm.Trim();
                items = items
                    .Where(x => x.Title.Contains(paperDtoParameters.SearchTerm) ||
                    x.Description.Contains(paperDtoParameters.SearchTerm) ||
                    x.Lable.Contains(paperDtoParameters.Lable));
            }

            //Get Mapping relations
            var mappingDictionary = propertyMappingService.GetPropertyMapping<PaperDto, Paper>();
            //Order By
            items = items.ApplySort(paperDtoParameters.OrderBy, mappingDictionary);
            
            var returnItems = await PagedList<Paper>.CreateAsync(items, paperDtoParameters.PageNumber, paperDtoParameters.PageSize);

            //Take the Data From DataBase
            foreach (var paper in returnItems)
            {
                paper.Author = await context.users
                    .Where(x => x.Id == paper.UserId)
                    .FirstOrDefaultAsync();
            }
            
            return returnItems;

        }
        public async Task<Paper> GetPublicPaperAsync(Guid userId, Guid paperId)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(userId));
            }
            if (paperId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(paperId));
            }

            var paper =  await context.papers
                .Where(x => x.UserId == userId && x.Id == paperId && x.Auth == true)
                .SingleOrDefaultAsync();

            paper.Author = await context.users
                .Where(x => x.Id == paper.UserId)
                .SingleOrDefaultAsync();

            return paper;
        }

    }
}
