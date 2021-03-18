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
                Size = new Vector2i(1280, 720),
                Title = "Gam Jam Lato 2K21",
            };
            //Ustawienia odswiezania
            var gameWindowSettings = new GameWindowSettings()
            {
               // RenderFrequency = 60,
                //UpdateFrequency = 60,
            };
            //Odpalanie okna
            using (var game = new Game(gameWindowSettings, nativeWindowSettings))
            {
                game.Run();
            }
        }
    }
}
