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
- Bifurcation diagram: plot long-run sampled `x_n` values against the control parameter `r`.
- Lyapunov exponent diagram: color regions by sensitivity to initial conditions.
- Parameter-map variants: change the sampled seed `x_0`, discard count, or accumulation window to expose different periodic bands.

## Interesting Coordinates or Presets
![Rendered on 2026-06-24](../../output/2026-06-24_logisticmap.png)

## Sources
- Wikipedia: [Logistic map](https://en.wikipedia.org/wiki/Logistic_map)
- Wikipedia: [Bifurcation diagram](https://en.wikipedia.org/wiki/Bifurcation_diagram)

## Related Notes
- [[mandelbrot]]
- [[julia]]
- [[burningship]]
