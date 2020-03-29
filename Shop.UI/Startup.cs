using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shop.Database;
using Stripe;

namespace Shop.UI
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
            // session added
            services.AddSession(options =>
            {
                options.Cookie.Name = "Cart";
                options.Cookie.MaxAge = TimeSpan.FromDays(365);
            });

            // fixed 404 error
            services.AddControllersWithViews();
            //services.AddMvc(option => option.EnableEndpointRouting = false); // using mvc

            services.AddRazorPages();


            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(Configuration["DefaultConnection"]));


            //Stripe config
            StripeConfiguration.ApiKey = Configuration.GetSection("Stripe")["SecretKey"];

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            // using mvc
            // app.UseMvc();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseSession(); // session added

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers(); // fixed 404 error
                endpoints.MapRazorPages();
            });
        }
    }
}
