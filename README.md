# Fractal Sandbox

Fractal Sandbox is a command-line fractal renderer for Windows/.NET. It renders fractal data into `exposure.dat`, `distance.dat`, and optional flow-field data, applies shader keywords to generate a canvas, and writes PNG output under `C:\Fractals\<FractalName>`.

## Quick Start

Render a small GPU Buddhabrot, save the data files, apply an exposure shader, and write a PNG:

```powershell
.\bin\Debug\net10.0-windows\Sandbox.exe buddhabrot-gpu width=1024 height=1024 highest=16 render save log1p-mapped draw
```

Render the prime-only GPU Buddhabrot variant using only prime-length escape orbits up to `1613`:

```powershell
.\bin\Debug\net10.0-windows\Sandbox.exe buddhabrot-primes-gpu width=1024 height=1024 highest=16 render save log1p-mapped draw
```

Render the coverage-based Buddhabrot variant until 40% of pixels have been hit at least once:

```powershell
.\bin\Debug\net10.0-windows\Sandbox.exe buddhabrot-coverage width=1024 height=1024 coverage=40 cutoff=50 bailout=2000 render save log1p-mapped draw
```

Load an existing GPU Buddhabrot render and generate every shader comparison image without rerendering:

```powershell
.\bin\Debug\net10.0-windows\Sandbox.exe buddhabrot-gpu load all
```

Print the built-in help:

```powershell
.\bin\Debug\net10.0-windows\Sandbox.exe /?
```

## CLI Model

The executable processes space-separated tokens from left to right.

```text
Sandbox.exe [fractal] [settings...] [render|load] [shader...] [save] [draw]
```

Settings can appear anywhere in the argument list. They are parsed before commands run, so a setting at the end still affects an earlier `render`. Tokens with `key=value` syntax are consumed as settings and are not treated as commands.

Commands are order-sensitive. A typical render selects a fractal first, then renders, then shades, then saves or draws:

```powershell
.\bin\Debug\net10.0-windows\Sandbox.exe buddhabrot render palette-mapped bloom unsharp-mask draw
```

`load` uses the currently selected fractal name to choose the folder. Put the fractal keyword before `load`:

```powershell
.\bin\Debug\net10.0-windows\Sandbox.exe buddhabrot-gpu load distance-exposure-hsv draw
```

Unknown or invalid `key=value` settings abort before rendering. Unknown non-setting tokens currently have no effect.

## Fractal Keywords

These keywords select the fractal implementation and initialize its hard-coded default viewport.

| Keyword | Output Folder | Notes |
| --- | --- | --- |
| `buddhabrot` | `C:\Fractals\Buddhabrot` | CPU Buddhabrot renderer. Uses `width`, `height`, `cutoff`, `bailout`, and `highest`. |
| `buddhabrot-gpu` | `C:\Fractals\Buddhabrot-GPU` | ILGPU/CUDA Buddhabrot renderer for NVIDIA GPUs. Uses `width`, `height`, `cutoff`, `bailout`, `highest`, and `gpu-progress-interval`. |
| `buddhabrot-primes-gpu` | `C:\Fractals\Buddhabrot-Primes-GPU` | ILGPU/CUDA Buddhabrot renderer that only uses prime-length escape orbits up to `1613`. Uses `width`, `height`, `cutoff`, `bailout`, `highest`, and `gpu-progress-interval`. |
| `buddhabrot-coverage` | `C:\Fractals\Buddhabrot-Coverage` | CPU Buddhabrot renderer that stops when a percentage of pixels have at least one hit. Uses `width`, `height`, `cutoff`, `bailout`, and `coverage`. |
| `glynn` | `C:\Fractals\Glynn` | Glynn fractal. Uses `width`, `height`, and `highest`. |
| `julia` | `C:\Fractals\Julia` | Julia fractal. Uses `width`, `height`, and `highest`. |
| `mandelbrot` | `C:\Fractals\Mandelbrot` | Mandelbrot fractal. Uses `width`, `height`, `bailout`, and `highest`. |
| `logisticmap` | `C:\Fractals\LogisticMap` | Logistic map render. Uses `width`, `height`, and `highest`. |

`buddhabrot-gpu` requires a working NVIDIA CUDA driver/runtime. It prints the detected NVIDIA GPU name and the ILGPU accelerator name when rendering.

## Core Commands

| Command | Behavior |
| --- | --- |
| `render` | Render the selected fractal into memory. If the GPU backend cannot start, the command prints the setup error and exits. |
| `load` | Load `settings.json`, `distance.dat`, `exposure.dat`, and optional `flow.dat` for the selected fractal. It checks `C:\Fractals\<FractalName>` first, then falls back to legacy root files under `C:\Fractals`. |
| `save` | Save `settings.json`, `distance.dat`, `exposure.dat`, and any available `flow.dat` for the selected fractal under `C:\Fractals\<FractalName>`. |
| `draw` | Save the current canvas as `C:\Fractals\<FractalName>\<FractalName>.png`. |
| `all` | Render first if needed, then apply every shader keyword in the built-in comparison list and save one PNG per shader. |
| `/?` | Print the quick help menu and shader list. |

`draw` only writes whatever the current canvas contains. Run a shader before `draw`, or use a canvas shader that auto-starts from `log1p-mapped`.

`save` writes data files, not PNG shader outputs. `all` writes comparison PNGs. `draw` writes the current canvas PNG using the fractal name.

If you run `draw` after `all`, it writes the canvas left by the last successfully applied shader, which is normally `smooth-step`.

PNG output is written through a streaming PNG writer. It writes to a temporary file first and only replaces the final `.png` after the PNG is complete.

## Render Settings

Defaults:

| Setting | Default | Accepted Keys | Meaning |
| --- | --- | --- | --- |
| Width | `32768` | `width`, `w` | Image width in pixels. Must be positive. |
| Height | `32768` | `height`, `h` | Image height in pixels. Must be positive. |
| Target peak exposure | `50` | `highest`, `target`, `exposure-target` | Render stop target. Buddhabrot renderers stop after the highest-hit pixel reaches this value. Must be positive. |
| Coverage threshold | `50` | `coverage`, `coverage-target`, `coverage-threshold` | For `buddhabrot-coverage`, stop when this percent of pixels have at least one exposure hit. Must be positive. |
| Cutoff | `50` | `cutoff` | Skip the first N orbit iterations before plotting. `cutoff=0` plots immediately. Must be non-negative. |
| Bailout | `2000` | `bailout`, `iterations` | Maximum orbit iterations. Must be positive. Use `bailout=max` to set no practical limit. |
| GPU progress interval | `8` | `gpu-progress-interval`, `gpu-progress` | Number of GPU batches queued before synchronizing and checking progress. Must be positive. |

The parser also accepts setting keys prefixed with `--` or `/`, for example `--width=1024` or `/height=1024`.

Invalid setting values abort before rendering. For example, `cutoff=0` is valid, but `width=0` is not.

Environment variable defaults:

| Environment Variable | CLI Equivalent |
| --- | --- |
| `SANDBOX_WIDTH` | `width` |
| `SANDBOX_HEIGHT` | `height` |
| `SANDBOX_HIGHEST` | `highest` |
| `SANDBOX_CUTOFF` | `cutoff` |
| `SANDBOX_BAILOUT` | `bailout` |
| `SANDBOX_GPU_PROGRESS_INTERVAL` | `gpu-progress-interval` |

Explicit CLI settings override environment defaults.

## Output Files

All current saves go under:

```text
C:\Fractals\<FractalName>
```

Core data files:

| File | Written By | Used By |
| --- | --- | --- |
| `settings.json` | `save` | `load` |
| `distance.dat` | `save` | Distance shaders and mixed distance/exposure shaders |
| `exposure.dat` | `save` | Exposure shaders and mixed distance/exposure shaders |
| `flow.dat` | `save` when the renderer emits flow data | Flow-field shaders |
| `<FractalName>.png` | `draw` | Final current-canvas output |

`all` comparison images use a source prefix when the shader name does not already include one:

| Data Source | Example Shader | Example Output |
| --- | --- | --- |
| `distance.dat` | `distance-mapped` | `distance-mapped.png` |
| `distance.dat` | `exponential` | `distance-exponential.png` |
| `exposure.dat` | `gamma` | `exposure-gamma.png` |
| `distance.dat+exposure.dat` | `false-color-palette` | `distance-exposure-false-color-palette.png` |
| `flow.dat` | `flow-current` | `flow-current.png` |
| `canvas` | `bloom` | `canvas-bloom.png` |

Large renders produce very large files. A `32768x32768` `distance.dat` is large, `flow.dat` has the same bytes per pixel as `distance.dat`, and `all` can write dozens of large PNGs.

## Shader Data Sources

Distance shaders use `distance.dat`:

| Keyword | Description |
| --- | --- |
| `distance-binned` | Fixed-range distance grayscale bins. |
| `distance-hsv` | Distance mapped to hue. |
| `distance-mapped` | Linear distance grayscale. |
| `exponential` | Distance exponential stretch with exponent `2`. |
| `distance-log1p-mapped` | `log1p` distance grayscale. |
| `distance-asinh-stretch` | Asinh distance stretch. |
| `distance-gamma` | Gamma-corrected distance grayscale. |
| `distance-percentile-mapped` | Percentile clipped distance grayscale. |
| `distance-palette-mapped` | Color palette distance map. |
| `distance-contour-mapped` | Quantized distance contours. |
| `distance-sobel-edges` | Sobel edges from the distance field. |
| `distance-emboss-light` | Embossed distance height map. |

Mixed shaders use paired data files:

| Keyword | Description |
| --- | --- |
| `false-color-palette` | False color from exposure and distance. |
| `distance-exposure-hsv` | Distance hue with exposure value. |

Flow-field shaders use `flow.dat`:

| Keyword | Description |
| --- | --- |
| `flow-direction-hsv` | Average incoming orbit segment direction as hue, with coherence as saturation. |
| `flow-coherence` | Grayscale strength of the average flow vector. |
| `flow-current` | Direction hue with current-like bands along the vector field. |

`flow.dat` stores two `float32` values per pixel: average incoming segment X and Y. `buddhabrot-classic-gpu` currently emits this data.

Canvas shaders operate on the current canvas:

| Keyword | Description |
| --- | --- |
| `background-gradient-removal` | Subtract low-frequency gradients. |
| `local-contrast` | Boost broad local contrast. |
| `gaussian-blur` | Gaussian blur current canvas. |
| `bloom` | Glow bright regions. |
| `star-reduction` | Reduce compact bright peaks. |
| `star-mask` | Mask compact bright peaks. |
| `denoise-median` | Median denoise hot pixels. |
| `bilateral-smooth` | Edge-preserving smoothing. |
| `deconvolution-sharpen` | Small Richardson-Lucy style sharpen. |
| `background-neutralize` | Neutralize dark background color casts. |
| `saturation-boost` | Boost color saturation. |
| `curves` | S-curve contrast. |
| `sobel-edges` | Sobel edges from current canvas. |
| `emboss-light` | Emboss current canvas. |
| `unsharp-mask` | Sharpen current canvas. |

All other shader keywords use `exposure.dat`:

| Keyword | Description |
| --- | --- |
| `gamma` | Gamma-corrected exposure grayscale. |
| `asinh-stretch` | Asinh exposure stretch. |
| `histogram-equalized` | Global histogram equalization. |
| `clahe` | Adaptive histogram equalization. |
| `black-point` | Subtract a dark background floor. |
| `screen-stretch` | Astrophotography preview stretch. |
| `percentile-mapped` | Percentile clipped exposure grayscale. |
| `palette-mapped` | Color palette exposure map. |
| `contour-mapped` | Quantized exposure contours. |
| `sigmoid-contrast` | Sigmoid exposure contrast. |
| `log1p-mapped` | `log1p` exposure grayscale. |
| `exposure-binned` | Rank-order exposure grayscale bins. |
| `exposure-hsv` | Exposure mapped to hue. |
| `hex-color` | Exposure bits as RGB. |
| `log-base-highest` | Log exposure grayscale. |
| `mapped` | Linear exposure grayscale. |
| `smooth-step` | Smooth-step exposure grayscale. |

Canvas shaders call `EnsureCanvasFromExposure()`. If no canvas content exists, they first generate a default `log1p-mapped` exposure image and then apply the canvas effect.

## `all` Shader Order

`all` applies and saves the shader keywords in this exact order:

```text
distance-binned
distance-hsv
distance-mapped
exponential
distance-log1p-mapped
distance-asinh-stretch
distance-gamma
distance-percentile-mapped
distance-palette-mapped
distance-contour-mapped
distance-sobel-edges
distance-emboss-light
gamma
asinh-stretch
histogram-equalized
clahe
black-point
screen-stretch
percentile-mapped
palette-mapped
false-color-palette
distance-exposure-hsv
flow-direction-hsv
flow-coherence
flow-current
contour-mapped
sigmoid-contrast
log1p-mapped
background-gradient-removal
local-contrast
gaussian-blur
bloom
star-reduction
star-mask
denoise-median
bilateral-smooth
deconvolution-sharpen
background-neutralize
saturation-boost
curves
sobel-edges
emboss-light
unsharp-mask
exposure-binned
exposure-hsv
hex-color
log-base-highest
mapped
smooth-step
```

Before each shader, `all` clears the canvas. Each shader failure is caught and logged, and the loop continues to the next shader.

## Practical Workflows

Render and save a GPU Buddhabrot dataset:

```powershell
.\bin\Debug\net10.0-windows\Sandbox.exe buddhabrot-gpu width=32768 height=32768 highest=255 cutoff=0 bailout=2000 gpu-progress-interval=16 render save
```

Load that dataset and generate all shader comparison PNGs:

```powershell
.\bin\Debug\net10.0-windows\Sandbox.exe buddhabrot-gpu load all
```

Load that dataset and draw one specific shader:

```powershell
.\bin\Debug\net10.0-windows\Sandbox.exe buddhabrot-gpu load distance-palette-mapped draw
```

Render a smaller GPU test:

```powershell
.\bin\Debug\net10.0-windows\Sandbox.exe buddhabrot-gpu width=1024 height=1024 highest=16 render save log1p-mapped draw
```

Render a classic GPU Buddhabrot with flow-field data and draw the current shader:

```powershell
.\bin\Debug\net10.0-windows\Sandbox.exe buddhabrot-classic-gpu width=1024 height=1024 cutoff=0 bailout=1500 render save flow-current draw
```

Render a CPU Buddhabrot and save every shader comparison:

```powershell
.\bin\Debug\net10.0-windows\Sandbox.exe buddhabrot width=2048 height=2048 highest=32 render save all
```

Render a Mandelbrot with exposure tone mapping and a canvas blur:

```powershell
.\bin\Debug\net10.0-windows\Sandbox.exe mandelbrot width=2048 height=2048 bailout=1000 render log1p-mapped gaussian-blur draw
```

## Notes and Limits

The default resolution is intentionally very large. For testing shaders or CLI behavior, use explicit smaller dimensions such as `width=1024 height=1024`.

`all` is expensive on large renders because it applies many shaders and writes many PNGs. Prefer `load <shader> draw` while iterating on a single shader.

Some shaders allocate additional full-image buffers. Very large images can require substantial system RAM even after the GPU render is complete.

## Code Layout

Shared fractal save/load/draw behavior lives in `Fractals/Fractal.cs`.

Shared shader state and helper functions live in `Fractals/Shader.cs`.

Each public shader algorithm lives in a separate partial class file under `Fractals/Shaders`.

The GPU Buddhabrot implementations live in `Fractals/BuddhabrotGPU.cs` and `Fractals/BuddhabrotClassicGPU.cs`.
