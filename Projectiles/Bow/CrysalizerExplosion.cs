
using Microsoft.Xna.Framework;
using Stellamod.Effects;
using Stellamod.Trails;
using Stellamod.Utilis;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Bow
{
    public class CrysalizerExplosion : ModProjectile
    {
        public float MaxRadius = 100;

        public PrimitiveTrailCopy FireDrawer;

        public ref float Time => ref Projectile.ai[0];

        public ref float Radius => ref Projectile.ai[1];




        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 8;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 84;
            Projectile.MaxUpdates = 2;
            Projectile.scale = 1f;
            CooldownSlot = ImmunityCooldownID.Bosses;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.MaxUpdates);
            writer.Write(MaxRadius);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.MaxUpdates = reader.ReadInt32();
            MaxRadius = reader.ReadSingle();
        }

        public override void AI()
        {
            Projectile.scale += 0.08f;
            Radius = MathHelper.Lerp(Radius, MaxRadius, 0.1f);
            Projectile.Opacity = Utils.GetLerpValue(8f, 42f, Projectile.timeLeft, true) * 0.55f;

            Time++;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => Utilities.CircularCollision(targetHitbox.Center.ToVector2(), projHitbox, Radius * 0.725f) && Time <= 30f;

        public override bool? CanDamage() => Projectile.Opacity >= 0.37f;

        public float SunWidthFunction(float completionRatio) => Radius * MathF.Sin(MathHelper.Pi * completionRatio);

        public Color SunColorFunction(float completionRatio)
        {

            return Color.Lerp(Color.Fuchsia, Color.DarkTurquoise, MathF.Sin(MathHelper.Pi * completionRatio) * 0.5f + 0.3f) * Projectile.Opacity;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            FireDrawer ??= new PrimitiveTrailCopy(SunWidthFunction, SunColorFunction, null, true, TrailRegistry.FireVertexShader);

            TrailRegistry.FireVertexShader.UseSaturation(0.45f);
            TrailRegistry.FireVertexShader.SetShaderTexture(TrailRegistry.CrystalNoise);

            List<float> rotationPoints = new();
            List<Vector2> drawPoints = new();

            for (float offsetAngle = -MathHelper.PiOver2; offsetAngle <= MathHelper.PiOver2; offsetAngle += MathHelper.Pi / 3f)
            {
                rotationPoints.Clear();
                drawPoints.Clear();

                float adjustedAngle = offsetAngle + MathHelper.Pi * -0.2f;
                Vector2 offsetDirection = adjustedAngle.ToRotationVector2();
                for (int i = 0; i < 16; i++)
                {
                    rotationPoints.Add(adjustedAngle);
                    drawPoints.Add(Vector2.Lerp(Projectile.Center - offsetDirection * Radius / 2f, Projectile.Center + offsetDirection * Radius / 2f, i / 16f));
                }

                FireDrawer.Draw(drawPoints, -Main.screenPosition, 24);
            }
            return false;
        }
    }
}
