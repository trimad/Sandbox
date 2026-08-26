---
tags:
  - fractal
  - fractal/attractor
  - fractal/dynamical-system
  - fractal/rare
  - csharp
  - math
renderer: gumowski-mira
image: ../../output/fractals/GumowskiMira/GumowskiMira.png
---
# Gumowski-Mira Attractor

## Summary

The Gumowski-Mira attractor is a nonlinear two-dimensional map historically associated with particle-accelerator dynamics and chaotic trajectories. It produces birdlike, woven, and looped phase portraits.

![Rendered Gumowski-Mira Attractor](../../output/fractals/GumowskiMira/GumowskiMira.png)

## Formula / Rule

A commonly rendered form uses:

$$x_{n+1}=y_n+b(1-0.05y_n^2)y_n+f(x_n)$$

$$y_{n+1}=-x_n+f(x_{n+1})$$

with helper:

$$f(x)=a x+rac{2(1-a)x^2}{1+x^2}$$

Current preset:

- $a=0.008$
- $b=0.05$

## Mathematical Background

This is a discrete nonlinear dynamical system. Small parameter changes alter the orbit from regular loops to chaotic attractor-like phase portraits.

## Rendering Method

Seed a small cloud near a known interesting region, discard an initial transient, then accumulate an orbit-density histogram.

## Parameters

- CLI keyword: `gumowski-mira`
- Rendered preset: `width=384 height=384 bailout=900 highest=50`
- Internal plot bounds: approximately $[-24,24]^2$
- Shader: `log1p-mapped`

## Coloring Techniques

Log-density mapping emphasizes the thin birdlike curves. Future work should search the $(a,b)$ plane for richer attractors.

## C# Implementation Notes

Implemented in `Fractals/RareFractals.cs` as `GumowskiMiraAttractor`.

## Known Variations

- Vary $a$ near zero.
- Use $b=1$ in traditional Gumowski-Mira explorations for some parameter regimes.
- Seed from different initial phase-plane points.

## Interesting Coordinates or Presets

- `a=0.008 b=0.05`, seed near `(10, 0)`

## Sources

- Paul Bourke, "Gumowski-Mira attractor": http://www.paulbourke.net/fractals/GumowskiMira
- Softology, "Visions Of Chaos 2D Strange Attractor Tutorial": https://softology.pro/tutorials/attractors2d/tutorial.htm

## Related Notes

Related: [[Strange Attractor]], [[Ikeda Map]], [[Tinkerbell Map]]
