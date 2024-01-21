using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Items.Materials;
using System;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace Stellamod.NPCs.Ice.WinterSouls
{

    public class WinterSoul1 : ModNPC
    {
        public bool Shooting;
        public float Timer;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Storm Spirit");
            Main.npcFrameCount[NPC.type] = 5;
            NPCID.Sets.TrailCacheLength[NPC.type] = 10;
            NPCID.Sets.TrailingMode[NPC.type] = 0;
        }

        public override void SetDefaults()
        {
            NPC.aiStyle = 0;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.defense = 3;
            NPC.lifeMax = 40;
            NPC.damage = 15;
            NPC.value = 65f;
            NPC.knockBackResist = 0.55f;
            NPC.width = 16;
            NPC.height = 16;
            NPC.scale = 1.1f;
            NPC.lavaImmune = false;
            NPC.alpha = 0;
            NPC.dontTakeDamage = false;
            NPC.HitSound = SoundID.NPCHit30;
            NPC.DeathSound = SoundID.NPCDeath38;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            float chance = SpawnCondition.OverworldNightMonster.Chance * 0.8f;
            if (!spawnInfo.Player.ZoneSnow)
                return 0f;
            return chance;
        }

        int frame = 0;
        public override void FindFrame(int frameHeight)
        {
            //bool expertMode = Main.expertMode;
            //Player player = Main.player[NPC.target];
            NPC.frameCounter += 0.5f;
            if (NPC.frameCounter >= 5)
            {
                frame++;
                NPC.frameCounter = 0;
            }
            if (frame >= 5)
            {
                frame = 0;
            }
            NPC.frame.Y = frameHeight * frame;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            int d = DustID.BlueTorch;
            int d1 = DustID.Frost;
            for (int k = 0; k < 30; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, d, 2.5f * hit.HitDirection, -2.5f, 0, Color.White, 0.7f);
                Dust.NewDust(NPC.position, NPC.width, NPC.height, d1, 2.5f * hit.HitDirection, -2.5f, 0, default(Color), .74f);
            }
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 20; i++)
                {
                    int num = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.CursedTorch, 0f, -2f, 0, default(Color), .8f);
                    Main.dust[num].noGravity = true;
                    Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                    Main.dust[num].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;
                    if (Main.dust[num].position != NPC.Center)
                        Main.dust[num].velocity = NPC.DirectionTo(Main.dust[num].position) * 6f;
                }
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            base.ModifyNPCLoot(npcLoot);
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<WinterbornShard>(), minimumDropped: 1, maximumDropped: 3));
        }

        float alphaCounter;
        public override void AI()
        {
            if (Main.dayTime)
            {
                if (NPC.alpha < 255)
                {
                    NPC.alpha += 2;
                }
                if (NPC.alpha >= 255)
                {
                    NPC.active = false;
                }
                if (alphaCounter >= 0)
                {
                    alphaCounter -= 0.6f;
                }

            }
            else
            {
                alphaCounter = 4;
            }
            float num = 1f - NPC.alpha / 255f;
            alphaCounter = 4;
            bool expertMode = Main.expertMode;
            NPC.spriteDirection = NPC.direction;
            Player player = Main.player[NPC.target];
            NPC.TargetClosest(true);
            NPC.rotation = NPC.velocity.X * 0.05f;
            Timer++;
            NPC.rotation = NPC.velocity.X * .08f;

            float velMax = 1f;
            float acceleration = 0.011f;
            NPC.TargetClosest(true);
            Vector2 center = NPC.Center;
            float deltaX = Main.player[NPC.target].position.X + Main.player[NPC.target].width / 2 - center.X;
            float deltaY = Main.player[NPC.target].position.Y + Main.player[NPC.target].height / 2 - center.Y;
            float distance = (float)Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
            if (NPC.ai[1] > 200.0)
            {

                if (NPC.ai[1] > 300.0)
                {
                    NPC.ai[1] = 0f;
                }
            }
            else if (distance < 120.0)
            {
                NPC.ai[0] += 0.9f;
                if (NPC.ai[0] > 0f)
                {
                    NPC.velocity.Y = NPC.velocity.Y + 0.039f;
                }
                else
                {
                    NPC.velocity.Y = NPC.velocity.Y - 0.019f;
                }
                if (NPC.ai[0] < -100f || NPC.ai[0] > 100f)
                {
                    NPC.velocity.X = NPC.velocity.X + 0.029f;
                }
                else
                {
                    NPC.velocity.X = NPC.velocity.X - 0.029f;
                }
                if (NPC.ai[0] > 25f)
                {
                    NPC.ai[0] = -200f;
                }
            }
            if (Main.rand.NextBool(30) && Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (Main.rand.NextBool(2))
                {
                    NPC.velocity.Y = NPC.velocity.Y + 0.439f;
                }
                else
                {
                    NPC.velocity.Y = NPC.velocity.Y - 0.419f;
                }
                NPC.netUpdate = true;
            }
            if (distance > 350.0)
            {
                velMax = 5f;
                acceleration = 0.2f;
            }
            else if (distance > 300.0)
            {
                velMax = 3f;
                acceleration = 0.25f;
            }
            else if (distance > 250.0)
            {
                velMax = 2.5f;
                acceleration = 0.13f;
            }
            float stepRatio = velMax / distance;
            float velLimitX = deltaX * stepRatio;
            float velLimitY = deltaY * stepRatio;
            if (Main.player[NPC.target].dead)
            {
                velLimitX = (float)(NPC.direction * velMax / 2.0);
                velLimitY = (float)(-velMax / 2.0);
            }
            if (NPC.velocity.X < velLimitX)
                NPC.velocity.X = NPC.velocity.X + acceleration;
            else if (NPC.velocity.X > velLimitX)
                NPC.velocity.X = NPC.velocity.X - acceleration;
            if (NPC.velocity.Y < velLimitY)
                NPC.velocity.Y = NPC.velocity.Y + acceleration;
            else if (NPC.velocity.Y > velLimitY)
                NPC.velocity.Y = NPC.velocity.Y - acceleration;
        }
        public virtual string GlowTexturePath => Texture + "_Glow";
        private Asset<Texture2D> _glowTexture;
        public Texture2D GlowTexture => (_glowTexture ??= (ModContent.RequestIfExists<Texture2D>(GlowTexturePath, out var asset) ? asset : null))?.Value;
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Lighting.AddLight(NPC.Center, Color.Blue.ToVector3() * (alphaCounter / 4) * Main.essScale);
            if (GlowTexture is not null && !Main.dayTime)
            {
   
                SpriteEffects spriteEffects = SpriteEffects.None;
                if (NPC.spriteDirection == 1)
                {
                    spriteEffects = SpriteEffects.FlipHorizontally;
                }
                Vector2 halfSize = new Vector2(GlowTexture.Width / 2, GlowTexture.Height / Main.npcFrameCount[NPC.type] / 2);
                spriteBatch.Draw(
                    GlowTexture,
                    new Vector2(NPC.position.X - screenPos.X + (NPC.width / 2) - GlowTexture.Width * NPC.scale / 2f + halfSize.X * NPC.scale, NPC.position.Y - screenPos.Y + NPC.height - GlowTexture.Height * NPC.scale / Main.npcFrameCount[NPC.type] + 4f + halfSize.Y * NPC.scale + Main.NPCAddHeight(NPC) + NPC.gfxOffY),
                    NPC.frame,
                    Color.White,
                    NPC.rotation,
                    halfSize,
                    NPC.scale,
                    spriteEffects,
                0);
            }
            Texture2D texture2D4 = ModContent.Request<Texture2D>("Stellamod/Effects/Masks/DimLight").Value;
            Main.spriteBatch.Draw(texture2D4, (NPC.Center - Main.screenPosition), null, new Color((int)(45f * alphaCounter), (int)(45f * alphaCounter), (int)(45f * alphaCounter), 0), NPC.rotation, new Vector2(64 / 2, 64 / 2), 0.2f * (2 + 0.3f), SpriteEffects.None, 0f);
            if (Main.rand.NextBool(5))
            {
                int dustnumber = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Frost, 0f, 0f, 150, Color.Blue, 1f);
                Main.dust[dustnumber].velocity *= 0.3f;
                Main.dust[dustnumber].velocity.Y -= 1.3f;
                Main.dust[dustnumber].noGravity = true;
            }

            Texture2D texture2D5 = ModContent.Request<Texture2D>("Stellamod/Effects/Masks/DimLight").Value;
            Main.spriteBatch.Draw(texture2D5, (NPC.Center - Main.screenPosition), null, new Color((int)(15f * alphaCounter), (int)(15f * alphaCounter), (int)(55f * alphaCounter), 0), NPC.rotation, new Vector2(64 / 2, 64 / 2), 0.2f * (2 + 0.3f * 2), SpriteEffects.None, 0f);

            Main.spriteBatch.Draw(texture2D5, (NPC.Center - Main.screenPosition), null, new Color((int)(15f * alphaCounter), (int)(15f * alphaCounter), (int)(55f * alphaCounter), 0), NPC.rotation, new Vector2(64 / 2, 64 / 2), 0.2f * (4 + 0.3f * 2), SpriteEffects.None, 0f);

            Texture2D texture2D6 = ModContent.Request<Texture2D>("Stellamod/Effects/Masks/DimLight").Value;
            Main.spriteBatch.Draw(texture2D6, (NPC.Center - Main.screenPosition), null, new Color((int)(15f * alphaCounter), (int)(15f * alphaCounter), (int)(55f * alphaCounter), 0), NPC.rotation, new Vector2(64 / 2, 64 / 2), 0.2f * (2 + 0.3f * 6), SpriteEffects.None, 0f);

            Texture2D texture2D7 = ModContent.Request<Texture2D>("Stellamod/Effects/Masks/DimLight").Value;
            Main.spriteBatch.Draw(texture2D7, (NPC.Center - Main.screenPosition), null, new Color((int)(15f * alphaCounter), (int)(15f * alphaCounter), (int)(55f * alphaCounter), 0), NPC.rotation, new Vector2(64 / 2, 64 / 2), 0.2f * (2 + 0.3f * 7), SpriteEffects.None, 0f);
        }
    }
}