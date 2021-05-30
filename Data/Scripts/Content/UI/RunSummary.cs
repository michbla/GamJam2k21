using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;

namespace GamJam2k21.Interface
{
    class RunSummary
    {
        public Vector2 MouseLocation;
        private Text GAME_OVER;
        private Text PLAYTIME;
        private Text PlayTime;
        private Text EXP;
        private Text Exp;

        private Button SummaryRankSwitchButton;
        private Button ExitGameButton;
        private Icon background;

        private NativeWindow ns;
        private Player player;
        private Time t;

        private bool isSummary = true;
        private bool isRanking = false;
        public RunSummary(NativeWindow _ns, Player _player)
        {
            GAME_OVER = new Text((-6.7f, 3.5f), "GAME OVER", TextType.ui, 1.5f);
            PLAYTIME = new Text((-6.7f, 2.5f), "Run Time: ", TextType.ui, 0.75f);
            EXP = new Text((-6.7f, 1.5F), "EXP: ", TextType.ui, 0.75f);
            Exp = new Text((1f, 1.5F), " ", TextType.ui, 0.75f);
            PlayTime = new Text((1f, 2.5f), Time.GetTime(), TextType.ui, 0.75f);
            background = new Icon((-9f, -6f), Sprite.Single(ResourceManager.GetTexture("UI_back_options"), (18f, 12f)));

            SummaryRankSwitchButton = new Button((-5F, -4F), (3, 11), "this run", TextType.ui);
            ExitGameButton = new Button((-1.5F, -5.5F), (3, 11), "Exit Game", TextType.ui);

            t = Time.GetInstance();
            ns = _ns;
            player = _player;
        }

        public void Update()
        {
            //Console.WriteLine("xD");
            PlayTime.UpdateText(Time.GetTime());
            Exp.UpdateText(player.stats.Exp.ToString());
            SummaryRankSwitchButton.Update(MouseLocation);
            ExitGameButton.Update(MouseLocation);

            if (SummaryRankSwitchButton.CanPerformAction())
            { 
                isRanking = !isRanking;
                isSummary = !isSummary;
            }
            if (ExitGameButton.CanPerformAction())
                ns.Close();

            if (isRanking)
                SummaryRankSwitchButton.UpdateText("This run");
            else
                SummaryRankSwitchButton.UpdateText("Ranking");
            
            Console.WriteLine(isSummary + " " + isRanking);
        }

        public void Render()
        {
            background.Render(Camera.GetScreenCenter());           
            if (isSummary)
            {
                GAME_OVER.Render(Camera.GetScreenCenter());
                PLAYTIME.Render(Camera.GetScreenCenter());
                EXP.Render(Camera.GetScreenCenter());
                Exp.Render(Camera.GetScreenCenter());
                PlayTime.Render(Camera.GetScreenCenter());
            }

            SummaryRankSwitchButton.Render(Camera.GetScreenCenter());
            ExitGameButton.Render(Camera.GetScreenCenter());

        }
    }
}
