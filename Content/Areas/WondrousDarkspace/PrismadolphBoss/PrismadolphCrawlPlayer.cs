using Microsoft.Xna.Framework.Graphics.PackedVector;
using Stellamod.Common;
using Stellamod.Common.Particles;
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

    public float fade;
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

    public override void PostUpdateMiscEffects()
    {
        base.PostUpdateMiscEffects();
        if (DownedBossTracker.IsDowned(DownedBossFlag.Prismadolph))
            return;

        var range = Vector2.DistanceSquared(Player.Center, PrismadolphRoamingSystem.RoamingPosition);
        if (range > 5096 * 5096)
            return;
     
        var sp = closestToPlayer - Main.screenPosition;
        var rect = new Rectangle(0, 0, Main.screenWidth, Main.screenHeight);
        rect = rect.CenterPad(1524);
        if (!rect.Contains(sp.ToPoint()))
        {
            Player.DisableMusic = true;

            if (isCrawling)
            {
                Player.ShakingNCrying = true;
                ShakeScreenPosition.Shake = 4;
                if (Main.GameUpdateCount % 4 == 0)
                {
                    var pos = Player.Center;
                    pos.Y -= 512;
                    pos.X += Main.rand.NextFloat(-1000, 1000);
                    var vel = new Vector2(Main.rand.NextFloat(-5F, 5F), 12);
                    PS.DarkspaceRock.Spawn(new()
                    {
                        position = pos,
                        velocity = vel,
                        timeLeft = 180
                    });
                }
            }
        }
   
    }

    public override void PostUpdate()
    {
        base.PostUpdate();
        if (Main.netMode == NetmodeID.Server)
            return;
        if (DownedBossTracker.IsDowned(DownedBossFlag.Prismadolph))
            return;

        if (isCrawling)
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
