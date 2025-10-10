namespace Stellamod.UI
{
    public interface IIndexedUI
    {
        int GetIndex();

    }
    public static class IndexedUIExtensions
    {
        public static int IndexCompareTo(this IIndexedUI a, IIndexedUI b)
        {
            return a.GetIndex().CompareTo(b.GetIndex());
        }
    }
}
