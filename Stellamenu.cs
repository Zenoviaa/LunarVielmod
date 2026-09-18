using ReLogic.Content;
using Stellamod.Content.Areas;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;


namespace Stellamod;

public class Stellamenu : ModMenu
{
    private const string menuAssetPath = "Stellamod/Assets/Textures/Menu";
    public override Asset<Texture2D> Logo => ModContent.Request<Texture2D>($"{menuAssetPath}/Logo");
    public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Music/BeforeTheFlames");
    public override ModSurfaceBackgroundStyle MenuBackgroundStyle => ModContent.GetInstance<NoBackgroundStyle>();
    public override string DisplayName => "Lunar Veil";
    public override void OnSelected()
    {
        SoundEngine.PlaySound(SoundID.Tink);
    }

    public override bool PreDrawLogo(SpriteBatch spriteBatch, ref Vector2 logoDrawCenter, ref float logoRotation, ref float logoScale, ref Color drawColor)
    {
        Texture2D logo = MenuLoader.CurrentMenu.Logo.Value;
        Vector2 logoDrawPos = new Vector2(215, 150f);
        float scale = logoScale;
        scale *= 0.26f;
        drawColor = Color.White;
        drawColor.A = 0;
        spriteBatch.Draw(logo, logoDrawPos, new Rectangle(0, 0, logo.Width, logo.Height), drawColor, logoRotation, new Vector2(logo.Width * 0.5f, logo.Height * 0.5f), scale, SpriteEffects.None, 0f);
        return false;

    }
}

