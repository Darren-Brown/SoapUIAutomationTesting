using System;
using System.Reflection;

namespace HypemediaWebApi2012.WebApi.ODataSupport
{
    public interface IResourceAssemblyGetter
    {
        Assembly GetAssembly(Type typeInAssembly);
    }
}