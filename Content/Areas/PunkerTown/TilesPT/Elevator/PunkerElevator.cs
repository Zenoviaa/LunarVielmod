using Stellamod.Assets;
using Stellamod.Common.Platforms;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.PunkerTown.TilesPT.Elevator;



public class PunkerElevator : ModNPC
{
    private enum AIState
    {
        Idle,
        Up,
        Down,
        Toggle,
    }
    private ref float Timer => ref NPC.ai[0];
    private Vector2 MoveTarget
    {
        get
        {
            Vector2 target = new Vector2();
            target.X = NPC.ai[1];
            target.Y = NPC.ai[2];
            return target;
        }
        set
        {
            NPC.ai[1] = value.X;
            NPC.ai[2] = value.Y;
        }
    }

    private AIState State
    {
        get
        {
            return (AIState)NPC.ai[3];
        }
        set
        {
            NPC.ai[3] = (float)value;
        }
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        NPC.width = 64;
        NPC.height = 16;
        NPC.friendly = true;
        NPC.noTileCollide = true;
        NPC.noGravity = true;
        NPC.damage = 1;
        NPC.defense = 1;
        NPC.lifeMax = 100;
        NPC.knockBackResist = 1f;
        NPC.dontCountMe = true;
        NPC.dontTakeDamage = true;
        NPC.dontTakeDamageFromHostiles = true;
    }


    public override void AI()
    {
        base.AI();
        Timer++;

        switch (State)
        {
            case AIState.Idle:
                AI_Idle();
                break;
            case AIState.Up:
                AI_Up();
                break;
            case AIState.Down:
                AI_Down();
                break;
            case AIState.Toggle:
                AI_Toggle();
                break;
        }
        //NPC.velocity.Y = MathHelper.Lerp(0, 5, EasingFunction.InOutSine(Timer/120));


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
        Rectangle rectangle = new Rectangle(0, 0, 90, 16);
        Vector2 rectangleStart = NPC.Center - new Vector2(rectangle.Width / 2f, 0);
        rectangle.X = (int)NPC.position.X;
        rectangle.Y = (int)NPC.position.Y;
        rectangle.X -= 12;
        return rectangle;
    }

    private void SwitchState(AIState state)
    {
        if (MultiplayerHelper.IsHost)
        {
            Timer = 0;
            State = state;
            NPC.netUpdate = true;
        }
    }

    private void AI_Idle()
    {
        NPC.velocity.Y = 0;
    }

    private void AI_Up()
    {
        float ratio = Timer / 60f;
        float ease = EasingFunction.InExpo(ratio);
        float yVelocity = MathHelper.Lerp(0, -2, ease);
        NPC.velocity.Y = yVelocity;

        Vector2 spawnCenter = NPC.Top;
        Vector2 upCenter = spawnCenter - Vector2.UnitY * 112;
        bool stop = !Collision.CanHitLine(spawnCenter, 1, 1, upCenter, 1, 1);
        if (stop)
        {
            // Console.WriteLine("stop");
            SwitchState(AIState.Idle);
        }
    }

    private void AI_Down()
    {
        float ratio = Timer / 60f;
        float ease = EasingFunction.InExpo(ratio);
        float yVelocity = MathHelper.Lerp(0, 2, ease);
        NPC.velocity.Y = yVelocity;

        Vector2 spawnCenter = NPC.Center;
        Vector2 downCenter = spawnCenter + Vector2.UnitY * 8;
        bool stop = !Collision.CanHitLine(spawnCenter, 1, 1, downCenter, 1, 1);
        if (stop)
        {
            SwitchState(AIState.Idle);
        }
    }

    private void AI_Toggle()
    {
        SoundStyle mechTurn = AssetRegistry.Sounds.SteamPunking.MechTurn;
        SoundEngine.PlaySound(mechTurn, NPC.position);
        SoundStyle triggerSound = new SoundStyle("Stellamod/Assets/Sounds/VDisappear");
        triggerSound.PitchVariance = 0.3f;
        SoundEngine.PlaySound(triggerSound, NPC.position);

        Vector2 spawnCenter = NPC.Center;

        Vector2 upCenter = spawnCenter - Vector2.UnitY * 8;
        Vector2 downCenter = spawnCenter + Vector2.UnitY * 8;

        Point tileCoordinates = NPC.position.ToTileCoordinates();
        Point upTile = tileCoordinates + new Point(0, -3);
        Point downTile = tileCoordinates + new Point(0, 3);

        if (!Main.tile[downTile].HasTile && NPC.velocity.Y <= 0)
        {
            SwitchState(AIState.Down);
        }
        else if (!Main.tile[upTile].HasTile && NPC.velocity.Y >= 0)
        {
            SwitchState(AIState.Up);
        }
    }

    private void QuickDrawRectangle(SpriteBatch spriteBatch, Rectangle rect)
    {
        rect.X -= (int)Main.screenPosition.X;
        rect.Y -= (int)Main.screenPosition.Y;
        Primitives2D.DrawRectangle(spriteBatch, rect, Color.Red);
    }

    private void DrawChain(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        Vector2 startPosition = NPC.position;
        startPosition.X += 16;
        startPosition.Y -= 80;
        Point startTile = startPosition.ToTileCoordinates();
        Point endTile = startTile;
        endTile.Y -= 64;
        Point currentTile = startTile;
        Texture2D chain = ModContent.Request<Texture2D>(Texture + "_Chain").Value;
        while (currentTile.Y > endTile.Y)
        {
            if (!WorldGen.InWorld(currentTile.X, currentTile.Y))
                break;
            if (WorldGen.SolidTile(currentTile))
                break;
            Vector2 worldPosition = currentTile.ToWorldCoordinates();
            spriteBatch.Draw(chain, worldPosition - screenPos, null, drawColor, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            currentTile.Y--;
        }
    }
    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        DrawChain(spriteBatch, screenPos, drawColor);
        return true;
    }
}
