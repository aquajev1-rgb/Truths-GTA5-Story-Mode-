# Truth's GTA5 Story Mode+

**v1.0.0 · Public release · GTA V Enhanced · Offline Story Mode only**

A green-and-black, translucent menu with boxed controls, an animated binary
backdrop, a compact monochrome logo, keyboard navigation and controller support.
F5 remains the open/close key.

## Version 1.0.0

The first public release follows the private development builds. Camera tracking,
on-foot first-person FOV and the 180 MPH stopping fix are release features,
confirmed working in-game by the project owner. All features are released for use;
model-specific availability and documented operating limits still apply.

- The startup banner now reads **Truths Story Mode+** on a separate full-width line,
  with `Loaded v1.0.0. Press F5 to open.` below it.

- Fixed the footer logo's drawing-budget issue; added a larger bordered badge.
- Assigned the binary animation a lower layer behind all menu boxes.
- Reduced gradient/logo draw calls: at most **313 rectangles including a toast**
  in the offline tests, versus roughly 644 for the previous menu alone.
- Reformatted the script, bounded keyboard input, corrected help text, expired
  stale confirmation prompts and removed private exception text from diagnostics.

See [FEATURES.md](FEATURES.md) for every menu option, [ADDONS.md](ADDONS.md) for
vehicle/sound/clothing installation paths, and [SOURCES.md](SOURCES.md) for citations.
[VALIDATION.md](VALIDATION.md) separates owner in-game confirmation from offline checks.

## Requirements

- GTA V **Enhanced**, Windows, offline Story Mode.
- [Script Hook V](https://www.dev-c.com/gtav/scripthookv/), compatible with your game build.
- [ScriptHookVDotNet Enhanced](https://github.com/Chiheb-Bacha/ScriptHookVDotNetEnhanced),
  with matching loader/API files and the Microsoft runtimes its documentation requires.

Loader binaries, the game and runtime installers are not included. This script
was compiled against Enhanced API v3.9.0; that is a tested compilation target,
not a guarantee of compatibility with every game or loader update.

Do not use in GTA Online. The session guard is not an anti-cheat bypass. Follow
the loader's documented offline Story Mode setup; use a clean, unmodded
installation before returning online.

## Install or update

1. Fully exit GTA V and back up your Story save.
2. Copy **only the included `scripts` folder** into your GTA V Enhanced installation
   folder, beside the game's executable.
3. Replace `scripts/TruthStoryPlus.3.cs`. Keep only one installed copy. Move older
   copies, including `StoryMenu.3.cs`, outside the loader's scripts directory.
   Do not install both this source and a compiled DLL of the same menu.
4. Keep existing preferences/outfits if desired; none are included or overwritten
   by this package. Load an offline Story save and press F5.

The PNG is reference artwork. The actual logo is embedded in the script and does
not depend on a PNG path or external texture hook. Replacing the PNG alone does
not change the displayed emblem.

To uninstall, exit GTA and move `scripts/TruthStoryPlus.3.cs` out of `scripts`.
Keep `scripts/TruthStoryPlus/` if you want to retain saved menu settings.

## Controls

| Action | Keyboard | Controller after F5 |
| --- | --- | --- |
| Open / close | F5 | F5 still opens the menu |
| Select / adjust | Arrows | D-pad |
| Apply | Enter / Numpad 5 | A / Cross |
| Back | Escape / Backspace | B / Circle |
| Category | Tab / Shift+Tab | RB / LB |
| Search / exact cash entry | F6 | X / Square |
| Scroll a page | PageUp / PageDown | Repeated D-pad input |

Text entry uses GTA's on-screen keyboard. F4 closes this menu to leave the loader
console accessible; another trainer may also bind F4.

## Features

Fourteen categories: Home, Player, Cash, Spawner, Vehicle, Performance, Custom Shop,
Weapons, Wardrobe, Teleport, World, Playground, Scenarios and Interface.

- Player survival, movement and wanted controls; confirmed Story-character cash editing.
- Detected vehicle catalog, search/class filters, favorites, exact model input,
  random customized builds, repair, doors, protection and tracked cleanup.
- Supported body parts, wheels, paint/RGB colors, plates, liveries, extras, lights,
  performance upgrades, torque and a gradual MPH cap. Zero cap means off.
- Weapon catalog, ammo and tints; clothing components, props and saved outfit slots.
- Weather/time, traffic types/density targets, slow motion, HUD options and
  gameplay-camera tracking and **on-foot** first-person FOV.
- Teleports, saved locations, scenarios, rainbow paint, car hopping, horn boost,
  ragdoll actions, props and character guests.
- UI opacity, scale, placement, help, sounds, branding and a driving speed display.

Random customized builds randomize supported cosmetics and maximize supported
performance upgrades. Vehicle availability depends on the game and installed
add-ons. Trains are excluded from the normal spawner; boats/aircraft need suitable space.

Traffic above 100% uses gradually added AI-driven cars, capped at 60, with streaming,
space and optional low-FPS guards. Percentages are targets, not guaranteed multiples
of visible traffic. Existing ambient/mission traffic is not replaced; traffic types
apply to new controller-created cars.

Vehicle FOV remains removed. The optional on-foot overlay releases when entering
vehicles, aiming, using third person, pausing, dying, entering cutscenes or resetting.
The gameplay camera's position and rotation are followed each frame. The 180 MPH
stopping issue is fixed; 180 MPH is not a forced speed cap.

## Local data and troubleshooting

Preferences, favorites, saved game-world coordinates, outfits and diagnostics live
under `scripts/TruthStoryPlus/`. Continuous effects start disabled. Use Interface's
save-preferences option to retain UI choices. Reset does not undo one-time money,
clothing, vehicle-upgrade or teleport changes; experiment with a separate Story save.

If F5 does nothing, inspect the Enhanced loader console/log and this menu's log.
Record game/loader versions and the first relevant error. Review every log before
sharing; other tools may include personal paths. See [SECURITY.md](SECURITY.md).

## Development and publication

[CONTRIBUTING.md](CONTRIBUTING.md) covers compilation, tests, packaging and the
in-game release checklist. This source-only archive excludes runtime settings,
logs, screenshots, dependency binaries and private install paths.

## License

[PolyForm Noncommercial 1.0.0](LICENSE.md) permits modification and noncommercial
sharing; commercial use, including commercial resale, is not granted. This is
**source-available**, not OSI open source. Apache 2.0 was not selected because it
does not implement the requested no-commercial-sale restriction.
See [LICENSE-NOTICE.md](LICENSE-NOTICE.md) and [licensing sources 7–9](SOURCES.md).

[RELEASE.md](RELEASE.md) provides the v1.0.0 release notes. This package does not
create a repository, push commits or publish a GitHub release.

References: [Enhanced API](https://github.com/Chiheb-Bacha/ScriptHookVDotNetEnhanced),
[native database](https://github.com/alloc8or/gta5-nativedb-data),
[DRAW_RECT notes](https://github.com/citizenfx/natives/blob/master/GRAPHICS/DrawRect.md).
