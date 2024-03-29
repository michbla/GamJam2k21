﻿using System;
using System.Collections.Generic;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Threading.Tasks;

namespace GamJam2k21.PlayerElements
{
    public class PlayerController
    {
        private static readonly float GRAVITY = 30.0f;

        private Player player;
        private List<Block> blocks;

        private Vector2 velocity = (0.0f, 0.0f);
        private Vector2 position;

        private float walkSpeed = 8.0f;
        private float backwardsWalkSpeedModifier = 0.65f;
        private float climbingSpeed = 10.0f;
        private float jumpForce = 8.0f;

        private readonly float fallMultiplier = 0.01f;
        private readonly float lowJumpMultiplier = 0.5f;
        private float rememberGrounded = 0.0f;
        private bool isJumping = false;
        private bool isFloating = false;

        private bool canMove = true;
        private bool isGrounded = true;

        private bool jumpFromLadder = false;

        private BoxCollider bottomBox;
        private BoxCollider upperBox;
        private BoxCollider rightBox;
        private BoxCollider leftBox;
        private Vector2 collisionPos;
        private bool[] hasColl = { false, false, false, false };
        //0 - top, 1 - right, 2 - bottom, 3 - left

        public Vector2 Velocity { get => velocity; }
        public List<Block> Blocks { set { blocks = value; } }

        public PlayerController(Player player)
        {
            this.player = player;
            setCollisionBoxes();
            collisionPos = player.Position;
        }

        private void setCollisionBoxes()
        {
            bottomBox = new BoxCollider(player.Transform,
                                        new Vector2(0.4f, 0.2f),
                                        new Vector2(0.3f, 0.0f));
            upperBox = new BoxCollider(player.Transform,
                                       new Vector2(0.4f, 0.2f),
                                       new Vector2(0.3f, 1.6f));
            leftBox = new BoxCollider(player.Transform,
                                      new Vector2(0.4f, 1.75f),
                                      new Vector2(0.00f, 0.05f));
            rightBox = new BoxCollider(player.Transform,
                                       new Vector2(0.4f, 1.75f),
                                       new Vector2(0.6f, 0.05f));
        }

        private float ladderCooldown = 0.45f;
        private float ladderCooldownBase = 0.45f;
        public void Update()
        {
            if (ladderCooldown >= 0.0f)
                ladderCooldown -= Time.DeltaTime;

            position = player.Position;
            isFloating = Input.IsKeyDown(Keys.K);

            resetBounds();
            checkCollisions();

            doMovement();
            reactToGravity();
            reactToCollision();

            if (player.Animator.IsWalkingBackwards)
                velocity.X *= backwardsWalkSpeedModifier;

            position.X = Math.Clamp(position.X + velocity.X * Time.DeltaTime,
                                    0.0f,
                                    127.0f);
            position.Y += velocity.Y * Time.DeltaTime;

            upperBox.Update();
            bottomBox.Update();
            rightBox.Update();
            leftBox.Update();

            checkCollisionsParallel();

            if (hasColl[2] && position.Y < collisionPos.Y)
                position.Y = collisionPos.Y;

            if (isFloating && canMove)
            {
                velocity.Y = 0.0f;
                if (Input.IsKeyDown(Keys.W) && !hasColl[0])
                    position.Y += Time.DeltaTime * climbingSpeed;
                else if (Input.IsKeyDown(Keys.S) && !hasColl[2])
                    position.Y -= Time.DeltaTime * climbingSpeed;

                if ((Input.IsKeyDown(Keys.W) || Input.IsKeyDown(Keys.S))
                    && ladderCooldown <= 0.0f)
                {
                    SoundManager.PlayFloat(player.StandingOn);
                    ladderCooldown = ladderCooldownBase;
                }
            }

            player.Position = position;

            player.Animator.InAir = !isGrounded;
        }
        private void resetBounds()
        {
            isGrounded = false;
            collisionPos = player.Position;
            for (var i = 0; i < 4; i++)
                hasColl[i] = false;
            isFloating = false;
            jumpFromLadder = false;
            player.StandingOn = "None";
        }

        private void checkCollisions()
        {
            foreach(var block in blocks) {
                checkBlockCollision(block);
            }
        }

        private void checkBlockCollision(Block block)
        {
            if (block.DistanceToPlayer <= 10.0f && block.IsCollidable)
                if (block.Name == "Ladder")
                    checkLadderCollision(block);
                else
                    checkCollision(block);
        }

        private void checkCollisionsParallel()
        {
            Parallel.For(0, blocks.Count, (index) =>
            {
                var block = blocks[index];
                checkBlockCollision(block);
            }
            );
        }

        private void doMovement()
        {
            if (!canMove)
                return;
            float vel = walkSpeed;
            if (!isGrounded)
            {
                vel *= 0.8f;
                if (rememberGrounded > 0.0f)
                    rememberGrounded -= Time.DeltaTime;
            }
            else
            {
                rememberGrounded = 0.1f;
                isJumping = false;
            }

            if (Input.IsKeyDown(Keys.A))
                velocity.X = -vel;
            else if (Input.IsKeyDown(Keys.D))
                velocity.X = vel;
            else
                velocity.X = 0.0f;

            if (Input.IsKeyDown(Keys.Space)
                && canJump())
            {
                velocity.Y = 1.0f * jumpForce;
                isJumping = true;
                player.Animator.PlayJumpParticles();
            }
        }

        private bool canJump()
        {
            return (rememberGrounded > 0.0f && !isFloating)
                || jumpFromLadder;
        }

        private void reactToGravity()
        {
            if (!isGrounded && !isFloating)
            {
                velocity.Y -= GRAVITY * Time.DeltaTime;

                if (velocity.Y < 0.0f)
                    velocity.Y += GRAVITY * (fallMultiplier - 1) * Time.DeltaTime;
                else if (velocity.Y > 0.0f && !Input.IsKeyDown(Keys.Space))
                    velocity.Y += GRAVITY * (lowJumpMultiplier - 1) * Time.DeltaTime;

                velocity.Y = Math.Clamp(velocity.Y, -30.0f, 30.0f);
            }
        }

        private void reactToCollision()
        {
            if (hasColl[0] && velocity.Y > 0.0f)
                velocity.Y = -GRAVITY * Time.DeltaTime;

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

        private void checkCollision(Block block)
        {
            BoxCollider collider = block.Collider;
            (bool, Direction, Vector2) upRes = upperBox.CheckCollision(collider);
            if (upRes.Item1 == true)
                hasColl[0] = true;
            (bool, Direction, Vector2) rRes = rightBox.CheckCollision(collider);
            if (rRes.Item1 == true)
                hasColl[1] = true;
            (bool, Direction, Vector2) botRes = bottomBox.CheckCollision(collider);
            if (botRes.Item1 == true)
            {
                isGrounded = true;
                collisionPos.Y = collider.Position.Y + collider.Size.Y;
                hasColl[2] = true;
                if (player.StandingOn == "None")
                    player.StandingOn = block.Name;
            }
            (bool, Direction, Vector2) lRes = leftBox.CheckCollision(collider);
            if (lRes.Item1 == true)
                hasColl[3] = true;
        }

        private void checkLadderCollision(Block block)
        {
            BoxCollider collider = block.Collider;
            (bool, Direction, Vector2) upRes = upperBox.CheckCollision(collider);
            (bool, Direction, Vector2) botRes = bottomBox.CheckCollision(collider);
            (bool, Direction, Vector2) rRes = rightBox.CheckCollision(collider);
            (bool, Direction, Vector2) lRes = leftBox.CheckCollision(collider);

            if (upRes.Item1 == true || botRes.Item1 == true
              || lRes.Item1 == true || rRes.Item1 == true)
            {
                isFloating = true;
                if (player.StandingOn == "None")
                    player.StandingOn = block.Name;
            }
            if (botRes.Item1 == true)
            {
                jumpFromLadder = true;
                rememberGrounded = 0.1f;
            }
        }
    }
}
