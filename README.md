# Pirates - The curse of the Turn-Based Island
---
**Pirates** is a grid-based pathfinding game. The main quest is to reach the Gold Sea Bream Figurehead trying not to be caught by the evil skeletons that "live" in the Island.
The main character doesn't have any type of weapon, so he/she needs to be smart enough to avoid the enemies and reach the exits! The less the steps, the better: if you get the fastest path, you can win the golden coin of the level!


> [![](http://dl1.cbsistatic.com/i/r/2016/09/29/3cbdb32a-56f7-4c40-af96-a81379be93bb/thumbnail/32x32/f052b3326ac6ffd185296169cd4a8e2b/imgingest-3839042575700168882.png) Download the build (.zip) and try me!](https://drive.google.com/open?id=155v6cUe1yLnCDMd2HfPySx6nHVPwYymc)

![Pirates](https://static.wixstatic.com/media/639397_50e68451882341398c96e84d8af35d0c~mv2.png/v1/fill/w_710,h_400,al_c,usm_0.66_1.00_0.01/639397_50e68451882341398c96e84d8af35d0c~mv2.png)

## Controls

* **Arrow Keys** to move.
* **Spacebar** to skip a move.
* **Return** to confirm/skip texts.
* **Backspace** to return to previous menu (when in Pause).
* **R** to restart a level when died.

## Infos and Videos
You can visit the following link for videos and infos about the project and the team!
> [https://www.riccardoverza.com/pirates](https://www.riccardoverza.com/pirates)

We have:

* [Trailer](https://youtu.be/gTXLC-PbWbY)
* [Gameplay Video (Sub ENG)](https://youtu.be/J-8YwkhzeuU)

## Scripts
Here are some examples of scripts used in this project.

| Class | Description |
| ----- | ----------- |
| [GameManager](https://github.com/CabbageWarrior/Pirates/blob/master/Assets/0_ProjectData/Scripts/GameManager.cs) | We used the Unity Roguelike2D Sample Project as a base in order to manage turns and actions of enemies during the game. Our component also manages the game state, scene initialization and movement of enemies. |
| [Loader](https://github.com/CabbageWarrior/Pirates/blob/master/Assets/0_ProjectData/Scripts/Loader.cs) | This component fills the GameManager instance with all the infos about the current level. |
| [Tutorial](https://github.com/CabbageWarrior/Pirates/blob/master/Assets/0_ProjectData/Scripts/Tutorial.cs) | When properly included in the scene, this component manages the tutorial texts that appear when a level is loaded. |
| [MovingObject](https://github.com/CabbageWarrior/Pirates/blob/master/Assets/0_ProjectData/Scripts/MovingObject.cs) | Base class for all the moving objects in scene (player and enemies). |
| [> Player](https://github.com/CabbageWarrior/Pirates/blob/master/Assets/0_ProjectData/Scripts/Player.cs) | Manages player infos, movement system and interactions with elements in scene. |
| [> Enemy](https://github.com/CabbageWarrior/Pirates/blob/master/Assets/0_ProjectData/Scripts/Enemies/Enemy.cs) | Manages movement, rotation and aiming previsions for all the enemies. |
| [>> PatrollingEnemy](https://github.com/CabbageWarrior/Pirates/blob/master/Assets/0_ProjectData/Scripts/Enemies/PatrollingEnemy.cs) | Specified class for Patrolling Enemies. |
| [>> RangedEnemy](https://github.com/CabbageWarrior/Pirates/blob/master/Assets/0_ProjectData/Scripts/Enemies/RangedEnemy.cs) | Specified class for Ranged Enemies. |
