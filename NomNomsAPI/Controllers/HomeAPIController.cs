using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NomNomsAPI.Models;

namespace NomNomsAPI.Controllers
{
    // Base URL -> http://localhost:5018/api/HomeAPI
    [Route("api/[controller]")]
    [ApiController]
    public class HomeAPIController : ControllerBase
    {
        NomNomDBContext nomNomDBAccessor;
        public HomeAPIController(NomNomDBContext nomNomDBContext) => nomNomDBAccessor = nomNomDBContext;

        // Not sure if this is gonna be needed

    }
}
