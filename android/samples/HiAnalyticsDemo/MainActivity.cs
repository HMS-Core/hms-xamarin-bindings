/*
*       Copyright 2020. Huawei Technologies Co., Ltd. All rights reserved.

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

            GetAAID();

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
            Bundle bundle = new Bundle();
            bundle.PutString("QuestionId", QuizData.CurrentQuestion.Id.ToString());
            bundle.PutString("Question", QuizData.CurrentQuestion.Text);
            bundle.PutString("RealAnswer", QuizData.CurrentQuestion.Answer);
            bundle.PutString("UserAnswer", (sender as Button).Text);
            bundle.PutString("SubmitDate", DateTime.UtcNow.ToLongDateString());

            // Report a custom Event
            instance.OnEvent("OnAnsweringEvent", bundle);

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
            scheduledTimePolice.Threshold = (Java.Lang.Integer)600;
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
        private void GetAAID()
        {
            Huawei.Hmf.Tasks.Task getAAID = instance.AAID;
            getAAID.AddOnSuccessListener(new GetAAIDSuccessListener()).AddOnFailureListener(new GetAAIDFailureListener());
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


        class GetAAIDSuccessListener : Java.Lang.Object, IOnSuccessListener
        {

            public void OnSuccess(Java.Lang.Object obj)
            {
                string AAID = (string)obj;
                Log.Info("GetAAIDSuccessListener", "AAID function executed. AAID: " + AAID);
            }
        }

        class GetAAIDFailureListener : Java.Lang.Object, IOnFailureListener
        {
            public void OnFailure(Java.Lang.Exception ex)
            {
                Log.Info("AIID cannot retrieved", "AAID function executed. AAID: " + ex);
            }
        }
    }
}