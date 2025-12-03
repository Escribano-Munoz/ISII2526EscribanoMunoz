using AppForSEII2526.API.Controllers;
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


namespace AppForSEII2526.UT.ComprasController_test
{
    public class PostCompras_test : AppForSEII25264SqliteUT
    {
        private const string _nombreCliente = "Ricardo";
        private const string _apellidoCliente = "Escribano";
        private const string _direccionEnvio = "Avda. España s/n, Albacete 02071";

        private const string _herramienta1Nombre = "Martillo";
        private const string _herramienta1Fabricante = "Hierro";
        private const string _herramienta2Nombre = "Sierra";
        private const string _herramienta2Fabricante = "Acero";

        public PostCompras_test()
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


            _context.Add(user);
            _context.AddRange(fabricantes);
            _context.AddRange(herramientas);
            _context.SaveChanges();
        }

        public static IEnumerable<object[]> TestCasesFor_CreateCompra()
        {
            var compraNoITem = new CompraCreateDTO(_nombreCliente, _apellidoCliente,
                _direccionEnvio,
                DateTime.Today.AddDays(2), TiposMetodoPago.TarjetaCredito, new List<CompraItemDTO>());

            var compraItems = new List<CompraItemDTO>() { new CompraItemDTO(1, _herramienta1Nombre, _herramienta1Fabricante, 30.0, 1, "Martillo con cabeza de hierro") };

            var compraFromBeforeToday = new CompraCreateDTO(_nombreCliente, _apellidoCliente,
                _direccionEnvio,
                DateTime.Today.AddDays(-1), TiposMetodoPago.TarjetaCredito, compraItems);

            var CompraApplicationUser = new CompraCreateDTO("Juan", "Palomo",
                _direccionEnvio,
                DateTime.Today.AddDays(2), TiposMetodoPago.TarjetaCredito, compraItems);

            var compraHerramientaNotAvailable = new CompraCreateDTO(_nombreCliente, _apellidoCliente,
                _direccionEnvio,
                DateTime.Today.AddDays(2), TiposMetodoPago.TarjetaCredito, new List<CompraItemDTO>() {new CompraItemDTO(999, "Alicates", "Hierro", 30.0, 2, "Descripcion" ) });

            var compraCantidadInvalida = new CompraCreateDTO(_nombreCliente, _apellidoCliente,
                _direccionEnvio,
                DateTime.Today.AddDays(2), TiposMetodoPago.TarjetaCredito, new List<CompraItemDTO>() { new CompraItemDTO(1, "Martillo", "Hierro", 30.0, 0, "Martillo con cabeza de hierro") });

            var compraMetodoPagoInvalido = new CompraCreateDTO(_nombreCliente, _apellidoCliente,
                _direccionEnvio,
                DateTime.Today.AddDays(2), (TiposMetodoPago)4, compraItems);

            var allTests = new List<object[]>
            {             //input for createcompra - Error expected
                new object[] { compraNoITem, "Error! Debes incluir al menos una herramienta para comprar",  },
                new object[] { compraFromBeforeToday, "Error! Tu fecha de compra debe empezar al menos hoy", },
                new object[] { CompraApplicationUser, "Error! Cliente no esta registrado", },
                new object[] { compraHerramientaNotAvailable, "Error! Herramienta con ID", },
                new object[] { compraCantidadInvalida, "Error! Debes seleccionar al menos una herramienta de ese tipo para comprarla", },
                new object[] { compraMetodoPagoInvalido, "Error! El Metodo de Pago no es valido", },
            };

            return allTests;
        }

        [Theory]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        [MemberData(nameof(TestCasesFor_CreateCompra))]
        public async Task CreateRental_Error_test(CompraCreateDTO compraDTO, string errorExpected)
        {
            // Arrange
            var mock = new Mock<ILogger<ComprasController>>();
            ILogger<ComprasController> logger = mock.Object;

            var controller = new ComprasController(_context, logger);

            // Act
            var result = await controller.CreateCompra(compraDTO);

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
        public async Task CreateCompra_Success_test()
        {
            // Arrange
            var mock = new Mock<ILogger<ComprasController>>();
            ILogger<ComprasController> logger = mock.Object;

            var controller = new ComprasController(_context, logger);

            var herramienta = await _context.Herramienta.FirstOrDefaultAsync(h => h.Id == 1);
            Assert.NotNull(herramienta);

            var compraDTO = new CompraCreateDTO("Ricardo", "Escribano",
                _direccionEnvio,
                DateTime.Today.AddDays(2), TiposMetodoPago.TarjetaCredito, new List<CompraItemDTO>() {new CompraItemDTO(1, "Martillo", "Hierro", 30.0, 1, "Martillo con cabeza de hierro")});

            var expectedcompraDetailDTO = new CompraDetailDTO(1, "Ricardo", "Escribano",
                _direccionEnvio,
                DateTime.Today.AddDays(2), TiposMetodoPago.TarjetaCredito, new List<CompraItemDTO>() {new CompraItemDTO(1, "Martillo", "Hierro", 30.0, 1, "Martillo con cabeza de hierro") });

            // Act
            var result = await controller.CreateCompra(compraDTO);

            if (result is ConflictObjectResult conflictResult)
            {
                // Esto te ayudará a ver qué conflicto está ocurriendo
                var conflictValue = conflictResult.Value;
                throw new Exception($"Conflict occurred: {conflictValue}");
            }

            //Assert
            //we check that the response type is BadRequest and obtain the error returned
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var actualCompraDetailDTO = Assert.IsType<CompraDetailDTO>(createdResult.Value);

            Assert.Equal(expectedcompraDetailDTO, actualCompraDetailDTO);
            Assert.Equal(expectedcompraDetailDTO.ApellidoCliente, actualCompraDetailDTO.ApellidoCliente);
            Assert.Equal(expectedcompraDetailDTO.NombreCliente, actualCompraDetailDTO.NombreCliente);
            Assert.Equal(expectedcompraDetailDTO.FechaCompra, actualCompraDetailDTO.FechaCompra);
            Assert.Equal(expectedcompraDetailDTO.DireccionEnvio, actualCompraDetailDTO.DireccionEnvio);
            Assert.Equal(expectedcompraDetailDTO.Id, actualCompraDetailDTO.Id);
            Assert.Equal(expectedcompraDetailDTO.MetodoPago, actualCompraDetailDTO.MetodoPago);
            Assert.Equal(expectedcompraDetailDTO.CompraItems, actualCompraDetailDTO.CompraItems);

        }
    }
}
