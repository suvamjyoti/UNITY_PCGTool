
# Proc2D Level Generation Tool for Unity

a Unity tool for procedural level generation using the Wave Function Collapse algorithm. Users provide simple tile assets and define rules, guiding tile placement. The tool dynamically assembles levels by analyzing constraints, ensuring coherent patterns and layouts, and enabling efficient creation of diverse and customizable game environments from modular components.


## Acknowledgements

 - [Wave Function Collapse](https://github.com/mxgmn/WaveFunctionCollapse)
 - [Demo by Oskar Stålberg](https://oskarstalberg.com/game/wave/wave.html)



## Tech Stack

**Engine:** Unity Engine

**Programming:** C#


## Features

- Custom Unity Editor Integration
- Visual GUI with Sprite Support
- Rule Editing Capability
- Vanilla Mode: In basic mode, users specify the desired level dimensions (length and breadth), and the tool automatically generates levels of the specified size.
- User-Assisted Mode: This dynamic mode allows users to place specific tiles in desired locations manually. The tool generates the remaining areas while considering user-defined tile placements.


## Screenshots
### TileSet 1
![TileSet_1](https://raw.githubusercontent.com/suvamjyoti/UNITY_PCGTool/main/RAW/SquareTileSet.png)
![Level 1](https://raw.githubusercontent.com/suvamjyoti/UNITY_PCGTool/main/RAW/Type1_Screenshot(1).png)
![Level 2](https://raw.githubusercontent.com/suvamjyoti/UNITY_PCGTool/main/RAW/Type1_Screenshot(2).png)
![Level 3](https://raw.githubusercontent.com/suvamjyoti/UNITY_PCGTool/main/RAW/Type1_Screenshot(3).png)

---

### TileSet 2
![TileSet_2](https://raw.githubusercontent.com/suvamjyoti/UNITY_PCGTool/main/RAW/OrganicTileSet.png)
![Level 1](https://raw.githubusercontent.com/suvamjyoti/UNITY_PCGTool/main/RAW/Screenshot_(1).png)
![Level 2](https://raw.githubusercontent.com/suvamjyoti/UNITY_PCGTool/main/RAW/Screenshot_(2).png)
![Level 3](https://raw.githubusercontent.com/suvamjyoti/UNITY_PCGTool/main/RAW/Screenshot_(3).png)

## Demo

### Vanilla Mode: 
In basic mode, users specify the desired level dimensions (length and breadth), and the tool automatically generates levels of the selected size.
![Vanilla Mode gif](https://raw.githubusercontent.com/suvamjyoti/UNITY_PCGTool/main/RAW/VanillaMode.gif)

### User-Assisted Mode: 
This dynamic mode allows users to manually place specific tiles in desired locations. The tool generates the remaining areas while considering user-defined tile placements.
![Editor Mode gif](https://raw.githubusercontent.com/suvamjyoti/UNITY_PCGTool/main/RAW/EditorMode.gif)



## Lessons Learned

- Custom Editor tool creation
- Wave function collapse algorithm
- Grammer-based procedural generation
- Visualisation
- Design patterns: Singleton
- Better Project architecture
- Async/Await function
- MetaData creation

