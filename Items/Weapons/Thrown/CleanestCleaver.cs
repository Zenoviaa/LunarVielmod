using Stellamod.Assets;
using Stellamod.Common.Particles;
using Stellamod.Common.Shaders;
using Stellamod.Content.CommonMaterials;
using Stellamod.Content.Dusts;
using Stellamod.Content.Gores;
using Stellamod.Core.Bases;
using Stellamod.Core.Effects.Trails;
using Stellamod.Core.SwingSystem;
using Stellamod.Helpers;
using Stellamod.Trailing;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Thrown;

public class CleanestCleaver : BaseSwingItemV2
{
    public override void SetDefaults2()
    {
        base.SetDefaults2();
        Item.damage = 16;
        Item.DamageType = DamageClass.Melee;
        Item.shootSpeed = 10;
        Item.shoot = ModContent.ProjectileType<CleanestCleaverSlash>();
        Item.autoReuse = true;
        staminaProjectileShoot = ModContent.ProjectileType<CleanestCleaverProg>();
        meleeWeaponType = Core.Bases.MeleeWeaponType.Sword;
        staminaDamageMultiplier = 2;
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew(
            mold: ModContent.ItemType<BlankSword>(),
            material: ModContent.ItemType<GintzlMetal>());
    }
}
public class CleanestCleaverSlash : BaseSwingProjectileV2
{
    public bool Hit;
    public override void SetDefaults2()
    {
        base.SetDefaults2();
    }

    public override void DefineCombo()
    {
        base.DefineCombo();
        ComboBuilder comboBuilder = new ComboBuilder();
        comboBuilder
            .AddCleaverSwordSlash(duration: 36, xSwingRadius: 32, ySwingRadius: 64, swingDegrees: 90, hitCount: 2)
            .AddCleaverSwordSlash(duration: 24, xSwingRadius: 32, ySwingRadius: 64, swingDegrees: 90, hitCount: 2)
            .AddCleaverSwordSlash(duration: 12, xSwingRadius: 32, ySwingRadius: 64, swingDegrees: 90, hitCount: 2)
            .AddCleaverSwordSlash(duration: 12, xSwingRadius: 32, ySwingRadius: 64, swingDegrees: 90, hitCount: 2)
            .AddCleaverThrowLikeGreatsword();
        comboBuilder.AddToProjectile(this);

        SlashTrailBuilder slashTrailBuilder = new SlashTrailBuilder();
        SlashTrailer slashTrailer = slashTrailBuilder.Instantiate();
        slashTrailer.invert = ComboIndex % 2 != 0;
        Trailer = slashTrailer;

        hitStopTime = EXTRA_UPDATE_COUNT * 8;
        useBloom = true;
        bloom.innerBloomColor = Color.White;
        bloom.outerBloomColor = Color.DarkGray;
        bloom.bloomWidthFunction = GetBloomWidth;
        bloom.bloomColorFunction = GetBloomColor;
        useAfterImage = true;
    }
    private float GetBloomWidth(float ratio)
    {
        return MathHelper.SmoothStep(4, 32, ratio) * 1.15f * MathHelper.Lerp(1f, 0f, EasingFunction.InExpo(Interpolant));
    }

    private Color GetBloomColor(float ratio)
    {
        Color blue = Color.Lerp(Color.Lerp(Color.White, Color.DarkGray, 0.5f), Color.DarkGray, ExtraMath.Osc(0f, 1f, speed: 4));
        return Color.Lerp(blue * 0.9f, Color.Black, ratio);
    }


    private float DefaultWidthFunction(float completionRatio)
    {
        float width = IsFinishingSwing() ? 0.25f : 1f;
        return MathHelper.Lerp(0, 64, completionRatio) * width;
    }

    private Color DefaultColorFunction(float p)
    {
        Color trailColor = Color.Lerp(Color.White, Color.LightBlue, p) * EasingFunction.QuadraticBump(p);
        return trailColor;
    }

    public override void RenderSwingTrail(ref Color lightColor, Vector2[] points)
    {
        //       base.RenderSwingTrail(ref lightColor, points);

    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
        if (!Hit)
        {

            var hitSound = AssetReferences.Assets.Sounds.Melee.SwordHit1.Asset with { PitchVariance = 0.35f, Volume =0.65F, Pitch = -0.5f };
            SoundEngine.PlaySound(hitSound, target.position);
        }
        if (!Hit && ComboIndex == 5)
        {
            FXUtil.ShakeCamera(target.Center, 1024, 4);
            FXUtil.GlowCircleBoom(target.Center,
                innerColor: Color.White,
                glowColor: Color.Black,
                outerGlowColor: Color.Black, duration: 12, baseSize: 0.24f);

     
        }
        Hit = true;

        for (var i = 0; i < 2; i++)
        {
            Particles.BitDust.Spawn(BitDustFactory.SlowingOverTime with
            {
                position = target.Center + Main.rand.NextVector2Circular(32, 32),
                velocity = Projectile.velocity.RotatedByRandom(0.5f).SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(5, 15),
                innerColor = Color.White.ToVector4(),
                outerColor = Color.White.ToVector4(),
                scale = new Vector2(Main.rand.NextFloat(0.4f, 1f)),
                velocityPerTickMult = 0.93f,
                timeLeft = 30
            });
        }

    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        base.ModifyHitNPC(target, ref modifiers);
        SoundStyle spearHit = AssetRegistry.Sounds.Melee.SpearHit1;
        spearHit.PitchVariance = 0.5f;
        SoundEngine.PlaySound(spearHit, Projectile.position);
        if (ComboIndex == 5)
        {
            modifiers.FinalDamage *= 2;
        }
    }
}

public class CleanestCleaverProg : ModProjectile
{
    public bool Sound;
    public int Bloody;
    public Vector2 ProjectileVelocityBeforeHit;
    public bool Hit;
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        Main.projFrames[Type] = 2;
    }

    public override void SetDefaults()
    {
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.penetrate = -1;
        Projectile.friendly = true;
        Projectile.timeLeft = 540;
    }

    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.WriteVector2(ProjectileVelocityBeforeHit);
    }

    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        ProjectileVelocityBeforeHit = reader.ReadVector2();
    }

    public override void AI()
    {
        if (Hit)
        {
            Projectile.ai[0]++;
            if (Projectile.ai[0] <= 7)
            {
                if (!Sound)
                {
                    if (Bloody == 3)
                    {
                        SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/CleanestCleaver3"), Projectile.position);
                    }
                    else
                    {
                        int Sound = Main.rand.Next(1, 3);
                        if (Sound == 1)
                        {
                            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/CleanestCleaver1"), Projectile.position);
                        }
                        else
                        {
                            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/CleanestCleaver2"), Projectile.position);
                        }
                    }
                    Sound = true;
                }


                Projectile.friendly = false;
                Projectile.velocity = Vector2.Zero;
            }
            else
            {

                Projectile.friendly = true;
                Hit = false;
                Projectile.velocity = ProjectileVelocityBeforeHit;
            }
        }
        else
        {
            Sound = false;
            if (Bloody == 5)
            {
                Projectile.velocity.Y += 0.3f;
                Projectile.velocity.X *= 0.96f;
                Projectile.ai[0] = 0;

            }
            else
            {
                Projectile.ai[0] = 0;
            }

            if (Projectile.velocity.X >= 0)
            {
                Projectile.rotation += 0.35f;
            }
            else
            {
                Projectile.rotation -= 0.35f;
            }
        }
    }
    public override void PostDraw(Color lightColor)
    {
        /*
        string B3Texture = Texture + "_Blood3";
        string B2Texture = Texture + "_Blood2";
        string B1Texture = Texture + "_Blood1";
        Texture2D Blood1Texture = ModContent.Request<Texture2D>(B1Texture).Value;
        Texture2D Blood2Texture = ModContent.Request<Texture2D>(B2Texture).Value;
        Texture2D Blood3Texture = ModContent.Request<Texture2D>(B3Texture).Value;

        Vector2 drawOrigin = Blood1Texture.Size() / 2f;

        //Lerping
        if (Bloody == 1)
        {
            Color drawColor = Color.Lerp(Color.Transparent, Color.White, 1);
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Main.spriteBatch.Draw(Blood1Texture, drawPosition, null, drawColor, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0f);
        }
        if (Bloody == 2)
        {
            Color drawColor = Color.Lerp(Color.Transparent, Color.White, 1);
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Main.spriteBatch.Draw(Blood2Texture, drawPosition, null, drawColor, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0f);
        }
        if (Bloody == 3)
        {
            Color drawColor = Color.Lerp(Color.Transparent, Color.White, 1);
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Main.spriteBatch.Draw(Blood3Texture, drawPosition, null, drawColor, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0f);
        }
        */
    }
    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        DeathEffect();
        return true;
    }

    void DeathEffect()
    {
        Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(Projectile.Center, 1024f, 6);
        SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/CleanestCleaver4"), Projectile.position);
        for (int i = 0; i < 20; i++)
        {
            Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 180, Color.Gray, Main.rand.NextFloat(0.5f, 1f)).noGravity = true;
        }
        for (float i = 0; i < 4; i++)
        {
            float progress = i / 4f;
            float rot = progress * MathHelper.ToRadians(360);
            Vector2 offset = rot.ToRotationVector2() * 24;
            var particle = FXUtil.GlowCircleLongBoom(Projectile.Center,
                innerColor: Color.White,
                glowColor: Color.LightGray,
                outerGlowColor: Color.Black, baseSize: 0.07f);
            particle.Rotation = rot + MathHelper.ToRadians(45);
        }
        for (var i = 0; i < 32; i++)
        {
            Particles.SwirlingFlameDust.Spawn(BitDustFactory.SlowingOverTime with
            {
                position = Projectile.Center + Main.rand.NextVector2Circular(32, 32),
                velocity = Projectile.velocity.RotatedByRandom(0.5f).SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(5, 25),
                innerColor = Color.White.ToVector4(),
                outerColor = Color.White.ToVector4(),
                scale = new Vector2(Main.rand.NextFloat(0.4f, 1f)),
                velocityPerTickMult = 0.93f,
                timeLeft = 60
            });
        }
    }
    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (!Hit && Bloody <= 4)
        {
            Bloody += 1;
            if (Bloody == 5)
            {

                Projectile.velocity.Y = -10;
                Projectile.velocity.X = -Projectile.velocity.X / 2;
            }
            Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(Projectile.Center, 1024f, 4f);

            Hit = true;
            ProjectileVelocityBeforeHit = Projectile.velocity;
        }
        for(var i =0; i < 32; i++)
        {
            var pos = target.Center + Main.rand.NextVector2Circular(32, 32);
            var velocity = Projectile.velocity.RotatedByRandom(0.5f).SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(5, 17);
            Dust.NewDustPerfect(pos, DustID.Blood, velocity, Scale: 1.5f);
        }

        for (var i = 0; i < 6; i++)
        {
            Particles.BitDust.Spawn(BitDustFactory.SlowingOverTime with
            {
                position = target.Center + Main.rand.NextVector2Circular(32, 32),
                velocity = Projectile.velocity.RotatedByRandom(0.5f).SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(5, 25),
                innerColor = Color.White.ToVector4(),
                outerColor = Color.White.ToVector4(),
                scale = new Vector2(Main.rand.NextFloat(0.4f, 1f)),
                velocityPerTickMult = 0.93f,
                timeLeft = 30
            });
        }
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Main.projFrames[Type] = 2;
        DrawUtilities.DrawSpriteAfterImage(Main.spriteBatch, Projectile, Color.White, Color.Transparent, 0.3f);
        var drawer = Projectile.Drawer;
        Main.spriteBatch.Draw(drawer);
        drawer.VerticalFrame(1, 2);
        drawer.color = Color.Green * ExtraMath.Osc(0.75f, 1f, speed: 6);
        drawer.color.A = 0;
        Main.spriteBatch.Draw(drawer);
        return false;
    }
}