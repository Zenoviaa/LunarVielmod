using Stellamod.Common.UI;
using Stellamod.Core.Utilities;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.UI;

namespace Stellamod.UI.CollectionSystem;

public class CollectionBookUI : UIPanel
{
    public Book book;

    public const int width = 432;
    public const int height = 800;

    public QuestTab questTab;
    public LoreTab loreTab;
    public CollectionTab collectionTab;
    public LevelingTab levelingTab;

    public int RelativeLeft => Main.screenWidth / 2 - (int)(Width.Pixels / 2);
    public int RelativeTop => Main.screenHeight / 2 - height / 2 + 128;

    public override void OnInitialize()
    {
        base.OnInitialize();
        Width.Pixels = 900;
        Height.Pixels = 600;
        Left.Pixels = RelativeLeft;
        Top.Pixels = RelativeTop;
        BackgroundColor = Color.Transparent;
        BorderColor = Color.Transparent;

        IgnoresMouseInteraction = false;


        book = new Book();
        Append(book);

        questTab = new QuestTab();
        questTab.Left.Pixels = 0;
        questTab.Top.Pixels = 0;
        Append(questTab);

        loreTab = new LoreTab();
        int o = 64;
        loreTab.Left.Pixels = questTab.Left.Pixels + o;
        loreTab.Top.Pixels = questTab.Top.Pixels;
        Append(loreTab);

        collectionTab = new CollectionTab();
        collectionTab.Left.Pixels = loreTab.Left.Pixels + o;
        collectionTab.Top.Pixels = loreTab.Top.Pixels;
        Append(collectionTab);

        levelingTab = new LevelingTab();
        levelingTab.Left.Pixels = collectionTab.Left.Pixels + o;
        levelingTab.Top.Pixels = collectionTab.Top.Pixels;
        Append(levelingTab);


    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        book.Left.Pixels = 232;
        Left.Pixels = RelativeLeft;
        Top.Pixels = RelativeTop;

        questTab.Left.Pixels = 512;
        questTab.Top.Pixels = 464;

        int o = 64;
        loreTab.Left.Pixels = questTab.Left.Pixels + o;
        loreTab.Top.Pixels = questTab.Top.Pixels;

        collectionTab.Left.Pixels = loreTab.Left.Pixels + o;
        collectionTab.Top.Pixels = loreTab.Top.Pixels;

        levelingTab.Left.Pixels = collectionTab.Left.Pixels + o;
        levelingTab.Top.Pixels = loreTab.Top.Pixels;

    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);
        Rectangle r = new Rectangle(0, 0, Main.screenWidth, Main.screenHeight);
        SpritebatchDrawer d = SpritebatchDrawer.FromTextureAsset(TextureAssets.BlackTile.Value, Vector2.Zero);
        d.dstRect = r;
        d.drawOrigin = Vector2.Zero;
        d.color = Color.Black * book.alpha * 0.5f;
        spriteBatch.Draw(d);

    }
}
