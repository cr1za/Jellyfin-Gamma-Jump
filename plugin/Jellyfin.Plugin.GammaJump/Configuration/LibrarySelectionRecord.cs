using System.Xml.Serialization;

namespace Jellyfin.Plugin.GammaJump.Configuration;

/// <summary>
/// XML-compatible persisted state for one discovered, supported collection
/// folder. The canonical ID is unhyphenated so it matches Jellyfin Web routes.
/// </summary>
public sealed class LibrarySelectionRecord
{
    /// <summary>Gets or sets the canonical unhyphenated collection-folder GUID.</summary>
    [XmlAttribute]
    public string LibraryId { get; set; } = string.Empty;

    /// <summary>Gets or sets whether Gamma Jump is enabled for this library.</summary>
    [XmlAttribute]
    public bool Enabled { get; set; }
}
