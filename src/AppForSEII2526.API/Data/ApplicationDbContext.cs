using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using AppForSEII2526.API.Models;

namespace AppForSEII2526.API.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options) 
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<AlquilarItem>().HasKey(ai => new { ai.HerramientaId, ai.AlquilerId });
        builder.Entity<CompraItem>().HasKey(ci => new { ci.IdHerramienta, ci.IdCompra });
        builder.Entity<ReparacionItem>().HasKey(ri => new { ri.idHerramienta, ri.idReparacion });
        builder.Entity<OfertaItem>().HasKey(oi => new { oi.idHerramienta, oi.idOferta });

    }
    public DbSet<Fabricante> Fabricantes { get; set; }
    public DbSet<Herramienta> Herramientas { get; set; }
    public DbSet<Reparacion> Reparaciones { get; set; }
    public DbSet<Compra> Compras { get; set; }
    public DbSet<Alquiler> Alquileres { get; set; }
    public DbSet<Oferta> Ofertas { get; set; }
    public DbSet<ApplicationUser> ApplicationUsers { get; set; }

}
