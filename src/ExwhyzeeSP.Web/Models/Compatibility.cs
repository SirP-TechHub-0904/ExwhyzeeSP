using System.Web.UI.WebControls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ExwhyzeeSP.Core.Models;
using ExwhyzeeSP.Core.Entities;

namespace Microsoft.AspNetCore.Mvc
{
    public class HandleErrorInfo
    {
        public HandleErrorInfo(Exception exception, string controllerName, string actionName)
        {
            Exception = exception;
            ControllerName = controllerName;
            ActionName = actionName;
        }

        public Exception Exception { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
    }

    public class HttpUnauthorizedResult : UnauthorizedResult
    {
    }

    public class HttpNotFoundResult : NotFoundResult
    {
        public HttpNotFoundResult() : base() { }
        public HttpNotFoundResult(string statusDescription) : base() { }
    }

    public class HttpStatusCodeResult : StatusCodeResult
    {
        public HttpStatusCodeResult(int statusCode) : base(statusCode) { }
        public HttpStatusCodeResult(HttpStatusCode statusCode) : base((int)statusCode) { }
        public HttpStatusCodeResult(int statusCode, string statusDescription) : base(statusCode) { }
        public HttpStatusCodeResult(HttpStatusCode statusCode, string statusDescription) : base((int)statusCode) { }
    }
}

namespace Microsoft.AspNetCore.Mvc.Rendering
{
    public static class HtmlHelperCompatibilityExtensions
    {
        public static IHtmlContent Label(this IHtmlHelper html, string expression, object htmlAttributes)
        {
            return html.Label(expression, expression, htmlAttributes);
        }

        public static IHtmlContent Action(this IHtmlHelper html, string actionName, object? routeValues = null)
        {
            return HtmlString.Empty;
        }

        public static IHtmlContent Action(this IHtmlHelper html, string actionName, string controllerName, object? routeValues = null)
        {
            return HtmlString.Empty;
        }

        public static IHtmlContent EnumDropDownListFor<TModel, TEnum>(
            this IHtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TEnum>> expression,
            object? htmlAttributes = null)
        {
            var selectList = htmlHelper.GetEnumSelectList(typeof(TEnum));
            return htmlHelper.DropDownListFor(expression, selectList, htmlAttributes);
        }

        public static IHtmlContent EnumDropDownListFor<TModel, TEnum>(
            this IHtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TEnum>> expression,
            string optionLabel,
            object? htmlAttributes = null)
        {
            var selectList = htmlHelper.GetEnumSelectList(typeof(TEnum));
            return htmlHelper.DropDownListFor(expression, selectList, optionLabel, htmlAttributes);
        }

        public static MvcForm BeginForm(
            this IHtmlHelper htmlHelper,
            string actionName,
            string controllerName,
            object? routeValues,
            FormMethod method,
            object? htmlAttributes)
        {
            return htmlHelper.BeginForm(actionName, controllerName, routeValues, method, antiforgery: null, htmlAttributes: htmlAttributes);
        }
    }
}

namespace Microsoft.AspNetCore.Http
{
    public static class FormFileCompatibilityExtensions
    {
        public static long ContentLength(this IFormFile file) => file.Length;

        public static void SaveAs(this IFormFile file, string filename)
        {
            var dir = Path.GetDirectoryName(filename);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            using var stream = new FileStream(filename, FileMode.Create);
            file.CopyTo(stream);
        }

        public static Stream InputStream(this IFormFile file) => file.OpenReadStream();
    }

    public static class HttpRequestCompatibilityExtensions
    {
        public static string PhysicalApplicationPath(this HttpRequest req) => AppContext.BaseDirectory;
        public static dynamic Browser(this HttpRequest req) => new { Browser = "Chrome" };
        public static Uri Url(this HttpRequest req) => new Uri($"{req.Scheme}://{req.Host}{req.Path}{req.QueryString}");
    }

    public static class HttpResponseCompatibilityExtensions
    {
        public static void AppendHeader(this HttpResponse res, string key, string value) => res.Headers[key] = value;
        public static void TransmitFile(this HttpResponse res, string filename) { }
        public static void End(this HttpResponse res) { }
        public static void Write(this HttpResponse res, string content) => res.WriteAsync(content).GetAwaiter().GetResult();
    }
}

namespace System.Security.Principal
{
    public static class IdentityExtensionsCompatibility
    {
        public static string? GetUserId(this IIdentity identity)
        {
            if (identity is ClaimsIdentity claimsIdentity)
            {
                return claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            }
            return null;
        }

        public static string? GetUserName(this IIdentity identity)
        {
            return identity?.Name;
        }
    }
}

namespace System.Web.Http
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class RoutePrefixAttribute : Attribute
    {
        public string Prefix { get; }
        public RoutePrefixAttribute(string prefix) => Prefix = prefix;
    }

    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class RouteAttribute : Attribute
    {
        public string Template { get; }
        public RouteAttribute(string template) => Template = template;
    }

    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class HttpGetAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class HttpPostAttribute : Attribute { }

    public interface IHttpActionResult : Microsoft.AspNetCore.Mvc.IActionResult { }

    public class ApiController : ControllerBase
    {
    }

    public class HttpResponseException : Exception
    {
        public HttpResponseException(HttpStatusCode statusCode) { }
        public HttpResponseException(Microsoft.AspNetCore.Mvc.IActionResult response) { }
    }
}

namespace System.Web.Http.Description
{
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class ResponseTypeAttribute : Attribute
    {
        public Type ResponseType { get; }
        public ResponseTypeAttribute(Type responseType) => ResponseType = responseType;
    }
}

namespace Microsoft.Owin.Security
{
    public interface IAuthenticationManager
    {
        void SignOut(params string[] authenticationTypes);
        Task SignOutAsync(params string[] authenticationTypes);
    }

    public class AuthenticationManagerShim : IAuthenticationManager
    {
        private readonly HttpContext _context;
        public AuthenticationManagerShim(HttpContext context) => _context = context;

        public void SignOut(params string[] authenticationTypes)
        {
            _context.SignOutAsync().GetAwaiter().GetResult();
        }

        public async Task SignOutAsync(params string[] authenticationTypes)
        {
            await _context.SignOutAsync();
        }
    }

    public class AuthenticationDescription
    {
        public string AuthenticationType { get; set; } = "";
        public string Caption { get; set; } = "";
    }
}

namespace Microsoft.AspNet.Identity
{
    public static class DefaultAuthenticationTypes
    {
        public const string ApplicationCookie = "ApplicationCookie";
        public const string ExternalCookie = "ExternalCookie";
        public const string ExternalBearer = "ExternalBearer";
        public const string TwoFactorCookie = "TwoFactorCookie";
        public const string TwoFactorRememberBrowserCookie = "TwoFactorRememberBrowserCookie";
    }

    public class IdentityMessage
    {
        public string Destination { get; set; } = "";
        public string Subject { get; set; } = "";
        public string Body { get; set; } = "";
    }

    public class UserStore<TUser> where TUser : class
    {
        public UserStore(object db) { }
    }

    public class RoleStore<TRole> where TRole : class
    {
        public RoleStore(object db) { }
    }
}

namespace Microsoft.AspNet.Identity.Owin
{
    public class OwinContextShim
    {
        private readonly HttpContext _context;
        public OwinContextShim(HttpContext context) => _context = context;

        public T GetUserManager<T>() where T : class
        {
            return _context.RequestServices.GetService<T>()
                ?? (_context.RequestServices.GetService<UserManager<ApplicationUser>>() as T)!;
        }

        public T Get<T>() where T : class
        {
            return _context.RequestServices.GetService<T>()
                ?? (_context.RequestServices.GetService<SignInManager<ApplicationUser>>() as T)!;
        }

        public Microsoft.Owin.Security.IAuthenticationManager Authentication =>
            new Microsoft.Owin.Security.AuthenticationManagerShim(_context);
    }

    public static class OwinContextExtensions
    {
        public static OwinContextShim GetOwinContext(this HttpContext context)
        {
            return new OwinContextShim(context);
        }
    }
}

namespace System.Web.UI
{
    public class Control { }
}

namespace System.Web.UI.WebControls
{
    public class MailDefinition
    {
        public string BodyFileName { get; set; } = "";
        public string From { get; set; } = "";
        public string Subject { get; set; } = "";
        public string CC { get; set; } = "";
        public bool IsBodyHtml { get; set; } = true;

        public System.Net.Mail.MailMessage CreateMailMessage(string to, System.Collections.IDictionary replacements, System.Web.UI.Control owner)
        {
            var msg = new System.Net.Mail.MailMessage();
            msg.To.Add(to);
            if (!string.IsNullOrEmpty(From)) msg.From = new System.Net.Mail.MailAddress(From);
            msg.Subject = Subject;
            return msg;
        }

        public System.Net.Mail.MailMessage CreateMailMessage(string to, System.Collections.IDictionary replacements, string body, System.Web.UI.Control owner)
        {
            var msg = new System.Net.Mail.MailMessage();
            msg.To.Add(to);
            if (!string.IsNullOrEmpty(From)) msg.From = new System.Net.Mail.MailAddress(From);
            msg.Subject = Subject;
            msg.Body = body;
            msg.IsBodyHtml = IsBodyHtml;
            return msg;
        }
    }
}

namespace System.Web.Optimization
{
    public static class Styles
    {
        public static IHtmlContent Render(params string[] paths) => HtmlString.Empty;
    }

    public static class Scripts
    {
        public static IHtmlContent Render(params string[] paths) => HtmlString.Empty;
    }
}

namespace ExwhyzeeSP.Web.Models
{
    public static class Server
    {
        public static string MachineName => Environment.MachineName;
        public static string MapPath(string path) => Path.Combine(AppContext.BaseDirectory, path.TrimStart('~', '/', '\\'));
        public static string HtmlEncode(string s) => System.Net.WebUtility.HtmlEncode(s);
        public static string HtmlDecode(string s) => System.Net.WebUtility.HtmlDecode(s);
        public static string UrlEncode(string s) => System.Net.WebUtility.UrlEncode(s);
        public static string UrlDecode(string s) => System.Net.WebUtility.UrlDecode(s);
    }

    public static class HttpHelper
    {
        private static IHttpContextAccessor? _accessor;
        public static void Configure(IHttpContextAccessor accessor) => _accessor = accessor;
        
        public static HttpContext? Current => _accessor?.HttpContext;
        public static ClaimsPrincipal? CurrentUser => _accessor?.HttpContext?.User;
        public static string? CurrentUserId => _accessor?.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
    }

    public static class IdentityCompatibilityExtensions
    {
        
        public static async Task<bool> IsInRoleAsync(this UserManager<ApplicationUser> userManager, string userId, string role)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null) return false;
            return await userManager.IsInRoleAsync(user, role);
        }

        public static bool IsInRole(this UserManager<ApplicationUser> userManager, string userId, string role)
        {
            var user = userManager.FindByIdAsync(userId).GetAwaiter().GetResult();
            if (user == null) return false;
            return userManager.IsInRoleAsync(user, role).GetAwaiter().GetResult();
        }

        public static bool IsInRole(this UserManager<ApplicationUser> userManager, ApplicationUser user, string role)
        {
            return userManager.IsInRoleAsync(user, role).GetAwaiter().GetResult();
        }

        
        
        public static ApplicationUser? FindById(this UserManager<ApplicationUser> userManager, string userId)
        {
            return userManager.FindByIdAsync(userId).GetAwaiter().GetResult();
        }

        public static ApplicationUser? FindByName(this UserManager<ApplicationUser> userManager, string userName)
        {
            return userManager.FindByNameAsync(userName).GetAwaiter().GetResult();
        }

        public static IdentityRole? FindByName(this RoleManager<IdentityRole> roleManager, string roleName)
        {
            return roleManager.FindByNameAsync(roleName).GetAwaiter().GetResult();
        }

        
        public static IdentityResult RemovePassword(this UserManager<ApplicationUser> userManager, string userId)
        {
            var user = userManager.FindByIdAsync(userId).GetAwaiter().GetResult();
            if (user == null) return IdentityResult.Failed(new IdentityError { Description = "User not found" });
            return userManager.RemovePasswordAsync(user).GetAwaiter().GetResult();
        }

        public static IdentityResult AddPassword(this UserManager<ApplicationUser> userManager, string userId, string newPassword)
        {
            var user = userManager.FindByIdAsync(userId).GetAwaiter().GetResult();
            if (user == null) return IdentityResult.Failed(new IdentityError { Description = "User not found" });
            return userManager.AddPasswordAsync(user, newPassword).GetAwaiter().GetResult();
        }

        
        public static async Task<IdentityResult> ChangePasswordAsync(this UserManager<ApplicationUser> userManager, string userId, string currentPassword, string newPassword)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null) return IdentityResult.Failed(new IdentityError { Description = "User not found" });
            return await userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        }

        public static async Task<IdentityResult> ConfirmEmailAsync(this UserManager<ApplicationUser> userManager, string userId, string token)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null) return IdentityResult.Failed(new IdentityError { Description = "User not found" });
            return await userManager.ConfirmEmailAsync(user, token);
        }

        public static async Task<IdentityResult> ResetPasswordAsync(this UserManager<ApplicationUser> userManager, string userId, string token, string newPassword)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null) return IdentityResult.Failed(new IdentityError { Description = "User not found" });
            return await userManager.ResetPasswordAsync(user, token, newPassword);
        }

        public static async Task<string> GeneratePasswordResetTokenAsync(this UserManager<ApplicationUser> userManager, string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null) return "";
            return await userManager.GeneratePasswordResetTokenAsync(user);
        }

        public static async Task<string> GenerateEmailConfirmationTokenAsync(this UserManager<ApplicationUser> userManager, string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null) return "";
            return await userManager.GenerateEmailConfirmationTokenAsync(user);
        }

        public static async Task<IList<string>> GetValidTwoFactorProvidersAsync(this UserManager<ApplicationUser> userManager, string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null) return new List<string>();
            return await userManager.GetValidTwoFactorProvidersAsync(user);
        }

        public static async Task<IdentityResult> AddLoginAsync(this UserManager<ApplicationUser> userManager, string userId, UserLoginInfo login)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null) return IdentityResult.Failed(new IdentityError { Description = "User not found" });
            return await userManager.AddLoginAsync(user, login);
        }

        
        public static async Task<bool> HasPasswordAsync(this UserManager<ApplicationUser> userManager, string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            return user != null && await userManager.HasPasswordAsync(user);
        }

        public static async Task<IList<UserLoginInfo>> GetLoginsAsync(this UserManager<ApplicationUser> userManager, string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null) return new List<UserLoginInfo>();
            return await userManager.GetLoginsAsync(user);
        }

        public static async Task<bool> GetTwoFactorEnabledAsync(this UserManager<ApplicationUser> userManager, string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            return user != null && await userManager.GetTwoFactorEnabledAsync(user);
        }

        public static async Task<string> GetPhoneNumberAsync(this UserManager<ApplicationUser> userManager, string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            return user?.PhoneNumber ?? "";
        }

        public static async Task<IdentityResult> SetTwoFactorEnabledAsync(this UserManager<ApplicationUser> userManager, string userId, bool enabled)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null) return IdentityResult.Failed(new IdentityError { Description = "User not found" });
            return await userManager.SetTwoFactorEnabledAsync(user, enabled);
        }

        public static async Task<IdentityResult> SetPhoneNumberAsync(this UserManager<ApplicationUser> userManager, string userId, string? phoneNumber)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null) return IdentityResult.Failed(new IdentityError { Description = "User not found" });
            return await userManager.SetPhoneNumberAsync(user, phoneNumber);
        }

        public static async Task<IdentityResult> ChangePhoneNumberAsync(this UserManager<ApplicationUser> userManager, string userId, string phoneNumber, string token)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null) return IdentityResult.Failed(new IdentityError { Description = "User not found" });
            return await userManager.ChangePhoneNumberAsync(user, phoneNumber, token);
        }

        public static async Task<string> GenerateChangePhoneNumberTokenAsync(this UserManager<ApplicationUser> userManager, string userId, string phoneNumber)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null) return "";
            return await userManager.GenerateChangePhoneNumberTokenAsync(user, phoneNumber);
        }

        public static async Task<IdentityResult> RemoveLoginAsync(this UserManager<ApplicationUser> userManager, string userId, string loginProvider, string providerKey)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null) return IdentityResult.Failed(new IdentityError { Description = "User not found" });
            return await userManager.RemoveLoginAsync(user, loginProvider, providerKey);
        }

        public static async Task<bool> TwoFactorBrowserRememberedAsync(this Microsoft.Owin.Security.IAuthenticationManager auth, string userId) => false;
        public static async Task<Microsoft.AspNetCore.Identity.ExternalLoginInfo?> GetExternalLoginInfoAsync(this Microsoft.Owin.Security.IAuthenticationManager auth, string xsrfKey, string userId) => await auth.GetExternalLoginInfoAsync();
        public static async Task<Microsoft.AspNetCore.Identity.ExternalLoginInfo?> GetExternalLoginInfoAsync(this Microsoft.Owin.Security.IAuthenticationManager auth)
        {
            var sim = ExwhyzeeSP.Web.Models.HttpHelper.Current?.RequestServices.GetService<Microsoft.AspNetCore.Identity.SignInManager<ExwhyzeeSP.Core.Models.ApplicationUser>>();
            if (sim != null) return await sim.GetExternalLoginInfoAsync();
            return null;
        }

        
        public static async Task<IdentityResult> AddPasswordAsync(this UserManager<ApplicationUser> userManager, string userId, string password)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null) return IdentityResult.Failed(new IdentityError { Description = "User not found" });
            return await userManager.AddPasswordAsync(user, password);
        }

        public static async Task<bool> IsEmailConfirmedAsync(this UserManager<ApplicationUser> userManager, string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            return user != null && await userManager.IsEmailConfirmedAsync(user);
        }

        public static async Task<IdentityResult> AddPhoneNumberAsync(this UserManager<ApplicationUser> userManager, string userId, string phoneNumber)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null) return IdentityResult.Failed(new IdentityError { Description = "User not found" });
            return await userManager.SetPhoneNumberAsync(user, phoneNumber);
        }

        public static IdentityResult Delete(this UserManager<ApplicationUser> userManager, ApplicationUser user)
        {
            return userManager.DeleteAsync(user).GetAwaiter().GetResult();
        }

        public static async Task<IdentityResult> AddToRoleAsync(this UserManager<ApplicationUser> userManager, string userId, string role)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null) return IdentityResult.Failed(new IdentityError { Description = "User not found." });
            return await userManager.AddToRoleAsync(user, role);
        }

        public static async Task<IdentityResult> RemoveFromRoleAsync(this UserManager<ApplicationUser> userManager, string userId, string role)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null) return IdentityResult.Failed(new IdentityError { Description = "User not found." });
            return await userManager.RemoveFromRoleAsync(user, role);
        }

        public static async Task<IList<string>> GetRolesAsync(this UserManager<ApplicationUser> userManager, string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null) return new List<string>();
            return await userManager.GetRolesAsync(user);
        }

        public static async Task<IdentityResult> AddToRolesAsync(this UserManager<ApplicationUser> userManager, string userId, params string[] roles)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null) return IdentityResult.Failed(new IdentityError { Description = "User not found." });
            return await userManager.AddToRolesAsync(user, roles);
        }

        public static async Task<IdentityResult> RemoveFromRolesAsync(this UserManager<ApplicationUser> userManager, string userId, params string[] roles)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null) return IdentityResult.Failed(new IdentityError { Description = "User not found." });
            return await userManager.RemoveFromRolesAsync(user, roles);
        }

        public static async Task<Microsoft.AspNet.Identity.Owin.SignInStatus> PasswordSignInAsync(
            this SignInManager<ApplicationUser> signInManager,
            string userName,
            string password,
            bool isPersistent,
            bool shouldLockout)
        {
            var res = await signInManager.PasswordSignInAsync(userName, password, isPersistent, lockoutOnFailure: shouldLockout);
            if (res.Succeeded) return Microsoft.AspNet.Identity.Owin.SignInStatus.Success;
            if (res.IsLockedOut) return Microsoft.AspNet.Identity.Owin.SignInStatus.LockedOut;
            if (res.RequiresTwoFactor) return Microsoft.AspNet.Identity.Owin.SignInStatus.RequiresVerification;
            return Microsoft.AspNet.Identity.Owin.SignInStatus.Failure;
        }

        public static async Task SignInAsync(
            this SignInManager<ApplicationUser> signInManager,
            ApplicationUser user,
            bool isPersistent,
            bool rememberBrowser)
        {
            await signInManager.SignInAsync(user, isPersistent);
        }

        public static async Task<Microsoft.AspNet.Identity.Owin.SignInStatus> TwoFactorSignInAsync(
            this SignInManager<ApplicationUser> signInManager,
            string provider,
            string code,
            bool isPersistent,
            bool rememberBrowser)
        {
            var res = await signInManager.TwoFactorSignInAsync(provider, code, isPersistent, rememberClient: rememberBrowser);
            if (res.Succeeded) return Microsoft.AspNet.Identity.Owin.SignInStatus.Success;
            if (res.IsLockedOut) return Microsoft.AspNet.Identity.Owin.SignInStatus.LockedOut;
            if (res.RequiresTwoFactor) return Microsoft.AspNet.Identity.Owin.SignInStatus.RequiresVerification;
            return Microsoft.AspNet.Identity.Owin.SignInStatus.Failure;
        }
    }

    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public dynamic? SmsService { get; set; }

        public ApplicationUserManager()
            : base(
                HttpHelper.Current?.RequestServices.GetRequiredService<IUserStore<ApplicationUser>>()!,
                HttpHelper.Current?.RequestServices.GetRequiredService<IOptions<IdentityOptions>>()!,
                HttpHelper.Current?.RequestServices.GetRequiredService<IPasswordHasher<ApplicationUser>>()!,
                HttpHelper.Current?.RequestServices.GetRequiredService<IEnumerable<IUserValidator<ApplicationUser>>>()!,
                HttpHelper.Current?.RequestServices.GetRequiredService<IEnumerable<IPasswordValidator<ApplicationUser>>>()!,
                HttpHelper.Current?.RequestServices.GetRequiredService<ILookupNormalizer>()!,
                HttpHelper.Current?.RequestServices.GetRequiredService<IdentityErrorDescriber>()!,
                HttpHelper.Current?.RequestServices.GetRequiredService<IServiceProvider>()!,
                HttpHelper.Current?.RequestServices.GetRequiredService<ILogger<UserManager<ApplicationUser>>>()!)
        {
        }

        public ApplicationUserManager(
            IUserStore<ApplicationUser> store,
            IOptions<IdentityOptions> optionsAccessor,
            IPasswordHasher<ApplicationUser> passwordHasher,
            IEnumerable<IUserValidator<ApplicationUser>> userValidators,
            IEnumerable<IPasswordValidator<ApplicationUser>> passwordValidators,
            ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors,
            IServiceProvider services,
            ILogger<UserManager<ApplicationUser>> logger)
            : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
        }
    }

    public class ApplicationSignInManager : SignInManager<ApplicationUser>
    {
        public ApplicationSignInManager()
            : base(
                HttpHelper.Current?.RequestServices.GetRequiredService<UserManager<ApplicationUser>>()!,
                HttpHelper.Current?.RequestServices.GetRequiredService<IHttpContextAccessor>()!,
                HttpHelper.Current?.RequestServices.GetRequiredService<IUserClaimsPrincipalFactory<ApplicationUser>>()!,
                HttpHelper.Current?.RequestServices.GetRequiredService<IOptions<IdentityOptions>>()!,
                HttpHelper.Current?.RequestServices.GetRequiredService<ILogger<SignInManager<ApplicationUser>>>()!,
                HttpHelper.Current?.RequestServices.GetRequiredService<IAuthenticationSchemeProvider>()!,
                HttpHelper.Current?.RequestServices.GetRequiredService<IUserConfirmation<ApplicationUser>>()!)
        {
        }

        public ApplicationSignInManager(
            UserManager<ApplicationUser> userManager,
            IHttpContextAccessor contextAccessor,
            IUserClaimsPrincipalFactory<ApplicationUser> claimsFactory,
            IOptions<IdentityOptions> optionsAccessor,
            ILogger<SignInManager<ApplicationUser>> logger,
            IAuthenticationSchemeProvider schemes,
            IUserConfirmation<ApplicationUser> confirmation)
            : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes, confirmation)
        {
        }
    }

    public class ApplicationRoleManager : RoleManager<IdentityRole>
    {
        public ApplicationRoleManager()
            : base(
                HttpHelper.Current?.RequestServices.GetRequiredService<IRoleStore<IdentityRole>>()!,
                HttpHelper.Current?.RequestServices.GetRequiredService<IEnumerable<IRoleValidator<IdentityRole>>>()!,
                HttpHelper.Current?.RequestServices.GetRequiredService<ILookupNormalizer>()!,
                HttpHelper.Current?.RequestServices.GetRequiredService<IdentityErrorDescriber>()!,
                HttpHelper.Current?.RequestServices.GetRequiredService<ILogger<RoleManager<IdentityRole>>>()!)
        {
        }

        public ApplicationRoleManager(
            IRoleStore<IdentityRole> store,
            IEnumerable<IRoleValidator<IdentityRole>> roleValidators,
            ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors,
            ILogger<RoleManager<IdentityRole>> logger)
            : base(store, roleValidators, keyNormalizer, errors, logger)
        {
        }
    }
}


namespace Microsoft.AspNetCore.Identity
{
    public static class SignInManagerCompatibilityExtensions
    {
        public static async Task<bool> HasBeenVerifiedAsync(this SignInManager<ApplicationUser> sim) => true;
        public static async Task<string?> GetVerifiedUserIdAsync(this SignInManager<ApplicationUser> sim)
        {
            var user = await sim.GetTwoFactorAuthenticationUserAsync();
            return user?.Id;
        }
        public static async Task<bool> SendTwoFactorCodeAsync(this SignInManager<ApplicationUser> sim, string provider) => true;
        public static async Task<Microsoft.AspNetCore.Identity.SignInResult> ExternalSignInAsync(this SignInManager<ApplicationUser> sim, Microsoft.AspNetCore.Identity.ExternalLoginInfo info, bool isPersistent)
        {
            return await sim.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent);
        }
        public static void Dispose(this SignInManager<ApplicationUser> sim) { }
        public static void Dispose(this UserManager<ApplicationUser> um) { }
    }
}

namespace Microsoft.Owin.Security
{
    public static class AuthManagerExtensions
    {
        public static IEnumerable<AuthenticationDescription> GetExternalAuthenticationTypes(this IAuthenticationManager auth)
        {
            return Enumerable.Empty<AuthenticationDescription>();
        }
    }
}

namespace Microsoft.Extensions.Primitives
{
    public static class StringValuesExtensions
    {
        public static bool IsMobileDevice(this StringValues val) => false;
        public static bool IsMobileDevice(this string val) => false;
    }
}

namespace System
{
    public static class ExceptionCompatExtensions
    {
        public static IEnumerable<dynamic> EntityValidationErrors(this Exception ex) => Enumerable.Empty<dynamic>();
    }
}


namespace Microsoft.AspNetCore.Http
{
    public static class PathStringExtensions
    {
        public static string PathAndQuery(this PathString path) => path.Value ?? "";
    }
}


namespace Excel
{
    public class Range
    {
        public dynamic Cells => this;
        public dynamic Rows => this;
        public int Count => 0;
        public string Text => "";
        public dynamic this[int row, int col] => this;
    }
    public class Worksheet
    {
        public Range UsedRange => new Range();
    }
    public class Workbook
    {
        public Worksheet ActiveSheet => new Worksheet();
    }
    public class Workbooks
    {
        public Workbook Open(string path) => new Workbook();
    }
    public class Application
    {
        public Workbooks Workbooks => new Workbooks();
    }
}


namespace Microsoft.AspNet.Identity.Owin
{
    public enum SignInStatus
    {
        Success = 0,
        LockedOut = 1,
        RequiresVerification = 2,
        Failure = 3
    }
}

namespace System.Net.Http
{
    public static class HttpContentExtensions
    {
        public static async Task<T?> ReadAsAsync<T>(this HttpContent content)
        {
            var json = await content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(json)) return default;
            return System.Text.Json.JsonSerializer.Deserialize<T>(json, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
    }
}




namespace Microsoft.AspNetCore.Http
{
    public static class HttpRequestCreateResponseExtensions
    {
        public static IActionResult CreateResponse<T>(this HttpRequest req, HttpStatusCode statusCode, T value)
        {
            return new ObjectResult(value) { StatusCode = (int)statusCode };
        }
        public static IActionResult CreateResponse(this HttpRequest req, HttpStatusCode statusCode)
        {
            return new StatusCodeResult((int)statusCode);
        }
    }
}
