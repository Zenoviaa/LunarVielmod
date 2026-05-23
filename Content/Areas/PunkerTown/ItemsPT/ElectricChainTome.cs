using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Common.Shaders.MagicTrails;
using Stellamod.Content.Areas.MoonspiralTower.WeaponsMT;
using Stellamod.Content.Areas.WaterSide.KingJellyfishBoss;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Bases;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Items.Materials;
using Stellamod.Items.Weapons.Mage.Tomes;
using Stellamod.Visual.Particles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.PunkerTown.ItemsPT;

public class ElectricChainTome : AbstractMagicTome
{
    public override void SetStaticDefaults()
    {
        CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
    }

    public override void SetDefaults2()
    {
        base.SetDefaults2();
        Item.damage = 45;
        Item.width = 50;
        Item.height = 50;
        Item.shoot = ModContent.ProjectileType<ElectricChain>();
        Item.shootSpeed = 15f;
        Item.mana = 15;
        Item.useTime = Item.useAnimation = 24;
        Item.UseSound = null;
    }

    public override Asset<Texture2D> GetMagicCircleTexture()
    {
        return AssetManager.GlowMask.MagicCircleVampiricVine;
    }

    public override Color GetTomeHintColor()
    {
        return Color.Goldenrod;
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew(
            mold: ModContent.ItemType<BlankStaff>(),
            material: ModContent.ItemType<MarshScrap>());
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        base.ModifyShootStats(player, ref position, ref velocity, ref type, ref damage, ref knockback);
        Vector2 target = Main.MouseWorld;
        Vector2 vectorToMouse = (target - player.Center);
        if (vectorToMouse.Length() < 800)
            velocity = vectorToMouse;
        else
            velocity = velocity.Resize(800);
       // velocity = velocity.RotatedByRandom(MathHelper.ToRadians(22));
    }
    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        return base.Shoot(player, source, position, velocity, type, damage, knockback);
    }
}

public class ElectricChain : ModProjectile
{
    private Asset<Texture2D> _gradientTextureAsset;
    private List<Vector2> _targets;
    private Vector2 _hitPoint;
    private Player Owner => Main.player[Projectile.owner];
    private ref float Timer => ref Projectile.ai[0];
    public override string Texture => TextureRegistry.EmptyTexture;
    public override void SetDefaults()
    {
        base.SetDefaults();
        _targets = new();
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 15; 
        Projectile.friendly = true;
        Projectile.timeLeft = 30;
        Projectile.ignoreWater = true;
    }

    public override bool ShouldUpdatePosition()
    {
        return false;
    }

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        //This damages everything in the trail
        float collisionPoint = 0;
        if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), _hitPoint + (_hitPoint - Projectile.Center).SafeNormalize(Vector2.Zero) * 32, Projectile.Center, 12, ref collisionPoint))
            return true;

        for (int i = 1; i < _targets.Count; i++)
        {
         
            Vector2 start = _targets[i - 1];
            Vector2 end = _targets[i];
            Vector2 velocity = (end - start).SafeNormalize(Vector2.Zero);
            end += velocity * 32;
            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), end, start, 12, ref collisionPoint))
                return true;
        }

        return false;
    }

    public override void AI()
    {
        base.AI();

        //OK SO HERE'S WHAT WE DO
        //First cast a lightning out based on the velocity of the projectile
        Timer++;
        if(Timer == 1)
        {
            string path = $"Stellamod/Assets/Sounds/Dreadmire__LightingRain{Main.rand.Next(3)+1}";
            SoundStyle sound = new SoundStyle(path) with { PitchVariance = 0.3f, Volume = 0.5f };
            SoundEngine.PlaySound(sound, Projectile.position);

            for(float f = 0; f < 4; f++)
            {
                Vector2 velocity = Projectile.velocity.Resize(Main.rand.NextFloat(10, 20)).RotatedByRandom(MathHelper.ToRadians(32));
                var dp =    DustParticle.Spawn(Projectile.Center, velocity);
                dp.outerColor = Color.Goldenrod;
                dp.Scale *= Main.rand.NextFloat(0.4f, 0.75f);
                dp.gravity = 0;
                dp.dampening = 0.1f;
            }
        }
     //   Projectile.Center = Owner.Center;
        float distance = ProjectileHelper.PerformBeamHitscan(Projectile.Center, Projectile.velocity, Projectile.velocity.Length());
        Vector2 endPoint = Projectile.Center + Projectile.velocity.Resize(distance);
        _hitPoint = endPoint;
        int bounceCount = 0;
        int found = -1;
        HashSet<int> hitTargets = new HashSet<int>();
        _targets.Clear();

        //With a do while loop, it'll always excute at least once and then all further executions only happen if the condition is true
        //Saves a bit of code to write it this way
        do
        {
            (found, endPoint) = FindNextTarget(endPoint, hitTargets);
            if (found != -1)
            {
                bounceCount++;
                hitTargets.Add(found);
                _targets.Add(Main.npc[found].Center);
            }
        }
        while (found >= 0 && bounceCount < 3);
    }

    private (int, Vector2) FindNextTarget(Vector2 startPoint, HashSet<int> exclusion)
    {
        Vector2 target = startPoint;
        float distanceToTarget = float.MaxValue;
        int found = -1;
        foreach(var npc in Main.ActiveNPCs)
        {
            if (npc.friendly)
                continue;
            if (npc.townNPC)
                continue;
            if (!npc.CanBeChasedBy())
                continue;
            if (exclusion.Contains(npc.whoAmI))
                continue;


            float distanceToEnemy = Vector2.Distance(target, npc.Center);
            if(distanceToEnemy < distanceToTarget && distanceToEnemy < 512)
            {
                found = npc.whoAmI;
                target = npc.Center;
                distanceToTarget = distanceToEnemy;
            }
        }

        return (found, target);
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
        target.AddBuff(BuffID.Electrified, 30);

        FXUtil.GlowCircleBoom(target.Center,
            innerColor: Color.White,
            glowColor: Color.Goldenrod,
            outerGlowColor: Color.DarkGoldenrod, duration: 25, baseSize: Main.rand.NextFloat(0.04f, 0.08f));

        for (float f = 0; f < 2; f++)
        {
            var dp = Particle<DustParticle>.Spawn(target.Center, Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(4, 7), Scale: Main.rand.NextFloat(0.5f, 1f));
            dp.outerColor = Color.DarkGoldenrod;
            dp.noTileCollide = true;
            dp.dampening = 0.05f;
            dp.Scale *= 0.5f;

        }

    }


    private Color GetTrailColor(float ratio)
    {
        Color bloomColor = Color.Lerp(Color.White, Color.Goldenrod, 0.8f);
        Color bloomColor2 = Color.Lerp(Color.Transparent, bloomColor, EasingFunction.QuadraticBump(ratio));
        return Color.Lerp(Color.DarkGoldenrod, Color.White, EasingFunction.QuadraticBump(ratio));
    }

    private float GetTrailWidth(float ratio)
    {

        float w = 72;
        float outEasing = EasingFunction.InExpo((float)Projectile.timeLeft / 30f);
        float outEasing2 = MathHelper.SmoothStep(0.5f, 1f, Timer / 15f);
        return MathHelper.SmoothStep(w * 0.85f, w, EasingFunction.QuadraticBump(ratio)) * outEasing * outEasing2;
    }

    private Color GetTrailColor2(float ratio)
    {
        Color bloomColor = Color.Lerp(Color.White, Color.Goldenrod, 0.8f);
        Color bloomColor2 = Color.Lerp(Color.Transparent, bloomColor, EasingFunction.QuadraticBump(ratio));
        return Color.Lerp(Color.DarkGoldenrod, Color.White, EasingFunction.QuadraticBump(ratio));
    }

    private float GetTrailWidth2(float ratio)
    {
        return GetTrailWidth(ratio) * 1.6f;
    }

    private void DrawPixelatedLightning(GraphicsDevice gDevice)
    {
        List<Vector2> points = new List<Vector2>();
        float numPointsEachChain = 32;
        void AddPoints(Vector2 start, Vector2 end)
        {
            for(float f = 0; f < numPointsEachChain; f++)
            {
                float ratio = (f / numPointsEachChain);
                points.Add(Vector2.Lerp(start, end, ratio));
            }
        }

        void DrawLightningBolt(Vector2 start, Vector2 end)
        {
            points.Clear();
            float numPoints = Vector2.Distance(start, end) / 8f;
            numPoints += 2;
            for (float f = 0; f < numPoints; f++)
            {
                float ratio = (f / numPoints);
                points.Add(Vector2.Lerp(start, end, ratio));
            }
            Render(points.ToArray());
        }

        void Render(Vector2[] lightningPoints)
        {
            ZapLightningShader lightingShader = ZapLightningShader.Instance;
            lightingShader.Amplitude = 0.8f;

            float time = Main.GlobalTimeWrappedHourly * 16;
            float levels = 4;
            time = MathF.Floor(time * levels) / levels;
            lightingShader.Time = time;
            Asset<Texture2D> laserTexture = AssetManager.LaserTextures.TexturedLaser;
            lightingShader.LaserTexture = laserTexture;
            lightingShader.Noise = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/BlurryPerlinNoise").Value;
            lightingShader.Gradient = _gradientTextureAsset.Value;
            lightingShader.TransformMatrix = TrailDrawer.WorldViewPoint2;
            lightingShader.Levels = 64;
            lightingShader.Tiling = new Vector2(2f);
            TrailDrawer.Draw(Main.spriteBatch, lightningPoints, GetTrailColor, GetTrailWidth, lightingShader, Projectile.Size * 0.5f);

            BloomTrailShader bloom = BloomTrailShader.Instance;
            bloom.InnerColor = Color.Goldenrod;
            bloom.OuterColor = Color.DarkGoldenrod;
            TrailDrawer.Draw(Main.spriteBatch, lightningPoints, GetTrailColor2, GetTrailWidth2, bloom, Projectile.Size * 0.5f);
        }
        DrawLightningBolt(Projectile.Center, _hitPoint);
        for (int i = 0; i < _targets.Count; i++)
        {
            Vector2 prev;
            if (i == 0)
                prev = _hitPoint;
            else
                prev = _targets[i - 1];
            Vector2 next = _targets[i];
            DrawLightningBolt(prev, next);
        }

    }

    public override bool PreDraw(ref Color lightColor)
    {
        _gradientTextureAsset ??= ModContent.Request<Texture2D>(ModContent.GetInstance<ZapShockwave>().Texture + "_Gradient");
        PixelationManager.QueuePrimitivesDrawAction(DrawPixelatedLightning);
        SpritebatchDrawer sb = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, Projectile.Center);
        //Main.spriteBatch.Draw(sb);
        return false;
    }
}