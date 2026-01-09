#!/usr/bin/env python3
from __future__ import annotations

import argparse
import json
import math
from dataclasses import dataclass
from pathlib import Path
from typing import Dict, Any, Tuple


INCHES_PER_METER = 39.37007874015748
FEET_PER_METER = 3.280839895013123


@dataclass(frozen=True)
class AspectRatio:
    w: float
    h: float

    @property
    def value(self) -> float:
        return self.w / self.h


def parse_aspect_ratio(s: str) -> AspectRatio:
    """
    Accepts formats like '16:10' or '16/9'.
    """
    s = s.strip().replace("/", ":")
    parts = s.split(":")
    if len(parts) != 2:
        raise ValueError(f"Invalid aspect ratio '{s}'. Use like 16:10 or 16:9.")
    w = float(parts[0])
    h = float(parts[1])
    if w <= 0 or h <= 0:
        raise ValueError("Aspect ratio values must be > 0.")
    return AspectRatio(w, h)


def load_profiles(spec_path: Path) -> Dict[str, Any]:
    if not spec_path.exists():
        raise FileNotFoundError(f"Spec file not found: {spec_path}")
    data = json.loads(spec_path.read_text(encoding="utf-8"))
    profiles = data.get("profiles", {})
    if not isinstance(profiles, dict) or not profiles:
        raise ValueError("Spec file must contain a non-empty object at data['profiles'].")
    return profiles


def distance_to_meters(value: float, unit: str) -> float:
    unit = unit.lower().strip()
    if value <= 0:
        raise ValueError("Distance must be > 0.")
    if unit in ("m", "meter", "meters"):
        return value
    if unit in ("ft", "feet", "foot"):
        return value / FEET_PER_METER
    if unit in ("in", "inch", "inches"):
        return value / INCHES_PER_METER
    raise ValueError(f"Unsupported distance unit '{unit}'. Use m, ft, or in.")


def meters_to_unit(meters: float, unit: str) -> float:
    unit = unit.lower().strip()
    if unit in ("m", "meter", "meters"):
        return meters
    if unit in ("ft", "feet", "foot"):
        return meters * FEET_PER_METER
    if unit in ("in", "inch", "inches"):
        return meters * INCHES_PER_METER
    raise ValueError(f"Unsupported output unit '{unit}'. Use m, ft, or in.")


def compute_geometry(distance_m: float, throw_ratio: float, aspect: AspectRatio) -> Tuple[float, float, float]:
    """
    Returns (width_m, height_m, diagonal_m)
    """
    if throw_ratio <= 0:
        raise ValueError("Throw ratio must be > 0.")
    width_m = distance_m / throw_ratio
    height_m = width_m / aspect.value
    diagonal_m = math.hypot(width_m, height_m)
    return width_m, height_m, diagonal_m


def main() -> None:
    ap = argparse.ArgumentParser(
        description="Compute projected image size from throw distance and lens throw ratio."
    )
    ap.add_argument("--spec", default="specs.json", help="Path to spec JSON file.")

    # defeault to the projector we're using if no key is provided
    ap.add_argument("--key", default="EPSON_EB-PU2213B__ELPX02S",help="Profile key name to use (see --list).")

    ap.add_argument("--list", action="store_true", help="List available profile keys and exit.")

    ap.add_argument("--distance", type=float, help="Throw distance numeric value (required unless --list).")
    ap.add_argument("--distance-unit", default="m", help="Distance unit: m, ft, in (default: m).")
    ap.add_argument("--out-unit", default="in", help="Output unit for width/height/diagonal: m, ft, in (default: in).")
    ap.add_argument("--aspect", help="Override aspect ratio (e.g., 16:10 or 16:9). If omitted, uses profile's native_aspect_ratio.")

    args = ap.parse_args()

    spec_path = Path(args.spec)
    profiles = load_profiles(spec_path)

    if args.list:
        print("Available profile keys:")
        for k in sorted(profiles.keys()):
            p = profiles[k]
            proj = p.get("projector", {})
            lens = p.get("lens", {})
            tr = lens.get("throw_ratio", "n/a")
            ar = proj.get("native_aspect_ratio", "n/a")
            print(f"  {k}  (TR={tr}, AR={ar})")
        return

    if not args.key:
        raise SystemExit("ERROR: --key is required unless --list is used.")
    if args.key not in profiles:
        raise SystemExit(f"ERROR: Unknown --key '{args.key}'. Use --list to see valid keys.")
    if args.distance is None:
        raise SystemExit("ERROR: --distance is required unless --list is used.")

    profile = profiles[args.key]
    proj = profile.get("projector", {})
    lens = profile.get("lens", {})

    throw_ratio = float(lens.get("throw_ratio", 0))
    if throw_ratio <= 0:
        raise SystemExit(f"ERROR: Profile '{args.key}' has invalid throw_ratio.")

    if args.aspect:
        aspect = parse_aspect_ratio(args.aspect)
    else:
        native_ar = proj.get("native_aspect_ratio", "16:9")
        aspect = parse_aspect_ratio(native_ar)

    distance_m = distance_to_meters(args.distance, args.distance_unit)

    width_m, height_m, diag_m = compute_geometry(distance_m, throw_ratio, aspect)

    out_unit = args.out_unit
    width_out = meters_to_unit(width_m, out_unit)
    height_out = meters_to_unit(height_m, out_unit)
    diag_out = meters_to_unit(diag_m, out_unit)

    print(f"Profile: {args.key}")
    print(f"  Projector: {proj.get('manufacturer','')} {proj.get('model','')}")
    print(f"  Lens:      {lens.get('manufacturer','')} {lens.get('model','')}  (TR={throw_ratio}:1)")
    print(f"  Aspect:    {aspect.w:g}:{aspect.h:g}")
    print()
    print(f"Input:")
    print(f"  Throw distance: {args.distance:g} {args.distance_unit}  ({distance_m:.4f} m)")
    print()
    print(f"Output ({out_unit}):")
    print(f"  Image width:    {width_out:.3f} {out_unit}")
    print(f"  Image height:   {height_out:.3f} {out_unit}")
    print(f"  Diagonal:       {diag_out:.3f} {out_unit}")


if __name__ == "__main__":
    main()
