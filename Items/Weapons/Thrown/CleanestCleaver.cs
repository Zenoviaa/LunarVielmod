using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Bases;
using Stellamod.Core.SwingSystem;
using Stellamod.Dusts;
using Stellamod.Helpers;
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
            .AddCleaverSwordSlash(duration: 36, xSwingRadius: 64, ySwingRadius: 64, swingDegrees: 135, hitCount: 2)
            .AddCleaverSwordSlash(duration: 24, xSwingRadius: 64, ySwingRadius: 64, swingDegrees: 135, hitCount: 2)
            .AddCleaverSwordSlash(duration: 12, xSwingRadius: 64, ySwingRadius: 64, swingDegrees: 135, hitCount: 2)
            .AddCleaverSwordSlash(duration: 12, xSwingRadius: 64, ySwingRadius: 64, swingDegrees: 135, hitCount: 2)
            .AddCleaverThrowLikeGreatsword();
        comboBuilder.AddToProjectile(this);
        useAfterImage = true;
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
        var shader = BlackFireShader.Instance;
        shader.InnerColor = Color.White;
        shader.OuterColor = Color.Silver;
        shader.BackColor = Color.Black;
        TrailDrawer.Draw(Main.spriteBatch, points, Projectile.oldRot, DefaultColorFunction, DefaultWidthFunction, shader);
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
        if (!Hit && ComboIndex == 5)
        {
            FXUtil.ShakeCamera(target.Center, 1024, 4);
            FXUtil.GlowCircleBoom(target.Center,
                innerColor: Color.White,
                glowColor: Color.Black,
                outerGlowColor: Color.Black, duration: 12, baseSize: 0.24f);

            Hit = true;
        }

        for (float i = 0; i < 2; i++)
        {
            float progress = i / 4f;
            float rot = progress * MathHelper.ToRadians(360);
            rot += Main.rand.NextFloat(-0.5f, 0.5f);
            Vector2 offset = rot.ToRotationVector2() * 24;
            var particle = FXUtil.GlowCircleLongBoom(target.Center,
                innerColor: Color.White,
                glowColor: Color.LightGray,
                outerGlowColor: Color.Black,
                baseSize: Main.rand.NextFloat(0.05f, 0.1f),
                duration: Main.rand.NextFloat(5, 10));
            particle.Rotation = rot + MathHelper.ToRadians(45);
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
    }

    public override void SetDefaults()
    {
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.penetrate = 11;
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
            if (Bloody == 3)
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

    }
    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        if (Bloody == 3)
        {
            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/CleanestCleaver4"), Projectile.position);
            Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(Projectile.Center, 1024f, 13);
            SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
            for (int i = 0; i < 5; i++)
            {
                Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 180, Color.DarkGray, Main.rand.NextFloat(1, 1.1f)).noGravity = true;
            }
            for (int i = 0; i < 7; i++)
            {
                Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<BloodDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.OrangeRed, 1f).noGravity = true;
            }
            for (float i = 0; i < 4; i++)
            {
                float progress = i / 4f;
                float rot = progress * MathHelper.ToRadians(360);
                Vector2 offset = rot.ToRotationVector2() * 24;
                var particle = FXUtil.GlowCircleLongBoom(Projectile.Center,
                    innerColor: Color.White,
                    glowColor: Color.LightGray,
                    outerGlowColor: Color.Black, baseSize: 0.09f);
                particle.Rotation = rot + MathHelper.ToRadians(45);
            }
            for (int i = 0; i < 16; i++)
            {
                float p = i / 16f;
                float rot = p * MathHelper.ToRadians(360);
                Vector2 vel = rot.ToRotationVector2() * 6;
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Dirt, vel * 0.2f);
                d = Dust.NewDustPerfect(Projectile.Center, DustID.SilverCoin, vel * 0.4f);
                d.noGravity = true;
            }
        }
        else
        {
            Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(Projectile.Center, 1024f, 6);
            int Sound = Main.rand.Next(1, 3);
            if (Sound == 1)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/CleanestCleaver6"), Projectile.position);
            }
            else
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/CleanestCleaver5"), Projectile.position);
            }
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
        }

        return true;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (!Hit && Bloody <= 2)
        {
            Bloody += 1;
            if (Bloody == 3)
            {

                Projectile.velocity.Y = -10;
                Projectile.velocity.X = -Projectile.velocity.X / 2;
            }
            Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(Projectile.Center, 1024f, 4f);
            for (int i = 0; i < 7; i++)
            {
                SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
                Dust.NewDust(target.position, target.width, target.height, ModContent.DustType<BloodDust>());
            }
            Hit = true;
            ProjectileVelocityBeforeHit = Projectile.velocity;
        }
    }

    public override bool PreDraw(ref Color lightColor)
    {
        SpriteEffects Effects = Projectile.spriteDirection != 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
        SpriteBatch spriteBatch = Main.spriteBatch;
        Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

        // Redraw the projectile with the color not influenced by light
        Vector2 drawOrigin = texture.Size() / 2f;
        for (int k = 0; k < Projectile.oldPos.Length; k++)
        {
            Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + Projectile.Size / 2f + new Vector2(0f, Projectile.gfxOffY);
            Color color = Projectile.GetAlpha(Color.Lerp(new Color(254, 231, 255), new Color(100, 200, 255), 1f / Projectile.oldPos.Length * k) * (1f - 1f / Projectile.oldPos.Length * k));
            color.A = 0;
            spriteBatch.Draw(texture, drawPos, null, color * 0.5f, Projectile.oldRot[k], drawOrigin, Projectile.scale, Effects, 0);
        }

        Vector2 drawPosition = Projectile.Center - Main.screenPosition;
        Color drawColor = Color.White.MultiplyRGB(lightColor);
        spriteBatch.Draw(texture, drawPosition, null, drawColor, Projectile.rotation, drawOrigin, Projectile.scale, Effects, 0);
        return false;
    }
}