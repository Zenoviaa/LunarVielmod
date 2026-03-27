using Stellamod.Common;
using Stellamod.Common.Shaders;
using Stellamod.Content.CommonMaterials;
using Stellamod.Helpers;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.WaterSide.NPCsWS;

/// <summary>
/// Batches all the fish draw calls together since they use a shader
/// </summary>
[Autoload(Side = ModSide.Client)]
public class HarmonicFishRenderer : ModSystem
{
    private Queue<SpritebatchDrawAction> _drawActionQueue;
    public delegate void SpritebatchDrawAction(SpriteBatch sb);

    public override void Load()
    {
        base.Load();
        _drawActionQueue = new Queue<SpritebatchDrawAction>();
        On_Main.DoDraw_DrawNPCsOverTiles += RenderFishies;
    }

    public override void Unload()
    {
        base.Unload();
        _drawActionQueue.Clear();
        _drawActionQueue = null;
        On_Main.DoDraw_DrawNPCsOverTiles -= RenderFishies;
    }

    public void QueueDraw(SpritebatchDrawAction drawAction)
    {
        _drawActionQueue.Enqueue(drawAction);
    }

    private void RenderFishies(On_Main.orig_DoDraw_DrawNPCsOverTiles orig, Main self)
    {
        orig(self);
        
        //Not sure why I'm putting this here but yeah
        if (Main.gameMenu)
            return;

        if (_drawActionQueue.Count <= 0)
            return;

        SpriteBatch sb = Main.spriteBatch;
        FlagWavingShader wavingShader = FlagWavingShader.Instance;
        wavingShader.OscStrength = 0.1f;
        wavingShader.XOffset = 5;
        wavingShader.Time = Main.GlobalTimeWrappedHourly * 2;

        sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, 
            Main.Rasterizer, wavingShader.Effect, Main.GameViewMatrix.TransformationMatrix);
        while(_drawActionQueue.Count > 0)
        {
            _drawActionQueue.Dequeue().Invoke(sb);
        }
        sb.End();
    }
}

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

    private void Draw(SpriteBatch sb)
    {
        SpritebatchDrawer drawer = SpritebatchDrawer.FromNPC(NPC);
        sb.Draw(drawer);
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        ModContent.GetInstance<HarmonicFishRenderer>().QueueDraw(Draw);
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
        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<MusicalHarmonise>(), minimumDropped: 2, maximumDropped: 4));
    }
}
