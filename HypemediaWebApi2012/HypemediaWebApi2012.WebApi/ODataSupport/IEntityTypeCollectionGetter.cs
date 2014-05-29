using System;
using System.Collections.Generic;

namespace HypemediaWebApi2012.WebApi.ODataSupport
{
    public interface IEntityTypeCollectionGetter
    {
        IEnumerable<Type> GetEntityTypeCollection();
    }
}