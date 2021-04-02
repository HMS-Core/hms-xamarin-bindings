/*
 * Copyright 2020. Huawei Technologies Co., Ltd. All rights reserved.

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
using System.Collections.Generic;
using System.Linq;
using Foundation;
using Huawei.Hms.Analytics;
using UIKit;

namespace HiAnalyticsXamariniOSDemo
{
    public partial class QuizViewController : UIViewController
    {
        public QuizViewController() : base()
        {
        }

        public QuizViewController(IntPtr ptr) : base(ptr)
        {
        }

       

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.
            RefreshUi();
            HiAnalytics.SetReportPolicies(new HAReportPolicy[] { HAReportPolicy.OnAppLaunchPolicy });
            HiAnalytics.SetAnalyticsEnabled(true);
            HiAnalytics.UserProfiles(true);

        }

        public override void ViewDidAppear(bool animated)
        {
            if (Data.IsQuizDone)
                RefreshUi();
            base.ViewDidAppear(animated);
        }

        private void RefreshUi()
        {
            Data.InitQuiz();

            lblQuestionNumber.Text = Data.Questions.FirstOrDefault().Id.ToString();
            lblQuestion.Text = Data.Questions.FirstOrDefault().Text;
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }



        partial void BtnNext_TouchUpInside(UIButton sender)
        {
            HiAnalytics.SetUserId("UserID");
            Data.Result += (sender.TitleLabel.Text == Data.CurrentQuestion.Answer.ToString()) ? 10 : 0;

            var keys = new[]
            {
                    new NSString("QuestionId"),
                    new NSString("Question"),
                    new NSString("RealAnswer"),
                    new NSString("UserAnswer"),
                    new NSString("SubmitDate"),
                };

            var objects = new NSObject[]
            {
                  new NSString(Data.CurrentQuestion.Id.ToString()),
                  new NSString(Data.CurrentQuestion.Text),
                  new NSString(Data.CurrentQuestion.Answer.ToString()),
                  new NSString(sender.TitleLabel.Text),
                  new NSString(DateTime.Now.ToLongDateString()),
            };

            var dictionary = new NSDictionary<NSString, NSObject>(keys, objects);
            HiAnalytics.OnEvent("OnAnsweringEvent", dictionary);

            if (Data.CurrentQuestion.Id < Data.Questions.Count)
            {
                Data.CurrentQuestion = Data.Questions.FirstOrDefault(a => a.Id == Data.CurrentQuestion.Id + 1);
                lblQuestion.Text = Data.CurrentQuestion.Text;
                lblQuestionNumber.Text = Data.CurrentQuestion.Id.ToString();
            }
            else
                ResultPage();
        }

        private void ResultPage()
        {
            PresentViewController(new ResultViewController(), true, null);
        }


    }
}

