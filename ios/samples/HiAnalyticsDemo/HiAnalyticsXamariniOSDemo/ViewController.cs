/*
 * Copyright 2020-2021. Huawei Technologies Co., Ltd. All rights reserved.

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

     http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

using System;
using UIKit;
using Huawei.Hms.Analytics;
using Foundation;

namespace HiAnalyticsXamariniOSDemo
{
    public partial class ViewController : UIViewController
    {
        private UIViewController quizViewController;
        private UIViewController settingsViewController;


        public ViewController(IntPtr handle) : base(handle)
        {

        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.

            SetReportPolicies();

            //Adds default event parameters. 
            var keys = new[]
            {
                    new NSString("Default Event Params"), 
            };

            var objects = new NSObject[]
            {
                  new NSString( "This is a default event param."), 
            };

            var dictionary = new NSDictionary<NSString, NSObject>(keys, objects); 
            HiAnalytics.AddDefaultEventParams(dictionary);

            quizViewController = Storyboard?.InstantiateViewController("Quiz");
            quizViewController.View.Frame = View.Frame;
            SwitchViewController(null, quizViewController);
        }

     

        private void SetReportPolicies()
        {
            HAReportPolicy appLaunchPolicy = HAReportPolicy.OnAppLaunchPolicy;
            HAReportPolicy scheduledTimePolicy = HAReportPolicy.OnScheduledTimePolicy(1000);

            HiAnalytics.SetReportPolicies(new HAReportPolicy[] { appLaunchPolicy, scheduledTimePolicy });
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            if (quizViewController != null && quizViewController!.View.Superview == null)
            {
                quizViewController = null;
            }
            if (settingsViewController != null && settingsViewController!.View.Superview == null)
            {
                settingsViewController = null;
            }
        }

        partial void Change_Selection(UISegmentedControl sender)
        {

            if (settingsViewController?.View.Superview == null)
            {
                if (settingsViewController == null)
                {
                    settingsViewController = Storyboard?.InstantiateViewController("Settings") as SettingsViewController;
                }
            }
            else if (quizViewController?.View.Superview == null)
            {
                if (quizViewController == null)
                {
                    quizViewController = Storyboard?.InstantiateViewController("Quiz") as QuizViewController;
                }
            }
            var selection = sender.SelectedSegment;
            switch (selection)
            {
                case 0:
                    SwitchViewController(settingsViewController, quizViewController);
                    break;
                case 1:
                    SwitchViewController(quizViewController, settingsViewController);
                    break;
                default:
                    break;
            }
        }

        private void SwitchViewController(UIViewController source, UIViewController target)
        {
            if (source != null)
            {
                source?.WillMoveToParentViewController(null);
                source?.View.RemoveFromSuperview();
                source?.RemoveFromParentViewController();
            }
             if (target != null)
            {
                this.AddChildViewController(target);
                this.View.InsertSubview(target?.View, 0);
                target?.DidMoveToParentViewController(this);
            }
        }
    }
}