using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using System;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Core
{
    public class DomainExpansionPlayer : ModPlayer
    {

        public override void PreUpdateMovement()
        {
            base.PreUpdateMovement();
            DomainExpansionManager domainExpansionManager = ModContent.GetInstance<DomainExpansionManager>();
            if (domainExpansionManager.inSpace)
            {

                Player.gills = true;
                Player.breath = Player.breathMax;
                Player.ignoreWater = true;
                Player.waterWalk = false;
                Player.waterWalk2 = false;
            }
        }


    }

    public class DomainExpansionGlobalProjectile : GlobalProjectile
    {
        public override void SetDefaults(Projectile entity)
        {
            base.SetDefaults(entity);
            DomainExpansionManager domainExpansionManager = ModContent.GetInstance<DomainExpansionManager>();
            if (domainExpansionManager.noProjTileCollide)
            {
                entity.tileCollide = false;
            }
        }
    }

    public struct DomainParameters
    {
        public bool inSpace;
        public bool noWings;
        public bool hoveringPlatform;
        public float hoverPlatformY;
        public bool noProjTileCollide;
        
        /// <summary>
        /// Disables tile rendering, wall rendering, and water rendering as they are not needed while in a domain
        /// </summary>
        public bool noRender;
    }
    public class DomainExpansionManager : ModSystem
    {
        private bool[] _prevTileSolid;

        public bool inSpace;
        public bool noWings;
        public bool hoveringPlatform;
        public float hoverPlatformY;
        public bool noProjTileCollide;
        public bool noRender;
        public override void OnModLoad()
        {
            base.OnModLoad();
            //Disable rendering
            On_Main.DoDraw_Tiles_NonSolid += DisableNonSolidTileDraw;
            On_Main.DoDraw_WallsAndBlacks += DisableWallsDraw;
            On_Main.DoDraw_Waterfalls += DisableWaterfallsDraw;
            On_Main.DoDraw_Tiles_Solid += DisableSolidTileDraw;
            On_Main.DrawLiquid += DisableLiquidDraw;

            //Platform collision
            On_Player.DryCollision += FallThroughPlatform;
            On_Player.SlopingCollision += HoverPlatformCollisionCheck;
            On_Collision.TileCollision += HoverPlatformTileCollision;
            On_Collision.WetCollision += DisableWetCollisions;
        }

        private void DisableLiquidDraw(On_Main.orig_DrawLiquid orig, Main self, bool bg, int waterStyle, float Alpha, bool drawSinglePassLiquids)
        {
            if (noRender)
                return;

            orig(self, bg, waterStyle, Alpha, drawSinglePassLiquids);
        }

        private void DisableSolidTileDraw(On_Main.orig_DoDraw_Tiles_Solid orig, Main self)
        {
            if (noRender)
                return;

            orig(self);
        }

        private void DisableWaterfallsDraw(On_Main.orig_DoDraw_Waterfalls orig, Main self)
        {
            if (noRender)
                return;

            orig(self);
        }

        private void DisableNonSolidTileDraw(On_Main.orig_DoDraw_Tiles_NonSolid orig, Main self)
        {
            if (noRender)
                return;

            orig(self);
          //  throw new NotImplementedException();
        }

        private void DisableWallsDraw(On_Main.orig_DoDraw_WallsAndBlacks orig, Main self)
        {
            if (noRender)
                return;

            orig(self);
        }

        public static void UseDomain(in DomainParameters parameters)
        {
            DomainExpansionManager domain = ModContent.GetInstance<DomainExpansionManager>();
            domain.inSpace = parameters.inSpace;
            domain.noWings = parameters.noWings;
            domain.hoveringPlatform = parameters.hoveringPlatform;
            domain.hoverPlatformY = parameters.hoverPlatformY;
            domain.noProjTileCollide = parameters.noProjTileCollide;
            domain.noRender = parameters.noRender;
        }

        private void FallThroughPlatform(On_Player.orig_DryCollision orig, Player self, bool fallThrough, bool ignorePlats)
        {
            if (self.GetModPlayer<MovePlayer>().grabbed)
            {
                ignorePlats = true;
                fallThrough = true;
            }
            orig(self, fallThrough, ignorePlats);
        }

        private Vector2 HoverPlatformTileCollision(On_Collision.orig_TileCollision orig, 
            Vector2 Position, Vector2 Velocity, int Width, int Height, bool fallThrough, bool fall2, int gravDir)
        {
            if (hoveringPlatform)
            {
                if ((Position.Y-18) > hoverPlatformY)
                    return Vector2.Zero;
                return orig(Position, Velocity, Width, Height, fallThrough, fall2, gravDir);
            }
            else
            {
                return orig(Position, Velocity, Width, Height, fallThrough, fall2, gravDir);
            }
        }

        private bool DisableWetCollisions(On_Collision.orig_WetCollision orig, Vector2 Position, int Width, int Height)
        {
            if (inSpace)
                return false;

           return orig(Position, Width, Height);
        }

        public override void OnModUnload()
        {
            base.OnModUnload();
            On_Collision.TileCollision -= HoverPlatformTileCollision;
            On_Collision.WetCollision -= DisableWetCollisions;
            On_Player.SlopingCollision -= HoverPlatformCollisionCheck;

        }

        private void WaterCollisionCheck(On_Player.orig_WaterCollision orig, Player self, bool fallThrough, bool ignorePlats)
        {
        
            orig(self, fallThrough, ignorePlats);
        }
 
        private void HoverPlatformCollisionCheck(On_Player.orig_SlopingCollision orig, Player self, bool fallThrough, bool ignorePlats)
        {

            if (self.GetModPlayer<MovePlayer>().grabbed)
            {
                ignorePlats = true;
                fallThrough = true;
            }

            if (hoveringPlatform)
            {
                float y = hoverPlatformY + 36;
                Rectangle playerRectangle = self.getRect();
                int width = 128;
                Rectangle nextPlatformRectangle = new Rectangle(playerRectangle.X - width / 2, (int)y, width, 1);
                if (nextPlatformRectangle.Intersects(playerRectangle) || nextPlatformRectangle.Contains(playerRectangle))
                {
                    if (!self.justJumped && self.velocity.Y >= 0)
                    {
                        self.velocity.Y = 0;

                        int inside = 4;
                        //We had it a little bit into the elevator so it doesn't stop colliding with it
                        self.position.Y = (nextPlatformRectangle.TopLeft().Y) - self.height + inside;
                        self.position.Y -= self.gfxOffY;
                    }
                } else if (self.Bottom.Y > y)
                {
                    float dist = MathF.Abs(y - self.Bottom.Y);
                    float strength = dist / 1000;
                    strength = MathHelper.Clamp(strength, 0, 1);
                    strength = EasingFunction.InOutSine(strength);
                    self.position.Y -= MathHelper.Lerp(12, 48, strength);
                    self.velocity.Y = 0;
                }
            }

            orig(self, fallThrough, ignorePlats);
        }

        public override void PreUpdatePlayers()
        {
            base.PreUpdatePlayers();
            if (!inSpace)
                return;

            _prevTileSolid ??= new bool[Main.tileSolid.Length];
            for (int t = 0; t < _prevTileSolid.Length; t++)
            {
                _prevTileSolid[t] = Main.tileSolid[t];
                Main.tileSolid[t] = false;
            }


        }

        public override void PostUpdatePlayers()
        {
            base.PostUpdatePlayers();
            if (!inSpace)
                return;


            for (int t = 0; t < Main.tileSolid.Length; t++)
            {
                Main.tileSolid[t] = _prevTileSolid[t];
            }
        }

        public override void PreUpdateNPCs()
        {
            base.PreUpdateNPCs();
            inSpace = false;
            noWings = false;
            hoveringPlatform = false;
            noProjTileCollide = false;
            noRender = false;
        }
    }
}
