﻿using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using GamJam2k21.PlayerElements;

namespace GamJam2k21
{
    public class Player
    {
        private Transform transform;
        private Vector2 size;

        private GameLevel level;
        private Vector2i cursorOnGidPos;
        private bool hasSelectedBlock;

        private PlayerController controller;

        private PlayerAnimator animator;
        private readonly float animFrameRate = 12.0f;

        private int lowestPosition = 1;
        public PlayerStatistics stats;
        public Inventory inventory;

        public Skills Skills = new Skills();

        public bool isReadyToDamage = true;
        private float diggingSpeedBase = 400.0f;
        private float diggingSpeed = 400.0f;
        private float diggingCooldown = 0.0f;
        private readonly float DIG_CD_BASE = 10.0f;

        private StaminaHandler stamina;

        public Pickaxe equippedPickaxe;

        private bool canBeControlled = true;

        public bool IsHoldingBomb = true;

        public Transform Transform { get => transform; }
        public Vector2 Position
        {
            get => transform.Position;
            set { transform.Position = value; }
        }
        public Vector2 Center { get => transform.Position + size * 0.5f; }
        public Vector2 HeadPosition { get => (Center.X, Center.Y + 0.5f); }
        public Vector2 Size { get => size; }
        public Vector2 Velocity { get => controller.Velocity; }
        public PlayerAnimator Animator { get => animator; }
        public bool IsDigging { get => animator.IsSwinging; }
        public float Damage { get => equippedPickaxe.Damage; }
        public int Hardness { get => equippedPickaxe.Hardness; }
        public GameLevel Level { set { level = value; } }
        public Vector2i CursorOnGridPosition { set { cursorOnGidPos = value; } }
        public bool HasSelectedBlock { get => hasSelectedBlock; }
        public bool CanBeControlled { set { canBeControlled = value; } }
        public int Gold { get => inventory.Gold; }
        public int LadderCount { get => inventory.Ladders; }
        public int BombCount { get => inventory.Bombs; }
        public float Stamina { get => stamina.Stamina; }
        public float StaminaMax { get => stamina.StaminaMax; }

        public Player(Transform transform, Vector2 size)
        {
            this.transform = transform;
            this.size = size;

            animator = new PlayerAnimator(this, animFrameRate);
            controller = new PlayerController(this);
            stats = new PlayerStatistics(0, 0, 0);
            inventory = new Inventory();
            hasSelectedBlock = false;
            EquipPickaxe(0);

            stamina = new StaminaHandler(this);
        }

        public void Render()
        {
            animator.Render();
        }

        public void Update()
        {
            if (!canBeControlled)
                return;
            test();

            stamina.Update();

            handleDigging();
            handlePlacing();
            handleSwitchingAccessory();

            controller.Blocks = level.currentBlocks;
            controller.Update();

            animator.Update();

            setMaxPlayerDepth();

            if (diggingCooldown > 0.0f)
                diggingCooldown -= Time.DeltaTime * diggingSpeed / 10.0f;
            else
                isReadyToDamage = true;

            updateSkills();
        }

        private void handleDigging()
        {
            animator.IsSwinging = false;
            hasSelectedBlock = false;
            Vector2i position = (cursorOnGidPos.X, cursorOnGidPos.Y);
            Block block = level.GetBlockInPlayersRange(position);
            if (block == null || !block.CanBeDamaged(Hardness))
                return;
            hasSelectedBlock = true;
            if (Input.IsMouseButtonDown(MouseButton.Button1))
            {
                animator.IsSwinging = true;
                if (!isReadyToDamage)
                    return;
                block.Damage(Damage);
                level.playDamageParticles(block);
                ResetDiggingCooldown();
                if (block.IsDestroyed())
                {
                    if (block.HasOre())
                        earnGoldFromBlock(block);

                    level.DestroyBlockAtPosition(block, position);
                }
            }
        }

        private void earnGoldFromBlock(Block block)
        {
            inventory.Gold += block.GetDrop().Value;
        }

        public void AddGold(int amount)
        {
            inventory.Gold += amount;
        }

        private bool canPlaceBlock = true;
        private void handlePlacing()
        {
            Vector2i position = (cursorOnGidPos.X, cursorOnGidPos.Y);
            if (Input.IsMouseButtonDown(MouseButton.Button2) && canPlaceBlock)
            {
                canPlaceBlock = false;
                if (IsHoldingBomb)
                    tryPlaceBomb(position);
                else
                    tryPlaceLadder(position);
            }
            if (!Input.IsMouseButtonDown(MouseButton.Button2))
                canPlaceBlock = true;
        }

        private void tryPlaceBomb(Vector2i position)
        {
            if (inventory.Bombs > 0 && level.CanPlaceBlockAtPosition(position))
            {
                inventory.Bombs--;
                level.PlaceBlockAtPosition(ResourceManager.GetBlockByID(101), position);
                level.ActivateBombAtPosition(position);
            }
        }

        private void tryPlaceLadder(Vector2i position)
        {
            if (inventory.Ladders > 0 && level.CanPlaceBlockAtPosition(position))
            {
                inventory.Ladders--;
                level.PlaceBlockAtPosition(ResourceManager.GetBlockByID(100), position);
            }
        }

        private void handleSwitchingAccessory()
        {
            if (Input.IsKeyPressed(Keys.Q))
                switchAccessory();
        }

        private void switchAccessory()
        {
            IsHoldingBomb = !IsHoldingBomb;
        }

        public void ResetDiggingCooldown()
        {
            diggingCooldown = DIG_CD_BASE;
            isReadyToDamage = false;
        }
        public void EquipPickaxe(int pickaxeId)
        {
            equippedPickaxe = ResourceManager.GetPickaxeByID(pickaxeId);
            animator.PickaxeSprite = equippedPickaxe.Sprite;
        }

        public void Pay(int amount)
        {
            inventory.Gold -= amount;
        }

        public void AddBomb()
        {
            inventory.Bombs++;
        }

        public void AddLadder()
        {
            inventory.Ladders++;
        }

        private void setMaxPlayerDepth()
        {

            if (lowestPosition > transform.Position.Y)
            {
                lowestPosition = (int)transform.Position.Y;
                stats.addLevelReached();
            }
        }

        private void updateSkills()
        {
            diggingSpeed = diggingSpeedBase + 100.0f + Skills.SpeedPoints * 300.0f;
        }

        public void resetDiggingSpeed()
        {
            diggingSpeed = diggingSpeedBase;
        }

        //TODO: REMOVE TESTS
        private void test()
        {
            if (Input.IsKeyDown(Keys.D0))
                EquipPickaxe(0);
            if (Input.IsKeyDown(Keys.D1))
                EquipPickaxe(1);
            if (Input.IsKeyDown(Keys.D2))
                EquipPickaxe(2);
            if (Input.IsKeyDown(Keys.D3))
                EquipPickaxe(3);
            if (Input.IsKeyDown(Keys.D4))
                EquipPickaxe(4);
            if (Input.IsKeyDown(Keys.D5))
                EquipPickaxe(5);
        }

    }
}
