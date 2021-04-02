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
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Text.Method;
using Android.Text.Style;
using Android.Util;
using Android.Views;
using Android.Widget;
using Huawei.Hms.Ads.Consent.Bean;
using Huawei.Hms.Ads.Consent.Constant;
using Huawei.Hms.Ads.Consent.Inter;
using XamarinAdsLiteDemo.Utils;

namespace XamarinAdsLiteDemo.Dialogs
{
    public class ConsentDialog : Dialog, View.IOnClickListener
    {
        private Context context;

        private LayoutInflater inflater;

        private LinearLayout contentLayout;

        private TextView titleTv;

        private TextView initInfoTv;

        private TextView moreInfoTv;

        private TextView partnersListTv;

        private View consentDialogView;

        private View initView;

        private View moreInfoView;

        private View partnersListView;

        private Button consentYesBtn;

        private Button consentNoBtn;

        private Button moreInfoBackBtn;

        private Button partnerListBackBtn;

        private IList<AdProvider> adProviders;

        private IConsentDialog callback;
        public ConsentDialog(Context context, IList<AdProvider> adProviders) : base(context)
        {
            this.context = context;
            this.adProviders = adProviders;
        }


        //Consent dialog box callback interface.
        public interface IConsentDialog
        {
            /**
        * Update a user selection result.
        *
        * @param consentStatus ConsentStatus
        */
            void UpdateConsentStatus(ConsentStatus consentStatus);
        }

        /// <summary>
        /// Set a dialog box callback.
        /// </summary>
        /// <param name="callback"></param>
        public void SetCallback(ConsentDialog.IConsentDialog callback)
        {
            this.callback = callback;
        }


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Window window = Window;
            window.RequestFeature(WindowFeatures.NoTitle);

            inflater = LayoutInflater.From(context);
            consentDialogView = inflater.Inflate(Resource.Layout.dialog_consent, null);
            SetContentView(consentDialogView);

            titleTv = FindViewById<TextView>(Resource.Id.consent_dialog_title_text);
            titleTv.Text = context.GetString(Resource.String.consent_title);

            initView = inflater.Inflate(Resource.Layout.dialog_consent_content, null);
            moreInfoView = inflater.Inflate(Resource.Layout.dialog_consent_moreinfo, null);
            partnersListView = inflater.Inflate(Resource.Layout.dialog_consent_partner_list, null);

            //Add content to the initialization dialog box.
            ShowInitConsentInfo();
        }


        /// <summary>
        /// Display initial consent content
        /// </summary>
        private void ShowInitConsentInfo()
        {
            AddContentView(initView);

            // Add a button and a text link their tapping events to the initial page.
            AddInitButtonAndLinkClick(consentDialogView);
        }



        /// <summary>
        /// Add a button and a text link and their tapping events to the initial page.    
        /// </summary>
        /// <param name="consentDialogView"></param>
        private void AddInitButtonAndLinkClick(View consentDialogView)
        {
            consentYesBtn = consentDialogView.FindViewById<Button>(Resource.Id.btn_consent_init_yes);
            consentNoBtn = consentDialogView.FindViewById<Button>(Resource.Id.btn_consent_init_skip);

            consentNoBtn.SetOnClickListener(this);
            consentYesBtn.SetOnClickListener(this);



            initInfoTv = consentDialogView.FindViewById<TextView>(Resource.Id.consent_center_init_content);
            initInfoTv.MovementMethod = ScrollingMovementMethod.Instance;

            string initText = context.GetString(Resource.String.consent_init_text);

            SpannableStringBuilder spanInitText = new SpannableStringBuilder(initText);

            // Set the listener on the event for tapping some text.
            ClickableSpan initTouchHere = new InitTouchHere(this);

            ForegroundColorSpan colorSpan = new ForegroundColorSpan(Color.ParseColor("#0000FF"));
            int initTouchHereStart = context.Resources.GetInteger(Resource.Integer.init_here_start);
            int initTouchHereEnd = context.Resources.GetInteger(Resource.Integer.init_here_end);

            spanInitText.SetSpan(initTouchHere, initTouchHereStart, initTouchHereEnd, SpanTypes.ExclusiveExclusive);
            spanInitText.SetSpan(colorSpan, initTouchHereStart, initTouchHereEnd, SpanTypes.ExclusiveExclusive);
            initInfoTv.TextFormatted = spanInitText;
            initInfoTv.MovementMethod = LinkMovementMethod.Instance;
        }



        /// <summary>
        /// Update the consent status.
        /// </summary>
        /// <param name="consentStatus"></param>
        private void UpdateConsentStatus(ConsentStatus consentStatus)
        {
            Consent.GetInstance(context).SetConsentStatus(consentStatus);

            //Save a user selection to the local SP.
            ISharedPreferences sharedPreferences = context.GetSharedPreferences(AdsConstant.SpName, FileCreationMode.Private);
            ISharedPreferencesEditor editor = sharedPreferences.Edit();
            editor.PutInt(AdsConstant.SpConsentKey, consentStatus.Value).Commit();

            //Callback activity
            if (callback != null)
            {
                callback.UpdateConsentStatus(consentStatus);
            }
        }


        private void ShowTouchHereInfo()
        {
            AddContentView(moreInfoView);

            //Set the listener on the tapping event on the more information page.
            AddMoreInfoButtonAndLinkClick(consentDialogView);
        }
        /// <summary>
        /// Add a button and a text link and their tapping events to the more information page.
        /// </summary>
        /// <param name="consentDialogView"></param>
        private void AddMoreInfoButtonAndLinkClick(View consentDialogView)
        {
            moreInfoBackBtn = consentDialogView.FindViewById<Button>(Resource.Id.btn_consent_more_info_back);
            moreInfoTv = consentDialogView.FindViewById<TextView>(Resource.Id.consent_center_more_info_content);
            moreInfoTv.MovementMethod = ScrollingMovementMethod.Instance;

            moreInfoBackBtn.SetOnClickListener(this);

            string moreInfoText = context.Resources.GetString(Resource.String.consent_more_info_text);
            SpannableStringBuilder spanMoreInfoText = new SpannableStringBuilder(moreInfoText);

            // Set the listener on the event for tapping some text.
            ClickableSpan moreInfoToucHere = new MoreInfoTouchHere(this);

            ForegroundColorSpan foregroundColorSpan = new ForegroundColorSpan(Color.ParseColor("#0000FF"));
            int moreInfoTouchHereStart = context.Resources.GetInteger(Resource.Integer.more_info_here_start);
            int moreInfoTouchHereEnd = context.Resources.GetInteger(Resource.Integer.more_info_here_end);
            spanMoreInfoText.SetSpan(moreInfoToucHere, moreInfoTouchHereStart, moreInfoTouchHereEnd, SpanTypes.ExclusiveExclusive);
            spanMoreInfoText.SetSpan(foregroundColorSpan, moreInfoTouchHereStart, moreInfoTouchHereEnd, SpanTypes.ExclusiveExclusive);

            moreInfoTv.TextFormatted = spanMoreInfoText;
            moreInfoTv.MovementMethod = LinkMovementMethod.Instance;


        }

        [Obsolete]
        private void ShowPartnersListInfo()
        {
            partnersListTv = partnersListView.FindViewById<TextView>(Resource.Id.partners_list_content);
            partnersListTv.MovementMethod = ScrollingMovementMethod.Instance;
            partnersListTv.Text = "";
            IList<AdProvider> learnAdProviders = adProviders;
            if (learnAdProviders != null)
            {
                foreach (var learnAdProvider in learnAdProviders)
                {
                    string link = "<font color='#0000FF'><a href=" + learnAdProvider.PrivacyPolicyUrl + ">"
                    + learnAdProvider.Name + "</a>";

                    partnersListTv.Append(Html.FromHtml(link));
                    partnersListTv.Append("  ");
                }
            }
            else
            {
                partnersListTv.Append(" 3rd party’s full list of advertisers is empty");
            }
            partnersListTv.MovementMethod = LinkMovementMethod.Instance;
            AddContentView(partnersListView);

            // Set the listener on the tapping event on the partner list page.
            AddPartnersListButtonLinkClick(consentDialogView);
        }
        /// <summary>
        /// Add a button and a text link and their tapping events to the partner list page.
        /// </summary>
        /// <param name="consentDialogView"></param>
        private void AddPartnersListButtonLinkClick(View consentDialogView)
        {
            partnerListBackBtn = consentDialogView.FindViewById<Button>(Resource.Id.btn_partners_list_back);

            partnerListBackBtn.SetOnClickListener(this);
        }


        public void OnClick(View v)
        {
            switch (v.Id)
            {
                case Resource.Id.btn_partners_list_back:
                    ShowTouchHereInfo();
                    break;
                case Resource.Id.btn_consent_more_info_back:
                    ShowInitConsentInfo();
                    break;
                case Resource.Id.btn_consent_init_yes:
                    Dismiss();
                    UpdateConsentStatus(ConsentStatus.Personalized);
                    break;
                case Resource.Id.btn_consent_init_skip:
                    Dismiss();
                    UpdateConsentStatus(ConsentStatus.NonPersonalized);
                    break;
                default:
                    Log.Error("Error", "Somethings gone wrong");
                    break;
            }
        }
        private void AddContentView(View view)
        {
            contentLayout = FindViewById<LinearLayout>(Resource.Id.consent_center_layout);
            contentLayout.RemoveAllViews();
            contentLayout.AddView(view);
        }
        public class InitTouchHere : ClickableSpan
        {
            private ConsentDialog mConsentdialog;
            public InitTouchHere(ConsentDialog consentdialog)
            {
                mConsentdialog = consentdialog;
            }
            public override void OnClick(View widget)
            {
                mConsentdialog.ShowTouchHereInfo();

            }
        }

        public class MoreInfoTouchHere : ClickableSpan
        {
            private ConsentDialog mConsentdialog;
            public MoreInfoTouchHere(ConsentDialog consentdialog)
            {
                mConsentdialog = consentdialog;
            }

            [Obsolete]
            public override void OnClick(View widget)
            {
                mConsentdialog.ShowPartnersListInfo();

            }
        }
    }
}