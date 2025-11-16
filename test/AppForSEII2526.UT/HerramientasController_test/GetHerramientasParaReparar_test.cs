using AppForSEII2526.API.Controllers;
using AppForSEII2526.API.DTOs.HerramientaDTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppForSEII2526.UT.HerramientasController_test
{
    public class GetHerramientasParaReparar_test : AppForSEII25264SqliteUT

    {
        public GetHerramientasParaReparar_test()
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


            var reparacion = new Reparacion(new List<ReparacionItem>(), DateTime.Today.AddDays(4), DateTime.Now, 30.0f, tiposMetodoPago.TarjetaCredito, user);



            reparacion.ReparacionItems.Add(new ReparacionItem(herramientas[0], reparacion));

            _context.Add(user);
            _context.AddRange(fabricantes);
            _context.AddRange(herramientas);
            _context.Add(reparacion);
            _context.SaveChanges();
        }


        public static IEnumerable<object[]> TestCasesFor_GetHerramientasParaReparar_OK()
        {

            var herramientaDTOs = new List<HerramientaParaRepararDTO>() {
                new HerramientaParaRepararDTO(1,"Makita","Hierro","Martillo" ,30, 3),
                new HerramientaParaRepararDTO(2,"Stanley","Acero","Sierra", 50, 7),
                new HerramientaParaRepararDTO(3,"Bosch","Acero","Cuchilla" ,20, 2),
                new HerramientaParaRepararDTO(4,"Makita","Hierro","Llave",35, 4)
            };

            
            var herramientaDTOsTC1 = new List<HerramientaParaRepararDTO>() { herramientaDTOs[0] };
            var herramientaDTOsTC2 = new List<HerramientaParaRepararDTO>() { herramientaDTOs[2] };

            var herramientaDTOsTC3 = new List<HerramientaParaRepararDTO>() { herramientaDTOs[0], herramientaDTOs[1], herramientaDTOs[2], herramientaDTOs[3] }
               .OrderBy(h => h.Nombre).ToList();

            var allTests = new List<object[]>
            {
                new object[] { "Martillo", 4, herramientaDTOsTC1 },
                new object[] { "Marti", null, herramientaDTOsTC1 },
                new object[] { null, 2, herramientaDTOsTC2 },
                new object[] { null, null, herramientaDTOsTC3 },
            };

            return allTests;
        }

        [Theory]
        [MemberData(nameof(TestCasesFor_GetHerramientasParaReparar_OK))]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetHerramientasParaReparar_OK_test(string? nombre, int? tiempoReparacion,
            IList<HerramientaParaRepararDTO> expectedHerramientas)
        {
            // Arrange
            var controller = new HerramientasController(_context, null);

            // Act
            var result = await controller.GetHerramientasParaReparar(nombre, tiempoReparacion);

            //Assert
            //we check that the response type is OK 
            var okResult = Assert.IsType<OkObjectResult>(result);
            //and obtain the list of herramientas
            var herramientaDTOsActual = Assert.IsType<List<HerramientaParaRepararDTO>>(okResult.Value);

            Assert.Equal(expectedHerramientas.Count, herramientaDTOsActual.Count);
            for (int i = 0; i < expectedHerramientas.Count; i++)
            {
                Assert.Equal(expectedHerramientas[i].Nombre, herramientaDTOsActual[i].Nombre);
                Assert.Equal(expectedHerramientas[i].Material, herramientaDTOsActual[i].Material);
                Assert.Equal(expectedHerramientas[i].Precio, herramientaDTOsActual[i].Precio);
                Assert.Equal(expectedHerramientas[i].TiempoReparacion, herramientaDTOsActual[i].TiempoReparacion);
            }

        }


        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        public async Task GetHerramientasParaReparar_badrequest_test()
        {
            // Arrange
            var mock = new Mock<ILogger<HerramientasController>>();
            ILogger<HerramientasController> logger = mock.Object;
            var controller = new HerramientasController(_context, logger);

            // Act
            var result = await controller.GetHerramientasParaReparar("invalid", -10);

            // Assert - Como no hay validación, debería devolver Ok con lista vacía
            var okResult = Assert.IsType<OkObjectResult>(result);
            var herramientas = Assert.IsType<List<HerramientaParaRepararDTO>>(okResult.Value);
            Assert.Empty(herramientas); // No debería encontrar herramientas con esos filtros
        }
    }
}
