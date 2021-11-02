using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VieTrails_API.Data;
using VieTrails_API.Mapper;
using VieTrails_API.Repository;
using VieTrails_API.Repository.IRepository;

namespace VieTrails_API
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
            services.AddCors();
            services.AddDbContext<ApplicationDbContext>
                (options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<INationalParkRepository, NationalParkRepository>();
            services.AddScoped<ITrailRepository, TrailRepository>();
            services.AddScoped<IUserRepository, UserRepository>();

            services.AddAutoMapper(typeof(Mappings));
            services.AddApiVersioning(options =>
            {
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ReportApiVersions = true;
            });

            services.AddVersionedApiExplorer(options => options.GroupNameFormat = "'v'VVV");
            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
            services.AddSwaggerGen();

            var appSettingSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingSection);

            var appSettings = appSettingSection.Get<AppSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
            //services.AddSwaggerGen(c =>
            //{
            //    c.SwaggerDoc("VieTrailsOpenAPISpec", 
            //        new OpenApiInfo { 
            //            Title = "VieTrails API", 
            //            Version = "1" ,
            //            Description = "VieTrails API .NET 5",
            //            Contact = new OpenApiContact()
            //            {
            //                Email = "huynhngocthao989@gmail.com",
            //                Name = "Thao Huynh",
            //                Url = new Uri("https://github.com/thaohuynh44")
            //            },
            //            License = new OpenApiLicense()
            //            {
            //                Name = "MIT License",
            //                Url = new Uri("https://opensource.org/licenses/MIT")
            //            }
            //        });

                //c.SwaggerDoc("VieTrailsOpenAPISpecTrails",
                //    new OpenApiInfo
                //    {
                //        Title = "VieTrails API Trails",
                //        Version = "1",
                //        Description = "VieTrails API .NET 5 Trails",
                //        Contact = new OpenApiContact()
                //        {
                //            Email = "huynhngocthao989@gmail.com",
                //            Name = "Thao Huynh",
                //            Url = new Uri("https://github.com/thaohuynh44")
                //        },
                //        License = new OpenApiLicense()
                //        {
                //            Name = "MIT License",
                //            Url = new Uri("https://opensource.org/licenses/MIT")
                //        }
                //    });

            //    var xmlCommentFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            //    var cmlCommentsFullPath = Path.Combine(AppContext.BaseDirectory, xmlCommentFile);
            //    c.IncludeXmlComments(cmlCommentsFullPath);
            //});

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    foreach (var desc in provider.ApiVersionDescriptions)
                    {
                        options.SwaggerEndpoint($"/swagger/{desc.GroupName}/swagger.json",
                            desc.GroupName.ToUpperInvariant());
                    }
                });

                //app.UseSwaggerUI(c =>
                //{
                //    c.SwaggerEndpoint("/swagger/VieTrailsOpenAPISpec/swagger.json", "VieTrails API NP");
                //    //c.SwaggerEndpoint("/swagger/VieTrailsOpenAPISpecTrails/swagger.json", "VieTrails API Trails");
                //});
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
