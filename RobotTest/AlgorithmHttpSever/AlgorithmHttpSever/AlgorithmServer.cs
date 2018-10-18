using System;
using System.Collections.Generic;
using XiaLM.Owin.source;
using XiaLM.Owin.source.FileSever;

namespace AlgorithmHttpSever
{
    public class AlgorithmSever
    {
        private OwinHelper owin;
        public void Start()
        {
            List<HttpRoute> routes = new List<HttpRoute>();
            routes.Add(new HttpRoute() { RouteTemplate = @"{controller}/{action}/{id}" });
            owin = new OwinHelper(@"https://127.0.0.1:11508", new OwinStart()
            {
                IsOpenSignalR = true,
                IsCorss = true,
                FileServerOptions = null,
                Routes = routes,
                ResultToJson = true,
                IsOpenWebApi = true
            });
            owin.Start();
            Console.WriteLine("CameraHttpServer已启动!");
        }
    }
}
