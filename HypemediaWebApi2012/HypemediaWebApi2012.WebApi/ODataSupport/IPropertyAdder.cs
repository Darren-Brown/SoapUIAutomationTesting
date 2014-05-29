using System.Reflection;
using System.Web.Http.OData.Builder;

namespace HypemediaWebApi2012.WebApi.ODataSupport
{
    // Deprecated
    //    public interface IPropertyAdder
    //    {
    //        Type AddProperty(IStructuralTypeConfiguration entity, PropertyInfo property);
    //    }

    // Cleanup
    // Deprecated
    //    public interface IEntityTypePropertyAdder
    //    {
    //        void AddProperty(EntityTypeConfiguration entityConfig, PropertyInfo propertyInfo);
    //    }

    // Deprecated
    //    public interface IStructuralTypePropertyAdder
    //    {
    //        void AddProperty(StructuralTypeConfiguration structuralConfig, PropertyInfo propertyInfo);
    //    }

    // Cleanup
    /// <summary>
    /// Adds a primitive property to the TypeConfiguration
    /// </summary>
    public interface IPrimitivePropertyBuilder
    {
        bool AddProperty(StructuralTypeConfiguration structuralConfig, PropertyInfo propertyInfo);
    }

    public interface IComplexPropertyBuilder
    {
        bool AddProperty(StructuralTypeConfiguration structuralTypeConfiguration, PropertyInfo propertyInfo, IPropertyCollectionBuilder propertyCollectionBuilder);
    }

    public interface ICollectionPropertyBuilder
    {
        bool AddProperty(StructuralTypeConfiguration structuralConfig, PropertyInfo propertyInfo);
    }
}