
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
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Huawei.Agconnect.Config;

namespace XamarinHmsSiteDemo
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity

    {
        private string ApiKey;
        private Button StartPlaceSearchActivityButton;
        private Button StartPlaceDetailSearchActivityButton;
        private Button StartPlaceSuggestionSearchActivityButton;
        private Button StartKeywordSearchActivityButton;
        private Button StartAutoCompleteSearchActivityButton;

        protected override void AttachBaseContext(Context context)
        {
            base.AttachBaseContext(context);
            AGConnectServicesConfig config = AGConnectServicesConfig.FromContext(context);
            config.OverlayWith(new HmsLazyInputStream(context));

            // Read api_key entry from config
            // and assign to apiKey property
            ApiKey = ReadApiKeyFromConfig(config);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);
            StartPlaceSearchActivityButton = FindViewById<Button>(Resource.Id.btn_start_place_search_activity);
            StartPlaceDetailSearchActivityButton = FindViewById<Button>(Resource.Id.btn_start_place_detail_search_activity);
            StartPlaceSuggestionSearchActivityButton = FindViewById<Button>(Resource.Id.btn_start_place_suggestion_search_activity);
            StartKeywordSearchActivityButton= FindViewById<Button>(Resource.Id.btn_start_keyword_search_activity);
            StartAutoCompleteSearchActivityButton = FindViewById<Button>(Resource.Id.btn_start_auto_complete_search_activity); ;

            StartPlaceSearchActivityButton.Click += delegate { HandleOnClick(Resource.Id.btn_start_place_search_activity); };
            StartPlaceDetailSearchActivityButton.Click += delegate { HandleOnClick(Resource.Id.btn_start_place_detail_search_activity); };
            StartPlaceSuggestionSearchActivityButton.Click += delegate { HandleOnClick(Resource.Id.btn_start_place_suggestion_search_activity); };
            StartAutoCompleteSearchActivityButton.Click += delegate { HandleOnClick(Resource.Id.btn_start_auto_complete_search_activity); };
            StartKeywordSearchActivityButton.Click += delegate { HandleOnClick(Resource.Id.btn_start_keyword_search_activity); };
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public void HandleOnClick(int Id)
        {
            Intent intent = CreateIntentWithApiKey(ApiKey);

            switch (Id)
            {
                case Resource.Id.btn_start_place_search_activity:
                    StartActivity(intent.SetClass(this, typeof(PlaceSearchActivity)));
                    break;

                case Resource.Id.btn_start_place_detail_search_activity:
                    StartActivity(intent.SetClass(this, typeof(PlaceDetailSearchActivity)));
                    break;

                case Resource.Id.btn_start_place_suggestion_search_activity:
                    StartActivity(intent.SetClass(this, typeof(PlaceSuggestionSearchActivity)));
                    break;
                case Resource.Id.btn_start_auto_complete_search_activity:
                    StartActivity(intent.SetClass(this, typeof(QueryAutoCompleteActivity)));
                    break;
                case Resource.Id.btn_start_keyword_search_activity:
                    StartActivity(intent.SetClass(this, typeof(KeywordSearchActivity)));
                    break;
                default:
                    break;
            }
        }

        private string ReadApiKeyFromConfig(AGConnectServicesConfig config)
        {
            string clientConfig = config.GetString("client");
            return new GoogleGson.JsonParser()
                .Parse(clientConfig)
                .AsJsonObject
                .Get("api_key")
                .AsString;
        }

        private Intent CreateIntentWithApiKey(string apiKey)
        {
            return new Intent().PutExtra("apiKey", apiKey);
        }
    }
}