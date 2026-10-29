using ReLogic.Content;
using Stellamod.Common;
using Stellamod.Common.Particles;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Bases;
using Stellamod.Items;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Underground.WeaponsUG;

public class EmeraldLightArtifact : ModItem
{
    public override string Texture => AssetReferences.Assets.Textures.Underground.Items.EmeraldLightArtifact.KEY;
    public override void SetDefaults()
    {
        base.SetDefaults();
        //If we make this a custom use style, the hold up animation we'll be easily usable late
        //In this case, I want the staff to be held up and wave back and forth a little bit
        //To create this, we'll first default it to an artifact, and then reference the ExampleCustomUseStyle on example mod
        //Create a folder called CoolUseStyles in the common folder.

        //BUTTT
        //In this case it should probably be a held projectile 
        Item.DefaultToArtifact();
        Item.damage = 47;
        Item.knockBack = 1;
        Item.useAnimation = Item.useTime = 36;
        Item.useStyle = ItemUseStyleID.HoldUp;
        Item.mana = 25;
        Item.channel = true;
        Item.autoReuse = false;
        Item.shoot = ModContent.ProjectileType<EmeraldLightArtifactHeld>();
        Item.noUseGraphic = true;
        Item.noMelee = true;
    }

    public override bool CanShoot(Player player)
    {
        return base.CanShoot(player) ;
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        base.ModifyShootStats(player, ref position, ref velocity, ref type, ref damage, ref knockback);
        velocity = -Vector2.UnitY;
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew<
            MinersGold, 
            BlankStaff>();
    }
}

public class EmeraldLightBlastHit : ModProjectile
{
    float Time => 66;
    ref float Timer => ref Projectile.ai[0];
    public override string Texture => AssetReferences.Assets.Textures.Underground.Items.EmeraldBlastHit.KEY;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.projFrames[Type] = 22;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;
        Projectile.light = 0.8f;
        Projectile.timeLeft = 22 * 2;
        Projectile.WidthAndHeight = 16;

    }

    public override void AI()
    {
        base.AI();
        Projectile.frame = (int)(Timer / 2F);
        Projectile.velocity = Vector2.Zero;
        Timer++;

    }

    public override bool PreDraw(ref Color lightColor)
    {
        var drawer = Projectile.Drawer;
        drawer.color = Color.White;
        drawer.color.A = 0;
        Main.spriteBatch.Draw(drawer);
        return false;
    }
}

public class EmeraldLightArtifactHeld : ModProjectile
{
    bool _swing;
    float MinVel => 128;
    float MaxVel => 300;
    float AttackPower
    {
        get
        {
            return AttackNumber / AttackCount;
        }
    }

    float ChargeRatio
    {
        get
        {
            var owner = Projectile.PlayerOwner;
            var chargeTime = owner.HeldItem.useTime * owner.HeldItem.ModItem.UseSpeedMultiplier(owner);
            var chargeRatio = AttackTimer / chargeTime;
            return chargeRatio;

        }
    }
    Vector2 Tip => Projectile.Center + Projectile.velocity * 64;
    float AttackCount => 3;
    ref float AttackNumber => ref Projectile.ai[2];
    ref float Timer => ref Projectile.ai[0];
    ref float AttackTimer => ref Projectile.ai[1];
    public override string Texture => AssetReferences.Assets.Textures.Underground.Items.EmeraldLightArtifactHeld.KEY;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Projectile.SetTrailCacheLength(8);
        Main.projFrames[Type] = 3;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.tileCollide = false;
        Projectile.timeLeft = 600;
        Projectile.WidthAndHeight = 60;
        Projectile.ignoreWater = true;
        Projectile.light = 0.6f;
    }

    public override bool ShouldUpdatePosition()
    {
        return false;
    }

    void MakeAmbientParticles()
    {
        //This effects pretty cool might pul it out to a function
        if (Main.rand.NextBool(3))
        {
            var owner = Projectile.PlayerOwner;
            var pos = owner.Center + Main.rand.NextVector2Circular(64, 64);
            var d = Dust.NewDustPerfect(owner.Center, DustID.GemEmerald, Main.rand.NextVector2Circular(0.4f, 0.4f), Scale: 0.3f);
            d.noGravity = true;
        }
    }

    void MakePullingParticles()
    {
        //This effects pretty cool might pul it out to a function
        if (Main.rand.NextBool(3))
        {
            var pos = Tip + Main.rand.NextVector2Circular(256, 256);
            var vel = (Tip - pos).Resize(7);
            Particles.BitDust.Spawn(BitDustFactory.SlowingOverTime with
            {
                position = pos,
                velocity = vel,
                timeLeft = 60,
                innerColor = Color.White.ToVector4(),
                outerColor = Color.LightSeaGreen.ToVector4(),
                scale = new Vector2(Main.rand.NextFloat(0.3f, 0.7f))
            });
        }
    }
    void GainCharge()
    {
        var sound = AssetReferences.Assets.Sounds.Magic.EmeraldCharge.Asset with { PitchVariance = 0.35f };
        SoundEngine.PlaySound(sound, Projectile.position);

        var owner = Projectile.PlayerOwner;
        owner.CheckMana(owner.HeldItem.mana, true, false);

        var fx = FXUtil.GlowCircleBoom(Tip, Color.White, Color.LightGreen, Color.DarkGreen, 25);
        fx.Scale *= 1f + 0.25f;

        var fac = ParticleUtils.ParticleFactory.FromSmallBurst(Tip, Tip + Projectile.velocity.SafeNormalize(Vector2.Zero), TriColorPalette.Foresty, new Vector2(4, 12));
        ParticleUtils.CreateSwirlingDustBurst(fac);

        var pC = 8 + 2 ;
        for (var i = 0; i < pC; i++)
        {
            var pos = Tip + Main.rand.NextVector2Circular(32, 32);
            var vel = Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedByRandom(1.5f) * Main.rand.NextFloat(5, 12);
            Particles.GlowDust.Spawn(new()
            {
                position = pos,
                velocity = vel,
                timeLeft = 30,
                color = Color.LightGreen,
                scale = Main.rand.NextFloat(0.15f, 0.25f)
            });
        }


        AttackNumber++;
        AttackTimer = 0;
    }

    void FireBlastConsumeManaResetCharge()
    {
        if (AttackNumber <= 0)
            return;

        var owner = Projectile.PlayerOwner;
        owner.CheckMana(owner.HeldItem.mana, true, false);
        if (this.OwnedByLocalClient())
        {
            var firer = ProjFirer.From<EmeraldLightBlast>(Projectile);
            firer.velocity = (Main.MouseWorld - Tip).SafeNormalize(Vector2.Zero) * MathHelper.Lerp(MinVel, MaxVel, AttackPower);
            if(AttackNumber == 3)
            {
                firer.damage = Projectile.damage * 5; 
            }
            else
            {
                firer.damage = (int)(Projectile.damage * (1f + (AttackPower * 2)));
            }
    
            firer.knockback = 2;
            firer.position = Tip;
            firer.ai1 = (AttackNumber - 1);
            firer.New();
        }
        AttackTimer = 0;
    }

    void AI_Swing()
    {
        var owner = Projectile.PlayerOwner;
        var startUp = -Vector2.UnitY * 16;
        var oscRange = 0.08f;
        startUp = startUp.RotatedBy(MathF.Sin(Timer * 0.02f) * oscRange);
        startUp *= MathHelper.Lerp(0f, 1f, EasingFunction.InOutSine(Timer / 24f));

        //Have the weapon wiggle up and down
        var osc2 = MathHelper.Lerp(0.75f, 1f, MathF.Sin(Timer * 0.04f) * 0.5f + 0.5f);
        startUp.Y *= osc2;
    
        var holdPosition = owner.MountedCenter + startUp;

        var currentRadians = startUp.ToRotation();
        var targetRadians = (Main.MouseWorld - Projectile.Center).ToRotation();
        var easingRadians = Utils.AngleLerp(currentRadians, targetRadians, EasingFunction.InOutExpo7(Timer / 55f));

        Projectile.Center = owner.MountedCenter + Projectile.velocity.SafeNormalize(Vector2.Zero) * 16;
        Projectile.velocity = easingRadians.ToRotationVector2();
        Projectile.rotation = easingRadians + MathHelper.PiOver4;

        //Quickest way to orient hand is to just grab the swinging code
        MoonUtils.OrientHandForLikeSword(Projectile, owner);
        owner.itemTime = owner.itemAnimation = 16;
        if (Timer == 55f)
        {
            FireBlastConsumeManaResetCharge();

        }
        Projectile.spriteDirection = 1;
        if(Timer >= 75)
        {
            Projectile.Kill();
        }
    }



    public override void AI()
    {
        base.AI();
        Timer++;
        if (_swing)
        {
            AI_Swing();
            return;
        }
        //Wave upwards/side to side above the player
        var owner = Projectile.PlayerOwner;
        var startUp = -Vector2.UnitY * 16;
        var oscRange = 0.08f;
        startUp = startUp.RotatedBy(MathF.Sin(Timer * 0.02f) * oscRange);
        startUp *= MathHelper.Lerp(0f, 1f, EasingFunction.InOutSine(Timer / 24f));

        //Have the weapon wiggle up and down
        var osc2 = MathHelper.Lerp(0.75f, 1f, MathF.Sin(Timer * 0.04f) * 0.5f + 0.5f);
        startUp.Y *= osc2;

        //Mounted center is used here cause that position changes when you're on a mount.
        //I believe that's better than using player.Center?
        var holdPosition = owner.MountedCenter + startUp;
        Projectile.Center = holdPosition;
        Projectile.velocity = startUp.SafeNormalize(Vector2.Zero);
        Projectile.rotation = startUp.ToRotation() + MathHelper.PiOver4;

        //Quickest way to orient hand is to just grab the swinging code
        MoonUtils.OrientHandForLikeSword(Projectile, owner);
        owner.itemTime = owner.itemAnimation = 16;

        if (this.OwnedByLocalClient())
        {
            Projectile.direction = Main.MouseWorld.X > owner.Center.X ? 1 : -1;
            Projectile.spriteDirection = owner.direction;
            Projectile.netUpdate = true;
        }
        Projectile.spriteDirection = 1;
        if (!owner.channel || !owner.CheckMana(owner.HeldItem.mana, false, false) || owner.CCed || owner.noItems)
        {
            _swing = true;
            Projectile.netUpdate = true;
            Timer = 0;

            //FireBlastConsumeManaResetCharge();
            //Projectile.Kill();
        }
        owner.ChangeDir(Projectile.direction);


        //Here we'll do the attack logic
        //The weapon needs to have little particles come into it and the attack time should be absed on the held item
        if(AttackNumber < AttackCount)
        {
            AttackTimer++;


            //here we go a bit above 1 so it stays flashing slightly before it bursts
            if (ChargeRatio >= 1.1f)
            {
                GainCharge();
            }
        }


        MakeAmbientParticles();
        MakePullingParticles();
    }


    public override bool PreDraw(ref Color lightColor)
    {
        var drawer = Projectile.Drawer;
        drawer.BottomLeftOrigin();
        drawer.drawOrigin += new Vector2(8, -8);
        Main.spriteBatch.Draw(drawer);

        var whiteDrawer = drawer;
        whiteDrawer.VerticalFrame(1, Main.projFrames[Type]);
        var flickerColor = Color.Lerp(Color.LightGreen, Color.White, ExtraMath.Osc(0f, 1f, speed: 10));
        whiteDrawer.color = Color.Lerp(Color.Transparent, flickerColor, ChargeRatio);
        Main.spriteBatch.Draw(whiteDrawer);

        var outlineDrawer = drawer;
        outlineDrawer.VerticalFrame(2, Main.projFrames[Type]);
        outlineDrawer.color = Color.Lerp(Color.Transparent, Color.LightGreen, ChargeRatio);
        Main.spriteBatch.Draw(outlineDrawer);

        DrawUtilities.DrawBasicGlow(Main.spriteBatch,
            Tip, 0.16f * AttackNumber / AttackCount, Color.LightGreen * ExtraMath.Osc(0.5f, 1f, speed: 6));
        return false;
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);

        //little effect when it dissipates
        var fac = ParticleUtils.ParticleFactory.FromSmallBurst(
            Projectile.Center, 
            Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.Zero),
            TriColorPalette.Foresty, new Vector2(4, 15));
        fac.particleCount += 10;
        ParticleUtils.CreateSwirlingDustCircle(fac);
    }
}

public class EmeraldLightBlast : ModProjectile
{
    float Width => 100;
    float Time => 33 * 1;
    ref float Timer => ref Projectile.ai[0];
    int Style => (int)Projectile.ai[1];
    public override string Texture => ModType.NoTexture;
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.LocalHitOnce = true;
        Projectile.WidthAndHeight = 64;
        Projectile.friendly = true;
        Projectile.light = 0.6f;
        Projectile.timeLeft = (int)Time;
        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;
    }

    (Asset<Texture2D> asset, int frameCount) GetBlastTextureAsset()
    {
        switch (Style)
        {
            default:
            case 0:
                return (AssetReferences.Assets.Textures.Underground.Items.EmeraldLightBlast1.Asset, frameCount: 30);
            case 1:
                return (AssetReferences.Assets.Textures.Underground.Items.EmeraldLightBlast2.Asset, frameCount: 22);
            case 2:
                return (AssetReferences.Assets.Textures.Underground.Items.EmeraldLightBlast3.Asset, frameCount: 33);
        }
    }

    SoundStyle GetBlastSound()
    {
        switch (Style)
        {
            default:
            case 0:
                return AssetReferences.Assets.Sounds.Magic.EmeraldBlast1.Asset;
            case 1:
                return AssetReferences.Assets.Sounds.Magic.EmeraldBlast2.Asset;
            case 2:
                return AssetReferences.Assets.Sounds.Magic.EmeraldBlast3.Asset;
        }
    }

    public override bool ShouldUpdatePosition()
    {
        return false;
    }
    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
        var firer = ProjFirer.From<EmeraldLightBlastHit>(Projectile);
        firer.position = target.Center;
        firer.New();
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if (Timer == 1)
        {
            FXUtil.GlowDonut(Projectile.Center, Projectile.velocity.SafeNormalize(Vector2.Zero), Color.White, Color.LightGreen, Color.DarkBlue, scale: 0.2f + Style / 3f);
        
            //Now we add the little light effects on top
            //First a glowy boom and some particles
            var blastSound = GetBlastSound();
            SoundEngine.PlaySound(blastSound with { PitchVariance = 0.5f }, Projectile.position);

            var fx = FXUtil.GlowCircleBoom(Projectile.Center, Color.White * 0.25f, Color.LightGreen * 0.25f, Color.DarkGreen * 0.25f, 25);
            fx.Scale *= 1.5f + 0.25f * Style;

            var fac = ParticleUtils.ParticleFactory.FromSmallBurst(Projectile.Center, Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.Zero), TriColorPalette.Foresty, new Vector2(4, 12));
            ParticleUtils.CreateSwirlingDustBurst(fac);

            var pC = 8 + 2 * Style;
            for (var i = 0; i < pC; i++)
            {
                var pos = Projectile.Center + Main.rand.NextVector2Circular(32, 32);
                var vel = Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedByRandom(1.5f) * Main.rand.NextFloat(5, 12);
                Particles.GlowDust.Spawn(new()
                {
                    position = pos,
                    velocity = vel,
                    timeLeft = 30,
                    color = Color.LightGreen,
                    scale = Main.rand.NextFloat(0.15f, 0.25f)
                });
            }

            for(var i = 0; i < 4; i++)
            {
                var pos = Projectile.Center + Main.rand.NextVector2Circular(32, 32);
                var vel = Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedByRandom(1.5f) * Main.rand.NextFloat(5, 12);
                var d = Dust.NewDustPerfect(pos, DustID.GemEmerald, vel, Scale: Main.rand.NextFloat(0.3f, 0.6f));
                d.noGravity = true;
            }
        }

        Projectile.frame = (int)(Timer / 1f);
        Projectile.rotation = Projectile.velocity.ToRotation();
        if (Timer >= Time)
        {
            Projectile.Kill();
        }
    }

    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {
        base.OnHitPlayer(target, info);
        var fac = ParticleUtils.ParticleFactory.FromSmallBurst(target.Center, target.Center + Projectile.velocity.SafeNormalize(Vector2.Zero), TriColorPalette.Foresty, new Vector2(4, 12));
        fac.particleCount -= 4;
        ParticleUtils.CreateSwirlingDustBurst(fac);
    }

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        float collisionPoint = 0f;
        var hitboxStart = Projectile.Center;
        var hitboxEnd = hitboxStart + Projectile.velocity;
        bool check = Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(),
            hitboxStart, hitboxEnd, Width, ref collisionPoint);
        return check;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Projectile.rotation = Projectile.velocity.ToRotation();
        var drawData = GetBlastTextureAsset();
        var drawer = SpritebatchDrawer.FromTextureAsset(drawData.asset, Projectile.Center);
        drawer.VerticalFrame(Projectile.frame, drawData.frameCount);
        drawer.LeftCenterOrigin();
        drawer.rotation = Projectile.rotation;
        drawer.color = Color.White;
        drawer.color.A = 0;
        drawer.scale *= 2f;
        if (Style == 1)
            drawer.scale *= 0.75f;
        Main.spriteBatch.Draw(drawer);

        var glowDrawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Assets.GlowMasks.MuzzleFlash.Asset, Projectile.Center);
        glowDrawer.BottomCenterOrigin();
        glowDrawer.drawOrigin += new Vector2(0, -16);
        glowDrawer.color = Color.Lerp(Color.LightGreen, Color.Transparent, EasingFunction.OutCubic(Timer / Time)) * 1.5F;
        glowDrawer.color.A = 0;
        glowDrawer.rotation = Projectile.rotation + MathHelper.PiOver2;
        glowDrawer.scale.Y *= (float)Projectile.velocity.Length() / (float)glowDrawer.Height;
        glowDrawer.scale.X *= 2f;
        Main.spriteBatch.Draw(glowDrawer);
        return false;
    }
}