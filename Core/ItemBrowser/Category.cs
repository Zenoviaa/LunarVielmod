using Terraria;

namespace Stellamod.Core.ItemBrowser
{
    public class Category
    {
        public Category(string displayName, ItemBrowserUtility.CompareFunction compareFunction, params Category[] subCategories)
        {
            this.displayName = displayName;
            this.subCategories = subCategories;
            for(int i = 0; i < subCategories.Length; i++)
            {
                subCategories[i].parentCategory = this;
            }
            this.compareFunction = compareFunction;
            ItemBrowserUtility.PopulateItems(this);
        }

        public string displayName;
        public ItemBrowserUtility.CompareFunction compareFunction;
        public Category parentCategory;
        public Category[] subCategories;
        public Item[] items;
        public Category[] GetCategories()
        {
            Category[] categories = new Category[subCategories.Length + 1];
            categories[0] = this;
            for(int i = 1; i < categories.Length; i++)
            {
                categories[i] = subCategories[i - 1];
            }

            return categories;
        }
    }
}
