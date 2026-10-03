using Stellamod.Common.Particles;
using Stellamod.Core;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.MoonspiralTower.EnemiesMT;

public class PearlbornCrystal : ModNPC
{
    enum AIState : byte
    {
        Idle,
        PickedUp,
        Throw
    }
    ref float Timer => ref NPC.ai[0];
    AIState State
    {
        get => (AIState)NPC.ai[1];
        set => NPC.ai[1] = (float)value;
    }
    Player PickedUpTarget
    {
        get => Main.player[(int)NPC.ai[2]];
        set => NPC.ai[2] = (int)value.whoAmI;
    }
    float ThrowSpeed => 8;
    float PickupRange => 128;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        NPCID.Sets.TrailCacheLength[Type] = 8;
        NPCID.Sets.TrailingMode[Type] = 0;
    }

    public override bool CanHitPlayer(Player target, ref int cooldownSlot)
    {
        return false;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        NPC.width = NPC.height = 32;
        NPC.damage = 1;
        NPC.defense = 9999;
        NPC.lifeMax = 15;

    }

    public override void AI()
    {
        base.AI();
        switch (State)
        {
            case AIState.Idle:
                AI_Idle();
                break;
            case AIState.PickedUp:
                AI_PickedUp();
                break;
            case AIState.Throw:
                AI_Throw();
                break;
        }
        if (Main.rand.NextBool(8))
        {
            SpawnParticles();
        }
        NPC.rotation = Utils.AngleLerp(NPC.rotation, NPC.velocity.X * 0.05f, 0.1f);
        Lighting.AddLight(NPC.Center, TorchID.White);
    }
    void SwitchState(AIState state)
    {
        if (MultiplayerHelper.IsHost)
        {
            Timer = 0;
            State = state;
            NPC.netUpdate = true;
        }
    }

    void AI_Idle()
    {
        Timer++;
        NPC.velocity.X *= 0.94f;
        NPC.TargetClosest();
        var playerWhoCanPickMeUp = Main.player[NPC.target];
        if (playerWhoCanPickMeUp.controlUseItem && Vector2.Distance(playerWhoCanPickMeUp.Center, NPC.Center) <= PickupRange)
        {
            PickedUpTarget = playerWhoCanPickMeUp;
            SwitchState(AIState.PickedUp);
        }
    }

    void SpawnParticles()
    {
        var pos = NPC.Center + Main.rand.NextVector2Circular(32, 32);
        var vel = Main.rand.NextVector2Circular(1, 3);
        Particles.SwirlingFlameDust.Spawn(BitDustFactory.SlowingOverTime with
        {
            position = pos,
            velocity = vel,
            innerColor = Color.SkyBlue.ToVector4(),
            outerColor = Color.DarkBlue.ToVector4(),
            scale = new Vector2(Main.rand.NextFloat(0.5f, 1f)),
            timeLeft = 120
        });
    }

    void AI_PickedUp()
    {
        Timer++;
        var posToMoveTo = PickedUpTarget.Center;
        posToMoveTo.Y -= 64;
        NPC.Center = posToMoveTo;
        var rotation = (posToMoveTo - PickedUpTarget.Center).ToRotation();
        rotation -= MathHelper.PiOver2;
        PickedUpTarget.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, rotation);
        if (PickedUpTarget.controlUseItem && Timer >= 15)
        {
            var throwVelocity = new Vector2(PickedUpTarget.direction, 0);
            throwVelocity *= ThrowSpeed;
            throwVelocity.Y -= 5;
            NPC.velocity = throwVelocity;
            SwitchState(AIState.Throw);
        }
    }

    void AI_Throw()
    {
        Timer++;
        NPC.rotation += MathF.Sign(NPC.velocity.X) * 0.07f;
        NPC.velocity.Y += 0.04f;
        if(NPC.collideX || NPC.collideY)
        {
            Break();
        }
    }

    void Break()
    {
        var deathSound = SoundID.DD2_CrystalCartImpact;
        SoundEngine.PlaySound(deathSound, NPC.position);
        FXUtil.GlowCircleBoom(NPC.Center, Color.White, Color.SkyBlue, Color.DarkBlue, duration: 0.22f, baseSize: 0.18f);
        for(var i = 0; i < 48; i++)
        {
            var pos = NPC.Center + Main.rand.NextVector2Circular(32, 32);
            var vel = Main.rand.NextVector2Circular(12, 12);
            Particles.SwirlingFlameDust.Spawn(BitDustFactory.SlowingOverTime with
            {
                position = pos,
                velocity = vel,
                innerColor = Color.SkyBlue.ToVector4(),
                outerColor = Color.DarkBlue.ToVector4(),
                scale = new Vector2(Main.rand.NextFloat(0.5f, 1f)),
                timeLeft = 120
            });
        }

        if (MultiplayerHelper.IsHost)
        {
            NPC.NewNPC(NPC.GetSource_FromThis(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<MoonAura>(), ai1: 1200);
        }
        NPC.active = false;
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        var glowDrawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Assets.GlowMasks.SimpleGlowCircle.Asset, NPC.Center);
        glowDrawer.color = Color.SkyBlue * ExtraMath.Osc(0.9f, 1f, speed: 2) * 0.45f;
        glowDrawer.color.A = 0;
        glowDrawer.scale *= ExtraMath.Osc(0.9f, 1f, speed: 3) * 0.25f;
        spriteBatch.Draw(glowDrawer);
        var drawer = SpritebatchDrawer.FromNPC(NPC);
        for(var i = 0; i < NPC.oldPos.Length; i++)
        {
            var op = NPC.oldPos[i] + NPC.Size * 0.5f;
            var afDrawer = drawer;
            afDrawer.worldPosition = op;
            var ratio = (float)i / (float)NPC.oldPos.Length;
            afDrawer.color = Color.Lerp(Color.SkyBlue, Color.Transparent, ratio) * 0.1f;
            spriteBatch.Draw(afDrawer);
        }

        drawer.worldPosition.Y += ExtraMath.Osc(-2f, 2f, speed: 1, offset: NPC.whoAmI);
        spriteBatch.Draw(drawer);


        return false;
    }
    
    public override void HitEffect(NPC.HitInfo hit)
    {
        base.HitEffect(hit);
    }

    public override void OnKill()
    {
        base.OnKill();
    }
}

