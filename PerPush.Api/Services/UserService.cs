﻿using Microsoft.EntityFrameworkCore;
using PerPush.Api.Data;
using PerPush.Api.DtoParameters;
using PerPush.Api.Entities;
using PerPush.Api.Helpers;
using PerPush.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PerPush.Api.Services
{
    public class UserService : IUserService
    {
        private readonly PerPushDbContext context;
        private readonly IPropertyMappingService propertyMappingService;

        public UserService(PerPushDbContext context, IPropertyMappingService propertyMappingService)
        {
            this.context = context ?? 
                throw new ArgumentNullException(nameof(context));
            this.propertyMappingService = propertyMappingService ??
                throw new ArgumentNullException(nameof(propertyMappingService));
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
        public async Task<PagedList<Paper>> GetUserPrivatePapersAsync(Guid userId, PaperDtoParameters parameters)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(userId));
            }
            

            var items = context.papers as IQueryable<Paper>;
            items = items.Where(x => x.UserId == userId && x.Auth == false);

            if(!string.IsNullOrWhiteSpace(parameters.Title))
            {
                parameters.Title = parameters.Title.Trim();
                items = items.Where(x => x.Title.Contains(parameters.Title));
            }

            if(!string.IsNullOrWhiteSpace(parameters.Lable))
            {
                parameters.Lable = parameters.Lable.Trim();
                items = items.Where(x => x.Lable.Contains(parameters.Lable));
            }

            if(!string.IsNullOrWhiteSpace(parameters.SearchTerm))
            {
                parameters.SearchTerm = parameters.SearchTerm.Trim();
                items = items.Where(x => x.Title.Contains(parameters.SearchTerm) ||
                x.Description.Contains(parameters.SearchTerm) ||
                x.Lable.Contains(parameters.SearchTerm));
            }

            //var user = context.papers.First(x => x.Likes == 5).Author;
            
            var mappingDictionary = propertyMappingService.GetPropertyMapping<PaperDto, Paper>();
            items = items.ApplySort(parameters.OrderBy, mappingDictionary);
            //var papers = items.ToListAsync();

            var returnItems = await PagedList<Paper>.CreateAsync(items, parameters.PageNumber, parameters.PageSize);

            //Take the Data From DataBase
            foreach (var paper in returnItems)
            {
                paper.Author = await context.users
                    .Where(x => x.Id == paper.UserId)
                    .FirstOrDefaultAsync();
            }

            return returnItems;

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
                .OrderBy(x => x.StartTime)
                .ToListAsync();
        }

        public async Task<PagedList<Paper>> GetUserPublicPaperAsync(Guid userId, PaperDtoParameters parameters)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(userId));
            }
            /*
            if (parameters == null)
            {
                return await context.papers
                .Where(x => x.UserId == userId && x.Auth == true)
                .OrderBy(x => x.StartTime)
                .ToListAsync();
            }*/

            var items = context.papers as IQueryable<Paper>;
            items = items.Where(x => x.UserId == userId && x.Auth == true);

            if (!string.IsNullOrWhiteSpace(parameters.Title))
            {
                parameters.Title = parameters.Title.Trim();
                items = items.Where(x => x.Title.Contains(parameters.Title));
            }

            if (!string.IsNullOrWhiteSpace(parameters.Lable))
            {
                parameters.Lable = parameters.Lable.Trim();
                items = items.Where(x => x.Lable.Contains(parameters.Lable));
            }

            if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
            {
                parameters.SearchTerm = parameters.SearchTerm.Trim();
                items = items.Where(x => x.Title.Contains(parameters.SearchTerm) ||
                x.Description.Contains(parameters.SearchTerm) ||
                x.Lable.Contains(parameters.SearchTerm));
            }
            //Get Mapping relations
            var mappingDictionary = propertyMappingService.GetPropertyMapping<PaperDto, Paper>();
            //Order By
            items = items.ApplySort(parameters.OrderBy, mappingDictionary);

            var returnItems = await PagedList<Paper>.CreateAsync(items, parameters.PageNumber, parameters.PageSize);

            //Take the Data From DataBase
            foreach (var paper in returnItems)
            {
                paper.Author = await context.users
                    .Where(x => x.Id == paper.UserId)
                    .FirstOrDefaultAsync();
            }

            return returnItems;
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
                .OrderBy(x => x.StartTime)
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
                .Where(x => x.UserId == userId && x.Id == paperId)
                .FirstOrDefaultAsync();
        }
        public Guid IsValid(LoginRequestDto req)
        {
            req.Password = EncryptString(req.Password);

            var user = context.users
                .Where(x => x.Email == req.UserName)
                .Where(x => x.password == req.Password)
                .SingleOrDefault();

            if (user == null)
            {
                return Guid.Empty;
            }

            return user.Id;
        }

        //add user
        public async Task<bool> RegisteredUser(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.Email = user.Email.Trim();

            var queryUser = await context.users.Where(x => x.Email == user.Email)
                .FirstOrDefaultAsync();

            if (!(queryUser == null))
            {
                return false;
            }

            user.Id = Guid.NewGuid();
            user.password = EncryptString(user.password);

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

        public async Task<Guid> GetUserIdAsync(string userName)
        {
            if (userName == null)
            {
                throw new ArgumentNullException(userName);
            }

            var Id = await (from u in context.users
                     .Where(u => u.Email.Equals(userName))
                            select u.Id).SingleOrDefaultAsync();

            return Id;

        }

        private string EncryptString(string str)
        {
            MD5 md5 = MD5.Create();

            byte[] byteOld = Encoding.UTF8.GetBytes(str);

            byte[] byteNew = md5.ComputeHash(byteOld);

            StringBuilder sb = new StringBuilder();
            foreach (byte b in byteNew)
            {

                sb.Append(b.ToString("x2"));
            }

            return sb.ToString();
        }
    }
}
