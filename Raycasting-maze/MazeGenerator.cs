using System;

namespace raycastingmaze
{
    class MazeGenerator
    {
        int numRows;
        int numCols;
        
        int[,] bitMapMaze;
        

        public MazeGenerator(int numRows, int numCols)
        {
            this.numRows = numRows;
            this.numCols = numCols;
            bitMapMaze = new int[numRows, numCols];
        }

        public int[,] GenerateBitMapMaze()
        {
            return bitMapMaze;
        }

        public void Print()
        {
            for (int i = 0; i < this.bitMapMaze.GetLength(0); i++)
            {
                for (int j = 0; j < this.bitMapMaze.GetLength(1); j++)
                {
                    Console.Write(this.bitMapMaze[i,j]);
                }
                Console.WriteLine();
            }
        }
    }
}