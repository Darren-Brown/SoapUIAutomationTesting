using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using IQ.Platform.Framework.WebApi.Handlers;
using HypemediaWebApi2012.ApiServices;
using HypemediaWebApi2012.ApiServices.Security;
using HypemediaWebApi2012.Documentation.Installers;
using HypemediaWebApi2012.WebApi.Handlers;
using HypemediaWebApi2012.WebApi.Hypermedia;
using HypemediaWebApi2012.IApiServices;
using HypemediaWebApi2012.WebApi.Infrastructure.Installers;
using HypemediaWebApi2012.Model;
using IQ.Platform.Framework.WebApi.Infrastructure;
using HypemediaWebApi2012.WebApi.Security;

namespace HypemediaWebApi2012.WebApi.Infrastructure
{

    public class DefaultApiContainer : ApiContainer
    {

        readonly IDomainServiceResolver _domainServiceResolver;
        readonly Assembly _apiDomainServicesAssembly = typeof(ITemplateResourceApiService).Assembly;
        readonly Assembly _resourceMappersAssembly = typeof(TemplateResourceApiService).Assembly;


        public DefaultApiContainer(HttpConfiguration configuration, IWindsorContainer windsorContainer, IDomainServiceResolver domainServiceResolver = null)
            : base(configuration, windsorContainer)
        {
            _domainServiceResolver = domainServiceResolver;
        }

        public override Assembly ResourceAssembly { get { return typeof(LinkRelations).Assembly; } }
        protected override Assembly ResourceSpecsAssembly { get { return typeof(TemplateResourceSpec).Assembly; } }
        protected override Assembly ResourceStateProvidersAssembly { get { return typeof(TemplateResource).Assembly; } }
        protected override Assembly ApiAppServicesAssembly { get { return typeof(TemplateResourceApiService).Assembly; } }


        protected override void RegisterCustomDependencies()
        {

            _windsorContainer
                .Install(new IWindsorInstaller[]
				         {
					         new DomainServicesInstaller(_domainServiceResolver, _apiDomainServicesAssembly, _apiDomainServicesAssembly),
					         new ResourceMapperInstaller(_resourceMappersAssembly),
							 new HelpControllerInstaller(), //TODO: read from assembly containing Help page views
				         });

        }

        protected override IEnumerable<DelegatingHandler> ResolveMessageHandlersInternal()
        {

            yield return ResolveCorsMessageHandler();
            yield return ResolveAuthenticationMessageHandler();
            yield return ResolveSecurityContextMessageHandler();

        }

        static CorsMessageHandler ResolveCorsMessageHandler()
        {
            return new CorsMessageHandler();
        }

        static DelegatingHandler ResolveAuthenticationMessageHandler()
        {

            return new AuthenticationMessageHandler(
                new AlwaysAuthenticateRequestAuthenticator());

        }

        public virtual DelegatingHandler ResolveSecurityContextMessageHandler(HypermediaConfiguration hConfiguration)
        {

            return new ApiSecurityContextProvidingHandler(
                            new NullUserContextProvider());

        }

        static DelegatingHandler ResolveSecurityContextMessageHandler()
        {

            return new ApiSecurityContextProvidingHandler(
                new NullUserContextProvider());

        }



    }
}
