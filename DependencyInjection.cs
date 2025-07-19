using Microsoft.AspNetCore.RateLimiting;
using Microsoft.OpenApi.Models;
using SurveyBasket.Services;
using System.Threading.RateLimiting;


namespace SurveyManagementSystemApi
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDependencies(this IServiceCollection services, IConfiguration configuration)
        {
           //services.Configure<JwtToken>(configuration.GetSection(""));
            var ConnectionStrings = configuration.GetConnectionString("Connection") ?? throw new InvalidOperationException("your ConnectionString is invalid ");
            services.AddDbContext<AppDbContext>(options => options.UseSqlServer(ConnectionStrings));
            services.AddEndpointsApiExplorer();
            //services.AddSwaggerGen();
            services.AddSwaggerGenDependecies();
            services.AddHangfire(conf =>
            {
                conf.UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSqlServerStorage(configuration.GetConnectionString("BackgroundJobs"));

            });

            services.AddProblemDetails();
            services.AddMapsterConfig();
            services.AddHealthChecks().
               AddDbContextCheck<AppDbContext>().
               AddHangfire(options =>
               {
                   options.MinimumAvailableServers = 1;

               });
          
            services.AddControllers();
            services.AddCors(options =>
            options.AddDefaultPolicy(builder =>
                   builder
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .WithOrigins(configuration.GetSection("AllowedOrigins").Get<string[]>()!)
                 )
            );
            services.AddRateLimiter(options => 
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                options.AddFixedWindowLimiter("FixedWindowLimiter", options => 
                {
                    options.QueueLimit = 10;
                    options.PermitLimit = 30;
                    options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    options.Window = TimeSpan.FromMinutes(3);
                });

                options.AddPolicy("IpLimiter", httpContext =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: httpContext.Connection.RemoteIpAddress?.ToString(),
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 2,
                            Window = TimeSpan.FromSeconds(20)
                        }
                    )
                );

                options.AddPolicy("UserLimiter", httpContext =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: httpContext.User.GetUserId(),
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 2,
                            Window = TimeSpan.FromSeconds(20)
                        }
                    )
                );

                options.AddConcurrencyLimiter("Concurrency", options =>
                {
                    options.PermitLimit = 1000;
                    options.QueueLimit = 100;
                    options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                });

            });

            services.AddFluentValidationAutoValidation().AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddMapster();
            services.AddDistributedMemoryCache();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<ICacheService, CacheService>();
            services.AddExceptionHandler<GlobalExceptionHandler>(); 

            services.AddScoped<IEmailSender, EmailService>();
            services.AddScoped<IVoteServices, VoteServices>();
            services.AddScoped<IQuestion, QuestionServices>();
            services.AddScoped<IPollService, PollService>();
            services.AddScoped<IResultServices, ResultServices>();
            services.AddScoped<IAuthService ,LoginService>();
            services.AddSingleton<IJwtToken, JwtToken>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IUsersService, UsersService>();
            services.AddScoped<IUsersManager, UsersManager>();
            services.AddHttpContextAccessor();
            services.AddIdentity<UserIdentity, UserRoles>()
                .AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();
            var jwt = configuration.GetSection("Jwt").Get<Jwt>();
            services.Configure<MailSettings>(configuration.GetSection(nameof(MailSettings)));
            services.Configure<Jwt>(configuration.GetSection(nameof(Jwt)));

            services.AddTransient<IAuthorizationHandler, PermissionAuthorizationHandler>();
            services.AddTransient<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).
                AddJwtBearer(
                optins =>
               {
                optins.SaveToken = true;
                    optins.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = true,
                        ValidateIssuer = true,
                        ValidateIssuerSigningKey = true,
                        ValidateLifetime = true,  
                        ValidAudience = jwt!.Audience,
                        ValidIssuer = jwt.Issuer,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key)),
                };
            });

   

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequiredLength = 8;
                options.SignIn.RequireConfirmedEmail = true;
                options.User.RequireUniqueEmail = true;
            }


            );

            services.AddHangfire(conf =>
            {
                conf.UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSqlServerStorage(configuration.GetConnectionString("BackgroundJobs"));

            });

            return services;
        }






        private static IServiceCollection AddMapsterConfig(this IServiceCollection services)
        {
             var mappingConfig = TypeAdapterConfig.GlobalSettings;
             mappingConfig.Scan(Assembly.GetExecutingAssembly());

            services.AddSingleton<IMapper>(new Mapper(mappingConfig));

            return services;
        }
        private static IServiceCollection AddSwaggerGenDependecies(this IServiceCollection Services)
        {


            Services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "ToDo API",
                    Description = "An ASP.NET Core Web API for managing ToDo items",
                    TermsOfService = new Uri("https://example.com/terms"),
                    Contact = new OpenApiContact
                    {
                        Name = "Example Contact",
                        Url = new Uri("https://example.com/contact")
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Example License",
                        Url = new Uri("https://example.com/license")
                    }
                });
            });
            return Services;
        }

        private static IServiceCollection AddSwaggerServices(this IServiceCollection services)
        {
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

            services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "ToDo API",
                    Description = "An ASP.NET Core Web API for managing ToDo items",
                    TermsOfService = new Uri("https://example.com/terms"),
                    Contact = new OpenApiContact
                    {
                        Name = "Example Contact",
                        Url = new Uri("https://example.com/contact")
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Example License",
                        Url = new Uri("https://example.com/license")
                    }
                });

                //var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                //options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));


            });

          
            return services;
        }


    }

}