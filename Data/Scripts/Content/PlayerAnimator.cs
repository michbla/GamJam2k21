using System;
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
        public float armSpeed = 0.0f;

        public bool isDigging = false;
        public bool inAir = false;
        public bool isWalkingBackwards = false;

        private string currentState = "idle";

        public float diffAngle = 0.0f;

        private ParticleEmmiter walkDust;
        private ParticleEmmiter jumpParticles;

        private bool wasInAir = false;

        public PlayerAnimator(GameObject p, Shader sha, Vector2i size, float rate) : base(p, sha, size, rate)
        {
            AddAnimation("idle", ResourceManager.GetTexture("charIdle1"), 16);
            AddAnimation("walk", ResourceManager.GetTexture("charWalk1"), 8);
            AddAnimation("walkBackwards", ResourceManager.GetTexture("charWalkBack1"), 8);
            walkDust = new ParticleEmmiter(ResourceManager.GetShader("particle"), ResourceManager.GetTexture("particle"), 2);
            jumpParticles = new ParticleEmmiter(ResourceManager.GetShader("particle"), ResourceManager.GetTexture("particle"), 4);
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
            walkDust.Draw(viewPos);
            jumpParticles.Draw(viewPos);
        }
        //Update logiki
        public void UpdatePlayerAnimator(float deltaTime)
        {
            if (MathHelper.Abs(parent.velocity.Y) > 0.0f)
            {
                wasInAir = true;
            }
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
                if (parent.velocity.Y == 0.0f)
                {
                    float partVelX;
                    if (parent.velocity.X < 0.0f)
                        partVelX = 1.0f;
                    else
                        partVelX = -1.0f;
                    walkDust.SpawnParticles(parent.position, 2, (0.5f, 0.0f), (0.7f, 0.7f, 0.7f), (partVelX + parent.velocity.X, 1.0f), (-0.1f, 1.0f), true, false, false);
                }
            }
            else
            {
                currentState = "idle";
            }

            base.Update(currentState, deltaTime);
            UpdateArm(deltaTime);
            walkDust.Update(deltaTime);
            jumpParticles.Update(deltaTime);
            if (wasInAir && !inAir)
            {
                Land();
            }
        }
        public void Jump()
        {
            jumpParticles.SpawnParticles(parent.position, 4, (0.5f, 0.0f), (0.7f, 0.7f, 0.7f), (0.0f, 0.2f), (10.0f, 1.0f), true, true, false);
        }
        public void Land()
        {
            jumpParticles.SpawnParticles(parent.position, 4, (0.5f, 0.0f), (0.7f, 0.7f, 0.7f), (0.0f, 0.2f), (10.0f, 1.0f), true, true, false);
            wasInAir = false;
        }
        public void UpdateDiffAngle(float angle)
        {
            diffAngle = angle;
        }
        //Update reki
        private float initPickRot = 60.0f;
        private float pickRot = 60.0f;
        private void UpdateArm(float deltaTime)
        {
            pickRot -= deltaTime * armSpeed / 3.0f;
            if (pickRot <= 0.0f)
                pickRot = initPickRot;

            if (isFlipped)
            {
                backPickaxe.position = parent.position + (-1.9f, -0.95f);
                backPickaxe.rotation = -40.0f;

                playerArm.size.X = -2.0f;
                playerArm.position = parent.position + armOffsetL;

                pickaxe.size.X = -4.0f;
                pickaxe.position = playerArm.position + (1.0f, -1.0f);
                if (isDigging)
                    playerArm.rotation = parent.rotation + diffAngle + 135.0f + pickRot;
                else
                    playerArm.rotation = parent.rotation;
                pickaxe.rotation = playerArm.rotation - 35.0f;
            }
            else
            {
                backPickaxe.position = parent.position + (-2.35f, -1.05f);
                backPickaxe.rotation = -50.0f;

                playerArm.size.X = 2.0f;
                playerArm.position = parent.position + armOffsetR;

                pickaxe.size.X = 4.0f;
                pickaxe.position = playerArm.position + (-1.0f, -1.0f);
                if (isDigging)
                    playerArm.rotation = parent.rotation + diffAngle - 135.0f - pickRot;
                else
                    playerArm.rotation = parent.rotation;
                pickaxe.rotation = playerArm.rotation + 35.0f;
            }
        }
    }
}
