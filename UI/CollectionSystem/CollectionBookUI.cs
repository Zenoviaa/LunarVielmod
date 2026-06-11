using Stellamod.Core.Utilities;
using Terraria;
using Terraria.GameContent.UI.Elements;

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

    public int RelativeLeft => Main.screenWidth / 2 - width / 2;
    public int RelativeTop => Main.screenHeight / 2 - height / 2 + 128;

    public override void OnInitialize()
    {
        base.OnInitialize();
        Width.Pixels = 700;
        Height.Pixels = 600;
        Left.Pixels = RelativeLeft;
        Top.Pixels = RelativeTop;
        BackgroundColor = Color.Transparent;
        BorderColor = Color.Transparent;

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

        Left.Pixels = RelativeLeft;
        Top.Pixels = RelativeTop;

        questTab.Left.Pixels = 252;
        questTab.Top.Pixels = 464;

        int o = 64;
        loreTab.Left.Pixels = questTab.Left.Pixels + o;
        loreTab.Top.Pixels = questTab.Top.Pixels;

        collectionTab.Left.Pixels = loreTab.Left.Pixels + o;
        collectionTab.Top.Pixels = loreTab.Top.Pixels;

        levelingTab.Left.Pixels = collectionTab.Left.Pixels + o;
        levelingTab.Top.Pixels = collectionTab.Top.Pixels;
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);
        this.QuickMouseInteraction();
    }
}
