using Microsoft.AspNetCore.Mvc;

namespace ThinkerThingsHost.Interfaces
{
    public interface IThinkerThingsAlexaService
    {
        public IActionResult GetResponse(ControllerBase controllerBase);
    }
}
