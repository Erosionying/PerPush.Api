using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using PerPush.Api.Data;
using PerPush.Api.Models;
using PerPush.Api.Services;

namespace PerPush.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(setup =>
            {
                setup.ReturnHttpNotAcceptable = true;
            }).AddXmlDataContractSerializerFormatters();

            services.AddDbContext<PerPushDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("Localteststring"));
            });

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IPaperService, PaperService>();
            services.AddScoped<IAuthenticateService, TokenAuthenticationService>();
            services.AddScoped<IParameterVerifyService, ParameterVerifyService>();

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());


            //------------------------TokenConfig-------------------------

            services.Configure<TokenManagement>(Configuration.GetSection("tokenConfig"));

            var token = Configuration.GetSection("tokenConfig").Get<TokenManagement>();

            services.AddAuthentication(option =>
            {
                option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(jwtoption =>
            {
                jwtoption.RequireHttpsMetadata = false;
                jwtoption.SaveToken = true;

                //Token Validation Parameters
                jwtoption.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,

                    //Get or set the Microsoft.IdentityModel.Tokens.SecurityKey to be used for signature verification.
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(token.Secret)),

                    ValidIssuer = token.Issuer,
                    ValidAudience = token.Audience,

                    ValidateIssuer = false,
                    ValidateAudience = false

                };

            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
