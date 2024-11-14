using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Stellamod.Common.MaskingShaderSystem;
using Stellamod.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Terraria.Graphics.Shaders;

namespace Stellamod.Projectiles.Test
{
    internal class AlcadBall : ModProjectile,
          IPreDrawMaskShader,
          IDrawMaskShader
    {
        private Vector2 _drawScale;
        private float _scale;
        private float _scaleMult;
        private float _scaleOutMult;
        private bool _hasHitWall;
        private float _scaleOutTimer;
        private ref float Timer => ref Projectile.ai[0];
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailCacheLength[Type] = 24;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            _drawScale = Vector2.Zero;
            _scaleMult = Main.rand.NextFloat(0.8f, 1f);
            _scaleOutMult = 1f;
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 120;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 20;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if (Timer % 4 == 0)
            {
                Color color = Color.Lerp(Color.White, Color.LightBlue, Main.rand.NextFloat(0f, 1f));
                color.A = 0;
                Vector2 velocity = Main.rand.NextVector2Circular(2, 2);
                Vector2 spawnPoint = Projectile.Center + Main.rand.NextVector2Circular(4, 4);
                //Dust.NewDustPerfect(spawnPoint, ModContent.DustType<MothlightDust>(), Velocity: velocity, Scale: 1f);
            }


            if (_hasHitWall)
            {
                _drawScale.X = MathHelper.Lerp(_drawScale.X, 0.2f * _scaleMult, 0.08f);
                _scale = _drawScale.X;
                Projectile.velocity *= 0.9f;
            }
            else
            {
                _drawScale = Vector2.Lerp(_drawScale, Vector2.One * 0.5f * _scaleMult, 0.04f);
                _scale = _drawScale.X;
                float maxHomingDetectDistance = 256f;
                float maxDegreesRotate = 10;
                NPC npcToChase = ProjectileHelper.FindNearestEnemy(Projectile.Center, maxHomingDetectDistance);
                if (npcToChase != null)
                    Projectile.velocity = ProjectileHelper.SimpleHomingVelocity(Projectile, npcToChase.Center, degreesToRotate: maxDegreesRotate);
                Projectile.rotation = Projectile.velocity.ToRotation();
            }

            if (Projectile.timeLeft <= 30)
            {
                _scaleOutTimer++;
                float scaleProgress = _scaleOutTimer / 30f;
                float easedProgress = Easing.InOutCubic(scaleProgress);
                _scaleOutMult = MathHelper.Lerp(1f, 0f, easedProgress);
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            _hasHitWall = true;
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }


        public MiscShaderData GetMaskDrawShader()
        {
            //Use the defaults
            var shaderData = GameShaders.Misc["LunarVeil:SimpleDistortion"];
            shaderData.Shader.Parameters["time"].SetValue(Main.GlobalTimeWrappedHourly * 15);
            shaderData.Shader.Parameters["distortion"].SetValue(0.2f);
            shaderData.Shader.Parameters["distortingNoiseTexture"].SetValue(TextureRegistry.CloudNoise2.Value);
            return shaderData;
        }
        public void PreDrawMask(SpriteBatch spriteBatch)
        {
            var shaderData = GameShaders.Misc["LunarVeil:SimpleDistortion"];
            shaderData.Shader.Parameters["time"].SetValue(Main.GlobalTimeWrappedHourly * 15);
            shaderData.Shader.Parameters["distortion"].SetValue(0.2f);
            shaderData.Shader.Parameters["distortingNoiseTexture"].SetValue(TextureRegistry.CloudNoise2.Value);
            shaderData.Apply();


            Texture2D texture = ModContent.Request<Texture2D>(Texture + "_Outline").Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Color drawColor = Color.White;
            Vector2 drawOrigin = texture.Size() / 2f;
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer,
                   shaderData.Shader, Main.GameViewMatrix.TransformationMatrix);

            spriteBatch.Draw(texture, drawPos, null, drawColor, Projectile.rotation, drawOrigin, _drawScale * _scaleOutMult, SpriteEffects.None, 0f);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        }

        private Color ColorFunc(float p)
        {
            return Color.White;
        }

        private float WidthFunc(float p)
        {
            return MathHelper.Lerp(64 * _scale * _scaleMult * _scaleOutMult, 0, Easing.OutExpo(p, 5));
        }

        public void DrawMask(SpriteBatch spriteBatch)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Color drawColor = Color.White;
            Vector2 drawOrigin = texture.Size() / 2f;

            spriteBatch.Draw(texture, drawPos, null, drawColor, Projectile.rotation, drawOrigin, _drawScale * _scaleOutMult, SpriteEffects.None, 0f);
        }
    }
}
