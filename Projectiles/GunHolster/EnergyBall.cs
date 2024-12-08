using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Common.Shaders;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Trails;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.GunHolster
{
    internal class EnergyBall : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailCacheLength[Type] = 32;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.timeLeft = 180;
            Projectile.friendly = true;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if(Timer % 8 == 0)
            {
                int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<GlowDust>(), newColor: Color.DarkGoldenrod);
                Dust dust = Main.dust[dustIndex];
                dust.velocity = Vector2.Zero;
            }
            Lighting.AddLight(Projectile.Center, Color.White.ToVector3() * 0.2f);
        }

        private float WidthFunction(float completionRatio)
        {
            return MathHelper.Lerp(24f, 0f, completionRatio);
        }

        private Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.AliceBlue, Color.Transparent, completionRatio);
        }

        private Color ColorFunction2(float completionRatio)
        {
            return Color.Lerp(Color.CadetBlue, Color.Transparent, completionRatio);
        }

        public PrimDrawer TrailDrawer { get; private set; } = null;
        private void DrawTrail()
        {
            Main.spriteBatch.RestartDefaults();
            Vector2 drawOffset = -Main.screenPosition + Projectile.Size / 2f; 
            TrailDrawer ??= new PrimDrawer(WidthFunction, ColorFunction, GameShaders.Misc["VampKnives:SuperSimpleTrail"]);
            TrailDrawer.ColorFunc = ColorFunction;
            TrailDrawer.Shader = GameShaders.Misc["VampKnives:SuperSimpleTrail"];
            GameShaders.Misc["VampKnives:SuperSimpleTrail"].SetShaderTexture(TrailRegistry.LightningTrail2);

            TrailDrawer.DrawPrims(Projectile.oldPos, drawOffset, 155);
            TrailDrawer.ColorFunc = ColorFunction2;
            GameShaders.Misc["VampKnives:SuperSimpleTrail"].SetShaderTexture(TrailRegistry.LightningTrail2Outline);
            TrailDrawer.DrawPrims(Projectile.oldPos, drawOffset, 155);
        }

        private void DrawEnergyBall(ref Color lightColor)
        {
            //Draw Code for the orb
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 centerPos = Projectile.Center - Main.screenPosition;
            GlowCircleShader shader = GlowCircleShader.Instance;

            //How quickly it lerps between the colors
            shader.Speed = 10f;

            //This effects the distribution of colors
            shader.BasePower = 1f;

            //Radius of the circle
            shader.Size = 0.12f;


            //Colors
            Color startInner = Color.Lerp(Color.SkyBlue, Color.BlueViolet, VectorHelper.Osc(0f, 1f, speed: 5f)); ;
            Color startGlow = Color.Lerp(Color.CadetBlue, Color.CadetBlue, VectorHelper.Osc(0f, 1f, speed: 3f));
            Color startOuterGlow = Color.Lerp(Color.Black, Color.Black, VectorHelper.Osc(0f, 1f, speed: 3f));
     
            shader.InnerColor = startInner;
            shader.GlowColor = startGlow;
            shader.OuterGlowColor = startOuterGlow;

            //Idk i just included this to see how it would look
            //Don't go above 0.5;
            shader.Pixelation = 0.0055f;
            shader.Apply();

            SpriteBatch spriteBatch = Main.spriteBatch;  
            spriteBatch.Restart(blendState: BlendState.Additive, effect: shader.Effect);
            for (int i = 0; i < 3; i++)
            {
                spriteBatch.Draw(texture, centerPos, null, Color.White, Projectile.rotation, texture.Size() / 2f, 1f, SpriteEffects.None, 0);
            }
      
            spriteBatch.RestartDefaults();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawTrail();
            DrawEnergyBall(ref lightColor);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            FXUtil.GlowCircleBoom(Projectile.Center,
              innerColor: Color.White,
              glowColor: Color.Blue,
              outerGlowColor: Color.Red, duration: 25f, baseSize: 0.06f);
            for (int i = 0; i < 4; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.AliceBlue, 1f).noGravity = true;
            }
        }
    }
}
