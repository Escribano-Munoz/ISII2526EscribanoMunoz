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
    public class GetHerramientasParaAlquilar_test : AppForSEII25264SqliteUT
    {
        public GetHerramientasParaAlquilar_test()
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
            ApplicationUser user = new ApplicationUser(1, "Juan José", "Escribano Tarraga",  "640502222", tiposMetodosPago.TarjetaCredito,"Calle Carretera de Valencia", "juanjose.escribano@alu.uclm.es");


            var alquiler = new Alquiler(user, 15.0, DateTime.Now, DateTime.Today.AddDays(2), DateTime.Today.AddDays(5), "Calle Carretera de Valencia", TiposMetodoPago.TarjetaCredito, "Juan José", "Escribano Tarraga", new List<AlquilarItem>());



            alquiler.AlquilarItems.Add(new AlquilarItem(herramientas[0], alquiler));

            _context.Add(user);
            _context.AddRange(fabricantes);
            _context.AddRange(herramientas);
            _context.Add(alquiler); //
            _context.SaveChanges();
        }


        public static IEnumerable<object[]> TestCasesFor_GetHerramientasParaAlquilar_OK()
        {

            var herramientaDTOs = new List<HerramientaParaAlquilarDTO>() {
                new HerramientaParaAlquilarDTO(1,"Makita","Hierro","Martillo" ,30),
                new HerramientaParaAlquilarDTO(2,"Stanley","Acero","Sierra", 50),
                new HerramientaParaAlquilarDTO(3,"Bosch","Acero","Cuchilla" ,20 ),
                new HerramientaParaAlquilarDTO(4,"Makita","Hierro","Llave" ,35 )
            };

            var herramientaDTOsTC1 = new List<HerramientaParaAlquilarDTO>() { herramientaDTOs[0] };
                    //the GetHerramientasParaAlquilar method returns the herramientas ordered by nombre

            var herramientaDTOsTC2 = new List<HerramientaParaAlquilarDTO>() { herramientaDTOs[2], herramientaDTOs[1] };
            var herramientaDTOsTC3 = new List<HerramientaParaAlquilarDTO>() { herramientaDTOs[1] };

            var herramientaDTOsTC4 = new List<HerramientaParaAlquilarDTO>() { herramientaDTOs[0], herramientaDTOs[1], herramientaDTOs[2], herramientaDTOs[3] }
                //the GetHerramientasParaAlquilar method returns the herramientas ordered by nombre
                .OrderBy(h => h.Nombre).ToList();

            var allTests = new List<object[]>
            {             //filters to apply - expected herramientas
                                          
                new object[] { null, null,  herramientaDTOsTC4},
                new object[] { "Mar", null, herramientaDTOsTC1},
                new object[] { null, "Acer", herramientaDTOsTC2},
                new object[] { "Sierra", "Acero", herramientaDTOsTC3},

            };

            return allTests;
        }

        [Theory]
        [MemberData(nameof(TestCasesFor_GetHerramientasParaAlquilar_OK))]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetHerramientasParaAlquilar_OK_test(string? nombre, string? material,
            IList<HerramientaParaAlquilarDTO> expectedHerramientas)
        {
            // Arrange
            var controller = new HerramientasController(_context, null);

            // Act
            var result = await controller.GetHerramientasParaAlquilar(nombre, material);

            //Assert
            //we check that the response type is OK 
            var okResult = Assert.IsType<OkObjectResult>(result);
            //and obtain the list of herramientas
            var herramientaDTOsActual = Assert.IsType<List<HerramientaParaAlquilarDTO>>(okResult.Value);
            Assert.Equal(expectedHerramientas.Count, herramientaDTOsActual.Count);
            for (int i = 0; i < expectedHerramientas.Count; i++)
            {
                Assert.Equal(expectedHerramientas[i].Nombre, herramientaDTOsActual[i].Nombre);
                Assert.Equal(expectedHerramientas[i].Material, herramientaDTOsActual[i].Material);
                

            }
        }


        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        public async Task GetHerramientasParaAlquilar_badrequest_test()
        {
            // Arrange
            var mock = new Mock<ILogger<HerramientasController>>();
            ILogger<HerramientasController> logger = mock.Object;
            var controller = new HerramientasController(_context, logger);

            // Act
            var result = await controller.GetHerramientasParaAlquilar("invalid", "invalid");

            // Assert - Como no hay validación, debería devolver Ok con lista vacía
            var okResult = Assert.IsType<OkObjectResult>(result);
            var herramientas = Assert.IsType<List<HerramientaParaAlquilarDTO>>(okResult.Value);
            Assert.Empty(herramientas); // No debería encontrar herramientas con esos filtros
        }

    }
}
