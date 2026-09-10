#!/usr/bin/env python3
"""Fetch verified Deltarune battle/action/spell art and convert it to card PNGs."""
from __future__ import annotations

import json
import urllib.parse
import urllib.request
from html.parser import HTMLParser
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
OUT = ROOT / "DeltaruneActs" / "images" / "card_portraits"
WIKI = "https://deltarune.wiki"

# IMPORTANT: filenames here match DeltaruneActsCard.CardName(Id.Entry), which
# means a card named IceShock must produce iceshock.png, RudeBuster must
# produce rudebuster.png, etc. Each entry is restricted to the exact
# Deltarune Wiki gallery/action caption needed for that card.
SOURCES = {
    "check.png": {"pages": [("/w/Kris", ["standard act"])], "required": [["kris"], ["standard act"]]},
    "compliment.png": {"pages": [("/w/Kris", ["standard act"])], "required": [["kris"], ["standard act"]]},
    "spare.png": {"pages": [("/w/Kris", ["standard act"])], "required": [["kris"], ["standard act"]]},
    "flirt.png": {"pages": [("/w/Kris", ["flirt", "japanese school uniforms"])], "required": [["kris"], ["flirt"]]},
    "kris_magic.png": {"pages": [("/w/Kris", ["standard act"])], "required": [["kris"], ["standard act"]]},

    "healprayer.png": {"pages": [("/w/Heal_Prayer", ["ralsei casting heal prayer"])], "required": [["ralsei"], ["heal prayer"]]},
    "dualheal.png": {"pages": [("/w/Dual_Heal", ["ralsei casting dualheal"])], "required": [["ralsei"], ["dualheal"]]},
    "pacify.png": {"pages": [("/w/Pacify", ["ralsei", "pacify"])], "required": [["ralsei"], ["pacify"]]},
    "revivesong.png": {"pages": [("/w/ReviveSong", ["ralsei", "revivesong"])], "required": [["ralsei"], ["revivesong"]]},
    "revivekris.png": {"pages": [("/w/ReviveKris", ["ralsei", "revivekris"])], "required": [["ralsei"], ["revivekris"]]},

    "rudebuster.png": {"pages": [("/w/Rude_Buster", ["susie using rude buster"])], "required": [["susie"], ["rude buster"]]},
    "redbuster.png": {"pages": [("/w/RedBuster", ["susie using redbuster"])], "required": [["susie"], ["redbuster"]]},
    "dualbuster.png": {"pages": [("/w/DualBuster", ["dualbuster", "susie"])], "required": [["dualbuster"], ["susie"]]},
    "scythemare.png": {"pages": [("/w/Scythemare", ["scythemare", "susie"])], "required": [["scythemare"], ["susie"]]},
    "wakekris.png": {"pages": [("/w/WakeKris", ["susie", "wake kris"])], "required": [["susie"], ["wake"]]},
    "healing.png": {"pages": [("/w/Susie%27s_Healing", ["susie casting her healing magic"])], "required": [["susie"], ["healing magic"]]},
    "okayheal.png": {"pages": [("/w/Susie%27s_Healing", ["susie casting her healing magic"])], "required": [["susie"], ["healing magic"]]},
    "betterheal.png": {"pages": [("/w/Susie%27s_Healing", ["susie casting her healing magic"])], "required": [["susie"], ["healing magic"]]},
    "ultraheal.png": {"pages": [("/w/Susie%27s_Healing", ["susie casting her healing magic"])], "required": [["susie"], ["healing magic"]]},
    "ultimateheal.png": {"pages": [("/w/Susie%27s_Healing", ["susie casting her healing magic"])], "required": [["susie"], ["healing magic"]]},

    # These two are intentionally Noelle, not a generic ice spell or Kris art.
    "iceshock.png": {"pages": [("/w/IceShock", ["noelle casting iceshock"])], "required": [["noelle"], ["iceshock"]]},
    "snowgrave.png": {"pages": [("/w/SnowGrave", ["noelle casting snowgrave"])], "required": [["noelle"], ["snowgrave"]]},
    "sleepmist.png": {"pages": [("/w/Sleep_Mist", ["sleep mist", "noelle"])], "required": [["noelle"], ["sleep mist"]]},

    "tp_kris.png": {"pages": [("/w/Kris", ["standard act"])], "required": [["kris"], ["standard act"]]},
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
    request = urllib.request.Request(url, headers={"User-Agent": "sts2dr-sprite-fetcher/4.0"})
    with urllib.request.urlopen(request, timeout=60) as response:
        return response.read().decode("utf-8", errors="replace")


def absolute_url(url: str) -> str:
    return urllib.parse.urljoin(WIKI, url)


def image_candidates(page_path: str) -> list[tuple[str, str]]:
    parser = ImageParser()
    parser.feed(fetch_text(absolute_url(page_path)))
    candidates: list[tuple[str, str]] = []
    seen: set[str] = set()
    for item in parser.images:
        url = absolute_url(item["url"])
        path = urllib.parse.urlparse(url).path.lower()
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
            score = sum(20 for term in preferred_terms if term.lower() in lowered)
            if "sprite" in lowered:
                score += 5
            if "battle" in lowered:
                score += 4
            if "icon" in lowered or "logo" in lowered:
                score -= 20
            if "concept" in lowered or "artwork" in lowered:
                score -= 20
            ranked.append((score, url, alt))

        if ranked:
            ranked.sort(reverse=True)
            _, url, alt = ranked[0]
            return page_path, alt or url, url

    return None


def download(url: str, destination: Path) -> None:
    request = urllib.request.Request(url, headers={"User-Agent": "sts2dr-sprite-fetcher/4.0"})
    with urllib.request.urlopen(request, timeout=60) as response:
        data = response.read()
    if not data:
        raise RuntimeError(f"Downloaded an empty sprite from {url}")
    destination.write_bytes(data)


def convert_representative_frame(source: Path, destination: Path) -> None:
    """Save a representative action frame, preferring the middle of animations."""
    from PIL import Image

    with Image.open(source) as image:
        try:
            frame_count = getattr(image, "n_frames", 1)
        except Exception:
            frame_count = 1
        frame_index = max(0, frame_count // 2)
        try:
            image.seek(frame_index)
        except EOFError:
            image.seek(0)
        image.convert("RGBA").save(destination, format="PNG", optimize=True)


def main() -> int:
    OUT.mkdir(parents=True, exist_ok=True)
    temp = OUT / ".sprite-download"
    temp.mkdir(exist_ok=True)
    resolved: dict[str, dict[str, str]] = {}

    try:
        for target, spec in SOURCES.items():
            selected = choose_image(spec)
            if not selected:
                raise RuntimeError(f"Could not find the exact verified Deltarune artwork for {target}")

            page_path, caption, url = selected
            parsed_name = Path(urllib.parse.urlparse(url).path).name or "source.bin"
            source = temp / parsed_name
            print(f"{target} <- {page_path} -> {caption}")
            download(url, source)
            convert_representative_frame(source, OUT / target)
            resolved[target] = {"page": absolute_url(page_path), "caption": caption, "url": url}

        manifest = {
            "source": WIKI,
            "description": "Exact Deltarune Wiki battle/action/spell artwork. Animated sources use a representative action frame.",
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
