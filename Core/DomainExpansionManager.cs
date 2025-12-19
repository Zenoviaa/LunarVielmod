using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Core
{
    public class DomainExpansionManager : ModSystem
    {
        public bool inSpace;
        public bool noWings;
        public bool hoveringPlatform;
        public float hoverPlatformY;
        public override void OnModLoad()
        {
            base.OnModLoad();
            On_Collision.TileCollision += NoCollision;
            On_Collision.AdvancedTileCollision += NoAdvancedCollision;
            On_Collision.WetCollision += NoWetCollision;
            On_Collision.AnyCollision += NoAnyCollision;
            On_Collision.SlopeCollision += NoSlopeCollision;
            On_Player.SlopingCollision += NoSlopingCollision;
            On_Player.DryCollision += NoDryCollision;
            On_Collision.SolidCollision_Vector2_int_int += NoSolidCollision;
            On_Collision.EmptyTile += AllEmptyTiles;
            On_Collision.SolidCollision_Vector2_int_int_bool += NoSolidCollision2;
            On_Collision.IsWorldPointSolid += NoSolid;
            On_Collision.StepDown += NoStepDown;
            On_Collision.StepUp += NoStepUp;
            On_Player.SlopeDownMovement += NoSlopeDown;
            On_Collision.CanHitLine += AlwaysHitLine;
        }
        public override void OnModUnload()
        {
            base.OnModUnload();
            On_Collision.TileCollision -= NoCollision;
            On_Collision.AdvancedTileCollision -= NoAdvancedCollision;
            On_Collision.WetCollision -= NoWetCollision;
            On_Collision.AnyCollision -= NoAnyCollision;
            On_Collision.SlopeCollision -= NoSlopeCollision;
            On_Player.SlopingCollision -= NoSlopingCollision;
            On_Player.DryCollision -= NoDryCollision;
 
            On_Player.SlopeDownMovement -= NoSlopeDown;
            On_Collision.CanHitLine -= AlwaysHitLine;
        }

        private bool AlwaysHitLine(On_Collision.orig_CanHitLine orig, Vector2 Position1, int Width1, int Height1, Vector2 Position2, int Width2, int Height2)
        {
            if (inSpace)
                return true;
            return orig(Position1, Width1, Height1, Position2, Width2, Height2);
        }

        private void NoSlopeDown(On_Player.orig_SlopeDownMovement orig, Player self)
        {
            if (inSpace)
                return;
            orig(self);
        }

        private void NoStepUp(On_Collision.orig_StepUp orig, ref Vector2 position, ref Vector2 velocity, int width, int height, ref float stepSpeed, ref float gfxOffY, int gravDir, bool holdsMatching, int specialChecksMode)
        {
            if (inSpace)
                return;
            orig(ref position, ref velocity, width, height, ref stepSpeed, ref gfxOffY, gravDir, holdsMatching, specialChecksMode);
        }

        private void NoStepDown(On_Collision.orig_StepDown orig, ref Vector2 position, ref Vector2 velocity, int width, int height, ref float stepSpeed, ref float gfxOffY, int gravDir, bool waterWalk)
        {
            if (inSpace)
                return;
            orig(ref position, ref velocity, width, height, ref stepSpeed, ref gfxOffY, gravDir, waterWalk);
        }


        private bool NoSolid(On_Collision.orig_IsWorldPointSolid orig, Vector2 pos, bool treatPlatformsAsNonSolid)
        {

            if (!inSpace)
            {
                return orig(pos, treatPlatformsAsNonSolid);
            }
            return false;
        }

        private bool NoSolidCollision2(On_Collision.orig_SolidCollision_Vector2_int_int_bool orig, Vector2 Position, int Width, int Height, bool acceptTopSurfaces)
        {
           
            if (!inSpace)
            {
                return orig(Position, Width, Height, acceptTopSurfaces);
            }
            return false;
        }


        private bool NoSolidCollision(On_Collision.orig_SolidCollision_Vector2_int_int orig, Vector2 Position, int Width, int Height)
        {
          
            if (!inSpace)
            {
                return orig(Position, Width, Height);
            }
            return false;
        }
        private bool AllEmptyTiles(On_Collision.orig_EmptyTile orig, int i, int j, bool ignoreTiles)
        {
        
            if (!inSpace)
            {
                return orig(i, j, ignoreTiles);
            }
            return true;
        }

        private void NoDryCollision(On_Player.orig_DryCollision orig, Player self, bool fallThrough, bool ignorePlats)
        {
            if (!inSpace)
            {
                orig(self, fallThrough, ignorePlats);
                return;
            }
            orig(self, true, true);
        }

        private void NoSlopingCollision(On_Player.orig_SlopingCollision orig, Player self, bool fallThrough, bool ignorePlats)
        {
            if (!inSpace)
            {
                orig(self, fallThrough, ignorePlats);
                return;
            }
            orig(self, true, true);
        }


        public override void PreUpdateNPCs()
        {
            base.PreUpdateNPCs();
            inSpace = false;
            noWings = false;
            hoveringPlatform = false;
        }
        private Vector4 NoSlopeCollision(On_Collision.orig_SlopeCollision orig, Vector2 Position, Vector2 Velocity, int Width, int Height, float gravity, bool fall)
        {
            if (!inSpace)
                return orig(Position, Velocity, Width, Height, gravity, fall);
            return new Vector4(Position.X, Position.Y, Velocity.X, Velocity.Y);
        }

        private Vector2 NoAnyCollision(On_Collision.orig_AnyCollision orig, Vector2 Position, Vector2 Velocity, int Width, int Height, bool evenActuated)
        {
            if (!inSpace)
                return orig(Position, Velocity, Width, Height, evenActuated);
            return Velocity;
        }

        private Vector2 NoCollision(On_Collision.orig_TileCollision orig, Vector2 Position, Vector2 Velocity, int Width, int Height, bool fallThrough, bool fall2, int gravDir)
        {
            if (!inSpace)
                return orig(Position, Velocity, Width, Height, fallThrough, fall2, gravDir);
            if (hoveringPlatform)
            {
                Vector2 vel = Velocity;
                if (Position.Y > hoverPlatformY)
                    vel.Y = -16;
                else if (Position.Y + Velocity.Y > hoverPlatformY)
                    vel.Y = 0;
                return vel;
            }
            return Velocity;
        }

        private Vector2 NoAdvancedCollision(On_Collision.orig_AdvancedTileCollision orig, bool[] forcedIgnoredTiles, Vector2 Position, Vector2 Velocity, int Width, int Height, bool fallThrough, bool fall2, int gravDir)
        {
            if (!inSpace)
                return orig(forcedIgnoredTiles, Position, Velocity, Width, Height, fallThrough, fall2, gravDir);
            return Velocity;
        }

        private bool NoWetCollision(On_Collision.orig_WetCollision orig, Vector2 Position, int Width, int Height)
        {
            if (!inSpace)
                return orig(Position, Width, Height);
            return false;
        }
    }
}
