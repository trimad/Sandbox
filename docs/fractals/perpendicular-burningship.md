---
tags:
  - fractal
  - burningship
---

# Perpendicular Burning Ship

## Summary
A Burning Ship-family escape-time fractal that folds the imaginary component before squaring. The one-axis fold creates ship-like mirrored structures while preserving a different handedness from the real-fold perpendicular Mandelbrot.

## Formula / Rule
```
z_{n+1} = (\operatorname{Re}(z_n) + i|\operatorname{Im}(z_n)|)^2 + c, \quad z_0 = 0
```

## Mathematical Background
A Burning Ship-family escape-time fractal that folds the imaginary component before squaring. The one-axis fold creates ship-like mirrored structures while preserving a different handedness from the real-fold perpendicular Mandelbrot.

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
![Rendered on 2026-07-22](../../output/2026-07-22_perpendicular-burningship.png)

## Sources
- Wikipedia: [Escape_time fractal](https://en.wikipedia.org/wiki/Escape-time_fractal)

## Related Notes
- [[mandelbrot]]
- [[julia]]
- [[burningship]]
