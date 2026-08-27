---
tags:
  - fractal
  - hopalong
---

# Hopalong Attractor

## Summary
Barry Martin Hopalong attractor rendered as a density field.

## Formula / Rule
```
x_{n+1}=y_n-sgn(x_n)sqrt(|b x_n-c|); y_{n+1}=a-x_n
```

## Mathematical Background
Barry Martin Hopalong attractor rendered as a density field.

## Rendering Method
Escape-time algorithm on CPU with 384×384 resolution.

## Parameters
| Setting | Value |
|---|---|
    | width | 384 |
    | height | 384 |
    | bailout | 900 |
    | highest | 50 |

## Coloring Techniques
- log1p-mapped exposure

## C# Implementation Notes
- Implemented as a standalone fractal class in `Fractals/`
- Bailout set to 900 to limit orbit tracing

## Known Variations
- Default viewport and parameters as defined in `fractal_queue.json`

## Interesting Coordinates or Presets
![Rendered on 2026-08-27](../../output/2026-08-27_hopalong.png)

## Sources
- Wikipedia: [Escape_time fractal](https://en.wikipedia.org/wiki/Escape-time_fractal)

## Related Notes
- [[mandelbrot]]
- [[julia]]
- [[burningship]]
