# pb_Stl

An STL exporter for Unity.

**pb_Stl** is the exporter used in ProBuilder to save [STL](https://en.wikipedia.org/wiki/STL_(file_format) model files.

## Quick Start

1. Select a `GameObject` in the Scene View with a `MeshFilter` and valid `Mesh`.
1. In the file menu,  `Edit > Export > STL (Ascii)`.
1. View your shiny new STL file.

## Features

- Export binary & ascii STL files.
- Option to convert from left to right handed coordinates (on by default, as per STL spec).

## Planned Improvements

- Import Ascii / Binary.
- Automatic merging of multiple meshes
- Options to swap model axis.

## Troubleshooting

#### Model is sideways / horizontal / rotated

Unity's coordinate system is left handed, with Y axis as the vertical.  Other 3d modeling programs may have different coordinate systems or axis assignments.  **pb_Stl** exports right handed coordinates by default, but can be modified to retain left handed coordinates.  Support for swapping axes is planned.

#### Only One Mesh is Exported

Currently exporting is restricted to the first selected mesh.  In the future support for automatically merging multiple meshes will be implemented.
