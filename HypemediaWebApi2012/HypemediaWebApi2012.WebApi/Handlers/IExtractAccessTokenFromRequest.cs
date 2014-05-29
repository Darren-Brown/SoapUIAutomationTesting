using System.Net.Http;
using System.Net.Http.Headers;

namespace HypemediaWebApi2012.WebApi.Handlers
{

    /// <summary>
    /// Reads the access token from request message.
    /// Used by <see cref="AuthenticationMessageHandler"/>
    /// </summary>
    interface IExtractAccessTokenFromRequest
    {
        string ExtractFrom(HttpRequestMessage request);
    }


    /// <summary>
    /// Reads the access token from request's Authorization header.
    /// Header example: 
    ///		Authorization: IQtemp 3A4DF1198F044D1591292DA20D2767BE3A4DF1198F044D1591292DA20D2767BE
    /// </summary>
    class AccessTokenRequestHeaderExtractor : IExtractAccessTokenFromRequest
    {
        // see: http://tools.ietf.org/html/rfc5849#section-3.5.1
        const string AuthenticationScheme = "IQtemp";

        public string ExtractFrom(HttpRequestMessage request)
        {
            var authHeader = request.Headers.Authorization;
            if (IsHeaderValid(authHeader))
                return authHeader.Parameter;

            return null;
        }

        static bool IsHeaderValid(AuthenticationHeaderValue authHeader)
        {
            return
                authHeader != null &&
                authHeader.Scheme.Equals(AuthenticationScheme, System.StringComparison.InvariantCultureIgnoreCase) &&
                !string.IsNullOrWhiteSpace(authHeader.Parameter);
        }
    }
}