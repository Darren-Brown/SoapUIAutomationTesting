using System;
using System.Collections.Generic;
using System.Web.Http.OData.Builder;
using Microsoft.Data.Edm;

namespace HypemediaWebApi2012.WebApi.ODataSupport
{
    /// <summary>
    /// Creates an ODataEntityModel using conventions this API uses to create Resources and Specs
    /// </summary>
    class CustomConventionEntityModelBuilder : IEntityModelBuilder
    {
        private readonly IEntityTypeCollectionGetter _entityTypeCollectionGetter;
        private readonly IEntitySetCollectionAdder _entitySetCollectionAdder;
        private readonly IEntityBuilder _entityBuilder;

        public CustomConventionEntityModelBuilder(IEntityTypeCollectionGetter entityTypeCollectionGetter, IEntitySetCollectionAdder entitySetCollectionAdder,
            IEntityBuilder entityBuilder)
        {
            if (entityTypeCollectionGetter == null) throw new ArgumentNullException("entityTypeCollectionGetter");
            if (entitySetCollectionAdder == null) throw new ArgumentNullException("entitySetCollectionAdder");
            if (entityBuilder == null) throw new ArgumentNullException("entityBuilder");

            _entityTypeCollectionGetter = entityTypeCollectionGetter;
            _entitySetCollectionAdder = entitySetCollectionAdder;
            _entityBuilder = entityBuilder;
        }

        public IEdmModel BuildModel()
        {
            var modelBuilder = new ODataModelBuilder();

            var resources = _entityTypeCollectionGetter.GetEntityTypeCollection();

            // Add all the entity collection types to the model builder
            _entitySetCollectionAdder.AddEntitySetCollection(modelBuilder, resources);

            // Build out all the entities
            _entityBuilder.BuildEntities(modelBuilder);

            // Return the model
            return modelBuilder.GetEdmModel();
        }
    }

    // Cleanup
    /// <summary>
    /// Builds out complex types
    /// </summary>
    public interface IComplexTypeBuilder
    {
        /// <summary>
        /// Builds out complex types
        /// </summary>
        /// <param name="oDataModelBuilder">The model builder</param>
        void BuildComplexTypes(ODataModelBuilder oDataModelBuilder);
    }

    class EntitySetCollectionAdder : IEntitySetCollectionAdder
    {
        private readonly IEntitySetAdder _entitySetAdder;

        public EntitySetCollectionAdder(IEntitySetAdder entitySetAdder)
        {
            if (entitySetAdder == null) throw new ArgumentNullException("entitySetAdder");
            _entitySetAdder = entitySetAdder;
        }

        public void AddEntitySetCollection(ODataModelBuilder modelBuilder, IEnumerable<Type> entitySetCollection)
        {
            // Add each of the entitySets to the model builder
            foreach (var entity in entitySetCollection)
            {
                _entitySetAdder.AddEntitySet(modelBuilder, entity);
            }
        }
    }


    // Deprecated
    //    class ImplicitEntityModelBuilder : IEntityModelBuilder
    //    {
    //        private readonly IEntityTypeCollectionGetter _entityTypeCollectionGetter;
    //
    //        public ImplicitEntityModelBuilder(IEntityTypeCollectionGetter entityTypeCollectionGetter)
    //        {
    //            if (entityTypeCollectionGetter == null) throw new ArgumentNullException("entityTypeCollectionGetter");
    //
    //            _entityTypeCollectionGetter = entityTypeCollectionGetter;
    //        }
    //
    //        public IEdmModel BuildModel()
    //        {
    //            ODataModelBuilder modelBuilder = new ODataConventionModelBuilder();
    //
    //            var resources = _entityTypeCollectionGetter.GetEntityTypeCollection();
    //
    //            foreach (var resource in resources)
    //            {
    //                var entityConfiguration = modelBuilder.AddEntity(resource);
    //
    //                var entityName = resource.Name;
    //                var entitySetName = PluralizationService.CreateService(CultureInfo.CurrentCulture).Pluralize(entityName);
    //
    //                //var entitySet = _entitySetAdder.AddEntitySet(modelBuilder, resource, entitySetName);
    //
    //                modelBuilder.AddEntitySet(entitySetName, entityConfiguration);
    //
    //                // Todo: Add Links such as Self/Edit/Navigation Property Links here
    //            }
    //
    //            return modelBuilder.GetEdmModel();
    //        }
    //    }
    //
    //    class NewEntityModelBuilder : IEntityModelBuilder
    //    {
    //        private readonly IEntityTypeCollectionGetter _entityTypeCollectionGetter;
    //        private readonly IEntitySetAdder _entitySetAdder;
    //
    //        public NewEntityModelBuilder(IEntityTypeCollectionGetter entityTypeCollectionGetter, IEntitySetAdder entitySetAdder)
    //        {
    //            if (entityTypeCollectionGetter == null) throw new ArgumentNullException("entityTypeCollectionGetter");
    //            if (entitySetAdder == null) throw new ArgumentNullException("entitySetAdder");
    //
    //            _entityTypeCollectionGetter = entityTypeCollectionGetter;
    //            _entitySetAdder = entitySetAdder;
    //        }
    //
    //        public IEdmModel BuildModel()
    //        {
    //            var modelBuilder = new ODataModelBuilder();
    //
    //
    //            return modelBuilder.GetEdmModel();
    //        }
    //    }
    //
    //
    //    class NaiiveImplicitEntityModelBuilder : IEntityModelBuilder
    //    {
    //        private readonly IEntityTypeCollectionGetter _entityTypeCollectionGetter;
    //
    //        public NaiiveImplicitEntityModelBuilder(IEntityTypeCollectionGetter entityTypeCollectionGetter)
    //        {
    //            if (entityTypeCollectionGetter == null) throw new ArgumentNullException("entityTypeCollectionGetter");
    //
    //            _entityTypeCollectionGetter = entityTypeCollectionGetter;
    //        }
    //
    //        public IEdmModel BuildModel()
    //        {
    //            ODataModelBuilder modelBuilder = new ODataConventionModelBuilder();
    //
    //            var resources = _entityTypeCollectionGetter.GetEntityTypeCollection();
    //
    //            foreach (var resource in resources)
    //            {
    //                var entityConfiguration = modelBuilder.AddEntity(resource);
    //
    //                var entityName = resource.Name;
    //                var entitySetName = PluralizationService.CreateService(CultureInfo.CurrentCulture).Pluralize(entityName);
    //
    //                //var entitySet = _entitySetAdder.AddEntitySet(modelBuilder, resource, entitySetName);
    //
    //                modelBuilder.AddEntitySet(entitySetName, entityConfiguration);
    //
    //                // Todo: Add Links such as Self/Edit/Navigation Property Links here
    //            }
    //
    //            return modelBuilder.GetEdmModel();
    //        }
    //    }
    //
    //    // TestCode
    //    class TestEntity
    //    {
    //        public string Name { get; set; }
    //        public int Value { get; set; }
    //    }
}