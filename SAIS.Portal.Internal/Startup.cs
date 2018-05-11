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
using SAIS.Portal.Data;
using SAIS.Portal.Models;
using SAIS.Portal.Services;
using SAIS.Data;
using SAIS.Service;
using SAIS.Model;
using SAIS.Portal.Util;
using SAIS.Model.Audit;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.AspNetCore.Server.Kestrel.Https;

namespace SAIS.Portal
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
            services.AddDbContext<SaisContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddDbContext<AuditContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // Add application services.
            services.AddTransient<IEmailSender, EmailSender>();

            services.AddScoped<ContextService>();
            services.AddScoped<Register1Service>();

            services.AddScoped<AuditAttribute>();
            services.AddScoped<AuditModel>();


            services.Configure<HttpsConnectionAdapterOptions>(options =>
            {
                options.ClientCertificateMode = ClientCertificateMode.RequireCertificate;
                options.CheckCertificateRevocation = false;
                options.ClientCertificateValidation = (certificate2, chain, policyErrors) =>
                {
                    // accept any cert (testing purposes only)
                    return true;
                };
            });

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

            StaticFileOptions options = new StaticFileOptions();
            FileExtensionContentTypeProvider typeProvider = new FileExtensionContentTypeProvider();
            if (!typeProvider.Mappings.ContainsKey(".vue"))
            {
                typeProvider.Mappings.Add(".vue", "application/js");
            }
            options.ContentTypeProvider = typeProvider;

            app.UseStaticFiles(options);

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            //app.UseMiddleware<AuditMiddleware>();
        }

    }
}
