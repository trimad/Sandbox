---
tags:
  - fractal
  - strange-attractor
---

# Clifford Attractor

## Summary
Pickover-associated sine/cosine strange attractor density field.

## Formula / Rule
```
x_{n+1}=sin(a y_n)+c cos(a x_n); y_{n+1}=sin(b x_n)+d cos(b y_n)
```

## Mathematical Background
The Clifford attractor is a two-dimensional strange-attractor map popularized in Clifford Pickover-style visual explorations. Unlike escape-time Julia/Mandelbrot families, each iteration advances a point through a nonlinear sine/cosine recurrence; the image is a density field built from many visits to the plane. Small changes in the four coefficients can move the orbit between compact wisps, butterfly-like lobes, and more diffuse chaotic clouds.

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
![Rendered on 2026-08-26](../../output/2026-08-26_clifford.png)

## Sources
- Paul Bourke: [Clifford Attractors](https://paulbourke.net/fractals/clifford/) — formula and example coefficient sets.
- Wikipedia: [Strange attractor](https://en.wikipedia.org/wiki/Attractor#Strange_attractor)

## Related Notes
- [[mandelbrot]]
- [[julia]]
- [[burningship]]
