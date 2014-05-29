using System;
using System.Collections.Generic;
using System.Web.Http.OData.Builder;

namespace HypemediaWebApi2012.WebApi.ODataSupport
{
    /// <summary>
    /// Adds a collection of entity sets to the ODataModelBuilder
    /// </summary>
    public interface IEntitySetCollectionAdder
    {
        /// <summary>
        /// Adds a collection of entity sets to the ODataModelBuilder
        /// </summary>
        /// <param name="modelBuilder">The model builder</param>
        /// <param name="entitySetCollection">The collection of types to be added as entity sets</param>
        void AddEntitySetCollection(ODataModelBuilder modelBuilder, IEnumerable<Type> entitySetCollection);
    }
}