using Jellyfin.Plugin.GammaJump.Configuration;
using Jellyfin.Plugin.GammaJump.Web;
using MediaBrowser.Controller;
using MediaBrowser.Controller.Plugins;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Jellyfin.Plugin.GammaJump;

/// <summary>
/// Registers the stock ASP.NET Core startup filter that places the narrowly
/// scoped Web-index middleware before Jellyfin's static-file middleware.
/// </summary>
public sealed class PluginServiceRegistrator : IPluginServiceRegistrator
{
    /// <inheritdoc />
    public void RegisterServices(IServiceCollection serviceCollection, IServerApplicationHost applicationHost)
    {
        serviceCollection.AddSingleton<IGammaJumpConfigurationService, GammaJumpConfigurationService>();
        serviceCollection.AddSingleton<IGammaJumpRuntimeInfo>(_ => Plugin.Instance?.RuntimeInfo
            ?? throw new InvalidOperationException("Gamma Jump runtime metadata is not available."));
        serviceCollection.AddTransient<IStartupFilter, GammaJumpStartupFilter>();
    }
}
