---
tags:
  - fractal
  - buddhabrot
---

# Coverage-Guided Buddhabrot

## Summary
A Buddhabrot rendering variant that keeps sampling escaping Mandelbrot orbits until a specified image-coverage target is reached, making the stopping condition density-driven rather than exposure-driven.

## Formula / Rule
```
z_{n+1} = z_n^2 + c, \quad z_0 = 0; accumulate escaping orbit points until a target percentage of pixels is hit
```

## Mathematical Background
A Buddhabrot rendering variant that keeps sampling escaping Mandelbrot orbits until a specified image-coverage target is reached, making the stopping condition density-driven rather than exposure-driven.

## Rendering Method
Escape-time algorithm on CPU with 512×512 resolution.

## Parameters
| Setting | Value |
|---|---|
    | width | 512 |
    | height | 512 |
    | cutoff | 20 |
    | bailout | 600 |
    | coverage | 5 |
    | highest | 35 |

## Coloring Techniques
- log1p-mapped exposure

## C# Implementation Notes
- Implemented as a standalone fractal class in `Fractals/`
- Bailout set to 600 to limit orbit tracing

## Known Variations
- Default viewport and parameters as defined in `fractal_queue.json`

## Interesting Coordinates or Presets
![Rendered on 2026-07-17](../../output/2026-07-17_buddhabrot-coverage.png)

## Sources
- Wikipedia: [Escape_time fractal](https://en.wikipedia.org/wiki/Escape-time_fractal)

## Related Notes
- [[mandelbrot]]
- [[julia]]
- [[burningship]]
