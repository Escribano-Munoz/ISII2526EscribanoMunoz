using AppForSEII2526.API.Controllers;
using AppForSEII2526.API.DTOs.ReparacionDTOs;
using AppForSEII2526.UT;
using Humanizer.Localisation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppForSEII2526.UT.ReparacionesController_test
{
    public class PostReparaciones_test : AppForSEII25264SqliteUT
    {
        public PostReparaciones_test()
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

            var reparacion = new Reparacion(new List<ReparacionItem>(), DateTime.Today.AddDays(4).Date, DateTime.Today, 30.0f, tiposMetodoPago.TarjetaCredito, user);


            _context.Add(user);
            _context.AddRange(fabricantes);
            _context.AddRange(herramientas);
            _context.Add(reparacion);
            _context.SaveChanges();
        }

        public static IEnumerable<object[]> TestCasesFor_CreateReparacion()
        {
            var reparacionItems = new List<ReparacionItemDTO>
            {
                new ReparacionItemDTO(1, "Martillo", 30.0, 2, "Mango roto"),
                new ReparacionItemDTO(2, "Sierra", 50.0, 1, "Filo desafilado")
            };

            //Caso 1: Cliente no existe
            var usuarioNoExiste = new ReparacionCreateDTO("NoExiste", "NoExiste",
                DateTime.Today.AddDays(5), DateTime.Today.AddDays(2), tiposMetodoPago.TarjetaCredito, reparacionItems);

            //Caso 2: Sin items en reparación
            var sinItems = new ReparacionCreateDTO("Victoria", "Escribano Tarraga",
                DateTime.Today.AddDays(5), DateTime.Today.AddDays(2), tiposMetodoPago.TarjetaCredito, new List<ReparacionItemDTO>());

            //Caso 3: Fecha de entrega anterior a hoy
            var fechaEntregaPasada = new ReparacionCreateDTO("Victoria", "Escribano Tarraga",
                DateTime.Today.AddDays(5), DateTime.Today.AddDays(-1), tiposMetodoPago.TarjetaCredito, reparacionItems);

            //Caso 4: Fecha recogida anterior a la Fecha entrega
            var fechasInvertidas = new ReparacionCreateDTO("Victoria", "Escribano Tarraga",
                DateTime.Today.AddDays(2), DateTime.Today.AddDays(5), tiposMetodoPago.TarjetaCredito, reparacionItems);

            //Caso 5: Metodo de pago invalido
            var metodoPagoInvalido = new ReparacionCreateDTO("Victoria", "Escribano Tarraga",
                DateTime.Today.AddDays(5), DateTime.Today.AddDays(2), (tiposMetodoPago)5, reparacionItems);

            //Caso 6: Cantidad invalida
            var cantidadInvalida = new ReparacionCreateDTO("Victoria", "Escribano Tarraga",
                DateTime.Today.AddDays(5), DateTime.Today.AddDays(2), tiposMetodoPago.TarjetaCredito, new List<ReparacionItemDTO>() { new ReparacionItemDTO(1, "Martillo", 30.0, 0, "Mango roto") });

            // Caso 7: Herramienta no existe
            var herramientaNoExiste = new ReparacionCreateDTO("Victoria", "Escribano Tarraga",
                DateTime.Today.AddDays(5), DateTime.Today.AddDays(2), tiposMetodoPago.TarjetaCredito, new List<ReparacionItemDTO>() { new ReparacionItemDTO(999, "HerramientaInexistente", 30.0, 2, "Descripción") });

            var allTests = new List<object[]>
            {             //input for createpurchase - Error expected
                new object[] { usuarioNoExiste, "Error! Cliente no registrado",  },
                new object[] { sinItems, "Error! Debes incluir al menos una herramienta para reparar", },
                new object[] { fechaEntregaPasada, "Error! La fecha de entrega debe ser posterior a hoy", },
                new object[] { fechasInvertidas, "Error! La fecha de recogida debe ser posterior a la fecha de entrega", },
                new object[] { metodoPagoInvalido, "Error! El metodo de pago seleccionado no es valido", },
                new object[] { cantidadInvalida, "Error! La cantidad para la herramienta debe ser mayor a 0", },
                new object[] { herramientaNoExiste, "Error! La herramienta con ID", }
            };

            return allTests;
        }

        [Theory]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        [MemberData(nameof(TestCasesFor_CreateReparacion))]
        public async Task CreateReparacion_Error_test(ReparacionCreateDTO reparacionDTO, string errorExpected)
        {
            // Arrange
            var mock = new Mock<ILogger<ReparacionesController>>();
            ILogger<ReparacionesController> logger = mock.Object;

            var controller = new ReparacionesController(_context, logger);

            // Act
            var result = await controller.CreateReparacion(reparacionDTO);

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
        public async Task CreateReparacion_Success_test()
        {
            // Arrange
            var mock = new Mock<ILogger<ReparacionesController>>();
            ILogger<ReparacionesController> logger = mock.Object;

            var controller = new ReparacionesController(_context, logger);

            DateTime entrega = DateTime.Today.AddDays(2);
            DateTime recogida = DateTime.Today.AddDays(10);

            var reparacionDTO = new ReparacionCreateDTO("Victoria", "Escribano Tarraga",
                recogida, entrega, tiposMetodoPago.TarjetaCredito,
                new List<ReparacionItemDTO>() {
            new ReparacionItemDTO(1, "Martillo", 30.0, 1, "Mango roto")
                });

            var expectedreparacionDetailDTO = new ReparacionDetailDTO(1, "Victoria", "Escribano Tarraga",
                recogida, entrega, tiposMetodoPago.TarjetaCredito,
                new List<ReparacionItemDTO>() {
            new ReparacionItemDTO(1, "Martillo", 30.0, 1, "Mango roto")
                });

            // Act
            var result = await controller.CreateReparacion(reparacionDTO);

            //Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var reparacionCreada = Assert.IsType<ReparacionDetailDTO>(createdResult.Value);

            Assert.Equal("Victoria", reparacionCreada.NombreCliente);
            Assert.Equal("Escribano Tarraga", reparacionCreada.ApellidoCliente);
            Assert.Equal(recogida, reparacionCreada.FechaRecogida);
            Assert.Equal(entrega, reparacionCreada.FechaEntrega);
            Assert.Equal(tiposMetodoPago.TarjetaCredito, reparacionCreada.MetodoPago);
            Assert.Single(reparacionCreada.ReparacionItems);
        }

    }
}
