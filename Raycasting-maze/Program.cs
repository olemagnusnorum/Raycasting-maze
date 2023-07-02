// See https://aka.ms/new-console-template for more information
using System;
namespace raycastingmaze
{
    class Program 
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            MazeGenerator a = new MazeGenerator(3,3);
            a.Print();
        }
    }
}


