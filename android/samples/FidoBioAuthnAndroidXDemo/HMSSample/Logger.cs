/*
 		Copyright 2021. Huawei Technologies Co., Ltd. All rights reserved.

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
using Android.App;
using Android.Views;
using Android.Widget;
using System;

namespace Fido_BioAuthn_AndroidX.HMSSample
{
    public class Logger
    {
        private LinearLayout ll_log;
        private ScrollView sv_log;
        private Activity context;
        public Logger(LinearLayout ll_log, ScrollView sv_log, Activity context)
        {
            this.ll_log = ll_log;
            this.sv_log = sv_log;
            this.context = context;
            this.ll_log.LongClick += (sender, e) => { Clean(); };
        }
        public void ShowLog(string msg)
        {
            context.RunOnUiThread(() =>
            {
                View tvView = ll_log;
                View svView = sv_log;
                if (tvView is LinearLayout linearLayout)
                {
                    linearLayout.AddView(new TextView(context) { Text = msg });
                }
                if (svView is ScrollView scrollView)
                {
                    scrollView.FullScroll(FocusSearchDirection.Down);
                    scrollView.PostDelayed(() => scrollView.SmoothScrollTo(0, scrollView.Bottom), 200);
                }

            });
        }

        public void Info(string tag, string msg)
        {
            Android.Util.Log.Info(tag, msg);
            ShowLog($"[{tag}]: {msg}");
        }
        public void Error(string tag, string msg)
        {
            Android.Util.Log.Error(tag, msg);
            ShowLog($"[{tag}]: {msg}");
        }
        public void Clean() { ll_log.RemoveAllViews(); }
        public void CleanEvent(object sender, EventArgs e) { ll_log.RemoveAllViews(); }
    }
}