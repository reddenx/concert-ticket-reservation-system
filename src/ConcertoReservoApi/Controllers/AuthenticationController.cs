using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using static ConcertoReservoApi.Controllers.AuthenticatedUser;

namespace ConcertoReservoApi.Controllers
{
    //this is not generally how I'd recommend doing auth for an api, I just thought it might be neat to be able to log in for venue and event managers
    [Route("auth")]
    public class AuthenticationController : Controller
    {
        public class CredentialsDto
        {
            public string Email { get; set; }
            public string Password { get; set; } //should be secure string
        }

        public class UserView
        {
            public string Email { get; set; }
            public string[] Permissions { get; set; }
        }

        [HttpGet()]
        [ProducesResponseType<UserView>(200)]
        [ProducesResponseType(404)]
        public IActionResult GetLoggedInContext()
        {
            var user = this.GetUser();
            if (user == null)
                return NotFound();

            return Json(new UserView
            {
                Email = user.Email,
                Permissions = user.Permissions.Select(p => p.ToString()).ToArray(),
            });
        }

        [HttpPost("login")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Login([FromBody] CredentialsDto dto)
        {
            var permissions = new List<string>();
            if (dto.Email.Contains("event"))
            {
                permissions.Add("Events");
            }
            if (dto.Email.Contains("venue"))
            {
                permissions.Add("Venues");
            }
            if (permissions.Count == 0)
                return StatusCode(400);

            var identity = new ClaimsIdentity([new Claim("email", dto.Email), new Claim("permissions", string.Join(",", permissions))], CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity),
                new AuthenticationProperties());
            return StatusCode(204);
        }

        [HttpPost("logout")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return StatusCode(204);
        }
    }

    public static class ControllerExtensions
    {
        public static AuthenticatedUser GetUser(this Controller controller)
        {
            var email = controller.User?.Claims?.FirstOrDefault(c => c.Type == "email")?.Value;
            var permissionsClaims = controller.User?.Claims?.FirstOrDefault(c => c.Type == "permissions")?.Value;
            if (email != null)
            {
                var permissions = new UserPermissions[] { };
                if (!string.IsNullOrWhiteSpace(permissionsClaims))
                {
                    permissions = permissionsClaims.Split(',').Select(c => (UserPermissions)Enum.Parse(typeof(UserPermissions), c)).ToArray();
                }
                return new AuthenticatedUser(email, permissions);
            }
            return null;
        }
    }

    public class AuthenticatedUser
    {
        public enum UserPermissions { Venues, Events }

        public string Email { get; }

        public UserPermissions[] Permissions { get; } //I'm more of a fan of hierarchical roles and permissions structures in the form of the identity framework or something similar, but this is just a simple example

        public AuthenticatedUser(string email, UserPermissions[] permissions)
        {
            Email = email;
            Permissions = permissions;
        }

    }
}
