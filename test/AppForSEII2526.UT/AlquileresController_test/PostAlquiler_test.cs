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
    public class PostAlquiler_test : AppForSEII25264SqliteUT
    {
        private const string _clienteNombre = "Juan Jose";
        private const string _clienteApellido = "Escribano";
        private const string _direccionEnvio = "Calle Carretera de Valencia";

        private const string _herramienta1Nombre = "Martillo";
        private const string _herramienta1Material = "Hierro";
        private const string _herramienta2Nombre = "Sierra";
        private const string _herramienta2Material = "Acero";

        public PostAlquiler_test()
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

            ApplicationUser user = new ApplicationUser(1, "Juan José", "Escribano", "640502222", tiposMetodosPago.TarjetaCredito, "Calle Carretera de Valencia", "juanjose.escribano@alu.uclm.es");

            var alquiler = new Alquiler(user, 15.0, DateTime.Now, DateTime.Today.AddDays(2), DateTime.Today.AddDays(5), "Calle Carretera de Valencia", TiposMetodoPago.TarjetaCredito, "Juan José", "Escribano Tarraga", new List<AlquilarItem>());

            alquiler.AlquilarItems.Add(new AlquilarItem(herramientas[0], alquiler, "Martillo", "Hierro", 30.0, 3));

            _context.ApplicationUsers.Add(user);
            _context.AddRange(fabricantes);
            _context.AddRange(herramientas);
            _context.Add(alquiler);
            _context.SaveChanges();
        }

        public static IEnumerable<object[]> TestCasesFor_CreateAlquiler()
        {
            var alquilerNoITem = new AlquilerCreateDTO(_clienteNombre, _clienteApellido,
                _direccionEnvio,
                DateTime.Today.AddDays(2), DateTime.Today.AddDays(5), new List<AlquilarItemDTO>());

            var alquilarItems = new List<AlquilarItemDTO>() { new AlquilarItemDTO(2, _herramienta2Nombre, _herramienta2Material, 50.0, 9) };

            var alquilerInicioAntesQueHoy = new AlquilerCreateDTO(_clienteNombre, _clienteApellido,
                _direccionEnvio,
                DateTime.Today, DateTime.Today.AddDays(5), alquilarItems);

            var alquilerFinAntesQueInicio = new AlquilerCreateDTO(_clienteNombre, _clienteApellido,
                _direccionEnvio,
                DateTime.Today.AddDays(5), DateTime.Today.AddDays(2), alquilarItems);

            var AlquilerApplicationUser = new AlquilerCreateDTO("Pepe", _clienteApellido,
                _direccionEnvio,
                DateTime.Today.AddDays(2), DateTime.Today.AddDays(4), alquilarItems);

            var alquilerHerramientaNoDisponible = new AlquilerCreateDTO(_clienteNombre, _clienteApellido,
                _direccionEnvio,
                DateTime.Today.AddDays(2), DateTime.Today.AddDays(5),
                new List<AlquilarItemDTO>() { new AlquilarItemDTO(1, _herramienta1Nombre, _herramienta1Material, 30.0,8) });


            var allTests = new List<object[]>
            {             //input for createalquiler - Error expected
                new object[] { alquilerNoITem, "Error! You must include at least one movie to be rented",  },
                new object[] { alquilerInicioAntesQueHoy, "Error! Your rental date must start later than today", },
                new object[] { alquilerFinAntesQueInicio, "Error! Your rental must end later than it starts", },
                new object[] { AlquilerApplicationUser, "Error! UserName is not registered", },
                new object[] { alquilerHerramientaNoDisponible, "Error! Movie titled 'The lord of the rings' is not available for being rented from", },
            };

            return allTests;
        }

        [Theory]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        [MemberData(nameof(TestCasesFor_CreateAlquiler))]
        public async Task CreateRental_Error_test(AlquilerCreateDTO alquilerDTO, string errorExpected)
        {
            // Arrange
            var mock = new Mock<ILogger<AlquileresController>>();
            ILogger<AlquileresController> logger = mock.Object;

            var controller = new AlquileresController(_context, logger);

            // Act
            var result = await controller.CreateAlquiler(alquilerDTO);

            //Assert
            //we check that the response type is BadRequest and obtain the error returned
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var problemDetails = Assert.IsType<ValidationProblemDetails>(badRequestResult.Value);

            var errorActual = problemDetails.Errors.First().Value[0];

            //we check that the expected error message and actual are the same
            Assert.StartsWith(errorExpected, errorActual);

        }

        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        public async Task CreateAlquiler_Success_test()
        {
            // Arrange
            var mock = new Mock<ILogger<AlquileresController>>();
            ILogger<AlquileresController> logger = mock.Object;

            var controller = new AlquileresController(_context, logger);

            DateTime to = DateTime.Today.AddDays(6);
            DateTime from = DateTime.Today.AddDays(7);

            var alquilerDTO = new AlquilerCreateDTO(_clienteNombre, _clienteApellido,
                _direccionEnvio,
                to, from, new List<AlquilarItemDTO>()
                { new AlquilarItemDTO(2, _herramienta1Nombre, _herramienta1Material, 30.0,8) });

            var expectedalquilerDetailDTO = new AlquilerDetailDTO(2, DateTime.Now,
                _clienteNombre, _clienteApellido,
                _direccionEnvio,
                to, from, new List<AlquilarItemDTO>()
                { new AlquilarItemDTO(2, _herramienta1Nombre, _herramienta1Material, 30.0,8) });

            // Act
            var result = await controller.CreateAlquiler(alquilerDTO);

            //Assert
            //we check that the response type is BadRequest and obtain the error returned
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var actualAlquilerDetailDTO = Assert.IsType<AlquilerDetailDTO>(createdResult.Value);

            Assert.Equal(expectedalquilerDetailDTO, actualAlquilerDetailDTO);

        }

    }
}
