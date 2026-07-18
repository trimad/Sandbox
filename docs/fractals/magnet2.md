---
tags:
  - fractal
  - magnet
---

# Magnet Type II Fractal

## Summary
A higher-degree rational-map magnet fractal. Points in the parameter plane are iterated toward the attracting fixed point z = 1, with poles and basin boundaries producing dense circular filigree.

## Formula / Rule
```
z_{n+1} = ((z_n^3 + 3(c - 1)z_n + (c - 1)(c - 2)) / (3z_n^2 + 3(c - 2)z_n + (c - 1)(c - 2) + 1))^2, \quad z_0 = 0
```

## Mathematical Background
A higher-degree rational-map magnet fractal. Points in the parameter plane are iterated toward the attracting fixed point z = 1, with poles and basin boundaries producing dense circular filigree.

## Rendering Method
Escape-time algorithm on CPU with 1024×1024 resolution.

## Parameters
| Setting | Value |
|---|---|
    | width | 1024 |
    | height | 1024 |
    | bailout | 250 |
    | highest | 250 |
    | min-real | -2.25 |
    | max-real | 2.25 |
    | min-imaginary | -2.25 |
    | max-imaginary | 2.25 |

## Coloring Techniques
- log1p-mapped exposure

## C# Implementation Notes
- Implemented as a standalone fractal class in `Fractals/`
- Bailout set to 250 to limit orbit tracing

## Known Variations
- Default viewport and parameters as defined in `fractal_queue.json`

## Interesting Coordinates or Presets
![Rendered on 2026-07-18](../../output/2026-07-18_magnet2.png)

## Sources
- Wikipedia: [Escape_time fractal](https://en.wikipedia.org/wiki/Escape-time_fractal)

## Related Notes
- [[mandelbrot]]
- [[julia]]
- [[burningship]]
