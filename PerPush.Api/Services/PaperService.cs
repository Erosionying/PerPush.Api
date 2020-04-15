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
        public async Task<IEnumerable<Paper>> GetPapersAsync()
        {

            var papers = await context.papers.Where(x => x.Auth == true).ToListAsync();
            foreach (var paper in papers)
            {
                paper.Author = await context.users.Where(x => x.Id == paper.UserId).FirstOrDefaultAsync();
            }
            return papers;
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
