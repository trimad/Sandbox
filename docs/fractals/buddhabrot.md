---
tags:
  - fractal
  - buddhabrot
---

# Buddhabrot

## Summary
A trajectory-density rendering of the Mandelbrot iteration. Instead of coloring c directly, it accumulates the paths of escaping orbits, revealing ghostly basin structure around the Mandelbrot set.

## Formula / Rule
```
z_{n+1} = z_n^2 + c, \quad z_0 = 0; plot escaping orbit points instead of parameter points
```

## Mathematical Background
The Buddhabrot is a nonstandard visualization of the Mandelbrot iteration introduced by Melinda Green. For each sampled parameter `c`, the renderer first checks whether the orbit escapes; if it does, the visited `z` positions are accumulated into an image-space exposure buffer. Bright regions therefore show where many escaping trajectories pass, not where parameter values themselves lie.

## Rendering Method
Escape-time algorithm on CPU with 512×512 resolution. This daily preset samples random `c` values in the standard ±2 complex-plane window, discards the first 20 orbit steps as a cutoff, follows escaping paths for up to 600 iterations, and stops when the exposure target reaches 35.

## Parameters
| Setting | Value |
|---|---|
    | width | 512 |
    | height | 512 |
    | cutoff | 20 |
    | bailout | 600 |
    | highest | 35 |

## Coloring Techniques
- log1p-mapped exposure

## C# Implementation Notes
- Implemented as a standalone fractal class in `Fractals/`
- Bailout set to 600 to limit orbit tracing

## Known Variations
- Default viewport and parameters as defined in `fractal_queue.json`

## Interesting Coordinates or Presets
![Rendered on 2026-07-16](../../output/2026-07-16_buddhabrot.png)

## Sources
- Melinda Green: [The Buddhabrot Technique](https://superliminal.com/fractals/bbrot/)
- Wikipedia: [Buddhabrot](https://en.wikipedia.org/wiki/Buddhabrot)
- Wikipedia: [Escape-time fractal](https://en.wikipedia.org/wiki/Escape-time_fractal)

## Related Notes
- [[mandelbrot]]
- [[julia]]
- [[burningship]]
