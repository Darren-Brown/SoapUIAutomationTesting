using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using HypemediaWebApi2012.IApiServices.Security;

namespace HypemediaWebApi2012.WebApi.Handlers
{
    //TODO: convert to type constructor with TAccessData and TUserContext
    public class ApiSecurityContextProvidingHandler : DelegatingHandler
    {

        readonly IProvideUserContext<NullAccessData, NullUserContext> _contextProvider;
        internal ApiSecurityContextProvidingHandler(IProvideUserContext<NullAccessData, NullUserContext> contextProvider)
        {
            if (contextProvider == null)
                throw new ArgumentNullException("contextProvider");
            _contextProvider = contextProvider;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var accessInfo = GetAccessInfo(request);
            IDisposable context = null;

            try
            {
                context = _contextProvider.CreateContext(accessInfo);
            }
            catch (AuthenticationFailedException ex)
            {
                return Task.Factory.StartNew(() => request.CreateErrorResponse(HttpStatusCode.Unauthorized, ex));
            }

            return base.SendAsync(request, cancellationToken).ContinueWith(t =>
            {
                if (context != null)
                    context.Dispose();
                return t;
            }).Unwrap();
        }

        static NullAccessData GetAccessInfo(HttpRequestMessage request)
        {
            object value;
            if (request.Properties.TryGetValue(RequestMessagePropertyKeys.Security.AuthorizationInfoKey, out value) &&
            value is NullAccessData)
                return (NullAccessData)value;

            throw new Exception("Cannot find authorization key in request properties.");
        }
    }

}
