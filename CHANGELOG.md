# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.0.0] - 2025-08-03

### Added
- Initial release of Pivot Adjuster for Unity
- 11 preset pivot positions (Center, Top/Bottom corners)
- Custom pivot point positioning
- Visual preview in Scene view with gizmos
- Two ways to use: dedicated window and MeshFilter inspector
- Automatic mesh asset creation with adjusted vertices
- Transform position adjustment to maintain world position
- Collider update support (Mesh, Box, Sphere, Capsule)
- Full Undo/Redo support
- Namespace organization (Vahaponur.PivotAdjuster)

### Features
- Changes mesh pivot by modifying vertex positions
- Preserves original mesh (creates new asset)
- Works with any mesh in Unity Editor
- Updates colliders automatically
- Maintains GameObject's visual position after pivot change