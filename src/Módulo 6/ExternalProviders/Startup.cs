using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ExternalProviders.Data;
using ExternalProviders.Models;
using ExternalProviders.Services;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace ExternalProviders
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddAuthentication().AddFacebook(facebookOptions =>
            {
                facebookOptions.AppId = Configuration["Authentication:Facebook:AppId"];
                facebookOptions.AppSecret = Configuration["Authentication:Facebook:AppSecret"];

                facebookOptions.Scope.Add("user_birthday");

                facebookOptions.ClaimActions.MapJsonKey(ClaimTypes.Locality, "locale");

                facebookOptions.SaveTokens = true;
            })
            .AddGoogle(googleOptions =>
            {
                googleOptions.ClientId = Configuration["Authentication:Google:ClientId"];

                googleOptions.ClientSecret = Configuration["Authentication:Google:ClientSecret"];

                googleOptions.SaveTokens = true;
            }).
            AddTwitter(twitterOptions => {
                twitterOptions.ConsumerKey = Configuration["Authentication:Twitter:ConsumerKey"];
                twitterOptions.ConsumerSecret = Configuration["Authentication:Twitter:ConsumerSecret"];

                twitterOptions.SaveTokens = true;

                twitterOptions.RetrieveUserDetails = true;

                twitterOptions.ClaimActions.MapJsonKey(ClaimTypes.Email, "email", ClaimValueTypes.Email);
            })
            .AddMicrosoftAccount(maoptions => {
                maoptions.ClientId = Configuration["Authentication:Microsoft:ClientId"];
                maoptions.ClientSecret = Configuration["Authentication:Microsoft:ClientSecret"];

                maoptions.SaveTokens = true;
            })
            .AddLinkedIn(linkedinOptions =>
            {
                linkedinOptions.ClientId = Configuration["Authentication:Linkedin:ClientId"];
                linkedinOptions.ClientSecret = Configuration["Authentication:Linkedin:ClientSecret"];

                linkedinOptions.SaveTokens = true;
            });

            // Add application services.
            services.AddTransient<IEmailSender, EmailSender>();

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
