using System.Globalization;
using Jellyfin.Plugin.GammaJump.Configuration;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Model.Plugins;
using MediaBrowser.Model.Serialization;
using Jellyfin.Plugin.GammaJump.Web;

namespace Jellyfin.Plugin.GammaJump;

/// <summary>
/// Jellyfin entry point for the Gamma Jump Web enhancement.
/// </summary>
public sealed class Plugin : BasePlugin<PluginConfiguration>, IHasWebPages
{
    /// <summary>
    /// The stable plugin identifier.
    /// </summary>
    public static readonly Guid PluginId = Guid.Parse("4dd1ed79-9e5e-441e-8ca3-8f1b29601a48");

    /// <summary>
    /// Gets the active plugin instance.
    /// </summary>
    public static Plugin? Instance { get; private set; }

    /// <summary>Gets this process's browser-payload identity.</summary>
    public IGammaJumpRuntimeInfo RuntimeInfo { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Plugin"/> class.
    /// </summary>
    public Plugin(IApplicationPaths applicationPaths, IXmlSerializer xmlSerializer)
        : base(applicationPaths, xmlSerializer)
    {
        Instance = this;
        using var script = GetType().Assembly.GetManifestResourceStream("Jellyfin.Plugin.GammaJump.Resources.gamma-jump.js")
            ?? throw new InvalidOperationException("The embedded Gamma Jump browser script is missing.");
        RuntimeInfo = GammaJumpRuntimeInfo.Create(script, GetType().Assembly.GetName().Version);
    }

    /// <inheritdoc />
    public override string Name => "Gamma Jump";

    /// <inheritdoc />
    public override Guid Id => PluginId;

    /// <inheritdoc />
    public IEnumerable<PluginPageInfo> GetPages()
    {
        return
        [
            new PluginPageInfo
            {
                Name = Name,
                EmbeddedResourcePath = string.Format(
                    CultureInfo.InvariantCulture,
                    "{0}.Configuration.configPage.html",
                    GetType().Namespace)
            }
        ];
    }
}
