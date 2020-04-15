using PerPush.Api.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PerPush.Api.Services
{
    public interface IPaperService
    {
        //All public documents
        Task<IEnumerable<Paper>> GetPapersAsync();
        //File under user name
        Task<IEnumerable<Paper>> GetUserPapersAsync(Guid userId);
        Task<IEnumerable<Paper>> GetUserPublicPaperAsync(Guid userId);
        Task<Paper> GetPaperAsync(Guid userId, Guid paperId);
        void AddPaper(Guid userId, Paper paper);
        void UpdatePaper(Paper paper);
        void DeletePaper(Paper paper);



        Task<bool> SaveAsync();
    }
}
