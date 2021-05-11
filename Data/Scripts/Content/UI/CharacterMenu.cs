using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace GamJam2k21.Interface
{
    public class CharacterMenu
    {
        public Vector2 MouseLocation;
        private Player player;

        private readonly int attributeCount = 4;

        private Icon background;

        private Text[] attributeNames;
        private Button[] attributeButtons;

        private Text[] attributePoints;

        private Text pointsToSpend = new Text((3.8f, 2.2f), " 0", TextType.ui, 0.8f);

        public CharacterMenu(Player player)
        {
            this.player = player;

            background = new Icon((-6.0f, -4.5f), Sprite.Single(ResourceManager.GetTexture("UI_back_char"), (12.0f, 8.0f)));

            attributeNames = new Text[attributeCount];
            attributeButtons = new Button[attributeCount];
            attributePoints = new Text[attributeCount];

            attributeNames[0] = new Text((-5.5f, 1.0f), "SPEED", TextType.ui, 0.6f);
            attributeNames[1] = new Text((-5.5f, -0.5f), "ENERGY", TextType.ui, 0.6f);
            attributeNames[2] = new Text((-5.5f, -2.0f), "REGEN", TextType.ui, 0.6f);
            attributeNames[3] = new Text((-5.5f, -3.5f), "LUCK", TextType.ui, 0.6f);

            for (int i = 0; i < attributeCount; i++)
                attributePoints[i] = new Text((-1.8f, 1.0f - i * 1.5f), "1110000000", TextType.ui_icon, 0.6f);

            for (int i = 0; i < attributeCount; i++)
                attributeButtons[i] = new Button((4.5f, 0.8f - i * 1.5f), (1, 1), "+", TextType.ui_icon);
        }

        public void Update()
        {
            foreach (var button in attributeButtons)
                button.Update(MouseLocation);

            attributePoints[0].UpdateText(pointsToString(player.Skills.SpeedPoints));
            attributePoints[1].UpdateText(pointsToString(player.Skills.EnergyPoints));
            attributePoints[2].UpdateText(pointsToString(player.Skills.RegenPoints));
            attributePoints[3].UpdateText(pointsToString(player.Skills.LuckPoints));

            pointsToSpend.UpdateText(skillPointsString());

            for (int i = 0; i < attributeCount; i++)
                if (attributeButtons[i].CanPerformAction())
                    tryAddAttribute(i);
        }

        private string pointsToString(int points)
        {
            string result = "";
            for (int i = 0; i < points; i++)
                result += '1';
            while (result.Length != 10)
                result += '0';
            return result;
        }

        private string skillPointsString()
        {
            if (player.Skills.SkillPoints < 10)
                return " " + player.Skills.SkillPoints;
            return player.Skills.SkillPoints.ToString();
        }

        private void tryAddAttribute(int attribId)
        {
            if (player.Skills.SkillPoints < 1)
                return;
            if (getAttribById(attribId) >= 10)
                return;
            player.Skills.SkillPoints--;
            addAttribById(attribId);
        }

        private int getAttribById(int attribId)
        {
            switch (attribId)
            {
                case 0:
                    return player.Skills.SpeedPoints;
                case 1:
                    return player.Skills.EnergyPoints;
                case 2:
                    return player.Skills.RegenPoints;
                case 3:
                    return player.Skills.LuckPoints;
            }
            return 0;
        }

        private void addAttribById(int attribId)
        {
            switch (attribId)
            {
                case 0:
                    player.Skills.SpeedPoints++;
                    break;
                case 1:
                    player.Skills.EnergyPoints++;
                    break;
                case 2:
                    player.Skills.RegenPoints++;
                    break;
                case 3:
                    player.Skills.LuckPoints++;
                    break;
            }
        }

        public void Render()
        {
            background.Render(Camera.GetScreenCenter());

            foreach (var name in attributeNames)
                name.Render(Camera.GetScreenCenter());

            foreach (var points in attributePoints)
                points.Render(Camera.GetScreenCenter());

            if (player.Skills.SkillPoints > 0)
            {
                foreach (var button in attributeButtons)
                    button.Render(Camera.GetScreenCenter());

                pointsToSpend.Render(Camera.GetScreenCenter());
            }
        }
    }
}
