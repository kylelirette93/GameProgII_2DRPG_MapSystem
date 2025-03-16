# Game Programming I Oriented Map System #

## Overview ##

### This is a Object-Oriented Map System in Monogame. Implementing a mixture Object-oriented programming principles and Entity Component structure. ###

# Design Decisions #

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
  
- **AssetManager Class (Static)**
  The 'AssetManager' class is responsible for loading all the textures, sprites and sound files at once and storing them in a dictionary. Assets can be retrieved by their string name, ensuring that Assets are shared across game objects, saving memory.
  This class is static so that it acts as a global resource manager for assets, preventing multiple objects from loading the same asset.
  
- **Sprite Class (Derived)**
  The 'Sprite' class inherits from 'DrawableComponent' and is responsible for rendering a textures to the screen. It determines the sprite's bounds and updates based on the 'Transform' component's position.
  
- **Transform Class (Derived)**
  The 'Transform' class inherits from 'Component' and is used update position, rotation and scale of a game object. This class is central to the game object's behaviour, similar to Unity's 'Transform' component, which is attached
  to every game object.
  
- **Tilemap class (Derived)**
  The 'Tilemap' class inherits from 'DrawableComponent' and holds tile mappings in a dictionary of characters and tiles. It is responsible for generating a map, or reading from a file. Anything related to the actual tilemap component is in this class.
  It uses cellular automata and weighted random to generate clusters of obstacles with interesting paths.
  
- **Tile class**
  The 'Tile' class holds information about individual tiles, including properties such as whether the tile is walkable or an exit tile. When a 'Tilemap' component is created, its draw method is called, rendering the tile data.
  
- **MapManager class**
  The 'MapManager' class holds the current 'Tilemap' and is responsible for loading levels based on a list of level paths.  It also determines the player's spawn point by identifying the exit tile.
  When a new map is created the 'MapManager' replaces the current map with the new one, ensuring multiple maps aren't existing in memory.
  
- **Scene class**
  The 'Scene' class represents a scene in the game, like in Unity. A new scene is instantiated in the main program, which sets up the default scene with instances of the 'Tilemap' and 'Player'. The 'Scene' class works with the 'MapManager' to set the player's
  position and handle scene transtions (currently by loading a new map and updating the player's position).

- **TurnComponent class (Abstract)**
  The 'TurnComponent' class inherits from 'Component' and is responsible for enforcing an abstract 'TakeTurn' method. This is used in conjunction with the 'TurnManager' class to keep track of which 'GameObject's turn it is.

- **HealthComponent class (Derived)**
  The 'HealthComponent' class inherits from 'Component' and serves as a health system for any game object with the component. This class handles taking damage, creating an instance of an animation and a sound, as well as handling death.

- **AnimationComponent class (Derived)**
  The 'AnimationComponent' class inherits from 'DrawableComponent' and is used to play an animation by displaying a sequence of frames from a sprite sheet using a loop and drawing the iteration to the source rectangle.
  
- **TurnManager class**
  The 'TurnManager' class is responsible for managing turn-based actions in the game. It manages a queue of 'TurnComponent' objects, each representing an entity that can take a turn. A queue lock object is used to synchronize access to the turn queue, preventing race conditions in multithreaded scenarios, keeping the queue's internal state from becoming corrupted.

- **PlayerController class (Derived)**
  The 'PlayerController' class inherits from 'TurnComponent' and is responsible for controlling a player game object. It determines if the player can move to the next tile, based on the direction of input.

- **EnemyAI class (Derived)**
  The 'EnemyAI' class inherits from 'TurnComponent' and handles enemy behaviour based on the turn-based system. It manages three distinct states: 'Stunned', 'Follow', and 'Attack'. In the 'Follow' state, the enemy calculates the direction toward the player,
  converts this to tile-based movement and attempts to move. Before moving, it verifies the target tile is walkable and free of collisions. If adjacent to the player, the enemy than transitions to the 'Attack' state.

- **UIManager class**
  The 'UIManager' class is responsible for rendering text to the screen using a sprite font.

  
