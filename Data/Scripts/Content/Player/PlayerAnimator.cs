using OpenTK.Mathematics;
using System;

namespace GamJam2k21.PlayerElements
{
    public class PlayerAnimator
    {
        private Player player;
        private float frameRate;

        private Transform frontArmTansform = Transform.Default;
        private Transform backArmTransform = new Transform(Vector2.Zero, (1f, 1f));
        private Transform headTransform = Transform.Default;
        private Transform torsoTransform = Transform.Default;
        private Transform legsTransform = Transform.Default;

        private Animator headAnimator;
        private Animator frontArmAnimator;
        private Animator backArmAnimator;
        private Animator torsoAnimator;
        private Animator legsAnimator;

        private Transform pickaxeTransform = Transform.Default;
        private Transform backPickaxeTransform = Transform.Default;

        private Sprite pickaxeSprite;

        private float swingSpeed = 400.0f;

        private bool isSwinging = false;
        private bool inAir = false;
        private bool isWalkingBackwards = false;

        private string currentState = "idle";
        private string armState = "idle";

        private float diffAngle = 0.0f;
        private float diffAngleHead = 0.0f;

        private ParticleEmmiter walkDust;
        private ParticleEmmiter jumpParticles;
        private bool wasInAir = false;

        private bool isFlipped = false;

        public Sprite PickaxeSprite { set { pickaxeSprite = value; } }
        public bool IsSwinging
        {
            get => isSwinging;
            set { isSwinging = value; }
        }
        public bool InAir
        {
            get => inAir;
            set { inAir = value; }
        }
        public bool IsWalkingBackwards { get => isWalkingBackwards; }

        public PlayerAnimator(Player player, float frameRate)
        {
            walkDust = new ParticleEmmiter(ResourceManager.GetShader("particle"), ResourceManager.GetTexture("particle"), 2);
            jumpParticles = new ParticleEmmiter(ResourceManager.GetShader("particle"), ResourceManager.GetTexture("particle"), 4);

            this.player = player;
            this.frameRate = frameRate;

            pickaxeSprite = Sprite.Single(ResourceManager.GetTexture("pickaxe1"), (4.0f, 4.0f));

            setupAnimators();
        }

        private void setupAnimators()
        {
            headAnimator = new Animator(headTransform, frameRate);
            addAnimation(headAnimator, "idle", 16, "heroHead_idle", (1.0f, 1.0f), (16, 1));

            frontArmAnimator = new Animator(frontArmTansform, frameRate);
            addAnimation(frontArmAnimator, "idle", 4, "heroArmFront_idle", (2.0f, 2.0f), (16, 1));
            addAnimation(frontArmAnimator, "walk", 16, "heroArmFront_walk", (2.0f, 2.0f), (16, 1));
            addAnimation(frontArmAnimator, "dig", 1, "heroArmFront_dig", (2.0f, 2.0f), (16, 1));

            backArmAnimator = new Animator(backArmTransform, frameRate);
            addAnimation(backArmAnimator, "idle", 4, "heroArmBack_idle", (2.0f, 2.0f), (16, 1));
            addAnimation(backArmAnimator, "walk", 16, "heroArmBack_walk", (2.0f, 2.0f), (16, 1));
            addAnimation(backArmAnimator, "dig", 4, "heroArmBack_idle", (2.0f, 2.0f), (16, 1));

            torsoAnimator = new Animator(torsoTransform, frameRate);
            addAnimation(torsoAnimator, "idle", 16, "heroTorso_idle", (2.0f, 2.0f), (16, 1));

            legsAnimator = new Animator(legsTransform, frameRate);
            addAnimation(legsAnimator, "idle", 4, "heroLegs_idle", (2.0f, 2.0f), (16, 1));
            addAnimation(legsAnimator, "walk", 12, "heroLegs_walk", (2.0f, 2.0f), (16, 1));
            addAnimation(legsAnimator, "walkBackwards", 12, "heroLegs_walkBack", (2.0f, 2.0f), (16, 1));
        }

        private void addAnimation(Animator animator,
                                  string name,
                                  int frameCount,
                                  string textureName,
                                  Vector2 singleSpriteSize,
                                  Vector2i sheetSize)
        {
            Sprite sheet = Sprite.Sheet(
                ResourceManager.GetTexture(textureName),
                singleSpriteSize,
                sheetSize);
            animator.AddAnimation(name, sheet, frameCount);
        }

        public void Render()
        {
            renderBack();

            backArmAnimator.Render(armState);
            legsAnimator.Render(currentState);
            torsoAnimator.Render("idle");
            headAnimator.Render("idle");

            walkDust.Draw();
            jumpParticles.Draw();

            renderFront();
        }
        private void renderBack()
        {
            if (!isSwinging)
                pickaxeSprite.RenderWithTransform(backPickaxeTransform);
        }

        private void renderFront()
        {
            if (isSwinging)
                pickaxeSprite.RenderWithTransform(pickaxeTransform, Vector2i.Zero, isFlipped);
            frontArmAnimator.Render(armState);
        }

        public void Update()
        {
            updateState();
            updateElements();
        }

        private float stepCooldown = 0.45f;
        private float stepCooldownBase = 0.45f;
        private void updateState()
        {
            if (stepCooldown >= 0.0f)
                stepCooldown -= Time.DeltaTime;

            if (MathHelper.Abs(player.Velocity.Y) > 0.0f)
            {
                wasInAir = true;
            }
            if (MathHelper.Abs(player.Velocity.X) > 0.0f)
            {
                if (!isSwinging)
                {
                    armState = "walk";
                    if (player.Velocity.X < 0.0f)
                        isFlipped = true;
                    else
                        isFlipped = false;
                }
                if ((isFlipped && player.Velocity.X > 0.0f)
                 || (!isFlipped && player.Velocity.X < 0.0f))
                {
                    isWalkingBackwards = true;
                    currentState = "walkBackwards";
                }
                else
                {
                    isWalkingBackwards = false;
                    currentState = "walk";
                }
                if (player.Velocity.Y == 0.0f)
                {
                    float partVelX;
                    if (player.Velocity.X < 0.0f)
                        partVelX = 1.0f;
                    else
                        partVelX = -1.0f;
                    walkDust.SpawnParticles(
                        player.Position,
                        2,
                        (0.5f, 0.0f),
                        (0.7f, 0.7f, 0.7f),
                        (partVelX + player.Velocity.X, 1.0f),
                        (-0.1f, 1.0f),
                        true,
                        false,
                        false);

                    if (stepCooldown <= 0.0f)
                    {
                        SoundManager.PlayWalk(player.StandingOn);
                        stepCooldown = stepCooldownBase;
                    }
                }
            }
            else
            {
                armState = "idle";
                currentState = "idle";
            }

            if (isSwinging)
                armState = "dig";

            float legsAnimSpeed = isWalkingBackwards ? 0.7f : 1.3f;
            legsAnimator.Update(currentState, legsAnimSpeed);
            torsoAnimator.Update("idle");
            headAnimator.Update("idle");
            backArmAnimator.Update(armState, 1.2f);
            frontArmAnimator.Update(armState, 1.2f);

            updateFrontArm();
            walkDust.Update();
            jumpParticles.Update();
            if (wasInAir && !inAir)
                playLandingParticles();
        }
        public void PlayJumpParticles()
        {
            jumpParticles.SpawnParticles(player.Position,
                                         4,
                                         (0.5f, 0.0f),
                                         (0.7f, 0.7f, 0.7f),
                                         (0.0f, 0.2f),
                                         (10.0f, 1.0f),
                                         true,
                                         true,
                                         false);
        }
        private void playLandingParticles()
        {
            jumpParticles.SpawnParticles(player.Position,
                                         4,
                                         (0.5f, 0.0f),
                                         (0.7f, 0.7f, 0.7f),
                                         (0.0f, 0.2f),
                                         (10.0f, 1.0f),
                                         true,
                                         true,
                                         false);
            wasInAir = false;
            SoundManager.PlayWalk(player.StandingOn);
        }

        private void updateElements()
        {
            setElementsFlip();
            updateElementsPositions();
            updateFrontArm();
            lookAtCursor();
        }

        private void setElementsFlip()
        {
            legsAnimator.IsFlipped = isFlipped;
            torsoAnimator.IsFlipped = isFlipped;
            headAnimator.IsFlipped = isFlipped;
            backArmAnimator.IsFlipped = isFlipped;
            frontArmAnimator.IsFlipped = isFlipped;
        }

        private void updateElementsPositions()
        {
            legsTransform.Position = player.Position - (0.5f, 0.15f);
            torsoTransform.Position = player.Position - (0.5f, 0.15f);
            headTransform.Position = player.Position + (0.0f, 1.25f);
            backArmTransform.Position = player.Position - (getNewOffset(0.25f), -0.45f);
            frontArmTansform.Position = player.Position - (getNewOffset(0.75f), -0.45f);
            backPickaxeTransform.Position = player.Position
                - (2.12f, 0.9f)
                + (isFlipped ? 0.25f : -0.25f, 0.0f);
            backPickaxeTransform.Rotation = 45.0f;
            pickaxeTransform.Position = frontArmTansform.Position + (-1.0f, -1.0f);
        }

        private float getNewOffset(float offset)
        {
            return isFlipped ? -offset + player.Size.X : offset;
        }

        private void lookAtCursor()
        {
            headTransform.Rotation = player.Transform.Rotation;
            if (!isSwinging)
                return;
            Vector2 limits = isFlipped ? (-110.0f, -10.0f) : (10.0f, 110.0f);
            Vector2 fullLimits = isFlipped ? (-180.0f, 0.0f) : (0.0f, 180.0f);
            if (!angleIsInLimits(diffAngleHead, fullLimits))
                return;
            diffAngleHead = Math.Clamp(diffAngleHead, limits.X, limits.Y);
            headTransform.Rotation = newRotation(diffAngleHead, 70.0f);
        }

        private bool angleIsInLimits(float angle, Vector2 limits)
        {
            return angle >= limits.X && angle <= limits.Y;
        }

        private float newRotation(float angle, float difference)
        {
            if (isFlipped)
                difference *= -1.0f;
            return player.Transform.Rotation - angle + difference;
        }

        private readonly float INIT_SWING_ROT = 60.0f;
        private float swingRot = 60.0f;

        private void updateFrontArm()
        {
            swingRot -= Time.DeltaTime * swingSpeed / 3.0f;
            if (swingRot <= 0.0f)
                swingRot = INIT_SWING_ROT;

            frontArmTansform.Rotation = player.Transform.Rotation;
            if (!isSwinging)
                return;
            Vector2 limits = isFlipped ? (-180.0f, 0.0f) : (0.0f, 180.0f);
            if (!angleIsInLimits(diffAngle, limits))
                return;
            diffAngle = Math.Clamp(diffAngle, limits.X, limits.Y);
            frontArmTansform.Rotation = newRotation(diffAngle, 150.0f);
            frontArmTansform.Rotation += getSwingRot();
            pickaxeTransform.Rotation = frontArmTansform.Rotation + (isFlipped ? 35.0f : -35.0f);
        }

        private float getSwingRot()
        {
            if (isFlipped)
                return -swingRot;
            return swingRot;
        }

        public void GiveMouseLocation(Vector2 mouseLocation)
        {
            setDiffAngles(calculateDiffAngles(mouseLocation));
            flipBasedOnMouseLocation(mouseLocation);
        }

        private void setDiffAngles(Vector2 diffAngles)
        {
            diffAngle = diffAngles.X;
            diffAngleHead = diffAngles.Y;
        }

        private Vector2 calculateDiffAngles(Vector2 mouseLocation)
        {
            Vector2 diffVec = mouseLocation - player.Center;
            float angle = (float)MathHelper.Atan2(diffVec.X, diffVec.Y);
            angle = ((float)MathHelper.Floor(MathHelper.RadiansToDegrees(angle)));
            Vector2 diffVecHead = mouseLocation - player.HeadPosition;
            float angleHead = (float)MathHelper.Atan2(diffVec.X, diffVec.Y);
            angleHead = ((float)MathHelper.Floor(MathHelper.RadiansToDegrees(angleHead)));
            return (angle, angleHead);
        }

        private void flipBasedOnMouseLocation(Vector2 mouseLocation)
        {
            if (isSwinging)
                isFlipped = (mouseLocation.X < player.Center.X);
        }
    }
}
