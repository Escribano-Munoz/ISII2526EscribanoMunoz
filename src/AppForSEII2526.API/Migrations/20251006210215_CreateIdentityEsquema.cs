using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppForSEII2526.API.Migrations
{
    /// <inheritdoc />
    public partial class CreateIdentityEsquema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Alquiler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PrecioTotal = table.Column<double>(type: "float", nullable: false),
                    FechaAlquiler = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DireccionEnvio = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MetodoPago = table.Column<int>(type: "int", nullable: false),
                    NombreCliente = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ApellidoCliente = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Correo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NumeroTelefono = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Periodo = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alquiler", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Compra",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    precioTotal = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    NombreCliente = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ApellidoCliente = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaCompra = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DireccionEnvio = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Telefono = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CorreoElectronico = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MetodoPago = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Compra", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Fabricante",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fabricante", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Oferta",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    fechaFinal = table.Column<DateTime>(type: "datetime2", nullable: false),
                    fechaInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    fechaOferta = table.Column<DateTime>(type: "datetime2", nullable: false),
                    metodoPago = table.Column<int>(type: "int", nullable: false),
                    paraSocio = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Oferta", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Reparacion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PrecioTotal = table.Column<float>(type: "real", nullable: false),
                    FechaRecogida = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaEntrega = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NombreCliente = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ApellidoCliente = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NumTelefono = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MetodoPago = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reparacion", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Herramienta",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Fabricanteid = table.Column<int>(type: "int", nullable: false),
                    Material = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TiempoReparacion = table.Column<int>(type: "int", nullable: false),
                    Precio = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Herramienta", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Herramienta_Fabricante_Fabricanteid",
                        column: x => x.Fabricanteid,
                        principalTable: "Fabricante",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AlquilarItem",
                columns: table => new
                {
                    HerramientaId = table.Column<int>(type: "int", nullable: false),
                    AlquilerId = table.Column<int>(type: "int", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false),
                    Precio = table.Column<double>(type: "float", nullable: false),
                    Cantidad = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlquilarItem", x => new { x.HerramientaId, x.AlquilerId });
                    table.ForeignKey(
                        name: "FK_AlquilarItem_Alquiler_AlquilerId",
                        column: x => x.AlquilerId,
                        principalTable: "Alquiler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AlquilarItem_Herramienta_HerramientaId",
                        column: x => x.HerramientaId,
                        principalTable: "Herramienta",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CompraItem",
                columns: table => new
                {
                    IdCompra = table.Column<int>(type: "int", nullable: false),
                    IdHerramienta = table.Column<int>(type: "int", nullable: false),
                    HerramientaId = table.Column<int>(type: "int", nullable: false),
                    CompraId = table.Column<int>(type: "int", nullable: false),
                    Cantidad = table.Column<int>(type: "int", nullable: false),
                    Precio = table.Column<float>(type: "real", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompraItem", x => new { x.IdHerramienta, x.IdCompra });
                    table.ForeignKey(
                        name: "FK_CompraItem_Compra_CompraId",
                        column: x => x.CompraId,
                        principalTable: "Compra",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompraItem_Herramienta_HerramientaId",
                        column: x => x.HerramientaId,
                        principalTable: "Herramienta",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OfertaItem",
                columns: table => new
                {
                    idHerramienta = table.Column<int>(type: "int", nullable: false),
                    idOferta = table.Column<int>(type: "int", nullable: false),
                    ofertaId = table.Column<int>(type: "int", nullable: false),
                    herramientaId = table.Column<int>(type: "int", nullable: false),
                    porcentaje = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    precioFinal = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OfertaItem", x => new { x.idHerramienta, x.idOferta });
                    table.ForeignKey(
                        name: "FK_OfertaItem_Herramienta_herramientaId",
                        column: x => x.herramientaId,
                        principalTable: "Herramienta",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OfertaItem_Oferta_ofertaId",
                        column: x => x.ofertaId,
                        principalTable: "Oferta",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReparacionItem",
                columns: table => new
                {
                    idReparacion = table.Column<int>(type: "int", nullable: false),
                    idHerramienta = table.Column<int>(type: "int", nullable: false),
                    descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    precio = table.Column<float>(type: "real", nullable: false),
                    cantidad = table.Column<int>(type: "int", nullable: false),
                    HerramientaId = table.Column<int>(type: "int", nullable: false),
                    ReparacionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReparacionItem", x => new { x.idHerramienta, x.idReparacion });
                    table.ForeignKey(
                        name: "FK_ReparacionItem_Herramienta_HerramientaId",
                        column: x => x.HerramientaId,
                        principalTable: "Herramienta",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReparacionItem_Reparacion_ReparacionId",
                        column: x => x.ReparacionId,
                        principalTable: "Reparacion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AlquilarItem_AlquilerId",
                table: "AlquilarItem",
                column: "AlquilerId");

            migrationBuilder.CreateIndex(
                name: "IX_CompraItem_CompraId",
                table: "CompraItem",
                column: "CompraId");

            migrationBuilder.CreateIndex(
                name: "IX_CompraItem_HerramientaId",
                table: "CompraItem",
                column: "HerramientaId");

            migrationBuilder.CreateIndex(
                name: "IX_Herramienta_Fabricanteid",
                table: "Herramienta",
                column: "Fabricanteid");

            migrationBuilder.CreateIndex(
                name: "IX_OfertaItem_herramientaId",
                table: "OfertaItem",
                column: "herramientaId");

            migrationBuilder.CreateIndex(
                name: "IX_OfertaItem_ofertaId",
                table: "OfertaItem",
                column: "ofertaId");

            migrationBuilder.CreateIndex(
                name: "IX_ReparacionItem_HerramientaId",
                table: "ReparacionItem",
                column: "HerramientaId");

            migrationBuilder.CreateIndex(
                name: "IX_ReparacionItem_ReparacionId",
                table: "ReparacionItem",
                column: "ReparacionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AlquilarItem");

            migrationBuilder.DropTable(
                name: "CompraItem");

            migrationBuilder.DropTable(
                name: "OfertaItem");

            migrationBuilder.DropTable(
                name: "ReparacionItem");

            migrationBuilder.DropTable(
                name: "Alquiler");

            migrationBuilder.DropTable(
                name: "Compra");

            migrationBuilder.DropTable(
                name: "Oferta");

            migrationBuilder.DropTable(
                name: "Herramienta");

            migrationBuilder.DropTable(
                name: "Reparacion");

            migrationBuilder.DropTable(
                name: "Fabricante");
        }
    }
}
