using System;
using HypemediaWebApi2012.IApiServices.Security;

namespace HypemediaWebApi2012.ApiServices.Security
{
    public class NullUserContextProvider : IProvideUserContext<NullAccessData, NullUserContext>
    {
        public IDisposable CreateContext(NullAccessData access)
        {
            return new NullDisposable();
        }

        public NullUserContext Context
        {
            get { return new NullUserContext(); }
        }

        class NullDisposable : IDisposable
        {
            public void Dispose() { }
        }
    }
}
