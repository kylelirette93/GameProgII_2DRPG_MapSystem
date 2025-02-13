#Game Programming I Oriented Map System#

##Overview##

###This is a Object-Oriented Map System in Monogame. Implementing a mixture Object-oriented programming principles and Entity Component structure.###

#Design Decisions#

- **GameObject Class**
  I created a 'GameObject' class that represents a game entity, similar to Unity. This class allows components to be added to or removed from the object dynamically. 
  The components are stored in a list and can be iterated through to update or manage their behaviour.
  
- **GameObjectFactory Class (Static)**
  I chose to create a factory class to encapsulate game object creation and gives control over what components belong to game objects. Basically a blueprint class for creating game objects, ensuring they are properly instantiated with the necessary components.
  I chose to make this class static so I don't have to create an instance of it each time I need a new game object, reducing overhead. It provides easy access to creation methods, just like prefabs in Unity.
  
- **ObjectManager Class**
  The 'ObjectManager' manages a list of game objects/entities. It allows for adding or removing game objects and calling their 'Draw()' and 'Update()' methods, ensuring they're updating each frame. 
  
- **Component Class (Abstract)**
  The 'Component' class serves as a base class for various components that can be attached to game objects. This structure allows for modularity, where compoents can be added, removed or modified without altering the game object itself.
  
- **DrawableComponent Class (Derived)**
  This 'DrawableComponent' class inherits from 'Component' and adds 'Draw()' method for components that require it. This was suggested by my instructor.
  
- **Collider Class (Derived)**
  The 'Collider' class inherits from 'DrawableComponent' and is responsible for handling collider's on game objects. The collider's bounds are determined by the sprite's bounds, similar to Unity.
  Although there isn't a full collision system yet, this class lays the groundwork for handling collisions in the future.
  
- **SpriteManager Class (Static)**
  The 'SpriteManager' class is responsible for loading all the textures at once and storing them in a dictionary. Textures can be retrieved by their string name, ensuring that textures are shared across game objects, saving memory.
  This class is static so that it acts as a global resource manager for textures, preventing multiple objects from loading the same texture.
  
- **Sprite Class (Derived)**
  The 'Sprite' class inherits from 'DrawableComponent' and is responsible for rendering a textures to the screen. It determines the sprite's bounds and updates based on the 'Transform' component's position.
  
- **Transform Class (Derived)**
  The 'Transform' class inherits from 'Component' and is used update position, rotation and scale of a game object. This class is central to the game object's behaviour, similar to Unity's 'Transform' component, which is attached
  to every game object.
  
- **Tilemap class (Derived)**
  The 'Tilemap' class inherits from 'DrawableComponent' and holds tile mappings in a dictionary of characters and tiles. It is responsible for generating a map, or reading from a file. Anything related to the actual tilemap component is in this class.
  
- **Tile class**
  The 'Tile' class holds information about individual tiles, including properties such as whether the tile is walkable or an exit tile. When a 'Tilemap' component is created, its draw method is called, rendering the tile data.
  
- **MapManager class**
  The 'MapManager' class holds the current 'Tilemap' and is responsible for loading levels based on a list of level paths.  It also determines the player's spawn point by identifying the exit tile.
  When a new map is created the 'MapManager' replaces the current map with the new one, ensuring multiple maps aren't existing in memory.
  
- **Scene class**
  The 'Scene' class represents a scene in the game, like in Unity. A new scene is instantiated in the main program, which sets up the default scene with instances of the 'Tilemap' and 'Player'. The 'Scene' class works with the 'MapManager' to set the player's
  position and handle scene transtions (currently by loading a new map and updating the player's position).
  
