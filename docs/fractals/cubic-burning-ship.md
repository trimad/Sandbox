---
tags:
  - fractal
  - burningship
---

# Cubic Burning Ship

## Summary
A degree-3 Burning Ship-family escape-time fractal. Both coordinates are folded to their absolute values before applying a cubic Multibrot step, producing ship-like cusps with higher-order lobes and branching.

## Formula / Rule
```
z_{n+1} = (|\operatorname{Re}(z_n)| + i|\operatorname{Im}(z_n)|)^3 + c, \quad z_0 = 0
```

## Mathematical Background
A degree-3 Burning Ship-family escape-time fractal. Both coordinates are folded to their absolute values before applying a cubic Multibrot step, producing ship-like cusps with higher-order lobes and branching.

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
![Rendered on 2026-07-25](../../output/2026-07-25_cubic-burning-ship.png)

## Sources
- Wikipedia: [Escape_time fractal](https://en.wikipedia.org/wiki/Escape-time_fractal)

## Related Notes
- [[mandelbrot]]
- [[julia]]
- [[burningship]]
