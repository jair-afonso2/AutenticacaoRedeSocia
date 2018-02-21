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
using PRLoginRedes.Data;
using PRLoginRedes.Models;
using PRLoginRedes.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;

namespace PRLoginRedes
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

            // services.AddDbContext<ApplicationDbContext>(options =>
            // options.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            //Chamando o serviço de Autenticação do Facebook
            #region Facebook
            services.AddAuthentication().AddFacebook(facebookOptions =>
            {
                facebookOptions.AppId = Configuration["Authentication:Facebook:AppId"];//Authentication é a referencia para inserir o App Id e App Secret no .jason
                facebookOptions.AppSecret = Configuration["Authentication:Facebook:AppSecret"];
            });
            #endregion

            //Chamando o serviço de Autenticação do Google
            #region Google
            services.AddAuthentication().AddGoogle(googleOptions =>
            {
                googleOptions.ClientId = Configuration["Authentication:Google:ClientId"];
                googleOptions.ClientSecret = Configuration["Authentication:Google:ClientSecret"];
            });
            #endregion

            //Chamando o serviço de Autenticação da Microsoft
            #region Microsoft
            services.AddAuthentication().AddMicrosoftAccount(microsoftOptions =>
            {
                microsoftOptions.ClientId = Configuration["Authentication:Microsoft:ApplicationId"];
                microsoftOptions.ClientSecret = Configuration["Authentication:Microsoft:Password"];
            });
            #endregion

            //Chamando o serviço de Autenticação do Twitter
            #region Twitter
            services.AddAuthentication().AddTwitter(twitterOptions =>
            {
                twitterOptions.ConsumerKey = Configuration["Authentication:Twitter:ConsumerKey"];
                twitterOptions.ConsumerSecret = Configuration["Authentication:Twitter:ConsumerSecret"];
            });
            #endregion
         

            // Criar seu aplicativo em https://developer.github.com
            // baixar o pacote AspNet.Security.OAuth.GitHub
            // rodar no terminal dotnet add package AspNet.Security.OAuth.GitHub
            // rodar dotnet restore
            // reiniciar o visual studio
            #region Twitter
            services.AddAuthentication().AddGitHub(githubOptions =>
            {
                githubOptions.ClientId = Configuration["Authentication:GitHub:clientid"];
                githubOptions.ClientSecret = Configuration["Authentication:GitHub:clientSecret"];
            });
            #endregion

            #region Linkedin
            // Criar seu aplicativo em https://www.linkedin.com/developer/apps
            // baixar o pacote AspNet.Security.OAuth.LinkedIn
            // rodar no terminal dotnet add package AspNet.Security.OAuth.LinkedIn --version 2.0.0-rc2-final
            //rodar dotnet restore
            // reiniciar o visual studio
            services.AddAuthentication().AddLinkedIn(options =>  
            {  
                options.ClientId = Configuration["Authentication:linkedin:Clientid"];  
                options.ClientSecret = Configuration["Authentication:linkedin:ClientSecret"];  
            }); 
            #endregion

            // Add application services.
            services.AddTransient<IEmailSender, EmailSender>();

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
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
