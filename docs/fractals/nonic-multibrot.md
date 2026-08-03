---
tags:
  - fractal
  - multibrot
---

# Nonic Multibrot

## Summary
The degree-9 Mandelbrot-family parameter set. Raising each orbit to the ninth power produces eightfold rotational structure, compact lobes, and extremely fine tendrils around the escape boundary.

## Formula / Rule
```
z_{n+1} = z_n^9 + c, \quad z_0 = 0
```

## Mathematical Background
The Nonic Multibrot is the connectedness locus for the unicritical polynomial family `z \mapsto z^9 + c`. Degree-`d` Multibrots have `d - 1` rotational symmetry in the parameter plane, so the nonic case forms eight arms arranged around a compact central body. Compared with lower-degree Multibrots, the ninth power compresses the interior and pushes visible detail into very narrow satellite bulbs and exterior tendrils.

## Rendering Method
Escape-time algorithm on CPU with 1024×1024 resolution.

## Parameters
| Setting | Value |
|---|---|
    | width | 1024 |
    | height | 1024 |
    | bailout | 500 |
    | highest | 80 |
    | min-real | -1.1 |
    | max-real | 1.1 |
    | min-imaginary | -1.1 |
    | max-imaginary | 1.1 |

## Coloring Techniques
- log1p-mapped exposure

## C# Implementation Notes
- Implemented as a standalone fractal class in `Fractals/`
- Bailout set to 500 to limit orbit tracing

## Known Variations
- Default viewport and parameters as defined in `fractal_queue.json`

## Interesting Coordinates or Presets
![Rendered on 2026-08-03](../../output/2026-08-03_nonic-multibrot.png)

## Sources
- Wikipedia: [Escape_time fractal](https://en.wikipedia.org/wiki/Escape-time_fractal)

## Related Notes
- [[mandelbrot]]
- [[julia]]
- [[burningship]]
