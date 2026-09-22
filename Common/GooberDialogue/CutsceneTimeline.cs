namespace Stellamod.Common.GooberDialogue;

public class CutsceneTimeline
{
    public ACutsceneType Cutscene;
    public ACutsceneAction Current
    {
        get => Cutscene.Actions[CutsceneIndex];
    }

    public float ElapsedTime
    {
        get;
        set;
    }

    public int CutsceneIndex
    {
        get;
        set;
    }

    public TimelineState State
    {
        get;
        private set;
    }

    public void Play(ACutsceneType type)
    {
        Cutscene = type;
        ElapsedTime = 0;
        CutsceneIndex = -1;
        State = TimelineState.IsPlaying;
        Next();
    }
    public void Next()
    {
        CutsceneIndex++;
        ElapsedTime = 0;
    }
    public void Update()
    {
        if (Cutscene == null)
            return;

        //Using the tried and true method of rebuilding the entire cutscene from scratch
        //This makes netsyncing wayyy less of a pain and testing individual parts of the cutscene easier
        Cutscene.Clear();
        Cutscene.Create();

        ElapsedTime++;
        if (CutsceneIndex >= Cutscene.Actions.Count)
        {
            State = TimelineState.None;
            return;
        }
        Current.Update(ElapsedTime);
        if (Current.IsComplete(ElapsedTime))
            Next();
    }
}

