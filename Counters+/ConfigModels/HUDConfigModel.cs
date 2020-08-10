using IPA.Config.Stores.Attributes;
using IPA.Config.Stores.Converters;
using System.Collections.Generic;
using UnityEngine;

namespace CountersPlus.ConfigModels
{
    public class HUDConfigModel
    {
        public string DisplayName => "HUD Settings";
        public HUDCanvas MainCanvasSettings { get; set; } = new HUDCanvas() { Name = "Main", IsMainCanvas = true };
        [UseConverter(typeof(ListConverter<HUDCanvas>))]
        public List<HUDCanvas> OtherCanvasSettings { get; set; } = new List<HUDCanvas>();
    }

    public class HUDCanvas
    {
        public string Name { get; set; } = "New Canvas";

        [Ignore]
        public bool IsMainCanvas = false;
        
        public virtual bool ParentedToBaseGameHUD { get; set; } = true;
        public virtual bool IgnoreNoTextAndHUDOption { get; set; } = false;
        public virtual float Size { get; set; } = 10;
        public virtual float PositionScale { get; set; } = 10;
        [Ignore]
        public virtual Vector3 Position => new Vector3(Pos_X, Pos_Y, Pos_Z);
        public virtual float Pos_X { get; set; } = 0;
        public virtual float Pos_Y { get; set; } = 0;
        public virtual float Pos_Z { get; set; } = 7;
        [Ignore]
        public virtual Vector3 Rotation => new Vector3(Rot_X, Rot_Y, Rot_Z);
        public virtual float Rot_X { get; set; } = 0;
        public virtual float Rot_Y { get; set; } = 0;
        public virtual float Rot_Z { get; set; } = 0;
        public virtual bool AttachHUDToCamera { get; set; } = false;
        public virtual string AttachedCamera { get; set; } = "Main Camera";
        public virtual bool IgnoreShockwaveEffect { get; set; } = true;
    }
}
