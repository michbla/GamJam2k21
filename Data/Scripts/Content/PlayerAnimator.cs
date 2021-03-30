using System.Collections.Generic;
using OpenTK.Mathematics;

namespace GamJam2k21
{
    public class PlayerAnimator : Animator
    {

        public GameObject playerArm = new GameObject((0.0f, 0.0f), (2.0f, 2.0f), ResourceManager.GetTexture("charArm1"));
        public GameObject pickaxe = new GameObject((0.0f, 0.0f), (4.0f, 4.0f), ResourceManager.GetTexture("pickaxe1"));
        public GameObject backPickaxe = new GameObject((0.0f, 0.0f), (4.0f, 4.0f), ResourceManager.GetTexture("pickaxe1"));

        private Vector2 armOffsetR = (-0.7f, 0.35f);
        private Vector2 armOffsetL = (1.7f, 0.35f);
        private float initialArmRot = 0.0f;
        private float stoppingArmRot = 0.0f;
        public float armSpeed = 0.0f;

        public bool isDigging = false;
        public bool inAir = false;
        public bool isWalkingBackwards = false;

        private string currentState = "idle";

        public float diffAngle = 0.0f;

        public PlayerAnimator(GameObject p, Shader sha, Vector2i size, float rate) : base(p, sha, size, rate)
        {
            AddAnimation("idle", ResourceManager.GetTexture("charIdle1"), 16);
            AddAnimation("walk", ResourceManager.GetTexture("charWalk1"), 8);
            AddAnimation("walkBackwards", ResourceManager.GetTexture("charWalkBack1"), 8);
        }
        //Rysowanie tla
        public void DrawBack(SpriteRenderer rend, Vector2 viewPos)
        {
            if (!isDigging)
                backPickaxe.Draw(rend, viewPos);
        }
        //Rysowanie frontu
        public void DrawFront(SpriteRenderer rend, Vector2 viewPos)
        {
            if (isDigging)
                pickaxe.Draw(rend, viewPos);
            playerArm.Draw(rend, viewPos);
        }
        //Rysowanie animacji
        public void DrawPlayerAnimator(Vector2 viewPos)
        {
            base.Draw(currentState, viewPos);
        }
        //Update logiki
        public void UpdatePlayerAnimator(float deltaTime)
        {
            if (MathHelper.Abs(parent.velocity.X) > 0.0f)
            {
                if (!isDigging)
                {
                    if (parent.velocity.X < 0.0f)
                        isFlipped = true;
                    else
                        isFlipped = false;
                }
                if ((isFlipped && parent.velocity.X > 0.0f) || (!isFlipped && parent.velocity.X < 0.0f))
                {
                    isWalkingBackwards = true;
                    currentState = "walkBackwards";
                }
                else
                {
                    isWalkingBackwards = false;
                    currentState = "walk";
                }
            }
            else
            {
                currentState = "idle";
            }

            base.Update(currentState, deltaTime);
            UpdateArm(deltaTime);
        }
        public void UpdateDiffAngle(float angle)
        {
                diffAngle = angle;
        }
        //Update reki
        private void UpdateArm(float deltaTime)
        {
            if (isFlipped)
            {
                backPickaxe.position = parent.position + (-1.8f, -0.65f);
                backPickaxe.rotation = -40.0f;

                playerArm.size.X = -2.0f;
                playerArm.position = parent.position + armOffsetL;

                pickaxe.size.X = -4.0f;
                pickaxe.position = playerArm.position + (1.0f, -1.0f);
                if (isDigging)
                {
                    initialArmRot = 225.0f + diffAngle;
                    stoppingArmRot = 105.0f + diffAngle;
                    playerArm.rotation -= parent.rotation + deltaTime * armSpeed / 1.5f;
                    if (playerArm.rotation < stoppingArmRot)
                    {
                        playerArm.rotation = initialArmRot + parent.rotation;
                    }
                }
                else
                {
                    playerArm.rotation = parent.rotation;
                }
            }
            else
            {
                backPickaxe.position = parent.position + (-2.25f, -0.75f);
                backPickaxe.rotation = -50.0f;

                playerArm.size.X = 2.0f;
                playerArm.position = parent.position + armOffsetR;
                pickaxe.size.X = 4.0f;
                pickaxe.position = playerArm.position + (-1.0f, -1.0f);
                if (isDigging)
                {
                    initialArmRot = -225.0f + diffAngle;
                    stoppingArmRot = -105.0f + diffAngle;
                    playerArm.rotation += parent.rotation + deltaTime * armSpeed / 1.5f;
                    if (playerArm.rotation > stoppingArmRot)
                    {
                        playerArm.rotation = initialArmRot + parent.rotation;
                    }
                }
                else
                {
                    playerArm.rotation = parent.rotation;
                }
            }
            pickaxe.rotation = playerArm.rotation;
        }
    }
}
