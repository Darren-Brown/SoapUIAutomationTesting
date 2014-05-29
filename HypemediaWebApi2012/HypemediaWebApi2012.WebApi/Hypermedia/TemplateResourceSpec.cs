using HypemediaWebApi2012.Model;
using IQ.Platform.Framework.WebApi.Hypermedia;
using IQ.Platform.Framework.WebApi.Hypermedia.Specs;
using IQ.Platform.Framework.WebApi.Model.Hypermedia;

namespace HypemediaWebApi2012.WebApi.Hypermedia
{
    public class TemplateResourceSpec : SingleStateResourceSpec<TemplateResource, string>
    {

        public static ResourceUriTemplate Uri = ResourceUriTemplate.Create("TemplateResources({id})");

        public override string EntrypointRelation
        {
            get { return LinkRelations.TemplateResource; }
        }


        // IResourceStateSpec is not required but can be overridden to define custom operations and links.
        // See example below...
        //
        //public override IResourceStateSpec<TemplateResource, NullState, string> StateSpec
        //{
        //    get
        //    {
        //        return
        //            new SingleStateSpec<TemplateResource, string>
        //            {
        //                Links =
        //                {
        //                    CreateLinkTemplate(LinkRelations.TemplateResource2, OrganizationSpec2.Uri, r => r.Id),
        //                },

        //                Operations = new StateSpecOperationsSource<TemplateResource, string>
        //                {
        //                    Get = ServiceOperations.Get,
        //                    InitialPost = ServiceOperations.Create,
        //                    Post = ServiceOperations.Update,
        //                    Delete = ServiceOperations.Delete,
        //                },
        //            };
        //    }
        //}

    }
}