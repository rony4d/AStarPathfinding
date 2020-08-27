using System;
using System.Collections.Generic;
using System.Text;

namespace AStarAlgorithm
{
    public static class Text
    {
        
        /// <summary>
        /// Adds new line and color to the message
        /// </summary>
        /// <param name="message"></param>
        /// <param name="color"></param>
        public static void WriteLine(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine($"{ message}\n");
            Console.ForegroundColor = ConsoleColor.White;   // reset color

        }

        /// <summary>
        /// Prints on the same line and adds color
        /// </summary>
        /// <param name="message"></param>
        /// <param name="color"></param>
        public static void Write(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.Write($"{ message}");
            Console.ForegroundColor = ConsoleColor.White;   // reset color

        }
    }
}
