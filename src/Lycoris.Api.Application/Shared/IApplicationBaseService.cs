using Lycoris.Api.Model.Contexts;

namespace Lycoris.Api.Application.Shared
{
    public interface IApplicationBaseService
    {
        Lazy<RequestContext>? RequestContext { get; set; }
    }
}
