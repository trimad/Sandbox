# Multibrot-3

## Summary
The degree-3 analogue of the Mandelbrot set. The main cardioid becomes a reentrant shape with three-fold symmetry.

## Formula / Rule
```
z_{n+1} = z_n^3 + c
```

## Mathematical Background
The degree-3 analogue of the Mandelbrot set. The main cardioid becomes a reentrant shape with three-fold symmetry.

## Rendering Method
Escape-time algorithm on CPU with 1024×1024 resolution.

## Parameters
| Setting | Value |
|---|---|
    | width | 1024 |
    | height | 1024 |
    | highest | 50 |

## Coloring Techniques
- log1p-mapped exposure

## C# Implementation Notes
- Rendered via `Sandbox.exe` CLI

## Interesting Coordinates or Presets

![Rendered on 2026-06-21](..\..\output\2026-06-21_multibrot3.png)
