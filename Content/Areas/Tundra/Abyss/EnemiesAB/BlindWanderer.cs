using Stellamod.Assets;
using Stellamod.Assets.ContentReader.Aseprite;
using Stellamod.Common;
using Stellamod.Core.NPCHelpers;
using Stellamod.Core.Particles;
using Stellamod.Visual.Particles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.Abyss.EnemiesAB;

internal class BlindWanderer : ModNPC
{
    private enum AIState
    {
        Wander,
        LanternUp,
        LanternHold,
        LanternDown,
        HELPMEMEMEMEM
    }


    private float _lanternScale;
    private ref float Timer => ref NPC.ai[0];
    private AIState State
    {
        get
        {
            return (AIState)NPC.ai[1];
        }
        set
        {
            NPC.ai[1] = (float)value;
        }
    }
    private ref float SpawnedMoth => ref NPC.ai[2];

    private Player MyTarget => Main.player[NPC.target];
    public override string Texture => TextureRegistry.EmptyTexture;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        NPCSets.UseAseprite[Type] = true;
        this.AddToAbyss();
    }

    public override bool CanHitPlayer(Player target, ref int cooldownSlot)
    {
        return false;
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        NPC.width = 40;
        NPC.height = 37;
        NPC.damage = 34;
        NPC.defense = 8;
        NPC.lifeMax = 140;
        NPC.HitSound = SoundID.NPCHit48;
        NPC.DeathSound = SoundID.DD2_SkeletonDeath;
        NPC.value = 563f;
        NPC.knockBackResist = .45f;
        NPC.aiStyle = -1;
    }

    public override void AI()
    {
        base.AI();
        NPC.TargetClosest(faceTarget: false);
        if(State == AIState.Wander || State == AIState.LanternDown)
        {
            _lanternScale = MathHelper.Lerp(_lanternScale, 0.2f, 0.1f);
        }
        else
        {
            _lanternScale = MathHelper.Lerp(_lanternScale, 1f, 0.1f);
        }
        switch (State)
        {
            case AIState.Wander:
                AI_Wander();

                break;
            case AIState.LanternUp:
                AI_LanternUp();
                break;
            case AIState.LanternHold:
                AI_LanternHold();
                break;
            case AIState.LanternDown:
                AI_LanternDown();
                break;
            case AIState.HELPMEMEMEMEM:
                AI_HelpMe();
                break;
        }

        this.SetDrawOrigin(new Vector2(22, 50));
        NPC.spriteDirection = NPC.direction;
        Lighting.AddLight(NPC.Center, TorchID.Ice);
    }
    private void SwitchState(AIState state)
    {
        if (MultiplayerHelper.IsHost)
        {
            Timer = 0;
            if(state == AIState.Wander)
            {
                Timer = Main.rand.Next(120, 200);

                //unsure if this is netsynced, I'll check
                NPC.direction = Main.rand.NextBool(2) ? -1 : 1;
            }
            if (state == AIState.LanternHold)
            {
                Timer = Main.rand.Next(120, 200);
            }
            State = state;
            NPC.netUpdate = true;
        }
    }

    private void ChooseNextState()
    {
        if (Main.rand.NextBool(2))
        {
            SwitchState(AIState.LanternUp);
        }
        else
        {
            SwitchState(AIState.Wander);
        }
    }

  
    private void AI_HelpMe()
    {
        SpawnedMoth = 1;
        Timer++;
        if(Timer == 1)
        {
            SoundStyle s = new SoundStyle("Stellamod/Assets/Sounds/Jack_Jump") with { PitchVariance = 0.3f };
            SoundEngine.PlaySound(s, NPC.position);
            NPC.velocity.Y = -6;
        }

        NPC.velocity.X *= 0.94f;
        this.GetAnimator().PlayAnimation("LanternUp", AnimationParams.Default with { IsLooping = false });
        if (Timer >= 60)
        {

            Vector2 pos = NPC.Center;
            pos.Y -= 368;
            FXUtil.GlowCircleBoom(pos, Color.White, Color.SkyBlue, Color.DarkBlue, duration: 35, baseSize: 0.24f);
            PixelPrimitiveCircleFactory.CreateGenericBoom(pos, Color.White, Color.SkyBlue, 60, 256);

            SoundStyle s = new SoundStyle("Stellamod/Assets/Sounds/IceyWind") with { PitchVariance = 0.3f };
            SoundEngine.PlaySound(s, NPC.position);
            if (MultiplayerHelper.IsHost)
            {
                NPC.NewNPC(NPC.GetSource_FromThis(), (int)pos.X, (int)pos.Y, ModContent.NPCType<BlindMoth>());
            }
            SwitchState(AIState.LanternDown);
        }
    }
    private void AI_Wander()
    {
        Timer--;
        float targetX = NPC.direction * 0.4f;
        NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, targetX, 0.02f);

        this.GetAnimator().PlayAnimation("Walk");
        if (Timer <= 0)
        {
            ChooseNextState();
        }

        float distTOTarget = Vector2.Distance(NPC.Center, MyTarget.Center);
        if(distTOTarget <= 64 && SpawnedMoth < 1)
        {
            SwitchState(AIState.HELPMEMEMEMEM);
        }
    }

    private void AI_LanternUp()
    {
        Timer++;
        NPC.velocity.X *= 0.8f;
        this.GetAnimator().PlayAnimation("LanternUp", AnimationParams.Default with { IsLooping = false });
        if(Timer >= 40)
        {
            SwitchState(AIState.LanternHold);
        }
    }

    private void AI_LanternHold()
    {
        Lighting.AddLight(NPC.Center, TorchID.Torch);
        Timer--;
        this.GetAnimator().PlayAnimation("LanternHold", AnimationParams.Default with { IsLooping = false });
        NPC.velocity.X *= 0.8f;
        if(Timer <= 0)
        {
            SwitchState(AIState.LanternDown);
        }
    }

    private void AI_LanternDown()
    {
        Timer++;
        NPC.velocity.X *= 0.8f;
        this.GetAnimator().PlayAnimation("LanternDown", AnimationParams.Default with { IsLooping = false });
        if (Timer >= 40)
        {
            SwitchState(AIState.Wander);
        }
    }


    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        NPC.DrawAnimator(spriteBatch, drawColor);
        return false;
    }

    public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        base.PostDraw(spriteBatch, screenPos, drawColor);
        Texture2D glowCircle = AssetManager.GlowMask.SimpleGlowCircle.Value;
        SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(glowCircle, NPC.Center +  Vector2.UnitX * NPC.direction * 18 * _lanternScale);
        drawer.color = Color.OrangeRed * ExtraMath.Osc(0.5f, 1f, speed: 3) * 0.2f * _lanternScale;
        drawer.color.A = 0;
        drawer.scale *= 0.5f * _lanternScale;
        spriteBatch.Draw(drawer);
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        base.HitEffect(hit);
        AbyssEnemyCommon.HitAndDeathEffects(NPC);
    }
    public override void OnKill()
    {
        base.OnKill();
    }
}
