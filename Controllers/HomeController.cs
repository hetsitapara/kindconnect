using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VolunteerManagementSystem.Data;
using VolunteerManagementSystem.Models;

namespace VolunteerManagementSystem.Controllers;

using Microsoft.AspNetCore.Authorization;

/// <summary>
/// Public landing pages: shows basic stats, About/Contact/Privacy and error page.
/// </summary>
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    // Landing page: redirect to appropriate dashboard based on user role
    public IActionResult Index()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            // Redirect to Dashboard which will handle role-based routing
            return RedirectToAction("Index", "Dashboard");
        }

        // If not authenticated, redirect to login
        return RedirectToAction("Login", "Account");
    }

    // Static page
    public IActionResult About()
    {
        return View();
    }

    // Static page
    public IActionResult Contact()
    {
        return View();
    }

    // Static page
    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    // Error page used by global exception handler in production
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
