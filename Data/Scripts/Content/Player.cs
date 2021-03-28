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

        //ANIMACJE
        //Animator TODO:: oddzielna klasa playerAnimator dziedziczaca po animatorze
        //                dla dokladniejszego zarzadzania animacjami gracza i przekazywania do niej zmiennych
        private Animator playerAnimator;
        //Aktualny stan, do wyrzucenia do klasy wyzej
        private string currentState;
        //Klatki na sekunde, jak wyzej
        private float animFrameRate = 8.0f;

        private GameObject playerArm;
        private Vector2 armOffsetR = (-0.7f, 0.35f);
        private Vector2 armOffsetL = (1.7f, 0.35f);
        private float initialArmRot = 0.0f;
        private float stoppingArmRot = 0.0f;
        private float armSpeed = 500.0f;

        private bool isDigging = false;

        private GameObject pickaxe;
        private GameObject backPickaxe;

        public bool isReadyToDamage = true;
        public bool isDamaging = false;

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
            PlayerStatistics = new PlayerStatistics(0, 0, 0);

            //Robie nowy animator
            playerAnimator = new Animator(this, ResourceManager.GetShader("sprite"), (16, 1), animFrameRate);
            //Dodaje do niego animacje
            playerAnimator.AddAnimation("idle", ResourceManager.GetTexture("charIdle1"), 16);
            playerAnimator.AddAnimation("walk", ResourceManager.GetTexture("charWalk1"), 8);
            playerAnimator.AddAnimation("walkBackwards", ResourceManager.GetTexture("charWalkBack1"), 8);
            //Settuje poczatkowy stan
            currentState = "idle";

            playerArm = new GameObject((0.0f, 0.0f), (2.0f, 2.0f), ResourceManager.GetTexture("charArm1"));

            pickaxe = new GameObject((0.0f, 0.0f), (4.0f, 4.0f), ResourceManager.GetTexture("pickaxe1"));

            backPickaxe = new GameObject((0.0f, 0.0f), (4.0f, 4.0f), ResourceManager.GetTexture("pickaxe1"));
        }
        //Przekazywanie do gracza blokow z poziomu
        public void SetBlocks(ref List<Block> b)
        {
            blocks = b;
        }
        //Rysowanie gracza
        public override void Draw(SpriteRenderer rend, Vector2 viewPos)
        {
            //OPTYMALIZACJA - nie renderuj obiektow poza ekranem
            if (position.Y + size.Y < viewPos.Y - 18f || position.Y > viewPos.Y + 18f || position.X + size.X < viewPos.X - 36f || position.X > viewPos.X + 36f)
                return;

            if (!isDigging)
                backPickaxe.Draw(rend, viewPos);

            //Rysowanie z animatora
            playerAnimator.Draw(currentState, viewPos);

            if (isDigging)
                pickaxe.Draw(rend, viewPos);
            playerArm.Draw(rend, viewPos);
        }

        //Logika gracza
        public override void Update(KeyboardState input, MouseState mouseInput, float deltaTime)
        {
            //Reset
            ResetBounds();
            //Kolizje
            DoCollisions();
            //Liczenie centrum gracza
            playerCenter = (position.X + size.X / 2.0f, position.Y + size.Y / 2.0f);

            //sprawdza wysokość
            SetMaxPlayerDepth();

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
            //Kolizje x2
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

            position.X = Math.Clamp(position.X + velocity.X * deltaTime, 0.0f, 127.0f);
            position.Y += velocity.Y * deltaTime;

            //Aktualizuj pozycje colliderow
            upperBox.Update();
            bottomBox.Update();
            rightBox.Update();
            leftBox.Update();
            //Kolizje x3
            DoCollisions();

            if (hasColl[2] && position.Y < collisionPos.Y)
                position.Y = collisionPos.Y;

            if (mouseInput.IsButtonDown(MouseButton.Button1))
            {
                isDigging = true;
            }
            else
                isDigging = false;

            if (MathHelper.Abs(velocity.X) > 0.0f)
            {
                if((playerAnimator.isFlipped && velocity.X > 0.0f) || (!playerAnimator.isFlipped && velocity.X < 0.0f))
                {
                    currentState = "walkBackwards";
                }
                else
                {
                    currentState = "walk";
                }
            }
            else
                currentState = "idle";
            playerAnimator.Update(currentState, deltaTime);

            UpdateArm(deltaTime);
        }
        public void SetFlip(bool flip)
        {
            playerAnimator.isFlipped = flip;
        }
        //Update ramienia
        private void UpdateArm(float deltaTime)
        {
            if (playerAnimator.isFlipped)
            {
                backPickaxe.position = position + (-1.8f, -0.65f);
                backPickaxe.rotation = -35.0f;

                playerArm.size.X = -2.0f;
                playerArm.position = position + armOffsetL;

                pickaxe.size.X = -4.0f;
                pickaxe.position = playerArm.position + (1.0f, -1.0f);
                if (isDigging)
                {
                    initialArmRot = 135.0f;
                    stoppingArmRot = 15.0f;
                    playerArm.rotation -= rotation + deltaTime * armSpeed;
                    if (playerArm.rotation < (initialArmRot + stoppingArmRot) / 2.0f)
                        isDamaging = true;
                    if (playerArm.rotation < stoppingArmRot)
                    {
                        playerArm.rotation = initialArmRot + rotation;
                        isReadyToDamage = true;
                        isDamaging = false;
                    }
                }
                else
                {
                    playerArm.rotation = rotation;
                }
            }
            else
            {
                backPickaxe.position = position + (-2.2f, -0.9f);
                backPickaxe.rotation = -65.0f;

                playerArm.size.X = 2.0f;
                playerArm.position = position + armOffsetR;
                pickaxe.size.X = 4.0f;
                pickaxe.position = playerArm.position + (-1.0f, -1.0f);
                if (isDigging)
                {
                    initialArmRot = -135.0f;
                    stoppingArmRot = -15.0f;
                    playerArm.rotation += rotation + deltaTime * armSpeed;
                    if (playerArm.rotation > (initialArmRot + stoppingArmRot) / 2.0f)
                        isDamaging = true;
                    if (playerArm.rotation > stoppingArmRot)
                    {
                        playerArm.rotation = initialArmRot + rotation;
                        isReadyToDamage = true;
                        isDamaging = false;
                    }
                }
                else
                {
                    playerArm.rotation = rotation;
                }
            }
            pickaxe.rotation = playerArm.rotation;
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
            //TODO: Pickaxe damage
            return 10.0f;
        }

    }
}
