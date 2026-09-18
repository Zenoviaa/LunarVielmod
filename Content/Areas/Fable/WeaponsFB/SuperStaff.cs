using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Content.CommonMaterials;
using Stellamod.Content.Dusts;
using Stellamod.Core.Bases;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Trails;
using Stellamod.Visual.Particles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Fable.WeaponsFB;

public class SuperStaff : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToArtifact();
        Item.damage = 9;
        Item.DamageType = DamageClass.Magic;
        Item.useTime = Item.useAnimation = 45;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.knockBack = 2;
        Item.rare = ItemRarityID.Green;
        Item.autoReuse = false;
        Item.shootSpeed = 30f;
        Item.shoot = ModContent.ProjectileType<SuperStaffHold>();
        Item.scale = 1f;
        Item.noMelee = true; // The projectile will do the damage and not the item
        Item.value = Item.buyPrice(silver: 12);
        Item.noUseGraphic = true;
        Item.channel = true;
        Item.mana = 4;
    }
    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew(
            mold: ModContent.ItemType<BlankStaff>(),
            material: ModContent.ItemType<AlcadizScrap>());
    }
}

public class SuperStaffConjureLightning : ModProjectile
{
    private float _scale;
    private float _width;
    private Vector2[] _lightningZaps;
    private ref float Timer => ref Projectile.ai[0];
    private ref float Charge => ref Projectile.ai[1];

    public LightningTrail[] LightningTrailPath;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ProjectileID.Sets.TrailCacheLength[Type] = 16;
        ProjectileID.Sets.TrailingMode[Type] = 2;
        Main.projFrames[Type] = 4;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        _width = 1;
        _lightningZaps = new Vector2[12];
        LightningTrailPath = new LightningTrail[4];
        for (int i = 0; i < 4; i++)
        {
            LightningTrailPath[i] = new LightningTrail();
        }

        Projectile.tileCollide = false;
        Projectile.width = 49;
        Projectile.height = 49;
        Projectile.friendly = true;
        Projectile.penetrate = -1;
        Projectile.usesIDStaticNPCImmunity = true;
        Projectile.idStaticNPCHitCooldown = 8;
        Projectile.timeLeft = 420;
        Projectile.light = 0.48f;
    }

    private float WidthFunction(float completionRatio)
    {
        float progress = completionRatio / 0.3f;
        float rounded = Easing.SpikeOutCirc(progress);
        float spikeProgress = Easing.SpikeOutExpo(completionRatio);
        float fireball = MathHelper.Lerp(rounded, spikeProgress, Easing.OutExpo(1.0f - completionRatio));
        float midWidth = 6 * _width;
        return MathHelper.Lerp(0, midWidth, fireball);
    }

    private Color ColorFunction(float p)
    {
        Color trailColor = Color.Lerp(Color.White, Color.Yellow, p);
        return trailColor;
    }

    private void DrawPixelated(SpriteBatch sb, Vector2 screenPos)
    {
        Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
        Vector2 drawOrigin = texture.Size() / 2f;
        Color lightColor = Lighting.GetColor(Projectile.Center.ToTileCoordinates());
        Color drawColor = Color.White.MultiplyRGB(lightColor);
        float drawRotation = Projectile.rotation;
        float drawScale = _scale;

        SpriteBatch spriteBatch = Main.spriteBatch;

        Vector2 drawPos = Projectile.Center - Main.screenPosition;
        //    spriteBatch.Draw(texture, drawPos, Projectile.Frame(), drawColor, drawRotation, Projectile.Frame().Size() / 2f, drawScale, SpriteEffects.None, 0);
        for (int i = 0; i < 8; i++)
        {
            Vector2 flameDrawPos = drawPos + Main.rand.NextVector2Circular(2, 2);
            float rot = Main.rand.NextFloat(0f, 3.14f);

            Color glowColor = drawColor * 0.15f;
            glowColor.A = 0;
            spriteBatch.Draw(texture, flameDrawPos, Projectile.Frame(), glowColor, drawRotation + rot, Projectile.Frame().Size() / 2f,
                drawScale * VectorHelper.Osc(0.5f, 1f, speed: 12, offset: i), SpriteEffects.None, 0);
        }

        SpritebatchDrawer glowDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, Projectile.Center);
        glowDrawer.color = Color.Lerp(Color.Goldenrod * 0.5f, Color.Goldenrod * 0.75f, ExtraMath.Osc(0f, 1f, speed: 6)) * 0.4f;
        glowDrawer.color.A = 0;
        glowDrawer.scale *= 0.6f * drawScale * 0.5f;

        Main.spriteBatch.Draw(glowDrawer);

        SpritebatchDrawer vortexDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SpiralVortex, Projectile.Center);
        vortexDrawer.color = Color.Lerp(Color.Goldenrod * 0.5f, Color.Goldenrod * 0.75f,
            ExtraMath.Osc(0f, 1f, speed: 6)) * 0.8f;
        vortexDrawer.color.A = 0;
        vortexDrawer.scale *= 0.5f * drawScale * 0.5f;
        vortexDrawer.rotation = Main.GlobalTimeWrappedHourly;
        Main.spriteBatch.Draw(vortexDrawer);
    }
    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
        Vector2 drawOrigin = texture.Size() / 2f;
        Color drawColor = Color.White.MultiplyRGB(lightColor);
        float drawRotation = Projectile.rotation;
        float drawScale = _scale;

        SpriteBatch spriteBatch = Main.spriteBatch;

        //uhhh what
        //im not even gonna touch this
        var prevBelndState = Main.graphics.GraphicsDevice.BlendState;
        Main.graphics.GraphicsDevice.BlendState = BlendState.Additive;
        _width = 1;
        for (int i = 0; i < 4; i++)
        {
            LightningTrailPath[i].Draw(spriteBatch, _lightningZaps, Projectile.oldRot, ColorFunction, WidthFunction, Projectile.Size / 2);
            _width -= 0.1f;
        }

        Main.graphics.GraphicsDevice.BlendState = prevBelndState;
        PixelationManager.QueueSpritebatchDrawAction(DrawPixelated);
        return false;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if (Timer % 3 == 0)
        {
            for (int i = 0; i < _lightningZaps.Length; i++)
            {
                float width = 96 + Charge * 16;
                float progress = i / (float)_lightningZaps.Length;
                float rot = progress * MathHelper.TwoPi * 1 + (Timer * 0.05f);
                Vector2 offset = rot.ToRotationVector2() * MathF.Sin(Timer * 8 * i) * MathF.Sin(Timer * i) * VectorHelper.Osc(0, 32, speed: 3);
                _lightningZaps[i] = Projectile.Center + offset;
            }

            for (int i = 0; i < LightningTrailPath.Length; i++)
            {
                LightningTrailPath[i].RandomPositions(_lightningZaps);
            }
        }
        if (Timer % 16 == 0)
        {
            FlameParticle dp = Particle<FlameParticle>.Spawn(Projectile.Center + Main.rand.NextVector2Circular(32, 32), -Projectile.velocity, Scale: Main.rand.NextFloat(0.2f, 0.35f));
            dp.innerColor = Color.Goldenrod;
            dp.outerColor = Color.DarkGoldenrod;
         //   dp.parent = Projectile;
            dp.gravity = 0f;
            dp.dampening = 0.05f;
            dp.fast = true;
            dp.Scale *= 0.6f;
        }

        if (Timer % 12 == 0)
        {
            Vector2 vel = Vector2.Zero;
            Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.GoldCoin, vel, Scale: 1);
            d.noGravity = true;
        }
        if (Timer % 6 == 0)
        {
            Vector2 vel = Vector2.Zero;
            Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(8, 8), DustID.GoldCoin, vel, Scale: 1);
            d.noGravity = true;
        }
        if (Timer <= 15)
        {
            _scale = MathHelper.Lerp(0f, Main.rand.NextFloat(0.8f, 1f) + (Charge * 1.4f), Easing.InCubic(Timer / 15f));
        }

        DrawHelper.AnimateTopToBottom(Projectile, 4);
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        base.ModifyHitNPC(target, ref modifiers);

    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
        if (Main.rand.NextBool(3))
        {
            target.AddBuff(BuffID.Electrified, 180);
        }

        SoundStyle zapSound = SoundID.DD2_LightningBugZap;
        zapSound.PitchVariance = 0.5f;
        SoundEngine.PlaySound(zapSound, target.Center);
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        for (int i = 0; i < 16; i++)
        {
            float progress = i / 16f;
            float rot = progress * MathHelper.TwoPi;
            Vector2 vel = rot.ToRotationVector2() * 2;
            Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(8, 8), DustID.GoldCoin, vel, Scale: 1);
            d.noGravity = true;
        }
    }
}
public class SuperStaffHold : ModProjectile
{
    private Asset<Texture2D> _whiteTextureAsset;
    private Asset<Texture2D> _outlineTextureAsset;
    private enum ActionState
    {
        Aim_And_Charge,
        Fire
    }

    private float Max_Charge_Time => 180;

    ActionState State
    {
        get => (ActionState)Projectile.ai[0];
        set => Projectile.ai[0] = (float)value;
    }

    private ref float HeldRotation => ref Projectile.ai[1];

    float ChargeTimer;
    float FireTimer;

    public override void SetDefaults()
    {
        Projectile.width = 1;
        Projectile.height = 1;
        Projectile.aiStyle = 595;
        Projectile.DamageType = DamageClass.Magic;

        Projectile.friendly = false;
        Projectile.hostile = false;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.ownerHitCheck = false;
        Projectile.penetrate = -1;
        Projectile.timeLeft = int.MaxValue;
    }

    public override void AI()
    {

        switch (State)
        {
            case ActionState.Aim_And_Charge:
                AimAndCharge();
                break;
            case ActionState.Fire:
                Fire();
                break;
        }
    }

    private void ChargeVisuals(float timer, float maxTimer)
    {
        float progress = timer / maxTimer;
        float minParticleSpawnSpeed = 20;
        float maxParticleSpawnSpeed = 6;
        int particleSpawnSpeed = (int)MathHelper.Lerp(minParticleSpawnSpeed, maxParticleSpawnSpeed, progress);
        if (timer % particleSpawnSpeed == 0)
        {
            for (int i = 0; i < 2; i++)
            {
                Vector2 pos = Projectile.Center + Main.rand.NextVector2CircularEdge(128, 128);
                Vector2 vel = (Projectile.Center - pos).SafeNormalize(Vector2.Zero) * 4;
                var d = DustParticle.Spawn(pos, vel, DustParticleSpawnParams.Default);
                d.noTileCollide = true;
                d.Scale *= 0.25f;
                // var d = Dust.NewDustPerfect(pos, ModContent.DustType<GlowDust>(), vel, newColor: Color.LightGoldenrodYellow, Scale: 0.35f);
                d.gravity = 0;
            }
        }
    }
    private bool ShouldConsumeMana()
    {
        return ChargeTimer % 8 == 0;
    }

    private void AimAndCharge()
    {
        //Aiming Code
        Player player = Main.player[Projectile.owner];
        Vector2 playerCenter = player.RotatedRelativePoint(player.MountedCenter, true);
        if (Main.myPlayer == Projectile.owner)
        {
            player.ChangeDir(Projectile.direction);
            HeldRotation = (Main.MouseWorld - player.Center).ToRotation();
            Projectile.netUpdate = true;
        }

        Projectile.velocity = HeldRotation.ToRotationVector2();
        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
        Projectile.Center = playerCenter + Projectile.velocity * 1f;

        player.heldProj = Projectile.whoAmI;
        player.itemTime = 2;
        player.itemAnimation = 2;
        player.itemRotation = (float)Math.Atan2(Projectile.velocity.Y * Projectile.direction, Projectile.velocity.X * Projectile.direction);

        //Charging Code
        if (ChargeTimer == Max_Charge_Time - 1)
        {
            //Complete Charge
            for (int i = 0; i < 16; i++)
            {
                Vector2 speed = Main.rand.NextVector2CircularEdge(4f, 4f);
                var d = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowDust>(), speed, Scale: 0.5f, newColor: Color.LightCyan);
                d.noGravity = true;
            }

            SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/StormDragon_WaveCharge");
            soundStyle.PitchVariance = 0.15f;
            SoundEngine.PlaySound(soundStyle, Projectile.position);
        }

        ChargeTimer++;
        if (ChargeTimer == 1)
        {

        }

        if (ChargeTimer % 9 == 0)
        {
            SoundStyle soundStyle = SoundID.DD2_LightningAuraZap;
            soundStyle.PitchVariance = 0.2f;
            SoundEngine.PlaySound(soundStyle, Projectile.position);
        }

        bool manaIsAvailable = !ShouldConsumeMana() || player.CheckMana(player.HeldItem.mana, true, false);

        // The Prism immediately stops functioning if the player is Cursed (player.noItems) or "Crowd Controlled", e.g. the Frozen debuff.
        // player.channel indicates whether the player is still holding down the mouse button to use the item.
        bool stillInUse = player.channel && manaIsAvailable && !player.noItems && !player.CCed;


        ChargeVisuals(ChargeTimer, Max_Charge_Time);
        ChargeTimer = MathHelper.Clamp(ChargeTimer, 0, Max_Charge_Time);
        if (!player.channel || !stillInUse)
        {
            State = ActionState.Fire;
        }
    }

    public override bool ShouldUpdatePosition()
    {
        //Make velocity not move it
        return false;
    }

    private void Fire()
    {
        //Stay on player
        Player player = Main.player[Projectile.owner];
        Vector2 playerCenter = player.RotatedRelativePoint(player.MountedCenter, true);
        float swordRotation = 0f;
        if (Main.myPlayer == Projectile.owner)
        {
            player.ChangeDir(Projectile.direction);
            swordRotation = (Main.MouseWorld - player.Center).ToRotation();
        }

        Projectile.velocity = swordRotation.ToRotationVector2();
        Projectile.Center = playerCenter + Projectile.velocity * 1f;

        player.heldProj = Projectile.whoAmI;
        player.itemTime = 2;
        player.itemAnimation = 2;
        player.itemRotation = (float)Math.Atan2(Projectile.velocity.Y * Projectile.direction, Projectile.velocity.X * Projectile.direction);

        FireTimer++;
        if (FireTimer == 1)
        {
            SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/StormDragon_Wave");
            soundStyle.PitchVariance = 0.15f;
            SoundEngine.PlaySound(soundStyle, Projectile.position);
            float chargeProgress = ChargeTimer / Max_Charge_Time;
            Vector2 velocity = Projectile.velocity;
            Vector2 targetVelocity = -velocity.SafeNormalize(Vector2.Zero) * MathHelper.Lerp(0, 14, chargeProgress);
            player.velocity = VectorHelper.VelocityUpTo(player.velocity, targetVelocity);

            //Funny Screenshake
            Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(player.Center, 1024f, MathHelper.Lerp(0, 32, chargeProgress));

            //Dust Burst Towards Mouse

            int count = (int)(48f * chargeProgress);
            for (int k = 0; k < count; k++)
            {
                Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(15)) * Main.rand.NextFloat(0, 18f);
                newVelocity *= 1f - Main.rand.NextFloat(0.3f);
                var d = DustParticle.Spawn(Projectile.Center, newVelocity);
                d.Scale *= 0.3f;
                d.innerColor = Color.Goldenrod;
                d.outerColor = Color.DarkGoldenrod;
                // Dust.NewDust(Projectile.Center, 0, 0, DustID.GoldCoin, newVelocity.X, newVelocity.Y);
            }

            float multiplier = chargeProgress * 3;
            int damage = Projectile.damage + (int)(multiplier * Projectile.damage);

            Vector2 shootVelocity = Projectile.velocity;
            shootVelocity *= MathHelper.Lerp(8f, 1f, chargeProgress);
            if (this.OwnedByLocalClient())
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, shootVelocity,
                    ModContent.ProjectileType<SuperStaffConjureLightning>(), damage, Projectile.knockBack, player.whoAmI, ai1: chargeProgress);
            }

        }

        if (FireTimer >= 30)
        {
            Projectile.Kill();
        }
    }

    public override bool PreDraw(ref Color lightColor)
    {
        SpriteBatch spriteBatch = Main.spriteBatch;
        _outlineTextureAsset ??= ModContent.Request<Texture2D>(Texture + "_Outline");
        _whiteTextureAsset ??= ModContent.Request<Texture2D>(Texture + "_White");

        SpritebatchDrawer sb = SpritebatchDrawer.FromProjectile(Projectile);
        sb.drawOrigin = new Vector2(0, TextureAssets.Projectile[Type].Value.Height);
        spriteBatch.Draw(sb);

        sb.texture = _whiteTextureAsset.Value;
        float chargeProgress = ChargeTimer / Max_Charge_Time;
        chargeProgress *= MathHelper.Lerp(1f, 0f, FireTimer / 30f);
        sb.color = Color.Lerp(Color.Transparent, Color.White.MultiplyRGB(lightColor), chargeProgress);
        spriteBatch.Draw(sb);

        sb.texture = _outlineTextureAsset.Value;
        sb.color = Color.Lerp(Color.Goldenrod * 0.3f, Color.Goldenrod * 0.7f, ExtraMath.Osc(0f, 1f, speed: 6));
        spriteBatch.Draw(sb);
        return false;
    }

}
