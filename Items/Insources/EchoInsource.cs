using Microsoft.Xna.Framework;
using Stellamod.Common.XixianFlaskSystem;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Particles;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Items.Insources
{
    public class EchoInsource : InsourceItem
    {
        public override int GetAddedTime()
        {
            return 60 * 10;
        }

        public override void UseInsource(FlaskPlayer flaskPlayer)
        {
            base.UseInsource(flaskPlayer);
            Player player = flaskPlayer.Player;
            if (player.whoAmI == Main.myPlayer)
            {
                Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, Vector2.Zero, ModContent.ProjectileType<EchoBoom>(), 300, 1, player.whoAmI);
            }
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew<ConvulgingMater, BlankBrooch>();
        }
    }

    public class EchoBoom : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        public override string Texture => TextureRegistry.EmptyTexture;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 256;
            Projectile.height = 256;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 15;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if (Timer == 1)
            {
                float numDust = 12;
                for (float n = 0; n < numDust; n++)
                {
                    Vector2 vel = Vector2.UnitY.RotatedByRandom(4f);
                    vel *= Main.rand.NextFloat(0.5f, 2.5f);
                    DustParticleSpawnParams spawnParams = new DustParticleSpawnParams();
                    spawnParams.innerColor = Color.LightSkyBlue;
                    spawnParams.outerColor = Color.DarkBlue;
                    DustParticle.Spawn(Projectile.Center, vel, spawnParams);
                }

                numDust = 4;
                for (float n = 0; n < numDust; n++)
                {
                    Vector2 vel = Vector2.UnitY.RotatedByRandom(0.5f);
                    vel *= Main.rand.NextFloat(0.5f, 2.5f);
                    SmokeParticle sp = Particle<SmokeParticle>.Spawn(Projectile.Center, -Vector2.UnitY, Scale: Main.rand.NextFloat(1f, 2f));
                    sp.initialColor = Color.DarkBlue;
                }


                numDust = 8;
                for (float n = 0; n < numDust; n++)
                {
                    Vector2 vel = Vector2.UnitY.RotatedByRandom(4f);
                    vel *= Main.rand.NextFloat(0.5f, 2.5f) * 24;
                    vel *= 2f;
                    FXUtil.GlowStretch(Projectile.Center, vel);
                }


                var part2 = FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.LightBlue, Color.Purple);
                part2.Scale *= 2;

                SoundStyle boomSound = new SoundStyle("Stellamod/Assets/Sounds/StormDragon_Bomb");
                boomSound.PitchVariance = 0.5f;
                SoundEngine.PlaySound(boomSound, Projectile.position);
                FXUtil.ShakeCamera(Projectile.Center, 1024, 4);
            }
        }
    }
}
