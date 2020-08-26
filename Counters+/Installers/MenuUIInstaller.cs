using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.MenuButtons;
using CountersPlus.Custom;
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

namespace CountersPlus.Installers
{
    [RequiresInstaller(typeof(ConfigModelInstaller))]
    public class MenuUIInstaller : MonoInstaller
    {
        private CountersPlusSettingsFlowCoordinator flowCoordinator;

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
                        Container.Bind(hostType).WithId(customCounter.Name).To<object>().FromComponentOnRoot().AsCached();
                    }
                    else
                    {
                        Container.Bind(hostType).WithId(customCounter.Name).To<object>().AsCached();
                    }
                }
            }

            BindViewController<CountersPlusCreditsViewController>();
            BindViewController<CountersPlusMainScreenNavigationController>();
            BindViewController<CountersPlusBlankViewController>();
            BindViewController<CountersPlusSettingSectionSelectionViewController>();
            BindViewController<CountersPlusHorizontalSettingsListViewController>();
            BindViewController<CountersPlusCounterEditViewController>();
            BindViewController<CountersPlusUnimplementedViewController>(); // TODO remove for Counters+ 2.0 Release
            BindViewController<CountersPlusMainSettingsEditViewController>();
            BindViewController<CountersPlusHUDListViewController>();
            BindViewController<CountersPlusHUDEditViewController>();

            flowCoordinator = BeatSaberUI.CreateFlowCoordinator<CountersPlusSettingsFlowCoordinator>();
            Container.InjectSpecialInstance<CountersPlusSettingsFlowCoordinator>(flowCoordinator);

            MenuButton button = new MenuButton("Counters+", "Configure Counters+ settings.", OnClick);
            MenuButtons.instance.RegisterButton(button);
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
            BeatSaberUI.MainFlowCoordinator.PresentFlowCoordinator(flowCoordinator);
        }
    }
}
