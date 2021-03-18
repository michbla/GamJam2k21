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
        //Predkosc gracza
        private float playerSpeed = 8.0f;
        //Sila skoku
        private float jumpForce = 8.0f;
        //Zmienne do plynnego skoku
        private float fallMultiplier = 0.01f;
        private float lowJumpMultiplier = 0.5f;

        private float rememberGrounded = 0.0f;

        private int playerPoints = 0;
        private int lastPointChange = 0;

        //Flagi
        public bool canMove = true;
        public bool isGrounded = true;

        //Collidery (kapsula)
        private BoxCollider bottomBox;
        private BoxCollider upperBox;
        private BoxCollider rightBox;
        private BoxCollider leftBox;

        private Vector2 collisionPos;

        private bool hasBotColl = false;
        private bool hasTopColl = false;
        private bool hasLeftColl = false;
        private bool hasRightColl = false;


        private bool isJumping = false;

        List<Block> blocks;

        //Grawitacja
        private float gravity = 30.0f;
        //Ostatnia pozycja gracza
        private Vector2 lastPlayerPos;
        //Kostruktor
        public Player(Vector2 pos, Vector2 size, Texture sprite) : base(pos, size, sprite)
        {
            lastPlayerPos = pos;
            velocity = (0.0f, 0.0f);
            playerCenter = new Vector2(position.X + size.X / 2.0f, position.Y + size.Y / 2.0f);

            bottomBox = new BoxCollider(this, new Vector2(0.3f, 0.0f), new Vector2(0.4f, 0.2f));
            upperBox = new BoxCollider(this, new Vector2(0.3f, 1.6f), new Vector2(0.4f, 0.2f));
            leftBox = new BoxCollider(this, new Vector2(0.00f, 0.05f), new Vector2(0.4f, 1.75f));
            rightBox = new BoxCollider(this, new Vector2(0.6f, 0.05f), new Vector2(0.4f, 1.75f));

            collisionPos = pos;
            
        }
        public void SetBlocks(List<Block> b)
        {
            blocks = b;
        }
        //Logika gracza
        public override void Update(KeyboardState input, float deltaTime)
        {
            
            playerPoints = MathHelper.Abs((int)lastPlayerPos.Y);
            if (playerPoints != lastPointChange && !isGrounded)
            {
                lastPointChange = playerPoints;
                Console.WriteLine(playerPoints);
            }
            
            foreach (var block in blocks)
            {
                if (block.distanceToPlayer <= 2f && !block.isDestroyed)
                    CheckCollision(block);
            }

            playerCenter = (position.X + size.X / 2.0f, position.Y + size.Y / 2.0f);


            if (canMove)
            {
                float vel = playerSpeed;
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
                {
                    velocity.X = -vel;
                }
                else if (input.IsKeyDown(Keys.D))
                {
                    velocity.X = vel;
                }
                else
                {
                    velocity.X = 0.0f;
                }

                if (rememberGrounded > 0.0f && input.IsKeyDown(Keys.Space))
                {
                    velocity.Y = 1.0f * jumpForce;
                    isJumping = true;
                }
            }
            //Grawitacja tylko jesli w powietrzu
            if (!isGrounded)
            {
                velocity.Y -= gravity * deltaTime;

                if (velocity.Y < 0.0f)
                {
                    velocity.Y += gravity * (fallMultiplier - 1) * deltaTime;
                }
                else if (velocity.Y > 0.0f && !input.IsKeyDown(Keys.Space))
                {
                    velocity.Y += gravity * (lowJumpMultiplier - 1) * deltaTime;
                }
            }

            foreach (var block in blocks)
            {
                if (block.distanceToPlayer <= 2f && !block.isDestroyed)
                    CheckCollision(block);
            }

            if (hasTopColl && velocity.Y > 0.0f)
                velocity.Y = -gravity * deltaTime;
            if (!isJumping && hasBotColl)
            {
                    position.Y = collisionPos.Y;
                    velocity.Y = 0.0f;
            }

            if (hasRightColl && velocity.X > 0.0f)
                velocity.X = 0.0f;
            if (hasLeftColl && velocity.X < 0.0f)
                velocity.X = 0.0f;


            position.X += velocity.X * deltaTime;
            position.Y += velocity.Y * deltaTime;

            //Aktualizuj pozycje colliderow
            upperBox.Update();
            bottomBox.Update();
            rightBox.Update();
            leftBox.Update();

            lastPlayerPos = position;

            
            foreach (var block in blocks)
            {
                if (block.distanceToPlayer <= 2f && !block.isDestroyed)
                    CheckCollision(block);
            }

            if (hasBotColl && position.Y < collisionPos.Y)
                position.Y = collisionPos.Y;
        }

        public void ResetBounds()
        {
            isGrounded = false;
            collisionPos = position;
            hasBotColl = false;
            hasTopColl = false;
            hasLeftColl = false;
            hasRightColl = false;
        }

        //Sprawdzenie kolizji z obiektem <collider>
        public void CheckCollision(GameObject collider)
        {

            (bool, Direction, Vector2) botRes = Collider.CheckBoxCollision(bottomBox, collider);
            if (botRes.Item1 == true)
            {
                    hasBotColl = true;
                    isGrounded = true;
                    collisionPos.Y = collider.position.Y + collider.size.Y;

            }
            (bool, Direction, Vector2) rRes = Collider.CheckBoxCollision(rightBox, collider);
            if (rRes.Item1 == true)
            {
                hasRightColl = true;
            }

            (bool, Direction, Vector2) lRes = Collider.CheckBoxCollision(leftBox, collider);
            if (lRes.Item1 == true)
            {
                hasLeftColl = true;
            }

            (bool, Direction, Vector2) upRes = Collider.CheckBoxCollision(upperBox, collider);
            if (upRes.Item1 == true)
            {
                hasTopColl = true;
            }
        }
    }
}
