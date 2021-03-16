using System;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;

namespace GamJam2k21
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("KOPALNIARZE AUUUUU");

            //Ustawienia okna
            var nativeWindowSettings = new NativeWindowSettings()
            {
                Size = new Vector2i(800, 600),
                Title = "Gam Jam Lato 2K21",
            };
            //Ustawienia odswierzania
            var gameWindowSettings = new GameWindowSettings()
            {
                //RenderFrequency = 60,
                //UpdateFrequency = 60,
            };
        }
    }
}
