using IPA.Config.Stores.Attributes;
using IPA.Config.Stores.Converters;
using System.Collections.Generic;
using UnityEngine;

namespace CountersPlus.ConfigModels
{
    public class HUDConfigModel
    {
        public string DisplayName => "HUD Settings";
        public HUDCanvas MainCanvasSettings { get; set; } = new HUDCanvas() { Name = "Main" };
        [UseConverter(typeof(ListConverter<HUDCanvas>))]
        public List<HUDCanvas> OtherCanvasSettings { get; set; } = new List<HUDCanvas>();
    }

    public class HUDCanvas
    {
        public string Name { get; set; } = "New Canvas";
        
        public virtual bool AttachBaseGameHUD { get; set; } = true;
        public virtual float HUDSize { get; set; } = 10;
        public virtual float HUDPositionScaleFactor { get; set; } = 10;
        public virtual Vector3 HUDPosition => new Vector3(HUDPosition_X, HUDPosition_Y, HUDPosition_Z);
        public virtual float HUDPosition_X { get; set; } = 0;
        public virtual float HUDPosition_Y { get; set; } = 0;
        public virtual float HUDPosition_Z { get; set; } = 7;
        public virtual Vector3 HUDRotation => new Vector3(HUDRotation_X, HUDRotation_Y, HUDRotation_Z);
        public virtual float HUDRotation_X { get; set; } = 0;
        public virtual float HUDRotation_Y { get; set; } = 0;
        public virtual float HUDRotation_Z { get; set; } = 0;
        public virtual bool AttachHUDToCamera { get; set; } = false;
        public virtual string AttachedCamera { get; set; } = "Main Camera";
    }
}
