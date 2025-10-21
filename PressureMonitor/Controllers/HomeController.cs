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