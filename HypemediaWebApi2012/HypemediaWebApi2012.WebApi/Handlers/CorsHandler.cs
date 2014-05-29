using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace HypemediaWebApi2012.WebApi.Handlers
{

    /// <summary>
    /// The header adds CORS standard support. See http://www.w3.org/TR/cors/ for more information.
    /// </summary>
    class CorsHandler : DelegatingHandler
    {
        static IProvideCORSDomains _corsDomainProvider;
        const string AccessControlAllowOriginHeaderName = "Access-Control-Allow-Origin";
        const string AccessControlAllowMethodsHeaderName = "Access-Control-Allow-Methods";
        static readonly string[] AllowedVerbs = { "GET", "OPTIONS", "HEAD", "POST", "PUT", "DELETE", "TRACE", "CONNECT" };

        public CorsHandler(IProvideCORSDomains corsDomainProvider)
        {
            if (corsDomainProvider == null) throw new ArgumentNullException("corsDomainProvider");
            _corsDomainProvider = corsDomainProvider;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            var responseTask = request.Method == HttpMethod.Options
                                    ? Task.Factory.StartNew(() => request.CreateResponse(HttpStatusCode.OK))
                                    : base.SendAsync(request, cancellationToken);

            return responseTask
                .ContinueWith(t => AddAccessControllHeadersIfSaveVerb(t.Result));
        }


        static HttpResponseMessage AddAccessControllHeadersIfSaveVerb(HttpResponseMessage response)
        {
            if (IsFromSaveRequest(response))
            {
                var allowedDomains = _corsDomainProvider.ReadDomains();
                response.Headers.Add(AccessControlAllowOriginHeaderName, allowedDomains);
                response.Headers.Add(AccessControlAllowMethodsHeaderName, AllowedVerbs);
            }

            return response;
        }

        static bool IsFromSaveRequest(HttpResponseMessage response)
        {
            return response.RequestMessage.Method == HttpMethod.Get || response.RequestMessage.Method == HttpMethod.Options;
        }
    }


    public class CORSDomainsFromConfigurationProvider : IProvideCORSDomains
    {
        public IEnumerable<string> ReadDomains()
        {
            throw new NotImplementedException();
        }
    }

    interface IProvideCORSDomains
    {
        IEnumerable<string> ReadDomains();
    }

    class AllAllowedCORSDomainProvider : IProvideCORSDomains
    {
        const string AccessControlAllowOriginAllowAll = "*";

        public IEnumerable<string> ReadDomains()
        {
            yield return AccessControlAllowOriginAllowAll;
        }
    }
}