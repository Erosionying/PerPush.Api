﻿using PerPush.Api.Entities;
using PerPush.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PerPush.Api.Services
{
    public interface IUserService
    {
        Task<User> GetUserInfoAsync(Guid userId);
        void UpdateUserInfo(User user);
        void RegisteredUser(User user);
        //---------------------Paper------------------
        
        //File under user name
        Task<IEnumerable<Paper>> GetUserPapersAsync(Guid userId);
        Task<IEnumerable<Paper>> GetUserPublicPaperAsync(Guid userId);
        Task<Paper> GetPaperAsync(Guid userId, Guid paperId);
        //--------------------------------------------
        Task<bool> UserExistsAsync(Guid userId);
        Task<bool> SaveAsync();

        //----------判断登陆参数是否有效-------
        bool IsValid(LoginRequestDto req);
        
    }
}
