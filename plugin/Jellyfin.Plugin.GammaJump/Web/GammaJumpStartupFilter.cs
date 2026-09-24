using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace Jellyfin.Plugin.GammaJump.Web;

/// <summary>
/// Adds Gamma Jump's HTML middleware at the start of the server pipeline.
/// </summary>
public sealed class GammaJumpStartupFilter : IStartupFilter
{
    /// <inheritdoc />
    public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
    {
        return application =>
        {
            application.UseMiddleware<GammaJumpInjectionMiddleware>();
            next(application);
        };
    }
}
