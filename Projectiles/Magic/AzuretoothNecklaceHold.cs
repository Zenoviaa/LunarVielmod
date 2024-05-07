using Microsoft.Xna.Framework;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Trails;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Magic
{
    internal class AzuretoothNecklaceHold : ModProjectile
    {
        private float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        private Player Owner => Main.player[Projectile.owner];
        private Vector2 PlayerCenter => Owner.RotatedRelativePoint(Owner.MountedCenter, true);
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 16;
            ProjectileID.Sets.TrailingMode[Type] = 2;
            Main.projFrames[Type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = 26;
            Projectile.height = 32;
            Projectile.tileCollide = false;
            Projectile.timeLeft = int.MaxValue;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            Timer++;
            AI_Hold();
            AI_Channel();
            Visuals();
        }

        private void AI_Hold()
        {
            if (Owner.noItems || Owner.CCed || Owner.dead || !Owner.active)
                Projectile.Kill();

            if (Main.myPlayer == Projectile.owner)
            {
                if (!Owner.channel)
                    Projectile.Kill();
            }

            Vector2 holdOffset = new Vector2(0, -64);
            Projectile.Center = Owner.Center + holdOffset.RotatedBy(Timer / 240 * MathHelper.TwoPi);
            Projectile.Center += new Vector2(0, VectorHelper.Osc(-4, 4));
        }

        private void AI_Channel()
        {
            if (Timer % 8 == 0)
            {
                //Get the mana to continue using this
                int manaChannelCost = Owner.HeldItem.mana;
                if (!Owner.CheckMana(manaChannelCost, true))
                {
                    //Not enough mana? Well die
                    Projectile.Kill();
                }
                else
                {
                    //Spawn the projectile
                    Vector2 spawnPosition = PlayerCenter + Main.rand.NextVector2CircularEdge(80, 80);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), spawnPosition, Vector2.Zero,
                        ModContent.ProjectileType<AzuretoothDragon>(), Projectile.damage, Projectile.knockBack, Projectile.owner);

                    for (int i = 0; i < 16; i++)
                    {
                        Vector2 speed = Main.rand.NextVector2CircularEdge(4f, 4f);
                        var d = Dust.NewDustPerfect(spawnPosition, DustID.SpectreStaff, speed, Scale: 4f);
                        d.noGravity = true;
                    }

                    Dust.QuickDustLine(Projectile.Center, spawnPosition, 8, Color.White);
                }
            }

            if (Timer % 16 == 0)
            {
                //Spawn sparkles or something idk
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height,
                    ModContent.DustType<GunFlash>(), newColor: new Color(69, 43, 149), Scale: 0.4f);

                Dust.NewDustPerfect(Projectile.position, ModContent.DustType<GlowDust>(),
                    (new Vector2(0, -1) * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.LightSkyBlue, 0.5f).noGravity = true;
            }

        }

        private void Visuals()
        {
            int frameSpeed = 6;
            DrawHelper.AnimateTopToBottom(Projectile, frameSpeed);

            // Some visuals here
            Lighting.AddLight(Projectile.Center, Color.White.ToVector3() * 0.78f);
        }


        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width * 0.5f;
            return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.White * 0.3f, Color.Transparent, completionRatio);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            //Could also set this manually like
            //frameSize = new Vector2(58, 34);
            DrawHelper.DrawSimpleTrail(Projectile, WidthFunction, ColorFunction, TrailRegistry.SmallWhispyTrail);
            return base.PreDraw(ref lightColor);
        }
    }
}
