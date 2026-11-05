using Stellamod.Core.Camera;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using static Stellamod.Core.LocalizationReferences.Mods.Stellamod.ArmorReforge.Zemmified;

namespace Stellamod.Content.Areas.Illuria.BossesIL.Niivi;

public partial class Niivi
{
    float Idle_SwoopOutTime => 180;
    float IdleTime => 240;
    float Idle_HoverTowardSpeedUpDistance => 256 * 256;
    float Idle_HoverAboveDistance => 256;
    float Idle_HoverAwayDistance => 128;
    float Idle_MaxChaseSpeed => 27;
    float Idle_MinChaseSpeed => 6;
    float Idle_HoverTowardSpeed => 1;
    float Idle_Acceleration => 0.5F;
    void AI_IdleOut()
    {
        NPC.velocity.X += 0.05f;
        NPC.velocity.Y -= 0.135f;
        NPC.rotation = Utils.AngleLerp(NPC.rotation, NPC.velocity.ToRotation(), 0.1F);
        if (Timer >= Idle_SwoopOutTime)
        {
            Timer = 0;
            AttackCycle++;
            //ChooseAttack();
        }
    }
    void AI_Idle()
    {
        ref Vector2 StartPosition = ref _vec1;
        Timer++;

        CameraTargetSystem.AddTarget(Vector2.Lerp(Main.LocalPlayer.Center, NPC.Center, 0.02f));

        //Alright, completely reworking her movement cause she should NOT be doing the Eel thing.
        /*
         * 1 - She checks the distance to the player,
         * if you’re too far she’ll fly towards you, 
         * how I’m thinking this will working is she’ll go into her like swooping state and sin over to you and then go back into arc hovering,
         * so she needs to smoothly transition into her ik state so it flows nicely moving up and down
         */
        if (Timer % 30 == 0)
        {
            NPC.TargetClosest();
        }

        var positionAboutPlayToMoveTo = MyTarget.Center;
        var inverseXDirection = (positionAboutPlayToMoveTo.X < NPC.Center.X) ? 1 : -1;
        positionAboutPlayToMoveTo.Y -= Idle_HoverAboveDistance;
        positionAboutPlayToMoveTo.X += inverseXDirection * Idle_HoverAwayDistance;
        var sqrDistToTarget = Vector2.DistanceSquared(NPC.Center, positionAboutPlayToMoveTo);

        //This is what will control the flipping, she needs to flip in 3D btw.
        NPC.direction = (positionAboutPlayToMoveTo.X < NPC.Center.X) ? -1 : 1;
        switch (AttackCycle)
        {
            case 0:
                {
                    //Have niivi look at you while arcing, flying up and down
                    var oscillatedPositionToMoveTo = positionAboutPlayToMoveTo;
                    oscillatedPositionToMoveTo.Y += MathF.Sin(Timer * 0.04f) * 32;
                    var velToTarget = (oscillatedPositionToMoveTo - NPC.Center).SafeNormalize(Vector2.Zero);
                    var chaseVelocity = velToTarget * Idle_HoverTowardSpeed;
                    NPC.velocity = NPC.velocity.MoveTowards(chaseVelocity, 0.2f);

                    var lookRotation = (MyTarget.Center - NPC.Center).ToRotation();
                    NPC.rotation = Utils.AngleLerp(NPC.rotation, lookRotation, 0.1f);

                    _useFK = true;
                    FK_ArcBody(-0.2f, 0.1f);
                    if (sqrDistToTarget > Idle_HoverTowardSpeedUpDistance)
                    {
                        AttackCycle = 1;
                    }
                }
                break;
            case 1:
                {
                    //She'll speedu p and move towards you
                    var velToTarget = (positionAboutPlayToMoveTo - NPC.Center).SafeNormalize(Vector2.Zero);
                    var flyingSpeed = sqrDistToTarget / 256F;
                    flyingSpeed = MathHelper.Clamp(flyingSpeed, Idle_MinChaseSpeed, Idle_MaxChaseSpeed);
                    var chaseVelocity = velToTarget * flyingSpeed;

                    //While flying we want niivi to oscillate to the position
                    var osc = MathF.Sin(Timer * 0.04f) * 0.6F;
                    chaseVelocity = chaseVelocity.RotatedBy(osc);
                    NPC.velocity = NPC.velocity.MoveTowards(chaseVelocity, Idle_Acceleration);

                    var chaseRotation = NPC.velocity.ToRotation();
                    NPC.rotation = Utils.AngleLerp(NPC.rotation, chaseRotation, 0.2f);


                    //Easing between forms should not be handled in this logic, we'll do that elsewhere
                    //The same goes for flipping
                    
                    _simpleChainRendering = true;
                    LimpIKTarget();

                    //Once close go back to hovering
                    //We have padding here to prevent flickering
                    if(sqrDistToTarget < (Idle_HoverTowardSpeedUpDistance - 144))
                    {
                        AttackCycle = 0;
                    }
                }
                break;
        }

        //Once of the few idle states that has a bit more movement lol
        if(Timer >= IdleTime)
        {
            ChooseAttack();
        }
    }
}
