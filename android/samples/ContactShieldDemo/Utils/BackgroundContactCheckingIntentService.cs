/*
        Copyright 2020-2021. Huawei Technologies Co., Ltd. All rights reserved.

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
using Android.Nfc;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Huawei.Hms.Contactshield;

namespace XamarinHmsContactShieldDemo.Utils
{
    [Service(Exported = true, Enabled = true, Name = "Huawei.projectname.utils.BackgroundContactCheckingIntentService")]
    public class BackgroundContactCheckingIntentService : IntentService
    {
        private const string TAG = "ContactShield_BackgroundContackCheckingIntentService";
        private ContactShieldEngine contactShield;

        public BackgroundContactCheckingIntentService()
        {
            
        }

        public override void OnCreate()
        {
            base.OnCreate();
            contactShield = ContactShield.GetContactShieldEngine(this);
            Log.Debug(TAG, "BackgroundContackCheckingIntentService onCreate");
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            Log.Debug(TAG, "BackgroundContackCheckingIntentService onDestroy");
        }
        protected override void OnHandleIntent(Intent intent)
        {
            if(intent != null)
            {
                contactShield.HandleIntent(intent, new ContactShieldCallBackClass());
            }
        }

        class ContactShieldCallBackClass : Java.Lang.Object, IContactShieldCallback
        {
            public void OnHasContact(string p0)
            {
                Log.Debug(TAG, "OnHasContact");
            }

            public void OnNoContact(string p0)
            {
                Log.Debug(TAG, "OnNoContact");
            }
        }
    }

    
}