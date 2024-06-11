using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Buffs;
using Stellamod.Helpers;
using Stellamod.Trails;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.GothiviaTheSun.REK.Projectiles
{
    internal class RekFireEyeLaserMiniProj : ModProjectile,
        IPixelPrimitiveDrawer
    {
        private ref float Timer => ref Projectile.ai[0];
        public override string Texture => TextureRegistry.EmptyTexture;
        private PrimitiveTrail BeamDrawer;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 16;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.hostile = true;
            Projectile.timeLeft = 180;
            Projectile.light = 0.78f;
        }

        public override void AI()
        {
            Timer++;
            if(Timer == 1)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/RekFireballShoot"), Projectile.position);
            }
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/RekFireballDeath"), Projectile.position);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<GothivianFlames>(), 4 * 20);
        }

        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width;
            return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.Orange, Color.RoyalBlue, completionRatio);
        }

        public void DrawPixelPrimitives(SpriteBatch spriteBatch)
        {
            BeamDrawer ??= new PrimitiveTrail(WidthFunction, ColorFunction, null, true, TrailRegistry.LaserShader);
            BeamDrawer.SpecialShader = TrailRegistry.FireVertexShader;
            BeamDrawer.SpecialShader.UseColor(Color.DarkGoldenrod);
            BeamDrawer.SpecialShader.SetShaderTexture(TrailRegistry.WaterTrail);
            BeamDrawer.DrawPixelated(Projectile.oldPos, -Main.screenPosition, Projectile.oldPos.Length);
            Main.spriteBatch.ExitShaderRegion();
        }
    }
}
