using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Net.Http.Headers;
using ReviewsDeGames.Database;
using ReviewsDeGames.Models;
using ReviewsDeGames.Repository;
using ReviewsDeGames.Services;
using System.Security.Policy;
using System.Text.RegularExpressions;

namespace ReviewsDeGames.Helpers
{
    public static class Extensions
    {
        #region Startup
        public static IServiceCollection AddMyIdentity(this IServiceCollection services)
        {
            services.AddIdentity<User, IdentityRole>().AddEntityFrameworkStores<ReviewGamesContext>();
            services.Configure<IdentityOptions>(opt =>
            {
                opt.Password.RequiredLength = 6;
                opt.Password.RequireDigit = true;
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequireUppercase = false;


            });

            //Redefinindo redirecionamento de acesso negado para statuscode 401
            services.ConfigureApplicationCookie(opt =>
            {
                opt.Events.OnRedirectToLogin = context =>
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return Task.CompletedTask;
                };
            });
            return services;
        }

        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IImagesRepository, ImagesRepository>();
            services.AddScoped<IPostRepository, PostRepository>();

            return services;
        }
        public static IServiceCollection AddValidations(this IServiceCollection services)
        {
            services.AddValidatorsFromAssemblyContaining<UserRegisterDto>();
            services.AddValidatorsFromAssemblyContaining<ImageRequestDto>();
            services.AddValidatorsFromAssemblyContaining<PostRequestDto>();
            return services;
        }

        public static IServiceCollection AddGeneralServices(this IServiceCollection services)
        {
            services.AddScoped<DbInit>();
            services.AddSingleton<IDescribesService, DescribesService>();
            services.AddSingleton<IHostImageService, HostImageService>();

            return services;
        }

        public static IServiceCollection AddFormattersForOData(this IServiceCollection services)
        {

            services.AddMvcCore(options =>
            {
                foreach (var formatter in options.OutputFormatters.OfType<OutputFormatter>().Where(f => f.SupportedMediaTypes.Count == 0))
                {
                    formatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/prs.odatatestxx-odata"));
                }
                foreach (var formatter in options.InputFormatters.OfType<InputFormatter>().Where(f => f.SupportedMediaTypes.Count == 0))
                {
                    formatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/prs.odatatestxx-odata"));
                }
            });

            return services;
        }

        public static void AwakeDB(this WebApplication app, bool seed, bool restart)
        {
            if (!(restart || seed))
                return;

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var dbInit = services.GetRequiredService<DbInit>();
                Task.Run(() => dbInit.Initialize(seed, restart)).Wait();

            }
        }

        public static string Placeholder(this string str, params string[] args)
        {
            var pattern = @"\{([^}]*)\}";
            var count = 0;
            var formated = Regex.Replace(str, pattern, match =>
            {
                return "{" + count++ + "}";
            });
            return string.Format(formated, args);
        }

        /// <summary>
        /// Converte para uma string base64 segura para urls, sem {/, +, =}
        /// </summary>
        public static string ToSafeBase64(this Guid guid)
        {
            return Convert.ToBase64String(guid.ToByteArray()).Substring(0, 22)
                .Replace("+", "_")
                .Replace("/", ",")
                .Replace("=", "");
        }

        #endregion

        #region Libs
        
        public static IRuleBuilderOptions<T, string> SupportedImageUrl<T>(this IRuleBuilder<T, string> ruleBuilder, IEnumerable<string> supportedExtensions)
        {
            return ruleBuilder.Must(url =>
            {
                url = url.Trim().ToLower();
                var validHttp = false;
                if(url.StartsWith("http://") || url.StartsWith("https://"))
                    validHttp = Uri.IsWellFormedUriString(url, UriKind.Absolute);
                else
                    validHttp = Uri.IsWellFormedUriString(url, UriKind.Relative);

                

                
                return validHttp && supportedExtensions.Contains(Path.GetExtension(url));
            });

        }
        public static void AddToModelState(this ValidationResult result, ModelStateDictionary modelState)
        {
            foreach (var error in result.Errors)
            {
                modelState.AddModelError(error.PropertyName, error.ErrorMessage);
            }
        }
        #endregion
    }
}
