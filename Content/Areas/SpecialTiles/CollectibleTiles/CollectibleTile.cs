using Stellamod.Common.XixianFlaskSystem;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using Stellamod.Items.Insources;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.SpecialTiles.CollectibleTiles;

public class CollectibleDrawLayerSystem : ModSystem
{
    public override void Load()
    {
        base.Load();
        On_Main.DoDraw_WallsAndBlacks += DrawWalls;
    }

    public override void Unload()
    {
        base.Unload();
        On_Main.DoDraw_WallsAndBlacks -= DrawWalls;
    }

    private void DrawWalls(On_Main.orig_DoDraw_WallsAndBlacks orig, Main self)
    {
        //Draw Behind the walls
        orig(self);
        DrawCollectibles();
    }

    private void DrawCollectibles()
    {
        SpriteBatch spriteBatch = Main.spriteBatch;
        (Point topLeft, Point bottomRight) = TileUtilities.CameraTileBounds(128);
        for (int x = topLeft.X; x < bottomRight.X; x++)
        {
            for (int y = topLeft.Y; y < bottomRight.Y; y++)
            {
                Tile tile = Main.tile[x, y];
                if (!tile.HasTile)
                    continue;
                if (!TileSets.Collectible[tile.TileType])
                    continue;

                var modTile = ModContent.GetModTile(tile.TileType);
                (modTile as BaseCollectibleTile).Draw(x, y, spriteBatch);
            }
        }
    }
}
public abstract class AbstractCollectibleItem<T> : ModItem where T : ModTile
{
    public override string Texture => this.PathHere() + "/CollectibleItem";
    public override void SetDefaults()
    {
        Item.width = 12;
        Item.height = 12;
        Item.maxStack = Item.CommonMaxStack;
        Item.useTurn = true;
        Item.autoReuse = true;
        Item.useAnimation = 10;
        Item.useTime = 10;
        Item.useStyle = ItemUseStyleID.Swing;
        Item.consumable = true;
        Item.value = 1;
        Item.createTile = ModContent.TileType<T>();
    }
}

public abstract class BaseCollectibleTile : ModTile
{
    public virtual int CollectibleItem { get; set; }
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.tileSolid[Type] = false;
        Main.tileMerge[Type][Type] = true;
        Main.tileBlockLight[Type] = true;
        Main.tileMerge[TileID.Mud][Type] = true;
        Main.tileMerge[TileID.ClayBlock][Type] = true;
        Main.tileBlendAll[Type] = true;
        Main.tileLighted[Type] = true;
        Main.tileBlockLight[Type] = true;
        LocalizedText name = CreateMapEntryName();
        AddMapEntry(new Color(178, 163, 190), name);
        TileSets.Collectible[Type] = true;

        MineResist = 1f;
        MinPick = 210;
    }

    public override bool CanDrop(int i, int j)
    {
        return false;
    }

    public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
    {
        return false;
    }

    public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
    {
        base.ModifyLight(i, j, ref r, ref g, ref b);
        r += 1f;
        g += 1f;
        b += 1f;
    }

    public virtual void Draw(int i, int j, SpriteBatch spriteBatch)
    {
        Player player = Main.LocalPlayer;
        Vector2 drawPos = new Vector2(i, j) * 16;
        drawPos.Y += VectorHelper.Osc(-4f, 4f);

        Color drawColor = Color.White.MultiplyRGB(Lighting.GetColor(i, j));
        if (!CanCollect(player, drawPos))
        {
            drawColor = drawColor.MultiplyRGB(Color.Lerp(Color.White, Color.Black, 0.75f));
        }

        Vector2 tileCheckPos = new Vector2(i, j).ToWorldCoordinates();
        float drawRotation = VectorHelper.Osc(-0.2f, 0.2f, speed: 2);
        bool canCollect = CanCollect(player, tileCheckPos);
        if (canCollect)
        {
            if (Main.rand.NextBool(16) && Main.hasFocus)
            {
                var sp = SparkleParticle.Spawn(tileCheckPos - new Vector2(8) + Main.rand.NextVector2Circular(64, 64), -Vector2.UnitY * Main.rand.NextFloat(0.5f, 2.5f), Scale: Main.rand.NextFloat(0.4f, 0.7f));
                sp.outerColor = Color.Gray;
                sp.gravity = 0;
                sp.noTileCollide = true;
                sp.Scale *= 0.4f;

            }

            for (float f = 0f; f < 1f; f += 0.2f)
            {
                float rot = f * MathHelper.TwoPi;
                rot += Main.GlobalTimeWrappedHourly;
                Vector2 offset = rot.ToRotationVector2() * VectorHelper.Osc(12, 16);

                SpritebatchDrawer glowDrawer = SpritebatchDrawer.FromTextureAsset(TextureAssets.Tile[Type], drawPos + offset);
                glowDrawer.color = Color.Red * 0.3f;
                glowDrawer.color.A = 0;
                glowDrawer.VerticalFrame(1, 3);
                glowDrawer.CenterOrigin();
                glowDrawer.rotation = drawRotation;
                spriteBatch.Draw(glowDrawer);
            }

            //Draw Outkline
            SpritebatchDrawer outlineDrawer = SpritebatchDrawer.FromTextureAsset(TextureAssets.Tile[Type], drawPos);
            outlineDrawer.color = drawColor * ExtraMath.Osc(0f, 1f, speed: 3);
            outlineDrawer.VerticalFrame(2, 3);
            outlineDrawer.CenterOrigin();
            outlineDrawer.rotation = drawRotation;
            spriteBatch.Draw(outlineDrawer);

        }

        SpritebatchDrawer mainDrawer = SpritebatchDrawer.FromTextureAsset(TextureAssets.Tile[Type], drawPos);
        mainDrawer.color = drawColor;
        mainDrawer.VerticalFrame(0, 3);
        mainDrawer.CenterOrigin();
        mainDrawer.rotation = drawRotation;
        spriteBatch.Draw(mainDrawer);

        float distanceToPlayer = Vector2.Distance(player.Center, tileCheckPos);
        if (distanceToPlayer < 64 && canCollect)
        {
            Collect(player, tileCheckPos);
        }
    }

    public virtual bool CanCollect(Player player, Vector2 position)
    {
        return !player.HasItem(CollectibleItem);
    }

    public virtual void Collect(Player player, Vector2 position)
    {
        player.QuickSpawnItem(player.GetSource_FromThis(), CollectibleItem);
        SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/CollectSpecial"), player.position);
        for (float i = 0; i < 12; i++)
        {
            float rot = MathHelper.TwoPi * Main.rand.NextFloat(0f, 1f);
            Vector2 velocity = rot.ToRotationVector2() * Main.rand.NextFloat(5f, 25f);
            var particle = FXUtil.GlowStretch(position, velocity);
            particle.InnerColor = Color.White;
            particle.GlowColor = Color.LightCyan;
            particle.OuterGlowColor = Color.Black;
            particle.Duration = Main.rand.NextFloat(25, 50);
            particle.BaseSize = Main.rand.NextFloat(0.04f, 0.07f);
            particle.VectorScale *= 0.5f;
        }
    }
}

public class XixianFlaskCollectibleItem : AbstractCollectibleItem<XixianFlaskCollectible>
{

}

public class XixianFlaskCollectible : BaseCollectibleTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        CollectibleItem = ModContent.ItemType<XixianFlask>();
    }

    public override bool CanCollect(Player player, Vector2 position)
    {
        return !player.GetModPlayer<FlaskPlayer>().unlockedFlask;
    }

    public override void Collect(Player player, Vector2 position)
    {
        base.Collect(player, position);
        SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Harv1"));
        player.QuickSpawnItem(player.GetSource_FromThis(), ModContent.ItemType<HealthInsource>());
    }
}
