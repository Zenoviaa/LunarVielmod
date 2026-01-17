using System.Collections.Generic;
using Terraria.ModLoader;

namespace Stellamod.Content.Gores
{
    public class AutoGoreLoader : ModSystem
    {
        private static Dictionary<string, ModGore> _goreLookup;
        public override void OnModLoad()
        {
            base.OnModLoad();
            _goreLookup = new Dictionary<string, ModGore>();
            foreach (var modGore in ModContent.GetContent<ModGore>())
            {
                _goreLookup.TryAdd(modGore.Name, modGore);
            }
        }
        public override void Unload()
        {
            base.Unload();
            _goreLookup = null;
        }


        public static int[] FindGores(string rootName)
        {
            List<int> goreTypes = new List<int>();
            int index = 0;
            bool found = true;
            while (found)
            {
                string name = rootName + "_Gore" + "_" + index.ToString();
                if (_goreLookup.ContainsKey(name))
                {
                    found = true;
                    goreTypes.Add(_goreLookup[name].Type);
                }
                else
                {
                    found = false;
                }
                index++;
            }
            return goreTypes.ToArray();
        }
    }
}
