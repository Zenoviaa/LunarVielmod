using Stellamod.Common.Particles;
using Stellamod.Content.Areas.WondrousDarkspace.PrismadolphBoss.Projectiles;
using Stellamod.Core.NPCHelpers;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Rendering.RTs;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.WondrousDarkspace.PrismadolphBoss;

public class PrismadolphSegment : ModNPC,
    IDrawToRenderTarget
{
    float _aggression;
    float _jiggle;
    float _timer;
    enum AIState : byte
    {
        Passive,
        Aggressive,
        SuperAggressive
    }

    enum Segment : byte
    {
        Head,
        Body,
        Tail
    }

    Prismadolph Parent
    {
        get
        {
            var parent = Main.npc[(int)NPC.ai[0]];
            if (parent.ModNPC is Prismadolph p)
            {
                return p;
            }
            else
            {
                return null;
            }
        }
    }

    float _attackAlpha;
    float _armorProgress;
    float _chargeTimer;
    float _healTimer;
    Vector2 _shakeOffset;
    Outliner _outliner;
    AIState State
    {
        get => (AIState)NPC.ai[1];
        set => NPC.ai[1] = (float)value;
    }

    public override string Texture => AssetReferences.Assets.Textures.Darkspace.NPCs.Prismadolph.KEY;
    int SegmentIndex => (int)NPC.ai[2];
    public float ChargeAttackProgress;
    Vector2 SegmentPosition
    {
        get => Parent.Chain.points[SegmentIndex];
    }
    Vector2 NextSegmentPosition
    {
        get => Parent.Chain.points[SegmentIndex + 1];
    }
    //ref float Timer => ref NPC.ai[3];
    Segment SegmentType
    {
        get => (Segment)NPC.ai[3];
        set => NPC.ai[3] = (float)value;
    }
    float ChargeAttackTime => 500;
    float MiniAggression => 25;
    float MegaAggression => 100;
    float MegaAggressionLerp => (_aggression - 75) / (MegaAggression - 75);
    float AttackChargeLerp
    {
        get
        {
            if(State == AIState.Aggressive)
                return EasingFunction.InExpo(_timer / ChargeAttackTime) + ChargeAttackProgress; ;
            return ChargeAttackProgress;
        }
    }
    int Damage => 35;
    int BigDamage => 65;
    float JiggleRotOffset => MathF.Sin(_jiggle * 0.3f) * 0.2f * MathHelper.Lerp(0f, 1f, _jiggle / 30F);
    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.Write(_aggression);
        writer.Write(_timer);
        writer.Write(_armorProgress);
    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        _aggression = reader.ReadSingle();
        _timer = reader.ReadSingle();
        _armorProgress = reader.ReadSingle();
    }

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        NPCID.Sets.TrailCacheLength[Type] = 16;
        NPCID.Sets.TrailingMode[Type] = 0;
        NPCID.Sets.ImmuneToAllBuffs[Type] = true;
        NPCID.Sets.ImmuneToRegularBuffs[Type] = true;
        NPCSets.Heavy[Type] = true;
        Main.npcFrameCount[Type] = 10;
    }

    public override bool CanHitPlayer(Player target, ref int cooldownSlot)
    {
        return false;
    }

    public override bool CanHitNPC(NPC target)
    {
        return false;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        NPC.defense = 50;
        NPC.width = NPC.height = 100;
        NPC.damage = 70;
        NPC.lifeMax = 500;
        NPC.value = 5000f;
        NPC.knockBackResist = 0f;
        NPC.npcSlots = 5;
        NPC.noTileCollide = true;
        NPC.noGravity = true;
        NPC.aiStyle = -1;
        NPC.lavaImmune = true;
        NPC.HitSound = SoundID.DD2_WitherBeastCrystalImpact;
        NPC.DeathSound = SoundID.DD2_CrystalCartImpact;
    }

    void MakeImmune()
    {
        NPC.dontTakeDamage = true;
        NPC.dontTakeDamageFromHostiles = true;
        NPC.dontCountMe = true;
    }

    void Heal()
    {
        var hp = Parent.NPC.lifeMax * 0.1F;
        Parent.NPC.life += (int)(hp);
        _healTimer = 120;
        PixelPrimitiveCircleFactory.CreateGenericBoom(NPC.Center, Color.White, Color.SkyBlue, 60, 256);
        NPC.HealEffect((int)hp);
    }

    void ManageHealingEffectsDecreaseTimer()
    {
        if (_healTimer > 0)
        {
            if (_healTimer % 2 == 0)
            {
                var pos = NPC.Center;
                pos += Main.rand.NextVector2Circular(70, 70);
                PS.BitDust.Spawn(BitDustFactory.SlowingOverTime with
                {
                    position = pos,
                    velocity = -Vector2.UnitY * 8,
                    innerColor = Color.Green.ToVector4(),
                    outerColor = Color.DarkGreen.ToVector4(),
                    timeLeft = 90,
                    scale = new Vector2(Main.rand.NextFloat(0.6F, 1.1F)),
                    color = Color.White.ToVector4(),
                });
            }

            _healTimer--;
        }
    }
    public override bool CheckActive()
    {
        return false;
    }

    public override void AI()
    {
        base.AI();
        if (Parent == null || !Parent.NPC.active)
        {
            NPC.active = false;
            return;
        }

        _attackAlpha = MathHelper.Lerp(_attackAlpha, AttackChargeLerp, 0.1f);
        var segmentRatio = (SegmentIndex + 1) / (float)Parent.Chain.points.Length;
        var killMyselfPart = 1f - segmentRatio;
        if (Parent.deathProgress > killMyselfPart)
        {
            DeathEffect();
            NPC.Kill();
        }
        if (_chargeTimer > 0)
        {
            _chargeTimer--;
        }

        if (SegmentType == Segment.Head || SegmentType == Segment.Tail)
        {
            MakeImmune();
        }
        ChargeAttackProgress = MathHelper.Lerp(ChargeAttackProgress, 0, 0.03f);
        var targetArmorOn = _chargeTimer > 0 ? 1 : 0;
        _armorProgress = MathHelper.Lerp(_armorProgress, targetArmorOn, 0.05F);
        if (_jiggle > 0)
        {
            _jiggle--;
        }

        if (_timer % 4 == 0)
        {
            _shakeOffset = Main.rand.NextVector2Circular(8, 8);
        }
        NPC.life = NPC.lifeMax;
        NPC.Center = Parent.Chain.points[SegmentIndex];
        _outliner.SetDefaults();
        switch (State)
        {
            case AIState.Passive:
                AI_Passive();
                break;
            case AIState.Aggressive:
                AI_Aggressive();
                break;
            case AIState.SuperAggressive:
                AI_SuperAggressive();
                break;
        }
        _outliner.Update();
        ManageHealingEffectsDecreaseTimer();
        Lighting.AddLight(NPC.Center, new Vector3(0.6F));
    }
    SoundStyle NextRandomZapSound()
    {
        SoundStyle sound;
        switch (Main.rand.Next(3))
        {
            default:
                sound = AssetReferences.Assets.Sounds.Dreadmire_LightingRain1.Asset;
                break;
            case 1:
                sound = AssetReferences.Assets.Sounds.Dreadmire_LightingRain2.Asset;
                break;
            case 2:
                sound = AssetReferences.Assets.Sounds.Dreadmire_LightingRain3.Asset;
                break;
        }
        sound.PitchVariance = 1F;
        return sound;
    }

    Color NextRandomDustColor()
    {
        Color dustColor = Color.DeepSkyBlue;
        dustColor.ScrollHue(Main.rand.NextFloat(-24f, 24f));
        return dustColor;
    }
    void DeathEffect()
    {
        for (var i = 0; i < 16; i++)
        {
            var ratio = Main.rand.NextFloat();
            var pos = NPC.Center;
            var vel = Main.rand.NextVector2Circular(16, 16);
            PS.GlowDust.Spawn(new()
            {
                position = pos,
                velocity = vel,
                color = NextRandomDustColor() with { A = 0 },
                timeLeft = Main.rand.Next(60, 120),
                scale = Main.rand.NextFloat(0.6f, 1f) * 0.2f,
            });
        }

        SoundEngine.PlaySound(NextRandomZapSound(), NPC.position);
        switch (SegmentType)
        {
            case Segment.Tail:
                Parent.NPC.ModNPC.MakeGore("_Gore_Tail", NPC.position, NPC.velocity);
                break;
            case Segment.Body:
                Parent.NPC.ModNPC.MakeGore("_Gore_Body", NPC.position, NPC.velocity);
                break;
            case Segment.Head:
                Parent.NPC.ModNPC.MakeGore("_Gore_Head", NPC.position, NPC.velocity);
                break;
        }
        var fx = FXUtil.GlowCircleBoom(NPC.Center, Color.White, Color.SkyBlue, Color.DarkBlue, 25, 0.18F);
        fx.Scale *= 1.7F;

        PixelPrimitiveCircleFactory.CreateGenericBoom(NPC.Center, Color.White, Color.SkyBlue, 35, 256);
    }
    void SwitchState(AIState state)
    {
        if (MultiplayerHelper.IsHost)
        {
            State = state;
            _timer = 0;
            NPC.netUpdate = true;
        }
    }
    void AI_Passive()
    {
        _timer++;
        if (_aggression >= MiniAggression)
        {
            SwitchState(AIState.Aggressive);
        }
        if (Main.rand.NextBool(32))
        {
            var pos = NPC.Center + Main.rand.NextVector2CircularEdge(100, 100);
            var vel = Vector2.Zero;
            Dust.NewDustPerfect(pos, DustID.Electric, vel, Scale: 0.3F);
        }
    }
    void AI_Aggressive()
    {
        _timer++;

        NPC.TargetClosest();
        _outliner.warning = true;
        _chargeTimer = 15;
        if (Main.rand.NextBool(4))
        {
            var pos = NPC.Center + Main.rand.NextVector2CircularEdge(100, 100);
            var vel = (NPC.Center - pos) * 0.04F;
            PS.SwirlingFlameDust.Spawn(BitDustFactory.SlowingOverTime with
            {
                position = pos,
                velocity = vel,
                innerColor = Color.White.ToVector4(),
                outerColor = Color.SkyBlue.ToVector4(),
                timeLeft = 60,
                scale = new Vector2(Main.rand.NextFloat(0.5f, 1f)),
            });
        }

        if (Main.rand.NextBool(8))
        {
            var pos = NPC.Center + Main.rand.NextVector2CircularEdge(100, 100);
            var vel = (NPC.Center - pos) * 0.04F;
            Dust.NewDustPerfect(pos, DustID.Electric, vel, Scale: 0.7F);
        }


        if (_timer >= ChargeAttackTime)
        {
            _aggression -= 5;
            if (MultiplayerHelper.IsHost)
            {
                var target = NPC.PlayerImTargeting;
                var vel = (target.Center - NPC.Center);
                vel *= 2f;

                var firer = ProjFirer.From<WondrousLightningZap>(NPC);
                firer.damage = Damage;
                firer.velocity = vel;
                firer.ai0 = NPC.whoAmI;
                firer.ai2 = 60;
                firer.New();
            }

            _timer = 0;
        }
        if (_aggression <= 5)
        {
            SwitchState(AIState.Passive);
        }
        if (_aggression >= MegaAggression)
        {
            SwitchState(AIState.SuperAggressive);
        }
    }

    void AI_SuperAggressive()
    {
        _chargeTimer = 30;
        _timer++;
        if (_timer == 1 && MultiplayerHelper.IsHost)
        {
            var target = NPC.PlayerImTargeting;
            var vel = (target.Center - NPC.Center).Resize(2048);
            //vel *= 4f;

            var firer = ProjFirer.From<WondrousLightningZap>(NPC);
            firer.damage = BigDamage;
            firer.velocity = vel;
            firer.ai0 = NPC.whoAmI;
            firer.ai1 = 2;
            firer.ai2 = 60;
            firer.New();
        }
        _outliner.attacking = true;
        if (Main.rand.NextBool(2))
        {
            var pos = NPC.Center + Main.rand.NextVector2CircularEdge(384, 384);
            var vel = (NPC.Center - pos) * 0.08F;
            var fx = FXUtil.GlowStretch(pos, vel);
            fx.InnerColor = Color.Pink;
            fx.GlowColor = Color.Violet;
            fx.OuterGlowColor = Color.DarkViolet;
        }

        if (_timer >= 100)
        {
            Heal();
            _aggression = 0;
            _timer = 0;
            SwitchState(AIState.Passive);
        }
    }


    public void Charge()
    {
        _chargeTimer = 15;
    }
    float _rot;
    public void DrawJigglyCore(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor, float rot)
    {
        _rot = rot;
        if (SegmentType == Segment.Tail)
            return;
        if (SegmentType == Segment.Head)
            return;

        var tex = TextureAssets.Npc[Type].Value;
        var pos = NPC.Center;
        pos += _shakeOffset * MathHelper.Lerp(0f, 1f, EasingFunction.InExpo(MegaAggressionLerp));
        var drawer = SpritebatchDrawer.FromTextureGetLightingColor(tex, pos);
        drawer.VerticalFrame(Prismadolph.FRAME_CORE, Main.npcFrameCount[Type]);
        drawer.CenterOrigin();
        drawer.rotation = rot + JiggleRotOffset;
        spriteBatch.Draw(drawer);

        drawer.VerticalFrame(Prismadolph.FRAME_CORE_WHITE, Main.npcFrameCount[Type]);
        drawer.color = Color.Lerp(Color.Transparent, Color.White, _attackAlpha);
        drawer.color.A = 0;
        spriteBatch.Draw(drawer);


        drawer.color = Color.Lerp(Color.Transparent, Color.Lerp(Color.SkyBlue, Color.White, ExtraMath.Osc(0f, 1f, speed: 16)) * 0.4f, ChargeAttackProgress);
        drawer.color.A = 0;
        spriteBatch.Draw(drawer);


        drawer.color = Color.Lerp(Color.Transparent, Color.Purple, MegaAggressionLerp);
        drawer.color.A = 0;
        spriteBatch.Draw(drawer);


        drawer.VerticalFrame(Prismadolph.FRAME_CORE_PURPLE, Main.npcFrameCount[Type]);
        drawer.CenterOrigin();
        drawer.rotation = rot + JiggleRotOffset;
        drawer.color *= MegaAggressionLerp;
        spriteBatch.Draw(drawer);
        // throw new NotImplementedException();
    }

    public void DrawJigglyCoreBloom(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor, float rot)
    {
        var pos = NPC.Center;
        pos += _shakeOffset * MathHelper.Lerp(0f, 1f, EasingFunction.InExpo(MegaAggressionLerp));
        var drawer = SpritebatchDrawer.FromTextureWhite(AssetReferences.Assets.GlowMasks.SimpleGlowCircle.Asset);
        drawer.CenterOrigin();
        drawer.worldPosition = pos;
        drawer.color = Color.Lerp(Color.Lerp(Color.DarkBlue, Color.Aqua, 0.4F), Color.Aqua, ExtraMath.Osc(0f, 1f, speed: 4f, offset: SegmentIndex));
        drawer.color *= MathHelper.Lerp(1f, 0f, MegaAggressionLerp);
        drawer.color *= 0.35F;
        drawer.color.A = 0;
        drawer.scale *= 0.8F;
        spriteBatch.Draw(drawer);

        drawer = SpritebatchDrawer.FromTextureWhite(AssetReferences.Assets.GlowMasks.SimpleGlowCircle.Asset);
        drawer.CenterOrigin();
        drawer.worldPosition = pos;

        drawer.color = Color.Lerp(Color.Transparent, Color.SkyBlue * ExtraMath.Osc(0f, 1f, speed: 24), AttackChargeLerp);
        //  drawer.color *= 0.35F * MathHelper.Lerp(1f, 3f, (float)_aggression / 1500F);
        drawer.color.A = 0;
        drawer.scale *= 0.8F;
        spriteBatch.Draw(drawer);


        drawer = SpritebatchDrawer.FromTextureWhite(AssetReferences.Assets.GlowMasks.SimpleGlowCircle.Asset);
        drawer.CenterOrigin();
        drawer.worldPosition = pos;

        drawer.color = Color.Lerp(Color.Transparent, Color.Purple, MegaAggressionLerp);
        //  drawer.color *= 0.35F * MathHelper.Lerp(1f, 3f, (float)_aggression / 1500F);
        drawer.color.A = 0;
        drawer.scale *= 0.8F;
        spriteBatch.Draw(drawer);
        spriteBatch.Draw(drawer);
        DrawPurple(spriteBatch, screenPos, drawColor, rot);
    }

    void DrawPurple(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor, float rot)
    {

        var tex = TextureAssets.Npc[Type].Value;
        var pos = NPC.Center;
        pos += _shakeOffset * MathHelper.Lerp(0f, 1f, EasingFunction.InExpo(MegaAggressionLerp));
        var drawer = SpritebatchDrawer.FromTextureGetLightingColor(tex, pos);
        drawer.VerticalFrame(Prismadolph.FRAME_CORE, Main.npcFrameCount[Type]);
        drawer.CenterOrigin();
        drawer.rotation = rot + JiggleRotOffset;

        drawer.VerticalFrame(Prismadolph.FRAME_CORE_PURPLE, Main.npcFrameCount[Type]);
        drawer.CenterOrigin();
        drawer.FlipVertically();
        drawer.rotation = rot + JiggleRotOffset;
        drawer.color *= MegaAggressionLerp;
        spriteBatch.Draw(drawer);
    }
    public void DrawArmor(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor, float rot)
    {
        if (SegmentType == Segment.Head)
        {
            DrawArmorHead(spriteBatch, screenPos, drawColor, rot);
        }
        else if (SegmentType == Segment.Tail)
        {
            DrawArmorTail(spriteBatch, screenPos, drawColor, rot);
        }
        else
        {
            var tex = TextureAssets.Npc[Type].Value;
            var drawer = SpritebatchDrawer.FromTextureGetLightingColor(tex, Vector2.Lerp(SegmentPosition, NextSegmentPosition, _armorProgress));
            drawer.VerticalFrame(Prismadolph.FRAME_ARMOR, Main.npcFrameCount[Type]);
            drawer.CenterOrigin();
            drawer.rotation = rot + JiggleRotOffset;
            drawer.color *= MathHelper.Lerp(1f, 0.1f, _armorProgress);
            spriteBatch.Draw(drawer);
        }
    }

    public void DrawArmorTail(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor, float rot)
    {
        var tex = TextureAssets.Npc[Type].Value;
        var drawer = SpritebatchDrawer.FromTextureGetLightingColor(tex, NPC.Center);
        drawer.VerticalFrame(Prismadolph.FRAME_TAIL, Main.npcFrameCount[Type]);
        drawer.CenterOrigin();
        drawer.rotation = rot + JiggleRotOffset;
        spriteBatch.Draw(drawer);

        drawer.VerticalFrame(Prismadolph.FRAME_TAIL_GLOW, Main.npcFrameCount[Type]);
        drawer.CenterOrigin();
        drawer.rotation = rot + JiggleRotOffset;
        drawer.color = Color.Lerp(Color.Transparent, Color.White, ExtraMath.Osc(0.5f, 1f, speed: 0.2f));
        drawer.color.A = 0;
        spriteBatch.Draw(drawer);
    }

    public void DrawArmorHead(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor, float rot)
    {
        var offY = 12;

        var tex = TextureAssets.Npc[Type].Value;
        var drawer = SpritebatchDrawer.FromTextureGetLightingColor(tex, NPC.Center);
        drawer.VerticalFrame(Prismadolph.FRAME_HEAD, Main.npcFrameCount[Type]);
        drawer.CenterOrigin();
        drawer.drawOrigin.Y += offY;
        drawer.rotation = rot + JiggleRotOffset;
        drawer.color *= MathHelper.Lerp(1f, 0f, _armorProgress);
        spriteBatch.Draw(drawer);

        drawer.VerticalFrame(Prismadolph.FRAME_HEAD_GLOW, Main.npcFrameCount[Type]);
        drawer.CenterOrigin();
        drawer.drawOrigin.Y += offY;
        drawer.rotation = rot + JiggleRotOffset;
        drawer.color = Color.Lerp(Color.Transparent, Color.White, ExtraMath.Osc(0.5f, 1f, speed: 0.2f));
        drawer.color.A = 0;
        spriteBatch.Draw(drawer);

        var col = drawer.color;
        var pos = drawer.worldPosition;

        foreach (var offset in Vector2.CardinalOffsets)
        {
            var o = offset.RotatedBy(Main.GlobalTimeWrappedHourly * 2);
            drawer.color = col * 0.2f;
            drawer.worldPosition = pos + o * 4;
            spriteBatch.Draw(drawer);
        }
        drawer = SpritebatchDrawer.FromTextureWhite(AssetReferences.Assets.GlowMasks.SimpleGlowCircle.Asset);
        drawer.CenterOrigin();
        drawer.worldPosition = NPC.Center;
        drawer.color = Color.Lerp(Color.Lerp(Color.DarkBlue, Color.Aqua, 0.4F), Color.Aqua, ExtraMath.Osc(0f, 1f, speed: 4f, offset: SegmentIndex));
        drawer.color *= 0.25F;
        drawer.color.A = 0;
        drawer.scale *= 0.8F;
        spriteBatch.Draw(drawer);
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        return false;
    }

    public override void ModifyIncomingHit(ref NPC.HitModifiers modifiers)
    {
        base.ModifyIncomingHit(ref modifiers);

        if (State == AIState.Passive || State == AIState.SuperAggressive)
            modifiers.FinalDamage *= 0.01F;
    }


    public override void HitEffect(NPC.HitInfo hit)
    {
        base.HitEffect(hit);
        _aggression++;
        if (State == AIState.Aggressive)
            _aggression += 2;
        _timer+= 5;
        _jiggle = 30;
        Parent.NPC.life -= hit.Damage;
        if (Parent.NPC.life <= 0)
            Parent.NPC.life = 1;
        Parent.aggression++;
    }

    public void DrawToRenderTargets()
    {
        if (!_outliner.VisibleEnough)
            return;
        void DrawOutline(SpriteBatch sb)
        {

            var tex = TextureAssets.Npc[Type].Value;
            var pos = NPC.Center;
            pos += _shakeOffset * MegaAggressionLerp;
            var drawer = SpritebatchDrawer.FromTextureGetLightingColor(tex, pos);
            drawer.VerticalFrame(Prismadolph.FRAME_CORE, Main.npcFrameCount[Type]);
            drawer.color = _outliner.outlineColor;
            drawer.CenterOrigin();
            drawer.rotation = _rot + JiggleRotOffset;
            sb.Draw(drawer);
        }
        OutlineRenderer.Queue(DrawOutline);
    }
}
