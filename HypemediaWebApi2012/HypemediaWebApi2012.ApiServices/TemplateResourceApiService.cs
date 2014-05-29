using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using HypemediaWebApi2012.IApiServices;
using HypemediaWebApi2012.Model;
using HypemediaWebApi2012.ApiServices.DomainServices;
using IQ.Platform.Framework.WebApi;

namespace HypemediaWebApi2012.ApiServices
{
    public class TemplateResourceApiService : ITemplateResourceApiService
    {
        private readonly StubTemplateResourceService _repository;

        public TemplateResourceApiService()
        {
            _repository = new StubTemplateResourceService();
        }

        public ResourceCreationResult<TemplateResource, string> Create(TemplateResource resource, IRequestContext context)
        {
            //
            _repository.Add(resource);
            throw new NotImplementedException();
            //return new ResourceCreationResult<TemplateResource, string>();
        }

        public IEnumerable<TemplateResource> GetMany(IRequestContext context)
        {
            var filter = context.GetFilter<TemplateResource>();
            return _repository.GetAll();
            //throw new NotImplementedException();
            //var testCollection = new Collection<TemplateResource>();
            //var testTemplateResource = new TemplateResource();
            //testTemplateResource.Name = "Time for testing";
            //testTemplateResource.Id = "1";
            //testTemplateResource.Description =
            //    "Hardcoded template resource used to test if I get how to use the api service.";
            //testCollection.Add(testTemplateResource);
            //var testTemplateResource2 = new TemplateResource();
            //testTemplateResource2.Name = "Time for testing";
            //testTemplateResource2.Id = "2";
            //testTemplateResource2.Description =
            //    "Hardcoded template resource used to test if I get how to use the api service.";
            //testCollection.Add(testTemplateResource2);
            //return testCollection;

        }

        public TemplateResource Get(string id, IRequestContext context)
        {
            throw new NotImplementedException();
        }

        public void Delete(string id, IRequestContext context)
        {
            throw new NotImplementedException();
        }

        public TemplateResource Update(TemplateResource resource, IRequestContext context)
        {
            throw new NotImplementedException();
        }
    }
}
