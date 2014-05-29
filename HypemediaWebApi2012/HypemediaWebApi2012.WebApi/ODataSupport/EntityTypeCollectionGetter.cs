using System;
using System.Collections.Generic;
using HypemediaWebApi2012.Model;
using IQ.Platform.Framework.WebApi.Reflection;

namespace HypemediaWebApi2012.WebApi.ODataSupport
{
    /// <summary>
    /// Gets an array of resources from an assembly. Resources inherit from HypermediaResource
    /// </summary>
    class EntityTypeCollectionGetter : IEntityTypeCollectionGetter
    {
        private readonly IAssemblyGetter _assemblyGetter;

        public EntityTypeCollectionGetter(IAssemblyGetter assemblyGetter)
        {
            if (assemblyGetter == null) throw new ArgumentNullException("assemblyGetter");

            _assemblyGetter = assemblyGetter;
        }

        public IEnumerable<Type> GetEntityTypeCollection()
        {
            return ResourceRelatedGenericTypesResolver.GetDefaultResourceTypesSelector(_assemblyGetter.GetAssembly(typeof(LinkRelations)))();
        }
    }

    // Deprecated
    // Cleanup
    //    /// <summary>
    //    /// Gets an array of resources from an assembly. Resources inherit from HypermediaResource
    //    /// </summary>
    //    class IgnoreEntityTypeCollectionGetter : IEntityTypeCollectionGetter
    //    {
    //        private readonly IAssemblyGetter _assemblyGetter;
    //        private readonly IEnumerable<Type> _typesToIgnore;
    //
    //        public IgnoreEntityTypeCollectionGetter(IAssemblyGetter assemblyGetter, IEnumerable<Type> typesToIgnore = null)
    //        {
    //            if (assemblyGetter == null) throw new ArgumentNullException("assemblyGetter");
    //
    //            _assemblyGetter = assemblyGetter;
    //            _typesToIgnore = typesToIgnore;
    //        }
    //
    //        public IEnumerable<Type> GetEntityTypeCollection()
    //        {
    //            var result = ResourceRelatedGenericTypesResolver.GetDefaultResourceTypesSelector(_assemblyGetter.GetAssembly(typeof(LinkRelations)))().ToList();
    //
    //            if(_typesToIgnore == null)
    //                return result;
    //
    //            foreach (var type in _typesToIgnore)
    //            {
    //                if (result.Contains(type))
    //                    result.Remove(type);
    //            }
    //
    //            return result;
    //        }
    //    }
}