using ItemChanger;
using ItemChanger.Internal;
using Newtonsoft.Json;
using System;
using UnityEngine;

namespace GodhomeRandomizer.IC
{
    [Serializable]
    public class GodhomeSprite : ISprite
    {
        private static SpriteManager EmbeddedSpriteManager = new(typeof(GodhomeSprite).Assembly, "GodhomeRandomizer.Resources.Sprites.");
        public string Key { get; set; }
        public GodhomeSprite(string key)
        {
            if (!string.IsNullOrEmpty(key))
                Key = key;
        }
        [JsonIgnore]
        public Sprite Value => EmbeddedSpriteManager.GetSprite(Key);
        public ISprite Clone() => (ISprite)MemberwiseClone();
    }
}