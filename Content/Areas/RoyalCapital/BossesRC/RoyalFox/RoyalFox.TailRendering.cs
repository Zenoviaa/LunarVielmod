using Stellamod.Common.Shaders;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using Stellamod.Trails;
using Terraria;

namespace Stellamod.Content.Areas.RoyalCapital.BossesRC.RoyalFox;

public partial class RoyalFox
{
    private void SimulateHairIK()
    {
        for (int i = 0; i < Tails.Length; i++)
        {
            var tail = Tails[i];
            ref Vector2 targetPosition = ref TailEndIK[i];
            float dist = Vector2.Distance(Rig.rootSegment.worldPosition, targetPosition);
            if (dist > 196 * 4)
            {
                Vector2 vec = (targetPosition - Rig.rootSegment.worldPosition);
                vec = vec.Resize(196 * 4);
                targetPosition = Rig.rootSegment.worldPosition + vec;
            }

            tail.IK(Rig.rootSegment.worldPosition, targetPosition);
        }

    }

    private void DragTailsLiimp()
    {
        for (int k = 0; k < 16; k++)
        {
            for (int i = 0; i < Tails.Length; i++)
            {
                Tails[i].segments[0].a = Rig.rootSegment.worldPosition;
                Tails[i].ResolveBackToRoot();
            }
        }
    }

    private void DragTailsLoose()
    {
        Vector2 centerPoint = Rig.rootSegment.worldPosition;
        centerPoint -= RegularRotation.ToRotationVector2() * 384;
        for (int i = 0; i < Tails.Length; i++)
        {
            Vector2 offset = Vector2.UnitY * -1 * 32;
            offset = offset.RotatedBy(Main.GlobalTimeWrappedHourly * 2 + i * 0.8f);

            Vector2 pos = centerPoint + offset;
            ref Vector2 endEffector = ref TailEndIK[i];
            endEffector = Vector2.Lerp(endEffector, pos, 0.2f);
            endEffector = pos;
        }
        SimulateHairIK();
    }

    private float GetHairWidth(float ratio)
    {
        return MathHelper.SmoothStep(80, 0, ratio) * _invisibleAlpha * EasingFunction.QuadraticBump(ratio);
    }

    private Color GetHairColor(float ratio)
    {
        return Color.White * _invisibleAlpha * EasingFunction.OutExpo(ratio + 0.5f);
    }

    private void DrawHair(GraphicsDevice gDevice)
    {
        void DrawHairIK()
        {
            for (int i = 0; i < Tails.Length; i++)
            {
                HairShader shader = ShaderContent.GetInstance<HairShader>();
                shader.LaserTexture = TrailRegistry.GlowTrailNoBlack;
                shader.Time = Main.GlobalTimeWrappedHourly * 0.2f + i * 0.6f;
                shader.WaveFrequency = 8;
                shader.XOffset = 12;

                var tail = Tails[i];
                Vector2[] points = new Vector2[tail.segments.Length];
                for (int p = 0; p < points.Length; p++)
                {
                    points[p] = tail.segments[p].a;
                }

                TrailDrawer.Draw(Main.spriteBatch, points, GetHairColor, GetHairWidth, shader);
            }
        }

        DrawHairIK();
    }
}
