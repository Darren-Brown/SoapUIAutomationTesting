using System;
using System.Reflection;

namespace HypemediaWebApi2012.WebApi.ODataSupport
{
    class ResourceAssemblyGetter : IResourceAssemblyGetter
    {
        public Assembly GetAssembly(Type typeInAssembly)
        {
            return typeInAssembly.Assembly;
        }
    }
}