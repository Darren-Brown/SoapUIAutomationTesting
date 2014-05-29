using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HypemediaWebApi2012.Model;

namespace HypemediaWebApi2012.ApiServices.DomainServices
{
    public interface ITemplateResourceCreationService
    {
        
    }

    public class StubTemplateResourceService
    {
        private readonly IDictionary<string, TemplateResource> _repository;

        public StubTemplateResourceService()
        {
            _repository = new Dictionary<string, TemplateResource>();

            InitializeSampleResources();
        }

        void InitializeSampleResources()
        {
            var TempResource = new TemplateResource();
            TempResource.Description = "the first resource";
            TempResource.Name = "resource one";
            TempResource.Id = "1";
            this.Add(TempResource);
            //this.Add(new TemplateResource("1", "resource one", "the first resource"));
            //this.Add(new TemplateResource("2", "resource two", "the second resource"));
        }

        public TemplateResource Add(TemplateResource newTemplateResource)
        {
            
            newTemplateResource.Id = _repository.Values.Select(o => o.Id).DefaultIfEmpty("0").Max() + 1;
            _repository.Add(newTemplateResource.Id, newTemplateResource);
            return newTemplateResource;
        }

        public IEnumerable<TemplateResource> GetAll()
        {
            return _repository.Values;
        }

    }

    public class TemplateResourceDTO
    {        
        /// <summary>
        /// Unique Identifier for the TemplateResource
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Name for the TemplateResource
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Description for the TemplateResource
        /// </summary>
        public string Description { get; set; }

        public TemplateResourceDTO(string newId, string newName, string newDescription)
        {
            Id = newId;
            Name = newName;
            Description = newDescription;
        }
    }
}
