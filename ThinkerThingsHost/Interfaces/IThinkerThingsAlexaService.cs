using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ThinkerThingsHost.Interfaces
{
    public interface IThinkerThingsAlexaService
    {
        public IActionResult GetResponse(ControllerBase controllerBase);
    }
}
