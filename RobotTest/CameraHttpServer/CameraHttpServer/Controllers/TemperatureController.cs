using XiaLM.Owin.source.UserAttributes;

namespace CameraHttpServer.Controllers
{
    public class TemperatureController : ApiController
    {
        [HttpGet]
        public string GetImg()
        {
            return "index";
        }
    }
}
