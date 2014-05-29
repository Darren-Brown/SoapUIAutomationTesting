using System.Web.Http;
using HypemediaWebApi2012.WebApi.Infrastructure;

namespace HypemediaWebApi2012.WebApi
{

    public class HttpServerFactory
    {
        public HttpServer Create()
        {
            return new HttpServer(GetHttpConfiguration());
        }

        private static HttpConfiguration GetHttpConfiguration()
        {
            var config = new HttpConfiguration();
            BootStrapper.Initialize(config);
            return config;
        }
    }
}