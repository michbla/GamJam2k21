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

        //Flagi
        public bool canMove = true;
        public bool isGrounded = true;

        //Collidery (kapsula)
        private BoxCollider bottomBox;
        private BoxCollider upperBox;
        private BoxCollider rightBox;
        private BoxCollider leftBox;

        //Porzadana pozycja gracza wzgledem obiektu kolizji
        private Vector2 collisionPos;

        //Flaga posiadania kolizji
        private bool[] hasColl = { false, false, false, false };
        //0 - top, 1 - right, 2 - bottom, 3 - left

        //Flaga do sprawdzenia czy w danej klatce rozpoczety zostal skok
        private bool isJumping = false;

        //Lista blokow z poziomu przekazana jako referencja
        //Do liczenia kolizji
        private List<Block> blocks;

        //Grawitacja
        private float gravity = 30.0f;

        //Czy gracz ma byc rysowany odwrotnie
        public bool isFlipped = false;

        //Kostruktor
        public Player(Vector2 pos, Vector2 size, Texture sprite) : base(pos, size, sprite)
        {
            velocity = (0.0f, 0.0f);
            playerCenter = new Vector2(position.X + size.X / 2.0f, position.Y + size.Y / 2.0f);

            bottomBox = new BoxCollider(this, new Vector2(0.3f, 0.0f), new Vector2(0.4f, 0.2f));
            upperBox = new BoxCollider(this, new Vector2(0.3f, 1.6f), new Vector2(0.4f, 0.2f));
            leftBox = new BoxCollider(this, new Vector2(0.00f, 0.05f), new Vector2(0.4f, 1.75f));
            rightBox = new BoxCollider(this, new Vector2(0.6f, 0.05f), new Vector2(0.4f, 1.75f));

            collisionPos = pos;
            
        }
        public void SetBlocks(ref List<Block> b)
        {
            blocks = b;
        }
        //Logika gracza
        public override void Update(KeyboardState input, float deltaTime)
        {
            
            ResetBounds();
            DoCollisions();

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

            DoCollisions();

            if (hasColl[0] && velocity.Y > 0.0f)
                velocity.Y = -gravity * deltaTime;
            if (!isJumping && hasColl[2])
            {
                position.Y = collisionPos.Y;
                velocity.Y = 0.0f;
            }

            if (hasColl[1] && velocity.X > 0.0f)
                velocity.X = 0.0f;
            if (hasColl[3] && velocity.X < 0.0f)
                velocity.X = 0.0f;


            position.X += velocity.X * deltaTime;
            position.Y += velocity.Y * deltaTime;

            //Aktualizuj pozycje colliderow
            upperBox.Update();
            bottomBox.Update();
            rightBox.Update();
            leftBox.Update();

            DoCollisions();

            if (hasColl[2] && position.Y < collisionPos.Y)
                position.Y = collisionPos.Y;

            if (input.IsKeyDown(Keys.F))
            {
                isFlipped = true;
            }
            else
                isFlipped = false;
        }
        //Sprawdz wszystkie kolizje
        private void DoCollisions()
        {
            foreach (var block in blocks)
            {
                if (block.distanceToPlayer <= 2f && !block.isDestroyed)
                    CheckCollision(block);
            }
        }
        //Reset przed sprawdzeniem kolizji
        public void ResetBounds()
        {
            isGrounded = false;
            collisionPos = position;
            for (var i = 0; i < 4; i++)
                hasColl[i] = false;
        }

        //Sprawdzenie kolizji z obiektem <collider>
        public void CheckCollision(GameObject collider)
        {
            //Gorna kolizja
            (bool, Direction, Vector2) upRes = Collider.CheckBoxCollision(upperBox, collider);
            if (upRes.Item1 == true)
            {
                hasColl[0] = true;
            }
            //Prawa kolizja
            (bool, Direction, Vector2) rRes = Collider.CheckBoxCollision(rightBox, collider);
            if (rRes.Item1 == true)
            {
                hasColl[1] = true;
            }
            //Dolna kolizja
            (bool, Direction, Vector2) botRes = Collider.CheckBoxCollision(bottomBox, collider);
            if (botRes.Item1 == true)
            {
                isGrounded = true;
                collisionPos.Y = collider.position.Y + collider.size.Y;
                hasColl[2] = true;

            }
            //Lewa kolizja
            (bool, Direction, Vector2) lRes = Collider.CheckBoxCollision(leftBox, collider);
            if (lRes.Item1 == true)
            {
                hasColl[3] = true;
            }
        }

        public override void Draw(SpriteRenderer rend, Vector2 viewPos)
        {
            //OPTYMALIZACJA - nie renderuj obiektow poza ekranem
            if (position.Y + size.Y < viewPos.Y - 18f || position.Y > viewPos.Y + 18f || position.X + size.X < viewPos.X - 36f || position.X > viewPos.X + 36f)
                return;
            //Sprite flip
            if(!isFlipped)
                rend.DrawSprite(sprite, viewPos, position, size, rotation, color);
            else
            {
                rend.DrawSprite(sprite, viewPos, (position.X + size.X,position.Y), (size.X * -1,size.Y), rotation, color);
            }
        }
    }
}
