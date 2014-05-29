using System;
using System.Collections.Generic;
using HypemediaWebApi2012.Model;
using IQ.Platform.Framework.WebApi.Reflection;

namespace HypemediaWebApi2012.WebApi.ODataSupport
{
    /// <summary>
    /// Gets an array of resources from an assembly. Resources inherit from HypermediaResource
    /// </summary>
    class ResourceArrayGetter : IResourceArrayGetter
    {
        private readonly IResourceAssemblyGetter _resourceAssemblyGetter;

        public ResourceArrayGetter(IResourceAssemblyGetter resourceAssemblyGetter)
        {
            if (resourceAssemblyGetter == null) throw new ArgumentNullException("resourceAssemblyGetter");

            _resourceAssemblyGetter = resourceAssemblyGetter;
        }

        public IEnumerable<Type> GetResourceArray()
        {
            return ResourceRelatedGenericTypesResolver.GetDefaultResourceTypesSelector(_resourceAssemblyGetter.GetAssembly(typeof(LinkRelations)))();
        }
    }
}