

Tick the boxes below for the implemented features. Add a brief note only if necessary, e.g., if it's only partially working, or how to turn it on.

Formalities:
[ ] This readme.txt
[ ] Cleaned (no obj/bin folders)
[ ] Demonstration scene(s) with all implemented features
[ ] (Optional) Screenshots: make it clear which feature is demonstrated in which screenshot

Minimum requirements implemented:
[ ] Camera: position and orientation controls
Controls: ...
[ ] Model matrix: for each mesh, stored as part of the scene graph
[ ] Scene graph data structure: tree hierarchy, no limitation on breadth or depth or size
[ ] Rendering: recursive scene graph traversal, correct model matrix concatenation
[ ] Shading in fragment shader: diffuse, glossy, uniform variable for ambient light color
[ ] Point light: at least 1, position/color may be hardcoded

Bonus features implemented:
[ ] Multiple point lights: at least 4, uniform variables to change position and color at runtime
[ ] Spot lights: position, center direction, opening angle, color
[ ] Environment mapping: cube or sphere mapping, used in background and/or reflections
[ ] Frustum culling: in C# code, using scene graph node bounds, may be conservative
[ ] Bump or normal mapping
[ ] Shadow mapping: render depth map to texture, only hard shadows required, some artifacts allowed
[ ] Vignetting and chromatic aberrations: darker corners, color channels separated more near corners
[ ] Color grading: color cube lookup table
[ ] Blur: separate horizontal and vertical blur passes, variable blur size
[ ] HDR glow: HDR render target, blur in HDR, tone-mapping
[ ] Depth of field: blur size based on distance from camera, some artifacts allowed
[ ] Ambient occlusion: darker in tight corners, implemented as screen-space post process
[ ] ...

Notes:
...
