using System;
using System.Collections.Generic;

namespace Raycasting_maze
{

    class Cell
    {
        private int row;
        private int col;

        private bool visited = false;
        public bool[] walls = {true, true, true, true};

        public Cell(int row, int col)
        {
            this.row = row;
            this.col = col;
        }

        public int GetRow()
        {
            return this.row;
        }

        public int GetCol()
        {
            return this.col;
        }

        public bool IsVisited()
        {
            return this.visited;
        }

        public void SetVisited(bool visited)
        {
            this.visited = visited;
        }

        public void RemoveWall(int wallIndex)
        {
            this.walls[wallIndex] = false;
        }

        public bool WallRemoved(int wallIndex)
        {
            return !this.walls[wallIndex];
        }

    }


    class MazeGenerator
    {
        private int numRows;
        private int numCols;
        
        private Cell[] mazeArray;
        
        private Random rnd = new Random();

        private const int Left = 0;
        private const int Top = 1;
        private const int Right = 2;
        private const int Bottom = 3;
        
        public MazeGenerator(int numRows, int numCols)
        {
            this.numRows = numRows;
            this.numCols = numCols;
            this.mazeArray = new Cell[this.numRows * this.numCols];
            
        }

        private void PopulateMaze()
        {
            for (int i = 0; i < this.mazeArray.Length; i++)
            {
                this.mazeArray[i] = new Cell(i / this.numCols, i % this.numCols);
            }
        }

        private int GetMazeArrayIndex(int row, int col)
        {
            return row * this.numCols + col;
        }

        private List<Cell> GetNeighbours(Cell currentCell)
        {
            List<Cell> neighbours = new List<Cell>();
            /// gjor finere og mer tydelig
            int leftCellIndex = currentCell.GetCol() > 0 ? this.GetMazeArrayIndex(currentCell.GetRow(), currentCell.GetCol() - 1) : -1;
            int topCellIndex = currentCell.GetRow() > 0 ? this.GetMazeArrayIndex(currentCell.GetRow() - 1, currentCell.GetCol()) : -1;
            int rightCellIndex = currentCell.GetCol() < this.numCols - 1 ? this.GetMazeArrayIndex(currentCell.GetRow(), currentCell.GetCol() + 1) : -1;
            int bottomCellIndex = currentCell.GetRow() < this.numRows - 1 ? this.GetMazeArrayIndex(currentCell.GetRow() + 1, currentCell.GetCol()): -1;

            if (leftCellIndex >= 0)
            {
                Cell leftCell = this.mazeArray[leftCellIndex];
                if (!leftCell.IsVisited()) 
                {
                    neighbours.Add(leftCell);
                }
            }
            if (topCellIndex >= 0)
            {
                Cell topCell = this.mazeArray[topCellIndex];
                if (!topCell.IsVisited())
                {
                    neighbours.Add(topCell);
                }
            }
            if (rightCellIndex >= 0)
            {
                Cell rightCell = this.mazeArray[rightCellIndex];
                if (!rightCell.IsVisited())
                {
                    neighbours.Add(rightCell);
                }
            }
            if (bottomCellIndex >= 0)
            {
                Cell bottomCell = this.mazeArray[bottomCellIndex];
                if (!bottomCell.IsVisited())
                {
                    neighbours.Add(bottomCell);
                }
            }
            return neighbours;
        }

        private void RemoveWallBetweenCells(Cell currentCell, Cell neighbourCell)
        {
            if (currentCell.GetRow() - neighbourCell.GetRow() == 0)
            {
                if (currentCell.GetCol() - neighbourCell.GetCol() < 0)
                {
                    currentCell.RemoveWall(Right);
                    neighbourCell.RemoveWall(Left);
                } 
                else 
                {
                    currentCell.RemoveWall(Left);
                    neighbourCell.RemoveWall(Right);
                }
            }
            else
            {
                if (currentCell.GetRow() - neighbourCell.GetRow() < 0)
                {
                    currentCell.RemoveWall(Bottom);
                    neighbourCell.RemoveWall(Top);
                }
                else
                {
                    currentCell.RemoveWall(Top);
                    neighbourCell.RemoveWall(Bottom);
                }
            }
            
        }

        /// using the random depth first search for maze generation
        public void GenerateMaze(int startRow, int startCol, int endRow, int endCol)
        {
            this.PopulateMaze();
            Stack<Cell> cellStack = new Stack<Cell>();
            Cell initCell = this.mazeArray[this.GetMazeArrayIndex(startRow, startCol)];
            initCell.SetVisited(true);
            cellStack.Push(initCell);

            while (cellStack.Count > 0)
            {
                Cell currentCell = cellStack.Pop();
                List<Cell> unvisitedNeighbours = this.GetNeighbours(currentCell);
                if (unvisitedNeighbours.Count > 0)
                {
                    cellStack.Push(currentCell);
                    int randomIndex = rnd.Next(unvisitedNeighbours.Count);
                    Cell neighbourCell = unvisitedNeighbours[randomIndex];
                    this.RemoveWallBetweenCells(currentCell, neighbourCell);
                    neighbourCell.SetVisited(true);
                    cellStack.Push(neighbourCell);
                }
            }
        }

        ///TODO:
        public int[,] GetMazeBitMap()
        {
            int[,] mazeBitMap = new int[this.numRows * 2 + 1, this.numCols * 2 + 1];
            // intiializing all as walls
            for (int i = 0; i < mazeBitMap.GetLength(0); i++){
                for (int j = 0; j < mazeBitMap.GetLength(1); j++){
                    mazeBitMap[i,j] = 1;
                }
            }

            for (int i = 0; i < this.mazeArray.Length; i++)
            {
                Cell cell = this.mazeArray[i];
                int bitMapRow = (i / this.numCols) * 2 + 1;
                int bitMapCol = (i % this.numCols) * 2 + 1;
                mazeBitMap[bitMapRow, bitMapCol] = 0;

                /// checking lef wall
                if (cell.WallRemoved(Left))
                {
                    mazeBitMap[bitMapRow, bitMapCol - 1] = 0;
                }

                /// cheking top wall
                if (cell.WallRemoved(Top))
                {
                    mazeBitMap[bitMapRow - 1, bitMapCol] = 0;
                }
            }
            return mazeBitMap;
        }

        
    }
}