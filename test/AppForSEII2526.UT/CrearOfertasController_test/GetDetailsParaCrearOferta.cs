using AppForSEII2526.API.Controllers;
using AppForSEII2526.API.DTOs.CrearOfertasDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppForSEII2526.UT.CrearOfertasController_test
{
    public class GetDetailsParaCrearOferta : AppForSEII25264SqliteUT
    {
        public GetDetailsParaCrearOferta()
        {
            var fabricantes = new List<Fabricante>() {
                new Fabricante("Makita"),
                new Fabricante("Bosch"),
                new Fabricante("Stanley"),
                new Fabricante("Bob")
            };

            var herramientas = new List<Herramienta>(){
                new Herramienta(fabricantes[0], "Hierro", "Martillo", 30, 3),
                new Herramienta(fabricantes[2], "Acero", "Sierra", 50, 7),
                new Herramienta(fabricantes[1], "Acero", "Cuchillo", 20, 2),
            };

            var oferta = new Oferta(
                DateTime.Today.AddDays(1),
                DateTime.Today.AddDays(10),
                tiposMetodoPago.TarjetaCredito,
                tiposDirigidaOferta.Clientes,
                DateTime.Now,
                new List<OfertaItem>()
            );

            var ofertaItem1 = new OfertaItem(
                herramientas[0], 
                oferta,
                10, 
                27 
            );
            ofertaItem1.precioOriginal = (decimal)herramientas[0].Precio;

            var ofertaItem2 = new OfertaItem(
                herramientas[2],
                oferta,
                15, 
                17  
            );
            ofertaItem2.precioOriginal = (decimal)herramientas[2].Precio;

            oferta.OfertaItems.Add(ofertaItem1);
            oferta.OfertaItems.Add(ofertaItem2);
            oferta.PrecioTotal = 44;

            _context.AddRange(fabricantes);
            _context.AddRange(herramientas);
            _context.Add(oferta);
            _context.SaveChanges();
        }

        [Fact]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetOfertaDetail_NotFound_test()
        {
            // Arrange
            var mock = new Mock<ILogger<CrearOfertasController>>();
            ILogger<CrearOfertasController> logger = mock.Object;
            var controller = new CrearOfertasController(_context, logger);

            // Act
            var result = await controller.GetOfertaDetail(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        public async Task GetOfertaDetail_Found_test()
        {
            // Arrange
            var mock = new Mock<ILogger<CrearOfertasController>>();
            ILogger<CrearOfertasController> logger = mock.Object;
            var controller = new CrearOfertasController(_context, logger);

            var expectedOfertaItems = new List<OfertaItemDTO>
            {
                new OfertaItemDTO("Martillo", "Hierro", "Makita", 30, 27),
                new OfertaItemDTO("Cuchillo", "Acero", "Bosch", 20, 17)
            };

            var expectedOferta = new CrearOfertasDetailDTO(
                1, 
                new DateTime(2025, 12, 23),
                new DateTime(2025, 12, 20),
                new DateTime(2025, 12, 25), 
                tiposMetodoPago.PayPal,
                tiposDirigidaOferta.Clientes, 
                expectedOfertaItems
            );

            // Act 
            var result = await controller.GetOfertaDetail(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var ofertaDTOActual = Assert.IsType<CrearOfertasDetailDTO>(okResult.Value);

            //propiedades básicas
            Assert.Equal(expectedOferta.Id, ofertaDTOActual.Id);
            Assert.Equal(expectedOferta.FechaInicio, ofertaDTOActual.FechaInicio);
            Assert.Equal(expectedOferta.FechaFinal, ofertaDTOActual.FechaFinal);
            Assert.Equal(expectedOferta.MetodoPago, ofertaDTOActual.MetodoPago);
            Assert.Equal(expectedOferta.DirigidoA, ofertaDTOActual.DirigidoA);

            //items
            Assert.Equal(expectedOferta.OfertaItems.Count, ofertaDTOActual.OfertaItems.Count);

            for (int i = 0; i < expectedOferta.OfertaItems.Count; i++)
            {
                Assert.Equal(expectedOferta.OfertaItems[i].Nombre, ofertaDTOActual.OfertaItems[i].Nombre);
                Assert.Equal(expectedOferta.OfertaItems[i].Material, ofertaDTOActual.OfertaItems[i].Material);
                Assert.Equal(expectedOferta.OfertaItems[i].Fabricante, ofertaDTOActual.OfertaItems[i].Fabricante);
                Assert.Equal(expectedOferta.OfertaItems[i].PrecioOriginal, ofertaDTOActual.OfertaItems[i].PrecioOriginal);
                Assert.Equal(expectedOferta.OfertaItems[i].PrecioFinal, ofertaDTOActual.OfertaItems[i].PrecioFinal);
            }
        }
    }
}
