using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Assets.Biomes;
using Stellamod.Helpers;
using Stellamod.Items.Materials.Tech;
using Stellamod.Items.Ores;
using Stellamod.NPCs.Catacombs.Trap.Cogwork;
using Stellamod.WorldG;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
using static Terraria.ModLoader.ModContent;

namespace Stellamod.NPCs.Underground
{
    public class Rustwork : ModNPC
    {
        // States
        public enum ActionState
        {

            Jump,
            Fall,
            Wait
        }
        // Current state

        public ActionState State = ActionState.Jump;
        // Current frame
        public int frameCounter;
        // Current frame's progress
        public int frameTick;
        // Current state's timer
        public float timer;

        // AI counter
        public int counter;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 1;
            NPCID.Sets.TrailCacheLength[NPC.type] = 3;
            NPCID.Sets.TrailingMode[NPC.type] = 0;

        }

        public override void SetDefaults()
        {
            NPC.width = 70; // The width of the npc's hitbox (in pixels)
            NPC.height = 70; // The height of the npc's hitbox (in pixels)
            NPC.aiStyle = -1;
            NPC.damage = 40; // The amount of damage that this npc deals
            NPC.defense = 12; // The amount of defense that this npc has
            NPC.lifeMax = 300; // The amount of health that this npc has
            NPC.HitSound = new SoundStyle("Stellamod/Assets/Sounds/Gintze_Hit") with { PitchVariance = 0.1f };
            NPC.DeathSound = new SoundStyle("Stellamod/Assets/Sounds/Gintze_Death") with { PitchVariance = 0.1f };
            NPC.value = 50f; // How many copper coins the NPC will drop when killed.
            NPC.knockBackResist = 0.4f;

        }

        public int dirX = 1;
        public int dirY = 1;
        private int attackCounter;
        public override void AI()
        {
            NPC.TargetClosest();
            NPC.rotation += 0.3f;

            if (NPC.collideX)
            {
                dirX = -dirX;
            }
            if (NPC.collideY)
            {
                dirY = -dirY;
            }
            float moveSpeed = 2.5f;
            NPC.velocity = new Vector2(dirX * moveSpeed, dirY * moveSpeed);


            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (attackCounter > 0)
                {
                    attackCounter--; // tick down the attack counter.
                }

                Player target = Main.player[NPC.target];


                // If the attack counter is 0, this NPC is less than 12.5 tiles away from its target, and has a path to the target unobstructed by blocks, summon a projectile.
                if (attackCounter <= 0 && Collision.CanHit(NPC.Center, 1, 1, target.Center, 1, 1))
                {
                    Vector2 direction = (target.Center - NPC.Center).SafeNormalize(Vector2.UnitX);
                    direction = direction.RotatedByRandom(MathHelper.ToRadians(10));

                    int projectile = Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, direction * 3,
                        ModContent.ProjectileType<SpikeBall>(), 40, 0, Main.myPlayer);
                    Main.projectile[projectile].timeLeft = 300;
                    Projectile ichor = Main.projectile[projectile];
                    ichor.hostile = true;
                    ichor.friendly = false;


                    attackCounter = 200;
                    NPC.netUpdate = true;
                }
            }
        }


        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {


            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<BrokenTech>(), 6, 1, 3));
            npcLoot.Add(ItemDropRule.Common(ItemID.IronOre, 1, 1, 5));

        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return SpawnRates.GetMechanicalEnemySpawnChance(spawnInfo);
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // We can use AddRange instead of calling Add multiple times in order to add multiple items at once
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
				// Sets the description of this NPC that is listed in the bestiary.
				new FlavorTextBestiaryInfoElement(LangText.Bestiary(this, "An old rusted cog with some magical abilities"))
            });
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.SilverCoin, 2.5f * hit.HitDirection, -2.5f, 180, default, .6f);
            }

            if (NPC.life <= 0)
            {
                for (int i = 0; i < 5; i++)
                {
                    int num = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Copper, 0f, -2f, 180, default, .6f);
                    Main.dust[num].noGravity = true;
                    Dust expr_62_cp_0 = Main.dust[num];
                    expr_62_cp_0.position.X = expr_62_cp_0.position.X + (Main.rand.Next(-50, 51) / 20 - 1.5f);
                    Dust expr_92_cp_0 = Main.dust[num];
                    expr_92_cp_0.position.Y = expr_92_cp_0.position.Y + (Main.rand.Next(-50, 51) / 20 - 1.5f);
                    if (Main.dust[num].position != NPC.Center)
                    {
                        Main.dust[num].velocity = NPC.DirectionTo(Main.dust[num].position) * 6f;
                    }
                }
            }

            SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Morrowpes"));
        }




        Vector2 Drawoffset => new Vector2(0, NPC.gfxOffY) + Vector2.UnitX * NPC.spriteDirection * 0;
        public virtual string GlowTexturePath => Texture + "_Glow";
        private Asset<Texture2D> _glowTexture;
        public Texture2D GlowTexture => (_glowTexture ??= (RequestIfExists<Texture2D>(GlowTexturePath, out var asset) ? asset : null))?.Value;
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            float num108 = 4;
            float num107 = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 1.4f / 1.4f * 6.28318548f)) / 2f + 0.5f;
            float num106 = 0f;
            Color color1 = Color.LightBlue * num107 * .8f;
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            spriteBatch.Draw(
                GlowTexture,
                NPC.Center - Main.screenPosition + Drawoffset,
                NPC.frame,
                color1,
                NPC.rotation,
                NPC.frame.Size() / 2,
                NPC.scale,
                effects,
                0
            );
            SpriteEffects spriteEffects3 = (NPC.spriteDirection == 1) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Vector2 vector33 = new Vector2(NPC.Center.X, NPC.Center.Y) - Main.screenPosition + Drawoffset - NPC.velocity;
            Color color29 = new Color(127 - NPC.alpha, 127 - NPC.alpha, 127 - NPC.alpha, 0).MultiplyRGBA(Color.White);
            for (int num103 = 0; num103 < 4; num103++)
            {
                Color color28 = color29;
                color28 = NPC.GetAlpha(color28);
                color28 *= 1f - num107;
                Vector2 vector29 = NPC.Center + (num103 / (float)num108 * 6.28318548f + NPC.rotation + num106).ToRotationVector2() * (4f * num107 + 2f) - Main.screenPosition + Drawoffset - NPC.velocity * num103;
                Main.spriteBatch.Draw(GlowTexture, vector29, NPC.frame, color28, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, spriteEffects3, 0f);
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            Lighting.AddLight(NPC.Center, Color.Silver.ToVector3() * 0.25f * Main.essScale);
            SpriteEffects Effects = NPC.spriteDirection != -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            var drawOrigin = new Vector2(TextureAssets.Npc[NPC.type].Width() * 0.5f, NPC.height * 0.5f);
            for (int k = 0; k < NPC.oldPos.Length; k++)
            {
                Vector2 drawPos = NPC.oldPos[k] - Main.screenPosition + NPC.Size / 2 + new Vector2(0f, NPC.gfxOffY);
                Color color = NPC.GetAlpha(Color.Lerp(new Color(191, 165, 160), new Color(191, 59, 51), 1f / NPC.oldPos.Length * k) * (1f - 1f / NPC.oldPos.Length * k));
                spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, drawPos, new Microsoft.Xna.Framework.Rectangle?(NPC.frame), color, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, Effects, 0f);
            }

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            return true;
        }
    }
}