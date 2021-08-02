/*
*       Copyright 2020-2021. Huawei. Technologies Co., Ltd. All rights reserved.

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
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Huawei.Hms.Analytics;
using Huawei.Hms.Analytics.Type;
using HiAnalyticsXamarinAndroidDemo.Data;

namespace HiAnalyticsXamarinAndroidDemo
{
    [Activity(Label = "ResultActivity")]
    public class ResultActivity : AppCompatActivity
    {
        HiAnalyticsInstance instance;
       
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_result);

            instance = HiAnalytics.GetInstance(this);


            FindViewById<TextView>(Resource.Id.txtScore).Text += QuizData.Result + "/" + QuizData.Questions.Count * 10;
            FindViewById<Button>(Resource.Id.submitButton).Click += SubmitButton_Click;
        }

        private void SubmitButton_Click(object sender, EventArgs e)
        {
            // TODO: Report score by using SUBMITSCORE Event
            // Initiate Parameters
            Bundle bundle = new Bundle();
            bundle.PutLong(HAParamType.Score, QuizData.Result);

            // Report a preddefined Event
            instance.OnEvent(HAEventType.Submitscore, bundle);

            DisplayAlert("Your score reported successfully");

           
        }

        public virtual void DisplayAlert(string message)
        {
            Android.App.AlertDialog.Builder alertDialog = new Android.App.AlertDialog.Builder(this);
            alertDialog.SetTitle("");
            alertDialog.SetMessage(message);
            alertDialog.SetPositiveButton("OK", delegate
            {
                alertDialog.Dispose();
                QuizData.IsQuizDone = true;
                StartActivity(typeof(MainActivity));
            });
            alertDialog.Show();
        }

    }
}