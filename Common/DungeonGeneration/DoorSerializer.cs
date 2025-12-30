using Terraria.ModLoader.IO;

namespace Stellamod.Common.DungeonGeneration
{
    public class DoorSerializer : TagSerializer<Door, TagCompound>
    {
        public override Door Deserialize(TagCompound tag)
        {
            return (Door)tag.Get<int>("door");
        }

        public override TagCompound Serialize(Door value)
        {
            return new TagCompound
            {
                ["door"] = (int)value
            };
        }
    }
}
