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
        [AllowAnonymous]
        [HttpPost, Route("requesToken",Name = nameof(RequestToken))]
        public ActionResult RequestToken([FromBody] LoginRequestDto request)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest("Invalid Request");
            }

            string token = null;
            if(authenticateService.IsAuthenticated(request, out token))
            {
                return Ok(token);
            }

            return BadRequest("InValid Request");
        }
    }
}
