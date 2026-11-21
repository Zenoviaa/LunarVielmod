using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Stellamod.Content.Areas.WondrousDarkspace.PrismadolphBoss;

public class PrismadolphCrawlPlayer : ModPlayer
{
    public Vector2 closestToPlayer;
    public Vector2 crawlStartScreenPosition;
    public Vector2 crawlTargetScreenPosition;
    public Vector2 crawlVelocity;
    public float crawlTimer;
    public float crawlTime;
    public bool isCrawling;

    /// <summary>
    /// Starts a crawl from the start position to the target position
    /// </summary>
    /// <param name="startScreenPosition"></param>
    /// <param name="targetScreenPosition"></param>
    /// <param name="time"></param>
    public void StartCrawl(Vector2 startScreenPosition, Vector2 targetScreenPosition, float time)
    {
        crawlStartScreenPosition = startScreenPosition;
        crawlTargetScreenPosition = targetScreenPosition;
        crawlTimer = 0;
        crawlTime = time;
        isCrawling = true;
    }
    public override void PostUpdate()
    {
        base.PostUpdate();

  
     
        if (Main.netMode == NetmodeID.Server)
            return;
        if (isCrawling)
            return;
        if (DownedBossTracker.IsDowned(DownedBossFlag.Prismadolph))
            return;
        var range = Vector2.DistanceSquared(Player.Center, PrismadolphRoamingSystem.RoamingPosition);
        if (range > 5096 * 5096)
            return;
  
        if (Main.rand.NextBool(256))
        {
            Vector2 startPos;
            Vector2 endPos;
            if (Main.rand.NextBool(2))
            {
                startPos = new Vector2(Main.rand.NextFloat(0f, Main.screenWidth / 2), -200);
                endPos = new Vector2(Main.screenWidth - startPos.X, Main.screenHeight + 4300);
            }
            else
            {
                startPos = new Vector2(Main.rand.NextFloat(Main.screenWidth / 2, Main.screenWidth), Main.screenHeight + 300);
                endPos = new Vector2(Main.screenWidth - startPos.X, -4200);
            }
            StartCrawl(startPos, endPos, 1500);
        }
    }
}
