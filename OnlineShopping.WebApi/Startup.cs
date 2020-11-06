using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OnlineShopping.FrontEnd.Api.Data;
using OnlineShopping.FrontEnd.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using AutoMapper;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Identity;
using OnlineShopping.FrontEnd.Api.Models.Entities;
using OnlineShopping.Models.ViewModels;
using OnlineShopping.FrontEnd.Api.Models;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using OnlineShopping.FrontEnd.Api.Auth;
using OnlineShopping.FrontEnd.Api.Helpers;
using Swashbuckle.AspNetCore.Swagger;
using NLog;
using System.IO;
using OnlineShopping.Email;

namespace OnlineShopping.WebApi
{
    public class Startup
    {
        private const string SecretKey = "iNivDmHLpUA223sqsfhqGbMRdRj1PVkH"; // todo: get this from somewhere secure
        private readonly SymmetricSecurityKey _signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SecretKey));
        public Startup(IConfiguration configuration)
        {
            LogManager.LoadConfiguration(String.Concat(Directory.GetCurrentDirectory(), "/nlog.config"));
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add Cors
            services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            }));

            services.AddTransient<IProductRepository>(x => new ProductRepository(Configuration.GetConnectionString("ShoppingConnectionString")));
            services.AddTransient<ICategoryRepository>(x => new CategoryRepository(Configuration.GetConnectionString("ShoppingConnectionString")));
            services.AddTransient<ICustomerRepository>(x => new CustomerRepository(Configuration.GetConnectionString("ShoppingConnectionString")));

            //services.AddSingleton<IConfiguration>(Configuration);
            services.AddSingleton<IJwtFactory, JwtFactory>();
            services.AddSingleton<ILoggerManager, LoggerManager>(); //NLog Error Logging
            services.AddSingleton<IEmailSender, EmailSender>(); //SendGrid EMail

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            // Add framework services.
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("ShoppingConnectionString"),
                b => b.MigrationsAssembly("OnlineShopping.FrontEnd.Api")));

            services.AddIdentity<AppUser, IdentityRole>
                (o =>
                {
                    // configure identity options
                    o.Password.RequireDigit = false;
                    o.Password.RequireLowercase = false;
                    o.Password.RequireUppercase = false;
                    o.Password.RequireNonAlphanumeric = false;
                    o.Password.RequiredLength = 6;
                })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddMvc().AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<Startup>());

            // jwt wire up
            // Get options from app settings
            var jwtAppSettingOptions = Configuration.GetSection(nameof(JwtIssuerOptions));

            // Configure JwtIssuerOptions
            services.Configure<JwtIssuerOptions>(options =>
            {
                options.Issuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)];
                options.Audience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)];
                options.SigningCredentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256);
            });

            services.AddAuthentication()
                .AddJwtBearer(jwt =>
                {
                    jwt.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)],

                        ValidateAudience = true,
                        ValidAudience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)],

                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = _signingKey,

                        RequireExpirationTime = false,
                        ValidateLifetime = false,
                        ClockSkew = TimeSpan.Zero
                    };
                }).AddGoogle(googleOptions =>
                {
                    //googleOptions.ClientId = Configuration["Authentication:Google:ClientId"];
                    //googleOptions.ClientSecret = Configuration["Authentication:Google:ClientSecret"];

                    googleOptions.ClientId = "83783849834-g0rn8rd8lj2b5csq56v0u9nps84i8eif.apps.googleusercontent.com";
                    googleOptions.ClientSecret = "R2vUEv2BUuMl9n--hh7k457d";
                }); ;
            //

            // api user claim policy
            services.AddAuthorization(auth =>
            {
                auth.AddPolicy("ApiUser", new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
                        .RequireAuthenticatedUser()
                        .Build());
            });
            //

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Online Shopping API", Version = "V1" });
                c.AddSecurityDefinition("Bearer",
                new ApiKeyScheme
                {
                    In = "header",
                    Description = "Please enter into field the word 'Bearer' following by space and JWT",
                    Name = "Authorization",
                    Type = "apiKey"
                });
                c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>> {
                    { "Bearer", Enumerable.Empty<string>() },
                });
            });
            //

            services.AddAutoMapper(typeof(Startup));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                c.RoutePrefix = string.Empty;
            });


            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            //Custom Exception Handling
            app.ConfigureCustomExceptionMiddleware();

            // Enable Cors
            app.UseCors("MyPolicy");

            app.UseAuthentication();
            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
