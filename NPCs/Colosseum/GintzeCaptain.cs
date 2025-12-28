using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Core.Shaders;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using Stellamod.Items.Armors.Pieces.RareMetals;
using Stellamod.NPCs.Colosseum.Common;
using Stellamod.NPCs.Colosseum.Projectiles;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Colosseum
{
    public class GintzeCaptain : BaseColosseumNPC,
        IDrawOutlines
    {
        private bool _warn;
        private bool _contactDamage;
        private Color _outlineColor;
        private Color TargetOutlineColor;

        private int _frame;
        private enum AIState
        {
            Idle,
            Pace,
            Summon
        }
        private ref float Timer => ref NPC.ai[0];
        private AIState State
        {
            get => (AIState)NPC.ai[1];
            set => NPC.ai[1] = (float)value;
        }
        private Player Target => Main.player[NPC.target];
        private float DirectionToTarget
        {
            get
            {
                if (Target.Center.X < NPC.Center.X)
                {
                    return -1;
                }
                return 1;
            }
        }

        private float Radius => 356;
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 18;
        }

        public override void SetDefaults()
        {
            NPC.width = 66; // The width of the npc's hitbox (in pixels)
            NPC.height = 70; // The height of the npc's hitbox (in pixels)
            NPC.aiStyle = -1; // This npc has a completely unique AI, so we set this to -1. The default aiStyle 0 will face the player, which might conflict with custom AI code.
            NPC.damage = 1; // The amount of damage that this npc deals
            NPC.defense = 2; // The amount of defense that this npc has
            NPC.lifeMax = 250; // The amount of health that this npc has
            NPC.HitSound = new SoundStyle("Stellamod/Assets/Sounds/Gintze_Hit") with { PitchVariance = 0.1f };
            NPC.DeathSound = new SoundStyle("Stellamod/Assets/Sounds/Gintze_Death") with { PitchVariance = 0.1f };
            NPC.value = 10f; // How many copper coins the NPC will drop when killed.
            NPC.knockBackResist = 0f;
            NPC.noGravity = false;
            NPC.noTileCollide = false;
        }

        public override void Colosseum_AI()
        {
            base.Colosseum_AI();


            if (_contactDamage)
                TargetOutlineColor = Color.Red;
            else if (_warn)
                TargetOutlineColor = Color.Yellow;
            else
                TargetOutlineColor = Color.Transparent;
            _outlineColor = Color.Lerp(_outlineColor, TargetOutlineColor, 0.1f);
            _warn = false;
            _contactDamage = false;


            if (!NPC.HasValidTarget)
                NPC.TargetClosest();
            NPC.spriteDirection = -NPC.direction;
            switch (State)
            {
                case AIState.Idle:
                    AI_Idle();
                    break;
                case AIState.Pace:
                    AI_Pace();
                    break;
                case AIState.Summon:
                    AI_Summon();
                    break;
            }
        }


        public override void FindFrame(int frameHeight)
        {
            base.FindFrame(frameHeight);
            NPC.frameCounter += 0.33f;
            if (NPC.frameCounter >= 1f)
            {
                _frame++;
                NPC.frameCounter = 0f;
            }

            switch (State)
            {
                default:
                case AIState.Idle:
                case AIState.Summon:
                    if (_frame < 11)
                    {
                        _frame = 11;
                    }
                    if (_frame >= 18)
                    {
                        _frame = 11;
                    }
                    break;
                case AIState.Pace:
                    if (_frame < 0)
                    {
                        _frame = 0;
                    }

                    if (_frame >= 11)
                    {
                        _frame = 0;
                    }
                    break;
            }
            NPC.frame.Y = frameHeight * _frame;
        }

        private void AI_Idle()
        {
            Timer++;
            NPC.velocity.X *= 0.92f;
            NPC.direction = (Target.Center.X > NPC.Center.X) ? 1 : -1;
            if (Timer > 120 && NPC.HasValidTarget)
            {
                SwitchState(AIState.Pace);
            }
        }

        private void AI_Pace()
        {
            Timer++;
            float moveSpeed = 0.5f;
            Vector2 targetVelocity = new Vector2(DirectionToTarget * moveSpeed, 0);
            NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, targetVelocity.X, 0.3f);
            NPC.direction = (Target.Center.X > NPC.Center.X) ? 1 : -1;
            float distanceToTarget = Vector2.Distance(NPC.Center, Target.Center);
            if (distanceToTarget < Radius && Timer >= 60)
            {
                SwitchState(AIState.Summon);
            }
            _warn = true;
        }

        private void AI_Summon()
        {
            Timer++;
            NPC.velocity.X *= 0.92f;
            if (Timer > 30 && Timer % 30 == 0 && Timer < 150)
            {
                if (MultiplayerHelper.IsHost)
                {
                    int damage = 12;
                    int knockback = 2;
                    Vector2 spawnPoint = NPC.Center;
                    spawnPoint.Y -= 48;
                    spawnPoint.X += Main.rand.NextFloat(-24, 24);
                    Vector2 velocity = (Target.Center - spawnPoint).SafeNormalize(Vector2.Zero);
                    velocity *= 7;
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), spawnPoint, velocity,
                        ModContent.ProjectileType<CaptainSpear>(), damage, knockback, Main.myPlayer);
                }
            }

            if (Timer > 240)
            {
                SwitchState(AIState.Idle);
            }
            _contactDamage = true;
        }

   
        private void SwitchState(AIState state)
        {
            if (MultiplayerHelper.IsHost)
            {
                Timer = 0;
                State = state;
                NPC.netUpdate = true;
            }
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            GintzeHitEffect(hit);
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<GintzeMask>(), 80, 1, 1));
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return false;
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // We can use AddRange instead of calling Add multiple times in order to add multiple items at once
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
				// Sets the description of this NPC that is listed in the bestiary.
				new FlavorTextBestiaryInfoElement(LangText.Bestiary(this, "A Captain of Gothivia's ranks, be careful"))
            });
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D bannerTexture = ModContent.Request<Texture2D>(Texture + "_MiniBanner").Value;
            Vector2 drawOrigin = bannerTexture.Size() / 2f;
            spriteBatch.Restart(blendState: BlendState.Additive);
            for (int i = 0; i < 10; i++)
            {
                float f = i;
                float progress = f / 10f;
                float rot = progress * MathHelper.TwoPi;
                rot += Main.GlobalTimeWrappedHourly * 0.1f;
                float rotation = 0;
                Vector2 offset = rot.ToRotationVector2() * Radius;
                Vector2 drawPos = NPC.Center - screenPos;
                float drawScale = 1f;

                drawPos += offset;
                spriteBatch.Draw(bannerTexture, drawPos, null, drawColor, rotation, drawOrigin, drawScale, SpriteEffects.None, 0);
            }
            spriteBatch.RestartDefaults();
            DrawSprite(spriteBatch, screenPos, drawColor);
            return false;
        }


        private void DrawSprite(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            Texture2D texture = TextureAssets.Npc[Type].Value;
            Vector2 drawOrigin = NPC.frame.Size() / 2f;
            Vector2 drawCenter = NPC.Center - screenPos;
            Color drawColor = Color.White.MultiplyRGB(lightColor);
            SpriteEffects spriteEffects = NPC.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            spriteBatch.Draw(texture, drawCenter, NPC.frame, drawColor, NPC.rotation, drawOrigin, NPC.scale, spriteEffects, 0);
        }


        public void DrawOutlines(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            DrawExtensions.DrawOutline(DrawSprite, spriteBatch, screenPos, _outlineColor);
        }
    }
}