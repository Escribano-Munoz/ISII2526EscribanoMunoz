using AppForSEII2526.API.Controllers;
using AppForSEII2526.API.DTOs.CrearOfertasDTOs;
using AppForSEII2526.API.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppForSEII2526.UT.CrearOfertasController_test
{
    public class PostParaCrearOfertas_test : AppForSEII25264SqliteUT
    {
        private const string _herramienta1Nombre = "Martillo";
        private const string _herramienta1Material = "Hierro";
        private const string _herramienta2Nombre = "Sierra";
        private const string _herramienta2Material = "Acero";
        private const string _herramienta3Nombre = "Cuchillo";
        private const string _fabricante1Nombre = "Makita";
        private const string _fabricante2Nombre = "Stanley";
        private const string _fabricante3Nombre = "Bosch";

        public PostParaCrearOfertas_test()
        {
            var fabricantes = new List<Fabricante>() {
                new Fabricante(_fabricante1Nombre),
                new Fabricante(_fabricante2Nombre),
                new Fabricante(_fabricante3Nombre)
            };

            var herramientas = new List<Herramienta>(){
                new Herramienta(fabricantes[0], _herramienta1Material, _herramienta1Nombre, 30, 3),
                new Herramienta(fabricantes[1], _herramienta2Material, _herramienta2Nombre, 50, 7),
                new Herramienta(fabricantes[2], _herramienta2Material, _herramienta3Nombre, 20, 2)
            };

            var oferta = new Oferta(
                DateTime.Today.AddDays(2),
                DateTime.Today.AddDays(5),
                (tiposMetodoPago)tiposMetodosPago.TarjetaCredito,
                tiposDirigidaOferta.Clientes,
                DateTime.Now,
                new List<OfertaItem>());

            oferta.OfertaItems.Add(new OfertaItem(herramientas[0], oferta, 20, 24.0m));

            _context.Fabricante.AddRange(fabricantes);
            _context.Herramienta.AddRange(herramientas);
            _context.Oferta.Add(oferta);
            _context.SaveChanges();
        }

        public static IEnumerable<object[]> TestCasesFor_CreateOferta()
        {
            
            var ofertaNoItems = new CrearOfertasCreateDTO(
                DateTime.Today.AddDays(2),
                DateTime.Today.AddDays(5),
                (int)tiposMetodosPago.TarjetaCredito,
                tiposDirigidaOferta.Clientes,
                new List<OfertaItemDTO>());

            
            var ofertaItems = new List<OfertaItemDTO>() {
                new OfertaItemDTO(_herramienta2Nombre, _herramienta2Material, _fabricante2Nombre, 50.0m, 40.0m)
                {
                    PorcentajeDescuento = 20
                }
            };

            var ofertaFromBeforeToday = new CrearOfertasCreateDTO(
                DateTime.Today,
                DateTime.Today.AddDays(5),
                (int)tiposMetodosPago.TarjetaCredito,
                tiposDirigidaOferta.Clientes,
                ofertaItems);

            
            var ofertaToBeforeFrom = new CrearOfertasCreateDTO(
                DateTime.Today.AddDays(5),
                DateTime.Today.AddDays(2),
                (int)tiposMetodosPago.TarjetaCredito,
                tiposDirigidaOferta.Clientes,
                ofertaItems);

           
            var ofertaHerramientaNoDisponible = new CrearOfertasCreateDTO(
                DateTime.Today.AddDays(2),
                DateTime.Today.AddDays(5),
                (int)tiposMetodosPago.TarjetaCredito,
                tiposDirigidaOferta.Clientes,
                new List<OfertaItemDTO>() {
                    new OfertaItemDTO("HerramientaInexistente", "Material", "Fabricante", 10.0m, 8.0m)
                    {
                        PorcentajeDescuento = 20
                    }
                });

            
            var ofertaMetodoPagoInvalido = new CrearOfertasCreateDTO(
                DateTime.Today.AddDays(2),
                DateTime.Today.AddDays(5),
                (tiposMetodoPago)99,
                tiposDirigidaOferta.Clientes,
                ofertaItems);

            
            var ofertaPorcentajeInvalido = new CrearOfertasCreateDTO(
                DateTime.Today.AddDays(2),
                DateTime.Today.AddDays(5),
                (int)tiposMetodosPago.TarjetaCredito,
                tiposDirigidaOferta.Clientes,
                new List<OfertaItemDTO>() {
                    new OfertaItemDTO(_herramienta2Nombre, _herramienta2Material, _fabricante2Nombre, 50.0m, 40.0m)
                    {
                        PorcentajeDescuento = 150
                    }
                });

            var allTests = new List<object[]>
            {
                new object[] { ofertaNoItems, "Error! Debe incluir al menos una herramienta para la oferta" },
                new object[] { ofertaFromBeforeToday, "Error! La fecha de inicio debe ser posterior a hoy" },
                new object[] { ofertaToBeforeFrom, "Error! La oferta debe terminar después de que comience" },
                new object[] { ofertaHerramientaNoDisponible, "Error! La herramienta" },
                new object[] { ofertaMetodoPagoInvalido, "Error! El método de pago seleccionado no es válido" },
                new object[] { ofertaPorcentajeInvalido, "Error! El porcentaje de rebaja debe estar entre 0% y 100%" }
            };

            return allTests;
        }

        [Theory]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        [MemberData(nameof(TestCasesFor_CreateOferta))]
        public async Task CreateOferta_Error_test(CrearOfertasCreateDTO ofertaDTO, string errorExpected)
        {
            // Arrange
            var mock = new Mock<ILogger<CrearOfertasController>>();
            ILogger<CrearOfertasController> logger = mock.Object;

            var controller = new CrearOfertasController(_context, logger);

            // Act
            var result = await controller.CreateOferta(ofertaDTO);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var problemDetails = Assert.IsType<ValidationProblemDetails>(badRequestResult.Value);

            var errorActual = problemDetails.Errors.First().Value[0];

            
            Assert.StartsWith(errorExpected, errorActual);
        }

        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        public async Task CreateOferta_Success_test()
        {
            // Arrange
            var mock = new Mock<ILogger<CrearOfertasController>>();
            ILogger<CrearOfertasController> logger = mock.Object;

            var controller = new CrearOfertasController(_context, logger);

            DateTime fechaInicio = DateTime.Today.AddDays(3);
            DateTime fechaFin = DateTime.Today.AddDays(10);

            var ofertaItems = new List<OfertaItemDTO>() {
            new OfertaItemDTO(_herramienta2Nombre, _herramienta2Material, _fabricante2Nombre, 50.0m, 40.0m)
            {
            PorcentajeDescuento = 20
            }
            };

            var ofertaDTO = new CrearOfertasCreateDTO(
                fechaInicio,
                fechaFin,
                tiposMetodoPago.TarjetaCredito,
                tiposDirigidaOferta.Clientes,
                ofertaItems);

            // Act
            var result = await controller.CreateOferta(ofertaDTO);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var actualOfertaDetailDTO = Assert.IsType<CrearOfertasDetailDTO>(createdResult.Value);

            
            Assert.Equal(2, actualOfertaDetailDTO.Id);
            Assert.Equal(fechaInicio, actualOfertaDetailDTO.FechaInicio);
            Assert.Equal(fechaFin, actualOfertaDetailDTO.FechaFinal);
            Assert.Equal(tiposMetodoPago.TarjetaCredito, actualOfertaDetailDTO.MetodoPago);
            Assert.Equal(tiposDirigidaOferta.Clientes, actualOfertaDetailDTO.DirigidoA);

            // Verificamos los items
            var expectedItem = ofertaItems.First();
            var actualItem = actualOfertaDetailDTO.OfertaItems.First();

            Assert.Equal(expectedItem.Nombre, actualItem.Nombre);
            Assert.Equal(expectedItem.Material, actualItem.Material);
            Assert.Equal(expectedItem.Fabricante, actualItem.Fabricante);
            Assert.Equal(expectedItem.PrecioOriginal, actualItem.PrecioOriginal);
            Assert.Equal(expectedItem.PrecioFinal, actualItem.PrecioFinal);
            Assert.Equal(expectedItem.PorcentajeDescuento, actualItem.PorcentajeDescuento);

           
            Assert.Equal(10.0m, actualOfertaDetailDTO.DescuentoTotal);
            Assert.Equal(40.0m, actualOfertaDetailDTO.PrecioTotalFinal); 

            
        }
    }
}