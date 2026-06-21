#!/usr/bin/env python3
"""
Daily Fractal Pipeline for Fractalnaut.

Picks a fractal from the queue by day-of-year, renders it on CPU,
creates an Obsidian-style markdown note, and commits the result to GitHub.
"""
import json
import os
import subprocess
import shutil
import textwrap
from datetime import date

REPO = r"C:\GitHub\Sandbox"
EXE = os.path.join(REPO, r"bin\Debug\net10.0-windows\Sandbox.exe")
QUEUE = os.path.join(REPO, "scripts", "fractal_queue.json")
DOCS = os.path.join(REPO, "docs", "fractals")
OUT = os.path.join(REPO, "output")

os.makedirs(DOCS, exist_ok=True)
os.makedirs(OUT, exist_ok=True)

def main():
    with open(QUEUE, "r", encoding="utf-8") as f:
        queue = json.load(f)

    today = date.today()
    idx = today.timetuple().tm_yday % len(queue)
    entry = queue[idx]
    keyword = entry["keyword"]
    name = entry["name"]
    tag = entry["tag"]
    params = entry["params"]
    formula = entry["formula"]
    summary = entry["summary"]
    notes = entry["notes"]

    # Build CLI args
    args = [EXE, keyword]
    for k, v in params.items():
        args.append(f"{k}={v}")
    args += ["render", "save", "log1p-mapped", "draw"]

    print(f"=== Daily Fractal: {name} ===")
    print(f"Keyword: {keyword}")
    print(f"Params: {params}")

    env = os.environ.copy()
    result = subprocess.run(args, cwd=REPO, env=env, capture_output=True, text=True)
    print(result.stdout)
    if result.returncode != 0:
        print("RENDER FAILED")
        print(result.stderr)
        return 1
    print("RENDER OK")

    # Find the generated PNG
    fractal_root = r"C:\Fractals"
    candidates = []
    if os.path.isdir(fractal_root):
        for dir_name in os.listdir(fractal_root):
            full_dir = os.path.join(fractal_root, dir_name)
            if not os.path.isdir(full_dir):
                continue
            for fname in os.listdir(full_dir):
                if fname.lower().endswith(".png"):
                    candidates.append(os.path.join(full_dir, fname))
    if not candidates:
        print(f"No PNG found under {fractal_root}")
        return 1

    dest_png = os.path.join(OUT, f"{today.isoformat()}_{keyword}.png")
    src_png = candidates[-1]
    shutil.copy2(src_png, dest_png)
    print(f"PNG copied to {dest_png}")

    # Write / update Obsidian-style note
    note_path = os.path.join(DOCS, f"{keyword}.md")
    rel_img = os.path.relpath(dest_png, DOCS)
    note = textwrap.dedent(f"""\
    # {name}

    ## Summary
    {summary}

    ## Formula / Rule
    ```
    {formula}
    ```

    ## Mathematical Background
    {summary}

    ## Rendering Method
    Escape-time algorithm on CPU with {params.get('width', '?')}×{params.get('height', '?')} resolution.

    ## Parameters
    | Setting | Value |
    |---|---|
    """)
    for k, v in params.items():
        note += f"    | {k} | {v} |\n"
    note += "\n## Coloring Techniques\n- log1p-mapped exposure\n\n"
    note += "## C# Implementation Notes\n- Rendered via `Sandbox.exe` CLI\n"
    if "bailout" in params:
        note += f"- Bailout set to {params['bailout']} to limit orbit tracing\n"
    note += "\n## Interesting Coordinates or Presets\n\n"
    note += f"![Rendered on {today.isoformat()}]({rel_img})\n"

    with open(note_path, "w", encoding="utf-8") as f:
        f.write(note)
    print(f"Note written to {note_path}")

    # Git add / commit / push
    subprocess.run(["git", "add", "docs/", "output/", "scripts/", "Fractals/", "Program.cs"], cwd=REPO)
    msg = f"Daily fractal: {name} ({today.isoformat()})"
    commit = subprocess.run(["git", "commit", "-m", msg], cwd=REPO, capture_output=True, text=True)
    if commit.returncode == 0:
        push = subprocess.run(["git", "push"], cwd=REPO, capture_output=True, text=True, timeout=120)
        if push.returncode == 0:
            print(f"Committed and pushed: {msg}")
        else:
            print(f"Committed but push failed: {push.stderr.strip()}")
    else:
        print(f"No changes to commit or commit failed: {commit.stderr.strip()}")
    return 0

if __name__ == "__main__":
    raise SystemExit(main())
