---
tags:
  - fractal
  - fractal/attractor
  - fractal/strange-attractor
  - fractal/rare
  - csharp
  - visualization
renderer: clifford
image: ../../output/fractals/CliffordAttractor/CliffordAttractor.png
---
# Clifford Attractor

## Summary

Clifford attractors are Pickover-associated strange attractors built from sine/cosine feedback. They render beautifully as occupancy-density maps rather than as escape-time sets.

![Rendered Clifford Attractor](../../output/fractals/CliffordAttractor/CliffordAttractor.png)

## Formula / Rule

$$x_{n+1}=\sin(a y_n)+c\cos(a x_n)$$

$$y_{n+1}=\sin(b x_n)+d\cos(b y_n)$$

Rendered preset:

- $a=-1.4$
- $b=1.6$
- $c=1.0$
- $d=0.7$

## Mathematical Background

The map repeatedly folds phase space through bounded trigonometric functions. The resulting invariant density appears as layered smoke, fans, and shells.

## Rendering Method

Iterate many nearby seeds, discard a short transient, and increment a histogram cell for each subsequent point.

## Parameters

- CLI keyword: `clifford`
- Rendered preset: `width=384 height=384 bailout=900 highest=50`
- Internal plot bounds: approximately $[-2.5,2.5]^2$
- Shader: `log1p-mapped`

## Coloring Techniques

Density mapping is essential. Future variants should encode local curvature or orbit speed into color channels.

## C# Implementation Notes

Implemented in `Fractals/RareFractals.cs` as `CliffordAttractor`.

## Known Variations

Paul Bourke lists several parameter sets, including $a=1.6,b=-0.6,c=-1.2,d=1.6$ and $a=-1.8,b=-2.0,c=-0.5,d=-0.9$.

## Interesting Coordinates or Presets

- `a=-1.4 b=1.6 c=1.0 d=0.7`

## Sources

- Paul Bourke, "Clifford Attractors": https://paulbourke.net/fractals/clifford/

## Related Notes

Related: [[Strange Attractor]], [[Histogram Rendering]], [[Ikeda Map]]
