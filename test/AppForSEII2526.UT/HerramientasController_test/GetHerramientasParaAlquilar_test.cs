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
                new Herramienta(fabricantes[0],"Hierro","Martillo" ,3,30),
                new Herramienta(fabricantes[2],"Acero","Sierra", 7,50),
                new Herramienta(fabricantes[1],"Acero","Cuchilla" ,2,20),
                new Herramienta(fabricantes[0],"Hierro","Llave" , 4,35)
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
            };

            var herramientaDTOsTC1 = new List<HerramientaParaAlquilarDTO>() { herramientaDTOs[1], herramientaDTOs[2] }
                    //the GetHerramientasParaAlquilar method returns the herramientas ordered by nombre
                    .OrderBy(h => h.Nombre).ToList();


            var herramientaDTOsTC2 = new List<HerramientaParaAlquilarDTO>() { herramientaDTOs[1] };
            var herramientaDTOsTC3 = new List<HerramientaParaAlquilarDTO>() { herramientaDTOs[2] };

            var herramientaDTOsTC4 = new List<HerramientaParaAlquilarDTO>() { herramientaDTOs[0], herramientaDTOs[1], herramientaDTOs[2] }
                //the GetHerramientasParaAlquilar method returns the herramientas ordered by nombre
                .OrderBy(h => h.Nombre).ToList();

            var allTests = new List<object[]>
            {             //filters to apply - expected herramientas
                                          //by default datefrom=today +1, dateto=today+2, thus herramientaDTOs[0] cannot be returned
                new object[] { null, null,  herramientaDTOsTC1,  },
                new object[] { "Mar", null, herramientaDTOsTC2, },
                new object[] { null, "Hierro", herramientaDTOsTC3, },
            };

            return allTests;
        }

        [Theory]
        [MemberData(nameof(TestCasesFor_GetHerramientasParaAlquilar_OK))]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetHerramientasParaAlquilar_OK_test(string? filterNombre, string? filterMaterial,
            IList<HerramientaParaAlquilarDTO> expectedHerramientas)
        {
            // Arrange
            var controller = new HerramientasController(_context, null);

            // Act
            var result = await controller.GetHerramientasParaAlquilar(filterNombre, filterMaterial);

            //Assert
            //we check that the response type is OK 
            var okResult = Assert.IsType<OkObjectResult>(result);
            //and obtain the list of herramientas
            var herramientaDTOsActual = Assert.IsType<List<HerramientaParaAlquilarDTO>>(okResult.Value);
            Assert.Equal(expectedHerramientas, herramientaDTOsActual);

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
            var result = await controller.GetHerramientasParaAlquilar(null, null);

            //Assert
            //we check that the response type is OK and obtain the list of movies
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var problemDetails = Assert.IsType<ValidationProblemDetails>(badRequestResult.Value);
            var problem = problemDetails.Errors.First().Value[0];

            Assert.Equal("fromDate must be earlier than toDate", problem);
        }

    }
}
