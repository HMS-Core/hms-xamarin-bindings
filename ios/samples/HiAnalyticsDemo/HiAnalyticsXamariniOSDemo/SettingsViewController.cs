/*
*       Copyright 2020-2021. Huawei Technologies Co., Ltd. All rights reserved.

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
using Huawei.Hms.Analytics;
using UIKit;

namespace HiAnalyticsXamariniOSDemo
{
    public partial class SettingsViewController : UIViewController
    {
        public SettingsViewController() : base()
        { 
        }

        public SettingsViewController(IntPtr ptr) : base(ptr)
        { 
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }

      
        partial void BtnSubmit_TouchUpInside(UIButton sender)
        {
            HiAnalytics.SetUserProfile("Occupation", string.IsNullOrEmpty(txtOccupation.Text) ? "Not specified" : txtOccupation.Text);
            HiAnalytics.SetUserProfile("Age", string.IsNullOrEmpty(txtAge.Text) ? "Not specified" : txtAge.Text);
            HiAnalytics.SetUserProfile("FavoriteColor", string.IsNullOrEmpty(txtColor.Text) ? "Not specified" : txtColor.Text);

            var alert = UIAlertController.Create("", "User profile set successfully", UIAlertControllerStyle.Alert);
            alert.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, (action) => {
                DismissViewController(true, null);
            }));
            this.PresentViewController(alert, true, null);
        }
    }
}

