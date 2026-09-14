using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.Particles;
using Stellamod.Common.Shaders;
using Stellamod.Content.Areas.Tundra.MoonspiralTower.AccMT;
using Stellamod.Content.CommonMaterials;
using Stellamod.Content.Dusts;
using Stellamod.Content.Rendering.Abyssal;
using Stellamod.Content.Rendering.MoonMagic;
using Stellamod.Core;
using Stellamod.Core.Bases;
using Stellamod.Core.Pixelation;
using Stellamod.Items;
using Stellamod.Visual.Particles;
using System.Diagnostics;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Stellamod.Content.Areas.Tundra.Abyss.WeaponsAB;

public class Amalgamating : ModBuff
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.debuff[Type] = true;
    }

    public override void Update(NPC npc, ref int buffIndex)
    {
        base.Update(npc, ref buffIndex);
        npc.lifeRegen -= 24;
        if (Main.rand.NextBool(3))
        {
            var dp = Dust.NewDust(npc.position, npc.width, npc.height, ModContent.DustType<GlowDust>(), newColor: Color.Cyan, Scale: 0.2f);
        }
    }
}

public class AmalgamAura : ModProjectile
{
    private ref float Timer => ref Projectile.ai[0];
    public override string Texture => TextureRegistry.EmptyTexture;
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 32;
        Projectile.height = 32;
        Projectile.penetrate = -1;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.tileCollide = false;
        Projectile.timeLeft = 90;
    }

    public override void AI()
    {
        base.AI();
        Timer++;

        float amalgamRange = 256 * 256;
        foreach (var npc in Main.ActiveNPCs)
        {
            float dq = Vector2.DistanceSquared(Projectile.Center, npc.Center);
            if (dq < amalgamRange)
            {
                npc.AddBuff(ModContent.BuffType<Amalgamating>(), 120);
            }
        }
    }

    public override bool PreDraw(ref Color lightColor)
    {
        PixelationManager.QueueSpritebatchDrawAction(DrawPixelSprites);
        return false;
    }

    private void DrawPixelSprites(SpriteBatch spriteBatch, Vector2 screenPos)
    {
        Asset<Texture2D> noise = AssetManager.GlowMask.MagicCircleVampiricVine;
        Vector2 drawOrigin = noise.Size() / 2f;
        Texture2D texture = noise.Value;

        Vector2 drawCenter = Projectile.Center - Main.screenPosition;

        float ease = EasingFunction.InOutSine(Projectile.timeLeft / 10f) * EasingFunction.InOutSine(Timer / 10f);
        Color drawColor = Color.White * ease;
        drawColor.A = 0;

        Color drawColor2 = Color.SkyBlue * ease;
        drawColor2.A = 0;

        Vector2 scale = Vector2.One;
        scale *= 4;
        var shader = CelestialAuraShader.Instance;
        shader.InnerColor = Color.SkyBlue * 0.4f;
        shader.OuterColor = Color.Black;
        shader.Time = -Timer * 0.05f + 1;
        shader.Tiling = Vector2.One * 0.1f;
        using (new SpritebatchContext(spriteBatch, spriteBatch.Parameters with { effect = shader }))
        {
            for (float f = 0; f < 4; f++)
            {
                Color glowColor = Color.Lerp(drawColor, drawColor2, (f + 1) / 3f);
                glowColor.A = 0;
                float rotOffset = (f / 4f) * MathHelper.TwoPi;
                spriteBatch.Draw(texture, drawCenter, null, glowColor, rotOffset + 0.5f, drawOrigin,
                    new Vector2(0.8f, 1f) * 0.25f * 0.75f * scale, SpriteEffects.None, 0);
                spriteBatch.Draw(texture, drawCenter, null, glowColor, rotOffset, drawOrigin,
                    new Vector2(0.8f, 1f) * 0.25f * scale, SpriteEffects.None, 0);
            }
        }
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}

public class AmalgamTrailingProjectile : ModProjectile
{
    private int Parent
    {
        get => (int)Projectile.ai[0];
        set => Projectile.ai[0] = value;
    }
    private ref float Timer => ref Projectile.ai[1];
    public override string Texture => TextureRegistry.EmptyTexture;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ProjectileID.Sets.TrailCacheLength[Type] = 24;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.tileCollide = false;
        Projectile.width = 24;
        Projectile.height = 24;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 600;
    }

    public override bool ShouldUpdatePosition()
    {
        return false;
    }

    public override void AI()
    {
        base.AI();
        Timer++;

        Projectile parent = null;
        foreach (var proj in Main.ActiveProjectiles)
        {
            if (proj.identity == Parent)
            {
                parent = proj;
            }
        }

        if (parent == null || !parent.active)
            Projectile.active = false;
        else
        {
            Projectile.Center = parent.Center;
        }
    }

    public float WidthFunction(float completionRatio)
    {
        float baseWidth = 32;
        return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
    }

    public Color ColorFunction(float completionRatio)
    {
        return Color.Lerp(Main.DiscoColor * 0.3f, Color.Transparent, completionRatio);
    }
    private Color GetColorFunction(float completionRatio)
    {
        Color inColor = Color.White;
        Color trailColor = Color.Lerp(Color.SpringGreen, Color.DarkBlue, completionRatio);


        Color rainbow = Color.Red;
        float degrees = completionRatio * 360f;
        degrees += Main.GlobalTimeWrappedHourly * 400;
        degrees %= 360;
        rainbow.ScrollHue(degrees);
        //DrawUtilities.IncreaseHueBy(ref rainbow, degrees, out float hue);
        trailColor = Color.Lerp(trailColor, rainbow, 0.5f);
        Color easeColor = Color.Lerp(inColor, trailColor, EasingFunction.InExpo(Timer / 60f));
        return easeColor;
    }

    private float GetWidthFunction(float completionRatio)
    {
        return MathHelper.SmoothStep(15, 2, completionRatio);
    }

    private float GetWidthFunction2(float completionRatio)
    {
        return WidthFunction(completionRatio) * 2f;
    }
    private void DrawBetter()
    {

        var verts1 = DrawUtilities.PrepareSimpleTrailing(Projectile.oldPos, GetColorFunction, GetWidthFunction, Projectile.Size * 0.5f);
        var verts2 = DrawUtilities.PrepareSimpleTrailing(Projectile.oldPos, GetColorFunction, GetWidthFunction2, Projectile.Size * 0.5f);
        var renderer = ModContent.GetInstance<MoonArrowTrailRenderer>();
        renderer.PrepareForRendering(verts1);
        renderer.PrepareForBigRendering(verts2);
       
    }

    public override bool PreDraw(ref Color lightColor)
    {
        DrawBetter();
        return false;
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}

public class AmalgamPlayer : ModPlayer
{

    public override void PostUpdateEquips()
    {
        base.PostUpdateEquips();
        if (Player.HeldItem.type == ModContent.ItemType<AmalgamRapidfirebow>())
        {
            CrossbowPlayer crossbowPlayer = Player.GetModPlayer<CrossbowPlayer>();
            crossbowPlayer.magicBigCircleColor = Color.SkyBlue;
            crossbowPlayer.magicBigCircleTextureAsset = AssetManager.GlowMask.MagicCircle;
        }
    }
}

public class AmalgamGlobalProjectile : GlobalProjectile
{
    public override bool InstancePerEntity => true;
    public bool hasAmalgam;
    public bool hasSpawnedTrail;
    public override void SendExtraAI(Projectile projectile, BitWriter bitWriter, BinaryWriter binaryWriter)
    {
        base.SendExtraAI(projectile, bitWriter, binaryWriter);
        binaryWriter.Write(hasAmalgam);
        binaryWriter.Write(hasSpawnedTrail);
    }
    public override void ReceiveExtraAI(Projectile projectile, BitReader bitReader, BinaryReader binaryReader)
    {
        base.ReceiveExtraAI(projectile, bitReader, binaryReader);
        hasAmalgam = binaryReader.ReadBoolean();
        hasSpawnedTrail = binaryReader.ReadBoolean();
    }

    public override void PostAI(Projectile projectile)
    {
        base.PostAI(projectile);
        if (hasAmalgam)
        {
            if (!hasSpawnedTrail && Main.myPlayer == projectile.owner)
            {
                ProjFirer firer = ProjFirer.From<AmalgamTrailingProjectile>(projectile);
                firer.ai0 = projectile.identity;
                firer.New();
                hasSpawnedTrail = true;
            }

            projectile.velocity *= 0.98f;
            MothMoonMagicParticleUtilities.SpawnMoonySmokeParticles(projectile.Center, projectile.velocity);
        }
    }

    public override void OnKill(Projectile projectile, int timeLeft)
    {
        base.OnKill(projectile, timeLeft);
        if (hasAmalgam)
        {
            ProjFirer firer = ProjFirer.From<AmalgamAura>(projectile);
            firer.New();
        }
    }

    public override void PostDraw(Projectile projectile, Color lightColor)
    {
        base.PostDraw(projectile, lightColor);
        if (hasAmalgam)
        {
            ModContent.GetInstance<MoonArrowRenderer>().PrepareForRendering(new()
            {
                oldPos = projectile.oldPos,
                position = projectile.Center,
                velocity = projectile.velocity
            });
        }
    }
}
public class AmalgamSuperShot : ModProjectile
{
    private ref float Timer => ref Projectile.ai[0];
    public override string Texture => TextureRegistry.EmptyTexture;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ProjectileID.Sets.TrailCacheLength[Type] = 32;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.friendly = true;
        Projectile.tileCollide = true;
        Projectile.extraUpdates = 4;
        Projectile.timeLeft = 120;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        MothMoonMagicParticleUtilities.SpawnMoonySmokeParticles(Projectile.Center, Projectile.velocity);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        return false;
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        return base.OnTileCollide(oldVelocity);
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
        Projectile.Kill();
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        if (this.OwnedByLocalClient())
        {
            var projFIrer = ProjFirer.From<MoonramBoom>(Projectile);
            projFIrer.New();

        }

    }
}

public class AmalgamRapidfirebow : BaseCrossbowItem
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.damage = 36;
        Item.shootSpeed = 15;
        Item.useAnimation = Item.useTime = 30;
    }

    public override Vector2? HoldoutOffset()
    {

        return new Vector2(-5f, 0f);
    }

    public override void ShootBow(Player player, EntitySource_ItemUse_WithAmmo source, ShootParams shootParams)
    {
        float v = 1f;
        void ShootSecondArrow()
        {
            Vector2 fireVelocity = shootParams.velocity * shootParams.speed;
            fireVelocity *= 2 * v;
            fireVelocity *= shootParams.chargeStrength;

            float bowDamage = shootParams.damage * shootParams.chargeStrength;
            Projectile crossShot = Projectile.NewProjectileDirect(source, shootParams.position, fireVelocity,
                shootParams.projToShoot, (int)bowDamage, shootParams.knockBack, player.whoAmI, ai0: shootParams.projToShoot);
            crossShot.GetGlobalProjectile<CrossbowGlobalProjectile>().isCrossbowShot = true;
            crossShot.GetGlobalProjectile<AmalgamGlobalProjectile>().hasAmalgam = true;
            v *= 0.35f;
        }

        FunctionRepeatHelper.Repeat(() => ShootSecondArrow(), repeats: 1, rate: 7);
        /*
        Vector2 fireVelocity = shootParams.velocity * shootParams.speed;
        fireVelocity *= 3;
        fireVelocity *= shootParams.chargeStrength;

        float bowDamage = shootParams.damage * shootParams.chargeStrength;
        Projectile crossShot = Projectile.NewProjectileDirect(source, shootParams.position, fireVelocity,
            shootParams.projToShoot, (int)bowDamage, shootParams.knockBack, player.whoAmI, ai0: shootParams.projToShoot);
        crossShot.GetGlobalProjectile<CrossbowGlobalProjectile>().isCrossbowShot = true;
        crossShot.GetGlobalProjectile<AmalgamGlobalProjectile>().hasAmalgam = true;*/
    }

    public override void StaminaShootBow(Player player, EntitySource_ItemUse_WithAmmo source, ShootParams shootParams)
    {
        base.StaminaShootBow(player, source, shootParams);
      Projectile.NewProjectile(source, shootParams.position, shootParams.fireVelocity, ModContent.ProjectileType<AmalgamSuperShot>(), shootParams.damage, shootParams.knockBack, player.whoAmI);
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew<ConvulgingMater, BlankBow>();
    }
}