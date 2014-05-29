using System;
using System.Web.Http.OData.Builder;

namespace HypemediaWebApi2012.WebApi.ODataSupport
{
    class EntityAdder : IEntityAdder
    {
        public EntityTypeConfiguration AddEntity(ODataModelBuilder modelBuilder, Type entityType)
        {
            // Add the entity type to the model builder
            return modelBuilder.AddEntity(entityType);

        }
    }
}
