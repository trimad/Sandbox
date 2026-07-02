---
tags:
  - fractal
  - logisticmap
---

# Logistic Map

## Summary
A one-dimensional chaotic map plotted in the (r, x) plane. Classic example of period-doubling route to chaos.

## Formula / Rule
```
x_{n+1} = r x_n (1 - x_n)
```

## Mathematical Background
A one-dimensional chaotic map plotted in the (r, x) plane. Classic example of period-doubling route to chaos.

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
- Bifurcation diagram: plot long-term `x` values against growth rate `r` after discarding transients.
- Cobweb plot: iterate a single seed against the curve `f(x) = r x (1 - x)` to show convergence, cycles, or chaos.
- Parameter sweeps can vary initial seed, transient skip count, and iteration count to reveal period-doubling windows.

## Interesting Coordinates or Presets
![Rendered on 2026-07-02](../../output/2026-07-02_logisticmap.png)

## Sources
- Wikipedia: [Logistic map](https://en.wikipedia.org/wiki/Logistic_map)
- Wikipedia: [Bifurcation diagram](https://en.wikipedia.org/wiki/Bifurcation_diagram)
- Wikipedia: [Escape-time fractal](https://en.wikipedia.org/wiki/Escape-time_fractal)

## Related Notes
- [[mandelbrot]]
- [[julia]]
- [[burningship]]
