using XiaLM.Owin.source.UserAttributes;

namespace AlgorithmHttpSever.Controllers
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
