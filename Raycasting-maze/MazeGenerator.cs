using System;
using System.Collections.Generic;

namespace Raycasting_maze
{

    class Cell
    {
        private readonly int Row;
        private readonly int Col;

        private bool Visited = false;
        public bool[] Walls = { true, true, true, true };

        public Cell(int row, int col)
        {
            Row = row;
            Col = col;
        }

        public int GetRow()
        {
            return Row;
        }

        public int GetCol()
        {
            return Col;
        }

        public bool IsVisited()
        {
            return Visited;
        }

        public void SetVisited(bool visited)
        {
            Visited = visited;
        }

        public void RemoveWall(int wallIndex)
        {
            Walls[wallIndex] = false;
        }

        public bool WallRemoved(int wallIndex)
        {
            return !Walls[wallIndex];
        }

    }


    class MazeGenerator
    {
        private readonly int NumRows;
        private readonly int NumCols;

        private readonly Cell[] MazeArray;

        private readonly Random Rnd = new Random();

        private const int Left = 0;
        private const int Top = 1;
        private const int Right = 2;
        private const int Bottom = 3;

        public MazeGenerator(int numRows, int numCols)
        {
            NumRows = numRows;
            NumCols = numCols;
            MazeArray = new Cell[NumRows * NumCols];

        }

        private void PopulateMaze()
        {
            for (int i = 0; i < MazeArray.Length; i++)
            {
                MazeArray[i] = new Cell(i / NumCols, i % NumCols);
            }
        }

        private int GetMazeArrayIndex(int row, int col)
        {
            return row * NumCols + col;
        }

        private List<Cell> GetNeighbours(Cell currentCell)
        {
            List<Cell> neighbours = new List<Cell>();
            /// gjor finere og mer tydelig
            int leftCellIndex = currentCell.GetCol() > 0 ? GetMazeArrayIndex(currentCell.GetRow(), currentCell.GetCol() - 1) : -1;
            int topCellIndex = currentCell.GetRow() > 0 ? GetMazeArrayIndex(currentCell.GetRow() - 1, currentCell.GetCol()) : -1;
            int rightCellIndex = currentCell.GetCol() < NumCols - 1 ? GetMazeArrayIndex(currentCell.GetRow(), currentCell.GetCol() + 1) : -1;
            int bottomCellIndex = currentCell.GetRow() < NumRows - 1 ? GetMazeArrayIndex(currentCell.GetRow() + 1, currentCell.GetCol()) : -1;

            if (leftCellIndex >= 0)
            {
                Cell leftCell = MazeArray[leftCellIndex];
                if (!leftCell.IsVisited())
                {
                    neighbours.Add(leftCell);
                }
            }
            if (topCellIndex >= 0)
            {
                Cell topCell = MazeArray[topCellIndex];
                if (!topCell.IsVisited())
                {
                    neighbours.Add(topCell);
                }
            }
            if (rightCellIndex >= 0)
            {
                Cell rightCell = MazeArray[rightCellIndex];
                if (!rightCell.IsVisited())
                {
                    neighbours.Add(rightCell);
                }
            }
            if (bottomCellIndex >= 0)
            {
                Cell bottomCell = MazeArray[bottomCellIndex];
                if (!bottomCell.IsVisited())
                {
                    neighbours.Add(bottomCell);
                }
            }
            return neighbours;
        }

        private static void RemoveWallBetweenCells(Cell currentCell, Cell neighbourCell)
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
            PopulateMaze();
            Stack<Cell> cellStack = new Stack<Cell>();
            Cell initCell = MazeArray[GetMazeArrayIndex(startRow, startCol)];
            initCell.SetVisited(true);
            cellStack.Push(initCell);

            while (cellStack.Count > 0)
            {
                Cell currentCell = cellStack.Pop();
                List<Cell> unvisitedNeighbours = GetNeighbours(currentCell);
                if (unvisitedNeighbours.Count > 0)
                {
                    cellStack.Push(currentCell);
                    int randomIndex = Rnd.Next(unvisitedNeighbours.Count);
                    Cell neighbourCell = unvisitedNeighbours[randomIndex];
                    RemoveWallBetweenCells(currentCell, neighbourCell);
                    neighbourCell.SetVisited(true);
                    cellStack.Push(neighbourCell);
                }
            }
        }

        ///TODO:
        public int[,] GetMazeBitMap()
        {
            int[,] mazeBitMap = new int[NumRows * 2 + 1, NumCols * 2 + 1];
            // intiializing all as walls
            for (int i = 0; i < mazeBitMap.GetLength(0); i++)
            {
                for (int j = 0; j < mazeBitMap.GetLength(1); j++)
                {
                    mazeBitMap[i, j] = 1;
                }
            }

            for (int i = 0; i < MazeArray.Length; i++)
            {
                Cell cell = MazeArray[i];
                int bitMapRow = (i / NumCols) * 2 + 1;
                int bitMapCol = (i % NumCols) * 2 + 1;
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