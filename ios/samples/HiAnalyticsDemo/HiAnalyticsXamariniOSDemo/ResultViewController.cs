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
using CoreGraphics;
using Foundation;
using Huawei.Hms.Analytics;
using UIKit;

namespace HiAnalyticsXamariniOSDemo
{
    public partial class ResultViewController : UIViewController
    {

        public ResultViewController() : base("ResultViewController", null)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            btnSubmit.Layer.CornerRadius = 0.5f * btnSubmit.Bounds.Size.Width;

            lblScore.Text = Data.Result + "/" + Data.Questions.Count * 10;
            // Perform any additional setup after loading the view, typically from a nib.
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }


        partial void SubmitScore_Clicked(UIButton sender)
        {
            var keys = new[]
           {
                    new NSString("Score"),
                    new NSString("SubmitDate")
                };

            var objects = new NSObject[]
            {
                  new NSString(Data.Result.ToString()),
                  new NSString(DateTime.Now.ToShortDateString()),
            };

            var dictionary = new NSDictionary<NSString, NSObject>(keys, objects);
            HiAnalytics.OnEvent(HAEventType.SubmitScore, dictionary);

            var alert = UIAlertController.Create("", "Your score reported successfully", UIAlertControllerStyle.Alert);
            alert.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, (action) => {
                Data.IsQuizDone=true;
                DismissViewController(true, null);
            }));
            this.PresentViewController(alert, true, null);
        }
    }
}

