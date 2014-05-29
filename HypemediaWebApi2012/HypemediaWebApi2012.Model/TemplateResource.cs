using IQ.Platform.Framework.WebApi.Model;
using IQ.Platform.Framework.WebApi.Model.Hypermedia;

namespace HypemediaWebApi2012.Model
{
    /// <summary>
    /// A Template Resource, used as a placeholder. To be removed after real-world API resources have been added.
    /// </summary>
    public class TemplateResource : HypermediaResource, IStatelessResource, IIdentifiable<string>
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
    }
}
