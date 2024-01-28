using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using Stellamod.Trails;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.Sylia.Projectiles
{
    internal class SyliaScissorSmall2 : ModProjectile
    {
        private bool _sync;
        public Vector2 startCenter;
        public Vector2 targetCenter;
        public int delay;
        public bool playedSound;

        //Stats
        private const float Projectile_Speed = 24;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.WriteVector2(startCenter);
            writer.WriteVector2(targetCenter);
            writer.Write(delay);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            startCenter = reader.ReadVector2();
            targetCenter = reader.ReadVector2();
            delay = reader.ReadInt32();
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 62;
            Projectile.height = 54;
            Projectile.tileCollide = false;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.timeLeft = 100;
        }

        public override void AI()
        {
            if(!_sync && Main.myPlayer == Projectile.owner)
            {
                Projectile.Center = startCenter;
                Projectile.netUpdate = true;
                _sync = true;
            }

            delay--;

            Vector2 direction = startCenter.DirectionTo(targetCenter);
            float targetRotation = direction.ToRotation() + MathHelper.ToRadians(45);
            Projectile.rotation = targetRotation;
        
            if (delay <= 0)
            {
                Projectile.velocity = direction * Projectile_Speed;
                if (!playedSound)
                {
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/RipperSlash2"), Projectile.position);
                    playedSound = true;
                }
            }

            Visuals();
        }

        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width;
            return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(new Color(60, 0, 118, 175), Color.Transparent, completionRatio);
        }

        //Visual Stuffs
        public override bool PreDraw(ref Color lightColor)
        {
            DrawHelper.DrawSimpleTrail(Projectile, WidthFunction, ColorFunction, TrailRegistry.VortexTrail);
            DrawHelper.DrawAdditiveAfterImage(Projectile, new Color(60, 0, 118), Color.Black, ref lightColor);
            return base.PreDraw(ref lightColor);
        }

        private void Visuals()
        {
            Lighting.AddLight(Projectile.Center, Color.Pink.ToVector3() * 0.28f);
        }
    }
}
