using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Core.Shaders;
using Stellamod.Helpers;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Illuria.BossesIL.CrumblingTowerOfIlluria.Projectiles
{
    public class DiscoLight : ModProjectile
    {
        private float _lightAlpha;


        private VertexPositionColorTexture[] _vertices;
        private ref float Timer => ref Projectile.ai[0];
        private ref float Style => ref Projectile.ai[1];
        private NPC Parent
        {
            get => Main.npc[(int)Projectile.ai[2]];
        }

        public Color discoColor;
        public override string Texture => TextureRegistry.EmptyTexture;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.tileCollide = false;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 120;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override void AI()
        {
            base.AI();
            switch (Style)
            {
                default:
                case 0:
                    discoColor = Color.White;
                    break;
                case 1:
                    discoColor = Color.Cyan;
                    break;
                case 2:
                    discoColor = Color.White * 0.5f;
                    break;
                case 3:
                    discoColor = Color.Cyan * 0.5f;
                    break;
            }
            Timer++;
            float discoTime = 120;
            float completionRatio = Timer / discoTime;
            _lightAlpha = EasingFunction.QuadraticBump(completionRatio);

            float direction = Style == 0 ? 1 : -1;
            Projectile.velocity = Projectile.velocity.RotatedBy(0.02f * direction);
            Projectile.Center = Parent.Center;
            RayCast(Projectile.Center, Projectile.velocity.SafeNormalize(Vector2.Zero), 800, Projectile.velocity.Length() * _lightAlpha);
        }


        public void RayCast(Vector2 position, Vector2 direction, float edgeLightWidth, float distance)
        {

            _vertices ??= new VertexPositionColorTexture[12];
            float edgeLightRadius = edgeLightWidth / 2f;
            float castMultiplier = 0.1f;
            float edgeColorMultiplier = 0f;
            Vector2 start = position;
            Vector2 end = start + direction * distance;

            //First Quad
            Vector2 topRightVertex = end - direction.RotatedBy(MathHelper.PiOver2) * edgeLightRadius;
            Vector2 bottomRightVertex = end;

            Vector2 topLeftVertex = start - direction.RotatedBy(MathHelper.PiOver2) * edgeLightRadius * castMultiplier;
            Vector2 bottomLeftVertex = start;
            Color lightColor = discoColor * _lightAlpha;

            _vertices[0] = new VertexPositionColorTexture(new Vector3(topLeftVertex, 0), lightColor, new Vector2(1, 1));
            _vertices[1] = new VertexPositionColorTexture(new Vector3(bottomLeftVertex, 0), lightColor, new Vector2(1, 0));
            _vertices[2] = new VertexPositionColorTexture(new Vector3(bottomRightVertex, 0), lightColor * edgeColorMultiplier, new Vector2(0, 0));

            _vertices[3] = new VertexPositionColorTexture(new Vector3(topLeftVertex, 0), lightColor, new Vector2(1, 1));
            _vertices[4] = new VertexPositionColorTexture(new Vector3(topRightVertex, 0), lightColor * edgeColorMultiplier, new Vector2(0, 1));
            _vertices[5] = new VertexPositionColorTexture(new Vector3(bottomRightVertex, 0), lightColor * edgeColorMultiplier, new Vector2(0, 0));

            //Second Quad
            topRightVertex = end;
            bottomRightVertex = end + direction.RotatedBy(MathHelper.PiOver2) * edgeLightRadius;

            topLeftVertex = start;
            bottomLeftVertex = start + direction.RotatedBy(MathHelper.PiOver2) * edgeLightRadius * castMultiplier;

            _vertices[6] = new VertexPositionColorTexture(new Vector3(topLeftVertex, 0), lightColor, new Vector2(0, 0));
            _vertices[7] = new VertexPositionColorTexture(new Vector3(bottomLeftVertex, 0), lightColor, new Vector2(0, 1));
            _vertices[8] = new VertexPositionColorTexture(new Vector3(bottomRightVertex, 0), lightColor * edgeColorMultiplier, new Vector2(1, 1));

            _vertices[9] = new VertexPositionColorTexture(new Vector3(topLeftVertex, 0), lightColor, new Vector2(0, 0));
            _vertices[10] = new VertexPositionColorTexture(new Vector3(topRightVertex, 0), lightColor * edgeColorMultiplier, new Vector2(1, 0));
            _vertices[11] = new VertexPositionColorTexture(new Vector3(bottomRightVertex, 0), lightColor * edgeColorMultiplier, new Vector2(1, 1));
        }

        private void SlowDrawConeLight(ref Color lightColor)
        {
            //TODO: Batch these together, just making sure it actually looks good before I do that.
            var shader = LanternShader.Instance;
            shader.Apply();
            foreach (var pass in shader.Effect.CurrentTechnique.Passes)
            {
                pass.Apply();
            }

            GraphicsDevice graphicsDevice = Main.instance.GraphicsDevice;
            GraphicsHelpers.SaveGraphicsDeviceState();

            graphicsDevice.RasterizerState.CullMode = CullMode.None;
            graphicsDevice.BlendState = BlendState.Additive;
            graphicsDevice.DrawUserPrimitives(
              PrimitiveType.TriangleList, _vertices, 0, _vertices.Length / 3);

            GraphicsHelpers.RestoreGraphicsDeviceState();

        }
        public override bool PreDraw(ref Color lightColor)
        {
            SlowDrawConeLight(ref lightColor);
            return false;
        }
    }
}
