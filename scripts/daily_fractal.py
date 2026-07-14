#!/usr/bin/env python3
"""
Daily Fractal Pipeline for Fractalnaut.

Picks the next not-yet-cataloged fractal from the queue, renders it on CPU,
creates an Obsidian-style markdown note, and commits the result to GitHub.

Duplicate guard: existing docs/fractals/<keyword>.md files are treated as already
cataloged, so the pipeline skips them instead of overwriting old notes/renders.
"""
import argparse
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
FRACTAL_ROOT = os.path.join(OUT, "fractals")

os.makedirs(DOCS, exist_ok=True)
os.makedirs(OUT, exist_ok=True)
os.makedirs(FRACTAL_ROOT, exist_ok=True)

def load_queue():
    with open(QUEUE, "r", encoding="utf-8") as f:
        return json.load(f)


def cataloged_keywords(docs_dir=DOCS):
    """Return lower-case keywords that already have Obsidian notes."""
    if not os.path.isdir(docs_dir):
        return set()
    return {
        os.path.splitext(name)[0].lower()
        for name in os.listdir(docs_dir)
        if name.lower().endswith(".md")
    }


def select_uncataloged_entry(queue, today=None, cataloged=None):
    """Cycle from today's slot until a queue entry without a note is found."""
    if not queue:
        raise ValueError("fractal_queue.json is empty")

    today = today or date.today()
    cataloged = cataloged if cataloged is not None else cataloged_keywords()
    start = today.timetuple().tm_yday % len(queue)
    skipped = []

    for offset in range(len(queue)):
        idx = (start + offset) % len(queue)
        entry = queue[idx]
        keyword = entry["keyword"].lower()
        if keyword in cataloged:
            skipped.append(entry["keyword"])
            continue
        return entry, skipped

    return None, skipped


def main(argv=None):
    parser = argparse.ArgumentParser(description="Render the next uncataloged queued fractal.")
    parser.add_argument(
        "--dry-run",
        action="store_true",
        help="select and report the next fractal without rendering, writing, committing, or pushing",
    )
    args = parser.parse_args(argv)

    queue = load_queue()
    today = date.today()
    cataloged = cataloged_keywords()
    entry, skipped = select_uncataloged_entry(queue, today=today, cataloged=cataloged)

    if skipped:
        print("Skipped already cataloged queue entries: " + ", ".join(skipped))

    if entry is None:
        print("No uncataloged fractals remain in scripts/fractal_queue.json.")
        print("Add new queue entries before running the daily pipeline again.")
        return 2

    keyword = entry["keyword"]
    name = entry["name"]
    tag = entry["tag"]
    params = entry["params"]
    formula = entry["formula"]
    summary = entry["summary"]

    # Build CLI args
    render_args = [EXE, keyword]
    for k, v in params.items():
        render_args.append(f"{k}={v}")
    render_args += ["render", "save", "log1p-mapped", "draw"]

    print(f"=== Daily Fractal: {name} ===")
    print(f"Keyword: {keyword}")
    print(f"Params: {params}")

    if args.dry_run:
        print("Dry run: duplicate guard and selection completed; no render/write/commit/push performed.")
        return 0

    env = os.environ.copy()
    result = subprocess.run(render_args, cwd=REPO, env=env, capture_output=True, text=True)
    print(result.stdout)
    if result.returncode != 0:
        print("RENDER FAILED")
        print(result.stderr)
        return 1
    print("RENDER OK")

    # Find the generated PNG
    candidates = []
    if os.path.isdir(FRACTAL_ROOT):
        for dir_name in os.listdir(FRACTAL_ROOT):
            full_dir = os.path.join(FRACTAL_ROOT, dir_name)
            if not os.path.isdir(full_dir):
                continue
            for fname in os.listdir(full_dir):
                if fname.lower().endswith(".png"):
                    candidates.append(os.path.join(full_dir, fname))
    if not candidates:
        print(f"No PNG found under {FRACTAL_ROOT}")
        return 1

    dest_png = os.path.join(OUT, f"{today.isoformat()}_{keyword}.png")
    # Use the most recently written local renderer output, not an arbitrary
    # directory-listing result, so the Obsidian post embeds this run's picture.
    src_png = max(candidates, key=os.path.getmtime)
    shutil.copy2(src_png, dest_png)
    print(f"PNG copied to {dest_png}")

    # Write / update Obsidian-style note. The duplicate guard above prevents
    # overwriting notes for already cataloged queue entries during daily runs.
    note_path = os.path.join(DOCS, f"{keyword}.md")
    rel_img = os.path.relpath(dest_png, DOCS).replace(os.sep, "/")
    note = textwrap.dedent(f"""\
    ---
    tags:
      - fractal
      - {tag.split('/')[-1]}
    ---

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
    note += "## C# Implementation Notes\n- Implemented as a standalone fractal class in `Fractals/`\n"
    if "bailout" in params:
        note += f"- Bailout set to {params['bailout']} to limit orbit tracing\n"
    note += "\n## Known Variations\n- Default viewport and parameters as defined in `fractal_queue.json`\n"
    note += "\n## Interesting Coordinates or Presets\n"
    note += f"![Rendered on {today.isoformat()}]({rel_img})\n\n"
    note += "## Sources\n- Wikipedia: [Escape_time fractal](https://en.wikipedia.org/wiki/Escape-time_fractal)\n\n"
    related = [q for q in queue if q["keyword"] != keyword][:3]
    if related:
        note += "## Related Notes\n"
        for r in related:
            note += f"- [[{r['keyword']}]]\n"
    with open(note_path, "w", encoding="utf-8") as f:
        f.write(note)
    print(f"Note written to {note_path}")

    # Git add / commit / push. Cron runs without a human at the terminal, so
    # disable interactive credential prompts; otherwise Git Credential Manager
    # can hang the scheduled run indefinitely while waiting for UI input.
    git_env = env.copy()
    git_env["GIT_TERMINAL_PROMPT"] = "0"
    git_env["GCM_INTERACTIVE"] = "Never"

    subprocess.run(["git", "add", "docs/", "output/", "scripts/", "Fractals/", "Program.cs"], cwd=REPO, env=git_env)
    msg = f"Daily fractal: {name} ({today.isoformat()})"
    commit = subprocess.run(["git", "commit", "-m", msg], cwd=REPO, env=git_env, capture_output=True, text=True)
    if commit.returncode == 0:
        try:
            push = subprocess.run(
                ["git", "-c", "credential.interactive=false", "push"],
                cwd=REPO,
                env=git_env,
                capture_output=True,
                text=True,
                timeout=120,
            )
        except subprocess.TimeoutExpired:
            print("Committed but push timed out after 120 seconds; not retrying.")
        else:
            if push.returncode == 0:
                print(f"Committed and pushed: {msg}")
            else:
                push_error = (push.stderr or push.stdout).strip()
                print(f"Committed but push failed: {push_error}")
    else:
        print(f"No changes to commit or commit failed: {commit.stderr.strip()}")
    return 0

if __name__ == "__main__":
    raise SystemExit(main())
