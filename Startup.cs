using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Rotativa.AspNetCore;
using Westwind.AspNetCore.LiveReload;
using Microsoft.AspNetCore.Authentication.Cookies;
using ICICE.Models;
using Microsoft.Extensions.FileProviders;
using System.IO;
using Microsoft.AspNetCore.Mvc.Filters;
//using ICICE.Services;
using Microsoft.AspNetCore.Mvc;
//using static ICICE.Interface;


   namespace ICICE
{
    public class Startup
    {   
       // private EmailSettings emailService;
        public Startup(IConfiguration configuration)
        {
         //   emailService = new EmailSettings();
            Configuration = configuration;
           // Configuration.GetSection("EmailSettings").Bind(emailService);
        }
        

        public IConfiguration Configuration { get; }
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.  
                options.CheckConsentNeeded = Context => false;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

           
            services.AddDistributedMemoryCache();

            services.AddSession();

            // services.AddDetection();

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromHours(12);//You can set Time   
            });

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddDbContext<DBContext>(options => options.UseSqlServer(Configuration.GetConnectionString("ICICE_ConnectionString")));
            //services.AddDbContext<DBContext>(options => options.UseSqlServer(Configuration.GetConnectionString("ICICE_ConnectionString")));



            services.AddAuthorization(options =>
            {
                //options.AddPolicy("AdminRoles",
                //     policy => policy.RequireRole(GeneralClass.ADMIN, GeneralClass.DIRECTOR, GeneralClass.SUPPORT, GeneralClass.SUPER_ADMIN, GeneralClass.IT_ADMIN));

                  });

            services.AddLiveReload(config =>
            {
            });

            //services.AddRazorPages().AddRazorRuntimeCompilation();
            //services.AddMvc().AddRazorRuntimeCompilation();
            services.AddControllersWithViews().AddRazorRuntimeCompilation(); 

            services.AddHttpContextAccessor();
            services.AddHttpClient();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = new PathString("/Auth/Login");
                });
            services.AddMvc(options => options.EnableEndpointRouting = false)
                           .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        //Scaffold-DbContext "Server=LAPTOP-02DHJQ6V\SQLEXPRESS;Trusted_Connection=True;Initial Catalog=ICICEC;MultipleActiveResultSets=False;" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models -context DBContext -UseDatabaseNames -Project ICICE -force
        // public void Configure(IApplicationBuilder app, IWebHostEnvironment env, depot context)
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, DBContext context)
        {
            context.Database.EnsureCreated();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseDeveloperExceptionPage();
                //app.UseExceptionHandler();
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }



            app.UseSession();
            app.UseCookiePolicy();
            app.UseAuthentication();
            //app.UseMvc();

           app.UseLiveReload();

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot"))
            });
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}/{option?}/{option2?}/{option3?}/{option4?}/{option5?}/{option6?}");
            });

            RotativaConfiguration.Setup(env.ContentRootPath, "wwwroot/Rotativa");

        }
    }
}