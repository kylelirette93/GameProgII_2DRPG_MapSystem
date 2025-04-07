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
  The 'TurnManager' class is responsible for managing turn-based actions in the game. It manages a queue of classes that implement the ITurnTaker interface, each representing a game object that can take a turn.
  
- **ITurnTaker class**
  The 'ITurnTaker' class is responsible for enforcing a common interface on any turn takers. It's implemented in the 'PlayerController', 'BaseEnemyAI' and any classes that derived from 'BaseEnemyAI'.
  
- **PlayerController class (Derived)**
  The 'PlayerController' class inherits from 'TurnComponent' and is responsible for controlling a player game object. It determines if the player can move to the next tile, based on the direction of input.

- **BaseEnemyAI class (Derived)**
  The 'BaseEnemyAI' class inherits from 'Component' and implements the ITurnTaker interface. It manages three distinct states: 'Stunned', 'Follow', and 'Attack'. In the 'Follow' state, the enemy calculates the direction toward the player,
  converts this to tile-based movement and attempts to move. Before moving, it verifies the target tile is walkable and free of collisions. If adjacent to the player, the enemy than transitions to the 'Attack' state.

  - **MeleeEnemyAI class (Derived)**
  The 'MeleeEnemyAI' class inherits from 'BaseEnemyAI'.

  - **RangedEnemyAI class (Derived)**
  The 'RangedEnemyAI' class inherits from 'BaseEnemyAI'. It uses most of the methods that 'BaseEnemyAI' implements, however it overrides update to introduce a 'RangedAttack' State. If the player is within the line of sight, the ranged
  enemy fires a projectile at the player.

  - **BossEnemyAI class (Derived)**
    The 'BossEnemyAI' class inherits from 'RangedEnemyAI'. It uses most of the methods that 'RangedEnemyAI' implements, however it override initialization to get a reference to the Boss 'GameObject's 'Component's. Additionally, it implements
    a different turn cycle than the ranged enemy. It predicts it's next action, based on states and display's an icon above it's head before taking an action. The Boss takes one of 4 random actions, Idle, Move, Shoot or Charge.

  - **PathFinder class**
    The 'PathFinder' class is responsible for finding the shortest path from the enemy to the player. It implements the A* pathfinding algorithm to find the shortest path based on a grid-based map, represented by 'PathNode' objects.

  - **PathNode class**
    The 'PathNode' class stores information about each node in the grid, including it's position, walkability and costs used in the algorithm.

  - **UIManager class**
  The 'UIManager' class is responsible for drawing and updating various menus and drawing text to the screen.

  ## Game Flow Implementation ##
  This section describes the implementation of the game's flow, including transitions between menus, levels and game states.

  ### Main Menu ###
  The main menu provides options to start in adventure mode with levels loaded from a file. After the third level a boss fight level is loaded.

  **Pause Menu:** Activated by pressing the 'ESC' key, it allows the player to resume gameplay or quit.
  **Game Over:** Triggered when the player's health reaches zero, displaying a game over screen with options to return to main menu or quit.
  **Victory:** Triggered when the player kills the boss, displaying a victory screen with options to return to main menu or quit.

  ## Boss Battle Implementation ##

  ### Boss AI ###
  The boss AI is implemented in the 'BossEnemyAI' class, which is derived from the 'RangedEnemyAI'.
  The boss implements a state machine of it's own, which four states, Idle, Move, Shoot, Charge.
  Before each action, it *predicts* what it will do on it's turn, before the boss makes a move. It does this by storing it's next action in order to use it for displaying an icon. Than the boss takes the previously stored action. It's an illusion! No really.
  If the predicted action is Idle, he does nothing.
  If the predicted action is to Move, it uses pathfinding to move one point.
  If the predicted action is to Shoot, it first checks if the player is in the line of sight. If the player is it shoots, otherwise it readjusts direction.
  If the predicted action is to charge, it get's the closest direction towards the player, and charges in that direction based on charge distance, which is 3 moves.
  The 'DisplayIcon' Component is used to visually indicate the boss's upcoming action. It gets drawn slightly above the boss's head and the delay for the icon is based on the boss's turn delay.

  ## Visual Improvements Added ##
  - Player now has an idle animation.
  - All scrolls now have animation and sound effects. Additionally drinking a potion makes a sound.
  - Buttons change between normal and grey color when hovering over them.
  - Tried to implement tile shaking effect when the boss charged, but failed to integrate it with boss enemy ai. Having multiple timers was causing issues.
  - Instructions are displayed on the right panel, positioned under inventory. It informs the player how to play, and also what level they are on. 
