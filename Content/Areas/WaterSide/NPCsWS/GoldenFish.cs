using Stellamod.Common;
using Stellamod.Common.Shaders;
using Stellamod.Helpers;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.WaterSide.NPCsWS;

public abstract class AbstractHarmonicFish : ModNPC
{
    private bool _contactDamage;
    private enum AIState
    {
        Idle,
        Swim
    }

    private ref float Timer => ref NPC.ai[0];
    private AIState State
    {
        get => (AIState)NPC.ai[1];
        set => NPC.ai[1] = (float)value;
    }
    private ref float SwimTimer => ref NPC.ai[2];

    public float idleTime;
    public float swimTime;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        this.AddToHarmonicCoralways();
    }

    public sealed override void SetDefaults()
    {
        base.SetDefaults();
        NPC.width = 64;
        NPC.height = 32;
        NPC.damage = 8;
        NPC.defense = 4;
        NPC.lifeMax = 100;
        NPC.life = 100;
        NPC.noGravity = true;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
        NPC.aiStyle = -1;

        idleTime = 60;
        swimTime = 240;
        SetDefaults2();
    }

    public virtual void SetDefaults2()
    {

    }


    public override bool CanHitPlayer(Player target, ref int cooldownSlot)
    {
        return base.CanHitPlayer(target, ref cooldownSlot) && _contactDamage;
    }

    public override void AI()
    {
        base.AI();
        if (Main.rand.NextBool(16))
        {
            var d = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.BubbleBlock);
            Main.dust[d].noGravity = true;

        }

        switch (State)
        {
            case AIState.Idle:
                AI_Idle();
                break;
            case AIState.Swim:
                AI_Swim();
                break;
        }

        NPC.spriteDirection = NPC.velocity.X > 0 ? 1 : -1;
        NPC.rotation = NPC.velocity.Y * 0.05f;
    }

    private void SwitchState(AIState state)
    {
        if (MultiplayerHelper.IsHost)
        {
            Timer = 0;
            State = state;
            NPC.netUpdate = true;
        }
    }

    private void AI_Idle()
    {
        Timer++;
        if(Timer == 1)
        {
            if (MultiplayerHelper.IsHost)
            {
                NPC.direction = Main.rand.NextBool(2) ? -1 : 1;
                NPC.netUpdate = true;
            }
        }

        NPC.velocity = Vector2.Lerp(NPC.velocity, Vector2.Zero, 0.01f);
        if (Timer >= idleTime)
        {
            SwitchState(AIState.Swim);
        }
    }

    private void AI_Swim()
    {
        Timer++;
        Vector2 targetVelocity = Vector2.UnitX * NPC.direction;
        targetVelocity *= 1;
        NPC.velocity = Vector2.Lerp(NPC.velocity, targetVelocity, 0.1f);
        NPC.velocity.Y += MathF.Sin(Timer * 0.05f) * 0.02f;

        SwimTimer--;
        if (SwimTimer <= 0)
        {
            if (NPC.collideX)
            {
                NPC.direction = -NPC.direction;
                SwimTimer = 80;
            }
        }
 
        if (Timer >= swimTime)
        {
            SwitchState(AIState.Idle);
        }
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        base.HitEffect(hit);
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        FlagWavingShader wavingShader = FlagWavingShader.Instance;
        wavingShader.OscStrength = 0.1f;
        wavingShader.XOffset = 5;
        wavingShader.Time = Main.GlobalTimeWrappedHourly * 2;

        spriteBatch.Restart(effect: wavingShader.Effect);
        SpritebatchDrawer drawer = SpritebatchDrawer.FromNPC(NPC);
        spriteBatch.Draw(drawer);
        spriteBatch.RestartDefaults();
        return false;
        //return base.PreDraw(spriteBatch, screenPos, drawColor);
    }

    public override void OnKill()
    {
        base.OnKill();
    }
    public override void ModifyNPCLoot(NPCLoot npcLoot)
    {
        base.ModifyNPCLoot(npcLoot);
    }
}

public class GoldenFish : AbstractHarmonicFish
{
    public override void SetDefaults2()
    {
        base.SetDefaults2();
    }
}
