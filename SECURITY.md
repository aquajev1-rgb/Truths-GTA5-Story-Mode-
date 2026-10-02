# Privacy and safe reporting

The runtime script contains no HTTP client, telemetry, updater, credentials,
remote command channel, or account integration. It interacts with the game and
writes local menu state only.

Local data includes interface preferences, vehicle favorites, saved game-world
coordinates, outfit slots and a diagnostic log under `scripts/TruthStoryPlus/`.
These files are excluded from the release allowlist and from normal Git adds.
Ignored files already tracked by Git must still be removed from the index before
publication. Review the actual staged diff, not only `.gitignore`.

Menu diagnostics record timestamps, version, operation names, exception types
and error codes. They deliberately omit exception messages and stack traces.
Other mods and the script loader maintain their own logs and may disclose paths,
account names, installed software or other private information.

Before reporting a bug, inspect every attachment. Share only a sanitized excerpt,
the game/loader versions, enabled menu options and reproduction steps. Crop
screenshots that show unrelated windows, notifications or personal information.
Do not post an entire game folder, save file, credentials or unreviewed logs.

Run `python tools/package.py --check` before each release. This applies a strict
file allowlist and searches common credential and private-path patterns. Pattern
checks are not proof that arbitrary sensitive information is absent; review
source changes and the final archive manually too.

Use only in offline Story Mode. Network-session checks are a safety gate, not an
anti-cheat bypass or an assurance that a modded installation is safe online.

