# Add-on content and installation paths

This guide targets **GTA V Enhanced, offline Story Mode**. `<GAME>` means the
installation folder containing `GTA5_Enhanced.exe`. These are portable examples,
not a path to the developer's computer. The menu does not download or install
content: install the content first, restart GTA, then select it in the menu.

## Path map

| Content | Location / action | In this menu |
| --- | --- | --- |
| This menu | `<GAME>/scripts/TruthStoryPlus.3.cs` | F5 |
| Menu preferences, favorites, saved locations | `<GAME>/scripts/TruthStoryPlus/preferences.ini` | Interface / Spawner / Teleport |
| Saved outfits | `<GAME>/scripts/TruthStoryPlus/outfit-1.txt` through `outfit-5.txt` | Wardrobe; same character model |
| Menu diagnostic log | `<GAME>/scripts/TruthStoryPlus/TruthStoryPlus.log` | Review before sharing |
| Vehicle add-on DLC | `<GAME>/mods/update/x64/dlcpacks/<pack>/dlc.rpf` | Spawner → Spawn exact model name, or detected catalog |
| DLC registration | Inside `<GAME>/mods/update/update.rpf`: `common/data/dlclist.xml` | No menu setting; edit with the compatible archive tool |
| Clothing add-on DLC | `<GAME>/mods/update/x64/dlcpacks/<clothing-pack>/dlc.rpf` plus its DLC entry | Wardrobe → component / drawable / texture |
| Engine-sound add-on DLC | `<GAME>/mods/update/x64/dlcpacks/<sound-pack>/dlc.rpf` plus its DLC entry | Heard on a vehicle configured to use that sound |
| Vehicle sound assignment | The target vehicle's active `vehicles.meta`, inside its DLC/archive | Set that vehicle's `audioNameHash` as its author directs |
| Replacement clothes, vehicles, weapons or textures | The **exact archive and internal path named by that mod's author**, mirrored under `mods` | Select the replaced model/item normally |
| Other .NET scripts | Usually `<GAME>/scripts/`, with dependencies/configuration exactly as their author specifies | Independent of this menu |
| ASI plugins / loaders | Usually `<GAME>/`, following the matching Enhanced release instructions | Independent of this menu |
| Menu navigation sound | `Click()` in `scripts/TruthStoryPlus.3.cs` uses GTA's `NAV_UP_DOWN` / `HUD_FRONTEND_DEFAULT_SOUNDSET` | Interface → Navigation sounds |

Menu data paths above come from the shipped source. The DLC layout is documented
by the vehicle and clothing authors in [sources 4–5](SOURCES.md); script installation
is covered by [source 1](SOURCES.md). An `.rpf` segment is an **archive**, not a
Windows folder you create by hand.

## Enhanced archive setup

Use an Enhanced-compatible archive editor and archive loader. OpenRPF's author
documents `mods`-folder loading and warns that it does **not** convert Legacy
resources into Enhanced resources. Use a pack's Enhanced/Gen9 download and its
current instructions. The linked clothing pack specifically
documents CodeWalker and an Enhanced archive loader. [Sources 3 and 5](SOURCES.md)

If another installed mod manager uses a different layout, follow that manager's
instructions; do not mix alternate directory conventions or install duplicate
archive loaders just for this guide. Preserve the original game archives and
back up the `mods` copy before editing. This menu itself needs no OpenRPF setup
unless you also install replacement/add-on game assets.

## Add a vehicle

1. Download the vehicle's Enhanced-compatible **single-player add-on** package.
2. Place the unpacked DLC folder, not the download ZIP, under
   `<GAME>/mods/update/x64/dlcpacks/`. The resulting layout should contain
   `<pack>/dlc.rpf`, without an extra nested download folder.
3. Open the `mods` copy of `update/update.rpf` with the appropriate archive tool.
   Edit `common/data/dlclist.xml` and add the author's entry inside the existing
   `<Paths>` element. Generic example:

   ```xml
   <Item>dlcpacks:/your_pack/</Item>
   ```

4. Save and restart GTA. Use **Spawner → Spawn exact model name** with the author's
   **spawn/model name**. That name can differ from the DLC folder name.
5. Use the detected catalog, class filters and favorites once the game exposes
   the installed model. If enumeration omits it, exact-model spawning is the fallback.

Verified layout example: the Devel Sixteen author's Enhanced release uses a
`devel` DLC folder and the spawn name `devel`. This is a path example, not bundled
content or a compatibility guarantee for future game versions. [Source 4](SOURCES.md)

For a replacement vehicle, follow its replacement-archive instructions and spawn
the **original vehicle name**. Do not invent a DLC registration for a replacement
that was not authored as an add-on.

## Add clothing

Use a pack for the character you actually play. Clothing for an online freemode
ped is not automatically compatible with Michael, Franklin or Trevor. This menu
edits the current ped's supported variations; it does not turn the current player
into a different ped model or install a FiveM clothing resource.

For a documented Enhanced example, Custom Trio Clothes uses:

```text
<GAME>/mods/update/x64/dlcpacks/spPlayersClothes_Gen9/dlc.rpf
```

Its registration entry is:

```xml
<Item>dlcpacks:/spPlayersClothes_Gen9/</Item>
```

After restart, select the appropriate protagonist and use **Wardrobe → Clothing
component → Clothing drawable → Clothing texture**. Hats/glasses and similar
items use the accessory controls. Save the completed outfit in one of five slots.
Some packs replace existing slots rather than adding new ones. The author supplies
the exact slot mapping. [Source 5](SOURCES.md)

There is no universal folder for loose `.ydd`, `.ytd` or `.ymt` clothing files.
Use the creator's exact Enhanced archive path and filenames; never drop these
files into this menu's `scripts/TruthStoryPlus` data folder.

## Add engine sounds

For an Enhanced-compatible single-player **audio DLC**, install its folder in
`mods/update/x64/dlcpacks`, register it in `dlclist.xml`, then edit the **target
vehicle's active** `vehicles.meta` entry. The author supplies the audio identifier:

```xml
<audioNameHash>AUTHOR_PROVIDED_AUDIO_NAME</audioNameHash>
```

The audio identifier need not match the sound-pack folder name. Find `vehicles.meta`
inside the vehicle's DLC/archive; its internal location varies by pack. Preserve
every other vehicle entry. A loose `.wav` or `.mp3` in `scripts` is not an audio DLC.
For replacement `.awc` banks or sirens, follow the author's exact archive mapping.

KCMIR0's instructions document this DLC-plus-`audioNameHash` workflow. That older
page is a **format reference**, not evidence that its download is Enhanced-ready;
use a sound pack explicitly compatible with your game build. [Source 6](SOURCES.md)

## Menu sounds, weapons, maps and scripts

- **Menu sound:** Interface can toggle the built-in navigation sound. To change
  the sound itself, edit the sound name/set in `Click()` and rebuild or reload the
  script. This version has no custom audio-file picker.
- **Weapons:** replacing a vanilla weapon preserves its normal catalog entry.
  Arbitrary add-on weapons are not automatically added: the current catalog uses
  valid entries from the installed API's `WeaponHash` enum. Supporting a new
  custom hash requires a deliberate source change.
- **Maps/interiors:** use the author's Enhanced DLC or map-loader instructions.
  This menu provides teleportation, not an XML/YMAP importer.
- **Other scripts:** install dependencies where their author specifies. Keep
  duplicate menu copies out of `scripts`, and avoid overlapping hotkeys/camera effects.

## If content is missing

Check the edition, unpacked folder depth, DLC registration, exact spawn name,
character model and the pack's dependencies. Restart after archive changes.
Test one new pack at a time. If a specific asset crashes on load, isolate that
pack instead of changing the menu's camera or speed code. Loaded model limits
and compatibility are the asset/loader's responsibility. Refer to the original
author pages in [SOURCES.md](SOURCES.md) for updates and exact pack instructions.
