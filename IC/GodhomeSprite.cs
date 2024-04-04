using ItemChanger;
using KorzUtils.Helper;
using Newtonsoft.Json;
using System;
using UnityEngine;

namespace GodhomeRandomizer.IC
{
    [Serializable]
    public class GodhomeSprite : ISprite
    {
        #region Constructors

        public GodhomeSprite() { }

        public GodhomeSprite(string key)
        {
            if (!string.IsNullOrEmpty(key))
                Key = key;
        }

        #endregion

        #region Properties

        public string Key { get; set; }

        [JsonIgnore]
        public Sprite Value => SpriteHelper.CreateSprite<GodhomeRandomizer>("Sprites." + Key.Replace("/", ".").Replace("\\", "."));

        #endregion

        public ISprite Clone() => new GodhomeSprite(Key);
    }
}