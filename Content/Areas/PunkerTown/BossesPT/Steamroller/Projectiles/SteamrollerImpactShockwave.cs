using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.PunkerTown.BossesPT.Steamroller.Projectiles;

/*
 * 
 *
 *X appears directly on the ground right underneath you and after a moment, 
 *a sound cue comes in too before and Steamroller drills through the ground at a fast pace, 
 *shooting into the air and then waiting a moment before trying to drill back down on top of you, you have to dodge twice

Steamroller pops its head out like snagrets and tries to start drilling on top of you but gets stuck in the ground with its head and he starts drilling, 
creating a bunch of flying rocks that come out to hit you

Steamroller comes out and starts to shoot little bombs from the side with like cool spell circles 
and stuff while being up in the air arched over

Dune jump, where he comes out of the ground over you and leaps over basically, you just have to not move for this

You see rocks rumbling under the ground as he starts doing a dung defender type attack, and stops and pokes his head out and goes back in for a minute

Phase two, he splits in half and basically this one goes on the other side of you, or it tries to attack right after the other, 
since this is a slow timing boss this will work

Pops off its head as it comes out the ground and shoots itself at you, detaching itself as the rest of the body goes underground,
the head drills into the ground as well and you just have to dodge really, it goes back underground after this attack to reconnect

 */


public class SteamrollerImpactShockwave : ModProjectile
{
    private ref float Timer => ref Projectile.ai[0];
    public override string Texture => TextureRegistry.EmptyTexture;
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 256;
        Projectile.height = 128;
        Projectile.hostile = true;
        Projectile.timeLeft = 120;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
    }
    public override void AI()
    {
        base.AI();
        Timer++;
        if (Timer > 20)
        {
            Projectile.hostile = false;
        }
        var shockwave = ScreenShader.GetInstance<SuperShockwave>();

        float ease = EasingFunction.OutCirc(Timer / 25);
        Vector2 center = Projectile.Center;
        Vector2 diff = center - Main.screenPosition;
        float x = diff.X / (float)Main.screenWidth;
        float y = diff.Y / (float)Main.screenHeight;
        Vector2 epicenter = new Vector2(x, y);
        shockwave.epicenter = epicenter;
        shockwave.radius = MathHelper.Lerp(0.3f, 0.6f, ease);
        shockwave.strength = 0.05f;
        shockwave.interp = MathHelper.Lerp(1f, 0f, ease);
        shockwave.alpha = 1;
    }
}
