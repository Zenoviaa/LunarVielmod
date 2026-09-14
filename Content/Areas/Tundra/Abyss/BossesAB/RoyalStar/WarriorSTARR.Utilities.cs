using Stellamod.Content.Areas.Tundra.Abyss.BossesAB.RoyalStar.Projectiles;
using System;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.Abyss.BossesAB.RoyalStar;

public partial class WarriorSTARR
{
    public void FaceTarget()
    {
        int dir = MyTarget.Center.X > NPC.Center.X ? 1 : -1;
        NPC.spriteDirection = dir;
    }

    public bool IsGrounded()
    {
        Point b = NPC.Bottom.ToTileCoordinates();
        b.Y++;
        Tile tile = Main.tile[b];
        if (tile.HasTile && WorldGen.SolidOrSlopedTile(tile))
            return true;
        return false;
    }

    public void StayGrounded()
    {
        NPC.velocity.X *= 0.94f;
        NPC.noGravity = false;
        NPC.noTileCollide = false;
    }
    public void LaughSound()
    {

    }

    public void MakeCapeInEffect()
    {

    }

    public void MakeCapeOutEffect()
    {

    }
    public void TeleportToAnArenaSide()
    {

    }

    public void MakeEyeFlashParticle()
    {

    }

    public void MakeFallingCrashParticles()
    {

    }

    public void MakeJumpingParticles()
    {
        //Probably a bunch of rocks, stars, smoke and stuff
        //We can make a rock particle :P 
    }

    public void GruntSound()
    {

    }

    public void KickSound()
    {

    }

    public void BigGruntSound()
    {

    }

    public void AirSwooshSound()
    {

    }

    public void CrashVFX(Vector2 position, Vector2 velocity)
    {

    }

    public void KickImpactVFX(Vector2 position, Vector2 velocity)
    {
        //Thinking some like circles and dust or something
    }

    public void PunchVFX(Vector2 position, Vector2 velocity)
    {

    }

    public void PunchBoulder(int index, Vector2 velocity)
    {
        int projType = ModContent.ProjectileType<STARBOULDER>();
        foreach(var proj in Main.ActiveProjectiles)
        {
            if (proj.type == projType && proj.ai[0] == NPC.whoAmI && proj.ai[1] == index)
            {
                proj.ai[2] = 10 + velocity.ToRotation();
            }
        }
    }

    public bool HasAnotherBoulder()
    {
        int projType = ModContent.ProjectileType<STARBOULDER>();
        foreach (var proj in Main.ActiveProjectiles)
        {
            if (proj.type == projType && proj.ai[2] <= -5)
            {
                return true;
            }
        }
        return false;
    }
}
