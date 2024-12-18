using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Common.MaskingShaderSystem;
using Stellamod.Helpers;
using Stellamod.Trails;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.IgniterExplosions
{
    public class AlcaricMushBoom2 : ModProjectile,
        IDrawMaskShader
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("FrostShotIN");
            Main.projFrames[Projectile.type] = 48;
        }

        public override void SetDefaults()
        {
            Projectile.friendly = false;
            Projectile.width = 128;
            Projectile.height = 128;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 48;
            Projectile.scale = 0.5f;

        }
        public float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        public override void AI()
        {
            Projectile.rotation -= 0.01f;
            Vector3 RGB = new(0.89f, 2.53f, 2.55f);
            // The multiplication here wasn't doing anything
            Lighting.AddLight(Projectile.position, RGB.X, RGB.Y, RGB.Z);

        }

        public override bool PreAI()
        {
            Projectile.tileCollide = false;
            if (++Projectile.frameCounter >= 1)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 48)
                {
                    Projectile.frame = 0;
                }
            }
            return true;


        }

        public MiscShaderData GetMaskDrawShader()
        {
            var shaderData = GameShaders.Misc["LunarVeil:SimpleDistortion"];
            shaderData.Shader.Parameters["time"].SetValue(Main.GlobalTimeWrappedHourly * 15);
            shaderData.Shader.Parameters["distortion"].SetValue(0.002f);
            shaderData.Shader.Parameters["distortingNoiseTexture"].SetValue(TextureRegistry.CloudNoise2.Value);
            return shaderData;
        }

        public void DrawMask(SpriteBatch spriteBatch)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, Projectile.Frame(), Color.White, Projectile.rotation, Projectile.Frame().Size() / 2f, Projectile.scale, SpriteEffects.None, 0f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }
}