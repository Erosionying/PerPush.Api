using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PerPush.Api.Models;
using PerPush.Api.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PerPush.Api.Controllers
{
    [Route("api/login")]
    [ApiController]
    public class AuthenticationController:ControllerBase
    {
        private readonly IAuthenticateService authenticateService;

        public AuthenticationController(IAuthenticateService authenticateService)
        {
            this.authenticateService = authenticateService ?? 
                throw new ArgumentNullException(nameof(authenticateService));
        }
        //Get Token / Login
        [AllowAnonymous]
        [HttpPost, Route("requestToken",Name = nameof(RequestToken))]
        public ActionResult RequestToken([FromBody] LoginRequestDto request)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest("Invalid Request");
            }

            string token = null;
            var userId = authenticateService.IsAuthenticated(request, out token);
            var newToken = userId + "." + token;
            if(userId != Guid.Empty)
            {
                return Ok(newToken);
            }

            return BadRequest("InValid Request");
        }
    }
}
