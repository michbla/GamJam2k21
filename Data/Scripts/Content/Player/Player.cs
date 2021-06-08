using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
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

        public bool isReadyToDamage = true;
        private float diggingSpeedBase = 400.0f;
        private float diggingSpeed = 400.0f;
        private float diggingCooldown = 0.0f;
        private readonly float DIG_CD_BASE = 10.0f;

        private StaminaHandler stamina;

        public Pickaxe equippedPickaxe;

        private bool canBeControlled = true;

        public bool IsHoldingBomb = true;

        public Skills Skills = new Skills();

        public string StandingOn = "Grass";

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
        public GameLevel GameLevel { set { level = value; } }
        public Vector2i CursorOnGridPosition { set { cursorOnGidPos = value; } }
        public bool HasSelectedBlock { get => hasSelectedBlock; }
        public bool CanBeControlled { set { canBeControlled = value; } }
        public int Gold { get => inventory.Gold; }
        public int LadderCount { get => inventory.Ladders; }
        public int BombCount { get => inventory.Bombs; }
        public float Stamina { get => stamina.Stamina; }
        public float StaminaMax { get => stamina.StaminaMax; }
        public int Level { get => stats.ExpLevel; }
        public float Exp { get => stats.Exp; }
        public float ExpPercent { get => stats.Exp / stats.ExpToNextLevel; }

        public Player(Transform transform, Vector2 size)
        {
            this.transform = transform;
            this.size = size;

            animator = new PlayerAnimator(this, animFrameRate);
            controller = new PlayerController(this);
            stats = new PlayerStatistics(this);
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
                SoundManager.PlayHit(block.Name);
                if (block.IsDestroyed())
                {
                    if (block.HasOre())
                    {
                        earnGoldFromBlock(block);
                        SoundManager.PlayCoins();
                    }
                    stats.AddExp(block.Exp);

                    level.DestroyBlockAtPosition(block, position);
                    SoundManager.PlayDest(block.Name);
                }
            }
        }

        private void earnGoldFromBlock(Block block)
        {
            int drop = block.GetDrop().Value;
            if (checkLuck())
                drop *= 2;
            inventory.Gold += drop;
        }

        private bool checkLuck()
        {
            Random rand = new Random();
            return rand.Next(16) < Skills.LuckPoints;
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

        public void AddAttributePoint()
        {
            Skills.SkillPoints++;
        }

        private void setMaxPlayerDepth()
        {

            if (lowestPosition > transform.Position.Y)
            {
                lowestPosition = (int)Math.Abs(transform.Position.Y) + 1;
                stats.SetLevelReached(lowestPosition);
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

    }
}
