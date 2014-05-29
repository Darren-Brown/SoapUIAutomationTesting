using System;
using System.Linq;
using System.Web.Http;
using Castle.Windsor;
using FluentAssertions;
using IQ.Platform.Framework.Common;
using IQ.Platform.Framework.WebApi;
using IQ.Platform.Framework.WebApi.Helpers;
using IQ.Platform.Framework.WebApi.Infrastructure;
using IQ.Platform.Framework.WebApi.Reflection;
using HypemediaWebApi2012.WebApi.Infrastructure;
using HypemediaWebApi2012.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HypemediaWebApi2012.Integration.Test
{
    [TestClass]
    public class ContainerTest
    {

        [TestMethod]
        [TestCategory("IntegrationTest")]
        public void ItShouldResolveGenericApiControllerForEachResourceFound()
        {

            Type[] excludedResources = 
			{
			};

            var config = new HttpConfiguration();

            IWindsorContainer container = new WindsorContainer();
            var sut = new DefaultApiContainer(config, container);
            BootStrapper.Initialize(config, sut, initializeHelpPage: false);

            var resourcesAssembly = typeof(LinkRelations).Assembly;
            var helper = new ResourceRelatedGenericTypesResolver(new TypesHelper(), resourcesAssembly);
            var allResources = ResourceRelatedGenericTypesResolver.GetDefaultResourceTypesSelector(resourcesAssembly)();

            // resolve api controller for each found resource
            allResources
                .Except(excludedResources)
                .Apply(r =>
                {
                    var desc = helper.ResolveResourceTypeInfo(r);
                    var controllerType = helper.MakeGenericTypeForResource(TypesHelper.GenericApiControllerType, desc);

                    try
                    {

                        // using reflection, calls: windsorContainer.Resolve<IGenericApiController<Resource, State, Key>>()
                        CallResolverFor(controllerType, container);

                    }
                    catch (Exception ex)
                    {
                        throw new Exception(string.Format("Cannot resolve api controller for {0} resource.", desc.ResourceType.Name), ex);
                    }

                });

            // the test pass if get here

        }

        static object CallResolverFor(Type typeToResolve, IWindsorContainer container)
        {
            return ReflectionHelper.InvokeGenericMethod<object>(container, "Resolve", new[] { typeToResolve });
        }


        public class TestEntryPointController : ApiController, IEntryPointRequestHandler { }

        [TestMethod]
        public void ItShouldResolveEntryPointControllerSetInConfiguration()
        {

            // arrange
            var config = new HttpConfiguration();
            var sut = new DefaultApiContainer(config, new WindsorContainer());
            BootStrapper.Initialize(config, sut, null, c => c.GetHypermediaConfiguration().EntryPoint.ControllerType = typeof(TestEntryPointController), initializeHelpPage: false);

            // act
            var result = sut.Resolve<IEntryPointRequestHandler>();

            // assert
            result.Should().BeOfType<TestEntryPointController>();

        }

    }
}
