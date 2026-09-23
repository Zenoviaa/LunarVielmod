namespace Stellamod.Common.DialogueTowning;

/// <summary>
/// Interface for creation a dialogue option on the talk menu
/// </summary>
public interface ITalkingOption
{
    string GetDisplayName();
    void Talk();
    void Show(SpriteBatch spriteBatch);
}
