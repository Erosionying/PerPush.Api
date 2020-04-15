using PerPush.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PerPush.Api.Services
{
    public interface IAuthenticateService
    {
        bool IsAuthenticated(LoginRequestDto request, out string token);
    }
}
