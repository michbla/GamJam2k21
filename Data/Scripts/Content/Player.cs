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

        private int lowestPosition = 1;
        public PlayerStatistics PlayerStatistics;
        public Inventory eq;

        //Flagi
        public bool canMove = true;
        public bool isGrounded = true;
        //Collidery
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
        //ANIMACJE
        private PlayerAnimator playerAnimator;
        
        //Klatki na sekunde
        private float animFrameRate = 8.0f;

        private float diggingSpeed = 0.0f;

        public bool isReadyToDamage = true;

        private float diggingCooldown = 0.0f;
        private float diggingCooldownBase = 10.0f;

        private bool isFloating = false;

        public Pickaxe equippedPickaxe = ResourceManager.GetPickaxeByID(0);

        //Kostruktor
        public Player(Vector2 pos, Vector2 size, Texture sprite) : base(pos, size, sprite)
        {
            velocity = (0.0f, 0.0f);
            playerCenter = new Vector2(position.X + size.X / 2.0f, position.Y + size.Y / 2.0f);
            //Kolizja boxy
            bottomBox = new BoxCollider(this, new Vector2(0.3f, 0.0f), new Vector2(0.4f, 0.2f));
            upperBox = new BoxCollider(this, new Vector2(0.3f, 1.6f), new Vector2(0.4f, 0.2f));
            leftBox = new BoxCollider(this, new Vector2(0.00f, 0.05f), new Vector2(0.4f, 1.75f));
            rightBox = new BoxCollider(this, new Vector2(0.6f, 0.05f), new Vector2(0.4f, 1.75f));
            collisionPos = pos;
            //Init staty
            PlayerStatistics = new PlayerStatistics(0, 0, 0);
            eq = new Inventory();
            //Init Player Animator
            playerAnimator = new PlayerAnimator(this, ResourceManager.GetShader("sprite"), (16, 1), animFrameRate);
            SetPickaxe(0);
        }
        //Przekazywanie do gracza blokow z poziomu
        public void SetBlocks(ref List<Block> b)
        {
            blocks = b;
        }
        //Rysowanie gracza
        public override void Draw(SpriteRenderer rend, Vector2 viewPos)
        {
            //Rysowanie z animatora
            playerAnimator.DrawBack(rend, viewPos);
            playerAnimator.DrawPlayerAnimator(viewPos);
            playerAnimator.DrawFront(rend, viewPos);
        }
        //Logika gracza
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

            //Reset
            ResetBounds();
            //Kolizje
            DoCollisions();
            //Liczenie centrum gracza
            playerCenter = (position.X + size.X / 2.0f, position.Y + size.Y / 2.0f);
            //sprawdza wysokość
            SetMaxPlayerDepth();
            //Update predkosci kopania
            playerAnimator.armSpeed = equippedPickaxe.speed;
            diggingSpeed = equippedPickaxe.speed;

            //Ruch
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

                if (rememberGrounded > 0.0f && input.IsKeyDown(Keys.Space) && !isFloating)
                {
                    velocity.Y = 1.0f * jumpForce;
                    isJumping = true;
                    playerAnimator.Jump();
                }
            }
            //Grawitacja tylko jesli w powietrzu
            if (!isGrounded && !isFloating)
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
            //Kolizje x2
            DoCollisions();
            //Gorna kolizja
            if (hasColl[0] && velocity.Y > 0.0f)
                velocity.Y = -gravity * deltaTime;
            //Dolkna kolizja
            if (!isJumping && hasColl[2] && !isFloating)
            {
                position.Y = collisionPos.Y;
                velocity.Y = 0.0f;
            }
            //Boczne kolizje
            if (hasColl[1] && velocity.X > 0.0f)
                velocity.X = 0.0f;
            if (hasColl[3] && velocity.X < 0.0f)
                velocity.X = 0.0f;
            //Spowolnienie gracza idacego do tylu
            if (playerAnimator.isWalkingBackwards)
                velocity.X *= 0.75f;
            //Update pozycji
            position.X = Math.Clamp(position.X + velocity.X * deltaTime, 0.0f, 127.0f);
            position.Y += velocity.Y * deltaTime;
            //Aktualizuj pozycje colliderow
            upperBox.Update();
            bottomBox.Update();
            rightBox.Update();
            leftBox.Update();
            //Kolizje x3
            DoCollisions();
            //Dolna kolizja
            if (hasColl[2] && position.Y < collisionPos.Y)
                position.Y = collisionPos.Y;
            //Drabina mechanic
            if (isFloating && canMove)
            {
                velocity.Y = 0.0f;
                if (input.IsKeyDown(Keys.W) && !hasColl[0])
                {
                    position.Y += deltaTime * 10.0f;
                }
                else if (input.IsKeyDown(Keys.S) && !hasColl[2])
                {
                    position.Y -= deltaTime * 10.0f;
                }
            }
            //Obliczanie kata miedzy kurosrem a graczem
            Vector2 diffVec = mousePos - playerCenter;
            double angle = MathHelper.Atan2(diffVec.X, diffVec.Y);
            playerAnimator.UpdateDiffAngle((float)MathHelper.Floor(MathHelper.RadiansToDegrees(angle)));
            //Setter kopania w animatorze
            if (mouseInput.IsButtonDown(MouseButton.Button1))
            {
                playerAnimator.isDigging = true;
            }
            else
                playerAnimator.isDigging = false;
            //Update bycia w powietrzu
            if (!isGrounded)
                playerAnimator.inAir = true;
            else
                playerAnimator.inAir = false;
            //Update animatora
            playerAnimator.UpdatePlayerAnimator(deltaTime);
            //Update cooldownu kopania
            if (diggingCooldown > 0.0f)
                diggingCooldown -= deltaTime * diggingSpeed / 10.0f;
            else
                isReadyToDamage = true;
        }
        //Reset cooldownu kopania
        public void ResetCooldown()
        {
            diggingCooldown = diggingCooldownBase;
            isReadyToDamage = false;
        }
        //Setter do flipa na animatorze
        public void SetFlip(bool flip)
        {
            playerAnimator.isFlipped = flip;
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
    }
}
