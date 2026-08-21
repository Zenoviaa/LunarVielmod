using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Common.Platforms;
//So to get a proper moving platform
//We need to move the player's position directly without affecting their velocity
//Then have the bottom thing be treated has a solid object that can't be moved past
//So it should act as moving ground since the y velocity is never changing

public struct ElevatorBounds
{
    public Rectangle rectangle;
    public Vector2 velocity;
}
public abstract class AbstractPlatformNPC : ModNPC
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Point platformSize = GetPlatformSize();
        NPC.width = platformSize.X;
        NPC.height = platformSize.Y;
        NPC.friendly = true;
        NPC.noTileCollide = true;

        NPC.damage = 1;
        NPC.defense = 1;
        NPC.lifeMax = 100;
        NPC.knockBackResist = 1f;
        NPC.dontCountMe = true;
        NPC.dontTakeDamage = true;
        NPC.dontTakeDamageFromHostiles = true;
        NPC.hide = true;
    }

    public override void DrawBehind(int index)
    {
        Main.instance.DrawCacheNPCsBehindNonSolidTiles.Add(index);
    }
    public abstract Point GetPlatformSize();
    public virtual Point GetPlatformOffset() => Point.Zero;
    public virtual bool RiseInLiquids() => true;
    public override void AI()
    {
        base.AI();
        if (RiseInLiquids())
        {
            Point tilePoint = NPC.Center.ToTileCoordinates();
            Tile tile = Main.tile[tilePoint];
            Tile tileBelow = Main.tile[tilePoint + new Point(0, 1)];

            if(tileBelow.LiquidAmount <= 0)
            {
                for (int i = 0; i < 100; i++)
                {
                    tile = Main.tile[tilePoint];
                    if (tile.LiquidAmount > 0)
                        break;
                    tilePoint.Y++;
                }
            }
            else
            {
                for (int i = 0; i < 100; i++)
                {
                    tile = Main.tile[tilePoint];
                    if (tile.LiquidAmount <= 0)
                        break;
                    tilePoint.Y--;
                }
            }
   

            Vector2 pointToMoveTo = tilePoint.ToWorldCoordinates();
            Vector2 normalVel = pointToMoveTo - NPC.Center;
            normalVel = normalVel.SafeNormalize(Vector2.Zero);
            float dist = Vector2.Distance(NPC.Center, pointToMoveTo);
            if (dist < 16)
            {
                NPC.velocity *= 0.82f;
            }
            else
            {
            
                Vector2 v = NPC.velocity.MoveTowards(normalVel * 8, 8);
                NPC.velocity = Vector2.Lerp(NPC.velocity, v, 0.1f);
            }

            NPC.noGravity = true;

        }

        Vector2 movement = NPC.position - NPC.oldPosition;
        Rectangle platformRectangle = GetPlatformRectangle();
        Rectangle nextPlatformRectangle = platformRectangle;
        nextPlatformRectangle.Y += (int)NPC.velocity.Y;
        foreach (var player in Main.ActivePlayers)
        {
            Rectangle playerRectangle = player.getRect();
            if (nextPlatformRectangle.Intersects(playerRectangle) || nextPlatformRectangle.Contains(playerRectangle))
            {
                player.position.Y += NPC.velocity.Y;

            }
        }

        ElevatorBounds elevatorBounds = new ElevatorBounds
        {
            rectangle = nextPlatformRectangle,
            velocity = NPC.velocity
        };
        ElevatorSystem.AddElevator(elevatorBounds);
    }

    private Rectangle GetPlatformRectangle()
    {
        var rect = NPC.getRect();
        var point = rect.Location;
        point += GetPlatformOffset();
        rect.Location = point;
        return rect;
    }
}
