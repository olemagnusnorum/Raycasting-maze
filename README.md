# Raycasting-maze
A C# program using random depth first algorithm for generating a bitmap of a random maze and using raycasting to visualize it in a pseudo 3D world.

Thanks to [Lode's Computer Graphics Tutorial](https://lodev.org/cgtutor/raycasting.html) blog for the introduction to basic raycasting and raycasting with textures.

The program generates a random maze represented as a 2D array and displays the current position of the player as an orange square.

![Map of the maze](/imgs/MapFullScreen.png)

By pressing **T** on the keyboard the view changes to the pseudo 3D perspective of the player in the maze with a minimap of the maze and player position in the top left corner.

![pseudo 3D view 1](/imgs/Maze1.png)

![pseudo 3D view 2](/imgs/Maze2.png)

Using raycasting makes it easy to make new levels just by changing the 2D array.

![pseudo 3D view 2](/imgs/Maze3.png)


**Note**: The floor rendering in this project is not efficient. For levels with open spaces, the floor rendering should be removed and replaced a colored background or one large texture covering the bottom half of the screen with the walls painted over.