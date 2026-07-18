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
Magnet Type II is the higher-degree companion to [[magnet1|Magnet Type I]], using a rational map rather than a polynomial escape map. The iteration is usually interpreted as a complexified parameter-plane map related to hierarchical-lattice magnetic renormalization formulas: stable parameters converge to the attracting fixed point `z = 1`, while poles and basin boundaries form the visible fractal structure.

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
- **Magnet Type I** lowers the rational-map degree and often produces a more two-lobed, disk-chain structure.
- Changing convergence tolerance, bailout, or using distance/exposure combined shaders emphasizes either basin interiors or the pole-driven boundary filigree.

## Interesting Coordinates or Presets
![Rendered on 2026-07-18](../../output/2026-07-18_magnet2.png)

## Sources
- Paul Bourke, “Magnet 1 / Magnet 2”: https://paulbourke.net/fractals/magnet/
- Wikipedia: [Escape-time fractal](https://en.wikipedia.org/wiki/Escape-time_fractal)

## Related Notes
- [[mandelbrot]]
- [[julia]]
- [[burningship]]
