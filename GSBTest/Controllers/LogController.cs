using GSBTest.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

public class LogController : Controller
{
	private readonly GsbtestContext _context;

	public LogController(GsbtestContext context)
	{
		_context = context;
	}

	public IActionResult LogListele()
	{
		var logs = _context.TblLogs.ToList();

		if (logs == null || !logs.Any())
		{
			// Eğer log yoksa bunu test etmek için bir log veya hata mesajı 
			Console.WriteLine("Veritabanında log kaydı bulunamadı.");
		}

		return View(logs);
	}
}
