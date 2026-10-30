using FirstTask_AutoParts_Search.Data;
using Microsoft.AspNetCore.Mvc;
namespace FirstTask_AutoParts_Search.Controllers;
public class PartsController : Controller
{
    private readonly DbHelper _db;
    public PartsController(DbHelper db) => _db = db;
    [HttpGet]
    public IActionResult Index(string? query)
    {
        ViewBag.Query = query ?? "";
        return View(_db.SearchParts(query));
    }
    [HttpGet]
    public IActionResult Search(string? query) =>
        RedirectToAction(nameof(Index), new { query });
}