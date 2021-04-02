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
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Text.Method;
using Android.Text.Style;
using Android.Views;
using Android.Widget;
using XamarinAdsLiteDemo.Utils;

namespace XamarinAdsLiteDemo.Dialogs
{
    public class ProtocolDialog : Dialog, IDialogInterfaceOnClickListener
    {
        private Context context;

        private TextView titleTV;

        private TextView protocolTV;

        private Button confirmButton;

        private Button cancelButton;

        private LayoutInflater layoutInflater;

        private IProtocolDialog dialogCallback;

        public ProtocolDialog(Context context) : base(context, Resource.Style.Dialog)
        {
            this.context = context;
        }
        //Privacy protocol dialog box callback interface.
        public interface IProtocolDialog
        {
            void Agree();
            void Cancel();
        }
        public void SetCallback(IProtocolDialog callback) { dialogCallback = callback; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Window dialogWindow = Window;
            dialogWindow.RequestFeature(WindowFeatures.NoTitle);

            layoutInflater = LayoutInflater.From(context);

            LinearLayout rootview = (LinearLayout)layoutInflater.Inflate(Resource.Layout.dialog_protocol, null);
            SetContentView(rootview);

            titleTV = FindViewById<TextView>(Resource.Id.uniform_dialog_title);
            titleTV.SetText(Resource.String.protocol_title);
            protocolTV = FindViewById<TextView>(Resource.Id.protocol_center_content);

            InitClickableSpan();
            InitButtonBar(rootview);
        }
        private void InitButtonBar(LinearLayout rootView)
        {
            confirmButton = rootView.FindViewById<Button>(Resource.Id.base_okBtn);
            confirmButton.Click += ConfirmButton_Click;

            cancelButton = rootView.FindViewById<Button>(Resource.Id.base_cancelBtn);
            cancelButton.Click += CancelButton_Click;
        }
        private void InitClickableSpan()
        {
            // Add a text-based tapping event.
            protocolTV.MovementMethod = ScrollingMovementMethod.Instance;
            string privacyInfoText = context.GetString(Resource.String.protocol_content_text);
            SpannableStringBuilder spanPrivacyInfoText = new SpannableStringBuilder(privacyInfoText);

            //Set the listener on the event for tapping some text.
            ClickableSpan adsAndPrivacyTouchHere = new AdsAndPrivacyTouchHere(this);

            ClickableSpan personalizedAdsTouchHere = new PersonalizedAdsTouchHere(this);

            ForegroundColorSpan colorPrivacy = new ForegroundColorSpan(Color.ParseColor("#0000FF"));
            ForegroundColorSpan colorPersonalize = new ForegroundColorSpan(Color.ParseColor("#0000FF"));
            int privacyTouchHereStart = context.Resources.GetInteger(Resource.Integer.privacy_start);
            int privacyTouchHereEnd = context.Resources.GetInteger(Resource.Integer.privacy_end);
            int personalizedTouchHereStart = context.Resources.GetInteger(Resource.Integer.personalized_start);
            int personalizedTouchHereEnd = context.Resources.GetInteger(Resource.Integer.personalized_end);

            spanPrivacyInfoText.SetSpan(adsAndPrivacyTouchHere, privacyTouchHereStart, privacyTouchHereEnd, SpanTypes.ExclusiveExclusive);
            spanPrivacyInfoText.SetSpan(colorPrivacy, privacyTouchHereStart, privacyTouchHereEnd, SpanTypes.ExclusiveExclusive);
            spanPrivacyInfoText.SetSpan(personalizedAdsTouchHere, personalizedTouchHereStart, personalizedTouchHereEnd, SpanTypes.ExclusiveExclusive);
            spanPrivacyInfoText.SetSpan(colorPersonalize, personalizedTouchHereStart, personalizedTouchHereEnd, SpanTypes.ExclusiveExclusive);

            protocolTV.TextFormatted = spanPrivacyInfoText;
            protocolTV.MovementMethod = LinkMovementMethod.Instance;
        }
        private void CancelButton_Click(object sender, EventArgs e)
        {
            Dismiss();
            if (dialogCallback != null)
            {
                dialogCallback.Cancel();
            }
        }
        private void ConfirmButton_Click(object sender, EventArgs e)
        {
            ISharedPreferences sharedPreferences = context.GetSharedPreferences(AdsConstant.SpName, FileCreationMode.Private);
            ISharedPreferencesEditor editor = sharedPreferences.Edit();
            editor.PutInt(AdsConstant.SpProtocolKey, 1).Commit();
            Dismiss();

            if (dialogCallback != null)
            {
                dialogCallback.Agree();
            }
        }
        private void StartActivity(string action)
        {
            Intent enterNative = new Intent(action);
            PackageManager pkgMng = context.PackageManager;
            if (pkgMng != null)
            {
                IList<ResolveInfo> list = pkgMng.QueryIntentActivities(enterNative, PackageInfoFlags.Activities);
                if (list.Count > 0)
                {
                    enterNative.SetPackage("com.huawei.hwid");
                    context.StartActivity(enterNative);
                }
                else
                {
                    // A message is displayed, indicating that no function is available and asking users to install HMS Core of the latest version.
                    AddAlertView();
                }
            }
        }
        private void AddAlertView()
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(context);
            builder.SetTitle(context.GetString(Resource.String.alert_title));
            builder.SetMessage(context.GetString(Resource.String.alert_content));
            builder.SetPositiveButton(context.GetString(Resource.String.alert_confirm), this);
            builder.Show();
        }

        public void OnClick(IDialogInterface dialog, int which)
        {

        }
        public class AdsAndPrivacyTouchHere : ClickableSpan
        {
            public string ACTION_SIMPLE_PRIVACY = "com.huawei.hms.ppskit.ACTION.SIMPLE_PRIVACY";
            private ProtocolDialog protocolDialog;
            public AdsAndPrivacyTouchHere(ProtocolDialog mprotocolDialog)
            {
                protocolDialog = mprotocolDialog;
            }
            public override void OnClick(View widget)
            {

                protocolDialog.StartActivity(ACTION_SIMPLE_PRIVACY);
            }
        }

        public class PersonalizedAdsTouchHere : ClickableSpan
        {
            private ProtocolDialog protocolDialog;
            public string ACTION_OAID_SETTING = "com.huawei.hms.action.OAID_SETTING";

            public PersonalizedAdsTouchHere(ProtocolDialog protocol)
            {
                protocolDialog = protocol;
            }
            public override void OnClick(View widget)
            {
                protocolDialog.StartActivity(ACTION_OAID_SETTING);
            }
        }

    }
}