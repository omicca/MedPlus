using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MedPlus.Models;
using MedPlus.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace MedPlus.Controllers;

public class AdminController(
    ILogger<AdminController> logger,
    UserManager<User> userManager,
    ApplicationDbContext context,
    IServiceScopeFactory factory)
    : Controller
{
    private readonly ILogger<AdminController> _logger = logger;
    private readonly ApplicationDbContext _context = context;

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public IActionResult UserManager()
    {
        return View();
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DisplayUsers()
    {
        var users = userManager.Users.ToList();
        var userRoles = new Dictionary<User, IList<string>>();

        foreach (var user in users)
        {
            var roles = await userManager.GetRolesAsync(user);
            userRoles[user] = roles;
        }

        return View(userRoles);
    }


    public async Task<IActionResult> UserManager(string email, string password, string role)
    {
        try
        {
            using var scope = factory.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            string aId = "404746b2-919f-4c57-84ea-9eaeab6d7a27";

            if (await userManager.FindByEmailAsync(email) == null)
            {
                User newAcc;
                if (role == "Doctor")
                {
                    newAcc = new Doctor
                    {
                        UserName = email,
                        Email = email,
                        AdminId = aId,
                    };
                }
                else if (role == "Patient")
                {
                    newAcc = new Patient
                    {
                        UserName = email,
                        Email = email,
                        AdminId = aId,
                    };
                }
                else
                {
                    return BadRequest("Role does not exist.");
                }

                var createUserResult = await userManager.CreateAsync(newAcc, password);

                if (!createUserResult.Succeeded)
                {
                    return BadRequest(createUserResult.Errors);
                }

                var createdUser = await userManager.FindByEmailAsync(email);
                if (createdUser == null)
                {
                    return StatusCode(500, "User creation failed.");
                }

                if (!await roleManager.RoleExistsAsync(role))
                {
                    var roleResult = await roleManager.CreateAsync(new IdentityRole(role));
                    if (!roleResult.Succeeded)
                    {
                        return StatusCode(500, "Role creation failed.");
                    }
                }

                var addToRoleResult = await userManager.AddToRoleAsync(createdUser, role);
                if (!addToRoleResult.Succeeded)
                {
                    return BadRequest(addToRoleResult.Errors);
                }
            }
            else
            {
                return BadRequest("User already exists.");
            }
        }
        catch (DbUpdateException dbEx)
        {
            Console.WriteLine(dbEx.InnerException?.Message);
            return StatusCode(500, "Internal server error: " + dbEx.InnerException?.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(500, "Internal server error: " + ex.Message);
        }

        return View();
    }
}