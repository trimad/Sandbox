---
tags:
  - polynomial-dynamics
  - escape-time
  - complex-dynamics
  - jacobian-map
  - visualization
---

# Rotating \(\mathbb C^3\) Polynomial-Map Escape-Time Slices

## Summary

This note documents a 2D visualization of the supplied polynomial map \(F:\mathbb C^3\to\mathbb C^3\). It is **not** a conventional Mandelbrot set, Julia set, or proof visualization. It is an escape-time rendering of the full six-real-dimensional dynamical system after choosing a rotating complex one-dimensional input slice.

Each image pixel represents a complex number \(w=u+iv\). For a chosen slice angle \(\theta\), the pixel initializes the full map at

\[
\iota_\theta(w)=\bigl(w,e^{i\theta}w,0\bigr),
\]

then the complete three-coordinate polynomial map is iterated. The video changes \(\theta\), showing how the escape-time pattern changes as this chosen input plane rotates.

> [!important]
> The supplied map is used here as a computational input. This visualization does not independently establish the claimed Jacobian-conjecture result, visualize its noninjective fibers, or replace a proof audit.

## Polynomial Map

Let \(t=1+xy\). The map used for every frame is

\[
F(x,y,z)=\left(
\begin{aligned}
&t^3z+y^2t(4+3xy),\\
&y+3xt^2z+3xy^2(4+3xy),\\
&2x-3x^2y-x^3z
\end{aligned}
\right).
\]

The supplied map has the formal properties

\[
\det JF=-2
\]

and the three distinct supplied points

\[
(0,0,-1/4),\quad (1,-3/2,13/2),\quad (-1,3/2,13/2)
\]

all map to \((-1/4,0,0)\). Those collision facts motivate the interest in the map, but they are not what the escape coloring directly shows.

## Rendering Rule

For every pixel \(w\), set

\[
(x_0,y_0,z_0)=\iota_\theta(w),
\qquad
(x_{n+1},y_{n+1},z_{n+1})=F(x_n,y_n,z_n).
\]

The pixel is assigned an escape-time color at the first iteration \(n\) for which

\[
|x_n|^2+|y_n|^2+|z_n|^2>32^2.
\]

The renderer uses a smooth escape estimate based on the escape iteration and orbit norm. Dark blue indicates rapid escape; cyan through gold/white indicates later escape. Pixels still below the threshold at the iteration cap are black.

## Why This Is Not Strictly a Fractal

The output has fractal-like escape boundaries and multiscale detail, but it should be described precisely as a **2D escape-time slice of a higher-dimensional polynomial dynamical system**.

- \(F\) acts on \(\mathbb C^3\), which has six real dimensions; a 2D image necessarily chooses a slice or projection.
- The animation changes the initial-condition slice \(\iota_\theta\); it does not rotate the map itself.
- It is neither a parameter-space image like the Mandelbrot set nor a standard one-variable Julia set.
- No invariant-plane claim is made: after the first application of \(F\), the orbit generally leaves \(\iota_\theta(\mathbb C)\). The full \(\mathbb C^3\) orbit is still iterated without projection or reset.

## 4× Zoom Render

The latest version is a 10-second loop created from 300 rotating slices:

| Setting | Value |
|---|---:|
| Resolution | 4096×4096 |
| Frame rate | 30 fps |
| Frames | 300 |
| Duration | 10 seconds |
| Domain | \([-0.5,0.5]\times[-0.5,0.5]\) |
| Zoom relative to the original \([-2,2]^2\) view | 4× |
| Slice angles | \(\theta=2\pi k/300\), \(k=0,\ldots,299\) |
| Maximum iterations | 100 |
| Escape radius | 32 |
| Encoder | FFmpeg / H.264 / `yuv420p` |

![4× zoomed escape-time slice sample](../../output/jacobian/jacobian-map-slice-4x-sample.png)

### Generated artifacts

The full local video exports are intentionally kept outside this repository to avoid committing large binary media:

- `C:\Users\Hermes\jacobian_rotating_slices_4k_30fps_4xzoom.mp4`
- `C:\Users\Hermes\jacobian_rotating_slices_4k_30fps_2xzoom.mp4`
- `C:\Users\Hermes\jacobian_rotating_slices_1024.mp4`

## Reproduction

The renderer is checked into this repository at [`scripts/jacobian_slice_frames.py`](../../scripts/jacobian_slice_frames.py). It uses NumPy and Pillow to render independent frames concurrently.

```bash
uv run --with numpy --with pillow python scripts/jacobian_slice_frames.py \
  --frames 300 --size 4096 --iterations 100 --radius 32 \
  --half-width 0.5 --workers 8 \
  --output-dir output/jacobian/frames-4x --clean

ffmpeg -y -framerate 30 \
  -i output/jacobian/frames-4x/frame_%04d.png \
  -c:v libx264 -preset medium -crf 16 -pix_fmt yuv420p \
  -movflags +faststart output/jacobian-escape-slices-4x.mp4
```

> [!note]
> The command uses a POSIX-style shell. On this Windows machine, Git Bash is the active shell. FFmpeg is installed and was used to create the local rendered videos.

## Related Notes

- [[mandelbrot]]
- [[julia]]
- [[multibrot4]]
