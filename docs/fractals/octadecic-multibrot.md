---
tags:
  - fractal
  - multibrot
---

# Octadecic Multibrot

## Summary
The degree-18 Mandelbrot-family parameter set. Raising each orbit to the eighteenth power produces seventeenfold rotational structure, a very compact central body, and hairline tendrils around the escape boundary.

## Formula / Rule
```
z_{n+1} = z_n^18 + c, \quad z_0 = 0
```

## Mathematical Background
For the Multibrot family `z -> z^d + c`, the parameter plane has `(d - 1)`-fold rotational symmetry around the origin. The octadecic case (`d = 18`) therefore presents seventeen principal arms; using a tight ±0.88 viewport keeps the compact high-degree body large enough to show its satellite bulbs and hairline exterior filaments.

## Rendering Method
Escape-time algorithm on CPU with 1024×1024 resolution.

## Parameters
| Setting | Value |
|---|---|
    | width | 1024 |
    | height | 1024 |
    | bailout | 500 |
    | highest | 80 |
    | min-real | -0.88 |
    | max-real | 0.88 |
    | min-imaginary | -0.88 |
    | max-imaginary | 0.88 |

## Coloring Techniques
- log1p-mapped exposure

## C# Implementation Notes
- Implemented as a standalone fractal class in `Fractals/`
- Bailout set to 500 to limit orbit tracing

## Known Variations
- Default viewport and parameters as defined in `fractal_queue.json`

## Interesting Coordinates or Presets
![Rendered on 2026-08-25](../../output/2026-08-25_octadecic-multibrot.png)

## Sources
- Wikipedia: [Escape_time fractal](https://en.wikipedia.org/wiki/Escape-time_fractal)

## Related Notes
- [[mandelbrot]]
- [[julia]]
- [[burningship]]
