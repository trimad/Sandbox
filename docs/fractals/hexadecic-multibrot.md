---
tags:
  - fractal
  - multibrot
---

# Hexadecic Multibrot

## Summary
The degree-16 Mandelbrot-family parameter set. Raising each orbit to the sixteenth power produces fifteenfold rotational structure, a very compact central body, and hairline tendrils around the escape boundary.

## Formula / Rule
```
z_{n+1} = z_n^16 + c, \quad z_0 = 0
```

## Mathematical Background
The degree-16 Mandelbrot-family parameter set. Raising each orbit to the sixteenth power produces fifteenfold rotational structure, a very compact central body, and hairline tendrils around the escape boundary.

## Rendering Method
Escape-time algorithm on CPU with 1024×1024 resolution.

## Parameters
| Setting | Value |
|---|---|
    | width | 1024 |
    | height | 1024 |
    | bailout | 500 |
    | highest | 80 |
    | min-real | -0.92 |
    | max-real | 0.92 |
    | min-imaginary | -0.92 |
    | max-imaginary | 0.92 |

## Coloring Techniques
- log1p-mapped exposure

## C# Implementation Notes
- Implemented as a standalone fractal class in `Fractals/`
- Bailout set to 500 to limit orbit tracing

## Known Variations
- Default viewport and parameters as defined in `fractal_queue.json`

## Interesting Coordinates or Presets
![Rendered on 2026-08-21](../../output/2026-08-21_hexadecic-multibrot.png)

## Sources
- Wikipedia: [Escape_time fractal](https://en.wikipedia.org/wiki/Escape-time_fractal)

## Related Notes
- [[mandelbrot]]
- [[julia]]
- [[burningship]]
