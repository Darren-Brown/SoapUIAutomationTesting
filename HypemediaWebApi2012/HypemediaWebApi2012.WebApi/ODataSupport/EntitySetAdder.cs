using System;
using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;
using System.Web.Http.OData.Builder;
using IQ.Platform.Framework.WebApi.Model;

namespace HypemediaWebApi2012.WebApi.ODataSupport
{
    // Deprecated
    //    class EntitySetAdder : IEntitySetAdder
    //    {
    //        private readonly IEntityAdder _entityAdder;
    //
    //        public EntitySetAdder(IEntityAdder entityAdder)
    //        {
    //            if (entityAdder == null) throw new ArgumentNullException("entityAdder");
    //
    //            _entityAdder = entityAdder;
    //        }
    //
    //        public EntitySetConfiguration AddEntitySet(ODataModelBuilder modelBuilder, Type entityType, string entitySetName)
    //        {
    //            if (modelBuilder.EntitySets.Where(p => p.EntityType.ClrType == entityType).Any())
    //                return modelBuilder.EntitySets.Where(p => p.EntityType.ClrType == entityType).FirstOrDefault();
    //            
    //            // Create the entity for the given type first, then create the entity set
    //            var entityTypeConfiguration = _entityAdder.AddEntity(modelBuilder, entityType);
    //            return modelBuilder.AddEntitySet(entitySetName, entityTypeConfiguration);
    //        }
    //    }

    class EntitySetAdder : IEntitySetAdder
    {
        private readonly IEntitySetNameGetter _entitySetNameGetter;
        private readonly IEntityAdder _entityAdder;

        public EntitySetAdder(IEntitySetNameGetter entitySetNameGetter, IEntityAdder entityAdder)
        {
            if (entitySetNameGetter == null) throw new ArgumentNullException("entitySetNameGetter");
            if (entityAdder == null) throw new ArgumentNullException("entityAdder");

            _entitySetNameGetter = entitySetNameGetter;
            _entityAdder = entityAdder;
        }

        public void AddEntitySet(ODataModelBuilder modelBuilder, Type entityType)
        {
            // Check to make sure the type is IIDentifiable<> otherwise it isn't an entity
            //Deprecated: if (!entityType.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IIdentifiable<>)))
            if (entityType.GetInterface(typeof(IIdentifiable<>).Name) == null)
                return;

            // Add the entity first
            var entity = _entityAdder.AddEntity(modelBuilder, entityType);

            // Add the entity set
            modelBuilder.AddEntitySet(_entitySetNameGetter.GetEntitySetName(entityType.Name), entity);
        }
    }

    // Cleanup
    internal interface IEntitySetNameGetter
    {
        string GetEntitySetName(string entityName);
    }

    class EntitySetNameGetter : IEntitySetNameGetter
    {
        public string GetEntitySetName(string entityName)
        {
            return PluralizationService.CreateService(CultureInfo.CurrentCulture).Pluralize(entityName);
        }
    }

    // Deprecated
    // Cleanup: Move to another file
    //    class EntitySetConventionAdder : IEntitySetAdder
    //    {
    //        public EntitySetConfiguration AddEntitySet(ODataModelBuilder modelBuilder, Type entityType, string entitySetName)
    //        {
    //            // Check for entity set already being added exists in the ODataModelBuilder already
    //
    //            
    //
    //            if (modelBuilder.EntitySets.Any(entitySet => entitySet.EntityType.ClrType == entityType))
    //            {
    //
    //
    //                return modelBuilder.EntitySets.FirstOrDefault(entitySet => entitySet.EntityType.ClrType == entityType);
    //            }
    //        }
    //    }
}