//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
namespace Aspire.Hosting
{
    public static partial class KeycloakResourceBuilderExtensions
    {
        public static ApplicationModel.IResourceBuilder<ApplicationModel.KeycloakResource> AddKeycloak(this IDistributedApplicationBuilder builder, string name, int? port = null, ApplicationModel.IResourceBuilder<ApplicationModel.ParameterResource>? adminUsername = null, ApplicationModel.IResourceBuilder<ApplicationModel.ParameterResource>? adminPassword = null) { throw null; }

        public static ApplicationModel.IResourceBuilder<ApplicationModel.KeycloakResource> WithDataBindMount(this ApplicationModel.IResourceBuilder<ApplicationModel.KeycloakResource> builder, string source) { throw null; }

        public static ApplicationModel.IResourceBuilder<ApplicationModel.KeycloakResource> WithDataVolume(this ApplicationModel.IResourceBuilder<ApplicationModel.KeycloakResource> builder, string? name = null) { throw null; }

        public static ApplicationModel.IResourceBuilder<ApplicationModel.KeycloakResource> WithRealmImport(this ApplicationModel.IResourceBuilder<ApplicationModel.KeycloakResource> builder, string import, bool isReadOnly = false) { throw null; }
    }
}

namespace Aspire.Hosting.ApplicationModel
{
    public sealed partial class KeycloakResource : ContainerResource, IResourceWithServiceDiscovery, IResourceWithEndpoints, IResource
    {
        public KeycloakResource(string name, ParameterResource? admin, ParameterResource adminPassword) : base(default!, default) { }

        public ParameterResource AdminPasswordParameter { get { throw null; } }

        public ParameterResource? AdminUserNameParameter { get { throw null; } }
    }
}