using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Content.CommonMaterials;
using Stellamod.Content.Dusts;
using Stellamod.Core.Bases;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Effects.Generic;
using Stellamod.Items;
using Stellamod.Visual.Particles;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Illuria.WeaponsIL;

public class AzuretoothNecklace : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToArtifact();
        Item.width = 26;
        Item.height = 32;
        Item.rare = ItemRarityID.Lime;
        Item.knockBack = 2;

        Item.DamageType = DamageClass.Magic;
        Item.damage = 98;
        Item.mana = 8;

        Item.useTime = 45;
        Item.useAnimation = 45;
        Item.useStyle = ItemUseStyleID.Shoot;

        Item.shoot = ModContent.ProjectileType<AzuretoothNecklaceHold>();
        Item.shootSpeed = 10;
        Item.autoReuse = false;
        Item.noMelee = true;
        Item.channel = true;
        Item.noUseGraphic = true;
    }


    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew(
            mold: ModContent.ItemType<BlankStaff>(), 
            material: ModContent.ItemType<IllurineScale>());
    }
}


public class AzuretoothDragon : ModProjectile
{
    private Asset<Texture2D> _glowTextureAsset;
    private float _hitCount;
    private float _deathTimer;
    private float _trailAlpha;
    private ref float Timer => ref Projectile.ai[0];
    private ref float Scale => ref Projectile.ai[1];
    private ref float HomingStrength => ref Projectile.ai[2];
    private float Alpha => MathHelper.Lerp(1f, 0f, _deathTimer / 30f) * EasingFunction.InOutSine(Timer / 30f);

    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.Write(_hitCount);
        writer.Write(_deathTimer);
    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        _hitCount = reader.ReadSingle();
        _deathTimer = reader.ReadSingle();
    }

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Type] = 32;
        ProjectileID.Sets.TrailingMode[Type] = 2;
        Main.projFrames[Type] = 4;
    }

    public override void SetDefaults()
    {
        Projectile.width = 48;
        Projectile.height = 48;
        Projectile.friendly = true;
        Projectile.hostile = false;
        Projectile.timeLeft = 600;
        Projectile.penetrate = -1;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 24;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
    }

    public override void AI()
    {

        Timer++;
        if (Timer == 1)
        {
            SoundStyle summounSou = new SoundStyle("Stellamod/Assets/Sounds/ArcaneExplode") with { PitchVariance = 0.9f };
            summounSou.Volume = 0.5f;
            SoundEngine.PlaySound(summounSou, Projectile.position);
            if (this.OwnedByLocalClient())
            {
                HomingStrength = MathHelper.Lerp(0.4f, 1f, Main.rand.NextFloat(0f, 1f));
                Scale = Main.rand.NextFloat(0.9f, 1.6f);
                Projectile.netUpdate = true;
            }
        }
        Visuals();
        if (_deathTimer > 0)
        {
            Projectile.friendly = false;
            _deathTimer++;
            if (_deathTimer >= 30)
            {
                Projectile.Kill();
            }
            Projectile.velocity *= 0.94f;
            return;
        }

        if (Main.rand.NextBool(16))
        {
            var sp = SparkleParticle.Spawn(Projectile.Center + Main.rand.NextVector2Circular(16, 16), Vector2.Zero, Scale: 0.6f);
            sp.innerColor = Color.White;
            sp.gravity = 0;
            sp.fast = true;
            sp.flickering = true;
            sp.outerColor = Color.SkyBlue;
        }

        float maxDetectDistance = 1500;
        NPC closestNpc = NPCHelper.FindClosestNPC(Projectile.position, maxDetectDistance);
        if (closestNpc != null)
        {
            _trailAlpha = MathHelper.Lerp(_trailAlpha, 1f, 0.1f);
            Vector2 targetVelocity = Projectile.Center.DirectionTo(closestNpc.Center) * 16;

            Projectile.velocity = Projectile.velocity.MoveTowards(targetVelocity, HomingStrength); ;
            Projectile.alpha++;
            if (Projectile.alpha >= 255)
                Projectile.alpha = 255;
        }
        else
        {
            _trailAlpha = MathHelper.Lerp(_trailAlpha, 0f, 0.1f);

        }

        Projectile.rotation = MathHelper.Lerp(Projectile.rotation, Projectile.velocity.ToRotation(), 0.5f);
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
        _hitCount++;
        if (_hitCount >= 2 && _deathTimer == 0)
        {
            _deathTimer = 1;
            Projectile.netUpdate = true;
        }
        Projectile.alpha -= 50;
        Projectile.velocity *= 1.2f;
    }

    private void DrawGlowyTrail(SpriteBatch spriteBatch, Vector2 screenPos)
    {
        Rectangle worldRectangle = DrawUtilities.CenterRectangle(Projectile.Center, 768, 768);
        Rectangle screenRectangle = worldRectangle;
        screenRectangle.X -= (int)Main.screenPosition.X;
        screenRectangle.Y -= (int)Main.screenPosition.Y;

        Vector2[] particles = DrawUtilities.TrailLocalRectanglePoints(Projectile.oldPos, Projectile.Center, worldRectangle, Projectile.Size * 0.5f);
        GlowyTrailShader trailShader = ShaderContent.GetInstance<GlowyTrailShader>();
        trailShader.ParticleRadius = 0.03f;
        trailShader.InsideColor = Color.SkyBlue ;//Color.Lerp(Color.PaleGoldenrod, Color.Gold, ExtraMath.Osc(0f, 1f, speed: 12, offset: Projectile.identity));
        trailShader.BloomColor = Color.DarkBlue ;
        trailShader.Particles = particles;
        SpritebatchParams spritebatchParams = SpritebatchParams.InWorldAndZoomed() with { effect = trailShader };

        Color particleColor = Color.Lerp(Color.White, Color.Transparent, Timer / 60f);
        using (var starter = SpritebatchStarter.Begin(spriteBatch, spritebatchParams))
        {
            Color c = Color.SkyBlue * Alpha * 0.1f * _trailAlpha; 
            c.A = 0;
            spriteBatch.Draw(TextureAssets.BlackTile.Value, screenRectangle, null, c, 0, Vector2.Zero, SpriteEffects.None, 0);
        }
    }

    public override bool PreDraw(ref Color lightColor)
    {
        _glowTextureAsset ??= ModContent.Request<Texture2D>($"{Texture}_Glow");
        PixelationManager.QueueSpritebatchDrawAction(DrawGlowyTrail);
        SpritebatchDrawer dRawer = SpritebatchDrawer.FromProjectile(Projectile);
        dRawer.scale = Vector2.One * Scale;
        dRawer.color *= Alpha * 0.6f;
        Main.spriteBatch.Draw(dRawer);

        dRawer.texture = _glowTextureAsset.Value;
        dRawer.color = Color.White * ExtraMath.Osc(0.6f, 1f, speed: 6, Projectile.identity) * Alpha;
        dRawer.color.A = 0;
        Main.spriteBatch.Draw(dRawer);

        return false;
    }

    private void Visuals()
    {
        int frameSpeed = 2;
        DrawHelper.AnimateTopToBottom(Projectile, frameSpeed);

        // Some visuals here
        Lighting.AddLight(Projectile.Center, Color.White.ToVector3() * 0.78f);
    }


    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        for (float f = 0; f < 4; f++)
        {
            Vector2 pos = Projectile.Center + Main.rand.NextVector2Circular(32, 32);
            Vector2 vel = (pos - Projectile.Center).SafeNormalize(Vector2.Zero);
            Dust.NewDustPerfect(pos, ModContent.DustType<SmokeDust>(), vel, newColor: Color.Lerp(Color.Aqua, Color.Black, 0.6f));
        }
    }
}
public class AzuretoothNecklaceHold : ModProjectile
{
    private ref float Timer => ref Projectile.ai[0];
    private ref float DeathTimer => ref Projectile.ai[1];

    private float Alpha => EasingFunction.InOutSine(Timer / 30f) * MathHelper.Lerp(1f, 0f, DeathTimer / 30f);
    private Player Owner => Main.player[Projectile.owner];
    private Vector2 PlayerCenter => Owner.RotatedRelativePoint(Owner.MountedCenter, true);
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Type] = 16;
        ProjectileID.Sets.TrailingMode[Type] = 2;
        Main.projFrames[Type] = 4;
    }

    public override void SetDefaults()
    {
        Projectile.width = 26;
        Projectile.height = 32;
        Projectile.tileCollide = false;
        Projectile.timeLeft = int.MaxValue;
        Projectile.friendly = false;
        Projectile.hostile = false;
        Projectile.ignoreWater = true;
    }

    public override void AI()
    {
        Timer++;
        AI_Hold();
        AI_Channel();
        Visuals();
    }

    private void AI_Hold()
    {
        if (DeathTimer >= 1)
            DeathTimer++;
        if(DeathTimer >= 30)
        {
            Projectile.Kill();
        }
        if (Owner.noItems || Owner.CCed || Owner.dead || !Owner.active)
        {
            if (DeathTimer < 1)
                DeathTimer = 1;
        }

        if (Main.myPlayer == Projectile.owner)
        {
            if (!Owner.channel)
            {
                if (DeathTimer < 1)
                {
                    DeathTimer = 1;
                    Projectile.netUpdate = true;
                }
            }
        }

        Vector2 holdOffset = new Vector2(0, -64);
        Projectile.Center = Owner.Center + holdOffset.RotatedBy(Timer / 240 * MathHelper.TwoPi);
        Projectile.Center += new Vector2(0, VectorHelper.Osc(-4, 4));
    }

    private void AI_Channel()
    {
        if (Timer % 12 == 0)
        {
            //Get the mana to continue using this
            int manaChannelCost = Owner.HeldItem.mana;
            if (!Owner.CheckMana(manaChannelCost, true))
            {
                //Not enough mana? Well die
                Projectile.Kill();
            }
            else
            {
                if (this.OwnedByLocalClient())
                {
                    //Spawn the projectile
                    Vector2 spawnPosition = PlayerCenter + Main.rand.NextVector2CircularEdge(80, 80);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), spawnPosition, Vector2.Zero,
                        ModContent.ProjectileType<AzuretoothDragon>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                }
            }
        }

        if (Timer % 16 == 0)
        {
            var sp = SparkleParticle.Spawn(Projectile.Center + Main.rand.NextVector2Circular(16, 16), Vector2.Zero, Scale: 0.6f);
            sp.innerColor = Color.White;
            sp.gravity = 0;
            sp.fast = true;
            sp.flickering = true;
            sp.outerColor = Color.SkyBlue;
        }

    }

    private void Visuals()
    {
        int frameSpeed = 6;
        DrawHelper.AnimateTopToBottom(Projectile, frameSpeed);

        // Some visuals here
        Lighting.AddLight(Projectile.Center, Color.White.ToVector3() * 0.78f);
    }


    public override bool PreDraw(ref Color lightColor)
    {
        SpritebatchDrawer azureHoldDraw = SpritebatchDrawer.FromProjectile(Projectile);
        azureHoldDraw.color *= Alpha;
        Main.spriteBatch.Draw(azureHoldDraw);

        SpritebatchDrawer glowDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, Projectile.Center);
        glowDrawer.color = Color.SkyBlue * ExtraMath.Osc(0.6f, 1f, speed: 2) * Alpha * 0.4f;
        glowDrawer.color.A = 0;
        glowDrawer.scale *= 0.26f;
        Main.spriteBatch.Draw(glowDrawer);
        return false;
    }
}
