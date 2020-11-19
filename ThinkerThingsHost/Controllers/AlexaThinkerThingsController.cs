using Microsoft.AspNetCore.Mvc;
using ThinkerThingsHost.Interfaces;

namespace ThinkerThingsHost.Controllers
{
    public class AlexaThinkerThingsController : ControllerBase
    {
        private readonly IThinkerThingsAlexaService _thinkerThingsAlexaService;

        public AlexaThinkerThingsController(IThinkerThingsAlexaService thinkerThingsAlexaService)
        {
            _thinkerThingsAlexaService = thinkerThingsAlexaService;
        }

        [Route("alexa/thinkerthings")]
        [HttpPost]
        public IActionResult ThinkerThings()
        {
            return _thinkerThingsAlexaService.GetResponse(this);
        }

    }

}
