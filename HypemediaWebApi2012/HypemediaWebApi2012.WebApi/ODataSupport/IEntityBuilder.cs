using System;
using System.Linq;
using System.Reflection;
using System.Web.Http.OData.Builder;
using Microsoft.Data.Edm;

namespace HypemediaWebApi2012.WebApi.ODataSupport
{
    /// <summary>
    /// Builds out each of the entities in the model builder
    /// </summary>
    public interface IEntityBuilder
    {
        /// <summary>
        /// Builds out each of the entities in the model builder
        /// </summary>
        /// <param name="modelBuilder">The model builder</param>
        void BuildEntities(ODataModelBuilder modelBuilder);
    }

    // Cleanup
    class EntityBuilder : IEntityBuilder
    {
        private readonly IPropertyCollectionBuilder _propertyCollectionBuilder;

        public EntityBuilder(IPropertyCollectionBuilder propertyCollectionBuilder)
        {
            if (propertyCollectionBuilder == null) throw new ArgumentNullException("propertyCollectionBuilder");
            _propertyCollectionBuilder = propertyCollectionBuilder;
        }

        public void BuildEntities(ODataModelBuilder modelBuilder)
        {
            // Get a list of all the entities
            // Note: This is converted to an array because when complex types are added (non-entities), they will be added
            //       to the StructuralTypes collection which will alter the collection in the middle of the enumeration
            //       which will throw an exception.  .ToArray() creates a copy as an array to iterate over instead.
            var entities = modelBuilder.StructuralTypes.Where(x => x.Kind == EdmTypeKind.Entity).ToArray();

            foreach (var structuralTypeConfiguration in entities)
            {
                _propertyCollectionBuilder.AddPropertyCollection(structuralTypeConfiguration);
            }

        }
    }

    /// <summary>
    /// Adds a property collection to a StructuralTypeConfiguration
    /// </summary>
    public interface IPropertyCollectionBuilder
    {
        void AddPropertyCollection(StructuralTypeConfiguration structuralTypeConfiguration);
    }

    class PropertyCollectionBuilder : IPropertyCollectionBuilder
    {
        private readonly INavigationPropertyBuilder _navigationPropertyBuilder;
        private readonly IComplexPropertyBuilder _complexPropertyBuilder;
        private readonly ICollectionPropertyBuilder _collectionPropertyBuilder;
        private readonly IPrimitivePropertyBuilder _primitivePropertyBuilder;

        public PropertyCollectionBuilder(INavigationPropertyBuilder navigationPropertyBuilder, IComplexPropertyBuilder complexPropertyBuilder,
            ICollectionPropertyBuilder collectionPropertyBuilder, IPrimitivePropertyBuilder primitivePropertyBuilder)
        {
            if (navigationPropertyBuilder == null) throw new ArgumentNullException("navigationPropertyBuilder");
            if (complexPropertyBuilder == null) throw new ArgumentNullException("complexPropertyBuilder");
            if (collectionPropertyBuilder == null) throw new ArgumentNullException("collectionPropertyBuilder");
            if (primitivePropertyBuilder == null) throw new ArgumentNullException("primitivePropertyBuilder");

            _navigationPropertyBuilder = navigationPropertyBuilder;
            _complexPropertyBuilder = complexPropertyBuilder;
            _collectionPropertyBuilder = collectionPropertyBuilder;
            _primitivePropertyBuilder = primitivePropertyBuilder;
        }

        public void AddPropertyCollection(StructuralTypeConfiguration structuralTypeConfiguration)
        {
            var type = structuralTypeConfiguration.ClrType;

            // Select all the properties that are not inherited
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => x.DeclaringType == type);

            foreach (var propertyInfo in properties)
            {
                var propertyAdded =
                    // If it's a navigation property, add it
                    _navigationPropertyBuilder.AddProperty(structuralTypeConfiguration, propertyInfo) |
                    // If it's a complex property, add it
                    _complexPropertyBuilder.AddProperty(structuralTypeConfiguration, propertyInfo, this) |
                    // If it's a collection property, add it
                    _collectionPropertyBuilder.AddProperty(structuralTypeConfiguration, propertyInfo) |
                    // If it's a primitive property, add it
                    _primitivePropertyBuilder.AddProperty(structuralTypeConfiguration, propertyInfo);

                if (propertyAdded == false)
                    throw new ODataModelException(String.Format("Structural type {0} contains an invalid property type: {1}.", structuralTypeConfiguration.Name, propertyInfo.Name));
            }
        }
    }

    class NavigationPropertyBuilder : INavigationPropertyBuilder
    {
        private readonly ITypeHelper _typeHelper;

        public NavigationPropertyBuilder(ITypeHelper typeHelper)
        {
            if (typeHelper == null) throw new ArgumentNullException("typeHelper");
            _typeHelper = typeHelper;
        }

        public bool AddProperty(StructuralTypeConfiguration structuralTypeConfiguration, PropertyInfo propertyInfo)
        {
            // Entity must be existing in order to add navigation property

            if (!IsEntityOrCollectionOfEntities(structuralTypeConfiguration, propertyInfo))
                return false;

            var entityTypeConfiguration = structuralTypeConfiguration as EntityTypeConfiguration;

            // The StructuralTypeConfiguration doesn't represent an entity
            if (entityTypeConfiguration == null)
                return false;

            // Refactor: Adding a NavProperty to a collection or to a single entity are different so this should probably be abstracted
            if (_typeHelper.IsCollection(propertyInfo.PropertyType))
            {
                entityTypeConfiguration.AddNavigationProperty(propertyInfo, EdmMultiplicity.Many);
                return true;
            }

            // Bug: EdmMultiplicity is set to ZeroOrOne because there isn't a good way to determine if there is always one
            entityTypeConfiguration.AddNavigationProperty(propertyInfo, EdmMultiplicity.ZeroOrOne);

            return true;
        }

        // Refactor: This should be abstracted
        private bool IsEntityOrCollectionOfEntities(StructuralTypeConfiguration structuralTypeConfiguration, PropertyInfo propertyInfo)
        {
            // Check if the type exists as an entity
            if (structuralTypeConfiguration.ModelBuilder.StructuralTypes.Any(x => x.ClrType == propertyInfo.PropertyType && x.Kind == EdmTypeKind.Entity))
                return true;

            // Check if its a collection
            if (_typeHelper.IsCollection(propertyInfo.PropertyType))
            {
                // And if it's generic
                if (propertyInfo.PropertyType.IsGenericType)
                {
                    var innerType = propertyInfo.PropertyType.GetGenericArguments()[0];

                    // And it contains entities
                    if (structuralTypeConfiguration.ModelBuilder.StructuralTypes.Any(x => x.ClrType == innerType && x.Kind == EdmTypeKind.Entity))
                        return true;
                }
            }

            return false;
        }
    }

    public interface INavigationPropertyBuilder
    {
        bool AddProperty(StructuralTypeConfiguration structuralTypeConfiguration, PropertyInfo propertyInfo);
    }

    /// <summary>
    /// Exception for failure to build the OData model
    /// </summary>
    public class ODataModelException : Exception
    {
        /// <summary>
        /// Exception for failure to build the OData model
        /// </summary>
        public ODataModelException()
            : base()
        {
        }

        /// <summary>
        /// Exception for failure to build the OData model
        /// </summary>
        /// <param name="message">The message that describes the exception</param>
        public ODataModelException(string message)
            : base(message)
        {
        }
    }

}