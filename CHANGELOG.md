# Changelog

## 1.0.0 — First public release

Public versioning starts at v1.0.0; the previous 2.x numbers were private builds.

- Fixed the startup/status notification: full-width `Truths Story Mode+` heading
  above the message; startup identifies `Loaded v1.0.0. Press F5 to open.`.
- Promoted camera tracking, on-foot FOV and the high-speed stopping fix to release
  status based on the project owner's successful in-game checks.
- Selected PolyForm Noncommercial 1.0.0 for modifiable, noncommercial source sharing.
- Added every menu row, add-on content paths, author-source citations, version
  metadata and GitHub release notes.

### UI and code cleanup retained

- Fixed the footer logo's native drawing-budget problem. Both logos now use
  compact embedded monochrome masks: 37 rectangles each instead of 201.
- Added a larger, bordered footer badge and the full menu name beside it.
- Reduced gradient bands without changing the green-and-black layout.
- Assigned the binary backdrop a lower graphics layer than all menu boxes.
- Kept per-string text sizing to prevent the earlier oversized-binary regression.
- Corrected empty-result help and zero-item counts; bounded queued keyboard input.
- Expired confirmation prompts now clear automatically.
- Added a direct vehicle check to the on-foot FOV guard, covering camera-context
  transitions. Vehicle FOV remains removed.
- Rejected invalid negative vehicle classes in the traffic model filter.
- Reformatted the complete script, removed unused imports and documented sections.
- Error diagnostics no longer serialize exception messages or stack traces,
  which can contain local account names and paths.
- Added offline UI/gameplay tests, build instructions, a release allowlist,
  ignore rules, privacy checks and a publishing checklist.

### Stable backdrop and random builds retained

- Restored the short, 18-column binary backdrop.
- Removed vehicle FOV; retained opt-in on-foot first-person FOV.
- Added random vehicle spawning with randomized cosmetics and supported
  maximum-performance upgrades.

## Earlier retained functionality

- Traffic controller with density targets, selected traffic types and a spawn cap.
- Smooth MPH limiter and horn boost that do not snap high-speed cars to a stop.
- Player, cash, clothing, weapons, vehicle tuning, teleport and sandbox controls.
