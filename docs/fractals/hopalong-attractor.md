---
tags:
  - fractal
  - fractal/attractor
  - fractal/hopalong
  - fractal/rare
  - csharp
  - visualization
renderer: hopalong
image: ../../output/fractals/HopalongAttractor/HopalongAttractor.png
---
# Hopalong Attractor

## Summary

Barry Martin's Hopalong attractors are compact two-dimensional recursive maps that produce almost-calligraphic orbit-density figures. They are a strong antidote to repetitive polynomial escape-time work.

![Rendered Hopalong Attractor](../../output/fractals/HopalongAttractor/HopalongAttractor.png)

## Formula / Rule

Classic Barry Martin form:

$$x_{n+1}=y_n-\operatorname{sgn}(x_n)\sqrt{|b x_n-c|}$$

$$y_{n+1}=a-x_n$$

Rendered preset:

- $a=-2.0$
- $b=-0.33$
- $c=0.01$

## Mathematical Background

The square-root fold causes points to "hop" between curved branches. The attractor is best understood through its invariant density, not through point membership.

## Rendering Method

Start from many nearby seeds, iterate the map, discard the first few transient points, and accumulate a density histogram.

## Parameters

- CLI keyword: `hopalong`
- Rendered preset: `width=384 height=384 bailout=900 highest=50`
- Internal plot bounds: approximately $[-18,18]^2$
- Shader: `log1p-mapped`

## Coloring Techniques

Log-density is the baseline. Higher-quality versions should use a larger resolution and palette-map the histogram after a black-point stretch.

## C# Implementation Notes

Implemented in `Fractals/RareFractals.cs` as `HopalongAttractor`.

## Known Variations

- Positive Barry Martin form
- Additive Barry Martin form
- Sinusoidal Barry Martin form
- Gingerbread Man special cases

## Interesting Coordinates or Presets

- `a=-2.0 b=-0.33 c=0.01`
- `a=-3.14 b=0.2 c=0.3`

## Sources

- Mitch Richling, "Hopalong Fractals": https://www.mitchr.me/SS/barrymartin/index.html
- Softology, "Visions Of Chaos 2D Strange Attractor Tutorial": https://softology.pro/tutorials/attractors2d/tutorial.htm

## Related Notes

Related: [[Strange Attractor]], [[Gingerbread Man Attractor]], [[Rare Fractal Scout]]
