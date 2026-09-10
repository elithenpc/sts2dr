#!/usr/bin/env python3
"""Fetch verified Deltarune battle-sprite art and convert it to card PNGs."""
from __future__ import annotations

import json
import re
import urllib.parse
import urllib.request
from html.parser import HTMLParser
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
OUT = ROOT / "DeltaruneActs" / "images" / "card_portraits"
WIKI = "https://deltarune.wiki"

# These are real article pages which document the corresponding in-battle
# character/spell artwork. We pick an image whose caption/URL identifies the
# character and the relevant ACT/spell, rather than accepting an arbitrary
# search result.
SOURCES = {
    "tp_kris.png": {
        "pages": [("/w/Kris", ["kris", "standard act"]), ("/w/ACT", ["kris", "act"])],
        "required": [["kris"], ["act"]],
    },
    "kris_check.png": {
        "pages": [("/w/ACT", ["kris", "act"]), ("/w/Kris", ["kris", "standard act"])],
        "required": [["kris"], ["act"]],
    },
    "kris_compliment.png": {
        "pages": [("/w/ACT", ["kris", "act"]), ("/w/Kris", ["kris", "standard act"])],
        "required": [["kris"], ["act"]],
    },
    "kris_social.png": {
        "pages": [("/w/ACT", ["kris", "act"]), ("/w/Kris", ["kris", "standard act"])],
        "required": [["kris"], ["act"]],
    },
    "kris_spare.png": {
        "pages": [("/w/ACT", ["kris", "act"]), ("/w/Kris", ["kris", "standard act"])],
        "required": [["kris"], ["act"]],
    },
    "kris_magic.png": {
        "pages": [("/w/Kris", ["kris", "spell"]), ("/w/Spells", ["kris", "spell"])],
        "required": [["kris"], ["spell", "magic", "battle"]],
    },
    "ralsei_healprayer.png": {
        "pages": [("/w/Heal_Prayer", ["ralsei", "heal prayer"]), ("/w/Ralsei", ["ralsei", "battle", "spell"])],
        "required": [["ralsei"], ["heal", "prayer", "spell"]],
    },
    "ralsei_heal.png": {
        "pages": [("/w/Heal_Prayer", ["ralsei", "heal"]), ("/w/Spells", ["ralsei", "spell"])],
        "required": [["ralsei"], ["heal", "spell", "battle"]],
    },
    "susie_rudebuster.png": {
        "pages": [("/w/Rude_Buster", ["susie", "rude buster"]), ("/w/Susie", ["susie", "spell"])],
        "required": [["susie"], ["rude", "buster"]],
    },
    "susie_redbuster.png": {
        "pages": [("/w/RedBuster", ["susie", "redbuster"]), ("/w/Susie", ["susie", "buster"])],
        "required": [["susie"], ["redbuster", "buster"]],
    },
    "dual.png": {
        "pages": [("/w/DualHeal", ["ralsei", "dualheal"]), ("/w/Spells", ["ralsei", "dualheal"])],
        "required": [["ralsei"], ["dual", "heal", "spell"]],
    },
}

IMAGE_EXTENSIONS = (".gif", ".png", ".webp", ".apng", ".jpg", ".jpeg")


class ImageParser(HTMLParser):
    def __init__(self) -> None:
        super().__init__()
        self.images: list[dict[str, str]] = []

    def handle_starttag(self, tag: str, attrs: list[tuple[str, str | None]]) -> None:
        if tag.lower() != "img":
            return
        item = {key.lower(): value or "" for key, value in attrs}
        sources = [item.get("src", ""), item.get("data-src", ""), item.get("data-lazy-src", "")]
        srcset = item.get("srcset", "") or item.get("data-srcset", "")
        if srcset:
            sources.extend(part.strip().split(" ")[0] for part in srcset.split(","))
        for source in sources:
            if source:
                self.images.append({"url": source, "alt": item.get("alt", "")})


def fetch_text(url: str) -> str:
    request = urllib.request.Request(
        url,
        headers={"User-Agent": "sts2dr-sprite-fetcher/3.0"},
    )
    with urllib.request.urlopen(request, timeout=60) as response:
        return response.read().decode("utf-8", errors="replace")


def absolute_url(url: str) -> str:
    return urllib.parse.urljoin(WIKI, url)


def image_candidates(page_path: str) -> list[tuple[str, str]]:
    page_url = absolute_url(page_path)
    parser = ImageParser()
    parser.feed(fetch_text(page_url))
    candidates: list[tuple[str, str]] = []
    seen: set[str] = set()
    for item in parser.images:
        url = absolute_url(item["url"])
        parsed = urllib.parse.urlparse(url)
        path = parsed.path.lower()
        if not path.endswith(IMAGE_EXTENSIONS) or url in seen:
            continue
        seen.add(url)
        candidates.append((url, item["alt"]))
    return candidates


def title_matches_required(text: str, required: list[list[str]]) -> bool:
    lowered = text.lower()
    return all(any(term in lowered for term in group) for group in required)


def choose_image(spec: dict[str, object]) -> tuple[str, str, str] | None:
    required = spec["required"]  # type: ignore[index]
    for page_path, preferred_terms in spec["pages"]:  # type: ignore[index]
        try:
            candidates = image_candidates(page_path)
        except Exception as exc:
            print(f"WARN: could not inspect {page_path}: {exc}")
            continue

        ranked: list[tuple[int, str, str]] = []
        for url, alt in candidates:
            text = f"{alt} {url}"
            if not title_matches_required(text, required):
                continue
            lowered = text.lower()
            score = sum(8 for term in preferred_terms if term.lower() in lowered)
            if "sprite" in lowered:
                score += 5
            if "battle" in lowered:
                score += 4
            if "icon" in lowered or "logo" in lowered:
                score -= 10
            if "concept" in lowered or "artwork" in lowered:
                score -= 8
            ranked.append((score, url, alt))

        if ranked:
            ranked.sort(reverse=True)
            _, url, alt = ranked[0]
            return page_path, alt or url, url

    return None


def download(url: str, destination: Path) -> None:
    request = urllib.request.Request(
        url,
        headers={"User-Agent": "sts2dr-sprite-fetcher/3.0"},
    )
    with urllib.request.urlopen(request, timeout=60) as response:
        data = response.read()
    if not data:
        raise RuntimeError(f"Downloaded an empty sprite from {url}")
    destination.write_bytes(data)


def convert_first_frame(source: Path, destination: Path) -> None:
    """Convert the first animation frame to an RGBA PNG using Pillow."""
    from PIL import Image

    with Image.open(source) as image:
        image.seek(0)
        frame = image.convert("RGBA")
        frame.save(destination, format="PNG", optimize=True)


def main() -> int:
    OUT.mkdir(parents=True, exist_ok=True)
    temp = OUT / ".sprite-download"
    temp.mkdir(exist_ok=True)
    resolved: dict[str, dict[str, str]] = {}

    try:
        for target, spec in SOURCES.items():
            selected = choose_image(spec)
            if not selected:
                raise RuntimeError(f"Could not find a verified battle-sprite image for {target}")

            page_path, caption, url = selected
            parsed_name = Path(urllib.parse.urlparse(url).path).name or "source.bin"
            source = temp / parsed_name
            print(f"{target} <- {page_path} -> {caption}")
            download(url, source)
            convert_first_frame(source, OUT / target)
            resolved[target] = {"page": absolute_url(page_path), "caption": caption, "url": url}

        manifest = {
            "source": WIKI,
            "description": "Real Deltarune Wiki battle/ACT/spell artwork, converted to static PNG first frames.",
            "files": resolved,
        }
        (OUT / "SPRITE_SOURCES.json").write_text(
            json.dumps(manifest, indent=2, ensure_ascii=False) + "\n", encoding="utf-8"
        )
        return 0
    finally:
        for child in temp.iterdir():
            if child.is_file():
                child.unlink()
        temp.rmdir()


if __name__ == "__main__":
    raise SystemExit(main())
