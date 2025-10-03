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

    // Landing page: aggregates public counts to display on home
    public async Task<IActionResult> Index()
    {
        var stats = new
        {
            TotalEvents = await _context.Events.CountAsync(e => e.IsPublic && e.IsActive),
            TotalNGOs = await _context.NGOProfiles.CountAsync(n => n.IsActive),
            TotalVolunteers = await _context.Users.CountAsync(u => u.IsActive)
        };

        ViewBag.Stats = stats;

        return View();
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
