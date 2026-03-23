using Microsoft.AspNetCore.Mvc;

namespace Demo.Mvc.Controllers;

/// <summary>
/// Handles the initial Expense Tracker pages.
/// Data is currently in-memory placeholder content for showcase purposes.
/// </summary>
public class ExpensesController : Controller
{
    public IActionResult Index()
    {
        var sampleExpenses = new[]
        {
            new ExpenseRowView("Groceries", 48.90m, DateOnly.FromDateTime(DateTime.Today.AddDays(-2))),
            new ExpenseRowView("Fuel", 35.00m, DateOnly.FromDateTime(DateTime.Today.AddDays(-1))),
            new ExpenseRowView("Coffee", 4.20m, DateOnly.FromDateTime(DateTime.Today))
        };

        return View(sampleExpenses);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(string description, decimal amount, DateOnly date)
    {
        if (string.IsNullOrWhiteSpace(description) || amount <= 0)
        {
            ModelState.AddModelError(string.Empty, "Please provide a valid description and an amount greater than zero.");
            return View();
        }

        TempData["SuccessMessage"] = $"Expense '{description}' ({amount:C}) captured for {date:yyyy-MM-dd}.";
        return RedirectToAction(nameof(Index));
    }

    public sealed record ExpenseRowView(string Description, decimal Amount, DateOnly Date);
}
