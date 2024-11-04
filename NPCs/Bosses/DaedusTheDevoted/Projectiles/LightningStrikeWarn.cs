using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Stellamod.Trails;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.DaedusTheDevoted.Projectiles
{
    internal class LightningStrikeWarn : ModProjectile
    {
        public Vector2[] BeamPoints;
        public override string Texture => TextureRegistry.EmptyTexture;
        private ref float Timer => ref Projectile.ai[0];
        private Player TargetPlayer
        {
            get
            {
                return Main.player[(int)Projectile.ai[1]];
            }
        }
        public CommonLightning Lightning { get; set; } = new CommonLightning();
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.light = 0.3f;
            Projectile.timeLeft = 210;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override void AI()
        {
            base.AI();

            Timer++;
            if (Timer % 12 == 0)
            {
                Vector2 vel = Vector2.Zero;
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.GoldCoin, vel, Scale: 1);
                d.noGravity = true;
            }

            if (Timer % 6 == 0)
            {
                Vector2 vel = Vector2.Zero;
                Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(8, 8), DustID.GoldCoin, vel, Scale: 1);
                d.noGravity = true;
            }

            Projectile.Center = TargetPlayer.Center - new Vector2(0, 196);
            //Should make it scale in and out
            //We'll manually sync the projectile in daedus's AI
            Lightning.WidthMultiplier = Easing.SpikeOutCirc(Timer / 200f);
            if (Timer % 3 == 0)
            {
                List<Vector2> beamPoints = new List<Vector2>();
                for (int i = 0; i <= 8; i++)
                {
                    beamPoints.Add(Vector2.Lerp(Projectile.Center, Projectile.Center + Projectile.velocity, i / 8f));
                }
                BeamPoints = beamPoints.ToArray();
                Lightning.RandomPositions(BeamPoints);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            Lightning.Draw(spriteBatch, BeamPoints, Projectile.oldRot);
            return false;
        }
    }
}
