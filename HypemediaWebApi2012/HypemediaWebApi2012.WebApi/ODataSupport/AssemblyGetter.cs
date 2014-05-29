using System;
using System.Reflection;

namespace HypemediaWebApi2012.WebApi.ODataSupport
{
    class AssemblyGetter : IAssemblyGetter
    {
        public Assembly GetAssembly(Type typeInAssembly)
        {
            return typeInAssembly.Assembly;
        }
    }
}