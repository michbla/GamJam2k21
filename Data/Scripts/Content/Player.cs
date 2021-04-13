using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Collections.Generic;
using System;


namespace GamJam2k21
{

    /// <summary>
    /// Klasa obiektu gracza
    /// </summary>
    public class Player : GameObject
    {
        public Vector2 playerCenter;
        public Vector2 playerHeadPos;

        private readonly float WALK_SPEED = 8.0f;
        private readonly float BACKWARDS_WALK_SPEED_MULTIPLIER = 0.65f;
        private readonly float CLIMB_SPEED = 10.0f;
        private readonly float JUMP_FORCE = 8.0f;

        private readonly float FALL_MULTIPLIER = 0.01f;
        private readonly float LOW_JUMP_MULTIPLIER = 0.5f;
        private float rememberGrounded = 0.0f;
        private bool isJumping = false;
        private bool isFloating = false;
        private readonly float gravity = 30.0f;

        private int lowestPosition = 1;
        public PlayerStatistics PlayerStatistics;
        public Inventory eq;

        public bool canMove = true;
        public bool isGrounded = true;

        public List<Block> blocks;

        private BoxCollider bottomBox;
        private BoxCollider upperBox;
        private BoxCollider rightBox;
        private BoxCollider leftBox;
        private Vector2 collisionPos;
        private bool[] hasColl = { false, false, false, false };
        //0 - top, 1 - right, 2 - bottom, 3 - left

        private PlayerAnimator playerAnimator;
        private readonly float animFrameRate = 12.0f;

        private float diggingSpeed = 0.0f;
        public bool isReadyToDamage = true;
        private float diggingCooldown = 0.0f;
        private readonly float DIG_CD_BASE = 10.0f;

        public Pickaxe equippedPickaxe;

        public Player(Vector2 pos, Vector2 size, Texture sprite) : base(pos, size, sprite)
        {
            velocity = (0.0f, 0.0f);
            playerCenter = new Vector2(position.X + size.X / 2.0f, position.Y + size.Y / 2.0f);

            bottomBox = new BoxCollider(this, new Vector2(0.3f, 0.0f), new Vector2(0.4f, 0.2f));
            upperBox = new BoxCollider(this, new Vector2(0.3f, 1.6f), new Vector2(0.4f, 0.2f));
            leftBox = new BoxCollider(this, new Vector2(0.00f, 0.05f), new Vector2(0.4f, 1.75f));
            rightBox = new BoxCollider(this, new Vector2(0.6f, 0.05f), new Vector2(0.4f, 1.75f));
            collisionPos = pos;

            PlayerStatistics = new PlayerStatistics(0, 0, 0);
            eq = new Inventory();

            playerAnimator = new PlayerAnimator(this, ResourceManager.GetShader("sprite"), (16, 1), animFrameRate);
            SetPickaxe(0);
        }

        public override void Draw(SpriteRenderer rend, Vector2 viewPos)
        {
            playerAnimator.DrawBack(rend, viewPos);
            playerAnimator.DrawPlayerAnimator(viewPos);
            playerAnimator.DrawFront(rend, viewPos);
        }

        public void UpdatePlayer(KeyboardState input, MouseState mouseInput, float deltaTime, Vector2 mousePos)
        {
            //TEMP:: Klawisze do testowania
            //-------------------------------------------------------------
            if (input.IsKeyDown(Keys.K))
            {
                isFloating = true;
            }
            else
            {
                isFloating = false;
            }

            if (input.IsKeyDown(Keys.D0))
            {
                SetPickaxe(0);
            }
            else if (input.IsKeyDown(Keys.D1))
            {
                SetPickaxe(1);
            }
            else if (input.IsKeyDown(Keys.D2))
            {
                SetPickaxe(2);
            }
            else if (input.IsKeyDown(Keys.D3))
            {
                SetPickaxe(3);
            }
            else if (input.IsKeyDown(Keys.D4))
            {
                SetPickaxe(4);
            }
            else if (input.IsKeyDown(Keys.D5))
            {
                SetPickaxe(5);
            }
            //-------------------------------------------------------------

            ResetBounds();
            CheckCollisions();

            playerCenter = (position.X + size.X / 2.0f, position.Y + size.Y / 2.0f);
            playerHeadPos = (playerCenter.X, playerCenter.Y + 0.5f);

            SetMaxPlayerDepth();

            playerAnimator.armSpeed = equippedPickaxe.speed;
            diggingSpeed = equippedPickaxe.speed;

            doMovement(deltaTime, input);
            doGravity(deltaTime, input);
            doCollisions(deltaTime);

            if (playerAnimator.isWalkingBackwards)
                velocity.X *= BACKWARDS_WALK_SPEED_MULTIPLIER;

            position.X = Math.Clamp(position.X + velocity.X * deltaTime, 0.0f, 127.0f);
            position.Y += velocity.Y * deltaTime;

            upperBox.Update();
            bottomBox.Update();
            rightBox.Update();
            leftBox.Update();

            CheckCollisions();

            if (hasColl[2] && position.Y < collisionPos.Y)
                position.Y = collisionPos.Y;
            //Drabina mechanic
            if (isFloating && canMove)
            {
                velocity.Y = 0.0f;
                if (input.IsKeyDown(Keys.W) && !hasColl[0])
                    position.Y += deltaTime * CLIMB_SPEED;
                else if (input.IsKeyDown(Keys.S) && !hasColl[2])
                    position.Y -= deltaTime * CLIMB_SPEED;
            }

            Vector2 diffVec = mousePos - playerCenter;
            double angle = MathHelper.Atan2(diffVec.X, diffVec.Y);
            playerAnimator.UpdateDiffAngle((float)MathHelper.Floor(MathHelper.RadiansToDegrees(angle)));
            Vector2 diffVecHead = mousePos - playerHeadPos;
            double angleHead = MathHelper.Atan2(diffVec.X, diffVec.Y);
            playerAnimator.UpdateDiffHead((float)MathHelper.Floor(MathHelper.RadiansToDegrees(angleHead)));
            playerAnimator.isDigging = mouseInput.IsButtonDown(MouseButton.Button1);
            playerAnimator.inAir = !isGrounded;
            playerAnimator.UpdatePlayerAnimator(deltaTime);

            if (diggingCooldown > 0.0f)
                diggingCooldown -= deltaTime * diggingSpeed / 10.0f;
            else
                isReadyToDamage = true;
        }

        public void ResetCooldown()
        {
            diggingCooldown = DIG_CD_BASE;
            isReadyToDamage = false;
        }

        public void SetFlip(bool flip)
        {
            playerAnimator.isFlipped = flip;
        }

        private void CheckCollisions()
        {
            foreach (var block in blocks)
                if (block.distanceToPlayer <= 2f && !block.isDestroyed)
                    CheckCollision(block);
        }

        public void ResetBounds()
        {
            isGrounded = false;
            collisionPos = position;
            for (var i = 0; i < 4; i++)
                hasColl[i] = false;
        }

        public void CheckCollision(GameObject collider)
        {
            (bool, Direction, Vector2) upRes = Collider.CheckBoxCollision(upperBox, collider);
            if(upRes.Item1 == true)
                hasColl[0] = true;
            (bool, Direction, Vector2) rRes = Collider.CheckBoxCollision(rightBox, collider);
            if(rRes.Item1 == true)
                hasColl[1] = true;
            (bool, Direction, Vector2) botRes = Collider.CheckBoxCollision(bottomBox, collider);
            if (botRes.Item1 == true)
            {
                isGrounded = true;
                collisionPos.Y = collider.position.Y + collider.size.Y;
                hasColl[2] = true;
            }
            (bool, Direction, Vector2) lRes = Collider.CheckBoxCollision(leftBox, collider);
            if(lRes.Item1 == true)
                hasColl[3] = true;
        }
        private void SetMaxPlayerDepth()
        {
            if (lowestPosition > position.Y && isGrounded)
            {
                lowestPosition = (int)position.Y;
                PlayerStatistics.addLevelReached();
            }
        }
        public float GetDamage()
        {
            return equippedPickaxe.damage;
        }

        public int GetHardness()
        {
            return equippedPickaxe.hardness;
        }

        public void SetPickaxe(int id)
        {
            equippedPickaxe = ResourceManager.GetPickaxeByID(id);
            playerAnimator.pickaxe.sprite = equippedPickaxe.sprite;
            playerAnimator.backPickaxe.sprite = equippedPickaxe.sprite;
        }
        public bool IsDigging()
        {
            return playerAnimator.isDigging;
        }

        private void doMovement(float deltaTime, KeyboardState input)
        {
            if (!canMove)
                return;
            float vel = WALK_SPEED;
            if (!isGrounded)
            {
                vel *= 0.8f;
                if (rememberGrounded > 0.0f)
                    rememberGrounded -= deltaTime;
            }
            else
            {
                rememberGrounded = 0.1f;
                isJumping = false;
            }

            if (input.IsKeyDown(Keys.A))
                velocity.X = -vel;
            else if (input.IsKeyDown(Keys.D))
                velocity.X = vel;
            else
                velocity.X = 0.0f;

            if (rememberGrounded > 0.0f && input.IsKeyDown(Keys.Space) && !isFloating)
            {
                velocity.Y = 1.0f * JUMP_FORCE;
                isJumping = true;
                playerAnimator.Jump();
            }
        }

        private void doGravity(float deltaTime, KeyboardState input)
        {
            if (!isGrounded && !isFloating)
            {
                velocity.Y -= gravity * deltaTime;

                if (velocity.Y < 0.0f)
                    velocity.Y += gravity * (FALL_MULTIPLIER - 1) * deltaTime;
                else if (velocity.Y > 0.0f && !input.IsKeyDown(Keys.Space))
                    velocity.Y += gravity * (LOW_JUMP_MULTIPLIER - 1) * deltaTime;
            }
        }

        private void doCollisions(float deltaTime)
        {
            if (hasColl[0] && velocity.Y > 0.0f)
                velocity.Y = -gravity * deltaTime;

            if (!isJumping && hasColl[2] && !isFloating)
            {
                position.Y = collisionPos.Y;
                velocity.Y = 0.0f;
            }

            if (hasColl[1] && velocity.X > 0.0f)
                velocity.X = 0.0f;
            if (hasColl[3] && velocity.X < 0.0f)
                velocity.X = 0.0f;
        }
    }
}
