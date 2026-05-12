using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Content.Areas.Snow.WeaponsSN;
using Stellamod.Content.Armors.Artisan;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Bases;
using Stellamod.Core.Palettes;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Utilities;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Items.Ores;
using Stellamod.Projectiles.Paint;
using Stellamod.Projectiles.Visual;
using Stellamod.Trails;
using Stellamod.Visual.Particles;
using System;
using System.Collections.Generic;
using System.Formats.Tar;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Hallowrooms.WeaponsHR;

public class PieceOfArtArtifact : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToArtifact();
        Item.damage = 90;
        Item.DamageType = DamageClass.Magic;
        Item.useTime = 100;
        Item.useAnimation = 100;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.knockBack = 2;
        Item.value = 10000;
        Item.noMelee = true;
        Item.rare = ItemRarityID.LightPurple;
        Item.UseSound = SoundID.Item1;
        Item.autoReuse = true;
        Item.shoot = ModContent.ProjectileType<PieceOfArtRainbow>();
        Item.shootSpeed = 20f;
        Item.noUseGraphic = true;
        Item.channel = true;
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        Projectile.NewProjectile(source, position, velocity, type, damage + player.GetModPlayer<ArtisanPlayer>().PPPaintDMG2, knockback, player.whoAmI);
        return false;
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew(
            mold: ModContent.ItemType<BlankStaff>(), 
            material: ModContent.ItemType<KaleidoscopicInk>());
    }
}

[Autoload(Side = ModSide.Client)]
public class PieceOfArtRenderer : ModSystem
{
    public struct BlobParticle
    {
        public BlobParticle(int length)
        {
            position = new Vector2[length];
            velocity = new Vector2[length];
            time = new float[length];
            active = new bool[length];
        }

        public Vector2[] position;
        public Vector2[] velocity;
        public bool[] active;
        public float[] time;
    }
    public delegate void DrawAction(GraphicsDevice gDevice);
    private Asset<Texture2D> _blobTextureAsset;
    private Queue<DrawAction> _drawActions;
    private ManagedRenderTarget _blobRT;
    private ManagedRenderTarget _maskRT;
    private BlobParticle _particles;
    public const int MAX_BLOB_COUNT = 400;
    public override void Load()
    {
        base.Load();
        _drawActions =new Queue<DrawAction>();
        _particles = new BlobParticle(MAX_BLOB_COUNT);
        On_Main.CheckMonoliths += Render;
    }


    public override void Unload()
    {
        base.Unload();
        _blobTextureAsset = null;
    }
    private void DrawDusts(SpriteBatch spriteBatch)
    {
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(_blobTextureAsset, Vector2.Zero);

        for (int i = 0; i < MAX_BLOB_COUNT; i++)
        {
            ref bool isActive = ref _particles.active[i];
            if (!isActive)
                continue;

            ref Vector2 position = ref _particles.position[i];

            int frame = (int)ExtraMath.Osc(0f, 4f, speed: 0, offset: i);
            drawer.VerticalFrame(frame, 4);
            drawer.CenterOrigin();
            drawer.worldPosition = position;
            float time = _particles.time[i];
            float ratio = time / 100f;
            drawer.color = Color.Lerp(Color.Transparent, Color.White, EasingFunction.InExpo(ratio));
          
            drawer.scale = Vector2.Lerp(Vector2.Zero, new Vector2(2f, 1f), EasingFunction.InOutSine(ratio));
            drawer.rotation = ratio * MathHelper.TwoPi + i;
            spriteBatch.Draw(drawer);
        }
        spriteBatch.End();
    }
    private void Render(On_Main.orig_CheckMonoliths orig)
    {
        if (!Main.gameMenu && _drawActions.Count > 0)
        {
            GraphicsDevice gDevice = Main.graphics.GraphicsDevice;
            SpriteBatch spriteBatch = Main.spriteBatch;
            gDevice.SetRenderTarget(_maskRT);
            gDevice.Clear(Color.Transparent);
            DrawDusts(spriteBatch);
 
            gDevice.SetRenderTarget(_blobRT);
            gDevice.Clear(Color.Transparent);


            while (_drawActions.Count > 0)
            {
                _drawActions.Dequeue()(gDevice);
            }

            PixelationManager.QueueSpritebatchDrawAction(DrawToScreen, DrawLayer.OverNPCsWithOutline);
        }

        orig();
    }


    private void DrawToScreen(SpriteBatch sb, Vector2 screenPos)
    {
     //   sb.Draw(_maskRT, Vector2.Zero, Color.White);
        var shader = PieceOfArtShader.Instance;
        shader.Blob = _maskRT;
        shader.Levels = 4;
        sb.Restart(effect: shader.Effect, blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);

        Color fogColor = Color.White;
        sb.Draw(_blobRT, Vector2.Zero, fogColor);
        sb.RestartDefaults();
   //     sb.Draw(_blobRT, Vector2.Zero, fogColor);
    }

    public override void OnModLoad()
    {
        base.OnModLoad();
        _blobRT = ManagedRenderTarget.New();
        _maskRT = ManagedRenderTarget.New();
        _blobTextureAsset = ModContent.Request<Texture2D>(ModContent.GetInstance<PieceOfArtArtifact>().Texture + "_Blob");
    }

    public override void PostUpdateDusts()
    {
        base.PostUpdateDusts();
        for(int i = 0; i < MAX_BLOB_COUNT; i++)
        {
            ref bool isActive = ref _particles.active[i];
            if (!isActive)
                continue;

            ref Vector2 position = ref _particles.position[i];
            ref Vector2 velocity = ref _particles.velocity[i];
            ref float time = ref _particles.time[i];

            position += velocity;
            velocity *= 0.99f;
            time--;
            if (time <= 0)
                isActive = false;
        }
    }

    public void SpawnBlob(Vector2 startPosition, Vector2 startVelocity, float timeLeft)
    {
        int indexToUse = 0;
        for(int i = 0; i < MAX_BLOB_COUNT; i++)
        {
            if (!_particles.active[i])
            {
                indexToUse = i;
                break;
            }
        }

        ref bool isActive = ref _particles.active[indexToUse];
        ref Vector2 position = ref _particles.position[indexToUse];
        ref Vector2 velocity = ref _particles.velocity[indexToUse];
        ref float time = ref _particles.time[indexToUse];

        position = startPosition;
        velocity = startVelocity;
        time = timeLeft;
        isActive = true;
    }

    public void QueueMaskDraw(DrawAction drawAction)
    {
        _drawActions.Enqueue(drawAction);
    }
}

public class PieceOfArtRainbow : ModProjectile
{

    private Player Owner => Main.player[Projectile.owner];
    private ref float Timer => ref Projectile.ai[0];
    private ref float DeathTimer => ref Projectile.ai[1];
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 32;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
    }

    public override void OnSpawn(IEntitySource source)
    {
        for (int i = 0; i < Projectile.oldPos.Length; i++)
        {
            Projectile.oldPos[i] = Projectile.position;
        }
    }
    public override void SetDefaults()
    {
        Projectile.height = 80;
        Projectile.width = 80;
        Projectile.penetrate = -1;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.friendly = true;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 4;
    }
    private bool ShouldConsumeMana()
    {
        // Should mana be consumed this frame?
        bool consume = Timer % 12 == 0 ;
        return consume;
    }

    public override void AI()
    {
     
        Timer++;
        if(Timer == 1)
        {
            SoundStyle castStyle = SoundID.Item28;
            castStyle.PitchVariance = 0.35f;
            SoundEngine.PlaySound(castStyle, Owner.position);
        }
        if(Timer % 1 == 0 && Main.netMode != NetmodeID.Server)
        {
            for(int i = 0; i < 2; i++)
            {
                Vector2 vel = Main.rand.NextVector2Circular(1, 1);
                vel -= Projectile.velocity.SafeNormalize(Vector2.Zero) * 1;
                ModContent.GetInstance<PieceOfArtRenderer>().SpawnBlob(Projectile.Center + Main.rand.NextVector2Circular(16, 16), vel, 100);
            }

        }

        if(Timer % 16 == 0)
        {
            Vector2 pos = Owner.Center;
            Vector2 dir = (Projectile.Center - pos);
            dir = dir.SafeNormalize(Vector2.Zero);
            pos += Main.rand.NextVector2Circular(32, 32);
            var sp = SparkleParticle.Spawn(pos + dir * 32, dir * 8);
            sp.color = Main.DiscoColor;
            sp.outerColor = Main.DiscoColor;
            sp.noTileCollide = true;
            sp.gravity = 0;
            sp.dampening = 0.05f;
            sp.Scale *= 0.2f;
        }
        if (Timer % 8 == 0)
        {
            Vector2 vel = -Projectile.velocity.SafeNormalize(Vector2.Zero) * 1;
             
            Vector2 pos = Projectile.Center + Main.rand.NextVector2Circular(16, 16);
            var fx = FXUtil.GlowStretch(pos, vel);
            fx.OuterGlowColor = new Color(Main.rand.NextFloat(0f, 255f), Main.rand.NextFloat(0f, 255f), Main.rand.NextFloat(0f, 255f));
            fx.VectorScale *= 0.5f;
        }
        // Update the Prism's behavior: project beams on frame 1, consume mana, and despawn if out of mana.
        if (Projectile.owner == Main.myPlayer && DeathTimer == 0)
        {
            Vector2 targetPosition = Projectile.Center.MoveTowards(Main.MouseWorld, 16);
            Projectile.velocity = (targetPosition - Projectile.Center);
            Projectile.netUpdate = true;

            // player.CheckMana returns true if the mana cost can be paid. Since the second argument is true, the mana is actually consumed.
            // If mana shouldn't consumed this frame, the || operator short-circuits its evaluation player.CheckMana never executes.
            bool manaIsAvailable = !ShouldConsumeMana() || Owner.CheckMana(Owner.HeldItem.mana, true, false);

            // The Prism immediately stops functioning if the player is Cursed (player.noItems) or "Crowd Controlled", e.g. the Frozen debuff.
            // player.channel indicates whether the player is still holding down the mouse button to use the item.
            bool stillInUse = Owner.channel && manaIsAvailable && !Owner.noItems && !Owner.CCed;

            // Spawn in the Prism's lasers on the first frame if the player is capable of using the item.
            if (stillInUse)
            {

            }

            // If the Prism cannot continue to be used, then destroy it immediately.
            else if (!stillInUse)
            {
                DeathTimer++;
            }
        } else
        {
            DeathTimer++;
            Projectile.velocity = ProjectileHelper.SimpleHomingVelocity(Projectile, Owner.Center) * MathHelper.Lerp(1f, 0f, EasingFunction.InSine(DeathTimer / 60f));
            if (DeathTimer >= 60)
            {
                Projectile.Kill();
            }
        }


        float rotation = Projectile.rotation;
        Owner.RotatedRelativePoint(Projectile.Center);

        if (Main.rand.NextBool(3))
        {
            Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);
            Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<PaintBlob3>(), speed * 2, 0, default(Color), 4f).noGravity = false;

        }

        if (Main.rand.NextBool(3))
        {

            Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);
            Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<PaintBlob5>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, default(Color), 4f).noGravity = false;

        }

        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 144;
        if (Main.rand.NextBool(3))
        {
            Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);
            Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<PaintBlob4>(), (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(19.0), 0, default(Color), 4f).noGravity = false;

        }

        Vector3 RGB = new(2.55f, 2.55f, 0.94f);
        // The multiplication here wasn't doing anything
        Lighting.AddLight(Projectile.Center, RGB.X, RGB.Y, RGB.Z);

        Owner.heldProj = Projectile.whoAmI;
        Owner.ChangeDir(Projectile.velocity.X < 0 ? -1 : 1);
        Owner.itemTime = 2;
        Owner.itemAnimation = 2;
        Owner.itemRotation = rotation * Owner.direction;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (Main.rand.NextBool(2))
        {
            Vector2 velocity = Main.rand.NextVector2Circular(16, 16);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ModContent.ProjectileType<SplashProj>(), 0, 0, Projectile.owner);
        }

        if (Main.rand.NextBool(2))
        {
            float speedXa = Main.rand.NextFloat(-35f, 35f);
            float speedYa = Main.rand.Next(-35, 35);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X + speedXa, Projectile.Center.Y + speedYa, 0, 0, ModContent.ProjectileType<PaintBomb1>(), (Projectile.damage / 2) + Owner.GetModPlayer<ArtisanPlayer>().PPPaintDMG2, 1, Projectile.owner, 0, 0);
            Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<PaintBlob3>(), (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(19.0), 0, default(Color), 4f).noGravity = false;
        }

        if (Main.rand.NextBool(1))
        {
            float speedXa = Main.rand.NextFloat(-35f, 35f);
            float speedYa = Main.rand.Next(-35, 35);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X + speedXa, Projectile.Center.Y + speedYa, 0, 0, ModContent.ProjectileType<PaintBomb2>(), (Projectile.damage / 4) + Owner.GetModPlayer<ArtisanPlayer>().PPPaintDMG2, 1, Projectile.owner, 0, 0);
        }

        if (Main.rand.NextBool(4))
        {
            float speedXa = Main.rand.NextFloat(-35f, 35f);
            float speedYa = Main.rand.Next(-35, 35);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X + speedXa, Projectile.Center.Y + speedYa, 0, 0, ModContent.ProjectileType<PaintBomb3>(), (Projectile.damage * 5) + Owner.GetModPlayer<ArtisanPlayer>().PPPaintDMG2, 1, Projectile.owner, 0, 0);
            Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<PaintBlob2>(), (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(19.0), 0, default(Color), 4f).noGravity = false;
        }


        if (Main.rand.NextBool(4))
        {
            float speedXa = Main.rand.NextFloat(-35f, 35f);
            float speedYa = Main.rand.Next(-35, 35);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X + speedXa, Projectile.Center.Y + speedYa, 0, 0, ModContent.ProjectileType<PaintBomb5>(), (Projectile.damage + Owner.GetModPlayer<ArtisanPlayer>().PPPaintDMG2) / 2, 1, Projectile.owner, 0, 0);
            Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<PaintBlob2>(), (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(19.0), 0, default(Color), 4f).noGravity = false;
        }

        if (Main.rand.NextBool(4))
        {
            float speedXa = Main.rand.NextFloat(-35f, 35f);
            float speedYa = Main.rand.Next(-35, 35);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X + speedXa, Projectile.Center.Y + speedYa, 0, 0, ModContent.ProjectileType<PaintBomb4>(), Projectile.damage + Owner.GetModPlayer<ArtisanPlayer>().PPPaintDMG2, 1, Projectile.owner, 0, 0);
            Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<PaintBlob2>(), (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(19.0), 0, default(Color), 4f).noGravity = false;
        }

        if (Main.rand.NextBool(4))
        {
            float speedXa = Main.rand.NextFloat(-35f, 35f);
            float speedYa = Main.rand.Next(-35, 35);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X + speedXa, Projectile.Center.Y + speedYa, 0, 0, ModContent.ProjectileType<PaintBomb6>(), (Projectile.damage + Owner.GetModPlayer<ArtisanPlayer>().PPPaintDMG2) / 3, 1, Projectile.owner, 0, 0);
            Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<PaintBlob2>(), (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(19.0), 0, default(Color), 4f).noGravity = false;
        }
        if (Owner.GetModPlayer<ArtisanPlayer>().PPPaintI)
        {
            if (Main.rand.NextBool(4))
            {
                float speedXa = Main.rand.NextFloat(-35f, 35f);
                float speedYa = Main.rand.Next(-35, 35);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X + speedXa, Projectile.Center.Y + speedYa, 0, 0, ModContent.ProjectileType<PaintBomb7>(), (Projectile.damage + Owner.GetModPlayer<ArtisanPlayer>().PPPaintDMG2) * 4, 1, Projectile.owner, 0, 0);
                Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<PaintBlob5>(), (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(19.0), 0, default(Color), 4f).noGravity = false;
            }
        }
        if (Owner.GetModPlayer<ArtisanPlayer>().PPPaintII)
        {
            if (Main.rand.NextBool(7))
            {
                float speedXa = Main.rand.NextFloat(-35f, 35f);
                float speedYa = Main.rand.Next(-35, 35);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X + speedXa, Projectile.Center.Y + speedYa, 0, 0, ModContent.ProjectileType<PaintBomb8>(), (Projectile.damage + Owner.GetModPlayer<ArtisanPlayer>().PPPaintDMG2) * 3, 1, Projectile.owner, 0, 0);
                Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<PaintBlob1>(), (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(19.0), 0, default(Color), 4f).noGravity = false;
            }
        }
    }


    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        //This damages everything in the trail
        Vector2[] positions = Projectile.oldPos;
        float collisionPoint = 0;
        for (int i = 1; i < positions.Length; i++)
        {
            Vector2 position = positions[i];
            Vector2 previousPosition = positions[i - 1];
            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), position, previousPosition, 6, ref collisionPoint))
                return true;
        }
        return base.Colliding(projHitbox, targetHitbox);
    }


    private Color GetTrailColor(float completionRatio)
    {
        return Color.Lerp(Color.White, Color.White * 0.33f, EasingFunction.OutExpo(completionRatio / 0.25f));
        return Color.White * 0.4f;
       // return Color.White;
    }
    private Color GetTrailColor2(float completionRatio)
    {
        return Color.Lerp(Color.Black, Color.Transparent, EasingFunction.OutExpo(completionRatio / 0.5f));
        // return Color.White;
    }
    private float GetTrailWidth(float completionRatio)
    {
        return MathHelper.SmoothStep(16, 80, EasingFunction.QuadraticBump(completionRatio)) * MathHelper.Lerp(1f, 0f, EasingFunction.InSine(DeathTimer / 60f));
    }
    private float GetTrailWidth2(float completionRatio)
    {
        return GetTrailWidth(completionRatio) * 1.6f;
    }
    private void DrawTrail(GraphicsDevice gDevice)
    {
        
        var shader = HairShader.Instance;
        shader.LaserTexture = TextureAssets.Projectile[Type];
        shader.Time = Main.GlobalTimeWrappedHourly * 0.4f;
        shader.WaveFrequency = 8;
        shader.WaveAmplitude = 0.05f;
        shader.XOffset = 12;
        TrailDrawer.Draw(Main.spriteBatch, Projectile.oldPos, GetTrailColor, GetTrailWidth, shader, Projectile.Size * 0.5f);


    }
    private void DrawTrail2(GraphicsDevice gDevice)
    {
        
        var shader = BasicLaserAlphaShader.Instance;
        shader.LaserTexture = TextureAssets.Projectile[Type];
        shader.Time = Main.GlobalTimeWrappedHourly * 0.2f;

        TrailDrawer.Draw(Main.spriteBatch, Projectile.oldPos, GetTrailColor2, GetTrailWidth2, shader, Projectile.Size * 0.5f);
        

    }

    private MagicCircleRenderer _magicCircleRenderer;
    public override bool PreDraw(ref Color lightColor)
    {
        Vector2 diff = Projectile.velocity;
        float rot = diff.ToRotation();
        float outScale = MathHelper.Lerp(1f, 0f, DeathTimer / 60f);
        _magicCircleRenderer ??= new MagicCircleRenderer(AssetManager.GlowMask.MagicCircle2);
        Vector2 velocity = (Projectile.Center - Owner.Center).SafeNormalize(Vector2.Zero);
        Color fadeColor = Color.Lerp(Color.White, Main.DiscoColor, 0.5f);
        fadeColor *= outScale;
        _magicCircleRenderer.DrawRing(Owner.Center + velocity * 32, velocity, 0, 1, fadeColor, Main.GlobalTimeWrappedHourly * 4);


        SpritebatchDrawer castDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, Owner.Center + velocity * 32);
        castDrawer.scale *= 0.5f * outScale;
        castDrawer.scale.Y *= 1;
        castDrawer.scale.X *= 0.25f;
        castDrawer.rotation = (Projectile.Center - Owner.Center).ToRotation();
        castDrawer.color = Main.DiscoColor * 0.5f * ExtraMath.Osc(0.5f, 1f, speed: 6);
        castDrawer.color.A = 0;
        Main.spriteBatch.Draw(castDrawer);


        ModContent.GetInstance<PieceOfArtRenderer>().QueueMaskDraw(DrawTrail);
     //   PixelationManager.QueuePrimitivesDrawAction(DrawTrail2, DrawLayer.BehindNPCsWithOutline);



 
        SpritebatchDrawer glowDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, Projectile.Center);
        glowDrawer.scale *= 0.12f * outScale;
        glowDrawer.worldPosition -= rot.ToRotationVector2() * 48;
        glowDrawer.scale.Y *= 2;
        glowDrawer.scale.X *= 0.25f;
        glowDrawer.rotation = rot;
        glowDrawer.color = Main.DiscoColor * 0.5f * ExtraMath.Osc(0.5f, 1f, speed: 6);
        glowDrawer.color.A = 0;
        Main.spriteBatch.Draw(glowDrawer);

        glowDrawer.scale *= 0.8f;
        glowDrawer.color = Color.White;
        glowDrawer.color.A = 0;
        Main.spriteBatch.Draw(glowDrawer);
        SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(TextureAssets.Item[ModContent.GetInstance<PieceOfArtArtifact>().Type], Projectile.Center);

        drawer.scale *= outScale;
        drawer.rotation = rot + MathHelper.PiOver4;
        drawer.worldPosition -= rot.ToRotationVector2() * 20;
        drawer.rotation += MathHelper.Pi;
        Main.spriteBatch.Draw(drawer);

        drawer.color *= ExtraMath.Osc(0f, 0.5f, speed: 12);
        drawer.color.A = 0;
        Main.spriteBatch.Draw(drawer);
        return false;
    }
}