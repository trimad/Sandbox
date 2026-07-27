---
tags:
  - fractal
  - burningship
---

# Quartic Burning Ship

## Summary
A degree-4 Burning Ship-family escape-time fractal. Both coordinates are folded to absolute values before a quartic Multibrot step, creating folded ship-like cusps with higher-order lobes and thin branching.

## Formula / Rule
```
z_{n+1} = (|\operatorname{Re}(z_n)| + i|\operatorname{Im}(z_n)|)^4 + c, \quad z_0 = 0
```

## Mathematical Background
The classic [[burningship]] applies a two-axis absolute-value fold before a quadratic Mandelbrot step. This quartic variant keeps the same folded parameter-plane idea but raises the folded orbit to the fourth power, so it sits between Burning Ship variants and [[multibrot4]]-style higher-degree escape-time sets. The even power tends to sharpen mirrored plume boundaries and produces stronger axis-aligned symmetry than the cubic Burning Ship.

## Rendering Method
Escape-time algorithm on CPU with 1024×1024 resolution.

## Parameters
| Setting | Value |
|---|---|
    | width | 1024 |
    | height | 1024 |
    | bailout | 500 |
    | highest | 80 |
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
![Rendered on 2026-07-27](../../output/2026-07-27_quartic-burning-ship.png)

## Sources
- Wikipedia: [Escape_time fractal](https://en.wikipedia.org/wiki/Escape-time_fractal)

## Related Notes
- [[mandelbrot]]
- [[julia]]
- [[burningship]]
