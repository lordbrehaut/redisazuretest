using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using DataAccessAPI;
using DataAccessAPI.Models;
using DataAccessAPI.Services;
using DataAccessAPI.Services.Interfaces;
using DataAccessAPI.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Data_Access_API
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
            var databaseConfig = new DatabaseSettings();
            Configuration.Bind("DatabaseSettings", databaseConfig);
            services.AddDbContext<CountryContext>(options => options.UseSqlServer(databaseConfig.ConnectionString));
            services.AddTransient<IRedisService, RedisService>();
            services.AddTransient<ISignedRedisService, SignedRedisService>();
            services.AddTransient<ISignedService, SignedService>();
            services.AddTransient<IEncryptionService, EncryptionService>();
            services.AddTransient<IEncryptedRedisService, EncryptedRedisService>();

            var authKey = new byte[32];
            new RNGCryptoServiceProvider().GetBytes(authKey);
            services.AddSingleton(new AuthenticationSettings { AuthKey = authKey });

            var cryptKey = new byte[32];
            new RNGCryptoServiceProvider().GetBytes(cryptKey);
            services.AddSingleton(new EncryptionSettings { CryptKey = cryptKey });

            var config = new CacheSettings();
            Configuration.Bind("CacheSettings", config);
            services.AddSingleton(config);

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
