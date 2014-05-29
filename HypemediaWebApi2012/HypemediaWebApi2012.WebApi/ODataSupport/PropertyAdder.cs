using System;
using System.Linq;
using System.Reflection;
using System.Web.Http.OData.Builder;
using Microsoft.Data.Edm;

namespace HypemediaWebApi2012.WebApi.ODataSupport
{
    // deprecated
    //    class PropertyAdder : IPropertyAdder
    //    {
    //        private readonly ITypeHelper _typeHelper;
    //
    //        public PropertyAdder(ITypeHelper typeHelper)
    //        {
    //            if (typeHelper == null) throw new ArgumentNullException("typeHelper");
    //
    //            _typeHelper = typeHelper;
    //        }
    //
    //        public Type AddProperty(IStructuralTypeConfiguration typeConfiguration, PropertyInfo property)
    //        {
    //            // Todo: If the property is complex, need to pass PropertyInfo for clrType
    //
    //            //Todo: For testing only, remove this
    //            var clrType = _typeHelper.GetClrType(property.PropertyType);
    //
    //            if (_typeHelper.IsPrimitiveType(property.PropertyType))
    //            {
    //                typeConfiguration.AddProperty(property);
    //                return null;
    //            }
    //            
    //            //if(!property.PropertyType.IsGenericType)
    //                typeConfiguration.AddComplexProperty(property);
    //            
    //            return property.PropertyType;
    //        }
    //    }

    // Cleanup

    // Deprecated
    //    class EntityPropertyAdder : IEntityTypePropertyAdder
    //    {
    //        private readonly IStructuralTypePropertyAdder _structuralTypePropertyAdder;
    //
    //        public EntityPropertyAdder(IStructuralTypePropertyAdder structuralTypePropertyAdder)
    //        {
    //            if (structuralTypePropertyAdder == null) throw new ArgumentNullException("structuralTypePropertyAdder");
    //            _structuralTypePropertyAdder = structuralTypePropertyAdder;
    //        }
    //
    //        public void AddProperty(EntityTypeConfiguration entityConfig, PropertyInfo propertyInfo)
    //        {
    //            // Note: 
    //            // For now, there are no Navigation Properties because the V1 resource models are not entity models and 
    //            // they have Id's relating nested models, not navigation properties so the OData models will be the same.
    //
    //            _structuralTypePropertyAdder.AddProperty(entityConfig, propertyInfo);
    //        }
    //    }

    // Deprecated
    //    class StructrualPropertyAdder : IStructuralTypePropertyAdder
    //    {
    //        private readonly IPrimitivePropertyBuilder _primitivePropertyBuilder;
    //        private readonly IComplexPropertyBuilder _complexPropertyBuilder;
    //        private readonly ICollectionPropertyBuilder _collectionPropertyBuilder;
    //
    //        public StructrualPropertyAdder(IPrimitivePropertyBuilder primitivePropertyBuilder, IComplexPropertyBuilder complexPropertyBuilder, ICollectionPropertyBuilder collectionPropertyBuilder)
    //        {
    //            if (primitivePropertyBuilder == null) throw new ArgumentNullException("primitivePropertyBuilder");
    //            if (complexPropertyBuilder == null) throw new ArgumentNullException("complexPropertyBuilder");
    //            if (collectionPropertyBuilder == null) throw new ArgumentNullException("collectionPropertyBuilder");
    //
    //            _primitivePropertyBuilder = primitivePropertyBuilder;
    //            _complexPropertyBuilder = complexPropertyBuilder;
    //            _collectionPropertyBuilder = collectionPropertyBuilder;
    //        }
    //
    //        public void AddProperty(StructuralTypeConfiguration structuralConfig, PropertyInfo propertyInfo)
    //        {
    //            // Need to determine if the property is a primitive, a complex property or a collection and handle that accordingly
    //            // If it's a collection, the collection needs to be added but so does the type of the collection elements
    //            
    //            _primitivePropertyBuilder.AddProperty(structuralConfig, propertyInfo);
    //            _complexPropertyBuilder.AddProperty(structuralConfig, propertyInfo, this);
    //            _collectionPropertyBuilder.AddProperty(structuralConfig, propertyInfo);
    //        }
    //    }

    // Cleanup

    class PrimitivePropertyBuilder : IPrimitivePropertyBuilder
    {
        private readonly ITypeHelper _typeHelper;

        public PrimitivePropertyBuilder(ITypeHelper typeHelper)
        {
            if (typeHelper == null) throw new ArgumentNullException("typeHelper");
            _typeHelper = typeHelper;
        }

        public bool AddProperty(StructuralTypeConfiguration structuralConfig, PropertyInfo propertyInfo)
        {
            if (!_typeHelper.IsPrimitiveType(propertyInfo.PropertyType))
                return false;

            // Refactor: Not sure if this is best way to determine if this is the key field or not
            // If the property is named 'Id' and the structuralTypeConfiguration is an entity, this is a key, otherwise it's a property
            if (propertyInfo.Name == "Id")
            {
                var entityTypeConfiguration = structuralConfig as EntityTypeConfiguration;

                if (entityTypeConfiguration != null)
                {
                    entityTypeConfiguration.HasKey(propertyInfo);
                    return true;
                }
            }

            structuralConfig.AddProperty(propertyInfo);

            return true;
        }
    }

    class ComplexPropertyBuilder : IComplexPropertyBuilder
    {
        private readonly ITypeHelper _typeHelper;

        public ComplexPropertyBuilder(ITypeHelper typeHelper)
        {
            if (typeHelper == null) throw new ArgumentNullException("typeHelper");
            _typeHelper = typeHelper;
        }

        public bool AddProperty(StructuralTypeConfiguration structuralTypeConfiguration, PropertyInfo propertyInfo, IPropertyCollectionBuilder propertyCollectionBuilder)
        {
            // If the property is an Entity, it is not complex
            if (structuralTypeConfiguration.ModelBuilder.StructuralTypes.Any(x => x.ClrType == propertyInfo.PropertyType && x.Kind == EdmTypeKind.Entity))
                return false;

            // If it's not primitive and not a collection, then it is complex
            if (!_typeHelper.IsComplexType(propertyInfo.PropertyType))
                return false;

            // Check if the ModelBuilder already contains this type as a structural type
            if (!structuralTypeConfiguration.ModelBuilder.StructuralTypes.Any(x => x.ClrType == propertyInfo.PropertyType))
            {
                // Manually add the complex type
                var newComplexType = structuralTypeConfiguration.ModelBuilder.AddComplexType(propertyInfo.PropertyType);

                // Add the properties for the new complex type
                propertyCollectionBuilder.AddPropertyCollection(newComplexType);
            }

            // Add the complex property
            structuralTypeConfiguration.AddComplexProperty(propertyInfo);
            return true;
        }
    }

    class CollectionPropertyBuilder : ICollectionPropertyBuilder
    {
        private readonly ITypeHelper _typeHelper;

        public CollectionPropertyBuilder(ITypeHelper typeHelper)
        {
            if (typeHelper == null) throw new ArgumentNullException("typeHelper");
            _typeHelper = typeHelper;
        }

        public bool AddProperty(StructuralTypeConfiguration structuralConfig, PropertyInfo propertyInfo)
        {
            if (!_typeHelper.IsCollection(propertyInfo.PropertyType))
                return false;

            // If it's a collection of entities, it's a Nav Property, not a CollectionProperty
            if (IsCollectionOfEntities(structuralConfig, propertyInfo))
                return false;

            structuralConfig.AddCollectionProperty(propertyInfo);
            return true;
        }

        // Refactor: This should be abstracted and has some repetition with ODataSupport.NavigationPropertyBuilder.IsEntityOrCollectionOfEntities
        private bool IsCollectionOfEntities(StructuralTypeConfiguration structuralTypeConfiguration, PropertyInfo propertyInfo)
        {
            // Is it generic?
            if (propertyInfo.PropertyType.IsGenericType)
            {
                var innerType = propertyInfo.PropertyType.GetGenericArguments()[0];

                // Is the inner type an entity?
                if (structuralTypeConfiguration.ModelBuilder.StructuralTypes.Any(x => x.ClrType == innerType && x.Kind == EdmTypeKind.Entity))
                    return true;
            }

            return false;
        }
    }
}