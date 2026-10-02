"""Build an allowlisted, deterministic source release. Requires Python 3.8+."""

import argparse
import hashlib
from pathlib import Path
import re
import struct
import sys
import zipfile

ROOT = Path(__file__).resolve().parents[1]
FILES = (
    ".editorconfig", ".gitattributes", ".gitignore",
    "README.md", "CHANGELOG.md", "CONTRIBUTING.md", "SECURITY.md",
    "LICENSE-NOTICE.md", "LICENSE.md", "VALIDATION.md", "VERSION",
    "FEATURES.md", "ADDONS.md", "SOURCES.md", "RELEASE.md",
    "scripts/TruthStoryPlus.3.cs", "scripts/TruthStoryPlus/logo.png",
    "tests/TruthTests.cs", "tests/CameraTrafficTests.cs", "tests/UiTests.cs",
    "tests/MockGta.cs", "tools/check.ps1", "tools/package.py", "tools/astyle.options",
)
PATTERNS = (
    ("private key", re.compile(r"-----BEGIN (?:RSA |EC |OPENSSH )?PRIVATE KEY-----")),
    ("GitHub token", re.compile(r"\b(?:gh[pousr]_[A-Za-z0-9]{32,}|github_pat_[A-Za-z0-9_]{40,})\b")),
    ("cloud access key", re.compile(r"\b(?:AKIA|ASIA)[A-Z0-9]{16}\b")),
    ("service token", re.compile(r"\b(?:sk-[A-Za-z0-9_-]{24,}|xox[baprs]-[A-Za-z0-9-]{20,})\b")),
    ("private local path", re.compile(r"(?:[A-Za-z]:\\|/(?:workspace|Users|home|root)/)")),
    ("stored secret", re.compile(r"(?im)^\s*(?:api[_-]?key|password|access[_-]?token|client[_-]?secret)\s*[:=]\s*[\"']?\S{8,}")),
)


def check_png(data):
    """Reject embedded textual metadata or trailing data."""
    if not data.startswith(b"\x89PNG\r\n\x1a\n"):
        raise ValueError("Logo is not a PNG")
    offset = 8
    allowed = {b"IHDR", b"IDAT", b"IEND", b"PLTE", b"tRNS", b"sRGB", b"gAMA", b"cHRM", b"pHYs"}
    while offset + 12 <= len(data):
        size = struct.unpack(">I", data[offset:offset + 4])[0]
        kind = data[offset + 4:offset + 8]
        if kind not in allowed or offset + size + 12 > len(data):
            raise ValueError("Logo contains unexpected metadata or malformed chunks")
        offset += size + 12
        if kind == b"IEND":
            if offset != len(data):
                raise ValueError("Logo contains trailing data")
            return
    raise ValueError("Incomplete PNG")


def collect():
    payload = {}
    for name in FILES:
        path = ROOT / name
        if path.is_symlink() or not path.is_file():
            raise ValueError("Missing or symlinked release file: " + name)
        path.resolve().relative_to(ROOT)
        data = path.read_bytes()
        if path.suffix == ".png":
            check_png(data)
        else:
            content = data.decode("utf-8")
            for label, pattern in PATTERNS:
                if pattern.search(content):
                    # Never echo a possible credential into the log.
                    raise ValueError("Privacy check: " + label + " in " + name)
        payload[name] = data
    extras = []
    for path in ROOT.rglob("*"):
        relative = path.relative_to(ROOT)
        if path.is_file() and relative.as_posix() not in FILES:
            if relative.parts[0] not in {".git", "build", "dist", ".vs", ".idea", ".vscode"} and "__pycache__" not in relative.parts:
                extras.append(relative.as_posix())
    if extras:
        # Filenames themselves may be private; report a count only.
        print("NOTICE: {} non-allowlisted local file(s) excluded; review before committing.".format(len(extras)))
    return payload


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--check", action="store_true", help="Validate without writing an archive")
    args = parser.parse_args()
    payload = collect()
    version = payload["VERSION"].decode("utf-8").strip()
    if not re.fullmatch(r"\d+\.\d+\.\d+", version):
        raise ValueError("VERSION must contain a three-part numeric release version")
    if 'Version = "' + version + ' / RELEASE"' not in payload["scripts/TruthStoryPlus.3.cs"].decode("utf-8"):
        raise ValueError("Runtime version does not match VERSION")
    print("PASS: {} allowlisted files; no matched secret/private-path patterns.".format(len(payload)))
    if args.check:
        return
    manifest = "".join(hashlib.sha256(data).hexdigest() + "  " + name + "\n" for name, data in sorted(payload.items()))
    payload["SHA256SUMS"] = manifest.encode("utf-8")
    output = ROOT / "dist" / ("Truths_GTA5_Story_Mode_Plus_v" + version + ".zip")
    output.parent.mkdir(exist_ok=True)
    with zipfile.ZipFile(output, "w", compression=zipfile.ZIP_DEFLATED, compresslevel=9) as archive:
        for name, data in sorted(payload.items()):
            info = zipfile.ZipInfo(name, date_time=(1980, 1, 1, 0, 0, 0))
            info.compress_type = zipfile.ZIP_DEFLATED
            info.create_system = 3
            info.external_attr = 0o100644 << 16
            archive.writestr(info, data)
    print("Created dist/" + output.name)
    print("SHA256 " + hashlib.sha256(output.read_bytes()).hexdigest())


if __name__ == "__main__":
    try:
        main()
    except (OSError, UnicodeError, ValueError) as error:
        print("FAILED: " + str(error), file=sys.stderr)
        sys.exit(1)
