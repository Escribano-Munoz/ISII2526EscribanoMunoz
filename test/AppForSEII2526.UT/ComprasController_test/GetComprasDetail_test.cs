using AppForSEII2526.API.Controllers;
using AppForSEII2526.API.DTOs.CompraDTOs;
using Humanizer.Localisation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppForSEII2526.UT.ComprasController_test
{
    public class GetComprasDetail_test : AppForSEII25264SqliteUT
    {
        public GetComprasDetail_test()
        {

            var fabricantes = new List<Fabricante>() {
                new Fabricante("Makita"),
                new Fabricante("Bosch"),
                new Fabricante("Stanley")
            };

            var herramientas = new List<Herramienta>(){
                new Herramienta(fabricantes[0],"Hierro","Martillo" ,30,3),
                new Herramienta(fabricantes[2],"Acero","Sierra", 50,7),
                new Herramienta(fabricantes[1],"Acero","Cuchilla" ,20,2),
                new Herramienta(fabricantes[0],"Hierro","Llave" , 35,4)
            };

            ApplicationUser user = new ApplicationUser(1, "Ricardo", "Escribano", "641530789", tiposMetodosPago.TarjetaCredito, "Avda. España s/n, Albacete 02071", "Ricardo@alu.uclm.es");

            var compra = new Compra(1, DateTime.Today.AddDays(2),
                    new List<CompraItem>(),TiposMetodoPago.TarjetaCredito, "Avda. España s/n, Albacete 02071",
                    user);
            compra.CompraItems.Add(new CompraItem(herramientas[0], compra, "Martillo", "Hierro", 30.0, 3, "Martillo con cabeza de hierro"));

            _context.Add(user);
            _context.AddRange(fabricantes);
            _context.AddRange(herramientas);
            _context.AddRange(compra);
            _context.SaveChanges();
        }

        [Fact]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetCompraDetail_NotFound_test()
        {
            // Arrange
            var mock = new Mock<ILogger<ComprasController>>();
            ILogger<ComprasController> logger = mock.Object;

            var controller = new ComprasController(_context, logger);

            // Act
            var result = await controller.GetCompraDetail(0);

            //Assert
            //we check that the response type is OK and obtain the list of herramientas
            Assert.IsType<NotFoundResult>(result);

        }

        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        public async Task GetCompraDetail_Found_test()
        {
            // Arrange
            var mock = new Mock<ILogger<ComprasController>>();
            ILogger<ComprasController> logger = mock.Object;
            var controller = new ComprasController(_context, logger);


            var expectedCompra = new CompraDetailDTO(1, "Ricardo", "Escribano",
                        "Avda. España s/n, Albacete 02071", 
                        DateTime.Today.AddDays(2),
                        new List<CompraItemDTO>());
            expectedCompra.CompraItems.Add(new CompraItemDTO(1, "Martillo", "Hierro", 30.0, 3, "Martillo con cabeza de hierro"));

            // Act 
            var result = await controller.GetCompraDetail(1);

            //Assert
            //we check that the response type is OK and obtain the compra
            var okResult = Assert.IsType<OkObjectResult>(result);
            var compraDTOActual = Assert.IsType<CompraDetailDTO>(okResult.Value);
            var eq = expectedCompra.Equals(compraDTOActual);
            //we check that the expected and actual are the same
            Assert.Equal(expectedCompra, compraDTOActual);

        }
    }
}
