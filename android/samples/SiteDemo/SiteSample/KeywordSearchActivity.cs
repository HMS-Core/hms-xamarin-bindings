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
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using Huawei.Hms.Site.Api;
using Uri = Android.Net.Uri;
using Huawei.Hms.Site.Api.Model;
using Android.Util;
using Java.Lang;
using GoogleGson;
using Android.Support.V7.App;

namespace XamarinHmsSiteDemo
{
    [Activity(Label = "KeywordSearchActivity")]
    public class KeywordSearchActivity : AppCompatActivity
    {
        public static string TAG = "KeywordSearchActivity";

        private string ApiKey;

        private EditText QueryInput;

        private EditText LatInput;

        private EditText LngInput;

        private EditText RadiusInput;

        private Spinner PoiTypeSpinner;

        private EditText CountryInput;

        private EditText LanguageInput;

        private EditText PageIndexInput;

        private EditText PageSizeInput;

        static public TextView ResultTextView;

        private Switch Childern;

        private Button Search;

        // Declare a SearchService object.
        private ISearchService SearchService;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_keyword);
            // Instantiate the SearchService object.
            ApiKey = Intent.Extras.GetString("apiKey");
            SearchService = SearchServiceFactory.Create(this, Uri.Encode(ApiKey));

            QueryInput = FindViewById<EditText>(Resource.Id.edit_text_text_search_query);
            LatInput = FindViewById<EditText>(Resource.Id.edit_text_text_search_location_lat);
            LngInput = FindViewById<EditText>(Resource.Id.edit_text_text_search_location_lng);
            RadiusInput = FindViewById<EditText>(Resource.Id.edit_text_text_search_radius);
            PoiTypeSpinner = FindViewById<Spinner>(Resource.Id.spinner_text_search_poitype);
            PoiTypeSpinner.Adapter = new ArrayAdapter<LocationType>(this, Android.Resource.Layout.SimpleSpinnerItem, LocationType.Values().ToList());

            Switch UsePOITypeSwitch = FindViewById<Switch>(Resource.Id.switch_text_search_poitype);

            UsePOITypeSwitch.CheckedChange += delegate (object sender, CompoundButton.CheckedChangeEventArgs e)
            {

                PoiTypeSpinner.Enabled = e.IsChecked;
            };
            CountryInput = FindViewById<EditText>(Resource.Id.edit_text_text_search_country);
            LanguageInput = FindViewById<EditText>(Resource.Id.edit_text_text_search_language);
            PageIndexInput = FindViewById<EditText>(Resource.Id.edit_text_text_search_pageindex);
            PageSizeInput = FindViewById<EditText>(Resource.Id.edit_text_text_search_pagesize);
            ResultTextView = FindViewById<TextView>(Resource.Id.response_text_search);

            Childern = FindViewById<Switch>(Resource.Id.switch_text_search_children);

            Search = FindViewById<Button>(Resource.Id.button_text_search);

            Search.Click += delegate { ExecTextSearch(); };

            PoiTypeSpinner.Enabled = false;
        }
        private void ExecTextSearch()
        {
            Log.Debug(TAG, "ExecTextSearch: " + this);
            Log.Debug(TAG, "ExecTextSearch: " + (Context)this);
            string query = QueryInput.Text;
            if (query.Length == 0)
            {
                ResultTextView.Text = "Error : Query is empty!";
                return;
            }
            // Create a KeywordSearchRequest body.
            TextSearchRequest KeywordSearchRequest = new TextSearchRequest();
            KeywordSearchRequest.Query = query;

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
                KeywordSearchRequest.Location = new Coordinate(lat, lng);
            }

            string radius = RadiusInput.Text;
            int radiusInt;
            if (!(Int32.TryParse(radius, out radiusInt)) || radiusInt <= 0)
            {
                ResultTextView.Text = "Error : Radius Must be greater than 0 !";
                return;
            }
            KeywordSearchRequest.Radius = (Integer)radiusInt;

            LocationType poiType = PoiTypeSpinner.Enabled ? (LocationType)PoiTypeSpinner.SelectedItem : null;
            if (poiType != null)
            {
                KeywordSearchRequest.PoiType = poiType;
            }

            string countryCode = CountryInput.Text;
            if (!string.IsNullOrEmpty(countryCode))
            {
                KeywordSearchRequest.CountryCode = countryCode;
            }

            string language = LanguageInput.Text;
            if (!string.IsNullOrEmpty(language))
            {
                KeywordSearchRequest.Language = language;
            }

            string pageIndex = PageIndexInput.Text;
            int pageIndexInt;
            if (!(Int32.TryParse(pageIndex, out pageIndexInt)) || pageIndexInt < 1 || pageIndexInt > 60)
            {
                ResultTextView.Text = "Error : PageIndex Must be between 1 and 60!";
                return;
            }
            KeywordSearchRequest.PageIndex = (Integer)pageIndexInt;

            string pageSize = PageSizeInput.Text;
            int pageSizeInt;
            if (!(Int32.TryParse(pageSize, out pageSizeInt)) || pageSizeInt < 1 || pageSizeInt > 20)
            {
                ResultTextView.Text = "Error : PageSize Must be between 1 and 20!";
                return;
            }
            KeywordSearchRequest.PageSize = (Integer)pageSizeInt;
            KeywordSearchRequest.Children = Childern.Checked;

            // Call the search API.
            SearchService.TextSearch(KeywordSearchRequest, new ResultListener());
        }
        override protected void OnDestroy()
        {
            base.OnDestroy();
            SearchService = null;
        }

        public class ResultListener : Java.Lang.Object, ISearchResultListener
        {
            public void OnSearchError(SearchStatus status)
            {
                ResultTextView.Text = "Error : " + status.ErrorCode + " " + status.ErrorMessage;
            }

            public void OnSearchResult(Java.Lang.Object results)
            {

                TextSearchResponse ResultWrapper = (TextSearchResponse)results;
                IList<Site> SiteList;

                if (ResultWrapper == null || ResultWrapper.TotalCount <= 0 || (SiteList = ResultWrapper.Sites) == null
                        || SiteList.Count <= 0)
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
                    string item = "[{0}] siteId: '{1}', name: {2}, formatAddress: {3}, country: {4}, countryCode: {5}, location: {6}, poiTypes: {7}, viewport: {8} ";
                    ResultText.Append(string.Format(item,
                            (count++).ToString(), site.SiteId, site.Name, site.FormatAddress,
                            (addressDetail == null ? "" : addressDetail.Country),
                            (addressDetail == null ? "" : addressDetail.CountryCode),
                            (location == null ? "" : (location.Lat + "," + location.Lng)),
                            (poi == null ? "" : string.Join(",", poi.PoiTypes.ToArray())),
                            (viewport == null ? "" : "northeast{lat=" + viewport.Northeast.Lat + ", lng=" + viewport.Northeast.Lng + "},"
                                    + "southwest{lat=" + viewport.Southwest.Lat + ", lng=" + viewport.Southwest.Lng + "}")));
                    if ((poi != null))
                    {
                        Gson gson = new Gson();
                        string jsonString = gson.ToJson(poi.GetChildrenNodes());
                        ResultText.Append(string.Format("childrenNode: {0} \n\n", jsonString));
                    }
                }
                ResultTextView.Text = ResultText.ToString();
                Log.Debug(KeywordSearchActivity.TAG, "OnTextSearchResult: " + ResultText.ToString());
            }
        }
    }
}