
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Helpers;
using Stellamod.Items.Consumables;
using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;
using Stellamod.Items.Weapons.Mage;
using Stellamod.Items.Weapons.Melee;
using Stellamod.Items.Weapons.Ranged;
using Stellamod.NPCs.Bosses.Caeva;
using Stellamod.NPCs.Bosses.DreadMire.Heart;
using Stellamod.NPCs.Bosses.Jack;
using Stellamod.Utilis;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;


namespace Stellamod.NPCs.Bosses.Fenix.Projectiles
{

    public class FenixSnipe : ModNPC
    {
        public float Die = 1;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Shadow Wraith");
            NPCID.Sets.TrailingMode[NPC.type] = 2;
            NPCID.Sets.TrailCacheLength[NPC.type] = 15;
        }


        public override void SetDefaults()
        {
            NPC.scale = 1f;
            NPC.width = 60;
            NPC.height = 60;
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.lifeMax = 1;
            NPC.HitSound = SoundID.DD2_SkeletonHurt;
            NPC.DeathSound = SoundID.DD2_SkeletonDeath;
            NPC.value = 30f;
            NPC.buffImmune[BuffID.ShadowFlame] = true;
            NPC.alpha = 60;
            NPC.knockBackResist = .75f;
            NPC.alpha = 0;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.dontCountMe = true;
            NPC.dontTakeDamage = true;
            NPC.aiStyle = 0;
        }



        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;

            // Redraw the projectile with the color not influenced by light
            Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, NPC.height * 0.5f);
            for (int k = 0; k < NPC.oldPos.Length; k++)
            {
                Vector2 drawPos = NPC.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, NPC.gfxOffY);
                Color color = NPC.GetAlpha(Color.Lerp(new Color(106, 255, 255), new Color(151, 46, 175), 1f / NPC.oldPos.Length * k) * (1f - 1f / NPC.oldPos.Length * k));
                Main.EntitySpriteDraw(texture, drawPos, null, color, NPC.rotation, drawOrigin, NPC.scale, SpriteEffects.None, 0);
            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            return true;
        }

        Vector2 Drawoffset => new Vector2(0, NPC.gfxOffY) + Vector2.UnitX * NPC.spriteDirection * 0;

        public virtual string GlowTexturePath => Texture + "_Glow";
        private Asset<Texture2D> _glowTexture;
        public Texture2D GlowTexture => (_glowTexture ??= (ModContent.RequestIfExists<Texture2D>(GlowTexturePath, out var asset) ? asset : null))?.Value;
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            
            float num107 = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 1.4f / 1.4f * 6.28318548f)) / 2f + 0.5f;
         
            Color color1 = Color.White * num107 * .8f;
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

            SpriteEffects spriteEffects3 = NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
        }

        public override void AI()
        {
            Player player = Main.player[NPC.target];

            NPC.spriteDirection = -NPC.direction;
            NPC.ai[0]++;

            if (NPC.ai[0] <= 1)
            {
                if (StellaMultiplayer.IsHost)
                {
                    NPC.position.X += Main.rand.Next(-350, 351);
                    NPC.position.Y += Main.rand.Next(-350, 351);
                    NPC.netUpdate = true;
                }
            }

            Die -= 0.008f;
            if (Die < 0.08)
            {
                for (int i = 0; i < 20; i++)
                {
                    int num1 = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.BoneTorch, 0f, -2f, 0, default, .8f);
                    Main.dust[num1].noGravity = true;
                    Main.dust[num1].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                    Main.dust[num1].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;
                    if (Main.dust[num1].position != NPC.Center)
                        Main.dust[num1].velocity = NPC.DirectionTo(Main.dust[num1].position) * 6f;
                    int num = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.WaterCandle, 0f, -2f, 0, default, .8f);
                    Main.dust[num].noGravity = true;
                    Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                    Main.dust[num].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;
                    if (Main.dust[num].position != NPC.Center)
                        Main.dust[num].velocity = NPC.DirectionTo(Main.dust[num].position) * 6f;
                }
                var entitySource = NPC.GetSource_FromThis();
                if (StellaMultiplayer.IsHost)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, 0, 0, ModContent.ProjectileType<CaevaSpawnEffect>(), 40, 1, Main.myPlayer, 0, 0);
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, 25, 0, ModContent.ProjectileType<NekoNeko>(), 60, 1, Main.myPlayer, 0, 0);
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, -25, 0, ModContent.ProjectileType<NekoNeko>(), 60, 1, Main.myPlayer, 0, 0);
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, 25 * 0, -25, ModContent.ProjectileType<NekoNeko>(), 60, 1, Main.myPlayer, 0, 0);
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, 25 * 0, 25, ModContent.ProjectileType<NekoNeko>(), 60, 1, Main.myPlayer, 0, 0);
                }

                Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(base.NPC.Center, 1212f, 62f);
                SoundEngine.PlaySound(SoundID.Item27, NPC.position);
                SoundEngine.PlaySound(SoundID.Item50, NPC.position);
                NPC.active = false;
            }

            if (NPC.alpha > 0)
            {
                NPC.alpha -= 2;
            }
            
            
            NPC.velocity = Vector2.Lerp(NPC.velocity, VectorHelper.MovemontVelocity(NPC.Center, Vector2.Lerp(NPC.Center, player.Center, 0.045f * Die), NPC.Center.Distance(player.Center) * 0.55f * Die), 0.018f * Die);
            NPC.rotation = NPC.velocity.X * 2;
            if (NPC.position.Y <= player.position.Y)
            {
                NPC.velocity.Y += 0.3f;
            }
            else
            {
                NPC.velocity.Y -= 0.3f;
            }
        }
    }
}