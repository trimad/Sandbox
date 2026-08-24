---
tags:
  - fractal
  - multibrot
---

# Heptadecic Multibrot

## Summary
The degree-17 Mandelbrot-family parameter set. Raising each orbit to the seventeenth power produces sixteenfold rotational structure, a very compact central body, and hairline tendrils around the escape boundary.

## Formula / Rule
```
z_{n+1} = z_n^17 + c, \quad z_0 = 0
```

## Mathematical Background
The degree-17 Mandelbrot-family parameter set. Raising each orbit to the seventeenth power produces sixteenfold rotational structure, a very compact central body, and hairline tendrils around the escape boundary.

## Rendering Method
Escape-time algorithm on CPU with 1024×1024 resolution.

## Parameters
| Setting | Value |
|---|---|
    | width | 1024 |
    | height | 1024 |
    | bailout | 500 |
    | highest | 80 |
    | min-real | -0.9 |
    | max-real | 0.9 |
    | min-imaginary | -0.9 |
    | max-imaginary | 0.9 |

## Coloring Techniques
- log1p-mapped exposure

## C# Implementation Notes
- Implemented as a standalone fractal class in `Fractals/`
- Bailout set to 500 to limit orbit tracing

## Known Variations
- Default viewport and parameters as defined in `fractal_queue.json`

## Interesting Coordinates or Presets
![Rendered on 2026-08-23](../../output/2026-08-23_heptadecic-multibrot.png)

## Sources
- Wikipedia: [Escape_time fractal](https://en.wikipedia.org/wiki/Escape-time_fractal)

## Related Notes
- [[mandelbrot]]
- [[julia]]
- [[burningship]]
