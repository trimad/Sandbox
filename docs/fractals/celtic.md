---
tags:
  - fractal
  - mandelbrot
---

# Celtic Mandelbrot

## Summary
A Mandelbrot-family escape-time fractal that folds the real component of z^2 before adding c, producing Celtic-knot-like lobes and mirrored boundary filaments.

## Formula / Rule
```
z_{n+1} = |\operatorname{Re}(z_n^2)| + i\operatorname{Im}(z_n^2) + c, \quad z_0 = 0
```

## Mathematical Background
A Mandelbrot-family escape-time fractal that folds the real component of z^2 before adding c, producing Celtic-knot-like lobes and mirrored boundary filaments.

## Rendering Method
Escape-time algorithm on CPU with 1024×1024 resolution.

## Parameters
| Setting | Value |
|---|---|
    | width | 1024 |
    | height | 1024 |
    | bailout | 500 |
    | highest | 50 |
    | min-real | -2.0 |
    | max-real | 2.0 |
    | min-imaginary | -2.0 |
    | max-imaginary | 2.0 |

## Coloring Techniques
- log1p-mapped exposure

## C# Implementation Notes
- Implemented as a standalone fractal class in `Fractals/`
- Bailout set to 500 to limit orbit tracing

## Known Variations
- Default viewport and parameters as defined in `fractal_queue.json`

## Interesting Coordinates or Presets
![Rendered on 2026-07-19](../../output/2026-07-19_celtic.png)

## Sources
- Wikipedia: [Escape_time fractal](https://en.wikipedia.org/wiki/Escape-time_fractal)

## Related Notes
- [[mandelbrot]]
- [[julia]]
- [[burningship]]
