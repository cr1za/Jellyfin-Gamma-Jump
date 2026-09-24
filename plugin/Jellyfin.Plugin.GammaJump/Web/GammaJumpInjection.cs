using System.Net;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Http;

namespace Jellyfin.Plugin.GammaJump.Web;

/// <summary>
/// Builds and inserts the fixed plugin bootstrap markup. Keeping this operation
/// pure makes duplicate-injection and base-path behavior testable without a
/// running Jellyfin server.
/// </summary>
public static class GammaJumpInjection
{
    private const string MarkerId = "gamma-jump-plugin-bootstrap";

    /// <summary>
    /// Tries to append the bootstrap marker and embedded-script URL to HTML.
    /// </summary>
    public static bool TryInject(string html, PathString pathBase, IGammaJumpRuntimeInfo runtimeInfo, out string transformed)
    {
        ArgumentNullException.ThrowIfNull(html);
        ArgumentNullException.ThrowIfNull(runtimeInfo);
        transformed = html;

        if (html.Contains($"id=\"{MarkerId}\"", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var closingHead = html.LastIndexOf("</head>", StringComparison.OrdinalIgnoreCase);
        if (closingHead < 0)
        {
            return false;
        }

        transformed = html.Insert(closingHead, BuildBootstrapMarkup(pathBase, runtimeInfo));
        return true;
    }

    /// <summary>
    /// Builds markup with a base-path-aware script and config URL.
    /// </summary>
    public static string BuildBootstrapMarkup(PathString pathBase, IGammaJumpRuntimeInfo runtimeInfo)
    {
        var prefix = pathBase.HasValue ? pathBase.Value!.TrimEnd('/') : string.Empty;
        var encodedPrefix = HtmlEncoder.Default.Encode(prefix);
        var fingerprint = WebUtility.UrlEncode(runtimeInfo.ScriptFingerprint);
        var runtimeId = HtmlEncoder.Default.Encode(runtimeInfo.RuntimeId);
        var version = HtmlEncoder.Default.Encode(runtimeInfo.PluginVersion);
        return $"<script id=\"{MarkerId}\" data-gamma-jump-mode=\"plugin\" data-gamma-jump-config-url=\"{encodedPrefix}/GammaJump/client-config\" data-gamma-jump-runtime-url=\"{encodedPrefix}/GammaJump/runtime\" data-gamma-jump-runtime-id=\"{runtimeId}\" data-gamma-jump-script-fingerprint=\"{fingerprint}\" data-gamma-jump-plugin-version=\"{version}\"></script>"
            + $"<script src=\"{encodedPrefix}/GammaJump/gamma-jump.js?h={fingerprint}\" defer></script>";
    }
}
