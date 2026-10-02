# Complete feature reference — v1.0.0

All 224 top-level rows from the shipped script are listed below, including selectors, presets and shortcuts. Dynamic vehicle/weapon catalogs and landmark entries are additional. These are released features; camera tracking, on-foot first-person FOV and the high-speed stopping fix have been confirmed in-game by the project owner.

Source: [`scripts/TruthStoryPlus.3.cs`](scripts/TruthStoryPlus.3.cs), its actual menu definitions. Availability of individual vehicle parts, clothes, animations and weapon variations depends on the loaded game model.

## HOME

| Feature | Behavior |
| --- | --- |
| Player controls | Health, protection, movement and wanted level. |
| Garage & vehicle catalog | Browse every installed vehicle model detected by the Enhanced API. |
| Performance workshop | Power boost, torque and speed cap with explicit reset controls. |
| Playground | Funny vehicle effects, physics toys, props and character antics. |
| Heal + armor + repair | Restore your character and repair the vehicle you occupy. |
| Clear wanted level | Remove the active wanted level. |
| Reset active effects | Turn off this menu's continuous effects and restore the states it tracked. |
| Clean up spawned toys | Delete only props and NPCs created by this menu. Spawned vehicles have separate cleanup. |
| Interface preferences | Transparency, scale, placement, binary rain, sounds and the driving HUD. |

## PLAYER

| Feature | Behavior |
| --- | --- |
| Invincibility | Protect the active character; the original state is restored when disabled. |
| Heal & full armor | Restore maximum health, fill armor and remove visible injuries. |
| Never wanted | Continuously keep your wanted level at zero. |
| Wanted stars | Choose 0-5 stars, then use Apply wanted level. |
| Apply wanted level | Disables Never wanted when applying a nonzero level. |
| Infinite stamina | Refill stamina each frame while enabled. |
| Super jump | Higher jumps while on foot. |
| Run multiplier | Uses the game's supported 1.00-1.49 sprint multiplier. |
| Swim multiplier | Uses the game's supported 1.00-1.49 swim multiplier. |
| Invisible | Hide your character. Collision and damage rules remain active. |
| Prevent ragdoll | Disable involuntary ragdoll. Playground ragdoll requires this to be off. |
| Police ignore player | Ask police to ignore your character. |
| Everyone ignores player | Ask NPCs to ignore your character. |
| Refill special ability | Fill the current protagonist's ability meter when available. |
| Clean character | Remove blood, wetness and visible damage. |
| Dry clothes | Clear the wetness effect on your character. |
| Soak clothes | Apply a visible wet clothing effect. |
| Give parachute | Equip a parachute for your next jump. |

## CASH

| Feature | Behavior |
| --- | --- |
| Amount | Adjust in steps of $10,000. Exact entry is available below or with F6 / X. |
| Enter exact amount | Enter a whole number from 0 to 2,000,000,000. |
| Add amount | Add the selected amount to the active Story protagonist. |
| Subtract amount | Subtract the selected amount; the result cannot go below zero. |
| Set exact balance | Replace your character's balance with the selected amount. Apply twice to confirm. |
| Clear balance | Set the active character's cash to zero. Apply twice to confirm. |
| Preset: $1,000 | Select this amount without changing your balance yet. |
| Preset: $10,000 | Select this amount without changing your balance yet. |
| Preset: $100,000 | Select this amount without changing your balance yet. |
| Preset: $1,000,000 | Select this amount without changing your balance yet. |
| Preset: $10,000,000 | Select this amount without changing your balance yet. |
| Preset: $100,000,000 | Select this amount without changing your balance yet. |

## SPAWNER

| Feature | Behavior |
| --- | --- |
| Browse all vehicles | Catalog is built from loaded game model metadata. Models stream only when selected. |
| Search models | Search display names, enum model names or hashes. F6 / X also opens search. |
| Vehicle class | Filter the catalog by vehicle class. |
| Favorites only | Limit the catalog to your saved favorite models. |
| Enter spawned vehicle | Place your character in the driver seat after spawning. |
| Spawn exact model name | Enter a model such as adder, sultanrs or an installed add-on model name. |
| Spawn random road vehicle | Choose an available car or motorcycle from the detected catalog. |
| Random customized vehicle | Spawn a random road vehicle with randomized body parts, wheels, paint, lights, extras and maximum supported performance upgrades. |
| Repeat last spawn | Spawn the last model you selected. |
| Favorite last selected model | Save or remove the last selected vehicle in your favorites. |
| Clear catalog filters | Show all classes and reset the search and favorites filter. |
| Remove last spawned vehicle | Only removes a vehicle created by this menu. Exit it first. |
| Clean up spawned vehicles | Delete unoccupied vehicles created by this menu; occupied vehicles are kept. |

## VEHICLE

| Feature | Behavior |
| --- | --- |
| Repair vehicle | Restore body, engine and fuel tank health. |
| Wash vehicle | Remove dirt and decals. |
| Vehicle invincibility | Protect the vehicle you occupy, restoring its prior state when you leave. |
| Automatic repair | Restores health while moving; full body repair when nearly stopped. Preserves driving momentum. |
| Seatbelt | Disable windscreen ejection while in a vehicle. |
| Engine on | Start the engine. |
| Engine off | Stop the engine without changing vehicle ownership. |
| Upright vehicle | Remove roll and pitch, then settle the vehicle onto the ground. |
| Open all doors | Open supported doors, including hood and trunk. |
| Close all doors | Close supported doors. |
| Roll windows down | Lower all supported windows. |
| Roll windows up | Raise all supported windows. |
| Fix all tires | Repair the standard tire indices on your current vehicle. |
| Bulletproof tires | Prevent standard tire punctures on this vehicle. |
| Normal tires | Allow standard tire punctures on this vehicle. |
| Lock doors | Lock vehicle doors. |
| Unlock doors | Unlock vehicle doors. |
| Siren on | Enable a siren if your vehicle has one. |
| Siren off | Disable the siren. |

## PERFORMANCE

| Feature | Behavior |
| --- | --- |
| Engine power boost | Additional power in percent. Zero returns to stock power. |
| Torque multiplier | 100% is stock; applied each frame while driving. |
| MPH speed cap | Set 0 for OFF. Smooth horizontal limiter preserves airborne motion; braking into a lower cap is gradual. |
| Enter exact MPH cap | Enter 0-400. Zero turns the cap off. |
| Launch speed | Select the one-time forward speed used by Apply launch speed. |
| Apply launch speed | Set forward vehicle speed to the selected MPH value. |
| Drift / reduced grip | Use the game's reduced-grip mode for your current vehicle. |
| Max performance upgrades | Apply the highest supported engine, brake, transmission, suspension and armor mods, plus turbo. |
| Stock performance upgrades | Remove performance mods and turn turbo off. |
| Toggle turbo | Toggle installed turbo on vehicles supporting it. |
| Reset performance effects | Reset power, torque, MPH cap and reduced grip. |
| Driving speedometer | Show a compact branded MPH display when the menu is closed. |

## CUSTOM SHOP

| Feature | Behavior |
| --- | --- |
| Modification category | Choose a supported part category, then change Part index. |
| Part index | Stock is -1. Unavailable categories remain unchanged. |
| Max performance package | Highest supported performance upgrades and turbo. |
| Primary paint index | GTA paint index from 0-159. Apply using the row below. |
| Secondary paint index | GTA paint index from 0-159. Apply using the row below. |
| Apply indexed paint | Clear custom RGB paint and apply the selected indexed colors. |
| Truth signature paint | Black paint with green accents and green underglow. |
| Custom primary RGB | Enter three numbers separated by spaces, such as 20 200 60. |
| Custom secondary RGB | Enter three numbers separated by spaces, such as 5 12 8. |
| Wheel type | Cycle wheel categories 0-12. Availability depends on the vehicle. |
| Apply wheel type | Apply wheel type, then use the Front wheels modification category. |
| Window tint | 0=none, 1=pure black, 2=dark, 3=light, 4=stock, 5=limo, 6=green. |
| Apply window tint | Apply the selected tint to supported windows. |
| Livery index | Choose a livery index. Apply checks the vehicle's native livery count. |
| Apply native livery | Some vehicles use modification slot 48 instead; select Liveries above for those. |
| Extra ID | Select an extra attachment ID from 0-20. |
| Toggle selected extra | Toggle only if the extra exists on this vehicle. |
| Underglow style | Off, green, lime, emerald or mint. |
| Apply underglow | Enable or disable neon strips on supported vehicles. |
| Toggle xenon lights | Toggle xenon headlights on supported vehicles. |
| Green tire smoke | Enable custom tire smoke and apply signature green. |
| Edit license plate | Enter up to eight characters, then apply to your vehicle. |
| Apply license plate | Write the chosen text to this vehicle's plate. |

## WEAPONS

| Feature | Behavior |
| --- | --- |
| Browse weapons | List of valid weapon hashes in your installed Enhanced API. Select to equip. |
| Search weapons | Search the weapon catalog by name. F6 / X also opens search. |
| Give selected weapon | Equip your selection with 500 rounds. |
| Basic loadout | Pistol, carbine rifle, shotgun and a parachute. |
| Refill equipped ammo | Add 500 rounds to the currently equipped weapon. |
| Give parachute | Add a parachute to your inventory. |
| Infinite reserve ammo | Apply infinite ammo to each weapon equipped while enabled; clear those flags when disabled. |
| Infinite clip | Skip clip depletion while enabled. |
| Explosive bullets | Explosive projectiles for supported weapons; disables fire bullets. |
| Fire bullets | Incendiary projectiles for supported weapons; disables explosive bullets. |
| Explosive melee | Explosive hits from supported melee attacks. |
| Damage multiplier | 100% is normal. Applies to the player's weapon damage. |
| Weapon tint index | Tint range depends on the equipped weapon. Apply clamps to the supported range. |
| Apply equipped weapon tint | Color the equipped weapon using a supported tint index. |
| Remove equipped weapon | Remove the currently held weapon. Apply twice to confirm. |
| Remove all weapons | Clear the player's weapon inventory. Apply twice to confirm. |

## WARDROBE

| Feature | Behavior |
| --- | --- |
| Clothing component | Availability depends on the character model. |
| Clothing drawable | Cycle every drawable supported by the selected component. |
| Clothing texture | Cycle every texture for the current drawable. |
| Accessory slot | Choose hats, glasses, ears, watches or bracelets. |
| Accessory drawable | -1 removes the selected accessory. |
| Accessory texture | Cycle the textures supported by the current accessory. |
| Clear selected accessory | Remove the accessory from the selected slot. |
| Clear all accessories | Remove hats, glasses and other props. |
| Randomize clothing | Select random component variations supported by this character. |
| Default outfit | Restore this character model's default component variations. |
| Saved outfit slot | Five disk-backed outfit slots. A saved outfit loads only on the same character model. |
| Save current outfit | Save components, textures and accessories to this slot. |
| Load saved outfit | Validate the character model and all saved component indices before applying. |

## TELEPORT

| Feature | Behavior |
| --- | --- |
| Landmarks | Twelve outdoor locations across Los Santos and Blaine County. |
| Waypoint teleport | Probe ground at your map waypoint. Cancel safely if a ground surface cannot be found. |
| Backtrack | Return to the position before your last teleport. |
| Saved position slot | Choose one of five slots stored on disk. |
| Save current position | Store your position and heading in the selected slot. |
| Load saved position | Teleport to the selected slot, including your occupied vehicle. |
| Step forward 5 meters | Move a short distance ahead. Check that the space is clear first. |
| Move up 3 meters | Raise your character or current vehicle three meters. |
| Skydiving jump | On foot: give a parachute and teleport 650 meters above your current position. |

## WORLD

| Feature | Behavior |
| --- | --- |
| On-foot first-person FOV | Opt-in camera overlay for first person while on foot. It releases immediately in vehicles, while aiming or during cutscenes. |
| On-foot FOV scale | Scales the on-foot first-person gameplay FOV, clamped to 45-110 degrees. Vehicle cameras are never modified. |
| Traffic controller | Enable custom density and extra AI drivers. Extra cars spawn gradually on roads outside your view. |
| Traffic density | 0-100% adjusts ambient traffic. Above 100% adds up to 60 extra AI cars at 500%, subject to the extra-car limit and FPS guard. |
| Extra traffic type | Type applies to menu-created traffic. Existing game cars are never replaced. |
| Selected traffic only | Suppress new ambient cars and populate with the chosen type instead. Existing traffic fades naturally; mission vehicles remain. |
| Extra traffic limit | Maximum active menu-created cars, independent of existing traffic. More AI vehicles can reduce FPS. |
| Traffic FPS guard | Pause new traffic spawns below approximately 30 FPS. This does not guarantee a frame rate. |
| Traffic status | Shows managed car count and spawn state. Density is a target, not an exact count multiplier. |
| Clear added traffic | Disable the controller and remove only its cars/drivers. A car occupied by you is kept. |
| Weather preset | Select a weather type, then apply. Weather remains forced until released. |
| Apply weather | Apply the selected weather persistently. |
| Release weather override | Allow the game's normal weather system to resume. |
| Hour | Select 0-23, then Apply time. |
| Minute | Select 0-59, then Apply time. |
| Apply time | Set the clock to your chosen hour and minute. |
| Golden hour | Set the clock to 18:30 and apply clear weather. |
| Midnight rain | Set the clock to midnight and apply rain. |
| Freeze clock | Pause game-clock progression until disabled. |
| Time speed | 100% is normal; 25-100% gives slow motion. |
| Gravity | World gravity: normal, low or very low. Applies to the whole session. |
| City blackout | Switch off artificial city lighting until disabled. |
| Suppress new traffic | Reduce ambient vehicle spawning each frame; existing vehicles remain. |
| Suppress new pedestrians | Reduce ambient pedestrian spawning each frame; existing NPCs remain. |
| Night vision | Enable night vision; switches off thermal vision. |
| Thermal vision | Enable thermal vision; switches off night vision. |
| Hide game HUD | Hide the game's HUD each frame for screenshots. |
| Hide minimap | Hide the radar while enabled. |

## PLAYGROUND

| Feature | Behavior |
| --- | --- |
| Rainbow paint | Cycle custom vehicle colors. Restore the vehicle's original colors when disabled or exited. |
| Bunny-hop vehicle | Give your grounded vehicle a hop every 1.5 seconds. Works while the menu is closed. |
| Horn boost | Hold the normal vehicle horn control to accelerate. Works while the menu is closed. |
| Vehicle hop | Add upward velocity to your vehicle for a single jump. |
| Send vehicle skyward | Launch your occupied vehicle vertically with an upward speed of 25 m/s. |
| Instant handbrake stop | Set your current vehicle's velocity to zero. |
| Turn vehicle around | Rotate your vehicle 180 degrees and stop its movement. |
| Ragdoll flop | On foot: make your character fall over. Requires Prevent ragdoll to be off. |
| Superhero leap | On foot: launch up and forward with a parachute. |
| Skydiving jump | On foot: parachute from 650 meters above your current location. |
| Clone my character | Create one copy of your character nearby. Tracked for cleanup. |
| Clown bodyguard | Spawn a clown who joins your player's group. |
| Mime bodyguard | Spawn a mime who joins your player's group. |
| Chimp cameo | Spawn a chimp nearby. Tracked for cleanup. |
| Poodle cameo | Spawn a poodle nearby. Tracked for cleanup. |
| Beach ball | Drop a physical beach ball a short distance ahead. |
| Traffic cone | Drop a physical cone a short distance ahead. |
| Barrel | Drop a physical barrel a short distance ahead. |
| Street party | Start the partying scenario on your character. |
| Surprise me | Randomly select a harmless menu action: heal, outfit shuffle, party, parachute or a prop. |
| Clean up toys & guests | Delete only the props and NPCs created by this menu. |
| Stop playground effects | Disable rainbow paint, bunny hops and horn boost. |

## SCENARIOS

| Feature | Behavior |
| --- | --- |
| Stop current scenario | Clear the player's current scenario task. |
| Dance / party | On foot only. Game chooses an animation variation supported by your current character. |
| Street musician | On foot only. Game chooses an animation variation supported by your current character. |
| Human statue | On foot only. Game chooses an animation variation supported by your current character. |
| Push-ups | On foot only. Game chooses an animation variation supported by your current character. |
| Sit-ups | On foot only. Game chooses an animation variation supported by your current character. |
| Yoga | On foot only. Game chooses an animation variation supported by your current character. |
| Meditate / picnic | On foot only. Game chooses an animation variation supported by your current character. |
| Drink coffee | On foot only. Game chooses an animation variation supported by your current character. |
| Smoke | On foot only. Game chooses an animation variation supported by your current character. |
| Use binoculars | On foot only. Game chooses an animation variation supported by your current character. |
| Fishing | On foot only. Game chooses an animation variation supported by your current character. |
| Jog in place | On foot only. Game chooses an animation variation supported by your current character. |
| Film with phone | On foot only. Game chooses an animation variation supported by your current character. |
| Tourist camera | On foot only. Game chooses an animation variation supported by your current character. |
| Stand guard | On foot only. Game chooses an animation variation supported by your current character. |
| Weld | On foot only. Game chooses an animation variation supported by your current character. |
| Hammer | On foot only. Game chooses an animation variation supported by your current character. |
| Golf swing | On foot only. Game chooses an animation variation supported by your current character. |

## INTERFACE

| Feature | Behavior |
| --- | --- |
| Panel opacity | Adjust the black translucent panels from 90 to 235. |
| Menu scale | Scale the entire menu from 80% to 105%; narrow screens are constrained to fit. |
| Right side placement | Anchor the menu to the right side of the screen. |
| Animated binary backdrop | Original subtle binary columns behind the interface boxes. |
| Navigation sounds | Play a quiet menu movement sound. |
| Option descriptions | Display help for the selected option in the lower panel. |
| Footer brand mark | Show the small secondary logo in the footer; the title remains in the header. |
| Driving speedometer | Small green-and-black MPH display when the menu is closed. |
| Save interface preferences | Save appearance and navigation preferences for the next launch. Gameplay cheats start off. |
| Reset interface appearance | Restore the default green transparent design. |
| Reset all active effects | Disable continuous effects from this menu and restore tracked states. |

## Available selector groups

### Clothing components

- Face
- Mask
- Hair
- Torso
- Legs
- Bags
- Shoes
- Accessories
- Undershirt
- Armor
- Decals
- Jacket

### Accessory slots

- Hat
- Glasses
- Ears
- Watch
- Bracelet

### Vehicle modification categories

- Spoilers
- Front bumpers
- Rear bumpers
- Side skirts
- Exhausts
- Chassis
- Grilles
- Hoods
- Left fenders
- Right fenders
- Roofs
- Engine
- Brakes
- Transmission
- Horns
- Suspension
- Armor
- Front wheels
- Rear wheels
- Plate holders
- Trim
- Ornaments
- Dials
- Seats
- Steering wheels
- Shifters
- Plaques
- Trunk
- Hydraulics
- Engine blocks
- Air filters
- Strut braces
- Arch covers
- Aerials
- Trim design
- Fuel tanks
- Windows
- Liveries

### Vehicle classes

- All classes
- Compacts
- Sedans
- SUVs
- Coupes
- Muscle
- Sports classics
- Sports
- Super
- Motorcycles
- Off-road
- Industrial
- Utility
- Vans
- Cycles
- Boats
- Helicopters
- Planes
- Service
- Emergency
- Military
- Commercial
- Trains
- Open wheel

### Weather presets

- CLEAR
- EXTRASUNNY
- CLOUDS
- OVERCAST
- RAIN
- THUNDER
- FOGGY
- SMOG
- CLEARING
- NEUTRAL
- SNOW
- BLIZZARD
- SNOWLIGHT
- XMAS
- HALLOWEEN

### Managed traffic types

- Normal road mix
- Supercars
- Sports
- Muscle
- Off-road / SUVs
- Motorcycles
- Commercial
- Chosen spawner model

### Scenarios

- Dance / party
- Street musician
- Human statue
- Push-ups
- Sit-ups
- Yoga
- Meditate / picnic
- Drink coffee
- Smoke
- Use binoculars
- Fishing
- Jog in place
- Film with phone
- Tourist camera
- Stand guard
- Weld
- Hammer
- Golf swing

### Landmarks

- Los Santos Airport
- Vespucci Beach
- Del Perro Pier
- Observatory
- Vinewood sign
- Sandy Shores airfield
- Mount Chiliad summit
- Paleto Bay
- Grapeseed airstrip
- Golf club
- Los Santos Customs
- Casino exterior

## Catalogs, ranges and persistence

- Vehicle catalog: models detected by the installed Enhanced API; names, class/search filters and favorites. Exact-model spawning handles installed add-ons by their model name. Train models are excluded from ordinary spawning.
- Weapon catalog: valid entries from the installed API's WeaponHash enum, with search and equip. This is not an arbitrary custom-weapon importer.
- Cash: 0–2,000,000,000 for supported Story protagonists. Run/swim multipliers: 1.00, 1.15, 1.30 or 1.49×.
- Engine boost: 0–1,000% additional; torque: 100–500%. MPH cap: 0–400, with 0 off. The 180 MPH stopping bug is fixed; 180 MPH is not a built-in cap.
- On-foot FOV: 80–160% in 5% steps, clamped to 45–110 degrees. Tracking follows the gameplay camera position and rotation. Vehicle FOV remains intentionally absent.
- Traffic density: 0–500% target, 10–60 extra-car limit, optional approximately 30 FPS spawn guard. Existing ambient and mission entities are not counted as menu-created traffic.
- Five outfit slots tied to the character model; five saved position slots; persistent interface settings and vehicle favorites.
- Tracked limits: 15 ordinary spawned vehicles and 20 toys/guests; controller traffic is tracked separately. Cleanup protects occupied vehicles.
- Automatic repair preserves momentum while moving; complete bodywork repair waits until nearly stopped. Continuous effects start disabled and have reset controls.
- F5 opens/closes; keyboard and controller navigation, repeat delay, category switching, search/exact input, scrollbar, descriptions, confirmation prompts and status notifications. See README for controls and ADDONS.md for content paths.

