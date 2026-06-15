using Stellamod.Assets;
using Stellamod.Content.Areas.PunkerTown.BossesPT.Gothivia.Projectiles;
using Stellamod.Core;
using Stellamod.Core.Particles;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.PunkerTown.BossesPT.Gothivia;

public partial class Gothivia
{
    public void CreateInCircle()
    {
        PixelPrimitiveCircleFactory.CreateGenericInBoom(NPC.Center, Color.White, Color.White, 45, 444);
    }
    public void MakeCircles(in float timer)
    {
        if(timer % 6 == 0)
        {
            var gd = LegacyParticle.NewParticle<GlowDonutParticle>(NPC.Center, -NPC.velocity.SafeNormalize(Vector2.Zero));
            gd.innerColor = Color.Yellow;
            gd.outerColor = Color.Red;
            gd.fadeToColor = Color.DarkRed;
        }
    }
    public void TeleportEffect(Vector2 position)
    {
        if (MultiplayerHelper.IsHost)
        {
            Projectile.NewProjectile(SourceFromThis, position, Vector2.Zero, ModContent.ProjectileType<GothinTorch>(), 1, 1, Main.myPlayer, ai2: 4);
        }

        ShakeScreenPosition.Shake = 4;
        SoundStyle fireShoot = new SoundStyle("Stellamod/Assets/Sounds/Fire/FireballShoot1");  //AssetReferences.Assets.Sounds.Fire.FireballShoot1.Asset;
        fireShoot.PitchVariance = 0.5f;
        fireShoot.MaxInstances = 0;
        SoundEngine.PlaySound(fireShoot, position);
        var fx = FXUtil.GlowCircleBoom(position, Color.White, Color.Yellow, Color.Red, duration: 45, baseSize: 0.24f);
        fx.Scale *= 2f;
        for (float n = 0; n < 24; n++)
        {
            var dp = DustParticle.Spawn(position, Main.rand.NextVector2Circular(24, 24));
            dp.dampening = 0.05f;
            dp.gravity = 0;
            dp.noTileCollide = true;
        }
        PixelPrimitiveCircleFactory.CreateGenericBoom(position, Color.Red, Color.Red, 45, 256);
    }

    public static void PlayBlowtorchSound(Vector2 position)
    {
        int rand = Main.rand.Next(3);
        SoundStyle fireSound;
        switch (rand)
        {
            default:
            case 0:
                fireSound = AssetRegistry.Sounds.Fire.BlowtorchBigger1;
                break;
            case 1:
                fireSound = AssetRegistry.Sounds.Fire.BlowtorchBigger2;
                break;
            case 2:
                fireSound = new SoundStyle("Stellamod/Assets/Sounds/GothingBow");
                break;
        }
        fireSound.PitchVariance = 0.55f;
        SoundEngine.PlaySound(fireSound, position);
    }
    public static void ChargeParticlesBig(Vector2 center, in float timer)
    {
        if (timer % 4 == 0)
        {
            Vector2 pos = center + Main.rand.NextVector2CircularEdge(768, 768);
            Vector2 vel = (center - pos);
            vel *= 0.09f;
            var fx = FXUtil.GlowStretch(pos, vel);
            fx.VectorScale *= 0.5f;
            fx.OuterGlowColor = Color.Red;
        }

        if (timer % 4 == 0)
        {
            Vector2 pos = center + Main.rand.NextVector2CircularEdge(768, 768);
            Vector2 vel = (center - pos);
            vel *= 0.09f;
            var fx = FXUtil.GlowStretch(pos, vel);
            fx.OuterGlowColor = Color.Red;
        }
        
        if (timer % 4 == 0)
        {
            Vector2 pos = center + Main.rand.NextVector2CircularEdge(768, 768);
            Vector2 vel = (center - pos);
            vel *= 0.09f;
            var fx = FXUtil.GlowStretch(pos, vel);
            fx.OuterGlowColor = Color.Red;
        }

        if (timer % 4 == 0)
        {
            Vector2 pos = center + Main.rand.NextVector2CircularEdge(768, 768);
            Vector2 vel = (center - pos);
            vel *= 0.09f;
            var dp = DustParticle.Spawn(pos, vel);
            dp.dampening = 0.1f;
            dp.noTileCollide = true;
            dp.Scale *= 0.35f;
            dp.outerColor = Color.Red;
            dp.gravity = 0;
        }
    }
}
