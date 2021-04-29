using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;

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

        private float diggingSpeed = 400.0f;
        public bool isReadyToDamage = true;
        private float diggingCooldown = 0.0f;
        private readonly float DIG_CD_BASE = 10.0f;

        public Pickaxe equippedPickaxe;

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
        }

        public void Render()
        {
            animator.Render();
        }

        public void Update()
        {
            test();

            handleDigging();

            controller.Blocks = level.currentBlocks;
            controller.Update();

            animator.Update();

            setMaxPlayerDepth();

            if (diggingCooldown > 0.0f)
                diggingCooldown -= Time.DeltaTime * diggingSpeed / 10.0f;
            else
                isReadyToDamage = true;
        }

        private void handleDigging()
        {
            animator.IsSwinging = false;
            hasSelectedBlock = false;
            Vector2i position = (cursorOnGidPos.X, cursorOnGidPos.Y);
            Block block = level.GetBlockAtPosition(position);
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
                    if(block.HasOre())
                        getDrop(block);
                    level.DestoyBlockAtPosition(block,position);
                }
            }
        }

        private void getDrop(Block block)
        {
            Item drop = block.GetDrop();
            if (inventory.HasFreeSpaceForItem(drop))
            {
                inventory.AddToInventory(new Item(drop));
                var ore = block.Ore;
                //player.PlayerStatistics.SetOresDestroyed(ore);
                //Succesfully added item to inventory
            }
            else
            {
                //Inventory full
            }
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

        private void setMaxPlayerDepth()
        {

            if (lowestPosition > transform.Position.Y)
            {
                lowestPosition = (int)transform.Position.Y;
                stats.addLevelReached();
            }
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
