---
tags:
  - fractal
  - multibrot
---

# Quintic Multibrot

## Summary
The degree-5 Mandelbrot-family parameter set. Raising each orbit to the fifth power produces fivefold rotational structure, narrow satellite bulbs, and delicate branching around the escape boundary.

## Formula / Rule
```
z_{n+1} = z_n^5 + c, \quad z_0 = 0
```

## Mathematical Background
The Quintic Multibrot belongs to the family of connectedness loci for maps of the form `z -> z^d + c`. Degree 5 gives `d - 1 = 4` primary lobes around the central component, with odd-power rotational symmetry and thinner satellite decorations than the quadratic Mandelbrot set.

## Rendering Method
Escape-time algorithm on CPU with 1024×1024 resolution.

## Parameters
| Setting | Value |
|---|---|
    | width | 1024 |
    | height | 1024 |
    | bailout | 500 |
    | highest | 80 |
    | min-real | -1.35 |
    | max-real | 1.35 |
    | min-imaginary | -1.35 |
    | max-imaginary | 1.35 |

## Coloring Techniques
- log1p-mapped exposure

## C# Implementation Notes
- Implemented as a standalone fractal class in `Fractals/`
- Bailout set to 500 to limit orbit tracing

## Known Variations
- Degree-`d` Multibrot sets for other integer powers such as [[multibrot3]] and [[multibrot4]].
- Multicorn variants replace `z_n` with its complex conjugate before exponentiation.
- Burning Ship variants fold coordinates with absolute values before taking the power.

## Interesting Coordinates or Presets
![Rendered on 2026-07-28](../../output/2026-07-28_quintic-multibrot.png)

## Sources
- Wikipedia: [Escape_time fractal](https://en.wikipedia.org/wiki/Escape-time_fractal)

## Related Notes
- [[mandelbrot]]
- [[julia]]
- [[burningship]]
