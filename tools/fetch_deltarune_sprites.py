#!/usr/bin/env python3
"""Fetch verified Deltarune battle-sprite art and convert it to card PNGs."""
from __future__ import annotations

import json
import subprocess
import sys
import urllib.parse
import urllib.request
from pathlib import Path

API = "https://deltarune.wiki/api.php"
ROOT = Path(__file__).resolve().parents[1]
OUT = ROOT / "DeltaruneActs" / "images" / "card_portraits"

# Each entry has real Deltarune Wiki image-search phrases plus terms that must
# appear in the resolved Wiki file title. This prevents an unrelated portrait
# or concept-art file from silently becoming card art.
SOURCES = {
    "tp_kris.png": {
        "phrases": ["Kris battle act", "Kris ACT battle"],
        "required": [["kris"], ["battle", "act"]],
    },
    "kris_check.png": {
        "phrases": ["Kris Check battle", "Kris battle Check", "Kris ACT Check"],
        "required": [["kris"], ["battle", "act"], ["check"]],
    },
    "kris_compliment.png": {
        "phrases": ["Kris Compliment battle", "Kris battle Compliment", "Kris ACT Compliment"],
        "required": [["kris"], ["battle", "act"], ["compliment"]],
    },
    "kris_social.png": {
        "phrases": ["Kris ACT battle", "Kris battle act"],
        "required": [["kris"], ["battle", "act"]],
    },
    "kris_spare.png": {
        "phrases": ["Kris Spare battle", "Kris battle Spare", "Kris ACT Spare"],
        "required": [["kris"], ["battle", "act"], ["spare"]],
    },
    "kris_magic.png": {
        "phrases": ["Kris battle spell", "Kris battle magic"],
        "required": [["kris"], ["battle", "spell", "magic"]],
    },
    "ralsei_healprayer.png": {
        "phrases": ["Ralsei Heal Prayer battle", "Ralsei battle Heal Prayer", "Ralsei battle spell"],
        "required": [["ralsei"], ["battle", "spell"], ["heal", "prayer"]],
    },
    "ralsei_heal.png": {
        "phrases": ["Ralsei healing battle", "Ralsei battle healing", "Ralsei battle spell"],
        "required": [["ralsei"], ["battle", "spell"], ["heal", "healing"]],
    },
    "susie_rudebuster.png": {
        "phrases": ["Susie Rude Buster", "Susie battle Rude Buster", "Susie battle spell"],
        "required": [["susie"], ["rude", "buster"], ["battle", "spell"]],
    },
    "susie_redbuster.png": {
        "phrases": ["Susie RedBuster", "Susie battle RedBuster", "Susie battle spell"],
        "required": [["susie"], ["redbuster", "red", "buster"], ["battle", "spell"]],
    },
    "dual.png": {
        "phrases": ["DualHeal battle", "DualBuster battle", "Ralsei battle spell", "Susie battle spell"],
        "required": [["ralsei", "susie"], ["battle", "spell", "dual"]],
    },
}


def api_json(params: dict[str, str]) -> dict:
    query = urllib.parse.urlencode({**params, "format": "json", "origin": "*"})
    request = urllib.request.Request(
        f"{API}?{query}",
        headers={"User-Agent": "sts2dr-sprite-fetcher/2.0"},
    )
    with urllib.request.urlopen(request, timeout=30) as response:
        return json.load(response)


def title_matches_required(title: str, required: list[list[str]]) -> bool:
    lowered = title.lower()
    return all(any(term in lowered for term in group) for group in required)


def search_file(spec: dict[str, object]) -> tuple[str, str] | None:
    for phrase in spec["phrases"]:  # type: ignore[index]
        data = api_json({
            "action": "query",
            "list": "search",
            "srnamespace": "6",
            "srsearch": str(phrase),
            "srlimit": "30",
        })
        hits = data.get("query", {}).get("search", [])
        if not hits:
            continue

        required = spec["required"]  # type: ignore[index]
        candidates: list[tuple[int, str]] = []
        wanted = [word.lower() for word in str(phrase).split()]
        for hit in hits:
            title = hit["title"]
            lower = title.lower()
            if not title_matches_required(title, required):
                continue

            score = sum(3 for word in wanted if word in lower)
            if lower.endswith((".gif", ".png", ".webp", ".apng")):
                score += 8
            if "sprite" in lower or "battle" in lower:
                score += 4
            if "concept" in lower or "artwork" in lower or "portrait" in lower:
                score -= 12
            candidates.append((score, title))

        if not candidates:
            continue

        candidates.sort(reverse=True)
        title = candidates[0][1]
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
    request = urllib.request.Request(url, headers={"User-Agent": "sts2dr-sprite-fetcher/2.0"})
    with urllib.request.urlopen(request, timeout=60) as response:
        data = response.read()
    if not data:
        raise RuntimeError(f"Downloaded an empty sprite from {url}")
    destination.write_bytes(data)


def convert_first_frame(source: Path, destination: Path) -> None:
    """Convert the first animation frame into a normal RGBA PNG."""
    try:
        from PIL import Image
    except ImportError:
        print("Pillow is required to convert Deltarune sprite animations.", file=sys.stderr)
        raise SystemExit(2)

    try:
        with Image.open(source) as image:
            image.seek(0)
            frame = image.convert("RGBA")
            frame.save(destination, format="PNG", optimize=True)
    except Exception as exc:
        raise RuntimeError(f"Could not convert {source.name} to PNG: {exc}") from exc


def main() -> int:
    OUT.mkdir(parents=True, exist_ok=True)
    temp = OUT / ".sprite-download"
    temp.mkdir(exist_ok=True)

    resolved: dict[str, str] = {}
    try:
        for target, spec in SOURCES.items():
            result = search_file(spec)
            if not result:
                raise RuntimeError(f"Could not find a verified Deltarune Wiki sprite for {target}")

            title, url = result
            parsed_name = Path(urllib.parse.urlparse(url).path).name or "source.bin"
            source = temp / parsed_name
            print(f"{target} <- {title}")
            download(url, source)
            convert_first_frame(source, OUT / target)
            resolved[target] = title

        manifest = {
            "source": "https://deltarune.wiki/",
            "description": "Real Deltarune Wiki battle-sprite files, converted to static PNG first frames.",
            "files": resolved,
        }
        (OUT / "SPRITE_SOURCES.json").write_text(
            json.dumps(manifest, indent=2) + "\n", encoding="utf-8"
        )
        return 0
    finally:
        for child in temp.iterdir():
            if child.is_file():
                child.unlink()
        temp.rmdir()


if __name__ == "__main__":
    raise SystemExit(main())
