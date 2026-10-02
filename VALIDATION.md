# Validation record — v1.0.0

Release review date: 2026-10-02.

## In-game acceptance

The project owner reported that the UI looked good and confirmed functioning
camera tracking, on-foot first-person FOV and the high-speed/180 MPH stopping fix.
These are released features. The sole reported UI
issue was the startup notification, which this release reformats into two lines.
That final notification change is covered by the offline checks below; it has
not been independently observed inside GTA by the development environment.

## Offline checks

Compilation and test executables were run with a portable Mono compiler/runtime
against the Enhanced API assembly. The supplied Windows PowerShell runner was
reviewed but not executed in a Windows environment here.

- C# 5, x64 compilation against Enhanced API v3.9.0, with warnings treated as
  errors: no errors or warnings.
- Pure-helper tests cover navigation, search, large catalogs, parsing, money and
  coordinate bounds, MPH conversion, limiter convergence and boost behavior.
- High-speed helper cases include 179, 180, 181, 200, 250 and 400 MPH, cap-off,
  below/above-cap and stalled-frame conditions. No abrupt zero-speed result.
- Mocked on-foot/vehicle camera contexts, FOV release during vehicle entry,
  random vehicle customization and traffic population caps.
- Queued F5 open/close, held-key suppression, controller apply and controller back.
- 291 direct native calls across 178 names checked against the native argument
  reference with no argument-count mismatches. This is not a native-semantics test.
- **2,688 menu-plus-toast frames:** all 14 categories, first/last selections,
  80/100/105% scale, left/right placement, keyboard/controller labels, footer
  branding on/off, and 4:3, 16:9, 21:9 and 32:9 aspect ratios.
- Native adapter enforces a 399-rectangle limit. No tested frame dropped a
  rectangle; maximum **313 including the toast**. Tests fail above 330 to retain
  some headroom for other overlays.
- Every displayed string initializes its scale; only short binary strings appear
  on the background layer. Rectangles remain on screen in tested layouts.
- Every tested menu/notification frame checks that `Truths Story Mode+` appears
  in full, without ellipsis, with the status on its own line.
- Logo data contains 36 valid white spans inside a 32×32 mask. Each complete logo
  costs 37 rectangles including its backing, with no external texture dependency.
- Privacy test verifies that exception-message contents do not enter diagnostics.
- Allowlisted source archive, SHA-256 manifest, common secret/private-path checks
  and review of source/documentation. No runtime state or loader binaries shipped.

An offline preview showed complete header/footer badges and readable layout.
The native adapter uses approximate font metrics and simulated calls; it cannot
certify actual rendering, controller mapping, FPS, FOV framing, traffic density
or model behavior. Other overlays share the rectangle budget.

Owner acceptance supplies the in-game confirmation of the existing UI and named
camera/speed features. Future game updates, third-party assets and other mods can
affect compatibility. Automated checks cannot guarantee zero defects or detect
every possible form of sensitive information.
