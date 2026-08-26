---
tags:
  - fractal
  - fractal/biomorph
  - fractal/escape-time
  - fractal/rare
  - csharp
  - math
  - visualization
renderer: pickover-biomorph
image: ../../output/fractals/PickoverBiomorph/PickoverBiomorph.png
---
# Pickover Biomorph

## Summary

Pickover biomorphs are component-escape fractals attributed to Clifford A. Pickover. They are not merely Mandelbrot variants: the distinctive "biological" forms come from testing whether the real or imaginary component escapes a threshold, rather than testing only the complex magnitude.

![Rendered Pickover Biomorph](../../output/fractals/PickoverBiomorph/PickoverBiomorph.png)

## Formula / Rule

One useful seed formula is:

$$z_{n+1}=\sin(z_n)+e^{z_n}+c$$

Current preset:

- $c=0.1+0.6i$
- $z_0$ is the pixel coordinate in the complex plane
- Escape condition: $|\operatorname{Re}(z)| > A$ or $|\operatorname{Im}(z)| > A$
- $A=10$

## Mathematical Background

The biomorph classification is defined less by one canonical polynomial and more by the escape predicate. Pickover-style component escape changes the boundary geometry and produces insectoid or cellular silhouettes.

## Rendering Method

CPU escape-time render over a rectangular complex-domain viewport. Each pixel iterates independently until the component threshold is crossed or the bailout iteration limit is reached.

## Parameters

- CLI keyword: `pickover-biomorph`
- Rendered preset: `width=384 height=384 min-real=-3 max-real=3 min-imaginary=-3 max-imaginary=3 bailout=120 highest=50`
- Shader: `log1p-mapped`

## Coloring Techniques

The first pass uses log-scaled exposure. Future passes should use component-excess coloring: encode whether the real part, imaginary part, or both components escaped first.

## C# Implementation Notes

Implemented in `Fractals/RareFractals.cs` as `PickoverBiomorph`, using `System.Numerics.Complex` for sine and exponential iteration.

## Known Variations

- $z_{n+1}=z_n^3+c$
- $z_{n+1}=z_n^5+c$
- $z_{n+1}=\sin(z_n)+z_n^2+c$
- $z_{n+1}=z_n^{z_n}+z_n^5+c$

## Interesting Coordinates or Presets

- `c=0.1+0.6i`, viewport $[-3,3]	imes[-3,3]$
- Try `c=0.3+0.3i` with $\sin(z)+z^2+c$

## Sources

- Paul Bourke, "Biomorphs": https://paulbourke.net/fractals/biomorph/

## Related Notes

Related: [[Escape-Time Algorithm]], [[Rare Fractal Scout]], [[Clifford Attractor]]
