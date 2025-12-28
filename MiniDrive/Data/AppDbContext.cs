using Microsoft.EntityFrameworkCore;
using MiniDrive.Models;
using System.Collections.Generic;

namespace MiniDriveVideo.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }

    public DbSet<ArquivoModel> Arquivos { get; set; }
}
