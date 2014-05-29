using System;
using System.Web.Http.OData.Builder;

namespace HypemediaWebApi2012.WebApi.ODataSupport
{
    public interface IEntitySetAdder
    {
        void AddEntitySet(ODataModelBuilder modelBuilder, Type entityType);
    }
}