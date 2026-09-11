#!/usr/bin/env python3
"""Generate one individual card portrait for every Deltarune Acts card.

The source pixels come from real Deltarune battle sprite frames in the
DeltamodKit extraction. Every card receives its own PNG and its own matching
4x PNG under images/card_portraits/big/.
"""
from __future__ import annotations

import json
import re
import shutil
import subprocess
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
OUT = ROOT / "DeltaruneActs" / "images" / "card_portraits"
BIG = OUT / "big"

DELTAMODKIT_COMMIT = "aff71f3427fd764ca7b05ad8f93e660975fe6def"
DELTAMODKIT = ROOT / ".deltamodkit"
DELTAMODKIT_URL = "https://github.com/deltamodders/deltamodkit.git"

# Real in-game Deltarune battle sprite families.
SOURCES = {
    "kris_act": {"sprite": "spr_krisb_act", "character": "Kris", "animation": "ACT"},
    "ralsei_spell": {"sprite": "spr_ralsei_spell", "character": "Ralsei", "animation": "spell"},
    "ralsei_spellready": {"sprite": "spr_ralsei_spellready", "character": "Ralsei", "animation": "spell ready"},
    "susie_spell": {"sprite": "spr_susieb_spell", "character": "Susie", "animation": "spell"},
    "susie_spellready": {"sprite": "spr_susieb_spellready", "character": "Susie", "animation": "spell ready"},
    "susie_act": {"sprite": "spr_susieb_act", "character": "Susie", "animation": "ACT"},
    "susie_actready": {"sprite": "spr_susieb_actready", "character": "Susie", "animation": "ACT ready"},
    "noelle_spell": {"sprite": "spr_noelleb_spell", "character": "Noelle", "animation": "spell"},
    "noelle_spellready": {"sprite": "spr_noelleb_spellready", "character": "Noelle", "animation": "spell ready"},
}

# Each card has its own output filename and its own source-frame selection.
# These are not shared output assets. Where the game has multiple battle
# sprite families for the same character, those are used to give the cards
# additional genuine in-game variation.
CARD_SPECS = {
    "check": ("kris_act", 0),
    "compliment": ("kris_act", 2),
    "warn": ("kris_act", 4),
    "joke": ("kris_act", 5),
    "flirt": ("kris_act", 7),
    "spare": ("kris_act", 8),

    "healprayer": ("ralsei_spell", 0),
    "dualheal": ("ralsei_spell", 2),
    "pacify": ("ralsei_spell", 4),
    "revivekris": ("ralsei_spell", 6),
    "revivesong": ("ralsei_spellready", 2),
    "lightup": ("ralsei_spellready", 5),

    "healing": ("susie_spell", 0),
    "okayheal": ("susie_spell", 2),
    "betterheal": ("susie_spell", 4),
    "ultraheal": ("susie_spell", 6),
    "ultimateheal": ("susie_spell", 8),
    "rudebuster": ("susie_spellready", 1),
    "redbuster": ("susie_spellready", 4),
    "dualbuster": ("susie_act", 2),
    "rudesword": ("susie_act", 5),
    "scythemare": ("susie_actready", 1),
    "wakekris": ("susie_actready", 4),

    # Noelle is the actual caster for all three of these spells.
    "iceshock": ("noelle_spell", 1),
    "snowgrave": ("noelle_spell", 4),
    "sleepmist": ("noelle_spell", 7),
}


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


def make_big(source: Path, destination: Path) -> None:
    from PIL import Image

    with Image.open(source) as image:
        image = image.convert("RGBA")
        enlarged = image.resize(
            (image.width * 4, image.height * 4),
            Image.Resampling.NEAREST,
        )
        enlarged.save(destination, format="PNG", optimize=True)


def discover_cards() -> set[str]:
    cards_dir = ROOT / "DeltaruneActs" / "Cards"
    return {
        path.stem.lower()
        for path in cards_dir.glob("*.cs")
        if path.stem != "DeltaruneActsCard"
    }


def validate_card_table() -> None:
    actual = discover_cards()
    expected = set(CARD_SPECS)
    if actual != expected:
        missing = sorted(actual - expected)
        extra = sorted(expected - actual)
        raise RuntimeError(
            "CARD_SPECS does not exactly match the C# card classes. "
            f"Missing specs={missing}; unknown specs={extra}"
        )


def cleanup_old_generated_art() -> None:
    # Remove previous shared/generated PNGs so stale assets cannot be mistaken
    # for current card art. Leave non-PNG files such as SVG/source manifests.
    for path in OUT.glob("*.png"):
        path.unlink()
    if BIG.exists():
        for path in BIG.glob("*.png"):
            path.unlink()


def main() -> int:
    validate_card_table()
    checkout_source_repo()
    cleanup_old_generated_art()
    OUT.mkdir(parents=True, exist_ok=True)
    BIG.mkdir(parents=True, exist_ok=True)

    frames_by_source: dict[str, list[str]] = {}
    source_meta: dict[str, dict[str, object]] = {}
    for source_key, spec in SOURCES.items():
        frames = load_frames(spec["sprite"])
        frames_by_source[source_key] = frames
        source_meta[source_key] = {
            "sprite": spec["sprite"],
            "character": spec["character"],
            "animation": spec["animation"],
            "frame_count": len(frames),
        }

    cards: dict[str, dict[str, object]] = {}
    output_paths: set[str] = set()
    big_paths: set[str] = set()

    for card, (source_key, frame_index) in sorted(CARD_SPECS.items()):
        spec = SOURCES[source_key]
        frames = frames_by_source[source_key]
        if frame_index < 0 or frame_index >= len(frames):
            raise RuntimeError(
                f"Frame index {frame_index} is out of range for {spec['sprite']} "
                f"({len(frames)} frames) while generating {card}.png"
            )

        frame_id = frames[frame_index]
        source = DELTAMODKIT / "sprites" / spec["sprite"] / f"{frame_id}.png"
        if not source.is_file():
            raise RuntimeError(f"Missing source frame: {source}")

        target = OUT / f"{card}.png"
        big_target = BIG / f"{card}.png"
        target.write_bytes(source.read_bytes())
        make_big(target, big_target)

        output_rel = target.relative_to(OUT).as_posix()
        big_rel = big_target.relative_to(OUT).as_posix()
        if output_rel in output_paths or big_rel in big_paths:
            raise RuntimeError(f"Duplicate output asset path detected for {card}")
        output_paths.add(output_rel)
        big_paths.add(big_rel)

        cards[card] = {
            "asset": output_rel,
            "big_asset": big_rel,
            "character": spec["character"],
            "animation": spec["animation"],
            "source_sprite": spec["sprite"],
            "source_commit": DELTAMODKIT_COMMIT,
            "frame_index": frame_index,
            "frame_id": frame_id,
            "source_url": (
                f"https://raw.githubusercontent.com/deltamodders/deltamodkit/"
                f"{DELTAMODKIT_COMMIT}/sprites/{spec['sprite']}/{frame_id}.png"
            ),
        }

    expected_cards = discover_cards()
    if set(cards) != expected_cards:
        raise RuntimeError(f"Generated card coverage mismatch: {sorted(set(cards) ^ expected_cards)}")
    if len(output_paths) != len(cards) or len(big_paths) != len(cards):
        raise RuntimeError("Each card must have a unique normal asset and unique big asset")

    for card in cards:
        normal = OUT / cards[card]["asset"]
        large = OUT / cards[card]["big_asset"]
        if not normal.is_file() or not large.is_file():
            raise RuntimeError(f"Missing final portrait for {card}")

    manifest = {
        "description": (
            "One individual card portrait per Deltarune Acts card, using real "
            "Deltarune battle sprite frames extracted from DeltamodKit."
        ),
        "source_repository": "https://github.com/deltamodders/deltamodkit",
        "source_commit": DELTAMODKIT_COMMIT,
        "individual_card_assets": True,
        "card_count": len(cards),
        "cards": cards,
    }
    (OUT / "SPRITE_SOURCES.json").write_text(
        json.dumps(manifest, indent=2, ensure_ascii=False) + "\n",
        encoding="utf-8",
    )

    print(f"Generated {len(cards)} individual card portraits.")
    print("Normal assets:", len(output_paths))
    print("Big assets:", len(big_paths))
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
