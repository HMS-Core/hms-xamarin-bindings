﻿/*
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
using System.Globalization;

using Android.App;
using Android.OS;
using Android.Widget;
using Android.Content;
using Android.Views;
using Android.Util;

using Huawei.Hms.Analytics;
using Huawei.Hms.Analytics.Type;
using System.Collections.Generic;
using Android.Support.Design.Widget;
using HiAnalyticsXamarinAndroidDemo.Data;
using System.Linq;
using Huawei.Hmf.Tasks;
using System.Threading.Tasks;
using Java.Util;

namespace HiAnalyticsXamarinAndroidDemo
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : BaseActivity
    {
        private TextView txtQuestion;
        HiAnalyticsInstance instance;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            BottomNavigationView navigation = FindViewById<BottomNavigationView>(Resource.Id.navigation);
            navigation.SetOnNavigationItemSelectedListener(this);

            // Enable Analytics Kit Log
            HiAnalyticsTools.EnableLog();
            // Generate the Analytics Instance
            instance = HiAnalytics.GetInstance(this);

            //You can also use Context initialization
            //Context context = this.ApplicationContext;
            //instance = HiAnalytics.GetInstance(context);

            // Enable collection capability
            instance.SetAnalyticsEnabled(true);

            instance.SetReportPolicies(new List<ReportPolicy> { ReportPolicy.OnAppLaunchPolicy });

            //Get AAID
            GetAAID();

            //Adds default event parameters. 
            Bundle bundle = new Bundle();
            bundle.PutString("Default Event Params", "This is a default event param.");
            instance.AddDefaultEventParams(bundle);

            FindViewById(Resource.Id.true_button).Click += AnswerButton_Click;
            FindViewById(Resource.Id.false_button).Click += AnswerButton_Click;
            txtQuestion = FindViewById<TextView>(Resource.Id.txtQuestion);

            RefreshUi();
        }

        private void RefreshUi()
        {
            QuizData.InitQuiz();
            txtQuestion.Text = QuizData.Questions.FirstOrDefault().Text;
        }

        private void AnswerButton_Click(object sender, EventArgs e)
        {
            instance.SetUserId("UserID");
            QuizData.Result += ((sender as Button).Text == QuizData.CurrentQuestion.Answer) ? 10 : 0;

            // Initiate Parameters
            Bundle bundle_pre1 = new Bundle();
            bundle_pre1.PutString("QuestionId", QuizData.CurrentQuestion.Id.ToString());
            bundle_pre1.PutString("Question", QuizData.CurrentQuestion.Text);
            bundle_pre1.PutString("RealAnswer", QuizData.CurrentQuestion.Answer);
            bundle_pre1.PutString("UserAnswer", (sender as Button).Text);
            bundle_pre1.PutString("SubmitDate", DateTime.UtcNow.ToLongDateString());


            Bundle bundle_pre2 = new Bundle();
            bundle_pre2.PutString(HAParamType.Usergroupname, "Beginner");
            bundle_pre2.PutString(HAParamType.Usergrouplevel, "3");
            Bundle bundle_pre3 = new Bundle();
            bundle_pre3.PutString(HAParamType.Skillname, "Smithing");
            bundle_pre3.PutString(HAParamType.Skilllevel, "2");


            List<IParcelable> listBundle = new List<IParcelable>();
            listBundle.Add(bundle_pre2);
            listBundle.Add(bundle_pre3);


            Bundle commonBundle = new Bundle();
            commonBundle.PutParcelableArrayList("items", listBundle);

            // Report a custom Event
            instance.OnEvent("OnAnsweringEvent", bundle_pre1);
            instance.OnEvent("UserLevelInfo", commonBundle);

            if (QuizData.CurrentQuestion.Id < QuizData.Questions.Count)
            {
                QuizData.CurrentQuestion = QuizData.Questions.FirstOrDefault(a => a.Id == QuizData.CurrentQuestion.Id + 1);
                txtQuestion.Text = QuizData.CurrentQuestion.Text;
            }
            else
                ResultPage();
        }

        public override bool MoveTaskToBack(bool nonRoot)
        {
            //Report an event upon app startup.
            ReportPolicy appLaunchPolicy = ReportPolicy.OnAppLaunchPolicy;
            //Report an event at the specified interval.
            ReportPolicy scheduledTimePolice = ReportPolicy.OnScheduledTimePolicy;
            //Set the event reporting interval to 600 seconds.
            scheduledTimePolice.Threshold = 600;
            List<ReportPolicy> reportPolicies = new List<ReportPolicy>();
            //Add the OnAppLaunchPolicy and OnScheduledTimePolicy policies.
            reportPolicies.Add(scheduledTimePolice);
            reportPolicies.Add(appLaunchPolicy);
            //Set the OnAppLaunchPolicy and OnScheduledTimePolicy policies.
            instance.SetReportPolicies(reportPolicies);
            return base.MoveTaskToBack(nonRoot);
        }

        private void ResultPage()
        {
            Intent intent = new Intent(this, typeof(ResultActivity));
            StartActivity(intent);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.mainMenu, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.menuOrder:
                    Intent intent = new Intent(this, typeof(OrderActivity));
                    StartActivity(intent);
                    return true;

            }
            return base.OnOptionsItemSelected(item);
        }
        private async void GetAAID()
        {
            Task<string> aaidTask = instance.GetAAIDAsync();
            await aaidTask;
            if (aaidTask.IsCompletedSuccessfully)
            {
                Console.WriteLine("AAID  retrieved: " + aaidTask.Result);
            }
            else
                Console.WriteLine("AAID cannot retrieved. " + aaidTask.Result);
        }

        public override int GetContentViewId()
        {
            return Resource.Layout.quiz_view;
        }

        public override int GetNavigationMenuItemId()
        {
            if (QuizData.IsQuizDone)
                RefreshUi();
            return Resource.Id.navigation_home;
        }

    }
}