using System.Diagnostics;
using System.Reflection;
using IQ.Platform.Framework.WebApi.Diagnostic;

namespace HypemediaWebApi2012.WebApi.Infrastructure
{
    /// <summary>
    /// A ping resource creator.
    /// </summary>
    class CustomPingResourceFactory : ICreatePingResource
    {
        public PingResource Create()
        {

            var version =
                Assembly.GetExecutingAssembly().GetName().Version.ToString();

            //TODO: provide the API data store version
            Debugger.Break();
            //var dbVersion = 

            return new PingResource(version, "");
        }
    }
}