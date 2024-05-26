using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Trails;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;


namespace Stellamod.NPCs.Bosses.GothiviaTheSun.REK.Projectiles
{
    internal class RekFireShockWave : ModProjectile,
        IPixelPrimitiveDrawer
    {
        //Texture
        public override string Texture => TextureRegistry.EmptyTexture;

        //AI
        private float LifeTime => 45f;
        private ref float Timer => ref Projectile.ai[0];
        private float Progress
        {
            get
            {
                float p = Timer / LifeTime;
                return MathHelper.Clamp(p, 0, 1);
            }
        }

        //Draw Code
        private PrimitiveTrail BeamDrawer;
        private int DrawMode;

        //Trailing
        private Asset<Texture2D> FrontTrailTexture => TrailRegistry.WhispyTrail;
        private MiscShaderData FrontTrailShader => TrailRegistry.FireVertexShader;

        private Asset<Texture2D> BackTrailTexture => TrailRegistry.FadedStreak;
        private MiscShaderData BackTrailShader => TrailRegistry.LaserShader;

        //Radius
        private float StartRadius => 4;
        private float EndRadius => 768;
        private float Width => 96;

        //Colors
        private Color FrontCircleStartDrawColor => Color.OrangeRed;
        private Color FrontCircleEndDrawColor => Color.OrangeRed;
        private Color BackCircleStartDrawColor => Color.Lerp(Color.White, Color.Orange, 0.4f);
        private Color BackCircleEndDrawColor => Color.Orange;
        private Vector2[] CirclePos;

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.timeLeft = (int)LifeTime;
            Projectile.tileCollide = false;

            //Points on the circle
            CirclePos = new Vector2[64];
        }

        public override void AI()
        {
            Timer++;
            if(Timer == 1)
            {
                //Shockwave Push
                float shockwavePushDistance = 1024;
                for(int i = 0; i < Main.maxPlayers; i++)
                {
                    Player player = Main.player[i];
                    if (!player.active)
                        continue;
                    if(Vector2.Distance(player.Center, Projectile.Center) <= shockwavePushDistance)
                    {
                        Vector2 directionToPlayer = Projectile.Center.DirectionTo(player.Center);
                        Vector2 pushVelocity = directionToPlayer * 24f;
                        player.velocity += pushVelocity;
                    }
                }
            }

            AI_ExpandCircle();
        }

        private void AI_ExpandCircle()
        {
            float easedProgess = Easing.InOutCirc(Progress);
            float radius = MathHelper.Lerp(StartRadius, EndRadius, Progress);
            DrawCircle(radius);
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.OnFire3, 60);
        }

        private void DrawCircle(float radius)
        {
            Vector2 startDirection = Vector2.UnitY;
            for (int i = 0; i < CirclePos.Length; i++)
            {
                float circleProgress = i / (float)CirclePos.Length;
                float radiansToRotateBy = circleProgress * (MathHelper.TwoPi + MathHelper.PiOver4 / 2);
                CirclePos[i] = Projectile.Center + startDirection.RotatedBy(radiansToRotateBy) * radius;
            }
        }

        public float WidthFunction(float completionRatio)
        {
            float width = Width;
            float startExplosionScale = 4f;
            float endExplosionScale = 0f;
            float easedProgess = Easing.OutCirc(Progress);
            float scale = MathHelper.Lerp(startExplosionScale, endExplosionScale, easedProgess);
            switch (DrawMode)
            {
                default:
                case 0:
                    return Projectile.scale * scale * width * Easing.SpikeInOutCirc(Progress);
                case 1:
                    return Projectile.scale * width * 2.2f * Easing.SpikeInOutCirc(Progress);

            }
        }

        public Color ColorFunction(float completionRatio)
        {
            switch (DrawMode)
            {
                default:
                case 0:
                    //Front Trail
                    return Color.Lerp(Color.Orange, Color.RoyalBlue, completionRatio);
                case 1:
                    //Back Trail
                    return Color.Transparent;
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            //This damages everything in the trail
            Vector2[] positions = CirclePos;
            float collisionPoint = 0;
            for (int i = 1; i < positions.Length; i++)
            {
                Vector2 position = positions[i];
                Vector2 previousPosition = positions[i - 1];
                if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), position, previousPosition, 6, ref collisionPoint))
                    return true;
            }
            return false;
        }

        public void DrawPixelPrimitives(SpriteBatch spriteBatch)
        {
            BeamDrawer ??= new PrimitiveTrail(WidthFunction, ColorFunction, null, true, TrailRegistry.LaserShader);
            float easedProgess = Easing.OutCubic(Progress);

            //Back Trail   
            DrawMode = 1;
            BeamDrawer.SpecialShader = BackTrailShader;
            BeamDrawer.SpecialShader.UseColor(
                Color.Lerp(BackCircleStartDrawColor, BackCircleEndDrawColor, easedProgess));
            BeamDrawer.SpecialShader.SetShaderTexture(BackTrailTexture);
            BeamDrawer.DrawPixelated(CirclePos, -Main.screenPosition, CirclePos.Length);

            //Front Trail
            DrawMode = 0;
            BeamDrawer.SpecialShader = FrontTrailShader;
            BeamDrawer.SpecialShader.UseColor(Color.Lerp(FrontCircleStartDrawColor, FrontCircleEndDrawColor,
                Easing.OutCirc(Progress)));
            BeamDrawer.SpecialShader.SetShaderTexture(FrontTrailTexture);
            BeamDrawer.DrawPixelated(CirclePos, -Main.screenPosition, CirclePos.Length);
            Main.spriteBatch.ExitShaderRegion();
        }
    }
}
