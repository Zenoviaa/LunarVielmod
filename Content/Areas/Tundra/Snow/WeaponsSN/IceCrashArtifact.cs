using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Bases;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Visual.Particles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.Snow.WeaponsSN;

public class IceCrashArtifact : ModItem
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToArtifact();
        Item.damage = 18;
        Item.width = 16;
        Item.height = 16;
        Item.mana = 7;
        Item.useAnimation = Item.useTime = 24;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.knockBack = 2;
        Item.crit = 4;
        Item.shoot = ModContent.ProjectileType<IceCrashCube>();
        Item.shootSpeed = 15;
        Item.noMelee = true;
        Item.noUseGraphic = true;
    }

    public override bool AltFunctionUse(Player player)
    {

        return true;
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        base.ModifyShootStats(player, ref position, ref velocity, ref type, ref damage, ref knockback);
        position = Main.MouseWorld;
    }
    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        float offsetDist = 232;
        float rot = Main.rand.NextFloat(0f, 3.14f);
        Vector2 leftOffset = -Vector2.UnitX * offsetDist;
        Vector2 rightOffset = Vector2.UnitX * offsetDist;
        leftOffset = leftOffset.RotatedBy(rot);
        rightOffset = rightOffset.RotatedBy(rot);
        Projectile.NewProjectile(source, position + leftOffset, rightOffset, type, damage, knockback, player.whoAmI, ai2: 1);
        Projectile.NewProjectile(source, position + rightOffset, leftOffset, type, damage, knockback, player.whoAmI);
        return false;
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew(
            mold: ModContent.ItemType<BlankStaff>(),
            material: ModContent.ItemType<WinterbornShard>());
    }
}

public class IceCrashCubeShard : ModProjectile
{
    private Vector2 _initialPosition;
    private ref float Timer => ref Projectile.ai[0];
    private int Frame
    {
        get => (int)Projectile.ai[1];
        set => Projectile.ai[1] = value;
    }
    private ref float Scale => ref Projectile.ai[2];
    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.WriteVector2(_initialPosition);

    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        _initialPosition = reader.ReadVector2();
    }
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.projFrames[Type] = 4;
        ProjectileID.Sets.TrailCacheLength[Type] = 16;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 48;
        Projectile.height = 48;
        Projectile.friendly = true;
        Projectile.timeLeft = 180;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.usesIDStaticNPCImmunity = true;
        Projectile.idStaticNPCHitCooldown = 24;
        Projectile.ignoreWater = true;
    }
    public override void AI()
    {
        base.AI();
        Timer++;
        if(Timer == 1 && this.OwnedByLocalClient())
        {
            _initialPosition = Projectile.Center;
            Frame = Main.rand.Next(4);
            Scale = Main.rand.NextFloat(0.35f, 0.75f);
            Projectile.netUpdate = true;
        }

        if (Timer % 4 == 0)
        {
            var d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(32, 32), DustID.GemSapphire, Vector2.Zero, Scale: 1.2f);
            d.noGravity = true;
        }

        if(Projectile.Center.Y > _initialPosition.Y + 16)
        {
            Projectile.tileCollide = true;
        }
        Projectile.scale = Scale;
        Projectile.frame = Frame;
        Projectile.velocity.X *= 0.99f;
        Projectile.velocity.Y += 0.25f;
        Projectile.rotation += Projectile.velocity.X * 0.05f;
    }
    public override bool PreDraw(ref Color lightColor)
    {
        SpritebatchDrawer backGlowDrawwer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, Projectile.Center);
        backGlowDrawwer.color = Color.DarkBlue * 0.35f;
        backGlowDrawwer.color.A = 0;
        backGlowDrawwer.scale = Vector2.One * 0.25f;
        Main.spriteBatch.Draw(backGlowDrawwer);


        SpritebatchDrawer glowDrawer2 = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.StarFlare3, Projectile.Center);
        glowDrawer2.color = Color.SkyBlue * 0.25f * ExtraMath.Osc(0.5f, 1f, speed: 3);
        glowDrawer2.color.A = 0;
        glowDrawer2.scale = Vector2.One * 0.2f;
        //    glowDrawer2.worldPosition.Y += yOsc;
        Main.spriteBatch.Draw(glowDrawer2);

        SpritebatchDrawer sbDrawer = SpritebatchDrawer.FromProjectile(Projectile);
        Main.spriteBatch.Draw(sbDrawer);
        return false;
    }
    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        SoundStyle impactSound;
        switch (Main.rand.Next(2))
        {
            default:
            case 0:
                impactSound = AssetRegistry.Sounds.Illuria.IceImpact1;
                break;
            case 1:
                impactSound = AssetRegistry.Sounds.Illuria.IceImpact2;
                break;
        }
        impactSound.PitchVariance = 0.3f;
        SoundEngine.PlaySound(impactSound, Projectile.position);
        float boomSize = Main.rand.NextFloat(0.03f, 0.04f);
        for (float n = 0; n < 3; n++)
        {
            var spawnParams = new DustParticleSpawnParams();
            spawnParams.innerColor = Color.LightSkyBlue;
            spawnParams.outerColor = Color.DarkBlue;
            spawnParams.scaleRange = new Vector2(0.3f, 1f);
            DustParticle.Spawn(Projectile.Center, -Projectile.oldVelocity.RotatedByRandom(1.5f) * Main.rand.NextFloat(0.5f, 1f) * 0.3f, spawnParams);
        }

        SmokeParticle sp = Particle<SmokeParticle>.SpawnInAlphaLayer(Projectile.Center, -Vector2.UnitY, Color.White, Scale: 1f);
        sp.initialColor = Color.White * 0.14f;
    }
}
public class IceCrashBoom : ModProjectile
{
    public override string Texture => TextureRegistry.EmptyTexture;
    private ref float Timer => ref Projectile.ai[0];
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ProjectileID.Sets.TrailCacheLength[Type] = 16;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 128;
        Projectile.height = 128;
        Projectile.friendly = true;
        Projectile.timeLeft = 60;
        Projectile.tileCollide = false;
        Projectile.light = 0.78f;
        Projectile.ignoreWater = true;
        Projectile.penetrate = -1;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = -1;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if(Timer == 1)
        {
            if (this.OwnedByLocalClient())
            {
                for(float f = 0; f < 4f; f++)
                {
                    Vector2 upVelocity = -Vector2.UnitY;
                    upVelocity *= Main.rand.NextFloat(7, 15);
                    upVelocity = upVelocity.RotatedByRandom(MathHelper.ToRadians(75));
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, upVelocity, 
                        ModContent.ProjectileType<IceCrashCubeShard>(), (int)(Projectile.damage * 0.3f), Projectile.knockBack, Projectile.owner);
                }
            }
            for (float f = 0; f < 5f; f++)
            {
                Vector2 spawnPosition = Projectile.Center + Main.rand.NextVector2Circular(64, 64);
                Vector2 spawnVelocity = Vector2.Zero;
                spawnVelocity.Y = Main.rand.NextFloat(-10, -1f);

                float spawnScale = Main.rand.NextFloat(0.75f, 1f);
                var steamParticle = Particle<ThickSmokeParticle>.Spawn(spawnPosition, spawnVelocity, color: Color.DarkGray, Scale: spawnScale);


                var sp2 = SirestiasSmokeParticle.SpawnInAlphaLayer(Projectile.Center + Main.rand.NextVector2Circular(64, 64), -Vector2.UnitY * Main.rand.NextFloat(0.2f, 0.8f));
                sp2.color = Color.Lerp(Color.White, Color.SkyBlue, Main.rand.NextFloat(1f));
                sp2.gravity = 0;
                sp2.noTileCollide = true;
                sp2.Scale *= 1f;
                //      sp2.stretchScale2 = new Vector2(1f, 0.5f);
                sp2.offsetRot = Main.rand.NextFloat(3.14f);
                sp2.noRot = true;
            }


            SoundStyle explosionSound;
            switch (Main.rand.Next(2))
            {
                default:
                case 0:
                    explosionSound = AssetRegistry.Sounds.Illuria.IceCrash1;
                    break;
                case 1:
                    explosionSound = AssetRegistry.Sounds.Illuria.IceCrash2;
                    break;
            }
            explosionSound.PitchVariance = 0.5f;
            SoundEngine.PlaySound(explosionSound, Projectile.position);

            for (int i = 0; i < 4; i++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(12, 12);
                DustParticleSpawnParams spawnParams = DustParticleSpawnParams.Default;
                spawnParams.outerColor = Color.DarkGray;
                spawnParams.scaleRange *= 0.5f;
                spawnParams.innerColor = Color.White;
                DustParticle.Spawn(Projectile.Center, velocity, spawnParams);
            }

            for (int i = 0; i < 8; i++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(12, 12);
                DustParticleSpawnParams spawnParams = DustParticleSpawnParams.Default;
                spawnParams.outerColor = Color.DarkGray;
                spawnParams.scaleRange *= 0.5f;
                spawnParams.innerColor = Color.White;
                DustParticle.Spawn(Projectile.Center, velocity, spawnParams);
            }

            var fx = FXUtil.GlowCircleBoom(Projectile.Center,
                innerColor: Color.White,
                glowColor: Color.LightSkyBlue,
                outerGlowColor: Color.DarkBlue, duration: 25, baseSize: 0.24f);
            fx.Scale *= 1f;
            FXUtil.ShakeCamera(Projectile.Center, 1024, 8);
            FXUtil.PunchCamera(Projectile.Center, Projectile.velocity.SafeNormalize(Vector2.Zero), 4, 4, 4);
        }
    }
    public override bool PreDraw(ref Color lightColor)
    {
        if (Timer < 2)
            return false;
        float outRatio = Timer / 60f;
        RadialShearShader shearShader = RadialShearShader.Instance;
        shearShader.Time = outRatio * 1.4f;

        Asset<Texture2D> magicCircle = AssetManager.GlowMask.SpiralVortex;
        SpritebatchDrawer waveDrawer = SpritebatchDrawer.FromTextureAsset(magicCircle, Projectile.Center);
        waveDrawer.rotation += Main.GlobalTimeWrappedHourly * 4;
        waveDrawer.scale = Vector2.Lerp(Vector2.One * 0.8f, Vector2.One * 1.6f, EasingFunction.OutExpo(outRatio)) * 1.5f;
        waveDrawer.color = Color.SkyBlue;
        waveDrawer.color *= MathHelper.SmoothStep(1f, 0f, outRatio);
        waveDrawer.color.A = 0;

        Main.spriteBatch.Restart(effect: shearShader.Effect);
        Main.spriteBatch.Draw(waveDrawer);

        SpritebatchDrawer backGlowDrawwer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, Projectile.Center);
        backGlowDrawwer.color = Color.DarkBlue * 0.5f;
        backGlowDrawwer.color.A = 0;
        backGlowDrawwer.scale = Vector2.One * 2f;
        Main.spriteBatch.Draw(backGlowDrawwer);

        waveDrawer.color = Color.Lerp(Color.Black, Color.White, EasingFunction.InOutSine(outRatio));
        waveDrawer.color.A = 0;
        Main.spriteBatch.Draw(waveDrawer);
        Main.spriteBatch.RestartDefaults();
        return false;
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
    }
}
public class IceCrashCube : ModProjectile
{
    private Vector2 _initialVelocity;
    private Vector2 _initialPosition;
    private float _inEasing;
    private Player Owner => Main.player[Projectile.owner];
    private ref float Timer => ref Projectile.ai[0];
    private ref float Scale => ref Projectile.ai[1];
    private ref float Explode => ref Projectile.ai[2];
    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.WriteVector2(_initialVelocity);
        writer.WriteVector2(_initialPosition);
    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        _initialVelocity = reader.ReadVector2();
        _initialPosition = reader.ReadVector2();
    }
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ProjectileID.Sets.TrailCacheLength[Type] = 16;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 64;
        Projectile.height = 64;
        Projectile.friendly = false;
        Projectile.timeLeft = 120;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
        Projectile.light = 0.78f;
        Projectile.scale = 0.001f;
        Projectile.ignoreWater = true;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if(Timer == 1)
        {
            SoundStyle growSound;
            int index = (int)Main.rand.Next(3);
            switch (index)
            {
                default:
                case 0:
                    growSound = AssetRegistry.Sounds.Illuria.SlushShot1;
                    break;
                case 1:
                    growSound = AssetRegistry.Sounds.Illuria.SlushShot2;
                    break;
                case 2:
                    growSound = AssetRegistry.Sounds.Illuria.SlushShot3;
                    break;
            }
            SoundEngine.PlaySound(growSound, Projectile.position);
            if (this.OwnedByLocalClient())
            {
                Scale = Main.rand.NextFloat(0.45f, 0.75f);
                Projectile.netUpdate = true;
            }
            _initialPosition = Projectile.Center;
            _initialVelocity = Projectile.velocity;
            Projectile.velocity = Vector2.Zero;
        }
        if (Timer % 4 == 0)
        {
            var d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(32, 32), DustID.GemSapphire, Vector2.Zero, Scale: 1.2f);
            d.noGravity = true;
        }
        _inEasing = EasingFunction.QuadraticBump(Timer / 60f);
        Vector2 targetPosition = _initialPosition + _initialVelocity;
        float easeTime = 55f;
        float ratio = Timer / easeTime;
        float ease = EasingFunction.InExpo(ratio);

        Vector2 startPosition = _initialPosition - _initialVelocity.SafeNormalize(Vector2.Zero) * 64;
        Vector2 easedPosition = Vector2.Lerp(startPosition, targetPosition, ease);
        Vector2 velocity = (easedPosition - Projectile.Center);
        Projectile.velocity = velocity;

        float targetRotation = Timer * _initialVelocity.SafeNormalize(Vector2.Zero).X * 0.1f;
        Projectile.rotation = MathHelper.Lerp(0f, targetRotation, EasingFunction.OutExpo(Timer / 25f));
        Projectile.scale = MathHelper.Lerp(0f, Scale, EasingFunction.Anticipation2(Timer / 30f));
        if(Timer >= easeTime)
        {
            if (this.OwnedByLocalClient() && Explode == 1)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), _initialPosition + _initialVelocity, Vector2.Zero, ModContent.ProjectileType<IceCrashBoom>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }
            Projectile.Kill();
        }
        if(Explode == 1)
        {
            AI_HoldAnimation();
        }
    }

    private void AI_HoldAnimation()
    {

        //Step 1. Calculate the spot we hold from
        Vector2 target = _initialPosition + _initialVelocity;
        float rotation = (target - Owner.Center).ToRotation();
        Owner.ChangeDir(Projectile.direction);
        Projectile.spriteDirection = Owner.direction;
        if (Main.myPlayer == Projectile.owner)
        {
            Owner.direction = Main.MouseWorld.X > Owner.MountedCenter.X ? 1 : -1;
        }

        //  Owner.GetModPlayer<SwingPlayerV2>().isSwinging = true;
        Owner.itemRotation = rotation * Owner.direction;
        // Set composite arm allows you to set the rotation of the arm and stretch of the front and back arms independently
        Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, rotation - MathHelper.ToRadians(90));// set arm position (90 degree offset since arm starts lowered)
    }


    private MagicCircleRenderer _magicCircleRenderer;
    private void DrawPixelatedPrims(GraphicsDevice graphicsDevice)
    {
        _magicCircleRenderer ??= new MagicCircleRenderer(AssetManager.GlowMask.MagicCircle2);
        Vector2 auraOffset = _initialVelocity * MathHelper.Lerp(0f, 0.7f, EasingFunction.OutExpo(Timer / 60f)); 
        Vector2 auraPos =_initialPosition - auraOffset;
        _magicCircleRenderer.DrawRing(auraPos, Projectile.velocity, 0, 1, Color.Lerp(Color.Transparent, Color.SkyBlue * 0.75f, EasingFunction.QuadraticBump(Timer / 60f)), Main.GlobalTimeWrappedHourly * 8);
    
        if(Explode == 1)
        {
            Vector2 target = _initialPosition + _initialVelocity;
            Vector2 vel = target - Owner.Center;
            _magicCircleRenderer.DrawRing(Owner.Center + vel.SafeNormalize(Vector2.Zero) * 48 * _inEasing, vel, 0, 1, Color.Lerp(Color.Transparent, Color.SkyBlue * 0.75f, 
                EasingFunction.QuadraticBump(Timer / 60f)), Main.GlobalTimeWrappedHourly * 8);
        }
    }
    public override bool PreDraw(ref Color lightColor)
    {
        Vector2 target = _initialPosition + _initialVelocity;
        Vector2 vel = target - Owner.Center;
        Vector2 auraPos = Owner.Center + vel.SafeNormalize(Vector2.Zero) * 48 * _inEasing;
        SpritebatchDrawer flareDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, auraPos);
        flareDrawer.color = Color.LightBlue * 0.2f * EasingFunction.QuadraticBump(Timer / 60f);
        flareDrawer.color.A = 0;
        flareDrawer.rotation = vel.ToRotation();
        flareDrawer.scale *= 0.4f;
        flareDrawer.scale.X *= 0.5f;
        Main.spriteBatch.Draw(flareDrawer);

        if(Explode == 1)
        {
            SpritebatchDrawer cubeDrawer = SpritebatchDrawer.FromTextureAsset(TextureAssets.Item[ModContent.ItemType<IceCrashArtifact>()], auraPos);
            cubeDrawer.rotation += Main.GlobalTimeWrappedHourly * 4f;
            cubeDrawer.color *= _inEasing;
            cubeDrawer.scale *= ExtraMath.Osc(0.85f, 1f, speed: 6);
            Main.spriteBatch.Draw(cubeDrawer);

        }

        SpritebatchDrawer backGlowDrawwer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, Projectile.Center);
        backGlowDrawwer.color = Color.DarkBlue * 0.5f;
        backGlowDrawwer.color.A = 0;
        backGlowDrawwer.scale = Vector2.One * 0.8f;
        Main.spriteBatch.Draw(backGlowDrawwer);


        SpritebatchDrawer spiralDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SpiralVortex, Projectile.Center);
        spiralDrawer.color = Color.SkyBlue * 0.15f;
        spiralDrawer.color.A = 0;
        spiralDrawer.scale = Vector2.One * 0.8f * EasingFunction.OutExpo(Timer / 30f);
        spiralDrawer.rotation += Main.GlobalTimeWrappedHourly * 4;
        Main.spriteBatch.Draw(spiralDrawer);

        SpritebatchDrawer glowDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, Projectile.Center);
        glowDrawer.color = Color.SkyBlue * 0.9f;
        glowDrawer.color.A = 0;
        glowDrawer.scale = Vector2.Lerp(new Vector2(0.1f, 0.05f), new Vector2(0.05f, 0.1f), ExtraMath.Osc(0f, 1f, speed: 3));
        Main.spriteBatch.Draw(glowDrawer);
        SpritebatchDrawer drawer2 = SpritebatchDrawer.FromProjectile(Projectile);
        for (int i = 0; i < Projectile.oldPos.Length; i++)
        {
            Vector2 pos = Projectile.oldPos[i];
            drawer2.worldPosition = pos + Projectile.Size * 0.5f;
            drawer2.rotation = Projectile.oldRot[i];
            float ratio = (float)i / (float)Projectile.oldPos.Length;
            ratio = 1f - ratio;
            drawer2.color = Color.White * ratio * 0.1f;
            drawer2.color.A = 0;
            Main.spriteBatch.Draw(drawer2);
        }
        SpritebatchDrawer drawer = SpritebatchDrawer.FromProjectile(Projectile);
        for (float f = 0f; f <= MathHelper.TwoPi; f += MathHelper.TwoPi / 4f)
        {
            Vector2 offset = (f + Main.GlobalTimeWrappedHourly * 3).ToRotationVector2();
            offset *= 4;
            SpritebatchDrawer drawer3 = drawer;
            drawer3.worldPosition += offset;
            drawer3.color.A = 0;
            Main.spriteBatch.Draw(drawer3);
        }
        for (float f = 0f; f <= MathHelper.TwoPi; f += MathHelper.TwoPi / 4f)
        {
            Vector2 offset = (f + Main.GlobalTimeWrappedHourly * 3).ToRotationVector2();
            offset *= 2;
            SpritebatchDrawer drawer3 = drawer;
            drawer3.worldPosition += offset;
            Main.spriteBatch.Draw(drawer3);
        }



        Main.spriteBatch.Draw(drawer);
        PixelationManager.QueuePrimitivesDrawAction(DrawPixelatedPrims);
        return false;
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);

    }
}