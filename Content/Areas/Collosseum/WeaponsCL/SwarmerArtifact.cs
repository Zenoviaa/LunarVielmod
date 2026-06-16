using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Content.Areas.PunkerTown.BossesPT.Steamroller;
using Stellamod.Content.Areas.Snow.WeaponsSN;
using Stellamod.Content.CommonMaterials;
using Stellamod.Content.Gores;
using Stellamod.Core.Bases;
using Stellamod.Core.Pixelation;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Items.Ores;
using Stellamod.Visual.Particles;

using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Collosseum.WeaponsCL;

public class SwarmerArtifact : ModItem
{
    private int _dir;
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToArtifact();
        Item.damage = 12;
        Item.width = 16;
        Item.height = 16;
        Item.mana = 20;
        Item.useAnimation = Item.useTime = 24;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.knockBack = 2;
        //    Item.crit = 4;
        Item.shoot = ModContent.ProjectileType<LilSwarmer>();
        Item.shootSpeed = 15;
        Item.noMelee = true;
        Item.noUseGraphic = true;
    }


    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        base.ModifyShootStats(player, ref position, ref velocity, ref type, ref damage, ref knockback);
        position = Main.MouseWorld;
    }
    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        if (_dir == 0)
        {
            _dir = 1;
        }
        else
        {
            _dir *= -1;
        }


        Point tilePos = position.ToTileCoordinates();
        if (tilePos.X < 0)
            tilePos.X = 0;
        if (tilePos.Y < 0)
            tilePos.Y = 0;

        if (tilePos.X >= Main.maxTilesX)
            tilePos.X = Main.maxTilesX - 1;
        if (tilePos.Y >= Main.maxTilesY)
            tilePos.Y = Main.maxTilesY - 1;
        for (int i = 0; i < 1000; i++)
        {
            if (WorldGen.SolidTile(tilePos))
                break;
            tilePos.Y++;
            if (tilePos.Y >= Main.maxTilesY)
            {
                tilePos.Y = Main.maxTilesY - 1;
                break;
            }
        }
        position = tilePos.ToWorldCoordinates();
        position += Main.rand.NextVector2Circular(32, 32);
        Projectile.NewProjectile(source, position, -Vector2.UnitY, type, damage, knockback, player.whoAmI);
        Projectile.NewProjectile(source, player.Center, velocity, ModContent.ProjectileType<StaffWaveHold>(), damage, knockback, player.whoAmI,
            ai2: _dir);

        return false;
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew(
            mold: ModContent.ItemType<BlankStaff>(),
            material: ModContent.ItemType<GintzlMetal>());
    }
}


public class StaffWaveHold : ModProjectile
{
    private Vector2 _holdDirection;
    private Vector2 _thrustOffset;
    private MagicCircleRenderer _magicCircleRenderer;
    public override string Texture => TextureRegistry.EmptyTexture;
    private ref float Timer => ref Projectile.ai[0];
    private ref float SwingTime => ref Projectile.ai[1];
    private ref float SwingDir => ref Projectile.ai[2];

    public int MagicCircleStyle;
    private Player Owner => Main.player[Projectile.owner];
    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.WriteVector2(_holdDirection);
        writer.Write(MagicCircleStyle);
    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        _holdDirection = reader.ReadVector2();
        MagicCircleStyle = reader.ReadInt32();
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 1;
        Projectile.height = 1;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.ignoreWater = true;

    }

    public override bool ShouldUpdatePosition()
    {
        return false;
    }

    private float GetSwingTime()
    {
        float aSpeed = Owner.GetTotalAttackSpeed(Projectile.DamageType);
        float multiplier = 1f / aSpeed;
        float baseTime = Owner.HeldItem.useTime;
        return baseTime * multiplier;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if (Timer == 1)
        {
            SwingTime = GetSwingTime();
        }

        float ratio = Timer / SwingTime;
        float inOut = EasingFunction.QuadraticBump(ratio);
        Projectile.scale = MathHelper.Lerp(0.2f, 1f, inOut);
        _thrustOffset = Vector2.Lerp(Vector2.Zero, Projectile.velocity * 36, inOut);

        float radians = MathHelper.ToRadians(12);
        float radsOffset = MathHelper.Lerp(0, radians * SwingDir, EasingFunction.InOutExpo(Timer / (SwingTime / 2f)));
        _thrustOffset = _thrustOffset.RotatedBy(radsOffset);
        if (this.OwnedByLocalClient())
        {
            Projectile.velocity = (Main.MouseWorld - Owner.Center).SafeNormalize(Vector2.Zero);
            _holdDirection = Projectile.velocity;
            Projectile.netUpdate = true;
        }


        float rotation = Projectile.rotation;
        Owner.ChangeDir(Projectile.direction);
        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4 + radsOffset;
        if (MagicCircleStyle == 2)
            Projectile.rotation += MathHelper.PiOver4;
        Projectile.Center = Owner.MountedCenter;
        Projectile.spriteDirection = Owner.direction;
        if (Main.myPlayer == Projectile.owner)
        {
            Owner.direction = Main.MouseWorld.X > Owner.MountedCenter.X ? 1 : -1;
        }

        //  Owner.GetModPlayer<SwingPlayerV2>().isSwinging = true;
        Owner.itemRotation = rotation * Owner.direction;
        Owner.itemTime = 2;
        Owner.itemAnimation = 2;
        // Set composite arm allows you to set the rotation of the arm and stretch of the front and back arms independently
        Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.ToRadians(135));// set arm position (90 degree offset since arm starts lowered)
        if (Timer >= SwingTime)
        {
            Projectile.Kill();
        }
    }
    private void DrawPixelatedRings(GraphicsDevice gDevice)
    {
        Asset<Texture2D> magicCircleTexture = AssetManager.GlowMask.ButterflyCircle;
        if (MagicCircleStyle == 2)
            magicCircleTexture = AssetManager.GlowMask.MagicCircleVampiricVine;

        _magicCircleRenderer ??= new MagicCircleRenderer(magicCircleTexture);
        float qb = EasingFunction.QuadraticBump(Timer / SwingTime);
        Vector2 pos = Owner.MountedCenter + _holdDirection * 64 * MathHelper.Lerp(0.75f, 1f, qb);
        Vector2 velociy = _holdDirection;



        Color targetColor = Color.Pink;
        if (MagicCircleStyle == 2)
            targetColor = Color.OrangeRed;

            Color glowColor = Color.Lerp(Color.Black, targetColor, qb);
        _magicCircleRenderer.DrawRing(pos, velociy, 0, 1, glowColor, Main.GlobalTimeWrappedHourly * 3);
    }

    private void DrawButterflyCircle()
    {
        float qb = EasingFunction.QuadraticBump(Timer / SwingTime);
        Vector2 pos = Owner.MountedCenter + _holdDirection * 64 * MathHelper.Lerp(0.75f, 1f, qb);
        SpritebatchDrawer bloomDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, pos);
        bloomDrawer.color = Color.Lerp(Color.Black, Color.Violet, qb) * 0.6f;
        bloomDrawer.color.A = 0;
        bloomDrawer.scale *= 0.3f;
        bloomDrawer.scale.X *= 0.5f;
        bloomDrawer.rotation = _holdDirection.ToRotation();
        Main.spriteBatch.Draw(bloomDrawer);
        PixelationManager.QueuePrimitivesDrawAction(DrawPixelatedRings, DrawLayer.OverNPCs);
    }
    private void DrawFireCircle()
    {
        float qb = EasingFunction.QuadraticBump(Timer / SwingTime);
        Vector2 pos = Owner.MountedCenter + _holdDirection * 64 * MathHelper.Lerp(0.75f, 1f, qb);
        SpritebatchDrawer bloomDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, pos);
        bloomDrawer.color = Color.Lerp(Color.Black, Color.Red, qb) * 0.6f;
        bloomDrawer.color.A = 0;
        bloomDrawer.scale *= 0.3f;
        bloomDrawer.scale.X *= 0.5f;
        bloomDrawer.rotation = _holdDirection.ToRotation();
        Main.spriteBatch.Draw(bloomDrawer);
        PixelationManager.QueuePrimitivesDrawAction(DrawPixelatedRings, DrawLayer.OverNPCs);
    }
    public override bool PreDraw(ref Color lightColor)
    {
        if (Timer < 2)
            return false;
        SpritebatchDrawer sbDrawer = SpritebatchDrawer.FromTextureAsset(TextureAssets.Item[Owner.HeldItem.type], Projectile.Center);
        sbDrawer.rotation = Projectile.rotation;
        sbDrawer.worldPosition += _thrustOffset;
        sbDrawer.scale = Vector2.One * Projectile.scale;
        Main.spriteBatch.Draw(sbDrawer);

        switch (MagicCircleStyle)
        {
            case 1:
                DrawButterflyCircle();
                break;
            case 2:
                DrawFireCircle();
                break;
        }

        return false;
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}
public class LilSwarmer : ModProjectile
{
    private enum AIState
    {
        Jump,
        Latch
    }
    private int _hitCounter;
    private ref float Timer => ref Projectile.ai[0];

    private AIState State
    {
        get => (AIState)Projectile.ai[1];
        set => Projectile.ai[1] = (float)value;
    }
    private int TargetNPC
    {
        get => (int)Projectile.ai[2];
        set => Projectile.ai[2] = value;
    }
    private Vector2 _latchOffset;
    private bool _fall;
    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.WriteVector2(_latchOffset);
        writer.Write(_fall);
    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        _latchOffset = reader.ReadVector2();
        _fall = reader.ReadBoolean();
    }
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ProjectileID.Sets.TrailCacheLength[Type] = 8;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.friendly = false;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 18;
    }
    private void DirtRiseEffect()
    {
        int[] gores = AutoGoreLoader.FindGores("GrayRock");
        foreach (int g in gores)
        {
            Gore.NewGore(Projectile.GetSource_FromThis(),
                Projectile.Center,
                -Vector2.UnitY.RotatedByRandom(MathHelper.ToRadians(20)) * Main.rand.NextFloat(5f, 15f), g, Main.rand.NextFloat(0f, 1f));
        }

        Point point = Projectile.Center.ToTileCoordinates();
        for (int i = 0; i < 500; i++)
        {
            point.Y++;
            if (WorldGen.SolidTile(point))
                break;
        }

        for (int i = 0; i < 4; i++)
        {
            Vector2 spawnVelocity = Vector2.UnitY * Main.rand.NextFloat(-5, -15);
            int d = WorldGen.KillTile_MakeTileDust(point.X, point.Y, Framing.GetTileSafely(point));
            Dust dust = Main.dust[d];
            dust.position += Main.rand.NextVector2Circular(32, 32);
            dust.velocity = spawnVelocity;
            dust.noLightEmittence = true;
        }
        if (Main.netMode != NetmodeID.Server)
        {
            Vector2 spawnPosition = point.ToWorldCoordinates();
            spawnPosition.X += Main.rand.NextFloat(-64, 64);
            spawnPosition.Y += Main.rand.NextFloat(-64, 64);

            Vector2 spawnVelocity = Vector2.UnitY * Main.rand.NextFloat(-5, -15);
            ModContent.GetInstance<FlyingSoilSystem>().NewSoil(spawnPosition, spawnVelocity);
        }

        int soundIndex = Main.rand.Next(4) + 1;
        string swamerJump = $"Stellamod/Assets/Sounds/SwarmerJump{soundIndex}";
        SoundStyle soundStyle = new SoundStyle(swamerJump);
        soundStyle.PitchVariance = 0.3f;
        SoundEngine.PlaySound(soundStyle, Projectile.position);

        SoundStyle digSound = SoundID.WormDig;
        SoundEngine.PlaySound(digSound, Projectile.position);
    }

    public override void AI()
    {
        base.AI();
        switch (State)
        {
            case AIState.Jump:
                AI_Jump();
                break;
            case AIState.Latch:
                AI_Latch();
                break;
        }
    }

    private void SwitchState(AIState state)
    {
        if (this.OwnedByLocalClient())
        {
            Timer = 0;
            State = state;
            Projectile.netUpdate = true;
        }
    }
    private void AI_Jump()
    {
        Timer++;
        if (Timer == 1)
        {
            DirtRiseEffect();
            Projectile.velocity.Y -= 12;
        }
        Projectile.velocity.Y += 0.5f;
        if (Projectile.velocity.Y > 0)
        {
            NPC nearest = NPCHelper.FindClosestNPC(Projectile.Center, 1024);
            if (nearest != null)
            {
                Vector2 homingVelocity = ProjectileHelper.SimpleHomingVelocity(Projectile.Center, nearest.Center, Projectile.velocity, 9);
                Projectile.velocity.X = MathHelper.Lerp(Projectile.velocity.X, homingVelocity.X, 0.3f);

            }
            Projectile.friendly = true;
        }
        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
    }

    private void AI_Latch()
    {
        Timer++;
        if (Timer == 1)
        {
            NPC target = Main.npc[TargetNPC];
            if (target.active)
            {
                _latchOffset = (Projectile.Center - target.Center);
                //   TargetNPC = -1;
                var hitFX = FXUtil.GlowStretch(target.Center, Main.rand.NextVector2Circular(1, 1));
                hitFX.OuterGlowColor = Color.Red;
                hitFX.VectorScale.X *= 2;

                for (float f = 0; f < 3; f++)
                {
                    Vector2 vel = Main.rand.NextVector2Circular(6, 6);
                    var dp = DustParticle.Spawn(target.Center, vel, DustParticleSpawnParams.Default);
                    dp.innerColor = Color.Red;
                    dp.outerColor = Color.DarkRed;
                }
            }
            SoundStyle bloodletSound;
            switch (Main.rand.Next(2))
            {
                default:
                case 0:
                    bloodletSound = AssetRegistry.Sounds.Magic.BloodletHit1;
                    break;
                case 1:
                    bloodletSound = AssetRegistry.Sounds.Magic.BloodletHit2;
                    break;
            }
            bloodletSound.PitchVariance = 0.3f;
            SoundEngine.PlaySound(bloodletSound, Projectile.position);
            FXUtil.ShakeCamera(Projectile.position, 1024, 4);
        }


        NPC latchedNPC = Main.npc[TargetNPC];
        if (!latchedNPC.active)
        {
            _fall = true;
        }

        if (!_fall)
        {
            Vector2 targetPosition = latchedNPC.Center + _latchOffset;
            Projectile.velocity = (targetPosition - Projectile.Center);
            Projectile.rotation = (-_latchOffset).ToRotation() + MathHelper.PiOver4;
            Projectile.scale = ExtraMath.Osc(0.75f, 1f, speed: 3, offset: Projectile.whoAmI);
            Projectile.friendly = true;
        }
        else
        {
            if (Timer % 8 == 0)
            {
                Dust.NewDustPerfect(latchedNPC.Center, DustID.Blood, Main.rand.NextVector2Circular(2, 2));
            }

            Projectile.velocity.Y += 0.5f;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
        }
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
        if (State != AIState.Latch)
        {
            TargetNPC = target.whoAmI;
            SwitchState(AIState.Latch);
        }
        _hitCounter++;
        if (_hitCounter >= 3)
        {
            Projectile.Kill();
        }
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        for (int i = 0; i < Projectile.oldPos.Length; i++)
        {
            Vector2 pos = Projectile.oldPos[i];
            pos += Projectile.Size * 0.5f;

            SpritebatchDrawer afDrawer = SpritebatchDrawer.FromProjectile(Projectile);
            afDrawer.worldPosition = pos;
            afDrawer.color = Color.Lerp(Color.OrangeRed, Color.Transparent, i / (float)Projectile.oldPos.Length) * 0.3f;
            Main.spriteBatch.Draw(afDrawer);
        }
        SpritebatchDrawer sbDrawer = SpritebatchDrawer.FromProjectile(Projectile);
        Main.spriteBatch.Draw(sbDrawer);
        return false;
    }
}
