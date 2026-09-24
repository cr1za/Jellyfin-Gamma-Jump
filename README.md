# Gamma Jump for Jellyfin

Gamma Jump turns Jellyfin Web's native alphabet picker into a **jump control**.

Instead of filtering the library to one letter, **A–Z** scrolls to the first matching title in the already rendered result. **#** returns to the beginning. Jellyfin keeps its own cards, layout, filters, search, playback controls, and library renderer.

If Gamma Jump cannot prove that the current result is complete and safe to use, it does **not** intercept the picker; Jellyfin's native alphabet filtering remains available.

> **Compatibility:** Gamma Jump currently targets **Jellyfin Server 12.1 / Jellyfin Web 12.1** and the Jellyfin plugin ABI `12.1.0.0`. Newer Jellyfin versions are not claimed compatible until they are tested. The enhancement is for **Jellyfin Web**; native clients are outside the tested scope.

## Release and migration status

Gamma Jump is the renamed continuation of Alpha Jump.

The rename preserves the existing plugin GUID:

`4dd1ed79-9e5e-441e-8ca3-8f1b29601a48`

That keeps the catalog/plugin identity stable for migration. The Gamma Jump manifest should list **Gamma Jump packages only**; Alpha Jump releases remain available in the repository's GitHub release history rather than being relabeled as Gamma Jump versions.

Gamma Jump release assets use:

- `gamma-jump_<VERSION>.zip`
- `Jellyfin.Plugin.GammaJump.dll`

An in-place **Alpha Jump → Gamma Jump** server upgrade still needs controlled validation before it should be described as seamless.

## Install from the Jellyfin plugin repository

If Gamma Jump is not yet visible in the Catalog after adding the repository, no Gamma Jump package has been published yet. Do not substitute an old Alpha Jump release as a Gamma Jump build.

1. Open **Jellyfin → Dashboard → Plugins → Manage Repositories**.
2. Add a repository named **Gamma Jump**.
3. Set the repository URL to:

   `https://raw.githubusercontent.com/cr1za/Jellyfin-Gamma-Jump/main/manifest.json`

4. Open **Catalog** and install **Gamma Jump**.
5. Restart Jellyfin when requested.
6. Open **Dashboard → Plugins → Gamma Jump** and choose the libraries you want enabled.
7. Refresh open Jellyfin Web tabs once after installation or upgrade.

Jellyfin's repository/catalog installation method is preferred because it provides normal plugin update handling.

### Manual installation

For controlled testing, download a Gamma Jump release archive and extract it beneath Jellyfin's plugin directory, then restart Jellyfin.

A Gamma Jump release archive contains only:

`Jellyfin.Plugin.GammaJump.dll`

Jellyfin supplies the runtime assemblies.

Do **not** place `Jellyfin.Plugin.AlphaJump.dll` and `Jellyfin.Plugin.GammaJump.dll` together in the same manual plugin directory. Test the Alpha Jump → Gamma Jump migration on a disposable server before using a manual replacement on an important installation.

## Using Gamma Jump

Open a supported library view and use:

- **Grid** layout
- **Name** ascending sort
- an enabled library in **Dashboard → Plugins → Gamma Jump**

Then use the existing Jellyfin alphabet picker:

- **A–Z** — jump to the first matching rendered title.
- **#** — return to the beginning.
- Clicking the same letter again repeats the jump.
- Search and non-alphabet filters remain active.
- Gamma Jump does not leave a persistent selected letter behind.

Gamma Jump does not fetch library items independently and does not replace Jellyfin's library UI.

## Why Library page size becomes `0`

For a jump to work across the complete constrained result, Gamma Jump can set the signed-in user's browser-local **Library page size** preference to `0`.

In Jellyfin Web 12.1, zero disables pagination. This allows the current result to render as one complete set, but it can significantly increase browser memory, CPU use, and rendering cost for large libraries.

This preference is local to the signed-in user and browser origin, but it affects that user's Jellyfin library views on that origin—not only the one library where Gamma Jump is enabled.

Gamma Jump does not treat page size `0` alone as proof that the visible result is complete. It still verifies the route, layout, sort, start index, card types, native picker, loading state, and exact toolbar-total/rendered-card count before intercepting a letter.

If another plugin such as JellyTweaks controls Library Page Size, set that value to `0` as well or disable the conflicting page-size tweak.

## Supported views

| Library/view | Supported tabs | Expected rendered card types |
| --- | --- | --- |
| Movies | Movies, Favorites, Collections | `Movie`, `BoxSet` |
| Shows | Series, Collections | `Series`, `BoxSet` |
| Books / Audiobooks | Folders, Books, Collections, Favorites | `Folder`, `AudioBook`, `Book`, `BoxSet` |
| Built-in Collections | Collections, Favorites | `BoxSet` |
| Home Videos / Photos | Folders, Photos, Photo Albums, Videos | `Folder`, `Photo`, `PhotoAlbum`, `Video` |
| Mixed libraries | Folders, Mixed, Collections | `Folder`, `Movie`, `Series`, `BoxSet` |
| Music | Albums, Collections | `MusicAlbum`, `BoxSet` |
| Music Videos | Folders, Music Videos | `Folder`, `MusicVideo` |
| Playlists | Playlists, Favorites | `Playlist` |

Support is based on Jellyfin's collection type, route/tab, and rendered card contract—not on the library's display name.

Live TV, standalone Photos pages, Suggestions, genres, studios/networks, people, authors/artists, songs, episodes/upcoming, embedded media-library playlists, item/collection detail pages, unknown layouts, unsupported sorts, and unrecognized card types remain native.

See [docs/feasibility.md](docs/feasibility.md) and [docs/testing.md](docs/testing.md) for the detailed compatibility and validation record.

## Configuration

The Gamma Jump settings page provides:

- **Enable Gamma Jump** — global injection switch.
- **Enable newly discovered compatible libraries** — default applied when a supported library is discovered for the first time.
- **Enable built-in Collections** — separate opt-in for Jellyfin's built-in Collections view.
- **Per-library enablement** — stored against stable Jellyfin library IDs, so library renames do not change the selection.
- **Set the active browser user's library page size to zero** — automatic complete-result setup; enabled by default.
- **Smooth scroll**.
- **Enable browser debug logging**.

Unsupported libraries are shown disabled with an explanation rather than silently enabled.

## Fail-closed behavior

Gamma Jump intercepts the native alphabet picker only when the current view satisfies its verified contract. Among other checks, it requires:

- a supported Jellyfin 12.1 route and tab;
- the expected page and exactly one native alphabet picker;
- Grid layout;
- ascending `SortName`;
- explicit persisted `StartIndex: 0`;
- allowed rendered card types with usable prefixes;
- no pending result state;
- an exact numeric toolbar-total to rendered-card-count match.

If any required state is missing, malformed, ambiguous, changing, or unsupported, Jellyfin's native behavior remains in control.

When Jellyfin already has an alphabet filter selected, Gamma Jump clears it through Jellyfin's existing control and waits for the complete unfiltered result before jumping.

## Updates and the Alpha Jump rename

Gamma Jump fingerprints the embedded browser script so a loaded Web tab can detect a changed payload after a plugin update and server restart.

Automatic reload is intentionally conservative. If the script cannot positively establish a safe reload state, it shows a **refresh to apply** action instead of forcing a reload.

For migration compatibility, the rename keeps the old browser page-size backup namespace and a temporary legacy `/AlphaJump` API alias. These are implementation compatibility details, not a second installed plugin.

After the Alpha Jump → Gamma Jump rename release, restart Jellyfin and manually refresh each open Jellyfin Web tab once.

## Troubleshooting

If the alphabet picker still filters instead of jumping:

1. Confirm the library is enabled in **Dashboard → Plugins → Gamma Jump**.
2. Confirm the view is one of the supported tabs above.
3. Use **Grid** layout.
4. Sort by **Name ascending**.
5. If automatic page-size setup is disabled, set Jellyfin's Library page size to `0`.
6. Remove conflicting page-size overrides from JellyTweaks or similar plugins.
7. Refresh the Jellyfin Web tab.

For browser diagnostics, enable **browser debug logging** in Gamma Jump settings and inspect the browser console for `[GammaJump]` messages.

Expected failures deliberately leave native Jellyfin behavior available rather than forcing the enhancement to run.

## Development

Browser checks:

```sh
node --check src/gamma-jump.js
node --test tests/gamma-jump.test.js
```

Plugin build and tests:

```sh
dotnet build plugin/Jellyfin.Plugin.GammaJump/Jellyfin.Plugin.GammaJump.csproj --configuration Release
dotnet test plugin/Jellyfin.Plugin.GammaJump.Tests/Jellyfin.Plugin.GammaJump.Tests.csproj --configuration Release
```

Release/static checks:

```sh
node scripts/validate-release-abi.js
git diff --check
```

The automated suites are deterministic regression checks. They are not a substitute for served Jellyfin browser compatibility or large-library performance testing.

## Release flow

The project uses four-part version tags such as `v0.5.0.0`.

A release tag validates the browser source and C# projects, builds the tagged plugin version, creates `gamma-jump_<VERSION>.zip`, calculates checksums, creates a draft GitHub Release, and prepares a manifest pull request. Merging the generated manifest PR publishes the prepared release.

The Gamma Jump manifest contains Gamma Jump release entries only. Historical Alpha Jump releases remain in GitHub release history and are not relabeled as Gamma Jump packages.

## Technical documentation

- [Architecture](docs/architecture.md)
- [Feasibility and compatibility model](docs/feasibility.md)
- [Testing and evidence](docs/testing.md)
- [Security controls](docs/security.md)
- [Project milestones](PROJECT-MILESTONES.md)

## Security

See [SECURITY.md](SECURITY.md) for vulnerability reporting.

Gamma Jump does not modify Jellyfin Web files on disk. Its server-side startup middleware rewrites only the served Web index HTML to add a same-origin bootstrap marker and script reference. API, media, image, CSS, JavaScript asset, and other non-index responses are not intentionally rewritten.

## License

See [LICENSE](LICENSE).
