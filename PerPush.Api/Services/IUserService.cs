using PerPush.Api.DtoParameters;
using PerPush.Api.Entities;
using PerPush.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PerPush.Api.Services
{
    public interface IUserService
    {
        //--------------------Account Management----------------
        Task<User> GetUserInfoAsync(Guid userId);
        void UpdateUserInfo(User user);
        Task<bool> RegisteredUser(User user);
        //---------------------Paper------------------
        
        //public or private File under user name
        Task<IEnumerable<Paper>> GetUserPrivatePapersAsync(Guid userId, PaperDtoParameters parameters);
        Task<IEnumerable<Paper>> GetUserPrivatePapersAsync(Guid userId, IEnumerable<Guid> papersId);
        Task<IEnumerable<Paper>> GetUserPublicPaperAsync(Guid userId, PaperDtoParameters parameters);
        Task<IEnumerable<Paper>> GetUserPublicPaperAsync(Guid userId, IEnumerable<Guid> papersId);
        Task<Paper> GetPaperAsync(Guid userId, Guid paperId);
        //------------ Need to execute SaveAsync Method
        void AddPaper(Guid userId, Paper paper);
        void UpdatePaper(Paper paper);
        void DeletePaper(Paper paper);
        //--------------------------------------------
        Task<bool> UserExistsAsync(Guid userId);
        //Save Data to DataBase
        Task<bool> SaveAsync();

        //----------Determine whether the login parameters are valid-------
        bool IsValid(LoginRequestDto req);
        
    }
}
