using Stellamod.Assets;
using Stellamod.Common.GunSystem;
using Stellamod.Common.Shaders;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Pixelation;
using Stellamod.Dusts;
using Stellamod.Items;
using Stellamod.Trails;
using Stellamod.Visual.Particles;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Junkyard.WeaponsJY;

public class VacuumDrill : BaseGun
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.width = 62;
        Item.height = 32;
        Item.rare = ItemRarityID.Green;
        Item.useTime = 60;
        Item.useAnimation = 60;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.autoReuse = false;
        Item.UseSound = SoundID.DD2_LightningAuraZap;

        // Weapon Properties
        Item.value = Item.sellPrice(gold: 2);
        Item.DamageType = DamageClass.Ranged;
        Item.damage = 42;
        Item.knockBack = 1;
        Item.noMelee = true;
        Item.noUseGraphic = true;
        Item.channel = true;
        // Gun Properties
        Item.shoot = ModContent.ProjectileType<VacuumLightningBolt>();
        Item.shootSpeed = 15f;
        muzzleOrigin = new Vector2(116, 16);
    }
    public override void ShootEffects(Vector2 position, Vector2 velocity)
    {
        BasicMuzzleFlash(position, velocity, Color.White, Color.Blue);
    //    base.ShootEffects(position, velocity);
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        base.ModifyShootStats(player, ref position, ref velocity, ref type, ref damage, ref knockback);
        type = ModContent.ProjectileType<VacuumLightningBolt>();
    }
    public override void SetMagazine(ref GunReloadParams fireParams)
    {
        base.SetMagazine(ref fireParams);
        fireParams.maxAmmo = 6;
    }

    public override void ModifyMuzzleFlashColors(ref Color hottestColor, ref Color coldestColor)
    {
        base.ModifyMuzzleFlashColors(ref hottestColor, ref coldestColor);
        hottestColor = Color.White;
        coldestColor = Color.SkyBlue;
    }

    public override Vector2? HoldoutOffset()
    {
        return new Vector2(2f, -2f);
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew(
            mold: ModContent.ItemType<BlankGun>(),
            material: ModContent.ItemType<MechanizedSoul>());
    }
}

public class VacuumLightningBolt : ModProjectile,
    IDrawToRenderTarget
{
    public override string Texture => TextureRegistry.EmptyTexture;
    private Vector2[] _lightningArcPos = new Vector2[1];
    public const int Trail_Width = 24;
    private ref float Timer => ref Projectile.ai[0];

    private Vector2 TargetPosition;
    private Player Owner => Main.player[Projectile.owner];
    public CoreLightning Lightning { get; set; } = new CoreLightning();
    public override void SetStaticDefaults()
    {
        // Sets the amount of frames this minion has on its spritesheet
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 48;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
    }

    public void PlayRandomZapSound(Vector2 position)
    {
        SoundStyle zapSound;
        int rand = Main.rand.Next(4);
        switch (rand)
        {
            default:
            case 0:
                zapSound = AssetRegistry.Sounds.LeviathanEel.LeviZap1 with { PitchVariance = 0.3f };
                break;
            case 1:
                zapSound = AssetRegistry.Sounds.LeviathanEel.LeviZap2 with { PitchVariance = 0.3f };
                break;
            case 2:
                zapSound = AssetRegistry.Sounds.LeviathanEel.LeviZap3 with { PitchVariance = 0.3f };
                break;
            case 3:
                zapSound = AssetRegistry.Sounds.LeviathanEel.LeviZap4 with { PitchVariance = 0.3f };
                break;
        }
        zapSound.MaxInstances = 3;
        //zapSound.Volume = 0.3f;
        SoundEngine.PlaySound(zapSound, position);
    }
    public override void SetDefaults()
    {
        Projectile.friendly = true;
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.timeLeft = 90;
        Projectile.penetrate = -1;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 10;
        Projectile.tileCollide = false;
    }

    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.WriteVector2(TargetPosition);
    }

    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        TargetPosition = reader.ReadVector2();
    }

    public override void AI()
    {
        if (TargetPosition == Vector2.Zero)
            TargetPosition = Owner.Center;
        Vector2 playerCenter = Owner.RotatedRelativePoint(Owner.MountedCenter, true);
        Timer++;
        if(Timer == 1)
        {
            PlayRandomZapSound(Projectile.position);
        }

        if (Main.myPlayer == Projectile.owner)
        {
            Owner.ChangeDir(Projectile.direction);
            TargetPosition = Vector2.Lerp(TargetPosition, Main.MouseWorld, 0.1f);
            Projectile.velocity = (TargetPosition - Owner.Center).SafeNormalize(Vector2.Zero);
            Projectile.netUpdate = true;
        }

        GunHoldPlayer gunHoldPlayer = Owner.GetModPlayer<GunHoldPlayer>();
        if(gunHoldPlayer.HeldGun != null)
        {
            Projectile.Center = gunHoldPlayer.HeldGun.GetMuzzlePosition(Owner, Projectile.velocity);
        }
     //   Projectile.Center = playerCenter + Projectile.velocity * 1f;// customization of the hitbox position

        //Dunno if this is needed but whatever
        Projectile.rotation = Projectile.velocity.ToRotation();
        _lightningArcPos = CalculateLightningArc();
        for (int i = 1; i < _lightningArcPos.Length - 1; i++)
        {
            float p = i / (float)_lightningArcPos.Length - 1;
            ref Vector2 pos = ref _lightningArcPos[i];
            ref Vector2 nextPos = ref _lightningArcPos[i + 1];
            Vector2 vec = (nextPos - pos);
            vec = vec.RotatedBy(MathHelper.ToRadians(90));
            vec *= p;

            pos += vec * MathF.Sin(Main.GlobalTimeWrappedHourly * -24 + p * 12 + Projectile.identity * 4);
            pos += vec * MathF.Sin((Main.GlobalTimeWrappedHourly + 8 + Projectile.identity * 4) * -12 + p * 6);

        }

        for (int i = 0; i < Lightning.Trails.Length; i++)
        {
            float progress = i / (float)Lightning.Trails.Length;
            var trail = Lightning.Trails[i];
            trail.LightningRandomOffsetRange = 4;
            trail.LightningRandomExpand = 24;
            trail.PrimaryColor = Color.Lerp(Color.White, Color.Cyan, progress);
            trail.NoiseColor = Color.Lerp(Color.White, Color.Cyan, progress);
            Lightning.WidthTrailFunction = WidthFunction;
        }
        for(int i = 0; i < _lightningArcPos.Length; i++)
        {
            if (Main.rand.NextBool(160))
            {
                Vector2 pos = _lightningArcPos[i];
                var dp = DustParticle.Spawn(pos, Main.rand.NextVector2Circular(8, 8));
                dp.innerColor = Color.SkyBlue;
                dp.outerColor = Color.DarkBlue;
                dp.noTileCollide = true;
                dp.gravity = 0;
                dp.dampening = 0.05f;
            }
        }
        if (Timer % 3 == 0)
        {
            // Lightning.SyncOffsets = true;
            Lightning.RandomPositions(_lightningArcPos);
            for (int i = 0; i < _lightningArcPos.Length - 3; i++)
            {
                Vector2 pos = _lightningArcPos[i];
                if (Main.rand.NextBool(8))
                {
                    Dust.NewDustPerfect(pos, ModContent.DustType<GlyphDust>(), Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(0.2f, 1f), 0, Color.Cyan, Main.rand.NextFloat(1f, 2f)).noGravity = true;
                }
            }
        }
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        //Electrifying!!!! nEMIES!!!
        target.AddBuff(BuffID.Electrified, 120);
        SoundEngine.PlaySound(SoundID.DD2_LightningBugZap, Projectile.position);

        for (int i = 0; i < 8; i++)
        {
            Vector2 speed = Main.rand.NextVector2CircularEdge(4, 4);
            var d = Dust.NewDustPerfect(target.Center, DustID.Electric, speed, Scale: Main.rand.NextFloat(0.5f, 1.5f));
            d.noGravity = true;
        }
    }

    public float WidthFunction(float completionRatio)
    {
        float outScale = EasingFunction.InOutSine((float)Projectile.timeLeft / 30f);
        float inScale = EasingFunction.InOutSine(Timer / 30f);
        float oscScalePosterized = ExtraMath.Osc(0.6f, 1f, speed: 16);
        oscScalePosterized = MathF.Floor(oscScalePosterized * 8f) / 8f;
        return MathHelper.SmoothStep(24, 16, completionRatio) * outScale * ExtraMath.Osc(0.7f, 1f, speed: 32) * inScale * oscScalePosterized * 0.5f;
    }

    public float WidthFunction2(float completionRatio)
    {
        return WidthFunction(completionRatio) * 2.5f;
    }
    public float WidthFunction3(float completionRatio)
    {
        return WidthFunction(completionRatio) * 4f;
    }

    public Color ColorFunction(float completionRatio)
    {
        Color startColor = Color.Cyan;
        Color endColor = Color.White;
        return Color.Lerp(startColor, endColor, ExtraMath.Osc(0f, 1f, speed: 64));
    }

    private Vector2[] CalculateLightningArc()
    {
        float teleportDistance = 96;
        Vector2 currentPosition = Projectile.position;
        List<Vector2> positions = new List<Vector2>();
        positions.Add(currentPosition);
        for (int i = 0; i < 48; i++)
        {
            Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.Zero);
            float distance = 40;
            Vector2 newPosition = currentPosition + direction * distance;
            currentPosition = newPosition;
            positions.Add(currentPosition);



            Vector2 targetCenter = currentPosition;
            bool foundTarget = false;
            NPC nearest = ProjectileHelper.FindNearestEnemy(currentPosition, teleportDistance);
            if (nearest != null)
            {
                targetCenter = nearest.Center;
                positions.Add(targetCenter);
                positions.Add(targetCenter);
                break;
            }

            if (!foundTarget)
            {
                float distanceToMouse = Vector2.Distance(currentPosition, TargetPosition);
                if (distanceToMouse < teleportDistance)
                {
                    positions.Add(TargetPosition);
                    positions.Add(TargetPosition);
                    break;
                }
            }
        }


        return positions.ToArray();
    }

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        //This damages everything in the trail
        Vector2[] positions = _lightningArcPos;
        float collisionPoint = 0;
        for (int i = 1; i < positions.Length; i++)
        {
            Vector2 position = positions[i];
            Vector2 previousPosition = positions[i - 1];
            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), position, previousPosition, Trail_Width, ref collisionPoint))
                return true;
        }
        return base.Colliding(projHitbox, targetHitbox);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        float outScale = EasingFunction.InOutSine((float)Projectile.timeLeft / 30f);
        float inScale = EasingFunction.InOutSine(Timer / 30f);
        float oscScalePosterized = ExtraMath.Osc(0.6f, 1f, speed: 16);
        oscScalePosterized = MathF.Floor(oscScalePosterized * 8f) / 8f;
        float s = outScale * inScale * oscScalePosterized;
        SpriteBatch spriteBatch = Main.spriteBatch;
      //  Lightning.Draw(spriteBatch, _lightningArcPos, Projectile.oldRot);

        Texture2D texture = ModContent.Request<Texture2D>(TextureRegistry.EmptyGlowParticle).Value;
        Vector2 centerPos = _lightningArcPos[_lightningArcPos.Length - 1] - Main.screenPosition;
        centerPos += Main.rand.NextVector2Circular(8, 8);
        GlowCircleShader shader = GlowCircleShader.Instance;

        //How quickly it lerps between the colors
        shader.Speed = 10f;

        //This effects the distribution of colors
        shader.BasePower = 2.5f;

        //Radius of the circle
        shader.Size = VectorHelper.Osc(0.09f, 0.14f, speed: 6);


        //Colors
        Color startInner = Color.White;
        Color startGlow = Color.Lerp(Color.Cyan, Color.Cyan, VectorHelper.Osc(0f, 1f, speed: 3f));
        Color startOuterGlow = Color.Lerp(Color.Black, Color.Black, VectorHelper.Osc(0f, 1f, speed: 3f));

        shader.InnerColor = startInner;
        shader.GlowColor = startGlow;
        shader.OuterGlowColor = startOuterGlow;

        //Idk i just included this to see how it would look
        //Don't go above 0.5;
        shader.Pixelation = 0.005f;

        //This affects the outer fade
        shader.OuterPower = 13.5f;
        shader.Apply();

        spriteBatch.Restart(blendState: BlendState.Additive, effect: shader.Effect);
        for (int i = 0; i < 2; i++)
        {
            spriteBatch.Draw(texture, centerPos, null, Color.White, Projectile.rotation, texture.Size() / 2f, s, SpriteEffects.None, 0);
        }

        spriteBatch.RestartDefaults();



        SpritebatchDrawer glowDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.JumbledGlowCircle.Asset, Projectile.Center);
        glowDrawer.color = Color.Lerp(Color.White, Color.SkyBlue, ExtraMath.Osc(0f, 1f, speed: 16)) * 0.6f;
        glowDrawer.color.A = 0;
        glowDrawer.scale *= 0.125f * ExtraMath.Osc(0.85f, 1f, speed: 16) * outScale * inScale * oscScalePosterized;
        glowDrawer.scale.X *= 0.75f;
        glowDrawer.scale.Y *= 2f;
        glowDrawer.rotation = Projectile.velocity.ToRotation();
        Main.spriteBatch.Draw(glowDrawer);

        glowDrawer.color = Color.SkyBlue * 0.6f;
        glowDrawer.color.A = 0;
        glowDrawer.scale *= 1.5f;
        Main.spriteBatch.Draw(glowDrawer);


        glowDrawer.color = Color.Blue * 0.6f;
        glowDrawer.color.A = 0;
        glowDrawer.scale *= 1.5f;
        Main.spriteBatch.Draw(glowDrawer);


        glowDrawer.color = Color.Blue * 0.6f;
        glowDrawer.color.A = 0;
        glowDrawer.scale = Vector2.One * ExtraMath.Osc(0.85f, 1f, speed: 16) * outScale * inScale * oscScalePosterized;
        glowDrawer.worldPosition = centerPos + Main.screenPosition;
        Main.spriteBatch.Draw(glowDrawer);
        return false;
    }

    private void DrawLightning(GraphicsDevice gDevice)
    {

        if (_lightningArcPos == null)
            return;

        FixedRichLaserShader richlaserShader = ShaderContent.GetInstance<FixedRichLaserShader>();
        richlaserShader.LaserColor = Color.Blue;
        richlaserShader.InnerColor = Color.SkyBlue;
        richlaserShader.OuterColor = Color.Lerp(Color.White, Color.SkyBlue, ExtraMath.Osc(0f, 1f, speed: 16));
        richlaserShader.LaserTexture = TrailRegistry.Beamlight;
        richlaserShader.BloomTexture = AssetManager.LaserTextures.TexturedLaser2;
        richlaserShader.Time = Main.GlobalTimeWrappedHourly * 77;
        TrailDrawer.Draw(_lightningArcPos, ColorFunction, WidthFunction, richlaserShader);

        var bloom = BloomTrailShader.Instance;
        bloom.InnerColor = Color.Turquoise * 0.5f;
        bloom.OuterColor = Color.DarkTurquoise;
        TrailDrawer.Draw(Main.spriteBatch, _lightningArcPos,  ColorFunction, WidthFunction2, bloom);

        bloom = BloomTrailShader.Instance;
        bloom.InnerColor = Color.SkyBlue;
        bloom.OuterColor = Color.Blue;
        TrailDrawer.Draw(Main.spriteBatch, _lightningArcPos, ColorFunction, WidthFunction3, bloom);


    }

    public void DrawToRenderTargets()
    {
        PixelationManager.QueuePrimitivesDrawAction(DrawLightning);
        //throw new NotImplementedException();
    }
}