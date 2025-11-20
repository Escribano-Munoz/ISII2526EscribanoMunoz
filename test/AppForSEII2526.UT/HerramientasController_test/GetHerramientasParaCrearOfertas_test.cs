using AppForSEII2526.API.Controllers;
using AppForSEII2526.API.DTOs.HerramientaDTOs;
using AppForSEII2526.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppForSEII2526.UT.HerramientasController_test
{
    public class GetHerramientasParaCrearOfertas_test : AppForSEII25264SqliteUT
    {
        public GetHerramientasParaCrearOfertas_test()
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



            _context.AddRange(fabricantes);
            _context.AddRange(herramientas);
            _context.SaveChanges();

        }


        public static IEnumerable<object[]> TestCasesFor_GetHerramientasParaCrearOfertas_OK()
        {
            var fabricantes = new List<Fabricante>() {
            new Fabricante("Makita"),
            new Fabricante("Bosch"),
            new Fabricante("Stanley"),
            new Fabricante("Bob")
            };

            var herramientasDTO = new List<HerramientaParaCrearOfertaDTO>()
            {
                new HerramientaParaCrearOfertaDTO(fabricantes[0], "Hierro", "Martillo", 30),    // Makita
                new HerramientaParaCrearOfertaDTO(fabricantes[1], "Acero", "Cuchillo", 20),      // Bosch
                new HerramientaParaCrearOfertaDTO(fabricantes[2], "Acero", "Sierra", 50),   // Stanley
              
            };

            var herramientaDTOsTC1 = new List<HerramientaParaCrearOfertaDTO>() { herramientasDTO[0], herramientasDTO[1], herramientasDTO[2] }
                    //the GetHerramientasParaAlquilar method returns the herramientas ordered by nombre
                    .OrderBy(h => h.Nombre).ToList();


            var herramientaDTOsTC2 = new List<HerramientaParaCrearOfertaDTO>() { herramientasDTO[1] };
            var herramientaDTOsTC3 = new List<HerramientaParaCrearOfertaDTO>() { herramientasDTO[2] };

            var herramientaDTOsTC4 = new List<HerramientaParaCrearOfertaDTO>() { herramientasDTO[1], herramientasDTO[2] };

            var allTests = new List<object[]>
            {             //filters to apply - expected tools
            new object[] { null, null,null, herramientaDTOsTC1 },
            new object[] { "Bosch", null,null, herramientaDTOsTC2 },
            new object[] { null, 50f,null, herramientaDTOsTC3 },
            new object[] { null, null,"Acero", herramientaDTOsTC4 }
            };

            return allTests;

        }

        [Theory]
        [MemberData(nameof(TestCasesFor_GetHerramientasParaCrearOfertas_OK))]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetHerramientasParaCrearOfertas_OK_test(string? fabricante, float? precio, string? materialHerramienta,
            IList<HerramientaParaCrearOfertaDTO> expectedHerramientas)
        {
            // Arrange
            var controller = new HerramientasController(_context, null);

            // Act
            var result = await controller.GetHerramientasParaCrearOfertas(fabricante, precio,materialHerramienta);

            //Assert
            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);

            var herramientasDTOActual = Assert.IsType<List<HerramientaParaCrearOfertaDTO>>(okResult.Value);

            Assert.Equal(expectedHerramientas.Count, herramientasDTOActual.Count);
            for (int i = 0; i < expectedHerramientas.Count; i++)
            {
                Assert.Equal(expectedHerramientas[i].Nombre, herramientasDTOActual[i].Nombre);
                Assert.Equal(expectedHerramientas[i].Material, herramientasDTOActual[i].Material);
                Assert.Equal(expectedHerramientas[i].Precio, herramientasDTOActual[i].Precio);
                Assert.Equal(expectedHerramientas[i].Fabricante.nombre, herramientasDTOActual[i].Fabricante.nombre);
            }

        }
        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        public async Task GetHerramientasParaCrearOfertas_BadRequest_test()
        {
            // Arrange
            var mock = new Mock<ILogger<HerramientasController>>();
            ILogger<HerramientasController> logger = mock.Object;
            var controller = new HerramientasController(_context, logger);

            // Act
            var result = await controller.GetHerramientasParaCrearOfertas("invalid", -10, "invalid");

            // Assert - Como no hay validación, debería devolver Ok con lista vacía
            var okResult = Assert.IsType<OkObjectResult>(result);
            var herramientas = Assert.IsType<List<HerramientaParaCrearOfertaDTO>>(okResult.Value);
            Assert.Empty(herramientas); // No debería encontrar herramientas con esos filtros
        }

    }
}





