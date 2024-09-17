using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Helpers;
using Stellamod.Utilis;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Stellamod.NPCs.Bosses.SupernovaFragment
{
    internal class SupernovaZapwarn : ModProjectile
    {
        private ref float _timer => ref Projectile.ai[0];
        private float _alphaCounter = 0;
        private float _counter = 8;
        private bool _appearing;

        public override string Texture => TextureRegistry.EmptyTexture;
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 1;
            Projectile.damage = 1;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Request<Texture2D>("Stellamod/Effects/Masks/Extra_47").Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            SpriteBatch spriteBatch = Main.spriteBatch;
            for (int i = 0; i < 3; i++)
            {
                spriteBatch.Draw(tex,
                    drawPos, null, new Color(
                        (int)(55f * _alphaCounter),
                        (int)(45f * _alphaCounter),
                        (int)(15f * _alphaCounter), 0), 0, new Vector2(30 / 4, 1028 / 2), 0.2f * (_counter + 0.3f), SpriteEffects.None, 0f);
            }

            return false;
        }

        public override void AI()
        {
            if (!_appearing)
            {
                _alphaCounter += 0.04f;
                if (_alphaCounter >= 4)
                {
                    _appearing = true;
                }
            }
            else
            {
                _timer++;
                if (_timer == 1)
                {
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/StormDragon_LightingZap"), Projectile.position);
                    int Sound = Main.rand.Next(1, 4);
                    if (Sound == 1)
                    {
                        SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Dreadmire__LightingRain1"), Projectile.position);
                    }
                    if (Sound == 2)
                    {
                        SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Dreadmire__LightingRain2"), Projectile.position);
                    }
                    if (Sound == 3)
                    {
                        SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Dreadmire__LightingRain3"), Projectile.position);
                    }

                    Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(Projectile.Center, 2048f, 32f);
                    if (StellaMultiplayer.IsHost)
                    {
                        Vector2 spawnPos = new Vector2(Projectile.Center.X, Projectile.Center.Y - 1500);
                        Vector2 spawnVelocity = new Vector2(0, 10);
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), spawnPos, spawnVelocity,
                            ModContent.ProjectileType<SupernovaBeam>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    }
                }
                if (_timer >= 10)
                {
                    if (_alphaCounter <= 0)
                    {
                        Projectile.Kill();
                    }
                    _alphaCounter -= 0.29f;
                }
            }
        }
    }
}

