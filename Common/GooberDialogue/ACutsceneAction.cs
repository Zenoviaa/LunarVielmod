namespace Stellamod.Common.GooberDialogue;

public abstract record class ACutsceneAction
{
    /// <summary>
    /// Called for every tick
    /// </summary>
    /// <param name="elapsedTime">The amount of time that's passed since this action started</param>
    public virtual void Update(float elapsedTime) { }

    /// <summary>
    /// Determines whether the cutscene is complete
    /// </summary>
    /// <returns></returns>
    public abstract bool IsComplete(float elapsedTime);
}

