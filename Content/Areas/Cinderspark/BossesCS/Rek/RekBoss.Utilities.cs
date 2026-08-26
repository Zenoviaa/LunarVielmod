using Stellamod.Assets;
using Stellamod.Common.Particles;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Cinderspark.BossesCS.Rek;

public partial class RekBoss
{
    private Rectangle _arenaRectangleToLava;
    private Vector2 _eruptionRight;
    private Vector2 _eruptionLeft;
    private Vector2 EruptionLeft
    {
        get
        {
            if (_eruptionLeft == Vector2.Zero)
                _eruptionLeft = FindEruptionLeft();
            return _eruptionLeft;
        }
    }

    private Vector2 EruptionRight
    {
        get
        {
            if (_eruptionRight == Vector2.Zero)
                _eruptionRight = FindEruptionRight();
            return _eruptionRight;
        }
    }

    private Rectangle ArenaRectangleToLava
    {
        get
        {
            if (_arenaRectangleToLava == Rectangle.Empty)
                _arenaRectangleToLava = ArenaRectangleUpToLava();
            return _arenaRectangleToLava;
        }
    }

    public void CreateArena()
    {
        int width = 145;
        NPC.NewNPC(NPC.GetSource_FromThis(),
            (int)_arenaCenter.X, (int)_arenaCenter.Y + 840, ModContent.NPCType<BigMoltenPlatform>());

        Vector2 left = _arenaCenter + new Vector2(-width * 16, 0) * new Vector2(0.5f, 0f);
        Vector2 right = _arenaCenter + new Vector2(width * 16, 0) * new Vector2(0.5f, 0f);
        void MakeSmallerPlatform(float p)
        {
            Vector2 pos = Vector2.Lerp(left, right, p);
            NPC.NewNPC(Main.LocalPlayer.GetSource_FromThis(),
                (int)pos.X,
                (int)pos.Y + 840,
                ModContent.NPCType<SmallMoltenPlatform>());
        }

        MakeSmallerPlatform(0.1f);
        MakeSmallerPlatform(0.2f);
        MakeSmallerPlatform(0.8f);
        MakeSmallerPlatform(0.9f);
    }

    public void CreateSegmentEatEffect(RekSegment segment)
    {
        for (float f = 0; f < 12; f++)
        {
            Vector2 pos = segment.position;
            pos += Main.rand.NextVector2Circular(16, 16);
            Color color = Color.Lerp(Color.Yellow, Color.Red, Main.rand.NextFloat(0f, 1f));
            Particles.SwirlingFlameDust.Spawn(BitDustFactory.Default with
            {
                position = pos,
                velocity = Main.rand.NextVector2Circular(18, 18),
                timeLeft = 100,
                innerColor = color.ToVector4(),
                outerColor = Color.Red.ToVector4(),
                scale = new Vector2(Main.rand.NextFloat(1f, 2f))
            });
        }

        SoundEngine.PlaySound(SoundID.Item74 with { PitchVariance = 0.6f }, segment.position);
    }
    public Rectangle ArenaRectangleUpToLava()
    {

        Point center = _arenaCenter.ToTileCoordinates();
        int left = center.X;
        int top = center.Y;
        int bottom = center.Y;
        int right = center.X;


        //Find left
        while (left > 0)
        {
            left--;
            Tile tile = Main.tile[left, center.Y];
            if (WorldGen.SolidTile(tile))
                break;
        }

        //Find right
        while (right < Main.maxTilesX - 1)
        {
            right++;
            Tile tile = Main.tile[right, center.Y];
            if (WorldGen.SolidTile(tile))
                break;
        }


        //Find top
        while (top > 0)
        {
            top--;
            Tile tile = Main.tile[center.X, top];
            if (WorldGen.SolidTile(tile))
                break;
        }

        //Find bottom
        while (bottom < Main.maxTilesY - 1)
        {
            bottom++;
            Tile tile = Main.tile[center.X, bottom];
            if (tile.LiquidAmount > 0)
                break;
        }

        Point topLeft = new Point(left, top);
        Point bottomRight = new Point(right, bottom);

        Vector2 topLeftWorld = topLeft.ToWorldCoordinates();
        Vector2 bottomRightWorld = bottomRight.ToWorldCoordinates();

        Point topLeftPoint = topLeftWorld.ToPoint();
        Point bottomRightPoint = bottomRightWorld.ToPoint();
        return  new Rectangle(
            topLeftPoint.X,
            topLeftPoint.Y,
            bottomRightPoint.X - topLeftPoint.X,
            bottomRightPoint.Y - topLeftPoint.Y);
    }
    private void CreateFlameSuckParticles(Vector2 position)
    {
        Vector2 spawnPos = position + Main.rand.NextVector2CircularEdge(444, 444);
        Vector2 spawnVelocity = position - spawnPos;
        spawnVelocity = spawnVelocity.SafeNormalize(Vector2.Zero);
        spawnVelocity *= 16;
        /*
        var d = DustParticle.Spawn(spawnPos, spawnVelocity, DustParticleSpawnParams.Default);
        d.Scale *= 0.8f;
        d.gravity = 0;*/
        Particles.BitDust.Spawn(BitDustFactory.Default with { position = spawnPos, velocity = spawnVelocity, timeLeft = 24 });
        if (Main.rand.NextBool(2))
        {
            spawnPos = position + Main.rand.NextVector2CircularEdge(384, 384);
            spawnVelocity = position - spawnPos;
            spawnVelocity = spawnVelocity.SafeNormalize(Vector2.Zero);
            spawnVelocity *= 16;
            var p = FXUtil.GlowStretch(spawnPos, spawnVelocity);
            p.InnerColor = Color.White;
            p.OuterGlowColor = Color.Red;
        }
    }
    /// <summary>
    /// Find the left side of the arena
    /// </summary>
    /// <returns></returns>
    private Vector2 FindEruptionLeft()
    {
        Point centerTile = _arenaCenter.ToTileCoordinates();
        for (int i = 0; i < 200; i++)
        {
            centerTile.Y++;

            if (WorldGen.SolidTile(centerTile))
            {
                centerTile.Y -= 1;
                break;
            }

        }
        for (int i = 0; i < 200; i++)
        {
            centerTile.X--;

            if (WorldGen.SolidTile(centerTile))
            {
                centerTile.X += 1;
                break;
            }

        }
        return centerTile.ToWorldCoordinates();
    }

    /// <summary>
    /// Find the right side of the arena
    /// </summary>
    /// <returns></returns>
    private Vector2 FindEruptionRight()
    {
        Point centerTile = _arenaCenter.ToTileCoordinates();
        for (int i = 0; i < 200; i++)
        {
            centerTile.Y++;

            if (WorldGen.SolidTile(centerTile))
            {
                centerTile.Y -= 1;
                break;
            }

        }
        for (int i = 0; i < 200; i++)
        {
            centerTile.X++;

            if (WorldGen.SolidTile(centerTile))
            {
                centerTile.X--;
                break;
            }

        }
        return centerTile.ToWorldCoordinates();
    }
}
