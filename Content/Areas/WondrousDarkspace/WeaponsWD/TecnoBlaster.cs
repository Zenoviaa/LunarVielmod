using Stellamod.Assets;
using Stellamod.Common.GunSystem;
using Stellamod.Common.Shaders;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Pixelation;
using Stellamod.Items;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.WondrousDarkspace.WeaponsWD;

public class TecnoBlaster : BaseGun
{
    public override void SetDefaults()
    {
        remainingAmmo = 16;
        Item.damage = 100;
        Item.DamageType = DamageClass.Ranged;
        Item.width = 56;
        Item.height = 56;
        Item.useTime = 12;
        Item.useAnimation = 12;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.knockBack = 6;
  //      Item.UseSound = SoundID.Item36;
        Item.autoReuse = true;
        Item.shoot = ModContent.ProjectileType<TechnoBeam>();
        Item.shootSpeed = 15;
        Item.noMelee = true;
        Item.noUseGraphic = true;
        muzzleOrigin = new Vector2(100, 10);
    }

    public override void ModifyMuzzleFlashColors(ref Color hottestColor, ref Color coldestColor)
    {
        base.ModifyMuzzleFlashColors(ref hottestColor, ref coldestColor);
        hottestColor = Color.LightBlue;
        coldestColor = Color.Purple;
    }

    public override void SetMagazine(ref GunReloadParams fireParams)
    {
        base.SetMagazine(ref fireParams);
        fireParams.maxAmmo = 16;
        fireParams.reloadWindow = 30;
    }
    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew(
            mold: ModContent.ItemType<BlankGun>(),
            material: ModContent.ItemType<MiracleThread>());
    }
    public override void ShootEffects(Vector2 position, Vector2 velocity)
    {
        // base.ShootEffects(position, velocity);
        BasicMuzzleFlash(position, velocity, Color.LightBlue, Color.Purple);
    }
}
public class TechnoBeam : ModProjectile
{
    private List<Vector2> _beamPoints;
    private ref float Timer => ref Projectile.ai[0];
    private ref float PulseTimer => ref Projectile.ai[1];
    private float BeamLength;
    private Player Owner => Main.player[Projectile.owner];
    private Vector2 EndPoint => Projectile.Center + Projectile.velocity * BeamLength;
    public override string Texture => TextureRegistry.EmptyTexture;

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        // Signals to Terraria that this Projectile requires a unique identifier beyond its index in the Projectile array.
        // This prevents the issue with the vanilla Last Prism where the beams are invisible in multiplayer.
        ProjectileID.Sets.NeedsUUID[Projectile.type] = true;

        // Prevents jitter when stepping up and down blocks and half blocks
        ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[Type] = true;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        _beamPoints = new List<Vector2>();
        Projectile.width = 4;
        Projectile.height = 4;
        Projectile.friendly = true;
        Projectile.timeLeft = 30;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 15;
    }


    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.Write(BeamLength);
    }

    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        BeamLength = reader.ReadSingle();
    }


    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        float _ = 0f;
        float width = Projectile.width;
        Vector2 start = Projectile.Center;

        Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.Zero);
        Vector2 end = start + direction * (BeamLength);
        return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, width, ref _);
    }

    public override void AI()
    {
        base.AI();

        Timer++;
        if (Timer == 1)
        {

            PulseTimer = 15;
            int Sound = Main.rand.Next(1, 3);
            if (Sound == 1)
            {
                SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/GunShootNew5");
                soundStyle.PitchVariance = 0.5f;
                soundStyle.Volume = 0.5f;
                SoundEngine.PlaySound(soundStyle, Projectile.position);
            }
            else
            {
                SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/GunShootNew5");
                soundStyle.PitchVariance = 3.5f;
                soundStyle.Volume = 0.5f;
                SoundEngine.PlaySound(soundStyle, Projectile.position);
            }
            BeamLength = ProjectileHelper.PerformBeamHitscan(Projectile, 600);
            Vector2 end = Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.Zero) * BeamLength;
        }
        PulseTimer--;
        if (PulseTimer <= 0)
        {
            PulseTimer = 0;
        }

        if (Timer % 24 == 0)
        {
            PulseTimer = 5;
            int Sound = Main.rand.Next(1, 3);
            if (Sound == 1)
            {
                SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/GunShootNew5");
                soundStyle.PitchVariance = 0.5f;
                soundStyle.Volume = 0.5f;
                SoundEngine.PlaySound(soundStyle, Projectile.position);
            }
            else
            {
                SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/GunShootNew5");
                soundStyle.PitchVariance = 3.5f;
                soundStyle.Volume = 0.5f;
                SoundEngine.PlaySound(soundStyle, Projectile.position);
            }
        }

        //Hitscan the Beam
        _beamPoints.Clear();


        Vector2 vel = Projectile.velocity.SafeNormalize(Vector2.Zero);
        Vector2 endPoint = Projectile.Center + vel * BeamLength;
        float num = Vector2.Distance(Projectile.Center, endPoint) / 16f;
        for (float i = 0; i < num; i++)
        {
            float progress = i / num;

            //      velocity = velocity.RotatedBy(MathF.Sin(progress + Main.GlobalTimeWrappedHourly) * MathHelper.ToRadians(15));
            Vector2 start = Projectile.Center;
            Vector2 end = Projectile.Center + vel * BeamLength;

            Vector2 point = Vector2.Lerp(start, end, progress);
            _beamPoints.Add(point);
        }
        _beamPoints.Add(endPoint);
        _beamPoints.Add(endPoint);
        _beamPoints.Add(endPoint);
    }

    public float WidthFunction(float completionRatio)
    {
        float inScale = EasingFunction.InOutSine(Timer / 15f);
        return MathHelper.Lerp(64, 0f, completionRatio) * Projectile.timeLeft / 30f * inScale * EasingFunction.QuadraticBump(completionRatio);
    }

    public Color ColorFunction(float completionRatio)
    {
        return Color.Lerp(Color.Violet, Color.Cyan, ExtraMath.Osc(0f, 1f, speed: 32));
    }

    public override bool PreDraw(ref Color lightColor)
    {

        PixelationManager.QueuePrimitivesDrawAction(DrawPixelated);
        return false;
    }

    public override bool ShouldUpdatePosition()
    {
        return false;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);


    }

    public void DrawPixelated(GraphicsDevice graphicsDevice)
    {

        for (int i = 1; i < _beamPoints.Count - 4; i++)
        {
            //APply Offests
            float progress = i / (float)_beamPoints.Count;
            float velOffset = MathF.Sin(progress * 8 + (-Main.GlobalTimeWrappedHourly * 12));
            Vector2 vel = Vector2.UnitY.RotatedBy(Projectile.velocity.ToRotation());
            vel = vel.SafeNormalize(Vector2.Zero);
            Vector2 offset = Vector2.Lerp(-vel, vel, velOffset) * 32 * (PulseTimer / 15f);
            _beamPoints[i] += offset;
        }
        var shader = ShaderContent.GetInstance<FixedRichLaserShader>();
        shader.LaserColor = Color.Lerp(Color.Violet, Color.Aqua, ExtraMath.Osc(0f, 1f, speed: 16));
        shader.LaserTexture = AssetManager.LaserTextures.Bloom;
        TrailDrawer.Draw(Main.spriteBatch, _beamPoints.ToArray(), ColorFunction, WidthFunction, shader);
    }
}