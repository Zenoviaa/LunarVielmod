using Stellamod.Common.Particles;
using Stellamod.Visual.Particles;
using Terraria;

namespace Stellamod.Content.Rendering.MoonMagic;

public static class MothMoonMagicParticleUtilities
{
    public static void SpawnMoonySmokeParticles(in Vector2 position, in Vector2 velocity)
    {
        for (int i = 0; i < 2; i++)
        {
            Vector2 pos = position;
            pos += Main.rand.NextVector2Circular(32, 32);
            Particles.CometMagicDust.Spawn(CometMagicDustData.Default with
            {
                position = pos,
                velocity = -velocity.SafeNormalize(Vector2.Zero) * 3,
                color = Color.Blue,
                timeLeft = Main.rand.Next(15, 25)
            });
        }

        for (int i = 0; i < 1; i++)
        {
            Vector2 pos = position;
            pos += Main.rand.NextVector2Circular(32, 32);
            Particles.CometMagicDust.Spawn(CometMagicDustData.Default with
            {
                position = pos,
                velocity = -velocity.SafeNormalize(Vector2.Zero) * 3,
                color = Color.Cyan,
                timeLeft = Main.rand.Next(15, 25)
            });
        }
        if (Main.rand.NextBool(6))
        {

            var sp = SmokeParticle.SpawnInAlphaLayer(position + Main.rand.NextVector2Circular(32, 32), -velocity * 0.25f, Color.DarkBlue);
            sp.fast = true;
            sp.initialColor = Color.DarkBlue;
            sp.fadeToColor = Color.DarkGray;
            sp.Scale *= 1.5f;
            sp.behindLayer = true;
        }
        if (Main.rand.NextBool(24))
        {

            var sp = SparkleParticle.Spawn(position + Main.rand.NextVector2Circular(32, 32), -velocity * 0.25f, Color.DarkBlue);
            sp.fast = true;
            sp.behindLayer = true;
            sp.Scale *= 0.5f;
            sp.gravity = 0;
        }

        if (Main.rand.NextBool(6))
        {
            Particles.SwirlingFlameDust.Spawn(BitDustFactory.SlowingOverTime with
            {
                position = position + Main.rand.NextVector2Circular(32, 32),
                velocity = Main.rand.NextVector2Circular(8, 8) - velocity,
                innerColor = Color.LightBlue.ToVector4(),
                outerColor = Color.DarkBlue.ToVector4(),
                scale = Vector2.One * 0.6f
            });
        }
    }
}
