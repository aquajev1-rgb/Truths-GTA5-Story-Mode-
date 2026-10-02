# Development and publication

## Source layout

- `scripts/TruthStoryPlus.3.cs`: the single runtime entry point, targeting C# 5
  and ScriptHookVDotNet API v3.
- `scripts/TruthStoryPlus/logo.png`: original reference artwork. The runtime
  uses an embedded monochrome mask, not a texture loaded from this file.
- `tests/`: small authored game/native stubs plus pure logic, camera/traffic and
  UI/privacy regression tests. Never install this directory into the game.
- `tools/check.ps1`: Windows compile and test runner.
- `tools/package.py`: Python 3 privacy preflight and deterministic source ZIP.

## Check a change

On Windows, from the extracted repository root:

```powershell
# Offline tests, using the .NET Framework C# compiler:
.\tools\check.ps1

# Also compile against your own installed Enhanced API DLL:
.\tools\check.ps1 -ApiPath "<GTA V folder>\ScriptHookVDotNet3.dll"
```

The runner writes only to this repository's `build/` directory. The API DLL is
referenced locally and is never copied into the release. C# 5 compatibility and
warnings-as-errors are enforced. Mock events intentionally have no subscribers;
only their unused-event warning is suppressed in test builds.

Mocks cannot certify native argument semantics, game font metrics, visual
layering, FPS, model support or controller behavior inside GTA. Treat a green
offline run as one layer of checking, not as an in-game certification.

Keep menu/game native calls on the script thread. Key callbacks may only enqueue
input. Continuous effects must remain opt-in and release tracked state. Do not
add online support, anti-cheat workarounds, telemetry or automatic downloads.

The menu currently stays below 330 rectangles including its toast. GTA's native
rectangle budget is shared; other overlays can consume the remaining headroom.
Avoid adding unbounded per-row or per-pixel draw calls. Keep each binary text
string short and explicitly initialize its font scale. The backdrop uses draw
order 3; panels, controls and notifications use order 4.

## Prepare a release

```powershell
python tools/package.py --check
python tools/package.py
```

The packager reads `VERSION` and writes `dist/Truths_GTA5_Story_Mode_Plus_v1.0.0.zip` with an exact allowlist
and a `SHA256SUMS` manifest. Local logs, settings, game files, toolchains, screenshots
and build outputs cannot enter the archive through an unrestricted folder glob.
Files outside the allowlist are reported for review, not silently added.

For future changes and releases (v1.0.0 acceptance is recorded in VALIDATION.md):

- [ ] Preserve the PolyForm Noncommercial license and notices; verify rights to any new assets.
- [ ] Run the compile, tests and privacy preflight; inspect the final ZIP.
- [ ] Test all 14 pages and scrolled states in GTA V Enhanced Story Mode.
- [ ] Test F5, keyboard and controller navigation; confirm input is restored on close.
- [ ] Confirm both logos, small binary text, footer labels, scale and placement.
- [ ] Check optional on-foot FOV, vehicle-entry camera release, MPH limiter and
  random customized spawning in an isolated Story save.
- [ ] Review the staged files and diff for secrets, local paths and runtime state.
- [ ] Enable repository secret scanning where available, and review issues/screenshots
  for private information before making them public.

Do not publish loader binaries, a copy of the game, your save files or a working
installation folder. This package prepares source files only; it does not create
a GitHub repository, push commits or publish a release.

Contributions must be offered under the project's PolyForm Noncommercial 1.0.0
terms. Change `VERSION`, the runtime version/startup message, release notes and
documentation together. Tag this initial public release `v1.0.0`.
