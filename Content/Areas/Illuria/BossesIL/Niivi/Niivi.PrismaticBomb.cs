using Stellamod.Content.Areas.Illuria.BossesIL.Niivi.Projectiles;
using Stellamod.Core.Camera;
using System;
using Terraria;

namespace Stellamod.Content.Areas.Illuria.BossesIL.Niivi;

public partial class Niivi
{
    float PrismaticBomb_PrepTime => 100;
    int PrismaticBomb_Damage => 78;
    float PrismaticBomb_AttackTime => 1380;
    void AI_PrismaticBomb()
    {
        ref Vector2 StartPosition = ref _vec1;
        ref Vector2 EndPosition = ref _vec2;
        ref Vector2 InitialVelocity = ref _vec3;

        Timer++;

        void HoverInPlace()
        {
            _simpleChainRendering = true;
            if (MathF.Abs(NPC.velocity.X) > 3)
                NPC.velocity *= 0.99F;
            NPC.velocity.X *= 0.999f;
            NPC.velocity.Y = MathHelper.Lerp(NPC.velocity.Y, MathF.Sin(Timer * 0.05f), MathF.Sin(Timer * 0.1f) * 0.5f + 0.5f);
            LimpIKTarget();
            var targetRot = NPC.velocity.ToRotation();
            NPC.rotation = Utils.AngleLerp(NPC.rotation, Utils.AngleLerp(NPC.rotation, targetRot, 0.2f), 0.04f);
        }

        switch (AttackCycle)
        {
            case 0:
                {
                    if(Timer == 1)
                    {
                        NPC.TargetClosest();
                        StartPosition = NPC.Center;
                        EndPosition = MyTarget.Center + new Vector2(0, -128);
                        InitialVelocity = NPC.velocity;
                    }
                    _isBLACK = true;
                    HoverInPlace();
                    CameraTargetSystem.AddTarget(Vector2.Lerp(Main.LocalPlayer.Center, NPC.Center, MathHelper.Lerp(0f, 0.5f, EasingFunction.InOutSine(Timer / 45F))));
                    if (Timer >= PrismaticBomb_PrepTime)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 1:
                {
                    if(Timer < 45)
                    {
                        CameraTargetSystem.AddTarget(Vector2.Lerp(Main.LocalPlayer.Center, NPC.Center, 0.5f)) ;
                    }
     
                    if (Timer == 45 && MultiplayerHelper.IsHost)
                    {
                        var firer = ProjFirer.From<NiiviPrismaticBomb>(NPC);
                        firer.ai2 = MyTarget.whoAmI;
                        firer.damage = PrismaticBomb_Damage;
                        firer.knockback = 1;
                        firer.New();
                    }
                    _isBLACK = true;
                    NPC.velocity *= 0.96F;
                    var targetRot = NPC.velocity.ToRotation();
                    NPC.rotation = Utils.AngleLerp(NPC.rotation, Utils.AngleLerp(NPC.rotation, targetRot, 0.2f), 0.04f);
                    //HoverInPlace();
                    if (Timer >= PrismaticBomb_AttackTime)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 2:
                {
                    SwitchState(AIState.Idle);
                }
                break;
        }
    }
}
