using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Core.Particles;
using Stellamod.Core.Utilities;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.PunkerTown.BossesPT.PunkerPrime;

//ALright so now we need to think about punker prime's arms
//The arms are able to operate individually of the boss
//The easiest way to do this is to make separate NPCs for each of them
//So we should probably have a base NPC for PunkerPrime's arms

public abstract class PunkerPrimeArm : ModNPC,
    IDrawOutlines
{
    public enum DabState : byte
    {
        None,
        DabLeft_Bent,
        DabLeft_Straight,
        DabRight_Bent,   
        DabRight_Straight,
        DabEnd,
    }
    protected float _flashAlpha;
    protected Color _outlineColor;
    protected Color TargetOutlineColor;
    protected PunkerPrimeArmPart[] _segmentsBackingField;

    protected PunkerPrimeArmPart[] Segments
    {
        get
        {
            if (_segmentsBackingField == null)
            {
                Vector2[] _segmentSizes = new Vector2[4]
                {
                    new Vector2(56, 34),
                    new Vector2(134, 24),
                    new Vector2(16, 16),
                    new Vector2(106, 28),
                };

                _segmentsBackingField = new PunkerPrimeArmPart[4];
                for (int a = 0; a < _segmentsBackingField.Length; a++)
                {
                    PunkerPrimeArmPart parent = a == 0 ? null : _segmentsBackingField[a - 1];
                    PunkerPrimeArmPart armPart = new PunkerPrimeArmPart(parent, _segmentSizes[a], 0);
                    _segmentsBackingField[a] = armPart;
                }
            }

            return _segmentsBackingField;
        }
    }

    private VerletChain _vchain;
    protected VerletChain VChain
    {
        get
        {
            if (_vchain == null)
            {
                _vchain = new VerletChain(NPC.Center, NPC.Center + Vector2.UnitY * 360, 20);
            }
            return _vchain;
        }
    }
    public bool isAttacking;
    public float superChargeTimer;
    public float afterImageStrength;
    public Color telegraphLineColor;
    public float heldLightningScale;
    private float _startAngleLerp;

    private float[] _originalAngles;
    protected bool DoAttack
    {
        get => NPC.ai[0] == 1;
        set => NPC.ai[0] = value ? 1 : 0;
    }

    protected NPC Parent
    {
        get => Main.npc[(int)NPC.ai[1]];
        set => NPC.ai[1] = value.whoAmI;
    }

    protected ref float Timer => ref NPC.ai[2];
    public DabState dabinState;
    public float dabTimer;
    protected Player Target => Main.player[NPC.target];
    private Texture2D[] _armTextures;
    protected Texture2D RequestSubTexture(string spriteName)
    {
        string texturePath = ModContent.GetInstance<PunkerPrime>().Texture;
        string subTexturePath = texturePath + "_" + spriteName;
        Texture2D texture = ModContent.Request<Texture2D>(subTexturePath, AssetRequestMode.ImmediateLoad).Value;
        return texture;
    }

    protected Texture2D[] RequestArmTextures()
    {
        Texture2D[] textures = new Texture2D[4];
        textures[0] = RequestSubTexture("Shoulder");
        textures[1] = RequestSubTexture("Arm");
        textures[2] = RequestSubTexture("Elbow");
        textures[3] = RequestSubTexture("ForeArm");
        return textures;
    }

    protected Vector2 GetGunHoldCenter()
    {
        return Segments[Segments.Length - 1].endPosition;
    }

    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.Write(isAttacking);
        writer.Write(superChargeTimer);
        writer.Write((byte)dabinState);
    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        isAttacking = reader.ReadBoolean();
        superChargeTimer = reader.ReadSingle();
        dabinState = (DabState)reader.ReadByte();
    }


    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.npcFrameCount[NPC.type] = 1;
        NPCID.Sets.MPAllowedEnemies[NPC.type] = true;
        NPCID.Sets.BossBestiaryPriority.Add(Type);
        NPCID.Sets.TrailCacheLength[NPC.type] = 16;
        NPCID.Sets.TrailingMode[Type] = 3;
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        NPC.width = 32;
        NPC.height = 32;
        NPC.damage = 100;
        NPC.defense = 14;
        NPC.lifeMax = 6000;

        NPC.value = Item.buyPrice(gold: 5);
        NPC.knockBackResist = 0f;
        NPC.noGravity = true;
        NPC.noTileCollide = true;
        NPC.npcSlots = 30f;

        NPC.dontTakeDamage = true;
        NPC.dontCountMe = true;
        NPC.dontTakeDamageFromHostiles = true;
    }

    public override bool CheckActive()
    {
        return false;
    }

    #region Arms
    private void AI_Arms()
    {
        switch (dabinState)
        {
            case DabState.None:
                dabTimer = 0;
                _startAngleLerp = 0;
                ArmAI();
                break;
            case DabState.DabLeft_Bent:
                AI_DabLeftBent();
                break;
            case DabState.DabLeft_Straight:
                AI_DabLeftStraight();
                break;
            case DabState.DabRight_Bent:
                AI_DabRightBent();
                break;
            case DabState.DabRight_Straight:
                AI_DabRightStraight();
                break;
            case DabState.DabEnd:
                AI_DabEnd();
                break;
        }
    }

    private void StoreOriginalAngles()
    {
        _startAngleLerp = Segments[0].angle;
        _originalAngles = new float[4];
        for (int i = 0; i < Segments.Length; i++)
            _originalAngles[i] = Segments[i].angle;
    }
    private void AI_DabLeftBent()
    {
        if (_startAngleLerp == 0)
        {
            StoreOriginalAngles();
        }
        NPC.velocity = Vector2.Zero;
        SetRootToParentCenter();
        float osc = MathF.Sin(Timer * 0.06f + NPC.whoAmI) * 0.5f + 0.5f;
        float baseAngle = -275;
        float targetAngle = MathHelper.ToRadians(baseAngle) + MathHelper.ToRadians(MathHelper.Lerp(0, 10, osc)); ;


        dabTimer++;
        float ease = EasingFunction.InOutExpo(dabTimer / 75f);

        float[] angles = new float[4];
        angles[0] = Utils.AngleLerp(_startAngleLerp, targetAngle, ease);
        angles[1] = angles[0] + MathHelper.ToRadians(75);
        angles[2] = angles[1];
        angles[3] = angles[2] + MathHelper.ToRadians(135);
        for(int i = 0; i < Segments.Length; i++)
        {
            Segments[i].angle = Utils.AngleLerp(Segments[i].angle, angles[i], 0.05f);
        }

        AimGunTowardTarget();
    }

    private void AI_DabLeftStraight()
    {
        if (_startAngleLerp == 0)
        {
            StoreOriginalAngles();
        }
        NPC.velocity = Vector2.Zero;
        SetRootToParentCenter();
        float osc = MathF.Sin(Timer * 0.06f + NPC.whoAmI) * 0.5f + 0.5f;
        float baseAngle = -45;
        float targetAngle = MathHelper.ToRadians(baseAngle) + MathHelper.ToRadians(MathHelper.Lerp(0, 10, osc)); ;


        dabTimer++;
        float ease = EasingFunction.InOutExpo(dabTimer / 75f);




        float[] angles = new float[4];
        angles[0] = Utils.AngleLerp(_startAngleLerp, targetAngle, ease);
        angles[1] = angles[0];
        angles[2] = angles[1];
        angles[3] = angles[2];
        for (int i = 0; i < Segments.Length; i++)
        {
            Segments[i].angle = Utils.AngleLerp(Segments[i].angle, angles[i], 0.05f);
        }

        AimGunTowardTarget();
    }

    private void AI_DabEnd()
    {
        NPC.velocity = Vector2.Zero;
        SetRootToParentCenter();
        dabTimer++;
        float ease = EasingFunction.InOutExpo(dabTimer / 45f);
        for (int i = 0; i < Segments.Length; i++)
        {
            Segments[i].angle = Utils.AngleLerp(Segments[i].angle, _originalAngles[i], 0.1f * ease);
        }
        AimGunTowardTarget();
    }
    private void AI_DabRightBent()
    {

    }
    private void AI_DabRightStraight()
    {

    }
    public virtual void ArmAI()
    {

    }

    private void AI_VerletIntegrationCoords()
    {
        float s = 4;
        Vector2 rootPosition = Parent.Center + Vector2.UnitY * 150;
        Vector2 targetPosition = rootPosition + Vector2.UnitY * 200;
        targetPosition.X += ExtraMath.Osc(-200, 200, speed: s, offset: NPC.whoAmI * 4);
        targetPosition.Y += ExtraMath.Osc(-50, 0, speed: 2, offset: NPC.whoAmI * 4);
        VChain.noTileCollide = true;
        VChain.points[0].pinned = true;
        VChain.points[0].position = Parent.Center;
        VChain.points[VChain.points.Length - 1].pinned = true;
        VChain.points[VChain.points.Length - 1].position = GetGunHoldCenter();
        VChain.gravity = 0;
        VChain.Update();
    }

    //Sealing this just so don't accidentally override it, we don't want to remove the base functionailty
    public sealed override void AI()
    {
        base.AI();

        if (!Parent.active)
            NPC.active = false;

        AI_VerletIntegrationCoords();
        AI_Arms();
        if (superChargeTimer > 0)
        {
            if (superChargeTimer % 2 == 0)
            {
                ArmAI();
            }

            superChargeTimer--;
        }

        _flashAlpha *= 0.92f;
        _outlineColor = Color.Lerp(_outlineColor, TargetOutlineColor, 0.1f);

        if (isAttacking)
        {
            if (Main.rand.NextBool(16))
            {
                Vector2 gunHoldCenter = GetGunHoldCenter();
                Vector2 spawnPos = gunHoldCenter;
                spawnPos += Main.rand.NextVector2Circular(8, 8);
                var zapParticle = LegacyParticle.NewParticle<SparkParticle>(spawnPos, Main.rand.NextVector2Circular(4, 4), Color.White, Scale: Main.rand.NextFloat(0.5f, 1f));
                zapParticle.innerColor = Color.White;
                zapParticle.outerColor = Color.Red;
                zapParticle.fadeToColor = Color.Yellow;
            }
        }

        Lighting.AddLight(NPC.Center, TorchID.Red);
    }



    #endregion
    public void SuperchargeAttack()
    {
        DoAttack = true;
        superChargeTimer = 300;
        NPC.netUpdate = true;
        SoundStyle superCharge = AssetRegistry.Sounds.SteamPunking.MechSupercharge;
        superCharge.PitchVariance = 0.3f;
        SoundEngine.PlaySound(superCharge, NPC.position);
    }


    public void Attack()
    {
        DoAttack = true;
        NPC.netUpdate = true;
    }

    protected void SetRootToParentCenter()
    {


        Segments[0].rootPosition = Parent.Bottom;
    }
    protected void AimGunTowardTarget()
    {
        Vector2 holdCenter = GetGunHoldCenter();
        Vector2 targetVelocity = (holdCenter - NPC.Center);
        NPC.velocity = Vector2.Lerp(Vector2.Zero, targetVelocity, EasingFunction.InOutSine(Timer / 60f));

        float targetAngle = Segments[Segments.Length - 1].angle;
        NPC.rotation = Utils.AngleLerp(NPC.rotation, targetAngle, 0.1f);
    }

    protected void CreateMuzzleFlash()
    {
        _flashAlpha = 1f;
        var bigPart = FXUtil.GlowCircleBoom(GetGunHoldCenter(), Color.White, Color.Red, Color.DarkRed);
        var littlePart = FXUtil.GlowCircleBoom(GetGunHoldCenter(), Color.White, Color.Red, Color.DarkRed);
        littlePart.Scale *= 0.6f;

        float numParticles = 4;
        for (float n = 0; n < numParticles; n++)
        {
            Vector2 fireVelocity = NPC.rotation.ToRotationVector2() * 5f;
            fireVelocity *= Main.rand.NextFloat(0.5f, 1f);
            Dust.NewDustPerfect(NPC.Center, ModContent.DustType<GlowDust>(), fireVelocity, Scale: Main.rand.NextFloat(0.5f, 1f));
        }
    }

    private void DrawTelegraphLine(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        Texture2D bloomLineTexture = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/BloomLine").Value;
        Vector2 drawOrigin = new Vector2(bloomLineTexture.Width / 2f, 0f);
        Vector2 drawCenter = NPC.Center - screenPos;
        Vector2 scale = Vector2.One;
        scale.X = 0.35f;
        scale.Y = 2;

        Color color = telegraphLineColor;
        color.A = 0;
        color *= 0.35f;
        float rotation = NPC.rotation - MathHelper.ToRadians(90);
        spriteBatch.Draw(bloomLineTexture, drawCenter, null, color, rotation, drawOrigin, scale, SpriteEffects.None, 0);
    }

    private void DrawTentacleArm(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        var texture2 = RequestSubTexture("ArmSmallGlow");
        var texture = RequestSubTexture("ArmSmall");
        for (int i = 0; i < VChain.points.Length - 1; i++)
        {

            var point = VChain.points[i];
            Vector2 drawPosition = point.position - Main.screenPosition;
            Vector2 drawOrigin = new Vector2(0f, texture.Height / 2f);
            Vector2 drawScale = Vector2.One;
            drawScale.Y *= 0.2f;
            drawScale.X *= 0.45f;
            var nextPoint = VChain.points[i + 1];
            float angle = (nextPoint.position - point.position).ToRotation();
            spriteBatch.Draw(texture, drawPosition, null, drawColor, angle, drawOrigin, drawScale, SpriteEffects.None, 0);
            if (isAttacking)
            {
                Color glowColor = Color.Yellow;
                glowColor *= ExtraMath.Osc(0f, 0.5f, speed: 8, offset: i * 4);
                glowColor.A = 0;
                spriteBatch.Draw(texture2, drawPosition, null, glowColor, angle, drawOrigin, drawScale, SpriteEffects.None, 0);
            }
        }
    }

    public void DrawPowerCord(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        DrawTentacleArm(spriteBatch, screenPos, drawColor);
    }

    public  void ForwardIK()
    {
        Segments[0].rootPosition = Parent.Bottom;
        for (int i = 0; i < Segments.Length; i++)
        {
            PunkerPrimeArmPart segment = Segments[i];
            segment.Update();
        }

    }
    public void DrawArm(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
     
        if (_armTextures == null)
            _armTextures = RequestArmTextures();
        for (int i = 0; i < Segments.Length; i++)
        {
            PunkerPrimeArmPart segment = Segments[i];
            segment.Draw(spriteBatch, _armTextures[i], screenPos, drawColor);
        }
    }

    private void DrawGunAfterImage(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
        Rectangle frame = NPC.frame;
        Vector2 drawOrigin = frame.Size() / 2f;
        Vector2 drawScale = Vector2.One;
        float length = NPCID.Sets.TrailCacheLength[Type];
        for (int i = 0; i < length; i++)
        {
            float f = i;
            float completionRatio = f / length;
            Vector2 oldPosition = NPC.oldPos[i];
            Vector2 oldCenter = oldPosition + NPC.Size / 2f - screenPos;
            Color color = Color.Red;
            color *= 0.1f;
            color *= afterImageStrength;
            color *= MathHelper.SmoothStep(1f, 0f, completionRatio);
            spriteBatch.Draw(texture, oldCenter, frame, color, NPC.rotation, drawOrigin, drawScale, SpriteEffects.None, 0);
        }
    }
    private Color ColorFunction(float completionRatio)
    {
        return Color.Lerp(Color.Transparent, Color.Gray, EasingFunction.QuadraticBump(completionRatio)) * heldLightningScale * 0.35f;
    }

    private float WidthFunction(float completionRatio)
    {
        return 8 * heldLightningScale;
    }
    private void DrawHeldLightning(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        if (heldLightningScale <= 0.02f)
            return;

        List<Vector2> conjureLightningPositions = new List<Vector2>();
        float numPoints = 32;
        for (float n = 0; n < numPoints; n++)
        {
            float completionRatio = n / numPoints;
            Vector2 position = Vector2.Lerp(GetGunHoldCenter(), NPC.Center, completionRatio);
            conjureLightningPositions.Add(position);
        }

        BlackFireShader shader = BlackFireShader.Instance;
        shader.PrimaryTexture = TrailRegistry.LightningTrail2;
        shader.PrimaryTexture2 = TrailRegistry.LightningTrail;
        shader.InnerColor = Color.White;
        shader.OuterColor = Color.Red;
        shader.Distortion = 0.2f;
        shader.Time = Main.GlobalTimeWrappedHourly * 16;
        TrailDrawer.Draw(spriteBatch, conjureLightningPositions.ToArray(), ColorFunction, WidthFunction, shader);
    }
    public void DrawGun(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        Texture2D texture = TextureAssets.Npc[Type].Value;//ModContent.Request<Texture2D>(Texture).Value;
        Vector2 drawPosition = NPC.Center - screenPos;

        Color baseColor = isAttacking ? Color.White : Color.Lerp(Color.White, Color.Black, 0.8f);
        Color finalColor = baseColor.MultiplyRGB(drawColor);
        Vector2 drawScale = Vector2.One;
        Vector2 drawOrigin = NPC.frame.Size() / 2f;
        spriteBatch.Draw(texture, drawPosition, NPC.frame, finalColor, NPC.rotation, drawOrigin, drawScale, SpriteEffects.None, 0);

        Color glowColor = Color.Red;
        glowColor.A = 0;
        glowColor *= _flashAlpha;
        spriteBatch.Draw(texture, drawPosition, NPC.frame, glowColor, NPC.rotation, drawOrigin, drawScale, SpriteEffects.None, 0);
    }

    public void DrawGunEffects(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        DrawHeldLightning(spriteBatch, screenPos, drawColor);
        DrawTelegraphLine(spriteBatch, screenPos, drawColor);
        DrawGunAfterImage(spriteBatch, screenPos, drawColor);
    }

    public void DrawGunArm(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        if (superChargeTimer > 0f)
        {
            DrawSuperchargedArm(spriteBatch, screenPos, drawColor);
        }
        else
        {
            DrawArm(spriteBatch, screenPos, drawColor);
        }
    }

    //Drawing is handled by parent npc for proper layering
    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        return false;
    }

    private void DrawSuperchargedArm(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        if (_armTextures == null)
            _armTextures = RequestArmTextures();
        for (int i = 0; i < Segments.Length; i++)
        {
            PunkerPrimeArmPart segment = Segments[i];
            Color finalColor = Color.Red;
            finalColor = Color.Lerp(finalColor, drawColor, ExtraMath.Osc(0f, 1f, speed: 32f));
            segment.Draw(spriteBatch, _armTextures[i], screenPos, finalColor);
        }
    }
    public void DrawGlowBall(SpriteBatch spriteBatch, Vector2 screen, Color drawColor)
    {
        if (heldLightningScale <= 0.02f)
            return;
        Texture2D glowballTexture = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/Extra_56").Value;
        Vector2 drawCenter = GetGunHoldCenter() - Main.screenPosition;
        Vector2 drawOrigin = glowballTexture.Size() / 2f;
        Color glowColor = Color.Lerp(Color.Red, Color.Yellow, ExtraMath.Osc(0f, 1f, speed: 16));
        glowColor.A = 0;
        glowColor *= ExtraMath.Osc(0.5f, 1f, speed: 64);
        spriteBatch.Draw(glowballTexture, drawCenter, null, glowColor, 0, drawOrigin, heldLightningScale * 0.35f * ExtraMath.Osc(0.9f, 1f, speed: 64), SpriteEffects.None, 0);
    }


    public void DrawOutlines(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
    {
        if (_outlineColor == Color.Transparent)
            return;
        ForwardIK();
        float outlineOffset = 2;
        Vector2 h = Vector2.UnitX * outlineOffset;
        Vector2 v = Vector2.UnitY * outlineOffset;
        DrawArm(spriteBatch, screenPos + h, _outlineColor);
        DrawArm(spriteBatch, screenPos - h, _outlineColor);
        DrawArm(spriteBatch, screenPos + v, _outlineColor);
        DrawArm(spriteBatch, screenPos - v, _outlineColor);

        DrawGun(spriteBatch, screenPos + h, _outlineColor);
        DrawGun(spriteBatch, screenPos - h, _outlineColor);
        DrawGun(spriteBatch, screenPos + v, _outlineColor);
        DrawGun(spriteBatch, screenPos - v, _outlineColor);
    }
}
