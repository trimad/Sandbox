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
- Default viewport and parameters as defined in `fractal_queue.json`

## Interesting Coordinates or Presets
![Rendered on 2026-06-24](../../output/2026-06-24_logisticmap.png)

## Sources
- Wikipedia: [Escape_time fractal](https://en.wikipedia.org/wiki/Escape-time_fractal)

## Related Notes
- [[mandelbrot]]
- [[julia]]
- [[burningship]]
