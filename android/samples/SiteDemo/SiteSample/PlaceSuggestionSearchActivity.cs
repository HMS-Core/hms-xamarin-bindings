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
using Android.OS;
using Android.Support.V7.App;
using Android.Util;
using Android.Widget;
using Huawei.Hms.Site.Api;
using Huawei.Hms.Site.Api.Model;
using System.Collections.Generic;
using GoogleGson;
using System.Linq;
using System;
using Java.Lang;
using Uri = Android.Net.Uri;

namespace XamarinHmsSiteDemo
{
    [Activity(Label = "PlaceSuggestionSearchActivity")]
    public class PlaceSuggestionSearchActivity : AppCompatActivity
    {
        private static readonly string TAG = "PlaceSuggestionSearchActivity";

        private string ApiKey;

        private static TextView ResultTextView;

        // Declare a SearchService object.
        private ISearchService SearchService;

        private EditText QueryInput;

        private EditText LatitudeInput;

        private EditText LongitudeInput;

        private EditText RadiusInput;

        private EditText LanguageInput;

        private EditText CountryCodeInput;

        private EditText NortheastLatInput;

        private EditText NortheastLngInput;

        private EditText SouthwestLatInput;

        private EditText SouthwestLngInput;

        private Switch Childern;

        private Switch BoundStrict;

        private Button SearchPlaceSuggestionButton;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_place_suggestion_search);

            ApiKey = Intent.Extras.GetString("apiKey");
            SearchService = SearchServiceFactory.Create(this, Uri.Encode(ApiKey));

            ResultTextView = FindViewById<TextView>(Resource.Id.textview_pss_results);
            QueryInput = FindViewById<EditText>(Resource.Id.edittext_search_query_pss);
            LatitudeInput = FindViewById<EditText>(Resource.Id.edittext_lat_pss);
            LongitudeInput = FindViewById<EditText>(Resource.Id.edittext_lon_pss);
            RadiusInput = FindViewById<EditText>(Resource.Id.edittext_radius_pss);
            LanguageInput = FindViewById<EditText>(Resource.Id.edittext_language_pss);
            CountryCodeInput = FindViewById<EditText>(Resource.Id.edittext_country_code_pss);
            
            NortheastLatInput = FindViewById<EditText>(Resource.Id.query_suggestion_bounds_northeast_lat_input);
            NortheastLngInput = FindViewById<EditText>(Resource.Id.query_suggestion_bounds_northeast_lng_input);
            SouthwestLatInput = FindViewById<EditText>(Resource.Id.query_suggestion_bounds_southwest_lat_input);
            SouthwestLngInput = FindViewById<EditText>(Resource.Id.query_suggestion_bounds_southwest_lng_input);
            
            Childern = FindViewById<Switch>(Resource.Id.switch_query_suggestion_children);
            BoundStrict = FindViewById<Switch>(Resource.Id.switch_query_suggestion_strict_bounds);

            SearchPlaceSuggestionButton = FindViewById<Button>(Resource.Id.btn_search_place_suggestion);
            SearchPlaceSuggestionButton.Click += delegate { ExecPlaceSuggestionSearch(); };
        }
        public void ExecPlaceSuggestionSearch()
        {
            QuerySuggestionRequest QuerySuggestionRequest = new QuerySuggestionRequest();
            if (QueryInput.Text.Length == 0)
            {
                ResultTextView.Text = "Error : Query is empty!";
                return;
            }
            QuerySuggestionRequest.Query = QueryInput.Text;

            
            if (!string.IsNullOrEmpty(LanguageInput.Text))
            {
                QuerySuggestionRequest.Language = LanguageInput.Text;
            }

            if (!string.IsNullOrEmpty(CountryCodeInput.Text))
            {
                QuerySuggestionRequest.CountryCode = CountryCodeInput.Text;
            }

            double lat;
            double lon;

            if (!string.IsNullOrEmpty(LatitudeInput.Text) || !string.IsNullOrEmpty(LongitudeInput.Text))
            {
                if (!(double.TryParse(LatitudeInput.Text, out lat)) || !(double.TryParse(LongitudeInput.Text, out lon)))
                {
                    ResultTextView.Text = "Error : Location is invalid!";
                    return;
                }
                QuerySuggestionRequest.Location = new Coordinate(lat, lon);
            }

            int radiusInt;
            if (!(Int32.TryParse(RadiusInput.Text, out radiusInt)) || radiusInt <= 0)
            {
                ResultTextView.Text = "Error : Radius Must be greater than 0 !";
                return;
            }
            QuerySuggestionRequest.Radius = (Integer)radiusInt;


            if (!string.IsNullOrEmpty(NortheastLatInput.Text) && !string.IsNullOrEmpty(NortheastLngInput.Text) && !string.IsNullOrEmpty(SouthwestLatInput.Text) && !string.IsNullOrEmpty(SouthwestLngInput.Text))
            {
                Coordinate northeast = new Coordinate(double.Parse(NortheastLatInput.Text), double.Parse(NortheastLngInput.Text));
                Coordinate southwest = new Coordinate(double.Parse(SouthwestLatInput.Text), double.Parse(SouthwestLngInput.Text));
                CoordinateBounds bounds = new CoordinateBounds(northeast, southwest);
                QuerySuggestionRequest.Bounds = bounds;
            }

            QuerySuggestionRequest.Children = Childern.Checked;
            QuerySuggestionRequest.StrictBounds = (Java.Lang.Boolean)BoundStrict.Checked;

            QuerySuggestionResultListener querySuggestionResultListener = new QuerySuggestionResultListener();

            // Call the search API.
            SearchService.QuerySuggestion(QuerySuggestionRequest, querySuggestionResultListener);


        }

        override protected void OnDestroy()
        {
            base.OnDestroy();
            SearchService = null;
        }
        private class QuerySuggestionResultListener : Java.Lang.Object, ISearchResultListener
        {
            public void OnSearchError(SearchStatus searchStatus)
            {
                ResultTextView.Text = "Error : " + searchStatus.ErrorCode + " " + searchStatus.ErrorMessage;
            }

            public void OnSearchResult(Java.Lang.Object resultObject)
            {
                QuerySuggestionResponse querySuggestionResponse = (QuerySuggestionResponse)resultObject;
                IList<Site> SiteList = querySuggestionResponse.Sites;

                if (SiteList == null)
                {
                    ResultTextView.Text = "Result is Empty!";
                    return;
                }
                System.Text.StringBuilder ResultText = new System.Text.StringBuilder();
                ResultText.AppendLine("Success");
                int count = 1;
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
                    string item = "[{0}] siteId: '{1}', name: {2}, formatAddress: {3}, country: {4}, countryCode: {5}, location: {6}, Distance: {7}, poiTypes: {8}, viewport: {9} ";
                    ResultText.Append(string.Format(item,
                            (count++).ToString(), site.SiteId, site.Name, site.FormatAddress,
                            (addressDetail == null ? "" : addressDetail.Country),
                            (addressDetail == null ? "" : addressDetail.CountryCode),
                            (location == null ? "" : (location.Lat + "," + location.Lng)), site.Distance,
                            (poi == null ? "" : string.Join(",", poi.PoiTypes.ToArray())),
                            (viewport == null ? "" : "northeast{lat=" + viewport.Northeast.Lat + ", lng=" + viewport.Northeast.Lng + "},"
                                    + "southwest{lat=" + viewport.Southwest.Lat + ", lng=" + viewport.Southwest.Lng + "}\n")));
                    if ((poi != null))
                    {
                        Gson gson = new Gson();
                        string jsonString = gson.ToJson(poi.GetChildrenNodes());
                        ResultText.Append(string.Format("childrenNode: {0} \n\n", jsonString));
                    }
                }
                ResultTextView.Text = ResultText.ToString();
                Log.Debug(KeywordSearchActivity.TAG, "OnSuggestionSearchResult: " + ResultText.ToString());
            }
        }
    }
}