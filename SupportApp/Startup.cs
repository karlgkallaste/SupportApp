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
using SupportApp.Data;
using Microsoft.EntityFrameworkCore;
using SupportApp.Areas.Identity.Data;
using SupportApp.Models.Categories;
using SupportApp.Models.Comments;
using SupportApp.Models.Tickets;

namespace SupportApp
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
            services.AddControllersWithViews();
            services.AddRazorPages();

            services.AddDbContext<SupportAppContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("SupportAppContext"));
            });
            services.AddScoped<ISupportAppContext, SupportAppContext>();
            
            services.AddDbContext<SupportAppUserContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("SupportAppContext")));

            services.AddDefaultIdentity<SupportAppUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<SupportAppRole>()
                .AddEntityFrameworkStores<SupportAppUserContext>();
            
            services.AddScoped<ICommentFinder, CommentFinder>();
            services.AddScoped<ICommentModifier, CommentModifier>();
            services.AddScoped<ITicketsFinder, TicketsFinder>();
            services.AddScoped<ITicketsModifier, TicketsModifier>();
            services.AddScoped<ICategoryFinder, CategoryFinder>();
            services.AddScoped<ICategoryModifier, CategoryModifier>();
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
