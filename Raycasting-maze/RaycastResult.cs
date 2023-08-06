namespace Raycasting_maze;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
public struct RaycastingResult
    {
        public int TextureId {get; set;}
        public int X {get; set;}

        public int Y {get; set;}

        public int Width {get; set;}

        public int Height {get; set;}

        public float PresentageAllongTheWall {get; set;}

        public Color Shaiding {get; set;}

        public RaycastingResult(int textureId, int x, int y, int width, int height, float persentageAllongTheWall, Color shaiding)
        {
            TextureId = textureId;
            X = x;
            Y = y;
            Width = width;
            Height = height;
            PresentageAllongTheWall = persentageAllongTheWall;
            Shaiding = shaiding;
        }
    }