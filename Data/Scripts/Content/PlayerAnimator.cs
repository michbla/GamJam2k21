using System;
using System.Collections.Generic;
using OpenTK.Mathematics;
using System.Diagnostics;

namespace GamJam2k21
{
    public class PlayerAnimator : Animator
    {
        public GameObject frontArm = new GameObject((0.0f, 0.0f), (2.0f, 2.0f), ResourceManager.GetTexture("charArm1"));

        public GameObject backArm = new GameObject((0.0f, 0.0f), (1.8f, 1.8f), ResourceManager.GetTexture("empty"));
        public GameObject head = new GameObject((0.0f, 0.0f), (1.0f, 1.0f), ResourceManager.GetTexture("empty"));
        public GameObject torso = new GameObject((0.0f, 0.0f), (2.0f, 2.0f), ResourceManager.GetTexture("empty"));
        public GameObject legs = new GameObject((0.0f, 0.0f), (2.0f, 2.0f), ResourceManager.GetTexture("empty"));

        private Animator headAnim;
        private Animator frontArmAnim;
        private Animator backArmAnim;
        private Animator torsoAnim;
        private Animator legsAnim;


        public GameObject pickaxe = new GameObject((0.0f, 0.0f), (4.0f, 4.0f), ResourceManager.GetTexture("pickaxe1"));
        public GameObject backPickaxe = new GameObject((0.0f, 0.0f), (4.0f, 4.0f), ResourceManager.GetTexture("pickaxe1"));

        private readonly Vector2 armOffsetR = (-0.7f, 0.35f);
        private readonly Vector2 armOffsetL = (1.7f, 0.35f);
        public float armSpeed = 0.0f;

        public bool isDigging = false;
        public bool inAir = false;
        public bool isWalkingBackwards = false;

        private string currentState = "idle";
        private string armState = "idle";

        public float diffAngle = 0.0f;
        public float diffAngleHead = 0.0f;

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

            headAnim = new Animator(head, sha, size, rate);
            headAnim.AddAnimation("idle", ResourceManager.GetTexture("heroHead_idle"), 16);

            frontArmAnim = new Animator(frontArm, sha, size, rate);
            frontArmAnim.AddAnimation("idle", ResourceManager.GetTexture("heroArmFront_idle"), 4);
            frontArmAnim.AddAnimation("walk", ResourceManager.GetTexture("heroArmFront_walk"), 16);
            frontArmAnim.AddAnimation("dig", ResourceManager.GetTexture("heroArmFront_dig"), 1);

            backArmAnim = new Animator(backArm, sha, size, rate);
            backArmAnim.AddAnimation("idle", ResourceManager.GetTexture("heroArmBack_idle"), 4);
            backArmAnim.AddAnimation("walk", ResourceManager.GetTexture("heroArmBack_walk"), 16);
            backArmAnim.AddAnimation("dig", ResourceManager.GetTexture("heroArmBack_idle"), 4);

            torsoAnim = new Animator(torso, sha, size, rate);
            torsoAnim.AddAnimation("idle", ResourceManager.GetTexture("heroTorso_idle"), 16);

            legsAnim = new Animator(legs, sha, size, rate);
            legsAnim.AddAnimation("idle", ResourceManager.GetTexture("heroLegs_idle"), 4);
            legsAnim.AddAnimation("walk", ResourceManager.GetTexture("heroLegs_walk"), 12);
            legsAnim.AddAnimation("walkBackwards", ResourceManager.GetTexture("heroLegs_walkBack"), 12);
        }
        public void DrawFront(SpriteRenderer rend, Vector2 viewPos)
        {
            if (isDigging)
                pickaxe.Draw(rend, viewPos);
            frontArmAnim.Draw(armState, viewPos);
        }
        public void DrawPlayerAnimator(Vector2 viewPos)
        {
            //base.Draw(currentState, viewPos);

            backArmAnim.Draw(armState, viewPos);
            legsAnim.Draw(currentState, viewPos);
            torsoAnim.Draw("idle", viewPos);
            headAnim.Draw("idle", viewPos);

            walkDust.Draw(viewPos);
            jumpParticles.Draw(viewPos);
        }

        public void DrawBack(SpriteRenderer rend, Vector2 viewPos)
        {
            if (!isDigging)
                backPickaxe.Draw(rend, viewPos);
        }

        public void UpdatePlayerAnimator(float deltaTime)
        {
            updateLimbs();

            if (MathHelper.Abs(parent.velocity.Y) > 0.0f)
            {
                wasInAir = true;
            }
            if (MathHelper.Abs(parent.velocity.X) > 0.0f)
            {
                if (!isDigging)
                {
                    armState = "walk";
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
                armState = "idle";
                currentState = "idle";
            }

            if (isDigging)
                armState = "dig";

            float legsAnimSpeed = isWalkingBackwards ? 0.7f : 1.3f;
            legsAnim.Update(currentState, deltaTime, legsAnimSpeed);
            torsoAnim.Update("idle", deltaTime);
            headAnim.Update("idle", deltaTime);
            backArmAnim.Update(armState, deltaTime, 1.2f);
            frontArmAnim.Update(armState, deltaTime, 1.2f);

            base.Update(currentState, deltaTime);
            updateArm(deltaTime);
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
        public void UpdateDiffHead(float angle)
        {
            diffAngleHead = angle;
        }
        //Update reki
        private readonly float INIT_PICK_ROT = 60.0f;
        private float pickRot = 60.0f;
        private void updateArm(float deltaTime)
        {
            pickRot -= deltaTime * armSpeed / 3.0f;
            if (pickRot <= 0.0f)
                pickRot = INIT_PICK_ROT;

            if (isFlipped)
            {
                backPickaxe.position = parent.position + (-1.9f, -0.95f);
                backPickaxe.rotation = -40.0f;

                frontArm.size.X = -2.0f;
                frontArm.position = parent.position + armOffsetL;

                pickaxe.size.X = -4.0f;
                pickaxe.position = frontArm.position + (1.0f, -1.0f);
                if (isDigging)
                    frontArm.rotation = parent.rotation + diffAngle + 135.0f + pickRot;
                else
                    frontArm.rotation = parent.rotation;
                pickaxe.rotation = frontArm.rotation - 35.0f;
            }
            else
            {
                backPickaxe.position = parent.position + (-2.35f, -1.05f);
                backPickaxe.rotation = -50.0f;

                frontArm.size.X = 2.0f;
                frontArm.position = parent.position + armOffsetR;

                pickaxe.size.X = 4.0f;
                pickaxe.position = frontArm.position + (-1.0f, -1.0f);
                if (isDigging)
                    frontArm.rotation = parent.rotation + diffAngle - 135.0f - pickRot;
                else
                    frontArm.rotation = parent.rotation;
                pickaxe.rotation = frontArm.rotation + 35.0f;
            }
        }

        private void updateLimbs()
        {
            legsAnim.isFlipped = isFlipped;
            torsoAnim.isFlipped = isFlipped;
            headAnim.isFlipped = isFlipped;
            backArmAnim.isFlipped = isFlipped;

            if (!isFlipped)
            {
                legs.position = (parent.position.X - 0.45f, parent.position.Y - 0.15f);
                torso.position = (parent.position.X - 0.45f, parent.position.Y - 0.25f);
                head.position = (parent.position.X, parent.position.Y + 1.15f);
                backArm.position = (parent.position.X - 0.1f, parent.position.Y + 0.45f);

                if (isDigging)
                {
                    diffAngleHead = Math.Clamp(diffAngleHead, 10.0f, 110.0f);
                    head.rotation = parent.rotation + diffAngleHead - 70.0f;
                }
                else
                    head.rotation = parent.rotation;
            }
            else
            {
                legs.position = (parent.position.X + 0.45f, parent.position.Y - 0.15f);
                torso.position = (parent.position.X + 0.45f, parent.position.Y - 0.25f);
                head.position = (parent.position.X, parent.position.Y + 1.15f);
                backArm.position = (parent.position.X + 0.1f, parent.position.Y + 0.45f);

                if (isDigging)
                {
                    diffAngleHead = Math.Clamp(diffAngleHead, -110.0f, -10.0f);
                    head.rotation = parent.rotation + diffAngleHead + 70.0f;
                }
                else
                    head.rotation = parent.rotation;
            }
        }
    }
}
