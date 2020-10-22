using BeatSaberMarkupLanguage.MenuButtons;
using CountersPlus.UI.FlowCoordinators;
using System;
using Zenject;
using HMUI;
using BeatSaberMarkupLanguage;

namespace CountersPlus.UI
{
    // Code taken from DiUi. Don't like field injection? Make a PR.
    internal class MenuButtonManager : IInitializable, IDisposable
    {
        private MenuButton menuButton;
        [Inject] private MainFlowCoordinator mainFlowCoordinator;
        [Inject] private CountersPlusSettingsFlowCoordinator flowCoordinator;

        public void Initialize()
        {
            menuButton = new MenuButton("Counters+", "Configure Counters+ settings.", SummonFlowCoordinator);
            MenuButtons.instance.RegisterButton(menuButton);
        }

        private void SummonFlowCoordinator()
        {
            mainFlowCoordinator.PresentFlowCoordinator(flowCoordinator, animationDirection: ViewController.AnimationDirection.Vertical);
        }

        public void Dispose()
        {
            if (MenuButtons.IsSingletonAvailable)
            {
                MenuButtons.instance.UnregisterButton(menuButton);
            }
        }
    }
}
