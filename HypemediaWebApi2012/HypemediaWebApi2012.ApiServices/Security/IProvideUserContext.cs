using System;

namespace HypemediaWebApi2012.IApiServices.Security
{
    /// <summary>
    /// Create a user context based on access data read from request headers
    /// </summary>
    public interface IProvideUserContext<in TAccessData, out TUserContext>
    {
        IDisposable CreateContext(TAccessData access);
        TUserContext Context { get; }
    }


    /// <summary>
    /// Stores authentication/authorization data read from request headers (e.g. Authorization header)
    /// </summary>
    public class NullAccessData { }

    public class NullUserContext { }



}
