using AppForSEII2526.API.Controllers;
using AppForSEII2526.API.DTOs.ReparacionDTOs;
using AppForSEII2526.API.Models;
using Humanizer.Localisation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppForSEII2526.UT.ReparacionesController_test
{
    public class GetDetailsReparaciones_test : AppForSEII25264SqliteUT
    {
        public GetDetailsReparaciones_test()
        {

            var fabricantes = new List<Fabricante>() {
                new Fabricante("Makita"),
                new Fabricante("Bosch"),
                new Fabricante("Stanley")
            };


            var herramientas = new List<Herramienta>(){
                new Herramienta(fabricantes[0],"Hierro","Martillo" ,30, 3),
                new Herramienta(fabricantes[2],"Acero","Sierra", 50, 7),
                new Herramienta(fabricantes[1],"Acero","Cuchilla" ,20, 2),
                new Herramienta(fabricantes[0],"Hierro","Llave" , 35, 4)
            };

            ApplicationUser user = new ApplicationUser(1, "Victoria", "Escribano Tarraga", "696852142", tiposMetodosPago.TarjetaCredito, "Calle Carretera de Valencia 1", "victoria.escribano2@alu.uclm.es");

            var reparacion = new Reparacion(new List<ReparacionItem>(), DateTime.Today.AddDays(4), DateTime.Today, 30.0f, tiposMetodoPago.TarjetaCredito, user);



            reparacion.ReparacionItems.Add(new ReparacionItem(herramientas[0], reparacion));

            _context.Add(user);
            _context.AddRange(fabricantes);
            _context.AddRange(herramientas);
            _context.Add(reparacion);
            _context.SaveChanges();
        }

        [Fact]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetReparacionDetail_NotFound_test()
        {
            // Arrange
            var mock = new Mock<ILogger<ReparacionesController>>();
            ILogger<ReparacionesController> logger = mock.Object;

            var controller = new ReparacionesController(_context, logger);

            // Act
            var result = await controller.GetReparacionDetail(0);

            //Assert
            //we check that the response type is OK and obtain the list of herramientas
            Assert.IsType<NotFoundResult>(result);

        }

        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        public async Task GetReparacionDetail_Found_test()
        {
            // Arrange
            var mock = new Mock<ILogger<ReparacionesController>>();
            ILogger<ReparacionesController> logger = mock.Object;
            var controller = new ReparacionesController(_context, logger);


            var expectedReparacion = new ReparacionDetailDTO(1, "Victoria", "Escribano Tarraga", DateTime.Today.AddDays(4), DateTime.Today, new List<ReparacionItemDTO>());
            expectedReparacion.ReparacionItems.Add(new ReparacionItemDTO(1, "Martillo", 30, 1, "Mango roto"));

            // Act 
            var result = await controller.GetReparacionDetail(1);

            //Assert
            //we check that the response type is OK and obtain the reparacion
            var okResult = Assert.IsType<OkObjectResult>(result);
            var reparacionDTOActual = Assert.IsType<ReparacionDetailDTO>(okResult.Value);

            Assert.Equal(expectedReparacion.Id, reparacionDTOActual.Id);
            Assert.Equal(expectedReparacion.NombreCliente, reparacionDTOActual.NombreCliente);
            Assert.Equal(expectedReparacion.ApellidoCliente, reparacionDTOActual.ApellidoCliente);
            Assert.Equal(expectedReparacion.MetodoPago, reparacionDTOActual.MetodoPago);
            Assert.Equal(expectedReparacion.FechaRecogida.Date, reparacionDTOActual.FechaRecogida.Date);
            Assert.Equal(expectedReparacion.FechaEntrega.Date, reparacionDTOActual.FechaEntrega.Date);
        }
    }
}
