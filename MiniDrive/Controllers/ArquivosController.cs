using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniDrive.Data;
using MiniDrive.Models;
using System.Data;

namespace MiniDrive.Controllers;

public class ArquivosController : Controller
{
    private readonly AppDbContext _context;

    public ArquivosController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(string tipo)
    {
        var arquivos = _context.Arquivos.AsQueryable();
        if (tipo is not "" and not null)
        {
            arquivos = arquivos.Where(a => a.Extensao.Contains(tipo));
        }

        ViewBag.Filtro = tipo;

        return View(await arquivos.OrderByDescending(a => a.DataUpload).ToListAsync());
    }

    [HttpPost]
    public async Task<IActionResult> Upload(IFormFile arquivo)
    {
        if (arquivo is not null && arquivo.Length > 0)
        {
            using var ms = new MemoryStream();
            await arquivo.CopyToAsync(ms);
            var arquivoModel = new ArquivoModel
            {
                NomeArquivo = Path.GetFileNameWithoutExtension(arquivo.FileName),
                Extensao = Path.GetExtension(arquivo.FileName).TrimStart('.'),
                TipoMime = arquivo.ContentType,
                Tamanho = arquivo.Length,
                DataUpload = DateTime.Now,
                ArquivoBytes = ms.ToArray()
            };

            await _context.Arquivos.AddAsync(arquivoModel);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction("Index");
    }

    public async Task<FileResult?> Download(int id)
    {
        var arquivo = await _context.Arquivos.FindAsync(id);
        if (arquivo is null) return null;

        return File(arquivo.ArquivoBytes, arquivo.TipoMime, $"{arquivo.NomeArquivo}.{arquivo.Extensao}");
    }

    public async Task<JsonResult> Delete(int id)
    {
        var arquivo = await _context.Arquivos.FindAsync(id);
        if (arquivo is null) return Json(new { success = false });

        _context.Arquivos.Remove(arquivo);
        await _context.SaveChangesAsync();

        return Json(new { success = true });

    }
}
