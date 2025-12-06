using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace Stellamod.Content.Areas.Collosseum.NPCsCL
{
    public class DesertPerson : ModNPC
    {
        private int _frame;
        // States
        public enum ActionState
        {
            Idle,
            Walk,
        }

        private ref float Timer => ref NPC.ai[0];
        private ActionState State
        {
            get => (ActionState)NPC.ai[1];
            set => NPC.ai[1] = (float)value;
        }
        private int Variant
        {
            get => (int)NPC.ai[2];
            set => NPC.ai[2] = (int)value;
        }
        private ref float WalkingDirection => ref NPC.ai[3];
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 17;
        }

        public override void SetDefaults()
        {
            NPC.width = 36; // The width of the npc's hitbox (in pixels)
            NPC.height = 52; // The height of the npc's hitbox (in pixels)
            NPC.aiStyle = -1; // This npc has a completely unique AI, so we set this to -1. The default aiStyle 0 will face the player, which might conflict with custom AI code.
            NPC.damage = 1; // The amount of damage that this npc deals
            NPC.defense = 2; // The amount of defense that this npc has
            NPC.lifeMax = 2000; // The amount of health that this npc has
            NPC.HitSound = new SoundStyle("Stellamod/Assets/Sounds/Gintze_Hit") with { PitchVariance = 0.1f };
            NPC.DeathSound = new SoundStyle("Stellamod/Assets/Sounds/Gintze_Death") with { PitchVariance = 0.1f };
            NPC.value = 10f; // How many copper coins the NPC will drop when killed.
            NPC.knockBackResist = 0f;
            NPC.noGravity = false;
            NPC.noTileCollide = false;
            NPC.dontTakeDamage = true;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (!Main.dayTime)
                return 0f;
            float spawnChance = SpawnCondition.Overworld.Chance * (spawnInfo.Player.ZoneDesert ? 3 : 0f);
            return spawnChance;
        }

        public override void FindFrame(int frameHeight)
        {
            base.FindFrame(frameHeight);
            NPC.frameCounter += 0.15f;
            if (NPC.frameCounter >= 1f)
            {
                _frame++;
                NPC.frameCounter = 0f;
            }

            switch (State)
            {
                case ActionState.Idle:
                    if (_frame < 10)
                    {
                        _frame = 10;
                    }

                    if (_frame >= 17)
                    {
                        _frame = 10;
                    }

                    break;
                case ActionState.Walk:
                    if (_frame >= 10)
                    {
                        _frame = 0;
                    }
                    break;
            }
            NPC.frame.Y = frameHeight * _frame;
        }
        public override void AI()
        {
            Timer++;
            if (MultiplayerHelper.IsHost)
            {
                if (Timer == 1)
                {
                    Variant = Main.rand.Next(0, 5);
                    NPC.netUpdate = true;
                }
            }

            float walkingSpeed = 0.5f;
            switch (State)
            {
                case ActionState.Idle:
                    NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, 0f, 0.1f);
                    if (WalkingDirection != 0)
                    {
                        State = ActionState.Walk;
                    }
                    break;
                case ActionState.Walk:
                    NPC.direction = WalkingDirection > 0 ? 1 : -1; 
                    float walkingX = NPC.direction * walkingSpeed;
                    NPC.velocity.X = walkingX;
                    if (WalkingDirection == 0)
                    {
                        State = ActionState.Idle;
                        NPC.netUpdate = true;
                    }
                    break;
            }
            if (MultiplayerHelper.IsHost)
            {
                if (Main.rand.NextBool(100))
                {
                    WalkingDirection = Main.rand.Next(-1, 2);
                }
            }
            NPC.spriteDirection = NPC.direction;
            if (NPC.collideX)
            {
                Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY);
            }
        }

        private Texture2D GetVariantTexture()
        {
            if (Variant == 0)
                return ModContent.Request<Texture2D>(Texture).Value;
            Texture2D subVariantTexture = ModContent.Request<Texture2D>(Texture + $"_{Variant}").Value;
            return subVariantTexture;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Vector2 drawCenter = NPC.Center - screenPos;
            Rectangle frame = NPC.frame;
            Vector2 drawOrigin = frame.Size() / 2f;
            Texture2D texture = GetVariantTexture();
            SpriteEffects spriteEffects = NPC.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            spriteBatch.Draw(texture, drawCenter, frame, drawColor, NPC.rotation, drawOrigin, NPC.scale, spriteEffects, 0);
            return false;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // We can use AddRange instead of calling Add multiple times in order to add multiple items at once
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
				// Sets the description of this NPC that is listed in the bestiary.
				new FlavorTextBestiaryInfoElement(LangText.Bestiary(this, "A Captain of Gofria's ranks, be careful"))
            });
        }
    }
}
