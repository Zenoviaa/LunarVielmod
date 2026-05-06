namespace Stellamod.Core.Utilities;

public struct Outliner
{
    public bool warning;
    public bool attacking;
    public Color outlineColor;
    public void SetDefaults()
    {
        warning = false;
        attacking = false;
    }
    public void Update()
    {
        Color targetOutlineColor = Color.Transparent;
        if (attacking)
        {
            targetOutlineColor = Color.Red;
        }
        else if(warning)
        {
            targetOutlineColor = Color.Yellow;
        }
        outlineColor = Color.Lerp(outlineColor, targetOutlineColor, 0.1f);
    }
}
