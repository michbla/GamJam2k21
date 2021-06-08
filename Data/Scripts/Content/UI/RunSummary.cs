using OpenTK.Mathematics;
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

        private Text SCOREBOARD;
        private Text TIME_SCORE;
        private Text rank;

        private Button SummaryRankSwitchButton;
        private Button ExitGameButton;
        private Button NewGameButton;
        private Icon background;
        private Icon RankCanvas;

        private RankingLoader rankingLoader;
        private NativeWindow ns;
        private Player player;
        private Time t;

        private int PositionInRanking;
        private bool isNewScoreSet = false;
        private bool isSummary = true;
        private bool isRanking = false;
        public RunSummary(NativeWindow _ns, Player _player)
        {
            GAME_OVER = new Text((-6.7f, 3.5f), "GAME OVER", TextType.ui, 1.5f);
            SCOREBOARD = new Text((-7.5f, 3.5f), "SCOREBOARD", TextType.ui, 1.5f);
            TIME_SCORE = new Text((-7.2f, 2.5f), "TIME    SCORE", TextType.ui, 0.70f);
            PLAYTIME = new Text((-6.7f, 2.5f), "Run Time: ", TextType.ui, 0.75f);
            EXP = new Text((-6.7f, 1.5F), "EXP: ", TextType.ui, 0.75f);
            Exp = new Text((1f, 1.5F), " ", TextType.ui, 0.75f);
            PlayTime = new Text((1f, 2.5f), Time.GetTime(), TextType.ui, 0.75f);
            background = new Icon((-9f, -6f), Sprite.Single(ResourceManager.GetTexture("UI_back_empty"), (18f, 12f)));
            RankCanvas = new Icon((-10.5f, 0.9f), Sprite.Single(ResourceManager.GetTexture("itemFrame"), (14f, 1.8f)));
            rank = new Text((-6f, 2.5f), " ", TextType.ui, 0.5f);

            SummaryRankSwitchButton = new Button((-1.5F, -4F), (3, 11), "this run", TextType.ui);
            ExitGameButton = new Button((-4.5F, -5.5F), (3, 11), "Exit Game", TextType.ui);
            NewGameButton = new Button((1.5F, -5.5F), (3, 11), "New Game", TextType.ui);

            t = Time.GetInstance();
            ns = _ns;
            player = _player;
            rankingLoader = new RankingLoader();
            rankingLoader.LoadRanking();
        }

        public void Update()
        {
            string currTime = Time.GetTime();
            float currExp = player.stats.Exp;
            
            PlayTime.UpdateText(currTime);
            Exp.UpdateText(currExp.ToString());
            SummaryRankSwitchButton.Update(MouseLocation);
            ExitGameButton.Update(MouseLocation);
            NewGameButton.Update(MouseLocation);
            
            if (SummaryRankSwitchButton.CanPerformAction())
            { 
                isRanking = !isRanking;
                isSummary = !isSummary;
            }
            if (ExitGameButton.CanPerformAction())
                ns.Close();
            if (NewGameButton.CanPerformAction())
                Game.state = GameState.postgame;
            

            if (isRanking)
            {
                SummaryRankSwitchButton.UpdateText("This run");
            }
            else
                SummaryRankSwitchButton.UpdateText("Ranking");
            if (!isNewScoreSet)
                {
                    PositionInRanking = rankingLoader.AddToRanking(currTime, currExp);
                    rankingLoader.SaveRanking();
                    isNewScoreSet = true;
                }
            
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
            else
            {
                SCOREBOARD.Render(Camera.GetScreenCenter());
                TIME_SCORE.Render(Camera.GetScreenCenter());
                Vector2 RankCanvasOffset;
                if (PositionInRanking >= 0 && PositionInRanking < 3)
                    RankCanvasOffset = (0f, 0f + PositionInRanking);
                else
                {
                    RankCanvasOffset = (0f, 4f);
                    rank.UpdateOffset((-7f, -2.5f));
                    Text dots = new Text((-7f, -1.5f), "...", TextType.ui, 0.5f);
                    dots.Render(Camera.GetScreenCenter());
                    rank.UpdateText(PositionInRanking + 1 + ": " + rankingLoader.GetRank(PositionInRanking));
                    rank.Render(Camera.GetScreenCenter());
                }

                RankCanvas.Render(Camera.GetScreenCenter() - RankCanvasOffset);
                for (int i=0;i<3;i++)
                {   
                    rank.UpdateOffset((-7f, 1.5f - i));
                    rank.UpdateText(i+1 + ": " + rankingLoader.GetRank(i));
                    
                    rank.Render(Camera.GetScreenCenter());
                }
                    
                
            }

            SummaryRankSwitchButton.Render(Camera.GetScreenCenter());
            ExitGameButton.Render(Camera.GetScreenCenter());
            NewGameButton.Render(Camera.GetScreenCenter());

        }
    }
}

