---
tags:
  - fractal
  - fractal/attractor
  - fractal/chaotic-map
  - fractal/rare
  - csharp
renderer: tinkerbell
image: ../../output/fractals/TinkerbellAttractor/TinkerbellAttractor.png
---
# Tinkerbell Map

## Summary

The Tinkerbell map is a compact chaotic two-dimensional map with a winglike strange attractor. It is simple enough to implement quickly but less commonly rendered than the standard Hénon or Lorenz examples.

![Rendered Tinkerbell Map](../../output/fractals/TinkerbellAttractor/TinkerbellAttractor.png)

## Formula / Rule

$$x_{n+1}=x_n^2-y_n^2+a x_n+b y_n$$

$$y_{n+1}=2x_ny_n+c x_n+d y_n$$

Rendered preset:

- $a=0.9$
- $b=-0.6013$
- $c=2.0$
- $d=0.5$
- Seed near $(-0.72,-0.64)$

## Mathematical Background

The map is a polynomial dynamical system in two real variables. Its attractor emerges from repeated nonlinear shearing and folding.

## Rendering Method

Iterate many nearby starting values, skip transients, and draw the orbit density.

## Parameters

- CLI keyword: `tinkerbell`
- Rendered preset: `width=384 height=384 bailout=900 highest=50`
- Internal plot bounds: $x\in[-2,2]$, $y\in[-2,1.2]$
- Shader: `log1p-mapped`

## Coloring Techniques

Log-density grayscale is the first pass. A future render should color by local velocity to reveal wing flow.

## C# Implementation Notes

Implemented in `Fractals/RareFractals.cs` as `TinkerbellAttractor`.

## Known Variations

- $a=0.3,b=0.6,c=2.0,d=0.27$
- Animate $d$ from `0.5` down toward `0.4`.

## Interesting Coordinates or Presets

- `a=0.9 b=-0.6013 c=2.0 d=0.5 x0=-0.72 y0=-0.64`

## Sources

- Wikipedia, "Tinkerbell map": https://en.wikipedia.org/wiki/Tinkerbell_map

## Related Notes

Related: [[Strange Attractor]], [[Ikeda Map]], [[Gumowski-Mira Attractor]]
