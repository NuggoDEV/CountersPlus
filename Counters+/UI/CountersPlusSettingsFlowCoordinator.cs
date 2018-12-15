using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomUI.Utilities;
using CustomUI.BeatSaber;
using VRUI;

namespace CountersPlus.UI
{
    /*
     * With this I'm trying to hack together a seperate Settings menu because Brian is having troubles making one.
     * 
     * Not sure how far I'm going to go before I delete all of this in defeat.
     */
    class CountersPlusSettingsFlowCoordinator : FlowCoordinator
    {
        private CountersPlusSettingsViewController controller;
        public MainFlowCoordinator mainFlow;
        protected override void DidActivate(bool firstActivation, ActivationType activationType)
        {
            if (firstActivation) title = "Counters+ Settings";
            if (activationType == ActivationType.AddedToHierarchy)
                ProvideInitialViewControllers(controller, null, null);
        }
    }
}
