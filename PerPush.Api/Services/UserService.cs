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
        public async Task<IEnumerable<Paper>> GetUserPapersAsync(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(userId));
            }
            return await context.papers
                .Where(x => x.UserId == userId)
                .OrderBy(x => x.StartTime)
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
                .Where(x => x.UserId == userId && x.Id == paperId)
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
        public void RegisteredUser(User user)
        {
            if(user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            user.Id = Guid.NewGuid();

            context.Add(user);
        }

        public void UpdateUserInfo(User user)
        {

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
