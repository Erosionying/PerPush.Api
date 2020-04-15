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
        void AddPaper(Guid userId, Paper paper);
        void UpdatePaper(Paper paper);
        void DeletePaper(Paper paper);


        Task<bool> SaveAsync();
    }
}
