using System;
using System.Reflection;

namespace HypemediaWebApi2012.WebApi.ODataSupport
{
    /// <summary>
    /// Gets an assembly
    /// </summary>
    public interface IAssemblyGetter
    {
        /// <summary>
        /// Gets an assembly
        /// </summary>
        /// <param name="typeInAssembly"></param>
        /// <returns></returns>
        Assembly GetAssembly(Type typeInAssembly);
    }
}