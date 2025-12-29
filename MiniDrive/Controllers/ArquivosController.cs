using Microsoft.AspNetCore.Mvc;
using MiniDriveVideo.Data;

namespace MiniDrive.Controllers;

public class ArquivosController : Controller
{
    private readonly AppDbContext _context;

    public ArquivosController(AppDbContext context)
    {
        _context = context;
    }

    public IActionResult Index(string tipo)
    {
        var arquivos = _context.Arquivos.AsQueryable();
        if(tipo != "")
        {
            arquivos = arquivos.Where(a => a.Extensao.Contains(tipo));
        }

        return View(arquivos.OrderByDescending(a => a.DataUpload).ToList());
    }
}
