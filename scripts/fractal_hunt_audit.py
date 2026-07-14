#!/usr/bin/env python3
"""Audit Fractalnaut's hunt queue against cataloged notes and implemented CLI keywords."""
import json
import re
from pathlib import Path

REPO = Path(r"C:\GitHub\Sandbox")
QUEUE = REPO / "scripts" / "fractal_queue.json"
DOCS = REPO / "docs" / "fractals"
PROGRAM = REPO / "Program.cs"


def load_queue():
    return json.loads(QUEUE.read_text(encoding="utf-8"))


def cataloged_keywords():
    if not DOCS.is_dir():
        return set()
    return {path.stem.lower() for path in DOCS.glob("*.md")}


def implemented_fractal_keywords():
    source = PROGRAM.read_text(encoding="utf-8")
    # Restrict to switch cases marked as fractal selectors. LogisticMap predates
    # the marker comment, so include it explicitly until Program.cs is normalized.
    return set(re.findall(r'case\s+"([^"]+)":\s*//fractal', source)) | {"logisticmap"}


def main():
    queue = load_queue()
    queued = [entry["keyword"].lower() for entry in queue]
    queued_set = set(queued)
    cataloged = cataloged_keywords()
    implemented = implemented_fractal_keywords()

    duplicate_queue_items = sorted({kw for kw in queued if queued.count(kw) > 1})
    already_cataloged_queue_items = [kw for kw in queued if kw in cataloged]
    ready_to_catalog = [kw for kw in queued if kw not in cataloged]
    implemented_not_queued_or_cataloged = sorted(implemented - queued_set - cataloged)
    cataloged_not_queued = sorted(cataloged - queued_set)

    print("Fractal Hunt Audit")
    print("===================")
    print(f"Queue entries: {len(queued)}")
    print(f"Cataloged notes: {len(cataloged)}")
    print(f"Implemented fractal keywords: {len(implemented)}")
    print()
    print("Already cataloged queue entries to skip:")
    print("  " + (", ".join(already_cataloged_queue_items) if already_cataloged_queue_items else "none"))
    print("Uncataloged queue entries ready for daily runs:")
    print("  " + (", ".join(ready_to_catalog) if ready_to_catalog else "none"))
    print("Duplicate keywords inside queue:")
    print("  " + (", ".join(duplicate_queue_items) if duplicate_queue_items else "none"))
    print("Implemented but neither queued nor cataloged opportunities:")
    print("  " + (", ".join(implemented_not_queued_or_cataloged) if implemented_not_queued_or_cataloged else "none"))
    print("Cataloged but not currently in queue/history rotation:")
    print("  " + (", ".join(cataloged_not_queued) if cataloged_not_queued else "none"))

    if not ready_to_catalog:
        print("\nAction needed: add fresh fractals to scripts/fractal_queue.json before the daily job runs again.")
    elif implemented_not_queued_or_cataloged:
        print("\nSelf-improvement opportunity: add implemented uncataloged keywords to the queue first; they are renderable now.")
    else:
        print("\nSelf-improvement opportunity: research and implement new fractal families before expanding the queue.")


if __name__ == "__main__":
    main()
