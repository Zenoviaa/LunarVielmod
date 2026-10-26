using System.Collections.Generic;
using Terraria.WorldBuilding;

namespace Stellamod.WorldG;

public class PassWriter
{
    private int _insertionIndex;
    public PassWriter(List<GenPass> tasks)
    {
        this.Tasks = tasks;
    }
    public readonly List<GenPass> Tasks;

    public void SetInsertionIndex(int index)
    {
        _insertionIndex = index;
    }
    public void SetInsertionIndex(string passName) => SetInsertionIndex(Tasks.FindIndex(genpass => genpass.Name.Equals(passName)));
    public void NextPass(GenPass genPass)
    {
        _insertionIndex++;
        Tasks.Insert(_insertionIndex, genPass);

    }
    public void DisablePass(string passName)
    {
        Tasks[Tasks.FindIndex(genpass => genpass.Name.Equals(passName))].Disable();

    }
    public void ReplacePass(GenPass genPass)
    {
        Tasks[_insertionIndex] = genPass;
    }
}
