using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PressureMonitor.Models;

namespace PressureMonitor.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<HomeController> _logger;

    public HomeController(ApplicationDbContext context, ILogger<HomeController> logger)
    {
        this._context = context;
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult Create()
    {
        return View();
    }

    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> HandleLogin(User? user)
    {
        if (user != null && !string.IsNullOrEmpty(user.Username) && !string.IsNullOrEmpty(user.Password))
        {
            try
            {
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username == user.Username && u.Password == user.Password);
                
                if (existingUser != null)
                {
                    // Login successful - store user session
                    HttpContext.Session.SetString("Username", existingUser.Username);
                    return RedirectToAction(nameof(Index));
                }
                
                // Login failed
                TempData["Error"] = "Invalid username or password";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user login");
                TempData["Error"] = "An error occurred during login. Please try again.";
            }
        }
        else
        {
            TempData["Error"] = "Please provide both username and password.";
        }
        
        return RedirectToAction(nameof(Login));
    }

    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> HandleRegister(User? user)
    {
        if (user != null && !string.IsNullOrEmpty(user.Username) && !string.IsNullOrEmpty(user.Password))
        {
            // Check if username already exists
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == user.Username);
            if (existingUser != null)
            {
                TempData["Error"] = "Username already exists";
                return RedirectToAction(nameof(Register));
            }

            try
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                
                // Auto-login after registration
                HttpContext.Session.SetString("Username", user.Username);
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error during user registration");
                TempData["Error"] = "An error occurred while creating your account. Please try again.";
                return RedirectToAction(nameof(Register));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user registration");
                TempData["Error"] = "An unexpected error occurred. Please try again.";
                return RedirectToAction(nameof(Register));
            }
        }
        
        TempData["Error"] = "Please provide valid username and password.";
        return RedirectToAction(nameof(Register));
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction(nameof(Login));
    }

    public async Task<IActionResult> Edit(int id)
    {

        var expense = await _context.Expenses.FindAsync(id);
        if (expense == null)
        {
            return RedirectToAction(nameof(ViewExpenses));
        }

        return View(expense);
    }
    
    public async Task<IActionResult> ViewExpenses()
    {
        var expenses = await _context.Expenses.ToListAsync();
        
        ViewBag.TotalExpense = expenses.Sum(e => e.Amount);
        return View(expenses);
    }

    public async Task<IActionResult> HandleEdit(Expense? expense)
    {
        if (expense != null)
        {
            _context.Update(expense);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(ViewExpenses));
    }
    

    public async Task<IActionResult> HandleCreate(Expense? expense)
    {
        if (expense != null)
        {
            _context.Expenses.Add(expense);
            _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id != null)
        {
            var existingExpense = await _context.Expenses.FindAsync(id);
            if (existingExpense != null)
            {
                _context.Expenses.Remove(existingExpense);
                await _context.SaveChangesAsync();
            }
        }

        return RedirectToAction("ViewExpenses");
            
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}