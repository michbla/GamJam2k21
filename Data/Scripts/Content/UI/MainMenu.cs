using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Windowing.Desktop;


namespace GamJam2k21.Interface
{
    public class MainMenu
    {
        public Vector2 MouseLocation;

        private Icon background;
        private Text logo;

        private Button newGame;
        private Button leaderBoard;
        private Button options;
        private Button exit;
        private Button back;

        private NativeWindow ns;
        private Player player;

        private RankingLoader Rl;
        private Settings settings;

        private string MENU_STATE;

        public MainMenu(NativeWindow _ns, Player _player)
        {
            background = new Icon((-9f, -6f), Sprite.Single(ResourceManager.GetTexture("UI_back_options"), (18f, 12f)));
            logo = new Text((-8.2f, 3.5f), "KOPALNIARZE", TextType.ui, 1.5f);
            newGame = new Button((-1.5f, 1.5f), (3, 11), "New Game", TextType.ui);
            leaderBoard = new Button((-1.5f, 0f), (3, 11), "Leaderboard", TextType.ui);
            options = new Button((-1.5f, -1.5f), (3, 11), "Options", TextType.ui);
            exit = new Button((-1.5f, -3f), (3, 11), "Exit Game", TextType.ui);
            back = new Button((-1.5f, -4.5f), (3, 11), "D", TextType.ui_icon);

            MENU_STATE = "menu";
            ns = _ns;
            player = _player;
            Rl = new RankingLoader();
            settings = new Settings(ns, (1.5f, 1.5f));
        }

        public void Update()
        {
            if (MENU_STATE == "menu")
            {
                newGame.Update(MouseLocation);
                leaderBoard.Update(MouseLocation);
                options.Update(MouseLocation);
                exit.Update(MouseLocation);
            }

            else if (MENU_STATE == "settings")
            {
                settings.Update();
                settings.MouseLocation = MouseLocation;
                back.Update(MouseLocation);
            }
            else
                back.Update(MouseLocation);

            
            if (newGame.CanPerformAction())
            {
                Game.state = GameState.active;
            }
            if (leaderBoard.CanPerformAction())
            {
                Rl.LoadRanking();
                MENU_STATE = "lboard";
            }
            if (options.CanPerformAction())
            {
                MENU_STATE = "settings";
                
            }
            if (exit.CanPerformAction())
            {
                ns.Close();
            }
            if (back.CanPerformAction())
            {
                MENU_STATE = "menu";
            }
        }

        private void RenderLB()
        {
            Text SCOREBOARD = new Text((-7.5f, 3.5f), "SCOREBOARD", TextType.ui, 1.5f);
            Text TIME_SCORE = new Text((-7.2f, 2.5f), "TIME    SCORE", TextType.ui, 0.70f);
            Text rank = new Text((-6f, 2.5f), " ", TextType.ui, 0.5f);

            
            SCOREBOARD.Render(Camera.GetScreenCenter());
            TIME_SCORE.Render(Camera.GetScreenCenter());
            for (int i = 0; i < 3; i++)
            {
                rank.UpdateOffset((-7f, 1.5f - i));
                rank.UpdateText(i + 1 + ": " + Rl.GetRank(i));

                rank.Render(Camera.GetScreenCenter());
            }
            back.Render(Camera.GetScreenCenter());
        }

        public void Render()
        { 
            background.Render(Camera.GetScreenCenter());
            if (MENU_STATE == "menu")
            {
                logo.Render(Camera.GetScreenCenter());
                newGame.Render(Camera.GetScreenCenter());
                leaderBoard.Render(Camera.GetScreenCenter());
                options.Render(Camera.GetScreenCenter());
                exit.Render(Camera.GetScreenCenter());
            }
            if (MENU_STATE == "lboard")
            {
                RenderLB();
            }
            if (MENU_STATE == "settings")
            {
                settings.Render();
                back.Render(Camera.GetScreenCenter());
            }
        }
    }
}
