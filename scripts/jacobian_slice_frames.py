"""Render a rotating family of full-map C³ escape-time slices.

For each theta, sample w=u+iv over a configurable square [-a,a]² and initialize the supplied map at
    iota_theta(w) = (w, exp(i theta) w, 0).
Then iterate the complete F:C³→C³. This is a changing complex-plane slice,
not a projection of F to a made-up one-variable polynomial.
"""
from pathlib import Path
import argparse
import shutil
from concurrent.futures import ProcessPoolExecutor
import numpy as np
from PIL import Image


def F(x, y, z):
    t = 1 + x * y
    return (
        t**3 * z + y**2 * t * (4 + 3 * x * y),
        y + 3 * x * t**2 * z + 3 * x * y**2 * (4 + 3 * x * y),
        2 * x - 3 * x**2 * y - x**3 * z,
    )


def palette(mu):
    q = np.clip(mu / 10.0, 0.0, 1.0)
    anchors = np.array([
        [5, 7, 22], [18, 48, 138], [23, 157, 210],
        [80, 225, 202], [248, 195, 72], [255, 245, 205],
    ], dtype=np.float64)
    pos = q * (len(anchors) - 1)
    lo = np.floor(pos).astype(np.int32)
    hi = np.minimum(lo + 1, len(anchors) - 1)
    r = (pos - lo)[:, None]
    return ((1 - r) * anchors[lo] + r * anchors[hi]).astype(np.uint8)


def render_frame(size, iterations, radius, theta, half_width):
    u = np.linspace(-half_width, half_width, size, dtype=np.float64)
    v = np.linspace(half_width, -half_width, size, dtype=np.float64)
    image = np.zeros((size, size, 3), dtype=np.uint8)
    radius2 = radius * radius
    phase = np.exp(1j * theta)

    for row0 in range(0, size, 16):
        row1 = min(row0 + 16, size)
        w = (u[None, :] + 1j * v[row0:row1, None]).ravel()
        x = w.copy()
        y = phase * w
        z = np.zeros_like(w)
        active = np.ones(w.size, dtype=bool)
        escaped_at = np.full(w.size, -1, dtype=np.int16)
        escaped_norm = np.ones(w.size, dtype=np.float64)

        for step in range(iterations):
            ids = np.flatnonzero(active)
            if not ids.size:
                break
            with np.errstate(over='ignore', invalid='ignore'):
                xn, yn, zn = F(x[ids], y[ids], z[ids])
                norm2 = np.abs(xn)**2 + np.abs(yn)**2 + np.abs(zn)**2
            escaped_now = ~np.isfinite(norm2) | (norm2 > radius2)
            if np.any(escaped_now):
                hit = ids[escaped_now]
                escaped_at[hit] = step + 1
                escaped_norm[hit] = np.sqrt(np.where(np.isfinite(norm2[escaped_now]), norm2[escaped_now], radius2 * radius2))
                active[hit] = False
            keep = ~escaped_now
            kept = ids[keep]
            x[kept], y[kept], z[kept] = xn[keep], yn[keep], zn[keep]

        mask = escaped_at >= 0
        if np.any(mask):
            mu = escaped_at[mask].astype(float) + 1 - np.log(np.log(np.maximum(escaped_norm[mask], radius))) / np.log(7)
            image[row0:row1].reshape(-1, 3)[mask] = palette(mu)
    return image


def render_and_save(task):
    index, total, size, iterations, radius, half_width, output_dir = task
    theta = 2 * np.pi * index / total
    frame = render_frame(size, iterations, radius, theta, half_width)
    Image.fromarray(frame, 'RGB').save(Path(output_dir) / f'frame_{index:04d}.png', compress_level=3)
    return index, theta


def main():
    p = argparse.ArgumentParser()
    p.add_argument('--frames', type=int, default=240)
    p.add_argument('--size', type=int, default=1024)
    p.add_argument('--iterations', type=int, default=100)
    p.add_argument('--radius', type=float, default=32)
    p.add_argument('--half-width', type=float, default=2.0, help='view is [-half-width,half-width]²')
    p.add_argument('--workers', type=int, default=1, help='independent frames rendered concurrently')
    p.add_argument('--output-dir', type=Path, default=Path.home() / 'jacobian_slice_frames')
    p.add_argument('--clean', action='store_true')
    args = p.parse_args()
    if args.clean and args.output_dir.exists():
        shutil.rmtree(args.output_dir)
    args.output_dir.mkdir(parents=True, exist_ok=True)

    tasks = [
        (index, args.frames, args.size, args.iterations, args.radius, args.half_width, str(args.output_dir))
        for index in range(args.frames)
    ]
    if args.workers == 1:
        results = map(render_and_save, tasks)
    else:
        pool = ProcessPoolExecutor(max_workers=args.workers)
        results = pool.map(render_and_save, tasks)
    try:
        for completed, (index, theta) in enumerate(results, start=1):
            if completed % 10 == 0 or completed == 1:
                print(f'{completed}/{args.frames}: frame={index:04d} theta={theta:.6f}', flush=True)
    finally:
        if args.workers != 1:
            pool.shutdown(wait=True)


if __name__ == '__main__':
    main()
