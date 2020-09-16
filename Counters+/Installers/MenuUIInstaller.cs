using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.MenuButtons;
using CountersPlus.ConfigModels;
using CountersPlus.Custom;
using CountersPlus.Harmony;
using CountersPlus.UI;
using CountersPlus.UI.FlowCoordinators;
using CountersPlus.UI.SettingGroups;
using CountersPlus.UI.ViewControllers;
using CountersPlus.UI.ViewControllers.Editing;
using CountersPlus.Utils;
using HMUI;
using SiraUtil.Zenject;
using System;
using UnityEngine;
using Zenject;
using HarmonyObj = HarmonyLib.Harmony;

namespace CountersPlus.Installers
{
    [RequiresInstaller(typeof(CoreInstaller))]
    public class MenuUIInstaller : MonoInstaller
    {
        private MenuButton menuButton;

        // Using Zenject for UI lets goooooooooooo
        public override void InstallBindings()
        {
            // CanvasUtility for UI
            Container.Bind<CanvasUtility>().AsSingle().NonLazy();
            Container.Bind<MockCounter>().AsSingle().NonLazy();

            BindSettingsGroup<MainSettingsGroup>();
            BindSettingsGroup<CountersSettingsGroup>();
            BindSettingsGroup<HUDsSettingsGroup>();

            foreach (CustomCounter customCounter in Plugin.LoadedCustomCounters.Values)
            {
                if (customCounter.BSML != null && customCounter.BSML.HasType)
                {
                    Type hostType = customCounter.BSML.HostType;
                    if (hostType.BaseType == typeof(MonoBehaviour))
                    {
                        Container.Bind(hostType).WithId(customCounter.Name).FromComponentOnRoot().AsCached();
                    }
                    else
                    {
                        Container.Bind(hostType).WithId(customCounter.Name).AsCached();
                    }
                }
            }

            BindViewController<CountersPlusCreditsViewController>();
            BindViewController<CountersPlusMainScreenNavigationController>();
            BindViewController<CountersPlusBlankViewController>();
            BindViewController<CountersPlusSettingSectionSelectionViewController>();
            BindViewController<CountersPlusHorizontalSettingsListViewController>();
            BindViewController<CountersPlusCounterEditViewController>();
            BindViewController<CountersPlusMainSettingsEditViewController>();
            BindViewController<CountersPlusHUDListViewController>();
            BindViewController<CountersPlusHUDEditViewController>();

            var flowCoordinator = BeatSaberUI.CreateFlowCoordinator<CountersPlusSettingsFlowCoordinator>();
            Container.InjectSpecialInstance<CountersPlusSettingsFlowCoordinator>(flowCoordinator);

            MenuTransitionsHelperPatch.Patch(Container.ResolveId<HarmonyObj>(CoreInstaller.HARMONY_ID),
                Container.Resolve<CanvasUtility>(),
                Container.Resolve<HUDConfigModel>(),
                this);

            AddButton();
        }

        public void AddButton()
        {
            menuButton = new MenuButton("Counters+", "Configure Counters+ settings.", OnClick);
            MenuButtons.instance.RegisterButton(menuButton);
        }

        public void RemoveButton()
        {
            MenuButtons.instance.UnregisterButton(menuButton);
        }

        private void BindViewController<T>() where T : ViewController
        {
            T view = BeatSaberUI.CreateViewController<T>();
            Container.InjectSpecialInstance<T>(view);
        }

        private void BindSettingsGroup<T>() where T : SettingsGroup
        {
            Container.Bind<SettingsGroup>().To<T>().AsCached().NonLazy();
        }

        private void OnClick()
        {
            var flowCoordinator = Container.TryResolve<CountersPlusSettingsFlowCoordinator>();
            if (flowCoordinator != null)
            {
                BeatSaberUI.MainFlowCoordinator.PresentFlowCoordinator(flowCoordinator);
            }
            else
            {
                Plugin.Logger.Error("Cannot obtain flow coordinator!");
            }
        }
    }
}
