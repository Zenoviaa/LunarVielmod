using Terraria;

namespace Stellamod.Content.Areas.PunkerTown.BossesPT.PunkerPrime;

//On top of that we need to make the custom draw code for the armrs
//We're just going to do this with forward kinematics since the arms don't need to be super precisely reaching for something, just coming out of the body really
//So let's make a simple system
public class PunkerPrimeArmPart
{
    public PunkerPrimeArmPart(PunkerPrimeArmPart parent, Texture2D texture, float initialAngle)
    {
        this.parent = parent;
        this.texture = texture;
        this.drawOrigin = new Vector2(0f, texture.Height / 2f);
        this.angle = initialAngle;
        this.length = texture.Width;
        this.color = Color.White;
    }
    public PunkerPrimeArmPart parent;
    public Texture2D texture;
    public Vector2 drawOrigin;
    public Vector2 rootPosition;
    public Vector2 endPosition;
    public float angle;
    public float length;
    public Color color;

    public void Update()
    {
        if (parent != null)
        {
            rootPosition = parent.endPosition;
        }
        endPosition = rootPosition + angle.ToRotationVector2() * length;
    }

    public void Draw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        Vector2 drawPosition = rootPosition - screenPos;
        Color finalColor = color.MultiplyRGB(drawColor);
        Vector2 drawScale = Vector2.One;
        spriteBatch.Draw(texture, drawPosition, null, finalColor, angle, drawOrigin, drawScale, SpriteEffects.None, 0);
    }
}
