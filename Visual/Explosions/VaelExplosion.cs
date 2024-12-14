using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Common.Bases;
using Stellamod.Common.Shaders.MagicTrails;
using Stellamod.Helpers;
using System.Collections.Generic;
using Terraria;

namespace Stellamod.Visual.Explosions
{
    public class VaelExplosion : BaseExplosionProjectile
    {
        private LightningTrail _lightningTrail;
        private float _timer;
        int rStart = 4;
        int rEnd = 128;
        public override void SetDefaults()
        {
            base.SetDefaults();
            _lightningTrail = new LightningTrail();
            Projectile.width = 48;
            Projectile.height = 48;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.timeLeft = 32;
            rStart = Main.rand.Next(4, 8) * 12;
            rEnd = Main.rand.Next(132, 180);
            Projectile.hide = true;
        }

        public override void AI()
        {
            base.AI();
            _timer++;
            if (_timer % 6 == 0)
            {
                _lightningTrail.RandomPositions(_circlePos);
            }
        }

        protected override float BeamWidthFunction(float p)
        {
            //How wide the trail is going to be
            float trailWidth = MathHelper.Lerp(96, 0, Easing.OutCubic(p));
            float fadeWidth = MathHelper.Lerp(0, trailWidth, Easing.SpikeOutExpo(p));// * Main.rand.NextFloat(0.75f, 1.0f);
            return fadeWidth;
        }

        protected override Color ColorFunction(float p)
        {
            Color trailColor = Color.Lerp(Color.Orange, Color.Purple, p);
            Color fadeColor = Color.Lerp(trailColor, Color.DarkViolet, UneasedProgress);
            return trailColor;
        }

        protected override float RadiusFunction(float p)
        {
            //How large the circle is going to be
            return MathHelper.Lerp(rStart, rEnd, Easing.OutExpo(p));
        }

        private void DrawTrail()
        {
            //Trail
            SpriteBatch spriteBatch = Main.spriteBatch;
            var shader = MagicVaellusShader.Instance;

            //Resets to the default settings for this shader
            shader.SetDefaults();

            _lightningTrail ??= new();
            //Making this number big made like the field wide
            _lightningTrail.LightningRandomOffsetRange = 8;

            //This number makes it more lightning like, lower this is the straighter it is
            _lightningTrail.LightningRandomExpand = 4;
            _lightningTrail.Draw(spriteBatch, _circlePos, Projectile.oldRot, ColorFunctionReal, WidthFunction, shader, offset: Projectile.Size / 2f);

            shader.BlendState = BlendState.Additive;
            _lightningTrail.Draw(spriteBatch, _circlePos, Projectile.oldRot, ColorFunctionReal, WidthFunction, shader, offset: Projectile.Size / 2f);
        }


        public override bool PreDraw(ref Color lightColor)
        {
            DrawTrail();
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            base.DrawBehind(index, behindNPCsAndTiles, behindNPCs, behindProjectiles, overPlayers, overWiresUI);
            overPlayers.Add(index);
        }
    }
}
