using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Common.Platforms;

public class ElevatorSystem : ModSystem
{
    private static List<ElevatorBounds> _elevators;
    public override void OnModLoad()
    {
        base.OnModLoad();
        _elevators = new List<ElevatorBounds>();
        On_Player.SlopingCollision += ElevatorLogic;
        On_Player.DryCollision += ElevatorDryLogic;
    }

    private void ElevatorDryLogic(On_Player.orig_DryCollision orig, Player self, bool fallThrough, bool ignorePlats)
    {
        /*
        foreach (ElevatorBounds elevator in _elevators)
        {
            Rectangle nextPlatformRectangle = elevator.rectangle;
            nextPlatformRectangle.Y += (int)elevator.velocity.Y;
            Rectangle playerRectangle = self.getRect();

            if (nextPlatformRectangle.Intersects(playerRectangle) || nextPlatformRectangle.Contains(playerRectangle))
            {
                if (!self.justJumped && self.velocity.Y >= 0)
                {
                    self.velocity.Y = 0;


                    int inside = 4;
                    //We had it a little bit into the elevator so it doesn't stop colliding with it
                    self.position.Y = (nextPlatformRectangle.TopLeft().Y) - self.height + inside;
                }
            }
        }*/
        orig(self, fallThrough, ignorePlats);
    }

    public override void OnModUnload()
    {
        base.OnModUnload();
        On_Player.SlopingCollision -= ElevatorLogic;
        On_Player.DryCollision -= ElevatorDryLogic;
    }

    public static void AddElevator(ElevatorBounds elevator)
    {
        _elevators.Add(elevator);
    }
    public override void PreUpdateNPCs()
    {
        base.PreUpdateNPCs();
        _elevators.Clear();
    }


    private void ElevatorLogic(On_Player.orig_SlopingCollision orig, Player self, bool fallThrough, bool ignorePlats)
    {
        foreach (ElevatorBounds elevator in _elevators)
        {
            Rectangle nextPlatformRectangle = elevator.rectangle;
            nextPlatformRectangle.Y += (int)elevator.velocity.Y;
            Rectangle playerRectangle = self.getRect();
          
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
            }
        }


        orig(self, fallThrough, ignorePlats);
    }
}
