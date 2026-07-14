---
tags:
  - fractal
  - tricorn
---

# Tricorn (Mandelbar)

## Summary
The conjugate of the Mandelbrot iteration. It has a self-similar anti-holomorphic structure with three-fold symmetries.

## Formula / Rule
```
z_{n+1} = \overline{z_n}^2 + c
```

## Mathematical Background
The conjugate of the Mandelbrot iteration. It has a self-similar anti-holomorphic structure with three-fold symmetries.

## Rendering Method
Escape-time algorithm on CPU with 1024×1024 resolution.

## Parameters
| Setting | Value |
|---|---|
    | width | 1024 |
    | height | 1024 |
    | highest | 100 |

## Coloring Techniques
- log1p-mapped exposure

## C# Implementation Notes
- Implemented as a standalone fractal class in `Fractals/`

## Known Variations
- Multicorn family: replaces the exponent 2 with higher powers in `z_{n+1} = \overline{z_n}^d + c`.
- Tricorn / Mandelbar Julia sets: fix `c` and iterate the conjugated quadratic map for each starting point.
- Default viewport and parameters as defined in `fractal_queue.json`

## Antiholomorphic Notes
Unlike the Mandelbrot set's holomorphic quadratic map, the Tricorn uses complex conjugation before squaring. This changes the symmetry and boundary structure, making it a useful comparison target for [[Mandelbrot Set]] renderers and smooth-coloring experiments.

## Interesting Coordinates or Presets
![Rendered on 2026-07-14](../../output/2026-07-14_tricorn.png)

## Sources
- Wikipedia: [Tricorn (mathematics)](https://en.wikipedia.org/wiki/Tricorn_(mathematics))
- Wikipedia: [Escape-time fractal](https://en.wikipedia.org/wiki/Escape-time_fractal)

## Related Notes
- [[mandelbrot]]
- [[julia]]
- [[burningship]]
