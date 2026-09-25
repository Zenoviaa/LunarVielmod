using System;

namespace Stellamod.Common.GooberDialogue;

public record class FunctionInvocationAction(Action Action) : ACutsceneAction
{
    private bool _invoked;
    public override void Update(float elapsedTime)
    {
        base.Update(elapsedTime);
        if (!_invoked)
        {
            Action();
            _invoked = true;
        }
    }

    public override bool IsComplete(float elapsedTime)
    {
        return true;
    }
}

