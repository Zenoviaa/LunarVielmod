using Terraria.GameContent.UI.Elements;

namespace Stellamod.UI
{
    public class IndexedUIText : UIText,
        IIndexedUI
    {
        private int _index;
        public IndexedUIText(int index, string text, float textScale = 1f, bool large = false) : base(text, textScale, large)
        {
            _index = index;
        }
        public int GetIndex()
        {
            return _index;
        }
        public override int CompareTo(object obj)
        {
            if (obj is IIndexedUI otherSlot)
            {
                return this.IndexCompareTo(otherSlot);
            }
            return base.CompareTo(obj);
        }

    }
}
