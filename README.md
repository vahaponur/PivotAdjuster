# Pivot Adjuster

A Unity Editor tool that allows you to change the pivot point of meshes by modifying vertex positions.

## Features

- Adjust mesh pivot points with 11 preset positions
- Custom pivot point positioning
- Visual preview in Scene view
- Undo/Redo support
- Works with any mesh in Unity
- Automatic mesh asset creation

## Installation

1. In Unity, go to Window > Package Manager
2. Click the + button and select "Add package from git URL"
3. Enter: `https://github.com/vahaponur/pivotadjuster.git`

Or add to your `manifest.json`:
```json
"com.vahaponur.pivotadjuster": "https://github.com/vahaponur/pivotadjuster.git"
```

## Usage

### Method 1: Pivot Adjuster Window
1. Go to Tools > Pivot Adjuster
2. Select your GameObject with a MeshFilter
3. Choose a preset or enter custom coordinates
4. Click "Apply Pivot Adjustment"

### Method 2: MeshFilter Inspector
1. Select a GameObject with a MeshFilter
2. In the Inspector, expand "Pivot Adjuster" section
3. Choose preset and click "Apply Pivot"

## Pivot Presets

- **Center**: Center of the mesh bounds
- **TopCenter**: Top center of the mesh
- **TopFront/Back/Left/Right**: Top corners
- **BottomCenter**: Bottom center of the mesh  
- **BottomFront/Back/Left/Right**: Bottom corners
- **Custom**: Manual pivot position

## Notes

- Creates a new mesh asset with adjusted vertices
- Original mesh is preserved
- GameObject position is adjusted to maintain visual position
- Works only in Unity Editor (not runtime)

## Requirements

- Unity 2021.3 or newer

## License

MIT License