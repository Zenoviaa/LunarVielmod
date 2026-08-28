using Microsoft.Xna.Framework.Input;
using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.Players;
using Stellamod.Common.Shaders;
using Stellamod.Core.Bases;
using Stellamod.Core.Effects;
using Stellamod.Core.Pixelation;
using Stellamod.Effects.RoyalMagic;
using Stellamod.Visual.Particles;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Stellamod.Core.SwingSystem;

public struct Bloom
{
    public Color innerBloomColor;
    public Color outerBloomColor;
    public Func<float, Color> bloomColorFunction;
    public Func<float, float> bloomWidthFunction;

    public Color GetBloomColor(float ratio)
    {
        if (bloomColorFunction == null)
            return Color.White;
        return bloomColorFunction(ratio);
    }

    public float GetBloomWidth(float ratio)
    {
        if (bloomWidthFunction == null)
            return 64;
        return bloomWidthFunction(ratio);
    }
}

public class MeleeEffectsPlayer : ModPlayer
{
    public List<AbstractMeleeAddon> addons = new List<AbstractMeleeAddon>();
    public int safunaiChainBonus;
    public int steinWordBonus;
    public float steinDistanceBonus;
    public float noProjectionDamageBonus;
    public float projectionOnlyDamageBonus;
    public bool noOwnerHitCheck;
    public bool smokyPendant;
    public bool superExtender;


    public override void ResetEffects()
    {
        base.ResetEffects();
        noOwnerHitCheck = false;
        safunaiChainBonus = 0;
        steinDistanceBonus = 0f;
        noProjectionDamageBonus = 0f;
        projectionOnlyDamageBonus = 0f;
        steinWordBonus = 0;
        smokyPendant = false;
        superExtender = false;
        addons ??= new List<AbstractMeleeAddon>();
        addons.Clear();
    }
}

public abstract class AbstractMeleeAddon : ModItem
{
    public sealed override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToAccessory();
    }
    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        base.UpdateAccessory(player, hideVisual);
        player.GetModPlayer<MeleeEffectsPlayer>().addons.Add(this);
    }

    public virtual void DefineCombo(BaseSwingProjectileV2 projectile) { }
    public virtual void OnSpawn(BaseSwingProjectileV2 projectile) { }
    public virtual void AI(BaseSwingProjectileV2 projectile) { }
    public virtual void OnModifyHitNPC(BaseSwingProjectileV2 projectile, NPC target, ref NPC.HitModifiers modifiers) { }
    public virtual void OnHitNPC(BaseSwingProjectileV2 projectile, NPC target, NPC.HitInfo hit, int damageDone) { }
    public virtual void PreDrawEffects(BaseSwingProjectileV2 projectile, ref Color lightColor) { }
    public virtual void PostDrawEffects(BaseSwingProjectileV2 projectile, ref Color lightColor) { }
}


public abstract class BaseSwingProjectileV2 : ScarletProjectile,
    ISwingProjectile
{
    public static int SwingTrailCacheLength => 128;
    public static int AfterImageCacheLength => 16;

    private Vector2 _hitboxStart;
    private Vector2 _hitboxProjectionEnd;
    private Vector2 _hitboxSwordEnd;

    private bool _hasInitializedRendering;
    private bool _hasInitialized;
    private bool _canHurtThings;
    private bool _hasHitStop;
    private float _fade;
    private List<ISwing> _swings;

    public ITrailer Trailer { get; set; }
    public ref float Timer => ref Projectile.ai[0];
    public ref float SwingDirection => ref Projectile.ai[1];
    public int ComboIndex => (int)Projectile.ai[2];

    public float HitstopTimer;
    public int ComboCount => _swings.Count;
    public float ComboProgress
    {
        get
        {
            return (ComboIndex + 1) / (float)ComboCount;
        }
    }
    public float Interpolant { get; private set; }
    public float EasedInterpolant { get; set; }
    public Vector2[] afterImageCache;
    public Vector2[] swingTrailCache;
    public Vector2[] bigSwingTrailCache;
    public float[] swingRotationCache;
    public float[] oldTime;
    public int hitStopTime;
    public bool useAfterImage;
    public Color glowColor;
    public float growScale;
    public float swordBeamLength;
    public float swingTime;
    public float bounceTimer;
    public float extraLength;
    public Color outlineColor;
    public Color glowAfterImageColor;
    public bool drawCentered;
    public bool isChildProjectile;
    public bool isAfterImageProjectile;
    public bool isStaminaMove;
    public bool additive;
    public float bigSwingTrailOffset;
    public float? trailOffsetOverride;
    public float swordRotation;
    public bool useBloom;
    public Bloom bloom;
    public float trailVisibilityOffset;

    public float bloomScale;
    public float bigTrailAlpha;
    public const int EXTRA_UPDATE_COUNT = 7;
    public float Size { get; private set; }
    public MeleeWeaponType MeleeWeaponType
    {
        get
        {
            var swingItem = Owner.HeldItem.ModItem as BaseSwingItemV2;
            if (swingItem == null)
                return MeleeWeaponType.Sword;
            return swingItem.meleeWeaponType;
        }
    }
    //Default to the item sprite of the texture, we can just predraw if we need to change it
    public override string Texture => TextureRegistry.EmptyTexture;
    private MeleeEffectsPlayer MeleeEffectsPlayer => Owner.GetModPlayer<MeleeEffectsPlayer>();
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();

    }

    public sealed override void SetDefaults()
    {
        base.SetDefaults();
        TrailCacheLength = 8;
        Projectile.width = 32;
        Projectile.height = 32;
        Projectile.friendly = true;
        Projectile.timeLeft = 7200;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.ignoreWater = true;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = -1;
        Projectile.DamageType = DamageClass.Melee;
        Projectile.ownerHitCheck = true;
        trailVisibilityOffset = 0.3f;
        bigTrailAlpha = 0.35f;

        //We're using extra updates to ensure the sword doesn't just pass through things
        Projectile.extraUpdates = EXTRA_UPDATE_COUNT - 1;
        hitStopTime = EXTRA_UPDATE_COUNT * 2;
        SetDefaults2();
    }

    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.Write(bounceTimer);
        writer.Write(isAfterImageProjectile);
        writer.Write(isStaminaMove);
    }

    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        bounceTimer = reader.ReadSingle();
        isAfterImageProjectile = reader.ReadBoolean();
        isStaminaMove = reader.ReadBoolean();
    }



    public virtual Asset<Texture2D> RequestHologramTexture()
    {
        return TextureRegistry.GlowSword_Sword;
    }

    public virtual void SetDefaults2()
    {

    }

    public virtual void DefineCombo()
    {

    }

    public void MakeFinisher(ISwing swing)
    {
        _swings[_swings.Count - 1] = swing;
    }

    public bool IsFinishingSwing()
    {
        //If we haven't initialized then yeah, though that won't happen lol
        if (!_hasInitialized)
            return false;
        return ComboIndex == _swings.Count - 1;
    }

    public virtual Color GetAfterImageColor(float interpolant)
    {
        return Color.Lerp(Color.White, Color.Transparent, MathHelper.SmoothStep(0, 1, interpolant));
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        if (_hasInitialized)
        {
            //Return arrays
            ArrayPool<Vector2>.Shared.Return(swingTrailCache);
            ArrayPool<Vector2>.Shared.Return(bigSwingTrailCache);
            ArrayPool<Vector2>.Shared.Return(afterImageCache);
            ArrayPool<float>.Shared.Return(swingRotationCache);
            ArrayPool<float>.Shared.Return(oldTime);
        }
    }


    public override void OnSpawn(IEntitySource source)
    {
        base.OnSpawn(source);
        foreach (var addon in MeleeEffectsPlayer.addons)
        {
            addon.OnSpawn(this);
        }
    }

    private void AI_Initialize()
    {
        if (!_hasInitialized)
        {
            _swings = new List<ISwing>();

            //Rent arrays so we're not constantly allocating new ones
            int cacheLength = SwingTrailCacheLength;
            int afterImageCacheLength = AfterImageCacheLength;
            swingTrailCache = ArrayPool<Vector2>.Shared.Rent(cacheLength);
            bigSwingTrailCache = ArrayPool<Vector2>.Shared.Rent(cacheLength);
            afterImageCache = ArrayPool<Vector2>.Shared.Rent(afterImageCacheLength);
            swingRotationCache = ArrayPool<float>.Shared.Rent(afterImageCacheLength);
            oldTime = ArrayPool<float>.Shared.Rent(cacheLength);
            DefineCombo();
            foreach (var addon in MeleeEffectsPlayer.addons)
            {
                addon.DefineCombo(this);
            }
            ISwing swing = GetSwing();
            swing.SetDirection((int)SwingDirection);
            float hitCount = swing.GetHitCount();
            if (hitCount > 1)
            {
                float duration = swing.GetDuration(1f / Owner.GetTotalAttackSpeed(Projectile.DamageType)) / hitCount;
                duration *= EXTRA_UPDATE_COUNT - 1;
                Projectile.localNPCHitCooldown = (int)duration;
            }
            Projectile.ResetLocalNPCHitImmunity();
            _hasInitialized = true;
        }
    }

    public virtual void Init_Rendering()
    {

    }

    private ISwing GetSwing()
    {
        if (_swings.Count > ComboIndex)
        {
            return _swings[ComboIndex];
        }

        //Main.NewText($"Somehow doing the deafult swing");
        return _swings[0];
    }

    public float GetSwingTime(float baseSwingTime)
    {
        float swingTime = baseSwingTime * EXTRA_UPDATE_COUNT;
        return (int)(swingTime);
    }

    public override bool? CanDamage()
    {
        //Only damage in the mid part of the swing
        return _canHurtThings;
    }
    public override bool ShouldUpdatePosition()
    {
        return false;
    }


    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {

        //Check if the sword is colliding, this does a line check instead of terraria default box.

        float collisionPoint = 0f;
        bool check = Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(),
            _hitboxStart, _hitboxProjectionEnd, 16, ref collisionPoint);
        return check;
    }

    public bool IsThrust()
    {
        ISwing swing = GetSwing();
        if (swing == null)
            return false;

        return swing is ThrustSwing;
    }

    public bool IsTouchingSword(Rectangle hitbox)
    {
        float collisionPoint = 0f;
        bool isTouchingSword = Collision.CheckAABBvLineCollision(hitbox.TopLeft(), hitbox.Size(),
            _hitboxStart, _hitboxSwordEnd, 16, ref collisionPoint);
        return isTouchingSword;
    }

    public bool IsTouchingONLYProjection(Rectangle hitbox)
    {
        if (swordBeamLength <= 0)
            return false;
        float collisionPoint = 0f;
        bool isTouchingSword = Collision.CheckAABBvLineCollision(hitbox.TopLeft(), hitbox.Size(),
            _hitboxSwordEnd, _hitboxProjectionEnd, 16, ref collisionPoint);
        return isTouchingSword;
    }



    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        base.ModifyHitNPC(target, ref modifiers);

        Rectangle hitbox = target.getRect();
        if (IsTouchingSword(hitbox))
        {
            modifiers.FinalDamage += MeleeEffectsPlayer.noProjectionDamageBonus;
            var sp = SparkleParticle.Spawn(target.Center, Vector2.Zero);
            sp.fast = true;
            sp.gravity = 0;
            sp.Scale *= 0.6f;
            sp.color *= 0.7f;
        }

        if (IsTouchingONLYProjection(hitbox))
        {

            modifiers.FinalDamage += MeleeEffectsPlayer.projectionOnlyDamageBonus;
            var sp = MoonSpiralParticle.Spawn(target.Center, Vector2.Zero);
            sp.gravity = 0;
            sp.Scale *= 0.6f;
            sp.color *= 0.7f;
            sp.fast = true;

            FXUtil.GlowStretch(target.Center, Main.rand.NextVector2Circular(1, 1));
        }

        foreach (var addon in MeleeEffectsPlayer.addons)
        {
            addon.OnModifyHitNPC(this, target, ref modifiers);
        }
    }

    private void UpdateHitbox()
    {
        if (isAfterImageProjectile)
            swordBeamLength = 0;
        Texture2D texture = GetTexture();
        float swordLength = texture.Width / 2 + texture.Height / 2;
        Size = swordLength;
        float edgeLength = swordLength;
        edgeLength *= 1.6f;
        edgeLength += swordBeamLength;
        edgeLength += extraLength;
        if (MeleeEffectsPlayer.superExtender)
            edgeLength += 152;
        float rotation = Projectile.rotation;
        rotation -= MathHelper.PiOver4;

        Vector2 rotationVec = rotation.ToRotationVector2();
        _hitboxStart = Projectile.Center - rotationVec * swordLength;
        _hitboxSwordEnd = Projectile.Center + rotationVec * swordLength;
        _hitboxProjectionEnd = Projectile.Center + rotationVec * edgeLength / 1.6f;
    }

    public override void AI()
    {
        base.AI();
        UpdateHitbox();
        if (MeleeEffectsPlayer.noOwnerHitCheck)
            Projectile.ownerHitCheck = false;

        //We want to initalize like this for better MP compatibility, using a timer might not always be seen on all clients
        AI_Initialize();
        Owner.GetModPlayer<SwingPlayerV2>().MaxCombo = _swings.Count;
        if (bounceTimer > 0)
        {
            Timer--;
            bounceTimer--;
        }
        else if (HitstopTimer <= 0)
            Timer++;
        else
            HitstopTimer--;

        foreach (AbstractMeleeAddon addon in MeleeEffectsPlayer.addons)
        {
            addon.AI(this);
        }

        //SmokyPendantEffect();
        ISwing swing = GetSwing();

        //Now we need to calculate the time/interpolant for this swinging
        if (swingTime == 0)
        {
            float duration = swing.GetDuration(1f / Owner.GetTotalAttackSpeed(Projectile.DamageType));
            swingTime = GetSwingTime(duration);
        }


        Interpolant = Timer / swingTime;
        Interpolant = MathHelper.Clamp(Interpolant, 0f, 1f);
        for (int i = oldTime.Length - 1; i > 0; i--)
        {
            oldTime[i] = oldTime[i - 1];
        }
        oldTime[0] = Interpolant;
        if (_fade < 1f)
        {
            _fade += 0.1f;
        }
        if (isAfterImageProjectile)
            if (_fade > 0.5f)
                _fade = 0.5f;
        _canHurtThings = swing.CanHurt(this);

        //For the purposes of netcode,
        //Killing the projectile manually instead of trying to sync time left is better I think.
        if (Timer >= swingTime)
        {
            Projectile.Kill();
        }

        //We now have the offset so we can apply that to the weapon
        drawCentered = false;
        swing.UpdateSwing(this);

        //Set the position of the hand for the swing
        AI_OrientHand();

        //Calculate the trailing
        swing.CalculateTrailingPoints(this);
        swing.CalculateAfterImagePoints(this);
        Matrix translationMatrix = Matrix.CreateTranslation(new Vector3(Owner.Center.X, Owner.Center.Y, 0));
        //Now we transform the points
        //Calculating points locally and then translating it is a bit simpler.

        for (int t = 0; t < swingTrailCache.Length; t++)
        {
            ref Vector2 point = ref swingTrailCache[t];
            point = Vector2.Transform(point, translationMatrix);
        }

        Vector2 normal = Projectile.velocity.SafeNormalize(Vector2.Zero);
        Vector2 bigO = normal * bigSwingTrailOffset;
        for (int t = 0; t < bigSwingTrailCache.Length; t++)
        {
            ref Vector2 point = ref bigSwingTrailCache[t];
            point = Vector2.Transform(point, translationMatrix);
            point += bigO;
        }
    }

    private void AI_OrientHand()
    {

        float rotation = Projectile.rotation;
        Owner.ChangeDir(Projectile.direction);
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

    }

    public virtual void PrepareTrailShader()
    {

    }
    private void DrawPixelatedSwingTrails(GraphicsDevice graphicsDevice)
    {

        PrepareTrailShader();
        Color lightColor = Color.White;
        RenderSwingTrail(ref lightColor, swingTrailCache);
        DrawSwingTrail(ref lightColor, swingTrailCache);
        DrawSwingTrail2(ref lightColor, bigSwingTrailCache);
        DrawAngelSwingTrail(ref lightColor, bigSwingTrailCache);
    }
    public virtual void RenderSwingTrail(ref Color lightColor, Vector2[] points)
    {

    }

    public void DrawBloom(GraphicsDevice gDevice)
    {
        Color lightColor = Color.White;
        RenderBloom(ref lightColor, swingTrailCache);
    }
    public void RenderBloom(ref Color lightColor, Vector2[] points)
    {
        BloomTrailShader bloomTrailShader = BloomTrailShader.Instance;
        bloomTrailShader.InnerColor = bloom.innerBloomColor;
        bloomTrailShader.OuterColor = bloom.outerBloomColor;
        TrailDrawer.Draw(Main.spriteBatch, points, bloom.GetBloomColor, bloom.GetBloomWidth, bloomTrailShader);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        if (!_hasInitializedRendering)
        {
            Init_Rendering();
            _hasInitializedRendering = true;
        }

        if (Timer <= 3)
            return false;
        foreach (var addon in MeleeEffectsPlayer.addons)
            addon.PreDrawEffects(this, ref lightColor);
        //Draw the texture, by 
        if (useAfterImage)
            DrawAfterImage(ref lightColor, OldCenterPos);
        PixelationManager.QueuePrimitivesDrawAction(DrawPixelatedSwingTrails, DrawLayer.OverNPCs);
        if (additive)
        {
            PixelationManager.QueuePrimitivesDrawAction(DrawPixelatedSwingTrails, DrawLayer.OverNPCsAdditive);
        }
        if (useBloom)
        {
            PixelationManager.QueuePrimitivesDrawAction(DrawBloom, DrawLayer.OverNPCs);
        }

        DrawAngelSwordBeam(ref lightColor);
        DrawSwordBeam(ref lightColor);
        DrawSwordSprite(ref lightColor);
        foreach (var addon in MeleeEffectsPlayer.addons)
            addon.PostDrawEffects(this, ref lightColor);
        return false;
    }

    private void DebugDrawHitboxCheck()
    {
        Primitives2D.DrawLine(Main.spriteBatch, _hitboxStart - Main.screenPosition, _hitboxSwordEnd - Main.screenPosition, Color.Green);
    }
    private void DebugDrawHitboxCheck2()
    {
        Primitives2D.DrawLine(Main.spriteBatch, _hitboxStart - Main.screenPosition, _hitboxProjectionEnd - Main.screenPosition, Color.Blue);

    }
    public void AfterImageProjectile()
    {
        if (isChildProjectile)
            return;

        if (Main.myPlayer == Projectile.owner)
        {
            ComboPlayer comboPlayer = Owner.GetModPlayer<ComboPlayer>();
            int combo = ComboIndex;
            float dir = SwingDirection;
            var p = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.position, Projectile.velocity,
                Type, (int)(Projectile.damage * 0.5f), Projectile.knockBack,
                           Owner.whoAmI, ai2: combo, ai1: dir);
            BaseSwingProjectileV2 swingProj = p.ModProjectile as BaseSwingProjectileV2;
            swingProj.isChildProjectile = true;
            swingProj.isAfterImageProjectile = true;
            swingProj.isStaminaMove = isStaminaMove;
            swingProj.Projectile.netUpdate = true;
        }
    }
    public void TrueCloneProjectile()
    {
        if (isChildProjectile)
            return;

        if (Main.myPlayer == Projectile.owner)
        {
            ComboPlayer comboPlayer = Owner.GetModPlayer<ComboPlayer>();
            int combo = ComboIndex;
            int dir = comboPlayer.ComboDirection;
            var p = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.position, Projectile.velocity,
                Type, Projectile.damage, Projectile.knockBack,
                           Owner.whoAmI, ai2: combo, ai1: dir);
            BaseSwingProjectileV2 swingProj = p.ModProjectile as BaseSwingProjectileV2;
            swingProj.isChildProjectile = true;

        }
    }
    public void CloneProjectile()
    {
        if (isChildProjectile)
            return;

        if (Main.myPlayer == Projectile.owner)
        {
            ComboPlayer comboPlayer = Owner.GetModPlayer<ComboPlayer>();
            int combo = ComboIndex + 1;
            int dir = comboPlayer.ComboDirection;
            var p = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.position, Projectile.velocity,
                Type, Projectile.damage, Projectile.knockBack,
                           Owner.whoAmI, ai2: combo, ai1: dir);
            BaseSwingProjectileV2 swingProj = p.ModProjectile as BaseSwingProjectileV2;
            swingProj.isChildProjectile = true;
        }
    }
    public void MirrorProjectile()
    {
        if (isChildProjectile)
            return;

        if (Main.myPlayer == Projectile.owner)
        {
            ComboPlayer comboPlayer = Owner.GetModPlayer<ComboPlayer>();
            int combo = ComboIndex;
            int dir = -(int)SwingDirection;
            var p = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.position, Projectile.velocity,
                Type, Projectile.damage, Projectile.knockBack,
                           Owner.whoAmI, ai2: combo, ai1: dir);
            BaseSwingProjectileV2 swingProj = p.ModProjectile as BaseSwingProjectileV2;
            swingProj.isChildProjectile = true;
        }
    }
    public void Bounce(float bounceTicks)
    {
        Projectile.ResetLocalNPCHitImmunity();
        bounceTimer += bounceTicks * EXTRA_UPDATE_COUNT;
        Projectile.netUpdate = true;
    }

    public Vector2 CalculateTrailOffset()
    {
        return Vector2.Zero;
    }
    public float GetTrailMultiplier()
    {
        Texture2D texture = GetTexture();
        Vector2 center = texture.Size() / 2f;
        Vector2 tip = new Vector2(texture.Width, 0);
        float distance = Vector2.Distance(center, tip);
        float worldDistance = distance / 16f / 2f;
        return worldDistance;
    }
    public float GetTrailCenterMultiplier()
    {
        Texture2D texture = GetTexture();
        Vector2 center = texture.Size() / 2f;
        Vector2 tip = new Vector2(texture.Width, 0);
        float distance = Vector2.Distance(center, tip);
        float worldDistance = distance / 2f;
        return worldDistance;
    }
    public virtual Texture2D GetTexture()
    {
        Texture2D texture = TextureAssets.Item[Owner.HeldItem.type].Value;
        return texture;
    }

    public virtual void DrawAfterImage(ref Color lightColor, Vector2[] afterImageCache)
    {
        if (afterImageCache == null)
            return;

        SpriteBatch spriteBatch = Main.spriteBatch;
        // spriteBatch.Restart(blendState: BlendState.Additive);
        for (int a = 0; a < afterImageCache.Length; a++)
        {
            float interpolant = a;
            interpolant /= afterImageCache.Length;
            Texture2D texture = GetTexture();

            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;

            Rectangle sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);
            Vector2 origin = sourceRectangle.Size() / 2f;
            Color drawColor = GetAfterImageColor(interpolant);
            drawColor *= EasingFunction.QuadraticBump(interpolant);
            drawColor *= 0.5f;
            float drawScale = 1.15f + growScale;
            Vector2 position = afterImageCache[a];
            float drawRotation = (position - Owner.Center).ToRotation() + MathHelper.PiOver4;



            SpriteEffects spriteEffects = SpriteEffects.None;
            if (SwingDirection == 1)
            {
                spriteEffects = SpriteEffects.FlipVertically;
                drawRotation -= MathHelper.PiOver2;
            }


            spriteBatch.Draw(texture,
              position - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
                sourceRectangle, drawColor, drawRotation, origin, drawScale, spriteEffects, 0); // drawing the sword itself
        }
        //     spriteBatch.RestartDefaults();
    }

    public virtual void DrawSwingTrail(ref Color lightColor, Vector2[] swingTrailCache)
    {
        if (Trailer == null)
            return;

        //I think it makes the most sense to abstract our trails out to a trailer and shader cache,
        //so we can just replace the trailer for different trails!
        //So much simpler, and we can just make new trailers
        var oldColorFunc = Trailer.TrailColorFunction;
        Color GetTrailColor(float interpolant)
        {
            return oldColorFunc(interpolant) * EasingFunction.QuadraticBump(Interpolant) * _fade;
        }
        Trailer.TrailColorFunction = GetTrailColor;
        Trailer?.DrawTrail(ref lightColor, swingTrailCache);
        Trailer.TrailColorFunction = oldColorFunc;
    }
    public virtual void DrawSwingTrail2(ref Color lightColor, Vector2[] swingTrailCache)
    {
        if (Trailer == null)
            return;


        if (swordBeamLength <= 0)
            return;
        //I think it makes the most sense to abstract our trails out to a trailer and shader cache,
        //so we can just replace the trailer for different trails!
        //So much simpler, and we can just make new trailers
        var oldWidthFunc = Trailer.TrailWidthFunction;
        var oldColorFunc = Trailer.TrailColorFunction;
        float GetTrailWidth(float interpolant)
        {
            return oldWidthFunc(interpolant) * 2;
        }
        Color GetTrailColor(float interpolant)
        {
            return oldColorFunc(interpolant) * bigTrailAlpha * EasingFunction.QuadraticBump(Interpolant) * _fade;
        }

        Trailer.TrailWidthFunction = GetTrailWidth;
        Trailer.TrailColorFunction = GetTrailColor;
        Trailer?.DrawTrail(ref lightColor, swingTrailCache);



        Trailer.TrailWidthFunction = oldWidthFunc;
        Trailer.TrailColorFunction = oldColorFunc;
    }

    public void DrawAngelSwingTrail(ref Color lightColor, Vector2[] swingTrailCache)
    {
        if (!MeleeEffectsPlayer.superExtender)
            return;
        AlcadSlashShader shader = ShaderContent.GetInstance<AlcadSlashShader>();
        shader.ScrollingLaser = TrailRegistry.Beamlight.Value;
        shader.Noise = AssetManager.Noise.Whirly.Value;
        shader.Slash = AssetManager.GlowMask.SwordSlash.Value;
        shader.BloomColor = Color.Gold;
        shader.Time = Main.GlobalTimeWrappedHourly * 24;
        shader.TransformMatrix = TrailDrawer.WorldViewPoint2;
        shader.Distortion = 0.15f;
        Color GetTrailColor(float completionRatio)
        {
            Color glowColor = DrawUtilities.InterpolateColorArray(ExtraMath.Osc(0f, 1f, speed: 4), Color.Gold, Color.White, Color.Orange, Color.DarkGoldenrod);
            Color trailColor = Color.Lerp(glowColor, Color.Orange, completionRatio)
                * MathHelper.Lerp(0f, 1f, EasingFunction.InCirc(completionRatio));
            return glowColor * ExtraMath.Osc(1f, 2f, speed: 32) * 0.6f;
        }
        Color GetTrailColor2(float completionRatio)
        {
            Color glowColor = DrawUtilities.InterpolateColorArray(ExtraMath.Osc(0f, 1f, speed: 4), Color.Gold, Color.White, Color.Orange, Color.DarkGoldenrod);
            Color trailColor = Color.Lerp(glowColor, Color.Orange, completionRatio)
                * MathHelper.Lerp(0f, 1f, EasingFunction.InCirc(completionRatio));
            return glowColor * ExtraMath.Osc(1f, 2f, speed: 32) * 0.6f;
        }

        float GetBigTrailWidth(float completionRatio)
        {
            return MathHelper.SmoothStep(0, 96, completionRatio);
        }


        float GetBigTrailWidth2(float completionRatio)
        {
            return MathHelper.SmoothStep(0, 96, completionRatio) * 1.2f;
        }

        FixedRichLaserShader laserShader = ShaderContent.GetInstance<FixedRichLaserShader>();
        laserShader.LaserColor = Color.White;
        laserShader.InnerColor = Color.Orange;
        laserShader.OuterColor = Color.Orange;
        laserShader.BloomTexture = AssetManager.LaserTextures.Bloom;
        laserShader.LaserTexture = TrailRegistry.StarTrail;
        TrailDrawer.Draw(Main.spriteBatch, swingTrailCache, GetTrailColor, GetBigTrailWidth, laserShader);

        BloomTrailShader b = BloomTrailShader.Instance;
        b.InnerColor = Color.Gold;
        b.OuterColor = Color.Lerp(Color.Orange, Color.Black, 0.5f);
        TrailDrawer.Draw(Main.spriteBatch, swingTrailCache, GetTrailColor2, GetBigTrailWidth2, b);

       // TrailDrawer.Draw(swingTrailCache, GetTrailColor, GetBigTrailWidth, shader);
    }

    public virtual void DrawSwordSprite(ref Color lightColor)
    {
        if (Keyboard.GetState().IsKeyDown(Keys.I))
            DebugDrawHitboxCheck();
        if (Keyboard.GetState().IsKeyDown(Keys.U))
            DebugDrawHitboxCheck2();

        Texture2D texture = GetTexture();
        int frameHeight = texture.Height / Main.projFrames[Projectile.type];
        int startY = frameHeight * Projectile.frame;

        Rectangle sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);
        Vector2 origin = sourceRectangle.Size() / 2f;
        Color drawColor = Projectile.GetAlpha(lightColor);
        if (isAfterImageProjectile)
            drawColor *= 0.5f;

        SpriteBatch spriteBatch = Main.spriteBatch;
        float drawScale = 1 + growScale;


        float rotation = Projectile.rotation;
        SpriteEffects spriteEffects = SpriteEffects.None;
        if (SwingDirection == 1)
        {
            spriteEffects = SpriteEffects.FlipVertically;
            rotation -= MathHelper.PiOver2;
        }

        swordRotation = rotation;


        Vector2 drawPosition = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
        if (outlineColor.A > 0)
        {
            Color drawOutlineColor = outlineColor.MultiplyRGB(lightColor);
            SpriteWhiteShader whiteShader = SpriteWhiteShader.Instance;
            spriteBatch.Restart(effect: whiteShader.Effect);
            spriteBatch.Draw(texture,
                drawPosition + Vector2.UnitX * 2,
                sourceRectangle, drawOutlineColor, rotation, origin, drawScale, spriteEffects, 0);
            spriteBatch.Draw(texture,
                drawPosition + Vector2.UnitX * -2,
                sourceRectangle, drawOutlineColor, rotation, origin, drawScale, spriteEffects, 0);

            spriteBatch.Draw(texture,
                drawPosition + Vector2.UnitY * 2,
                sourceRectangle, drawOutlineColor, rotation, origin, drawScale, spriteEffects, 0);
            spriteBatch.Draw(texture,
                drawPosition + Vector2.UnitY * -2,
                sourceRectangle, drawOutlineColor, rotation, origin, drawScale, spriteEffects, 0);
            spriteBatch.RestartDefaults();
        }

        spriteBatch.Draw(texture, drawPosition,
            sourceRectangle, drawColor, rotation, origin, drawScale, spriteEffects, 0);
        PostDrawSword(drawPosition,
            sourceRectangle, drawColor, rotation, origin, drawScale * Vector2.One, spriteEffects, 0);
        if (glowColor.A > 0)
        {
            Color colorToGlowWith = glowColor;
            colorToGlowWith.A = 0;
            for (int i = 0; i < 2; i++)
            {
                spriteBatch.Draw(texture,
                      Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
                      sourceRectangle, glowColor, rotation, origin, drawScale, spriteEffects, 0);

            }

        }

    }

    public virtual void PostDrawSword(Vector2 position, Rectangle srcRect, Color drawColor, float rotation, Vector2 origin, Vector2 drawScale, SpriteEffects spriteEffect, float layerDepth)
    {

    }
    public void DrawAngelSwordBeam(ref Color lightColor)
    {
        if (!MeleeEffectsPlayer.superExtender)
            return;
        SwordBeamShader swordBeamShader = SwordBeamShader.Instance;
        swordBeamShader.InnerColor = Color.Gold;
        swordBeamShader.OuterColor = glowAfterImageColor;

        SpriteEffects spriteEffects = SpriteEffects.None;
        if (SwingDirection == 1)
        {
            spriteEffects = SpriteEffects.FlipHorizontally;
        }



   

        Vector2 pos = _hitboxSwordEnd + (_hitboxSwordEnd - _hitboxStart).SafeNormalize(Vector2.Zero) * 64;
        float dist = Vector2.Distance(pos, Projectile.Center);
        Vector2 drawPos = _hitboxSwordEnd - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
        drawPos += (_hitboxSwordEnd - _hitboxStart).SafeNormalize(Vector2.Zero) * 64;
        
        float rotationOffset = MathHelper.ToRadians(45);
    //    drawPos += offset;


        SpriteBatch spriteBatch = Main.spriteBatch;
        float drawScale = 1.15f + growScale;
        spriteBatch.Restart(blendState: BlendState.AlphaBlend, effect: swordBeamShader.Effect);

        Texture2D superExtender = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/GlowSword_AngelSword").Value;
        Vector2 origin2 = superExtender.Size() * 0.5f;
        for (int a = 0; a < afterImageCache.Length; a++)
        {
            float interpolant = a;
            interpolant /= afterImageCache.Length;
            interpolant = 1f - interpolant;
            Color drawColor2 = Color.Gold * 0.05f;
            drawColor2 *= EasingFunction.InOutSine(interpolant);
            Vector2 position = afterImageCache[a];
            float drawRotation = swingRotationCache[a];

            Vector2 offset2 = (drawRotation + MathHelper.ToRadians(-45)).ToRotationVector2() * dist;
            position += offset2;
            spriteBatch.Draw(superExtender,
              position - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
                null, drawColor2, drawRotation + rotationOffset, origin2, drawScale, spriteEffects, 0); // drawing the sword itself
        }
  
        

        Color g = Color.Gold * 0.3f;
        g.A = 0;
        spriteBatch.Draw(superExtender,
           drawPos,
              null, g * ExtraMath.Osc(0.6f, 1f, speed: 32), Projectile.rotation + rotationOffset, origin2, drawScale, spriteEffects, 0);

        spriteBatch.RestartDefaults();
    }
    public virtual void DrawSwordBeam(ref Color lightColor)
    {
        if (swordBeamLength <= 0)
            return;

        SwordBeamShader swordBeamShader = SwordBeamShader.Instance;
        swordBeamShader.InnerColor = outlineColor;
        swordBeamShader.OuterColor = glowAfterImageColor;

        SpriteEffects spriteEffects = SpriteEffects.None;
        if (SwingDirection == 1)
        {
            spriteEffects = SpriteEffects.FlipHorizontally;
        }


        Texture2D texture = RequestHologramTexture().Value;
        Vector2 offset = (Projectile.rotation + MathHelper.ToRadians(-45)).ToRotationVector2() * swordBeamLength / 2;
        Vector2 origin = texture.Size() / 2f;
        Vector2 drawPos = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
        float rotationOffset = MathHelper.ToRadians(45);
        drawPos += offset;

        Color drawColor = Color.White.MultiplyRGB(lightColor) * 0.2f;

        SpriteBatch spriteBatch = Main.spriteBatch;
        float drawScale = 1.15f + growScale;
        spriteBatch.Restart(blendState: BlendState.AlphaBlend, effect: swordBeamShader.Effect);


        for (int a = 0; a < afterImageCache.Length; a++)
        {
            float interpolant = a;
            interpolant /= afterImageCache.Length;
            interpolant = 1f - interpolant;
            Color drawColor2 = glowAfterImageColor;
            drawColor2 *= EasingFunction.InOutSine(interpolant);
            Vector2 position = afterImageCache[a];
            float drawRotation = swingRotationCache[a];

            Vector2 offset2 = (drawRotation + MathHelper.ToRadians(-45)).ToRotationVector2() * swordBeamLength / 2;
            position += offset2;
            spriteBatch.Draw(texture,
              position - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
                null, drawColor2, drawRotation + rotationOffset, origin, drawScale, spriteEffects, 0); // drawing the sword itself
        }


        spriteBatch.Draw(texture,
           drawPos,
              null, drawColor, Projectile.rotation + rotationOffset, origin, drawScale, spriteEffects, 0);



        spriteBatch.RestartDefaults();
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
        //In here we'd spawn the hit effects
        //Hit stop effect and minor screenshake I think
        if (!_hasHitStop)
        {

            HitstopTimer = hitStopTime;
            _hasHitStop = true;
        }

        foreach (var addon in MeleeEffectsPlayer.addons)
        {
            addon.OnHitNPC(this, target, hit, damageDone);
        }
        float speedXa = -Projectile.velocity.X * Main.rand.NextFloat(.4f, .7f) + Main.rand.NextFloat(-8f, 8f);
        float speedYa = -Projectile.velocity.Y * Main.rand.Next(0, 0) * 0.01f + Main.rand.Next(-20, 21) * 0.0f;
        Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center.X, target.Center.Y, speedXa * 0, speedYa * 0,
            ModContent.ProjectileType<BaseHitEffect>(), Projectile.damage * 0, 0f, Projectile.owner, 0f, 0f);


    }

    public void Add(ISwing swing)
    {
        _swings.Add(swing);
    }
}
