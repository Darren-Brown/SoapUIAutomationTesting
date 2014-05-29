using System;
using System.Web.Http.OData.Builder;

namespace HypemediaWebApi2012.WebApi.ODataSupport
{
    public interface IEntityAdder
    {
        EntityTypeConfiguration AddEntity(ODataModelBuilder modelBuilder, Type entityType);
    }
}