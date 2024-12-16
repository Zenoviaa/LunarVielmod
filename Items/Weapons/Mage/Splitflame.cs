using Microsoft.Xna.Framework;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Mage
{
    internal class Splitflame : ClassSwapItem
    {
        public override DamageClass AlternateClass => DamageClass.Throwing;

        public override void SetClassSwappedDefaults()
        {
            Item.damage = 25;
            Item.mana = 0;
        }

        public override void SetDefaults()
        {
            Item.damage = 50;
            Item.mana = 5;
            Item.width = 18;
            Item.height = 21;
            Item.useTime = 40;
            Item.useAnimation = 40;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.noMelee = true;
            Item.knockBack = 4f;
            Item.DamageType = DamageClass.Magic;
            Item.value = 10000;
            Item.rare = ItemRarityID.Orange;
            Item.UseSound = SoundID.DD2_BookStaffCast;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<SplitFlameBlast>();
            Item.shootSpeed = 4f;
            Item.autoReuse = true;
            Item.crit = 7;
        }
    }

    public class SplitFlameBlast : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        public override string Texture => TextureRegistry.EmptyTexture;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.tileCollide = true;
            Projectile.penetrate = 1;
            Projectile.friendly = true;
            Projectile.timeLeft = 140;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if (Timer % 5 == 0)
            {
                Color glyphColor = Color.Red;
                switch (Main.rand.Next(3))
                {
                    case 0:
                        glyphColor = Color.Red;
                        break;
                    case 1:
                        glyphColor = Color.OrangeRed;
                        break;
                    case 2:
                        glyphColor = Color.Yellow;
                        break;
                }
                if (Main.rand.NextBool(3))
                    Dust.NewDustPerfect(Projectile.Center, DustID.Torch, Projectile.velocity * 0.1f, 0, glyphColor, 1f).noGravity = true;
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlyphDust>(), Projectile.velocity * 0.1f, 0, glyphColor, 1f).noGravity = true;
            }
            if (Timer < 10)
            {
                Projectile.velocity -= Projectile.velocity.SafeNormalize(Vector2.Zero);
                Projectile.velocity *= 0.99f;
            }

            if (Timer == 20 && Main.myPlayer == Projectile.owner)
            {
                Projectile.velocity = ProjectileHelper.SimpleHomingVelocity(Projectile, Main.MouseWorld, 360);
                Projectile.netUpdate = true;
            }

            if (Timer > 20)
            {
                if (Projectile.velocity.Length() < 15)
                    Projectile.velocity *= 1.5f;
            }
            Lighting.AddLight(Projectile.Center, Color.OrangeRed.ToVector3() * 1.75f * Main.essScale);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            if (Main.rand.NextBool(3))
                target.AddBuff(BuffID.OnFire, 180);
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                ModContent.ProjectileType<SplitflameBoom>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
        }
    }

    public class SplitflameBoom : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        public override string Texture => TextureRegistry.EmptyTexture;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 80;
            Projectile.height = 80;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.friendly = true;
        }
        public override void AI()
        {
            base.AI();
            Timer++;
            if (Timer == 1)
            {
                FXUtil.ShakeCamera(Projectile.Center, 1024, 8);
                SoundEngine.PlaySound(SoundRegistry.CombusterBoom, Projectile.position);
                for (float f = 0; f < 30; f++)
                {
                    Color glyphColor = Color.Red;
                    switch (Main.rand.Next(3))
                    {
                        case 0:
                            glyphColor = Color.Red;
                            break;
                        case 1:
                            glyphColor = Color.OrangeRed;
                            break;
                        case 2:
                            glyphColor = Color.Yellow;
                            break;
                    }
                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlyphDust>(),
                        (Vector2.One * Main.rand.NextFloat(0.2f, 5f)).RotatedByRandom(19.0), 0, glyphColor, Main.rand.NextFloat(1f, 3f)).noGravity = true;
                }

                SoundStyle morrowExp = new SoundStyle($"Stellamod/Assets/Sounds/MorrowExp");
                morrowExp.PitchVariance = 0.3f;
                SoundEngine.PlaySound(morrowExp, Projectile.position);
                for (float i = 0; i < 4; i++)
                {
                    float progress = i / 4f;
                    float rot = progress * MathHelper.ToRadians(360);
                    Vector2 offset = rot.ToRotationVector2() * 24;
                    var particle = FXUtil.GlowCircleDetailedBoom1(Projectile.Center,
                        innerColor: Color.White,
                        glowColor: Color.Yellow,
                        outerGlowColor: Color.Red,
                        baseSize: 0.2f);
                    particle.Rotation = rot + MathHelper.ToRadians(45);
                }

                for (int i = 0; i < 8; i++)
                {
                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.DarkGray, 1f).noGravity = true;
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            if (Main.rand.NextBool(3))
                target.AddBuff(BuffID.OnFire, 180);
        }
    }
}