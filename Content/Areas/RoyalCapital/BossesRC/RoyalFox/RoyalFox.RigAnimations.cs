using Stellamod.Helpers;
using Terraria;

namespace Stellamod.Content.Areas.RoyalCapital.BossesRC.RoyalFox;

public partial class RoyalFox
{

    private void AnimateFlying()
    {
        ResetW();
        float start = MathHelper.ToRadians(-2) * FacingDirectionToTarget;
        float end = MathHelper.ToRadians(2) * FacingDirectionToTarget;

        float runningSpeed = 4;
        float frontFrontLeg = ExtraMath.Osc(0f, 1f, speed: runningSpeed);

        Rig.frontFrontLeg[0].eulerAngles.Z = MathHelper.Lerp(start, end, frontFrontLeg);
        Rig.frontFrontLeg[0].eulerAngles.Z += MathHelper.ToRadians(52);
        Rig.frontFrontLeg[1].eulerAngles.Z = MathHelper.Lerp(start * 2, end * 2, frontFrontLeg);
        Rig.frontFrontLeg[2].angleOverride = MathHelper.ToRadians(45);

        float frontBackLeg = ExtraMath.Osc(0f, 1f, speed: runningSpeed, offset: 1);

        Rig.frontBehindLeg[0].eulerAngles.Z = MathHelper.Lerp(start, end, frontBackLeg);
        Rig.frontBehindLeg[0].eulerAngles.Z += MathHelper.ToRadians(52);
        Rig.frontBehindLeg[1].eulerAngles.Z = MathHelper.Lerp(start * 2, end * 2, frontBackLeg);
        Rig.frontBehindLeg[2].angleOverride = MathHelper.ToRadians(45);

        //Back Legs
        float backFrontLeg = ExtraMath.Osc(0f, 1f, speed: runningSpeed, offset: 3);

        Rig.backFrontLeg[0].eulerAngles.Z = MathHelper.Lerp(start, end, backFrontLeg);
        Rig.backFrontLeg[0].eulerAngles.Z += MathHelper.ToRadians(48);
        Rig.backFrontLeg[1].eulerAngles.Z = MathHelper.Lerp(start * 2, end * 2, backFrontLeg);


        float backBackLeg = ExtraMath.Osc(0f, 1f, speed: runningSpeed, offset: 3 + 1);

        Rig.backBehindLeg[0].eulerAngles.Z = MathHelper.Lerp(start, end, backBackLeg);
        Rig.backBehindLeg[0].eulerAngles.Z += MathHelper.ToRadians(48);
        Rig.backBehindLeg[1].eulerAngles.Z = MathHelper.Lerp(start * 2, end * 2, backBackLeg);
        start = MathHelper.ToRadians(8);
        start += MathHelper.ToRadians(2);
        end = start + MathHelper.ToRadians(7);

   
        float rotToPlayer = (MyTarget.Center - HeadPosition).ToRotation();
        for (int i = 0; i < Rig.bodyParts.Length; i++)
        {
            ref float zAngle = ref Rig.bodyParts[i].eulerAngles.Z;
            zAngle = MathHelper.Lerp(start, end, ExtraMath.Osc(0f, 1f, offset: i, speed: runningSpeed));
            if(i == 3)
            {
                zAngle += MathHelper.ToRadians(45);
       
                /*
                if (FacingDirectionToTarget == 1)
                    zAngle += MathHelper.ToRadians(60);
                else
                    zAngle += MathHelper.ToRadians(52);
                */           

                //


            }
        //    zAngle = 0;
       
        }


        Rig.headPart.fakeAngle = MathHelper.ToRadians(36 * FacingDirectionToTarget);

        /*
        Vector2 rootDirection = Vector2.UnitX * FacingDirectionToTarget;
        Vector2 directionToTarget = (MyTarget.Center - HeadPosition).SafeNormalize(Vector2.Zero);
        float dp = Vector2.Dot(rootDirection, directionToTarget);
        if(dp > 0.35f)
            Rig.headPart.angleOverride = rotToPlayer;*/
        if(FacingDirectionToTarget == -1)
        {
        //    Rig.headPart.angleOverride += MathHelper.ToRadians(135);
        }
    }
    private void AnimateStanding()
    {
        float start = MathHelper.ToRadians(-2);
        float end = MathHelper.ToRadians(2);

        float runningSpeed = 4;
        float frontFrontLeg = ExtraMath.Osc(0f, 1f, speed: runningSpeed);
        //     float easeing = EasingFunction.InOutSine(legPair1);
        Rig.frontFrontLeg[0].eulerAngles.Z = MathHelper.Lerp(start, end, frontFrontLeg);
        Rig.frontFrontLeg[1].eulerAngles.Z = MathHelper.Lerp(start * 2, end * 2, frontFrontLeg);

        float frontBackLeg = ExtraMath.Osc(0f, 1f, speed: runningSpeed, offset: 1);
        Rig.frontBehindLeg[0].eulerAngles.Z = MathHelper.Lerp(start, end, frontBackLeg);
        Rig.frontBehindLeg[1].eulerAngles.Z = MathHelper.Lerp(start * 2, end * 2, frontBackLeg);


        //Back Legs
        float backFrontLeg = ExtraMath.Osc(0f, 1f, speed: runningSpeed, offset: 3);
        Rig.backFrontLeg[0].eulerAngles.Z = MathHelper.Lerp(start, end, backFrontLeg);
        Rig.backFrontLeg[1].eulerAngles.Z = MathHelper.Lerp(start * 2, end * 2, backFrontLeg);


        float backBackLeg = ExtraMath.Osc(0f, 1f, speed: runningSpeed, offset: 3 + 1);
        Rig.backBehindLeg[0].eulerAngles.Z = MathHelper.Lerp(start, end, backBackLeg);
        Rig.backBehindLeg[1].eulerAngles.Z = MathHelper.Lerp(start * 2, end * 2, backBackLeg);


        float headRotOffset = MathHelper.Lerp(start, end, ExtraMath.Osc(0f, 1f, speed: runningSpeed));
        Rig.bodyParts[3].eulerAngles.Z = MathHelper.ToRadians(19) + headRotOffset;
    }

    private void AnimateRunning()
    {
        float start = MathHelper.ToRadians(-25);
        float end = MathHelper.ToRadians(25);

        float runningSpeed = 9;
        float frontFrontLeg = ExtraMath.Osc(0f, 1f, speed: runningSpeed);
        Rig.frontFrontLeg[0].eulerAngles.Z = MathHelper.Lerp(start, end, frontFrontLeg);
        Rig.frontFrontLeg[1].eulerAngles.Z = MathHelper.Lerp(start * 2, end * 2, frontFrontLeg);

        float frontBackLeg = ExtraMath.Osc(0f, 1f, speed: runningSpeed, offset: 1);
        Rig.frontBehindLeg[0].eulerAngles.Z = MathHelper.Lerp(start, end, frontBackLeg);
        Rig.frontBehindLeg[1].eulerAngles.Z = MathHelper.Lerp(start * 2, end * 2, frontBackLeg);

        //Back Legs
        float backFrontLeg = ExtraMath.Osc(0f, 1f, speed: runningSpeed, offset: 3);
        Rig.backFrontLeg[0].eulerAngles.Z = MathHelper.Lerp(start, end, backFrontLeg);
        Rig.backFrontLeg[1].eulerAngles.Z = MathHelper.Lerp(start * 2, end * 2, backFrontLeg);


        float backBackLeg = ExtraMath.Osc(0f, 1f, speed: runningSpeed, offset: 3 + 1);
        Rig.backBehindLeg[0].eulerAngles.Z = MathHelper.Lerp(start, end, backBackLeg);
        Rig.backBehindLeg[1].eulerAngles.Z = MathHelper.Lerp(start * 2, end * 2, backBackLeg);


        Rig.bodyParts[3].eulerAngles.Z = MathHelper.ToRadians(15);
    }

    private void ResetW()
    {
        for (int i = 1; i < Rig.bodyParts.Length; i++)
        {
            var part = Rig.bodyParts[i];
            part.eulerAngles.W = 0;// MathHelper.Lerp(radians, 0, (float)i / (float)Rig.bodyParts.Length);
        }
    }
    private void AnimateC()
    {
        for (int i = 1; i < Rig.bodyParts.Length; i++)
        {
            var part = Rig.bodyParts[i];
            float ratio = (float)i / (float)Rig.bodyParts.Length;
            float radians = MathHelper.ToRadians(MathHelper.Lerp(0, 130, ratio * _spinningCRot));
            part.eulerAngles.W = radians;// MathHelper.Lerp(radians, 0, (float)i / (float)Rig.bodyParts.Length);
        }
   //     Rig.bodyParts[0].eulerAngles.W += _spinningCRot;
  ;
    }
    private void AnimateTorpedo()
    {
        Rig.headPart.fakeAngle = 0;
        float start = MathHelper.ToRadians(65);
        float end = start + MathHelper.ToRadians(2);

        float runningSpeed = 9;
        float frontFrontLeg = ExtraMath.Osc(0f, 1f, speed: runningSpeed);
        float targetAngle = MathHelper.Lerp(start, end, frontFrontLeg);
        Rig.frontFrontLeg[0].eulerAngles.Z = MathHelper.Lerp(Rig.frontFrontLeg[0].eulerAngles.Z, targetAngle, 0.1f);
        Rig.frontFrontLeg[1].eulerAngles.Z = 0;

        float frontBackLeg = ExtraMath.Osc(0f, 1f, speed: runningSpeed, offset: 1);
        targetAngle = MathHelper.Lerp(start, end, frontBackLeg);

        Rig.frontBehindLeg[0].eulerAngles.Z = MathHelper.Lerp(Rig.frontBehindLeg[0].eulerAngles.Z, targetAngle, 0.1f);
        Rig.frontBehindLeg[1].eulerAngles.Z = 0;

        //Back Legs
        float backFrontLeg = ExtraMath.Osc(0f, 1f, speed: runningSpeed, offset: 3);
        start = MathHelper.ToRadians(65);
        end = start + MathHelper.ToRadians(2);
        targetAngle = MathHelper.Lerp(start, end, backFrontLeg);
        Rig.backFrontLeg[0].fakeAngle = MathHelper.Lerp(Rig.backFrontLeg[0].fakeAngle, MathHelper.ToRadians(35), 0.1f);
        Rig.backFrontLeg[0].eulerAngles.Z = MathHelper.Lerp(Rig.backFrontLeg[0].eulerAngles.Z, targetAngle, 0.1f);
        Rig.backFrontLeg[1].eulerAngles.Z = 0;
        Rig.backFrontLeg[2].fakeAngle = MathHelper.Lerp(Rig.backFrontLeg[2].fakeAngle, MathHelper.ToRadians(45), 0.1f);

        float backBackLeg = ExtraMath.Osc(0f, 1f, speed: runningSpeed, offset: 3 + 1);
        targetAngle = MathHelper.Lerp(start, end, backBackLeg);
        Rig.backBehindLeg[0].fakeAngle = MathHelper.Lerp(Rig.backBehindLeg[0].fakeAngle, MathHelper.ToRadians(35), 0.1f);
        Rig.backBehindLeg[0].eulerAngles.Z = MathHelper.Lerp(Rig.backBehindLeg[0].eulerAngles.Z, targetAngle, 0.1f);
        Rig.backBehindLeg[1].eulerAngles.Z = 0;
        Rig.backBehindLeg[2].fakeAngle = MathHelper.Lerp(Rig.backBehindLeg[2].fakeAngle, MathHelper.ToRadians(-45), 0.1f);


        float rotToPlayer = (MyTarget.Center - HeadPosition).ToRotation();
        for (int i = 0; i < Rig.bodyParts.Length; i++)
        {
            ref float zAngle = ref Rig.bodyParts[i].eulerAngles.Z;
            zAngle = 0;
            if (i == 3)
            {

                /*
                if (FacingDirectionToTarget == 1)
                    zAngle += MathHelper.ToRadians(60);
                else
                    zAngle += MathHelper.ToRadians(52);
                */

                //


            }

        }

        Rig.bodyParts[3].eulerAngles.Z = MathHelper.ToRadians(15);
    }
    private void AnimateStretched()
    {
        float start = MathHelper.ToRadians(-45);
        float end = MathHelper.ToRadians(-15);

        float runningSpeed = 9;
        float frontFrontLeg = ExtraMath.Osc(0f, 1f, speed: runningSpeed);
        Rig.frontFrontLeg[0].eulerAngles.Z = MathHelper.Lerp(start, end, frontFrontLeg);
        Rig.frontFrontLeg[1].eulerAngles.Z = MathHelper.Lerp(start * 2, end * 2, frontFrontLeg);

        float frontBackLeg = ExtraMath.Osc(0f, 1f, speed: runningSpeed, offset: 1);
        Rig.frontBehindLeg[0].eulerAngles.Z = MathHelper.Lerp(start, end, frontBackLeg);
        Rig.frontBehindLeg[1].eulerAngles.Z = MathHelper.Lerp(start * 2, end * 2, frontBackLeg);

        start = MathHelper.ToRadians(45);
        end = MathHelper.ToRadians(15);

        //Back Legs
        float backFrontLeg = ExtraMath.Osc(0f, 1f, speed: runningSpeed, offset: 3);
        Rig.backFrontLeg[0].eulerAngles.Z = MathHelper.Lerp(start, end, backFrontLeg);
        Rig.backFrontLeg[1].eulerAngles.Z = MathHelper.Lerp(start * 2, end * 2, backFrontLeg);


        float backBackLeg = ExtraMath.Osc(0f, 1f, speed: runningSpeed, offset: 3 + 1);
        Rig.backBehindLeg[0].eulerAngles.Z = MathHelper.Lerp(start, end, backBackLeg);
        Rig.backBehindLeg[1].eulerAngles.Z = MathHelper.Lerp(start * 2, end * 2, backBackLeg);


        Rig.bodyParts[3].eulerAngles.Z = MathHelper.ToRadians(15);
    }

}
