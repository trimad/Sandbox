---
tags:
  - fractal
  - fractal/attractor
  - fractal/dynamical-system
  - fractal/physics
  - fractal/rare
  - csharp
renderer: ikeda
image: ../../output/fractals/IkedaAttractor/IkedaAttractor.png
---
# Ikeda Map

## Summary

The Ikeda map is a chaotic map from nonlinear optics, originally modeling light behavior in a nonlinear optical resonator. Its attractor has a curled, spiral structure that makes it visually distinct from standard escape-time fractals.

![Rendered Ikeda Map](../../output/fractals/IkedaAttractor/IkedaAttractor.png)

## Formula / Rule

$$x_{n+1}=1+u(x_n\cos t_n-y_n\sin t_n)$$

$$y_{n+1}=u(x_n\sin t_n+y_n\cos t_n)$$

where:

$$t_n=0.4-rac{6}{1+x_n^2+y_n^2}$$

Rendered preset: $u=0.918$.

## Mathematical Background

The map combines radius-dependent rotation with dissipation. For sufficiently high $u$, repeated stretch-and-fold behavior produces a strange attractor.

## Rendering Method

Iterate many initial points, discard transients, then accumulate the last orbit positions as a density field.

## Parameters

- CLI keyword: `ikeda`
- Rendered preset: `width=384 height=384 bailout=900 highest=50`
- Internal plot bounds: approximately $[-2,3]^2$
- Shader: `log1p-mapped`

## Coloring Techniques

Log histogram mapping reveals the spiral bands. Density + iteration-age coloring would show how trajectories settle into the attractor.

## C# Implementation Notes

Implemented in `Fractals/RareFractals.cs` as `IkedaAttractor`.

## Known Variations

- Vary $u$ from `0.6` to `1.0` to inspect bifurcations and attractor thickening.

## Interesting Coordinates or Presets

- `u=0.918`

## Sources

- HandWiki, "Ikeda map": https://handwiki.org/wiki/Ikeda_map

## Related Notes

Related: [[Strange Attractor]], [[Gumowski-Mira Attractor]], [[Tinkerbell Map]]
