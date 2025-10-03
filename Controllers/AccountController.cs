using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VolunteerManagementSystem.Data;
using VolunteerManagementSystem.Models;
using VolunteerManagementSystem.Models.ViewModels;

namespace VolunteerManagementSystem.Controllers
{
    using Microsoft.AspNetCore.Authorization;

    /// <summary>
    /// Handles user registration, login, logout, and profile management.
    /// Roles: Volunteer and NGO can self-register. Superuser is created via seeding.
    /// </summary>
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _context = context;
        }

        // Render registration page
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        // Handle registration form POST
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Validate role selection
                if (string.IsNullOrEmpty(model.SelectedRole) || 
                    !new[] { "Volunteer", "NGO" }.Contains(model.SelectedRole))
                {
                    ModelState.AddModelError("SelectedRole", "Please select a valid role.");
                    return View(model);
                }

                // Check if email already exists
                var existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Email", "An account with this email already exists.");
                    return View(model);
                }

                // Create user
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Phone = model.Phone,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // Add user to selected role
                    await _userManager.AddToRoleAsync(user, model.SelectedRole);

                    // If NGO role, create NGO profile (now active immediately; no approval flow)
                    if (model.SelectedRole == "NGO")
                    {
                        if (string.IsNullOrEmpty(model.OrganizationName))
                        {
                            ModelState.AddModelError("OrganizationName", "Organization name is required for NGO registration.");
                            await _userManager.DeleteAsync(user);
                            return View(model);
                        }

                        var ngoProfile = new NGOProfile
                        {
                            UserId = user.Id,
                            Name = model.OrganizationName,
                            Mission = model.OrganizationMission ?? "",
                            ContactEmail = model.OrganizationContactEmail ?? model.Email,
                            ContactPhone = model.OrganizationContactPhone,
                            Address = model.OrganizationAddress,
                            Description = model.OrganizationDescription,
                            CreatedAt = DateTime.UtcNow,
                            IsActive = true
                        };

                        _context.NGOProfiles.Add(ngoProfile);
                        await _context.SaveChangesAsync();
                        
                        TempData["Message"] = "Your NGO profile has been created successfully.";
                    }

                    // Sign in the user
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    TempData["Message"] = $"Registration successful! Welcome, {user.FirstName}!";
                    
                    // Redirect based on role
                    if (model.SelectedRole == "NGO")
                    {
                        return RedirectToAction("Index", "Dashboard");
                    }
                    else
                    {
                        return RedirectToAction("Index", "Events");
                    }
                }

                // Add errors to model state
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        // Render login page
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        // Handle login form POST
        public async Task<IActionResult> Login(string email, string password, bool rememberMe = false)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError(string.Empty, "Email and password are required.");
                return View();
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null || !user.IsActive)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View();
            }

            var result = await _signInManager.PasswordSignInAsync(user.UserName, password, rememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                // Update last login time
                user.LastLoginAt = DateTime.UtcNow;
                await _userManager.UpdateAsync(user);

                TempData["Message"] = $"Welcome back, {user.FirstName}!";
                
                // Redirect based on role
                if (await _userManager.IsInRoleAsync(user, "NGO"))
                {
                    return RedirectToAction("Index", "Dashboard");
                }
                else
                {
                    return RedirectToAction("Index", "Events");
                }
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View();
        }

        // Log out current user
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            TempData["Message"] = "You have been logged out successfully.";
            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        // GET: Account/Manage - Show profile summary for current user
        [Authorize]
        public async Task<IActionResult> Manage()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var model = new
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Phone = user.Phone,
                CreatedAt = user.CreatedAt,
                LastLoginAt = user.LastLoginAt
            };

            return View(model);
        }

        // GET: Account/EditProfile - Render edit form
        [Authorize]
        public async Task<IActionResult> EditProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var model = new
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Phone = user.Phone
            };

            return View(model);
        }

        // POST: Account/EditProfile - Persist changes to current user's profile
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> EditProfile(string firstName, string lastName, string phone)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            try
            {
                // Update user properties
                user.FirstName = firstName;
                user.LastName = lastName;
                user.Phone = phone;

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    TempData["Success"] = "Profile updated successfully!";
                    return RedirectToAction("Manage");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            catch (Exception)
            {
                TempData["Error"] = "An error occurred while updating your profile. Please try again.";
            }

            // If we get here, something went wrong
            var model = new
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Phone = user.Phone
            };

            return View(model);
        }
    }
}
