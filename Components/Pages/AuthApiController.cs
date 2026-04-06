using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SleepFactorsApp.Components.Pages;

[ApiController]
[Route("/api/auth")]
public sealed class AuthApiController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager) : ControllerBase
{
    [Authorize(Roles = "Admin")]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromForm] string userNameOrEmail, [FromForm] string password, [FromForm] string confirmPassword)
    {
        if (string.IsNullOrWhiteSpace(userNameOrEmail) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(confirmPassword))
        {
            return BadRequest("All fields are required.");
        }

        if (password != confirmPassword)
        {
            return BadRequest("Passwords do not match.");
        }

        var normalized = userNameOrEmail.Trim();
        var email = normalized.Contains('@') ? normalized : null;
        var user = new IdentityUser { UserName = normalized, Email = email };
        var result = await userManager.CreateAsync(user, password);

        if (result.Succeeded)
        {
            await signInManager.SignInAsync(user, isPersistent: false);
            return Redirect("/");
        }

        var errors = string.Join(", ", result.Errors.Select(e => e.Description));
        return BadRequest(errors);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromForm] string userNameOrEmail, [FromForm] string password)
    {
        if (string.IsNullOrWhiteSpace(userNameOrEmail) || string.IsNullOrWhiteSpace(password))
        {
            return BadRequest("Username/email and password are required.");
        }

        var loginName = userNameOrEmail.Trim();
        if (loginName.Contains('@'))
        {
            var userByEmail = await userManager.FindByEmailAsync(loginName);
            if (userByEmail is not null && !string.IsNullOrWhiteSpace(userByEmail.UserName))
            {
                loginName = userByEmail.UserName;
            }
        }

        var result = await signInManager.PasswordSignInAsync(loginName, password, isPersistent: false, lockoutOnFailure: false);

        if (result.Succeeded)
        {
            return Redirect("/");
        }

        return BadRequest("Invalid email or password.");
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await signInManager.SignOutAsync();
        return Redirect("/login");
    }
}
