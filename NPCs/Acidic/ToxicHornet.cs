using Microsoft.Xna.Framework;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Buffs;
using Stellamod.Items.Materials;
using Stellamod.Items.Weapons.Ranged.GunSwapping;
using Stellamod.Items.Weapons.Summon;
using Stellamod.NPCs.Bosses.INest;
using Stellamod.NPCs.Overworld.ShadowWraith;
using Stellamod.Utilis;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
using static Terraria.ModLoader.ModContent;

namespace Stellamod.NPCs.Acidic
{
    public class ToxicHornet : ModNPC
    {
        public int moveSpeed = 0;
        public int moveSpeedY = 0;
        public int counter;
        public bool dash = false;
        public short npcCounter = 0;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Ruby Beetle");
            Main.npcFrameCount[NPC.type] = 4;
            NPCID.Sets.TrailCacheLength[NPC.type] = 5;
            NPCID.Sets.TrailingMode[NPC.type] = 2;
        }
        Vector2 Drawoffset => new Vector2(0, NPC.gfxOffY) + Vector2.UnitX * NPC.spriteDirection * 0;
        public virtual string GlowTexturePath => Texture + "_Glow";
        private Asset<Texture2D> _glowTexture;
        public Texture2D GlowTexture => (_glowTexture ??= (ModContent.RequestIfExists<Texture2D>(GlowTexturePath, out var asset) ? asset : null))?.Value;
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            float num108 = 4;
            float num107 = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 1.4f / 1.4f * 6.28318548f)) / 2f + 0.5f;
            float num106 = 0f;
            Color color1 = Color.DarkSeaGreen * num107 * .8f;
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
            //Vector2 vector33 = new Vector2(NPC.Center.X, NPC.Center.Y) - Main.screenPosition + Drawoffset - NPC.velocity;
            Color color29 = new Color(127 - NPC.alpha, 127 - NPC.alpha, 127 - NPC.alpha, 0).MultiplyRGBA(Color.Green);
            for (int num103 = 0; num103 < 4; num103++)
            {
                Color color28 = color29;
                color28 = NPC.GetAlpha(color28);
                color28 *= 1f - num107;
                Vector2 vector29 = NPC.Center + (num103 / (float)num108 * 6.28318548f + NPC.rotation + num106).ToRotationVector2() * (4f * num107 + 2f) - Main.screenPosition + Drawoffset - NPC.velocity * num103;
                Main.spriteBatch.Draw(GlowTexture, vector29, NPC.frame, color28, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, spriteEffects3, 0f);
            }
        }
        public override void SetDefaults()
        {
            NPC.width = 32;
            NPC.height = 42;
            NPC.damage = 43;
            NPC.defense = 16;
            NPC.lifeMax = 210;
            NPC.noGravity = true;
            NPC.value = 90f;
            NPC.noTileCollide = false;
            NPC.HitSound = SoundID.NPCHit42;
            NPC.DeathSound = SoundID.NPCDeath4;
            NPC.aiStyle = 0;
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            Player player = spawnInfo.Player;
            if (!(player.ZoneTowerSolar || player.ZoneTowerVortex || player.ZoneTowerNebula || player.ZoneTowerStardust && !Main.pumpkinMoon && !Main.snowMoon))
            {
                return spawnInfo.Player.ZoneAcid() && Main.hardMode ? 0.7f : 0f;
            }
            return 0f;
        }

        public override void AI()
        {
            if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
            {
                NPC.TargetClosest();
            }
            if (counter == 0)
            {
                if (npcCounter >= 4)
                {
                    npcCounter = 0;
                    NPC.ai[0] = 150;
                }
            }
            counter++;
            NPC.spriteDirection = NPC.direction;
            Player player = Main.player[NPC.target];
            NPC.rotation = NPC.velocity.X * 0.1f;
            int xSpeed = 21;
            if (NPC.Center.X >= player.Center.X && moveSpeed >= -xSpeed)
            {
                moveSpeed--;
            }

            if (NPC.Center.X <= player.Center.X && moveSpeed <= xSpeed)
            {
                moveSpeed++;
            }

            NPC.velocity.X = moveSpeed * 0.14f;

            if (NPC.Center.Y >= player.Center.Y - NPC.ai[0] && moveSpeedY >= -50)
            {
                moveSpeedY--;
                NPC.ai[0] = 150f;
            }

            if (NPC.Center.Y <= player.Center.Y - NPC.ai[0] && moveSpeedY <= 50)
            {
                moveSpeedY++;
            }

            NPC.velocity.Y = moveSpeedY * 0.23f;
            if (counter >= 110 && counter < 140)
            {
                dash = true;
                NPC.velocity *= 0.95f;
            }

            if (counter == 140)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Vector2 direction = player.Center - NPC.Center;
                    direction.Normalize();
                    direction.X *= 5f;
                    direction.Y *= 5f;
                    NPC.velocity = direction;
                }
            }
            if (counter == 180)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    NPC.ai[0] += -25f;
                NPC.velocity = Vector2.Zero;
                counter = 0;
                dash = false;
            }
            NPC.ai[3]++;
            if (NPC.ai[3] >= 300)
            {
                NPC.ai[3] = 0;
                Vector2 direction = Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center) * 8.5f;
                Vector2 KBdirection = Vector2.Normalize(NPC.Center - Main.player[NPC.target].Center) * 8.5f;
                NPC.velocity += KBdirection;
                SoundEngine.PlaySound(SoundID.DD2_BallistaTowerShot, NPC.position);
                SoundEngine.PlaySound(SoundID.Zombie48, NPC.position);
                float offsetX = Main.rand.Next(-50, 50) * 0.01f;
                float offsetY = Main.rand.Next(-50, 50) * 0.01f;
                int damage = Main.expertMode ? 30 : 37;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, direction.X * 3 + offsetX, direction.Y * 3 + offsetY, ModContent.ProjectileType<ToxicMissile>(), damage, 1, Main.myPlayer, 0, 0);
            }
        }


        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            base.ModifyNPCLoot(npcLoot);
            if (Main.hardMode)
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<GraftedSoul>(), 2, 1, 5));
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SrTetanus>(), 20, 1, 1));
            }
            npcLoot.Add(ItemDropRule.Common(ItemType<VirulentPlating>(), minimumDropped: 1, maximumDropped: 4));
            npcLoot.Add(ItemDropRule.Common(ItemType<ToxicHornetStaff>(), chanceDenominator: 20));
        }


        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            Lighting.AddLight(NPC.Center, Color.GreenYellow.ToVector3() * 1.25f * Main.essScale);
            SpriteEffects Effects = ((base.NPC.spriteDirection != -1) ? SpriteEffects.FlipHorizontally : SpriteEffects.None);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            var drawOrigin = new Vector2(TextureAssets.Npc[NPC.type].Width() * 0.5f, NPC.height * 0.5f);
            for (int k = 0; k < NPC.oldPos.Length; k++)
            {
                Vector2 drawPos = NPC.oldPos[k] - Main.screenPosition + NPC.Size / 2 + new Vector2(0f, NPC.gfxOffY);
                Color color = NPC.GetAlpha(Color.Lerp(new Color(255, 253, 90), new Color(72, 131, 56), 1f / NPC.oldPos.Length * k) * (1f - 1f / NPC.oldPos.Length * k));
                spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, drawPos, new Microsoft.Xna.Framework.Rectangle?(NPC.frame), color, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, Effects, 0f);
            }

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            return true;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            target.AddBuff(BuffType<AcidFlame>(), 200);
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 0.22f;
            NPC.frameCounter %= Main.npcFrameCount[NPC.type];
            int frame = (int)NPC.frameCounter;
            NPC.frame.Y = frame * frameHeight;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 3; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height,
                    ModContent.DustType<Dusts.GlowDust>(), newColor: new Color(24, 142, 61));
                int d = Dust.NewDust(NPC.position, NPC.width, NPC.height,
                    ModContent.DustType<Dusts.GunFlash>(), newColor: new Color(24, 142, 61));
                Main.dust[d].rotation = (Main.dust[d].position - NPC.position).ToRotation() - MathHelper.PiOver4;
            }
        }
    }
}