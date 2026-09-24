# Gamma Jump for Jellyfin

Gamma Jump turns Jellyfin Web's alphabet picker into a fast **A–Z jump control**.

Instead of filtering the library down to one letter, selecting a letter scrolls to the first matching rendered title while keeping the complete current result available. Select **#** to return to the beginning.

Gamma Jump uses Jellyfin's existing cards, layout, filters, search, playback, and library renderer. If the plugin cannot prove that the current result is complete and safe to jump within, it leaves Jellyfin's native alphabet behavior alone.

> **Compatibility:** Jellyfin **12.1+** / Jellyfin Web 12.1 behavior. Gamma Jump is a browser-side enhancement delivered by a Jellyfin server plugin.

## Quick install

1. Open **Jellyfin → Dashboard → Plugins → Repositories**.
2. Add a repository named **Gamma Jump**.
3. Use this repository URL:

   `https://raw.githubusercontent.com/cr1za/Jellyfin-Gamma-Jump/main/manifest.json`

4. Open **Catalog** and install **Gamma Jump**.
5. Restart Jellyfin if prompted.
6. Open **Dashboard → Plugins → Gamma Jump**.
7. Enable the libraries you want to use.
8. Refresh open Jellyfin Web tabs once.

Repository installation is recommended because it supports normal plugin updates.

### Manual installation

Download the latest `gamma-jump_<VERSION>.zip` from GitHub Releases, extract it into a Gamma Jump directory beneath Jellyfin's plugins directory, and restart Jellyfin.

The release archive contains only `Jellyfin.Plugin.GammaJump.dll`; Jellyfin provides the runtime assemblies.

## Upgrading from Alpha Jump

Gamma Jump is the continuation of Alpha Jump and keeps the same Jellyfin plugin GUID:

`4dd1ed79-9e5e-441e-8ca3-8f1b29601a48`

That allows an existing installation to upgrade rather than appear as a second unrelated plugin.

The rename release intentionally retains two hidden compatibility details:

- the previous browser-storage prefix, so existing page-size backups can still be restored;
- the previous `/AlphaJump` API route as a temporary alias, so already-open pre-rename tabs can detect the changed browser payload.

After upgrading from Alpha Jump, restart Jellyfin and refresh each open Jellyfin Web tab once.

## Using Gamma Jump

Open a supported library view in **Grid** mode and sort by **Name / SortName ascending**.

- **A–Z** — jump to the first matching rendered title.
- **#** — return to the beginning.
- Existing search and non-alphabet filters stay active.
- Repeated clicks repeat the jump; Gamma Jump does not leave a selected letter behind.
- Unsupported or uncertain views keep Jellyfin's normal picker behavior.

Gamma Jump does not independently request library items and does not replace Jellyfin's library UI.

## Requirements and performance

Gamma Jump's complete-result mode uses Jellyfin's browser-local **Library page size = 0** preference when the corresponding setting is enabled.

Jellyfin treats zero as unpaginated mode, allowing the current constrained result to render as one complete set. This may use considerably more browser memory and CPU for very large libraries.

Gamma Jump still refuses to arm merely because the page-size preference is zero. It additionally verifies the current grid, sort, start index, card types, native picker, and exact toolbar-total/card-count match.

If another plugin such as JellyTweaks controls Library Page Size, configure it to use `0` or disable its conflicting page-size tweak.

## Supported library views

| Library/view | Supported tabs |
| --- | --- |
| Movies | Movies, Favorites, Collections |
| Shows | Series, Collections |
| Books / Audiobooks | Folders, Books, Collections, Favorites |
| Built-in Collections | Collections, Favorites |
| Home Videos / Photos | Folders, Photos, Photo Albums, Videos |
| Mixed libraries | Folders, Mixed, Collections |
| Music | Albums, Collections |
| Music Videos | Folders, Music Videos |
| Playlists | Playlists, Favorites |

Support is determined by Jellyfin collection type and verified rendered card types, not by the library's display name.

Live TV, standalone Photos pages, Suggestions, genres, studios/networks, people, authors/artists, songs, episodes/upcoming, embedded media-library playlists, item detail pages, and unrecognized layouts remain native.

See [docs/feasibility.md](docs/feasibility.md) and [docs/testing.md](docs/testing.md) for the detailed compatibility model and validation notes.

## Configuration

The Gamma Jump plugin page provides:

- **Enable Gamma Jump** — global browser-enhancement switch.
- **Enable newly discovered compatible libraries** — default for newly found supported folders.
- **Enable built-in Collections** — separate opt-in for Jellyfin's server-provided Collections view.
- **Per-library enablement** — stable Jellyfin library IDs survive library renames.
- **Set the active browser user's library page size to zero** — complete-result setup, enabled by default.
- **Smooth scroll**.
- **Browser debug logging**.

Incompatible folders are shown disabled with an explanation.

## Safety model

Gamma Jump intercepts the native alphabet picker only when all required conditions are satisfied, including:

- a registered supported Jellyfin route/tab;
- the expected page and native alphabet picker;
- Grid view;
- `SortBy: ["SortName"]`;
- ascending sort;
- explicit persisted `StartIndex: 0`;
- allowed rendered card types with usable prefixes;
- an exact numeric toolbar-total to rendered-card-count match.

If any requirement is missing, pending, malformed, or ambiguous, native Jellyfin behavior remains in control.

An existing Jellyfin alphabet selection is cleared using Jellyfin's own button once, then Gamma Jump waits for the unfiltered complete result before jumping.

## Updates

Gamma Jump fingerprints its embedded browser script.

After a changed-script plugin upgrade and server restart, a loaded Web tab can detect the new runtime and request a refresh. Automatic reload is deliberately conservative; when safe playback state cannot be positively established, Gamma Jump displays a **refresh to apply** action instead.

The Alpha Jump → Gamma Jump rename itself requires one manual Web refresh after the server upgrade.

## Troubleshooting

If letters still filter instead of jump:

1. Confirm the library is enabled in **Dashboard → Plugins → Gamma Jump**.
2. Use **Grid** view.
3. Sort by **Name ascending**.
4. Ensure the Library page-size setting is `0` when automatic setup is disabled.
5. Remove conflicting page-size overrides from JellyTweaks or similar plugins.
6. Refresh the Jellyfin Web tab.

For browser diagnostics, enable **browser debug logging** in Gamma Jump's settings and inspect the browser console for `[GammaJump]` messages.

## Development

Browser validation:

```sh
node --check src/gamma-jump.js
node --test tests/gamma-jump.test.js
```

Plugin validation:

```sh
dotnet build plugin/Jellyfin.Plugin.GammaJump/Jellyfin.Plugin.GammaJump.csproj --configuration Release
dotnet test plugin/Jellyfin.Plugin.GammaJump.Tests/Jellyfin.Plugin.GammaJump.Tests.csproj --configuration Release
```

Additional release validation:

```sh
node scripts/validate-release-abi.js
git diff --check
```

The browser tests are deterministic regression tests, not a substitute for served Jellyfin compatibility/performance testing.

## Release flow

Four-part tags such as `v0.5.0.0` run validation, build `Jellyfin.Plugin.GammaJump.dll`, package `gamma-jump_<VERSION>.zip`, calculate checksums, and prepare the Jellyfin repository manifest update.

Historical manifest entries intentionally retain their original Alpha Jump release URLs. New Gamma Jump releases use the renamed repository and Gamma Jump asset names.

## Technical documentation

- [Architecture](docs/architecture.md)
- [Feasibility and compatibility model](docs/feasibility.md)
- [Testing](docs/testing.md)
- [Security controls](docs/security.md)
- [Project milestones](PROJECT-MILESTONES.md)

## Security

See [SECURITY.md](SECURITY.md) for vulnerability reporting.

The plugin does not patch installed Jellyfin Web files. Its startup middleware modifies only the served Web index HTML to add one same-origin bootstrap marker and script reference; APIs, media, images, CSS, JavaScript assets, and other paths are not rewritten.

## License

See [LICENSE](LICENSE).
