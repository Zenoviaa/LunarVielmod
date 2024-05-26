using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Dusts;
using Stellamod.Projectiles.IgniterExplosions;
using Stellamod.Projectiles.Spears;
using Stellamod.Trails;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Magic
{

    internal class ITProj : ModProjectile
    {
        float WhiteTimerProg;
        bool Moved;
        float WhiteTimer;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("The Irradiaspear");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 15;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }
        public override void SetDefaults()
        {
            Projectile.penetrate = 5;
            Projectile.width = 17;
            Projectile.height = 16;
            Projectile.timeLeft = 860;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }
        public override void AI()
        {
            Projectile.velocity *= .96f;
            Projectile.ai[1]++;
            if (!Moved && Projectile.ai[1] >= 0)
            {
                SoundEngine.PlaySound(new SoundStyle($"{nameof(Stellamod)}/Assets/Sounds/IrradiatedNest_Fall"), Projectile.position);
                Projectile.spriteDirection = Projectile.direction;
                Moved = true;
            }
            if (Projectile.ai[1] == 30)
            {
                SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/ITBeep");
                //Between -1 and 1f
                soundStyle.Pitch = 0.8f;
                SoundEngine.PlaySound(soundStyle);
                WhiteTimer = 1;
            }
            if (Projectile.ai[1] == 60)
            {
                SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/ITBeep");
                //Between -1 and 1f
                soundStyle.Pitch = 0.9f;
                SoundEngine.PlaySound(soundStyle);
                WhiteTimer = 1;
            }
            if (Projectile.ai[1] == 90)
            {
                SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/ITBeep");
                //Between -1 and 1f
                soundStyle.Pitch = 1f;
                SoundEngine.PlaySound(soundStyle);
                WhiteTimer = 1;
            }
            if (Projectile.ai[1] >= 120)
            {
                Projectile.Kill();
                WhiteTimer = 1;
            }


            if (Projectile.ai[1] >= 60)
            {
                int S1 = Main.rand.Next(0, 3);
                if (S1 == 0)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.DarkSeaGreen, 0.4f).noGravity = true;
                    }
                }


            }
            if (Projectile.ai[1] >= 90)
            {
                for (int i = 0; i < 2; i++)
                {
                    Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 150, Color.DarkGray, 1f).noGravity = true;
                }
            }
            WhiteTimer = MathHelper.Lerp(WhiteTimer, 0, 0.1f);
            if (Projectile.alpha >= 255)
            {

            }
            Rectangle myRect = Projectile.getRect();
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile p = Main.projectile[i];
                if (!p.active)
                    continue;
                if (p.type != ModContent.ProjectileType<ITExplosionProj>())
                    continue;
                if (p == Projectile)
                    continue;
                Rectangle otherRect = p.getRect();
                if (Projectile.Colliding(myRect, otherRect))
                {
                    if(Projectile.ai[1] <= 100)
                    {
                        SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/ITPrimer"), Projectile.position);
                        Projectile.ai[1] = 111;
                    }
                }
            }
            Projectile.spriteDirection = Projectile.direction;
        }
        public override void OnKill(int timeLeft)
        {
            var entitySource = Projectile.GetSource_Death();
            Projectile.NewProjectile(entitySource, Projectile.Center.X, Projectile.Center.Y, 0, 0, ModContent.ProjectileType<IrradiatedBoom>(), Projectile.damage, 1, Projectile.owner, 0, 0);

            SoundEngine.PlaySound(SoundID.DD2_BetsyFireballImpact, Projectile.position);
             Projectile.NewProjectile(entitySource, Projectile.Center.X, Projectile.Center.Y, 0, 0, ModContent.ProjectileType<ITExplosionProj>(), Projectile.damage, 1, Projectile.owner, 0, 0);
            int S1 = Main.rand.Next(0, 3);
            if (S1 == 0)
            {
                SoundEngine.PlaySound(new SoundStyle($"{nameof(Stellamod)}/Assets/Sounds/ITBomb1"), Projectile.position);
            }
            if (S1 == 1)
            {
                SoundEngine.PlaySound(new SoundStyle($"{nameof(Stellamod)}/Assets/Sounds/ITBomb2"), Projectile.position);
            }
            if (S1 == 2)
            {
                SoundEngine.PlaySound(new SoundStyle($"{nameof(Stellamod)}/Assets/Sounds/ITBomb3"), Projectile.position);
            }
            Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(Projectile.Center, 2048f, 16f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Main.rand.NextBool(5))
            {
                int dustnumber = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.CursedTorch, 0f, 0f, 150, Color.MediumPurple, 1f);
                Main.dust[dustnumber].velocity *= 0.3f;
                Main.dust[dustnumber].noGravity = true;
            }
            SpriteEffects Effects = Projectile.spriteDirection != 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            Main.instance.LoadProjectile(Projectile.type);
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

            // Redraw the projectile with the color not influenced by light
            Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(Color.Lerp(new Color(152, 208, 113), new Color(53, 107, 112), 1f / Projectile.oldPos.Length * k) * (1f - 1f / Projectile.oldPos.Length * k));
                Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, Effects, 0);
            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            return true;
        }
        public override void PostDraw(Color lightColor)
        {
            Lighting.AddLight(Projectile.Center, Color.DarkSeaGreen.ToVector3() * 1.75f * Main.essScale);
            string glowTexture = Texture + "_White";
            Texture2D whiteTexture = ModContent.Request<Texture2D>(glowTexture).Value;

            Vector2 textureSize = new Vector2(70, 74);
            Vector2 drawOrigin = textureSize / 2;

            //Lerping
            float progress = WhiteTimer;
            Color drawColor = Color.Lerp(Color.Transparent, Color.White, progress);
            Vector2 drawPosition = Projectile.position - Main.screenPosition + drawOrigin;
            Main.spriteBatch.Draw(whiteTexture, drawPosition, Projectile.Frame(), drawColor, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0f);
        }
    }

}


