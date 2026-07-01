---
tags:
  - fractal
  - glynn
---

# Glynn Fractal

## Summary
A complex-analytic fractal with fractional exponent 1.5. Produces a tree-like structure with dense branching.

## Formula / Rule
```
z_{n+1} = z_n^{1.5} - 0.2
```

## Mathematical Background
A complex-analytic fractal with fractional exponent 1.5. Produces a tree-like structure with dense branching.

## Rendering Method
Escape-time algorithm on CPU with 1024×1024 resolution.

## Parameters
| Setting | Value |
|---|---|
    | width | 1024 |
    | height | 1024 |
    | highest | 50 |

## Coloring Techniques
- log1p-mapped exposure

## C# Implementation Notes
- Implemented as a standalone fractal class in `Fractals/`

## Known Variations
- Default viewport and parameters as defined in `fractal_queue.json`

## Interesting Coordinates or Presets
![Rendered on 2026-07-01](../../output/2026-07-01_glynn.png)

## Sources
- Wikipedia: [Escape_time fractal](https://en.wikipedia.org/wiki/Escape-time_fractal)

## Related Notes
- [[mandelbrot]]
- [[julia]]
- [[burningship]]
