using PerPush.Api.DtoParameters;
using PerPush.Api.Entities;
using PerPush.Api.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PerPush.Api.Services
{
    public interface IPaperService
    {
        //All public documents
        Task<PagedList<Paper>> GetPapersAsync(PaperDtoParameters paperDtoParameters);
        Task<Paper> GetPublicPaperAsync(Guid userId, Guid paperId);

    }
}
