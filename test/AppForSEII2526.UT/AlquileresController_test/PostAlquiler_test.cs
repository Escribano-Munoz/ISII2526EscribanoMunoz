using AppForSEII2526.API.Controllers;
using AppForSEII2526.API.DTOs.AlquilerDTOs;
using AppForSEII2526.API.DTOs.CompraDTOs;
using AppForSEII2526.API.Models;
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

            ApplicationUser user = new ApplicationUser(1, "Juan Jose", "Escribano", "640502222", tiposMetodosPago.TarjetaCredito, "Calle Carretera de Valencia", "juanjose.escribano@alu.uclm.es");

            var alquiler = new Alquiler(user, 15.0, DateTime.Today, DateTime.Today.AddDays(2), DateTime.Today.AddDays(5), "Calle Carretera de Valencia", TiposMetodoPago.TarjetaCredito, "Juan Jose", "Escribano", new List<AlquilarItem>());

            

            _context.ApplicationUsers.Add(user);
            _context.AddRange(fabricantes);
            _context.AddRange(herramientas);
            _context.Add(alquiler);
            _context.SaveChanges();
        }

        public static IEnumerable<object[]> TestCasesFor_CreateAlquiler()
        {
            var alquilarItems = new List<AlquilarItemDTO>() { new AlquilarItemDTO(2, _herramienta2Nombre, _herramienta2Material, 50.0, 9) };

            var alquilerNoITem = new AlquilerCreateDTO(_clienteNombre, _clienteApellido,
                _direccionEnvio,
                DateTime.Today.AddDays(2), DateTime.Today.AddDays(5), TiposMetodoPago.TarjetaCredito, new List<AlquilarItemDTO>());

            var alquilerInicioAntesQueHoy = new AlquilerCreateDTO(_clienteNombre, _clienteApellido,
                _direccionEnvio,
                DateTime.Today, DateTime.Today.AddDays(5), TiposMetodoPago.TarjetaCredito, alquilarItems);

            var alquilerFinAntesQueInicio = new AlquilerCreateDTO(_clienteNombre, _clienteApellido,
                _direccionEnvio,
                DateTime.Today.AddDays(5), DateTime.Today.AddDays(2), TiposMetodoPago.TarjetaCredito, alquilarItems);

            var AlquilerApplicationUser = new AlquilerCreateDTO("Pepe", _clienteApellido,
                _direccionEnvio,
                DateTime.Today.AddDays(2), DateTime.Today.AddDays(4), TiposMetodoPago.TarjetaCredito, alquilarItems);

            var alquilerCantidadInvalida = new AlquilerCreateDTO(_clienteNombre, _clienteApellido,
                _direccionEnvio,
                DateTime.Today.AddDays(2), DateTime.Today.AddDays(5), TiposMetodoPago.TarjetaCredito, new List<AlquilarItemDTO>() { new AlquilarItemDTO(1, "Martillo", "Hierro", 30.0, 0) });

            var alquilerMetodoPagoInvalido = new AlquilerCreateDTO(_clienteNombre, _clienteApellido,
                _direccionEnvio,
                DateTime.Today.AddDays(2), DateTime.Today.AddDays(5), (TiposMetodoPago)4, alquilarItems);

            var alquilerHerramientaNoDisponible = new AlquilerCreateDTO(_clienteNombre, _clienteApellido,
                _direccionEnvio,
                DateTime.Today.AddDays(2), DateTime.Today.AddDays(5), TiposMetodoPago.TarjetaCredito,
                new List<AlquilarItemDTO>() { new AlquilarItemDTO(999, "Destornillador", "Hierro", 30.0,8) });


            var allTests = new List<object[]>
            {             //input for createalquiler - Error expected
                new object[] { alquilerNoITem,"Error! Debes incluir una herramienta para que pueda ser alquilada" },
                new object[] { alquilerInicioAntesQueHoy, "Error! Tu fecha de alquiler debe empezar despues hoy"},
                new object[] { alquilerFinAntesQueInicio, "Error! Tu fecha de alquiler final debe terminar despues de la fecha de inicio"},
                new object[] { AlquilerApplicationUser, "Error! Nombre de usuario no registrado"},
                new object[] { alquilerCantidadInvalida, "Error! Debes seleccionar al menos una herramienta de ese tipo para alquilarla" },
                new object[] { alquilerMetodoPagoInvalido, "Error! El Metodo de Pago no es valido", },
                new object[] { alquilerHerramientaNoDisponible,"Error! La herramienta con ID '999' no esta disponible para ser alquilada"},
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
                { new AlquilarItemDTO(1, _herramienta1Nombre, _herramienta1Material, 30.0,8) });

            var expectedalquilerDetailDTO = new AlquilerDetailDTO(2, DateTime.Today,
                _clienteNombre, _clienteApellido,
                _direccionEnvio,
                to, from, new List<AlquilarItemDTO>()
                { new AlquilarItemDTO(1, _herramienta1Nombre, _herramienta1Material, 30.0,8) });

            // Act
            var result = await controller.CreateAlquiler(alquilerDTO);

            //Assert
            //we check that the response type is BadRequest and obtain the error returned
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var actualAlquilerDetailDTO = Assert.IsType<AlquilerDetailDTO>(createdResult.Value);

            Assert.Equal(expectedalquilerDetailDTO, actualAlquilerDetailDTO);
            Assert.Equal(expectedalquilerDetailDTO.ApellidoCliente, actualAlquilerDetailDTO.ApellidoCliente);
            Assert.Equal(expectedalquilerDetailDTO.NombreCliente, actualAlquilerDetailDTO.NombreCliente);
            Assert.Equal(expectedalquilerDetailDTO.FechaAlquiler, actualAlquilerDetailDTO.FechaAlquiler);
            Assert.Equal(expectedalquilerDetailDTO.DireccionEnvio, actualAlquilerDetailDTO.DireccionEnvio);
            Assert.Equal(expectedalquilerDetailDTO.Id, actualAlquilerDetailDTO.Id);
            Assert.Equal(expectedalquilerDetailDTO.AlquilarItems, actualAlquilerDetailDTO.AlquilarItems);
            Assert.Equal(expectedalquilerDetailDTO.FechaInicio.Date, actualAlquilerDetailDTO.FechaInicio.Date);
            Assert.Equal(expectedalquilerDetailDTO.FechaFin.Date, actualAlquilerDetailDTO.FechaFin.Date);

        }

    }
}
