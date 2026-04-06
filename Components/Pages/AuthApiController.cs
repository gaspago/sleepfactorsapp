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
    public async Task<IActionResult> Register([FromForm] string email, [FromForm] string password, [FromForm] string confirmPassword)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(confirmPassword))
        {
            return BadRequest("All fields are required.");
        }

        if (password != confirmPassword)
        {
            return BadRequest("Passwords do not match.");
        }

        var user = new IdentityUser { UserName = email, Email = email };
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
    public async Task<IActionResult> Login([FromForm] string email, [FromForm] string password)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            return BadRequest("Email and password are required.");
        }

        var result = await signInManager.PasswordSignInAsync(email, password, isPersistent: false, lockoutOnFailure: false);

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
