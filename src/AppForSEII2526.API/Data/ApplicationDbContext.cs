using AppForSEII2526.API.Models;
using Humanizer.Localisation;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AppForSEII2526.API.Data;

public class ApplicationDbContext : DbContext
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<AlquilarItem>().HasKey(ai => new { ai.HerramientaId, ai.AlquilerId });
        builder.Entity<CompraItem>().HasKey(ci => new { ci.IdHerramienta, ci.IdCompra });
        builder.Entity<ReparacionItem>().HasKey(ri => new { ri.idHerramienta, ri.idReparacion });
        builder.Entity<OfertaItem>().HasKey(oi => new { oi.idHerramienta, oi.idOferta });

    }

public DbSet<Fabricante> Fabricante { get; set; }
public DbSet<Herramienta> Herramienta { get; set; }
public DbSet<Reparacion> Reparacion { get; set; }
public DbSet<Compra> Compra { get; set; }
public DbSet<Alquiler> Alquiler { get; set; }
public DbSet<Oferta> Oferta { get; set; }
public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
: base(options)
{
}
}
 

