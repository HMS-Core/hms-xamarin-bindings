/*
        Copyright 2020. Huawei Technologies Co., Ltd. All rights reserved.

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
using Android.Views;
using Android.Widget;
using XamarinAdsLiteDemo.Dialogs;

namespace XamarinAdsLiteDemo.Activities
{
    [Activity(Label = "ProtocolActivity")]
    public class ProtocolActivity : BaseActivity, ProtocolDialog.IProtocolDialog
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Title = GetString(Resource.String.privacy_settings);

            ShowProtocoldialog();
        }
        /// <summary>
        /// Display protocol dialog
        /// </summary>
        private void ShowProtocoldialog()
        {
            //Start to process the protocol dialog box.
            ProtocolDialog dialog = new ProtocolDialog(this);
            dialog.SetCallback(this);
            dialog.SetCanceledOnTouchOutside(false);
            dialog.Show();
        }

        public void Agree()
        {
            Finish();
        }

        public void Cancel()
        {
            Finish();
        }
    }
}