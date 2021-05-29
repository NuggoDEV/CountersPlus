using HMUI;
using System.Collections.Generic;

namespace CountersPlus.UI.ViewControllers
{
    public class CountersPlusMainScreenNavigationController : ViewController
    {
        protected HashSet<ViewController> viewControllers = new HashSet<ViewController>();
        protected HashSet<ViewController> wasAddedToHierarchy = new HashSet<ViewController>();

        protected ViewController activeViewController;

        public override void __Init(Screen screen, ViewController parentViewController, ContainerViewController containerViewController)
        {
            base.__Init(screen, parentViewController, containerViewController);
            
            foreach (var viewController in viewControllers)
            {
                viewController.__Init(screen, null, null);
            }
        }

        public override void __Activate(bool addedToHierarchy, bool screenSystemEnabling)
        {
            base.__Activate(addedToHierarchy, screenSystemEnabling);

            RefreshActiveViewControllers();
        }

        public override void __Deactivate(bool removedFromHierarchy, bool deactivateGameObject, bool screenSystemDisabling)
        {
            base.__Deactivate(removedFromHierarchy, deactivateGameObject, screenSystemDisabling);

            foreach (var viewController in viewControllers)
            {
                if (viewController.isActivated)
                {
                    viewController.__Deactivate(removedFromHierarchy, true, screenSystemDisabling);
                }
            }
        }

        public void AssignViewController(ViewController controller)
        {
            activeViewController = controller;

            viewControllers.Add(controller);
            
            if (screen != null)
            {
                RefreshActiveViewControllers();
            }
        }

        private void RefreshActiveViewControllers()
        {
            foreach (var otherViewController in viewControllers)
            {
                if (otherViewController.screen == null)
                {
                    otherViewController.__Init(screen, null, null);
                }

                if (otherViewController != activeViewController)
                {
                    if (otherViewController.isActivated)
                    {
                        otherViewController.__Deactivate(false, true, false);
                    }
                }
                else if (!activeViewController.isActivated)
                {
                    var addedToHierarchy = wasAddedToHierarchy.Add(activeViewController);
                    activeViewController.__Activate(addedToHierarchy, false);
                }
            }
        }
    }
}
