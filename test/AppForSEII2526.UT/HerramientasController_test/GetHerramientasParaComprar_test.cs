using AppForSEII2526.API.Controllers;
using AppForSEII2526.API.DTOs.HerramientaDTOs;
using Humanizer.Localisation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppForSEII2526.UT.HerramientasController_test
{
    public class GetHerramientasParaComprar_test: AppForSEII25264SqliteUT
    {
        public GetHerramientasParaComprar_test()
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
            ApplicationUser user = new ApplicationUser(1, "Ricardo", "Escribano Tarraga", "641530789", tiposMetodosPago.TarjetaCredito, "direccion c4", "Ricardo@alu.uclm.es");




            _context.Add(user);
            _context.AddRange(fabricantes);
            _context.AddRange(herramientas);
            _context.SaveChanges();
        }





        public static IEnumerable<object[]> TestCasesFor_GetHerramientasParaComprar_OK()
        {

            var herramientaDTOs = new List<HerramientaParaComprarDTO>() {
                new HerramientaParaComprarDTO(1, "Makita","Hierro","Martillo", 30),
                new HerramientaParaComprarDTO(2, "Stanley","Acero","Sierra", 50),
                new HerramientaParaComprarDTO(3, "Bosch","Acero","Cuchilla", 20),
                new HerramientaParaComprarDTO(4, "Makita","Hierro","Llave", 35)
            };

           
            var herramientaDTOsTC1 = new List<HerramientaParaComprarDTO>() { herramientaDTOs[0] };
            var herramientaDTOsTC2 = new List<HerramientaParaComprarDTO>() { herramientaDTOs[2] };
            var herramientaDTOsTC3 = new List<HerramientaParaComprarDTO>() { herramientaDTOs[2], herramientaDTOs[1] };
            var herramientaDTOsTC4 = new List<HerramientaParaComprarDTO>() { herramientaDTOs[0], herramientaDTOs[1], herramientaDTOs[2], herramientaDTOs[3] }
                //the GetMoviesForPurchase method returns the movies ordered by title
                .OrderBy(h => h.Nombre).ToList();

            var allTests = new List<object[]>
            {             
                new object[] { "Acero", 20f, herramientaDTOsTC2},
                new object[] { "Ace", null, herramientaDTOsTC3},
                new object[] { null, 30f, herramientaDTOsTC1},
                new object[] { null, null, herramientaDTOsTC4}
            };

            return allTests;
        }

        [Theory]
        [MemberData(nameof(TestCasesFor_GetHerramientasParaComprar_OK))]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetHerramientasParaComprar_OK_test(string? material, float? precio,
            IList<HerramientaParaComprarDTO> expectedHerramientas)
        {
            // Arrange
            var controller = new HerramientasController(_context, null);

            // Act
            var result = await controller.GetHerramientasParaComprar(material, precio);

            //Assert
            //we check that the response type is OK 
            var okResult = Assert.IsType<OkObjectResult>(result);
            //and obtain the list of movies
            var herramientaDTOsActual = Assert.IsType<List<HerramientaParaComprarDTO>>(okResult.Value);
            Assert.Equal(expectedHerramientas.Count, herramientaDTOsActual.Count);
            for (int i = 0; i < expectedHerramientas.Count; i++)
            {
                Assert.Equal(expectedHerramientas[i].Material, herramientaDTOsActual[i].Material);
                Assert.Equal(expectedHerramientas[i].Precio, herramientaDTOsActual[i].Precio);
            }


        }


        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        public async Task GetHerramientasParaComprar_badrequest_test()
        {
            // Arrange
            var mock = new Mock<ILogger<HerramientasController>>();
            ILogger<HerramientasController> logger = mock.Object;
            var controller = new HerramientasController(_context, logger);

            // Act
            var result = await controller.GetHerramientasParaComprar("invalid", -10);

            //Assert
            //we check that the response type is OK and obtain the list of movies
            var okResult = Assert.IsType<OkObjectResult>(result);
            var herramientas = Assert.IsType<List<HerramientaParaComprarDTO>>(okResult.Value);
            Assert.Empty(herramientas); // No debería encontrar herramientas con esos filtros
        }
    }
}
