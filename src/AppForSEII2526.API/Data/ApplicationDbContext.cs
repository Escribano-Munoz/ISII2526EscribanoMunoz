using AppForSEII2526.API.Models;
using Humanizer.Localisation;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AppForSEII2526.API.Data;

public class ApplicationDbContext : DbContext
{
    

    public DbSet<Fabricante> Fabricante { get; set; }
public DbSet<Herramienta> Herramienta { get; set; }
public DbSet<Reparacion> Reparacion { get; set; }
public DbSet<Compra> Compra { get; set; }
public DbSet<Alquiler> Alquiler { get; set; }
public DbSet<Oferta> Oferta { get; set; }
public DbSet<ApplicationUser> ApplicationUsers { get; set; }
public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
: base(options)
{
}
}
 

