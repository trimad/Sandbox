---
tags:
  - fractal
  - fractal/dynamical-system
  - fractal/pickover
  - fractal/rare
  - csharp
  - visualization
renderer: popcorn
image: ../../output/fractals/Popcorn/Popcorn.png
---
# Popcorn Fractal

## Summary

The Popcorn fractal is a Clifford Pickover dynamical-system fractal. It is rendered by iterating a two-variable trigonometric/tangent map from many starting locations and accumulating a density histogram.

![Rendered Popcorn Fractal](../../output/fractals/Popcorn/Popcorn.png)

## Formula / Rule

$$x_{n+1}=x_n-h\sin(y_n+	an(3y_n))$$

$$y_{n+1}=y_n-h\sin(x_n+	an(3x_n))$$

Typical $h$ is about `0.05`.

## Mathematical Background

The tangent terms introduce repeated discontinuity-driven folding. The plotted structure is not a direct filled set; it is a density map of where iterated trajectories land.

## Rendering Method

Sample every pixel as an initial condition over $[-3,3]^2$, iterate the Popcorn map, and increment a 2D histogram for every in-bounds orbit visit.

## Parameters

- CLI keyword: `popcorn`
- Rendered preset: `width=384 height=384 min-real=-3 max-real=3 min-imaginary=-3 max-imaginary=3 bailout=360 highest=50`
- Shader: `log1p-mapped`

## Coloring Techniques

Log-scaled histogram density works well. Palette experiments should emphasize wispy caustics and suppress isolated overbright hits.

## C# Implementation Notes

Implemented in `Fractals/RareFractals.cs` as `PopcornFractal`. Uses atomic histogram increments because many source pixels can land on the same output cell.

## Known Variations

- Subsample initial conditions instead of all pixels for a more historically accurate sparse plot.
- Use subpixel jitter and longer trajectories for smoother density.

## Interesting Coordinates or Presets

- $h=0.05$, viewport $[-3,3]^2$

## Sources

- Paul Bourke, "Popcorn": https://paulbourke.net/fractals/popcorn/index.html

## Related Notes

Related: [[Pickover Biomorph]], [[Histogram Rendering]], [[Rare Fractal Scout]]
