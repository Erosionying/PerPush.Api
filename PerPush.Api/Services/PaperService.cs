using Microsoft.EntityFrameworkCore;
using PerPush.Api.Data;
using PerPush.Api.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PerPush.Api.Services
{
    public class PaperService : IPaperService
    {
        private readonly PerPushDbContext context;

        public PaperService(PerPushDbContext context)
        {
            this.context = context ?? 
                throw new ArgumentNullException(nameof(context));
        }
        public async Task<IEnumerable<Paper>> GetUserPapersAsync(Guid userId)
        {
            if(userId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(userId));
            }
            return await context.papers
                .Where(x => x.Id == userId)
                .OrderBy(x => x.StartTime)
                .ToListAsync();
        }
        public async Task<IEnumerable<Paper>> GetPapersAsync()
        {
            return await context.papers
                .Where(x => x.Auth == true)
                .ToListAsync();
        }
        public async Task<Paper> GetPaperAsync(Guid userId, Guid paperId)
        {
            if(userId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(userId));
            }
            if(paperId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(paperId));
            }

            return await context.papers
                .Where(x => x.UserId == userId && x.Id == paperId)
                .FirstOrDefaultAsync();
        }
        public void AddPaper(Guid userId, Paper paper)
        {
            if(userId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(userId));
            }
            if( paper == null)
            {
                throw new ArgumentNullException(nameof(paper));
            }

            paper.UserId = userId;
            context.papers.Add(paper);

        }
        public void UpdatePaper(Paper paper)
        {

        }
        public void DeletePaper(Paper paper)
        {
            context.papers.Remove(paper);
        }

        

        public async Task<bool> SaveAsync()
        {
            return await context.SaveChangesAsync() >= 0;
        }

        
    }
}
