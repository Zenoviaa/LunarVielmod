namespace Stellamod.Core.LoadingSystems
{
    interface IOrderedLoadable
    {
        void Load();
        void Unload();
        float Priority { get; }
    }
}
