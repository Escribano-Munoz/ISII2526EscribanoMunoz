using AppForSEII2526.API.Controllers;
using AppForSEII2526.API.DTOs.AlquilerDTOs;
using AppForSEII2526.UT;
using Humanizer.Localisation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppForSEII2526.UT.AlquileresController_test
{
    public class GetAlquilerDetail_test : AppForSEII25264SqliteUT
    {
        public GetAlquilerDetail_test()
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

            ApplicationUser user = new ApplicationUser(1, "Juan Jose", "Escribano",  "640502222", tiposMetodosPago.TarjetaCredito,"Calle Carretera de Valencia", "juanjose.escribano@alu.uclm.es");

            var alquiler = new Alquiler(user, 15.0, DateTime.Today, DateTime.Today.AddDays(2), DateTime.Today.AddDays(5), "Calle Carretera de Valencia", TiposMetodoPago.TarjetaCredito, "Juan José", "Escribano Tarraga", new List<AlquilarItem>());

            alquiler.AlquilarItems.Add(new AlquilarItem(herramientas[0], alquiler, "Martillo", "Hierro", 30.0, 3));

            _context.ApplicationUsers.Add(user);
            _context.AddRange(fabricantes);
            _context.AddRange(herramientas);
            _context.Add(alquiler);
            _context.SaveChanges();
        }

        [Fact]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetAlquilerDetail_NotFound_test()
        {
            // Arrange
            var mock = new Mock<ILogger<AlquileresController>>();
            ILogger<AlquileresController> logger = mock.Object;

            var controller = new AlquileresController(_context, logger);

            // Act
            var result = await controller.GetAlquilerDetail(0);

            //Assert
            //we check that the response type is OK and obtain the list of herramientas
            Assert.IsType<NotFoundResult>(result);

        }

        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        public async Task GetAlquilerDetail_Found_test()
        {
            // Arrange
            var mock = new Mock<ILogger<AlquileresController>>();
            ILogger<AlquileresController> logger = mock.Object;
            var controller = new AlquileresController(_context, logger);


            var expectedAlquiler = new AlquilerDetailDTO(1, DateTime.Today, "Juan Jose", "Escribano",
                        "Calle Carretera de Valencia",
                        DateTime.Today.AddDays(2), DateTime.Today.AddDays(5),
                        new List<AlquilarItemDTO>());
            expectedAlquiler.AlquilarItems.Add(new AlquilarItemDTO(1, "Martillo", "Hierro", 30.0, 3));

            // Act 
            var result = await controller.GetAlquilerDetail(1);

            //Assert
            //we check that the response type is OK and obtain the alquiler
            var okResult = Assert.IsType<OkObjectResult>(result);
            var alquilerDTOActual = Assert.IsType<AlquilerDetailDTO>(okResult.Value);
            var eq = expectedAlquiler.Equals(alquilerDTOActual);
            //we check that the expected and actual are the same
            Assert.Equal(expectedAlquiler.ApellidoCliente, alquilerDTOActual.ApellidoCliente);
            Assert.Equal(expectedAlquiler.NombreCliente, alquilerDTOActual.NombreCliente);
            Assert.Equal(expectedAlquiler.FechaAlquiler, alquilerDTOActual.FechaAlquiler);
            Assert.Equal(expectedAlquiler.DireccionEnvio, alquilerDTOActual.DireccionEnvio);
            Assert.Equal(expectedAlquiler.Id, alquilerDTOActual.Id);
            Assert.Equal(expectedAlquiler.AlquilarItems, alquilerDTOActual.AlquilarItems);
            Assert.Equal(expectedAlquiler.FechaAlquiler.Date, alquilerDTOActual.FechaAlquiler.Date);
            Assert.Equal(expectedAlquiler.FechaFin.Date, alquilerDTOActual.FechaFin.Date);

        }
    }
}
