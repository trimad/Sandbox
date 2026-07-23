---
tags:
  - fractal
  - julia
---

# Phoenix Fractal

## Summary
A two-step Julia-family escape-time fractal where each new iterate depends on both the current and previous orbit values. The memory term produces bird-like wings, nested curls, and asymmetric filament structure.

## Formula / Rule
```
z_{n+1} = z_n^2 + c + p z_{n-1}, \quad c = -0.5, \quad p = -0.56667
```

## Mathematical Background
A two-step Julia-family escape-time fractal where each new iterate depends on both the current and previous orbit values. The memory term produces bird-like wings, nested curls, and asymmetric filament structure.

## Rendering Method
Escape-time algorithm on CPU with 1024×1024 resolution.

## Parameters
| Setting | Value |
|---|---|
    | width | 1024 |
    | height | 1024 |
    | bailout | 500 |
    | highest | 80 |
    | min-real | -1.5 |
    | max-real | 1.5 |
    | min-imaginary | -1.5 |
    | max-imaginary | 1.5 |

## Coloring Techniques
- log1p-mapped exposure

## C# Implementation Notes
- Implemented as a standalone fractal class in `Fractals/`
- Bailout set to 500 to limit orbit tracing

## Known Variations
- Default viewport and parameters as defined in `fractal_queue.json`

## Interesting Coordinates or Presets
![Rendered on 2026-07-23](../../output/2026-07-23_phoenix.png)

## Sources
- Wikipedia: [Escape_time fractal](https://en.wikipedia.org/wiki/Escape-time_fractal)

## Related Notes
- [[mandelbrot]]
- [[julia]]
- [[burningship]]
