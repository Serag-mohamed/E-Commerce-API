using E_Commerce.Data;
using E_Commerce.Entities;
using E_Commerce.Repositories;
using E_Commerce.Services;
using E_Commerce.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace E_Commerce.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<CategoryService>();
            services.AddScoped<ProductService>();
            services.AddScoped<OrderService>();
            services.AddScoped<CartService>();
            services.AddScoped<ReviewService>();
            services.AddScoped<AuthService>();
            services.AddScoped<AccountService>();
            services.AddScoped<StatisticsService>();

            return services;
        }

        public static IServiceCollection AddIdentityServices(this IServiceCollection services)
        {
            services.AddIdentityCore<ApplicationUser>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 8;

                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;

            })
                .AddRoles<IdentityRole>()
                .AddSignInManager()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            return services;
        }

        public static IServiceCollection AddAuthServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddOptions<JwtSettings>()
            .Bind(config.GetSection("JWT"))
            .ValidateOnStart();

            var jwtSettings = config.GetSection("JWT").Get<JwtSettings>()!;

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtSettings.Audience,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
                    ClockSkew = TimeSpan.Zero
                };
            });

            return services;
        }
    }
}