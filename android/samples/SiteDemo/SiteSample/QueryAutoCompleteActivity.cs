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
using Android.Support.V7.App;
using Android.Util;
using Android.Widget;
using Huawei.Hms.Site.Api;
using Huawei.Hms.Site.Api.Model;
using GoogleGson;
using Java.Lang;
using System;
using System.Collections.Generic;
using System.Linq;
using Uri = Android.Net.Uri;

namespace XamarinHmsSiteDemo
{
    [Activity(Label = "QueryAutoCompleteActivity")]
    public class QueryAutoCompleteActivity : AppCompatActivity
    {
        public static string TAG = "AutoCompleteActivity";

        // Declare a SearchService object.
        private ISearchService SearchService;

        private string ApiKey;

        private EditText QueryInput;

        private EditText LatInput;

        private EditText LngInput;

        private EditText RadiusInput;

        private EditText LanguageInput;

        private Switch Childern;

        private Button AutoCompleteSearch;

        static public TextView ResultTextView;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_auto_complete);
            // Instantiate the SearchService object.
            ApiKey = Intent.Extras.GetString("apiKey");
            SearchService = SearchServiceFactory.Create(this, Uri.Encode(ApiKey));

            QueryInput = FindViewById<EditText>(Resource.Id.query_suggestion_query_input);
            LatInput = FindViewById<EditText>(Resource.Id.query_suggestion_location_lat_input);
            LngInput = FindViewById<EditText>(Resource.Id.query_suggestion_location_lng_input);
            RadiusInput = FindViewById<EditText>(Resource.Id.query_suggestion_radius_input);
            LanguageInput = FindViewById<EditText>(Resource.Id.query_suggestion_language_input);
            Childern = FindViewById<Switch>(Resource.Id.switch_query_auto_complete_children);
            AutoCompleteSearch = FindViewById<Button>(Resource.Id.search_query_auto_complete_button);
            ResultTextView = FindViewById<TextView>(Resource.Id.response_text_search);

            AutoCompleteSearch.Click += delegate { QueryAutoComplete(); };

        }

        private void QueryAutoComplete()
        {
            // Create a AutoCompleteRequest body.
            QueryAutocompleteRequest AutoCompleteRequest = new QueryAutocompleteRequest();

            string query = QueryInput.Text;
            if (query.Length == 0)
            {
                ResultTextView.Text = "Error : Query is empty!";
                return;
            }
            AutoCompleteRequest.Query = query;

            double lat;
            double lng;
            string latStr = LatInput.Text;
            string lngStr = LngInput.Text;
            if (!string.IsNullOrEmpty(latStr) || !string.IsNullOrEmpty(lngStr))
            {
                if (!(double.TryParse(latStr, out lat)) || !(double.TryParse(lngStr, out lng)))
                {
                    ResultTextView.Text = "Error : Location is invalid!";
                    return;
                }
                AutoCompleteRequest.Location = new Coordinate(lat, lng);
            }

            string radius = RadiusInput.Text;
            int radiusInt;
            if (!(Int32.TryParse(radius, out radiusInt)) || radiusInt <= 0)
            {
                ResultTextView.Text = "Error : Radius Must be greater than 0 !";
                return;
            }
            AutoCompleteRequest.Radius = (Integer)radiusInt;
           

            string language = LanguageInput.Text;
            if (!string.IsNullOrEmpty(language))
            {
                AutoCompleteRequest.Language = language;
            }
            AutoCompleteRequest.Children = Childern.Checked;

            // Call the search API.
            SearchService.QueryAutocomplete(AutoCompleteRequest, new AutoCompleteResultListener());
        }

        override protected void OnDestroy()
        {
            base.OnDestroy();
            SearchService = null;
        }

        public class AutoCompleteResultListener : Java.Lang.Object, ISearchResultListener
        {
            public void OnSearchError(SearchStatus status)
            {
                ResultTextView.Text = "Error : " + status.ErrorCode + " " + status.ErrorMessage;
            }

            public void OnSearchResult(Java.Lang.Object results)
            {

                QueryAutocompleteResponse ResultWrapper = (QueryAutocompleteResponse)results;

                if (ResultWrapper == null)
                {
                    ResultTextView.Text = "Result is Empty!";
                    return;
                }

                System.Text.StringBuilder ResultText = new System.Text.StringBuilder();
                IList<Site> SiteList;
                IList<AutoCompletePrediction> predictions;

                predictions = ResultWrapper.Predictions;
                if (predictions != null && predictions.Count > 0)
                {
                    ResultText.Append("AutoCompletePrediction[ ]:\n");
                    int PredictionCount = 1;
                    foreach (AutoCompletePrediction mPrediction in predictions)
                    {
                        ResultText.Append(string.Format("[{0}] Prediction, description = {1} ,", "" + (PredictionCount++), mPrediction.Description));

                        Word[] matchedKeywords = mPrediction.GetMatchedKeywords();
                        foreach (Word matchedKeyword in matchedKeywords)
                        {
                            ResultText.Append("matchedKeywords: " + matchedKeyword.ToString());
                        }

                        Word[] matchedWords = mPrediction.GetMatchedWords();
                        foreach (Word matchedWord in matchedWords)
                        {
                            ResultText.Append(",matchedWords: " + matchedWord.ToString());
                        }

                        ResultText.Append("\n");
                    }
                }
                else
                {
                    ResultText.Append("Predictions 0 results");
                }

                ResultText.Append("\n\nSite[ ]:\n");
                SiteList = ResultWrapper.Sites;
                if (SiteList != null && SiteList.Count > 0)
                {
                    int SiteCount = 1;
                    AddressDetail addressDetail;
                    Coordinate location;
                    Poi poi;
                    CoordinateBounds viewport;
                    foreach (Site site in SiteList)
                    {
                        addressDetail = site.Address;
                        location = site.Location;
                        poi = site.Poi;
                        viewport = site.Viewport;
                        string item = "[{0}] siteId: '{1}', name: {2}, formatAddress: {3}, utcOffset: {4}, country: {5}, countryCode: {6}, location: {7}, distance: {8}, poiTypes: {9}, viewport: {10}, streetNumber: {11}, postalCode: {12} , tertiaryAdminArea: {13}, ";
                        ResultText.Append(string.Format(item,
                                (SiteCount++).ToString(), site.SiteId, site.Name, site.FormatAddress, site.UtcOffset,
                                (addressDetail == null ? "" : addressDetail.Country),
                                (addressDetail == null ? "" : addressDetail.CountryCode),
                                (location == null ? "" : (location.Lat + "," + location.Lng)), site.Distance,
                                (poi == null ? "" : string.Join(",", poi.PoiTypes.ToArray())),
                                (viewport == null ? "" : "northeast{lat=" + viewport.Northeast.Lat + ", lng=" + viewport.Northeast.Lng + "},"
                                        + "southwest{lat=" + viewport.Southwest.Lat + ", lng=" + viewport.Southwest.Lng + "}"),
                                (addressDetail == null ? "" : addressDetail.StreetNumber),
                                (addressDetail == null ? "" : addressDetail.PostalCode),
                                (addressDetail == null ? "" : addressDetail.TertiaryAdminArea)));
                        if ((poi != null))
                        {
                            Gson gson = new Gson();
                            string jsonString = gson.ToJson(poi.GetChildrenNodes());
                            ResultText.Append(string.Format("childrenNode: {0} \n\n", jsonString));
                        }
                    }
                }
                else
                {
                    ResultText.Append("sites 0 results\n");
                }
                ResultTextView.Text = ResultText.ToString();
                Log.Debug(QueryAutoCompleteActivity.TAG, "OnAutoCompleteResult: " + ResultText.ToString());
            }
        }
    }
}