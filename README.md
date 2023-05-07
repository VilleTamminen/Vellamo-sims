# Vellamo-sims interior design tool
Vellamo-sims is a tech pilot made for Merikeskus Vellamo that emplyees can use as a interior design tool. 

# Documentation for scripts:
CheckBuilablePlacement checks if object is inside other objects to determine if it can be placed by BuildingManager. Also if object has MeasurePoints, this script makes an array of them for other scripts to use.

SelectManager handles selecting objects that are in the scene. Selected object can be moved, rotated, scaled and deleted. Selected object has an outline that is activated. Also if selected object has MeasurePoints, it moves MeasureLines to their position and calculates distance forward from those positions until the lines hit something.

BuildingManager handles object building and raycasting to find build spot. Swaps materials based on if object can be placed between green and red.

CheckBuilablePlacement checks if object is inside other objects. Also if object has MeasurePoints, this script makes an array of them for other scripts to use.

MeasureDistance is used by special object called MeasureTool, that displays distance between two points.

CameraController controls camera.

UIManager handles UI changes.

SaveGameManager handles saving and loading. Saves position, rotation and scale of buildable objects in the scene, and includes timestamp to save file.

SaveGameButton is needed by save game buttons to store a name to correct save file.

# Documentation for scene building:
Prefabs folder has important prefabs that use script mentioned above. Object prefabs are in Assets/Resources/Prefabs folder, because save system uses resources to find prefabs to load objects into the scene. 

We want to use correct scales in Unity. Either place pivot point in Blender to objects bottom, or create an empty object as root for pivot point in Unity. When exporting 3d-models from Blender to Unity, use 1. Apply scaling: FBX Unity Scale 2. Forward: -Z forward 3. Up: Y up 4. Check toggles for Apply Unit, Use Space Transform and Apply Transform. Check the Read/Write toggle for 3d-model import inside Unity.

Buildable object needs 1. Buildable-tag and Buildable-layer 2. a collider to set boundaries where object cannot cross other objects 3. CheckBuilablePlacement-script 4. Outline script (optional). Outline is free from Unity Asset Store 5. MeasurePoint children, that must be named correctly MeasurePoint1,2,3,4 (optional). Amount must be 4 if MeasurePoints are included.


There are multiple public variable. Some have script that finds correct object during Awake(), but there are some that need the object placed in inspector:

SelectManager needs SelectUI=SelectedObjectPanel, (X,Y,Z)ScaleInputs = InputField(X,Y,Z). 4 child objects named MeasureLine1,2,3 and 4 with Line Renderer components.

BuildingManager needs Grid Toggle, Wall Toggle (Under UI Canvas/MenuPanel/Toggles) and Materials=canPlaceMat and cantPlaceMat. Objects array has ALL OBJECT you want user to be able to use in the game. The Element order is applied to ObjectPanel scrollview buttons in UIManager script.

CameraPoint has CameraController script. MainCamera is the child. This allows moving and rotating camera. MouseMovementToggle is in (Under UI Canvas/MenuPanel/Toggles).

UIManager needs Floor Blueprint = main show ref copy. ContentParent=UI Canvas/ObjectPanel/ObjectPanelScrollView/Viewport/Content. SearhBar = UI Canvas/ObjectPanel/ObjectPanelScrollView/SearhBar

SaveGameManager needs SaveGameButtonPrefab = SaveGame from Prefabs folder. SaveGameButtonParent = UI Canvas/MenuPanel/SaveGameScrollView/Viewport/Content.


It is important that buildable objects are not made into children in hierarchy. Many scripts use root object of buildable objects.
