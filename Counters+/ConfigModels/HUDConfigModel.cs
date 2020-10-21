using BeatSaberMarkupLanguage.Attributes;
using IPA.Config.Stores.Attributes;
using IPA.Config.Stores.Converters;
using System;
using System.Collections;
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
        [UIValue(nameof(Name))]
        public virtual string Name { get; set; } = "New Canvas";

        [Ignore]
        public bool IsMainCanvas = false;

        [UIValue(nameof(ParentedToBaseGameHUD))]
        public virtual bool ParentedToBaseGameHUD { get; set; } = true;
        [UIValue(nameof(IgnoreNoTextAndHUDOption))]
        public virtual bool IgnoreNoTextAndHUDOption { get; set; } = false;
        [UIValue(nameof(Size))]
        public virtual float Size { get; set; } = 10;
        [UIValue(nameof(PositionScale))]
        public virtual float PositionScale { get; set; } = 10;
        [Ignore]
        public virtual Vector3 Position => new Vector3(Pos_X, Pos_Y, Pos_Z);
        [UIValue(nameof(Pos_X))]
        public virtual float Pos_X { get; set; } = 0;
        [UIValue(nameof(Pos_Y))]
        public virtual float Pos_Y { get; set; } = 0;
        [UIValue(nameof(Pos_Z))]
        public virtual float Pos_Z { get; set; } = 7;
        [Ignore]
        public virtual Vector3 Rotation => new Vector3(Rot_X, Rot_Y, Rot_Z);
        [UIValue(nameof(Rot_X))]
        public virtual float Rot_X { get; set; } = 0;
        [UIValue(nameof(Rot_Y))]
        public virtual float Rot_Y { get; set; } = 0;
        [UIValue(nameof(Rot_Z))]
        public virtual float Rot_Z { get; set; } = 0;
        [UIValue(nameof(AttachHUDToCamera))]
        public virtual bool AttachHUDToCamera { get; set; } = false;
        public virtual string AttachedCamera { get; set; } = "Main Camera";
        [UIValue(nameof(IgnoreShockwaveEffect))]
        public virtual bool IgnoreShockwaveEffect { get; set; } = true;
        [UIValue(nameof(CurveRadius))]
        public virtual float CurveRadius { get; set; } = 0;

        #region UI
        public event Action OnCanvasSettingsChanged;
        public event Action OnCanvasSettingsApply;

        [UIAction("fire-update")]
        public void OnChanged(object _)
        {
            SharedCoroutineStarter.instance.StartCoroutine(DelayedFire(OnCanvasSettingsChanged));
        }

        [UIAction("fire-apply")]
        public void OnApply()
        {
            SharedCoroutineStarter.instance.StartCoroutine(DelayedFire(OnCanvasSettingsApply));
        }

        private IEnumerator DelayedFire(Action action)
        {
            yield return new WaitForEndOfFrame();
            action?.Invoke();
        }
        #endregion
    }
}
