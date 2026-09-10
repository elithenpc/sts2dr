#!/usr/bin/env python3
"""Generate card portraits from the actual Deltarune battle sprite animations."""
from __future__ import annotations

import json
import re
import subprocess
from pathlib import Path
from urllib.request import Request, urlopen

ROOT = Path(__file__).resolve().parents[1]
OUT = ROOT / "DeltaruneActs" / "images" / "card_portraits"
BIG = OUT / "big"

# DeltamodKit contains the extracted Deltarune GameMaker battle sprites.
# The exact commit is pinned so the generated art is reproducible.
DELTAMODKIT_COMMIT = "aff71f3427fd764ca7b05ad8f93e660975fe6def"
DELTAMODKIT = ROOT / ".deltamodkit"
DELTAMODKIT_URL = "https://github.com/deltamodders/deltamodkit.git"

# Every generated file has one explicit source sprite. These are real in-game
# battle animations, not fan-made recreations.
SOURCES = {
    "kris_act.png": {
        "sprite": "spr_krisb_act",
        "character": "Kris",
        "action": "ACT",
    },
    "ralsei_spell.png": {
        "sprite": "spr_ralsei_spell",
        "character": "Ralsei",
        "action": "spell casting",
    },
    "susie_spell.png": {
        "sprite": "spr_susieb_spell",
        "character": "Susie",
        "action": "spell casting",
    },
    "noelle_spell.png": {
        "sprite": "spr_noelleb_spell",
        "character": "Noelle",
        "action": "spell casting",
    },
}

# Card -> exact in-game battle animation category.
CARD_ART = {
    "check": "kris_act",
    "compliment": "kris_act",
    "warn": "kris_act",
    "joke": "kris_act",
    "flirt": "kris_act",
    "spare": "kris_act",
    "healprayer": "ralsei_spell",
    "dualheal": "ralsei_spell",
    "pacify": "ralsei_spell",
    "revivekris": "ralsei_spell",
    "revivesong": "ralsei_spell",
    "lightup": "ralsei_spell",
    "healing": "susie_spell",
    "okayheal": "susie_spell",
    "betterheal": "susie_spell",
    "ultraheal": "susie_spell",
    "ultimateheal": "susie_spell",
    "rudebuster": "susie_spell",
    "redbuster": "susie_spell",
    "dualbuster": "susie_spell",
    "rudesword": "susie_spell",
    "scythemare": "susie_spell",
    "wakekris": "susie_spell",
    # Noelle is the caster for all three of these. In the real game the
    # caster pose comes from her spell animation while the spell effect is a
    # separate layer.
    "iceshock": "noelle_spell",
    "snowgrave": "noelle_spell",
    "sleepmist": "noelle_spell",
}

EXPECTED = set(CARD_ART)


def run(cmd: list[str], cwd: Path | None = None) -> None:
    print("$", " ".join(cmd))
    subprocess.run(cmd, cwd=cwd, check=True)


def checkout_source_repo() -> None:
    if not DELTAMODKIT.exists():
        run(["git", "clone", "--filter=blob:none", DELTAMODKIT_URL, str(DELTAMODKIT)])
    run(["git", "fetch", "--depth", "1", "origin", DELTAMODKIT_COMMIT], cwd=DELTAMODKIT)
    run(["git", "checkout", "--force", DELTAMODKIT_COMMIT], cwd=DELTAMODKIT)


def load_frames(sprite_name: str) -> list[str]:
    yy = DELTAMODKIT / "sprites" / sprite_name / f"{sprite_name}.yy"
    if not yy.exists():
        raise RuntimeError(f"Missing DeltamodKit sprite definition: {yy}")

    text = yy.read_text(encoding="utf-8")
    match = re.search(r'"frames":\[(.*?)\],\s*"gridX"', text, flags=re.DOTALL)
    if not match:
        raise RuntimeError(f"Could not parse frame list from {yy}")

    frames = re.findall(r'"%Name":"([0-9a-f-]+)"', match.group(1), flags=re.IGNORECASE)
    if not frames:
        raise RuntimeError(f"No frames found in {yy}")

    return frames


def download_bytes(url: str) -> bytes:
    request = Request(url, headers={"User-Agent": "sts2dr-deltamodkit-art/1.0"})
    with urlopen(request, timeout=60) as response:
        data = response.read()
    if not data:
        raise RuntimeError(f"Empty download: {url}")
    return data


def download_frame(sprite_name: str, frame_id: str, destination: Path) -> str:
    source_name = f"{frame_id}.png"
    local = DELTAMODKIT / "sprites" / sprite_name / source_name
    if not local.exists():
        raise RuntimeError(f"Missing frame image {local}")
    destination.write_bytes(local.read_bytes())
    return f"https://raw.githubusercontent.com/deltamodders/deltamodkit/{DELTAMODKIT_COMMIT}/sprites/{sprite_name}/{source_name}"


def make_big(source: Path, destination: Path) -> None:
    from PIL import Image

    with Image.open(source) as image:
        image = image.convert("RGBA")
        # Nearest-neighbour preserves the crisp pixel-art edges.
        scale = 4
        enlarged = image.resize((image.width * scale, image.height * scale), Image.Resampling.NEAREST)
        enlarged.save(destination, format="PNG", optimize=True)


def main() -> int:
    OUT.mkdir(parents=True, exist_ok=True)
    BIG.mkdir(parents=True, exist_ok=True)
    checkout_source_repo()

    source_meta: dict[str, dict[str, str]] = {}
    generated = set()

    for category, spec in SOURCES.items():
        frames = load_frames(spec["sprite"])
        # Use the middle frame of the actual animation as the static card art.
        frame_id = frames[len(frames) // 2]
        target = OUT / category
        source_url = download_frame(spec["sprite"], frame_id, target)
        make_big(target, BIG / category)
        source_meta[category] = {
            "source_repository": "https://github.com/deltamodders/deltamodkit",
            "source_commit": DELTAMODKIT_COMMIT,
            "sprite": spec["sprite"],
            "character": spec["character"],
            "action": spec["action"],
            "frame": frame_id,
            "frame_url": source_url,
        }
        print(f"Generated {category}: {spec['character']} / {spec['action']} / frame {frame_id}")

    # Produce one manifest entry per card so it is possible to audit exactly
    # which real animation family each card displays.
    manifest_cards: dict[str, dict[str, str]] = {}
    for card, category in sorted(CARD_ART.items()):
        manifest_cards[card] = {
            "asset": f"{category}.png",
            "big_asset": f"big/{category}.png",
            "character": source_meta[f"{category}.png"]["character"],
            "action": source_meta[f"{category}.png"]["action"],
            "sprite": source_meta[f"{category}.png"]["sprite"],
            "frame": source_meta[f"{category}.png"]["frame"],
            "source_url": source_meta[f"{category}.png"]["frame_url"],
        }
        generated.add(card)

    if generated != EXPECTED:
        raise RuntimeError(f"Card asset coverage mismatch. Missing={sorted(EXPECTED - generated)} extra={sorted(generated - EXPECTED)}")

    manifest = {
        "description": "Verified Deltarune in-game battle sprite frames extracted from DeltamodKit.",
        "source_repository": "https://github.com/deltamodders/deltamodkit",
        "source_commit": DELTAMODKIT_COMMIT,
        "note": "Cards are static, so each portrait uses the middle frame of the real battle animation. Both normal and big portraits come from that same frame.",
        "cards": manifest_cards,
    }
    (OUT / "SPRITE_SOURCES.json").write_text(
        json.dumps(manifest, indent=2, ensure_ascii=False) + "\n", encoding="utf-8"
    )
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
