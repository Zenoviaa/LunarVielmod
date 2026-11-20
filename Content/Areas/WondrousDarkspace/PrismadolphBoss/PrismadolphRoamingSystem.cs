using Stellamod.WorldG;
using System;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.WondrousDarkspace.PrismadolphBoss;

public class PrismadolphRoamingSystem : ModSystem
{
    float _roamingTimer;
    public static Vector2 StartPoint
    {
        get
        {
            Vector2 pos = new();
            pos.X = ((float)Main.maxTilesX * 16) * 0.5F;
            pos.Y = (SavedGenerationParameters.DarkspaceTop * 16) + 400;
            return pos;
        }
    }
    public static bool CanDoJumpscare { get; set; } = true;
    public static float SpawnRadiusSquared => 2048F * 2048F;
    public static int PrismadolphType { get; private set; }
    public static Vector2 RoamingDirection { get; set; }
    public static Vector2 RoamingPosition { get; set; }
    public override void OnModLoad()
    {
        base.OnModLoad();
        PrismadolphType = ModContent.NPCType<Prismadolph>();
    }

    public override void ClearWorld()
    {
        base.ClearWorld();
        RoamingPosition = StartPoint;
        RoamingDirection = Main.rand.NextBool() ? -Vector2.UnitX : Vector2.UnitX;
    }
    public override void NetSend(BinaryWriter writer)
    {
        base.NetSend(writer);
        writer.WriteVector2(RoamingPosition);
        writer.WriteVector2(RoamingDirection);
    }
    public override void NetReceive(BinaryReader reader)
    {
        base.NetReceive(reader);
        RoamingPosition = reader.ReadVector2();
        RoamingDirection = reader.ReadVector2();
    }
    public override void PostUpdateEverything()
    {
        base.PostUpdateEverything();
        _roamingTimer++;
        if (ShouldChangeRoamingDirection())
        {
            _roamingTimer = 0;
            RoamingDirection = Main.rand.NextBool() ? -Vector2.UnitX : Vector2.UnitX;
        }
        if (ShouldResetRoamingPosition())
        {
            RoamingPosition = StartPoint;
        }

        var roamingVelocity = RoamingDirection * 1;

        //  roamingVelocity.Y += 
        RoamingPosition += roamingVelocity;


        //RoamingPosition += new Vector2(0, y);
        if (!MultiplayerHelper.IsHost)
            return;
        if (DownedBossTracker.IsDowned(DownedBossFlag.Prismadolph))
            return;
        if(Main.GameUpdateCount % 15 == 0)
        {
            if (!NPC.AnyNPCs(PrismadolphType))
            {
                foreach (var player in Main.ActivePlayers)
                {
                    var dsq = Vector2.DistanceSquared(RoamingPosition, player.Center);
                    var spos = player.Center;
                    spos.X -= RoamingDirection.X * 1444;
                    if (dsq < SpawnRadiusSquared)
                    {
                        NPC.NewNPC(new EntitySource_Misc("ctx"), (int)spos.X, (int)spos.Y, PrismadolphType);
                    }

                }
            }
        }

    }
    bool ShouldChangeRoamingDirection()
    {
        if (RoamingDirection == default)
            return true;
        if (_roamingTimer >= 2400)
            return true;
        return false;
    }
    bool ShouldResetRoamingPosition()
    {
        if (RoamingPosition == default)
            return true;
        if (RoamingPosition.Y < StartPoint.Y - 1000)
            return true;
        var xDist = (RoamingPosition.X - StartPoint.X);
        xDist = MathF.Abs(xDist);
        if (xDist >= 14000)
            return true;
        return false;
    }
}
