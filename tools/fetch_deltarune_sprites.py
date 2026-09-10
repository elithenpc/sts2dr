#!/usr/bin/env python3
"""Fetch the real Deltarune battle-sprite animations used by the card art.

The sprite files themselves are not bundled by this script's source code. They are
retrieved from their original Deltarune Wiki file pages into the local mod build
folder. The GitHub Actions workflow uses this script to prepare local build assets.
"""
from __future__ import annotations

import json
import os
import subprocess
import sys
import urllib.parse
import urllib.request
from pathlib import Path

API = "https://deltarune.wiki/api.php"
ROOT = Path(__file__).resolve().parents[1]
OUT = ROOT / "DeltaruneActs" / "images" / "card_portraits"

# Each target maps to the most specific file-search phrases first.  The wiki's
# MediaWiki API resolves the current canonical filename, so the repo does not
# depend on a hard-coded image URL changing.
SOURCES = {
    "kris_check.png": ["Kris battle act", "Kris ACT battle"],
    "kris_compliment.png": ["Kris battle act", "Kris ACT battle"],
    "kris_social.png": ["Kris battle act", "Kris ACT battle"],
    "kris_spare.png": ["Kris battle act", "Kris ACT battle"],
    "kris_magic.png": ["Kris battle spell", "Kris battle magic"],
    "ralsei_healprayer.png": ["Ralsei battle spell", "Ralsei battle spell action"],
    "ralsei_heal.png": ["Ralsei battle spell", "Ralsei battle spell action"],
    "susie_rudebuster.png": ["Susie Rude Buster", "Susie battle Rude Buster", "Susie battle spell"],
    "susie_redbuster.png": ["Susie RedBuster", "Susie battle Rude Buster", "Susie battle spell"],
    "dual.png": ["Ralsei battle spell", "Susie battle Rude Buster"],
}


def api_json(params: dict[str, str]) -> dict:
    query = urllib.parse.urlencode({**params, "format": "json", "origin": "*"})
    with urllib.request.urlopen(f"{API}?{query}", timeout=30) as response:
        return json.load(response)


def search_file(phrase: str) -> tuple[str, str] | None:
    data = api_json({
        "action": "query",
        "list": "search",
        "srnamespace": "6",
        "srsearch": phrase,
        "srlimit": "10",
    })
    hits = data.get("query", {}).get("search", [])
    if not hits:
        return None

    # Prefer titles that contain the important action/character keywords.
    wanted = [w.lower() for w in phrase.split()]
    ranked: list[tuple[int, str]] = []
    for hit in hits:
        title = hit["title"]
        score = sum(2 if word in title.lower() else 0 for word in wanted)
        if title.lower().endswith((".gif", ".png", ".webp")):
            score += 5
        ranked.append((score, title))
    ranked.sort(reverse=True)
    title = ranked[0][1]

    info = api_json({
        "action": "query",
        "prop": "imageinfo",
        "titles": title,
        "iiprop": "url",
    })
    pages = info.get("query", {}).get("pages", {})
    for page in pages.values():
        imageinfo = page.get("imageinfo") or []
        if imageinfo:
            return title, imageinfo[0]["url"]
    return None


def download(url: str, destination: Path) -> None:
    request = urllib.request.Request(
        url,
        headers={"User-Agent": "sts2dr-sprite-fetcher/1.0"},
    )
    with urllib.request.urlopen(request, timeout=60) as response:
        destination.write_bytes(response.read())


def convert_first_frame(source: Path, destination: Path) -> None:
    # ImageMagick is used only by the local build/workflow environment so the
    # card portrait is a regular PNG that Godot can import reliably.
    try:
        subprocess.run(
            ["magick", f"{source}[0]", str(destination)],
            check=True,
            stdout=subprocess.PIPE,
            stderr=subprocess.PIPE,
            text=True,
        )
    except FileNotFoundError:
        print("ImageMagick ('magick') is required to convert the sprite animation.", file=sys.stderr)
        raise SystemExit(2)


def main() -> int:
    OUT.mkdir(parents=True, exist_ok=True)
    temp = OUT / ".sprite-download"
    temp.mkdir(exist_ok=True)

    resolved: dict[str, str] = {}
    for target, phrases in SOURCES.items():
        result = None
        for phrase in phrases:
            result = search_file(phrase)
            if result:
                break
        if not result:
            raise RuntimeError(f"Could not find a Deltarune Wiki sprite for {target}: {phrases}")

        title, url = result
        source = temp / (Path(urllib.parse.urlparse(url).path).name or "source.gif")
        print(f"{target} <- {title}")
        download(url, source)
        convert_first_frame(source, OUT / target)
        resolved[target] = title

    manifest = {
        "source": "https://deltarune.wiki/",
        "files": resolved,
    }
    (OUT / "SPRITE_SOURCES.json").write_text(json.dumps(manifest, indent=2) + "\n", encoding="utf-8")

    for child in temp.iterdir():
        child.unlink()
    temp.rmdir()
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
