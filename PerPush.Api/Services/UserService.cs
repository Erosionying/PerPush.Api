using Microsoft.EntityFrameworkCore;
using PerPush.Api.Data;
using PerPush.Api.Entities;
using PerPush.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PerPush.Api.Services
{
    public class UserService : IUserService
    {
        private readonly PerPushDbContext context;

        public UserService(PerPushDbContext context)
        {
            this.context = context ?? 
                throw new ArgumentNullException(nameof(context));
        }

        public async Task<User> GetUserInfoAsync(Guid userId)
        {
            if(userId == Guid.Empty)
            {
                throw  new ArgumentNullException(nameof(userId));
            }

            var userInfo = await context.users.FirstOrDefaultAsync(x => x.Id == userId);

            return userInfo;
        }
        public async Task<IEnumerable<Paper>> GetUserPrivatePapersAsync(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(userId));
            }
            return await context.papers
                .Where(x => x.UserId == userId && x.Auth == false)
                .OrderBy(x => x.StartTime)
                .ToListAsync();
        }
        public async Task<IEnumerable<Paper>> GetUserPrivatePapersAsync(Guid userId, IEnumerable<Guid> papersId)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(userId));
            }
            if(papersId == null)
            {
                throw new ArgumentNullException(nameof(papersId));
            }
            return await context.papers
                .Where(x => x.UserId == userId && x.Auth == false && papersId.Contains(x.Id))
                .OrderBy(x => x.Title)
                .ToListAsync();
        }

        public async Task<IEnumerable<Paper>> GetUserPublicPaperAsync(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(userId));
            }
            var papers = await context.papers
                .Where(x => x.UserId == userId && x.Auth == true)
                .ToListAsync();

            return papers;
        }
        public async Task<IEnumerable<Paper>> GetUserPublicPaperAsync(Guid userId, IEnumerable<Guid> papersId)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(userId));
            }
            if (papersId == null)
            {
                throw new ArgumentNullException(nameof(papersId));
            }
            return await context.papers
                .Where(x => x.UserId == userId && x.Auth == true && papersId.Contains(x.Id))
                .OrderBy(x => x.Title)
                .ToListAsync();
        }
        public async Task<Paper> GetPaperAsync(Guid userId, Guid paperId)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(userId));
            }
            if (paperId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(paperId));
            }

            return await context.papers
                .Where(x => x.UserId == userId && x.Id == paperId && x.Auth == true)
                .FirstOrDefaultAsync();
        }
        public bool IsValid(LoginRequestDto req)
        {
            var user =  context.users
                .Where(x => x.Email == req.UserName)
                .Where(x => x.password == req.Password)
                .SingleOrDefault();
            if(user == null)
            {
                return false;
            }

            return true;
        }

        //add user
        public async Task<bool> RegisteredUser(User user)
        {
            if(user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var queryUser = await context.users.Where(x => x.Email == user.Email).FirstOrDefaultAsync();

            if(!(queryUser == null))
            {
                return false;
            }
            user.Id = Guid.NewGuid();
            context.Add(user);
            return true;
        }

        public void UpdateUserInfo(User user)
        {

        }
        public void AddPaper(Guid userId, Paper paper)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(userId));
            }
            if (paper == null)
            {
                throw new ArgumentNullException(nameof(paper));
            }

            paper.UserId = userId;
            paper.Id = Guid.NewGuid();
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

        public async Task<bool> UserExistsAsync(Guid userId)
        {
            if(userId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(userId));
            }

            return await context.users.AnyAsync(x => x.Id == userId);
        }
    }
}
