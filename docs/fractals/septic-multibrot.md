---
tags:
  - fractal
  - multibrot
---

# Septic Multibrot

## Summary
The degree-7 Mandelbrot-family parameter set. Raising each orbit to the seventh power produces sixfold rotational structure, compact lobes, and fine tendrils around the escape boundary.

## Formula / Rule
```
z_{n+1} = z_n^7 + c, \quad z_0 = 0
```

## Mathematical Background
The degree-7 Mandelbrot-family parameter set. Raising each orbit to the seventh power produces sixfold rotational structure, compact lobes, and fine tendrils around the escape boundary.

## Rendering Method
Escape-time algorithm on CPU with 1024×1024 resolution.

## Parameters
| Setting | Value |
|---|---|
    | width | 1024 |
    | height | 1024 |
    | bailout | 500 |
    | highest | 80 |
    | min-real | -1.2 |
    | max-real | 1.2 |
    | min-imaginary | -1.2 |
    | max-imaginary | 1.2 |

## Coloring Techniques
- log1p-mapped exposure

## C# Implementation Notes
- Implemented as a standalone fractal class in `Fractals/`
- Bailout set to 500 to limit orbit tracing

## Known Variations
- Default viewport and parameters as defined in `fractal_queue.json`

## Interesting Coordinates or Presets
![Rendered on 2026-08-01](../../output/2026-08-01_septic-multibrot.png)

## Sources
- Wikipedia: [Escape_time fractal](https://en.wikipedia.org/wiki/Escape-time_fractal)

## Related Notes
- [[mandelbrot]]
- [[julia]]
- [[burningship]]
