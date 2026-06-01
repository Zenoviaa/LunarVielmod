using Stellamod.Helpers;
using Terraria;

namespace Stellamod.Content.Areas.RoyalCapital.BossesRC.RoyalFox;

public partial class RoyalFox
{

    private void AnimateFlying()
    {
        float start = MathHelper.ToRadians(-2);
        float end = MathHelper.ToRadians(2);

        float runningSpeed = 4;
        float frontFrontLeg = ExtraMath.Osc(0f, 1f, speed: runningSpeed);
        //     float easeing = EasingFunction.InOutSine(legPair1);
        Rig.frontFrontLeg[0].eulerAngles.Z = MathHelper.Lerp(start, end, frontFrontLeg);
        Rig.frontFrontLeg[0].eulerAngles.Z += MathHelper.ToRadians(42);
        Rig.frontFrontLeg[1].eulerAngles.Z = MathHelper.Lerp(start * 2, end * 2, frontFrontLeg);

        float frontBackLeg = ExtraMath.Osc(0f, 1f, speed: runningSpeed, offset: 1);
        Rig.frontBehindLeg[0].eulerAngles.Z = MathHelper.Lerp(start, end, frontBackLeg);
        Rig.frontBehindLeg[0].eulerAngles.Z += MathHelper.ToRadians(42);
        Rig.frontBehindLeg[1].eulerAngles.Z = MathHelper.Lerp(start * 2, end * 2, frontBackLeg);


        //Back Legs
        float backFrontLeg = ExtraMath.Osc(0f, 1f, speed: runningSpeed, offset: 3);
        Rig.backFrontLeg[0].eulerAngles.Z = MathHelper.Lerp(start, end, backFrontLeg);
        Rig.backFrontLeg[0].eulerAngles.Z += MathHelper.ToRadians(40);
        Rig.backFrontLeg[1].eulerAngles.Z = MathHelper.Lerp(start * 2, end * 2, backFrontLeg);


        float backBackLeg = ExtraMath.Osc(0f, 1f, speed: runningSpeed, offset: 3 + 1);
        Rig.backBehindLeg[0].eulerAngles.Z = MathHelper.Lerp(start, end, backBackLeg);
        Rig.backBehindLeg[0].eulerAngles.Z += MathHelper.ToRadians(40);
        Rig.backBehindLeg[1].eulerAngles.Z = MathHelper.Lerp(start * 2, end * 2, backBackLeg);
        start = MathHelper.ToRadians(4);
        end = MathHelper.ToRadians(9);
        for (int i = 0; i < Rig.bodyParts.Length; i++)
        {
            ref float zAngle = ref Rig.bodyParts[i].eulerAngles.Z;
            zAngle = MathHelper.Lerp(start, end, ExtraMath.Osc(0f, 1f, offset: i, speed: runningSpeed));
            if(i == 3)
            {
                float rotToPlayer = (MyTarget.Center - HeadPosition).ToRotation();
                zAngle = rotToPlayer;
            }
        }

        Rig.headPart.fakeAngle = MathHelper.ToRadians(45);
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
