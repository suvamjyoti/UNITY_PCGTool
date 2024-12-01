<h1 style="text-align: center;">Proc2d Level Gen Tool for Unity</h1>

<h3 style="text-align: center;">A procedural level generation tool for Unity</h3>

---

<h2>Objective</h2>
Develop a Unity tool for procedural level generation using the Wave Function Collapse algorithm. Users provide simple tile assets and define rules, guiding tile placement. The tool dynamically assembles levels by analyzing constraints, ensuring coherent patterns and layouts, and enabling efficient creation of diverse and customizable game environments from modular components.

---

<h2>Features</h2>

### 1. Custom Unity Editor Integration  
The tool is seamlessly integrated into the Unity editor, providing an efficient and user-friendly environment for procedural level generation.

### 2. Visual GUI with Sprite Support  
The graphical interface allows users to input and visualize 2D image sprites directly, enhancing the user experience with intuitive visuals.

### 3. Rule Editing Capability  
Users can define and modify tile placement rules within the custom editor, offering flexibility and control over the level generation process.

### 4. Vanilla Mode  
In basic mode, users specify the desired level dimensions (length and breadth), and the tool automatically generates levels of the specified size.

### 5. User-Assisted Mode  
This dynamic mode allows users to manually place specific tiles in desired locations. The tool generates the remaining areas while considering user-defined tile placements.


### Some Known Errors:

1. Do not press **Reset** during generation, as this will mess up the algorithm, and it won't work then.

2. The **Load** button does not work at launch. You need to save a level after opening it to load it; it works only before the application is closed.

3. After exiting Edit Mode, the camera stays in orthographic mode in Vanilla Mode as well.

4. The **Generate** button does not work in Edit Mode if it is pressed before **Start**.

