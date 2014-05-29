using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using HypemediaWebApi2012.IApiServices.Security;
using HypemediaWebApi2012.WebApi.Security;


namespace HypemediaWebApi2012.WebApi.Handlers
{

    /// <summary>
    /// Reads the client information from request message.
    /// Used by <see cref="StubAccessTokenRequestAuthenticator"/>
    /// </summary>
    interface IExtractClientFromRequest
    {
        ApiClient ExtractFrom(HttpRequestMessage request);
    }


    /// <summary>
    /// Reads the client information from request header.
    /// </summary>
    class RequestHeaderClientExtractor : IExtractClientFromRequest
    {

        internal const string ClientHeaderName = "X-IQ-Client";

        public ApiClient ExtractFrom(HttpRequestMessage request)
        {
            IEnumerable<string> values;
            if (request.Headers.TryGetValues(ClientHeaderName, out values))
            {
                values = SeparateValuesIfSingle(values).ToArray();
                if (values.Count() >= 2)
                {
                    var arr = values.ToArray();

                    if (!string.IsNullOrWhiteSpace(arr[0]) && !string.IsNullOrWhiteSpace(arr[1]))
                    {
                        return new ApiClient
                        {
                            UserName = arr[0],
                            Environment = arr[1]
                        };
                    }
                }
            }

            return null;
        }

        static IEnumerable<string> SeparateValuesIfSingle(IEnumerable<string> headerValues)
        {
            if (headerValues.Count() == 1)
                return headerValues.First().Split(',', ';');

            return headerValues;
        }
    }
}