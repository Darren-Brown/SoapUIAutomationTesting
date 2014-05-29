using Microsoft.Data.Edm;

namespace HypemediaWebApi2012.WebApi.ODataSupport
{
    public interface IEntityModelBuilder
    {
        IEdmModel BuildModel();
    }
}