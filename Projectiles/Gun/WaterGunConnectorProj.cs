using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Common.Shaders.MagicTrails;
using Stellamod.Common.Shaders;
using Stellamod.Helpers;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Stellamod.Trails;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Stellamod.Dusts;
using System;


namespace Stellamod.Projectiles.Gun
{
    internal class WaterGunConnectorProj : ModProjectile,
         IPixelPrimitiveDrawer
    {
        private Vector2[] TrailPoints = new Vector2[1];
        private List<Projectile> Projectiles = new List<Projectile>();
        private List<Vector2> Connector = new List<Vector2>();
        private List<Vector2> Target = new List<Vector2>();
        private int trailingMode = 0;
        private int Smooth;
        private ref float Timer => ref Projectile.ai[0];
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.timeLeft = int.MaxValue;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 9;
        }

        public override void AI()
        {
            AI_Channel();
            AI_FillPoints();
        }

        private void AI_Channel()
        {
            //Channeling
            bool isReal = false;
            Player player = Main.player[Projectile.owner];
            Projectile.Center = player.Center;
            foreach (var proj in Main.ActiveProjectiles)
            {
                if (proj.type == ModContent.ProjectileType<WaterGunNodeProj>() && proj.owner == Projectile.owner)
                {
                    isReal = true;
                }
            }

            Timer++;
            if(Timer % 6 == 0)
            {
                Smooth = Main.rand.Next(65, 155);
                for(int i =0;i < TrailPoints.Length; i++)
                {
                    Vector2 trailPoint = TrailPoints[i];
                    if (Main.rand.NextBool(300))
                    {
                        Dust.NewDustPerfect(trailPoint, ModContent.DustType<GlyphDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.Aqua, 2f).noGravity = true;
                    }
                }
            }
            if (!isReal)
            {
                Projectile.Kill();
            }
        }

        private void AI_FillPoints()
        {
            //Get the points to connect
            Connector.Clear();
            Projectiles.Clear();
            int nodeType = ModContent.ProjectileType<WaterGunNodeProj>();
            foreach (var proj in Main.ActiveProjectiles)
            {
                if (proj.owner != Projectile.owner)
                    continue;
                if (proj.type != nodeType)
                    continue;
                Projectiles.Add(proj);
            }

            Projectiles.Sort((x, y) => y.timeLeft.CompareTo(x.timeLeft));
            for (int i = 1; i < Projectiles.Count; i++)
            {
                for(float j = 0; j < 8f; j++)
                {
                    Connector.Add(Vector2.Lerp(Projectiles[i - 1].Center, Projectiles[i].Center, j / 8f));
                }
           
            }


            TrailPoints = Connector.ToArray();
            for (int i = 1; i < TrailPoints.Length - 1; i++)
            {
                float p = (float)i / (float)TrailPoints.Length - 1;
                ref Vector2 pos = ref TrailPoints[i];
                ref Vector2 nextPos = ref TrailPoints[i + 1];
                Vector2 vec = (nextPos - pos);
                vec = vec.RotatedBy(MathHelper.ToRadians(90));
                vec *= p;

                pos += vec * MathF.Sin(Main.GlobalTimeWrappedHourly * -12 + p * 24);
                pos += vec * MathF.Sin((Main.GlobalTimeWrappedHourly + 4) * -12 + p * 12);

            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            //This damages everything in the trail
            Vector2[] positions = TrailPoints;
            float collisionPoint = 0;
            for (int i = 1; i < positions.Length; i++)
            {
                Vector2 position = positions[i];
                Vector2 previousPosition = positions[i - 1];
                if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), position, previousPosition, 6, ref collisionPoint))
                    return true;
            }
            return base.Colliding(projHitbox, targetHitbox);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }

        public PrimDrawer TrailDrawer2 { get; private set; } = null;
        public float WidthFunction2(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width;
            if (Timer % 6 == 0)
            {
                baseWidth *= 1.2f;
            }
            return MathHelper.SmoothStep(baseWidth, baseWidth, completionRatio) * 0.8f;
        }

        public Color ColorFunction2(float completionRatio)
        {
            Color color = Color.CadetBlue;
            if(Timer % 6 == 0)
            {
                color = Color.LightGoldenrodYellow;
            }
            return Color.Lerp(color, color, completionRatio);
        }

        public void DrawPixelPrimitives(SpriteBatch spriteBatch)
        {
            TrailDrawer2 ??= new PrimDrawer(WidthFunction2, ColorFunction2, GameShaders.Misc["VampKnives:SuperSimpleTrail"]);
            GameShaders.Misc["VampKnives:SuperSimpleTrail"].SetShaderTexture(TrailRegistry.BeamTrail);
            TrailDrawer2.DrawPixelPrims(TrailPoints, -Main.screenPosition, Smooth);
            Main.spriteBatch.ExitShaderRegion();
        }
    }
}
