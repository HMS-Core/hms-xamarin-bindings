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
using System.Linq;
using Java.Lang;
using System;
using Uri = Android.Net.Uri;
using StringBuilder = System.Text.StringBuilder;

namespace XamarinHmsSiteDemo
{
    [Activity(Label = "PlaceSearchActivity")]
    public partial class PlaceSearchActivity : AppCompatActivity
    {
        private static readonly string TAG = "PlaceSearchActivity";

        private string ApiKey;

        private static TextView ResultTextView;

        // Declare a SearchService object.
        private ISearchService SearchService;

        private EditText QueryInput;

        private EditText LatitudeInput;

        private EditText LongitudeInput;

        private EditText RadiusInput;

        private EditText LanguageInput;

        private EditText PageIndexInput;

        private EditText PageSizeInput;

        private Spinner PoiTypeSpinner;

        private Switch BoundStrict;

        private Button SearchNearbyPlaceButton;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_place_search);

            ApiKey = Intent.Extras.GetString("apiKey");
            SearchService = SearchServiceFactory.Create(this, Uri.Encode(ApiKey));

            ResultTextView = FindViewById<TextView>(Resource.Id.textview_text_search_results);
            QueryInput = FindViewById<EditText>(Resource.Id.edittext_text_search_query);
            LatitudeInput = FindViewById<EditText>(Resource.Id.edittext_lat);
            LongitudeInput = FindViewById<EditText>(Resource.Id.edittext_lon);
            RadiusInput = FindViewById<EditText>(Resource.Id.edittext_radius);
            LanguageInput = FindViewById<EditText>(Resource.Id.edittext_language);
            PageIndexInput = FindViewById<EditText>(Resource.Id.edittext_page_index);
            PageSizeInput = FindViewById<EditText>(Resource.Id.edittext_page_size);
            PoiTypeSpinner = FindViewById<Spinner>(Resource.Id.spinner_nearby_search_poitype);
            PoiTypeSpinner.Adapter = new ArrayAdapter<LocationType>(this, Android.Resource.Layout.SimpleSpinnerItem, LocationType.Values().ToList());
            BoundStrict = FindViewById<Switch>(Resource.Id.switch_nearby_search_strict_bounds);
            Switch UsePOITypeSwitch = FindViewById<Switch>(Resource.Id.switch_nearby_search_poitype);

            UsePOITypeSwitch.CheckedChange += delegate (object sender, CompoundButton.CheckedChangeEventArgs e)
            {

                PoiTypeSpinner.Enabled = e.IsChecked;
            };

            SearchNearbyPlaceButton = FindViewById<Button>(Resource.Id.btn_search_nearby_place);
            SearchNearbyPlaceButton.Click += delegate { ExecPlaceSearch(); };

            PoiTypeSpinner.Enabled = false;
        }

        private void ExecPlaceSearch()
        {
            NearbySearchRequest NearbySearchRequest = new NearbySearchRequest();

            string queryText = QueryInput.Text;
            if (queryText.Length == 0)
            {
                ResultTextView.Text = "Error : Query is empty!";
                return;
            }
            NearbySearchRequest.Query = queryText;

            string language = LanguageInput.Text;
            if (!string.IsNullOrEmpty(LanguageInput.Text))
            {
                NearbySearchRequest.Language = language;
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
                NearbySearchRequest.Location = new Coordinate(lat, lon);
            }

            int radiusInt;
            if (!(Int32.TryParse(RadiusInput.Text, out radiusInt)) || radiusInt <= 0)
            {
                ResultTextView.Text = "Error : Radius Must be greater than 0 !";
                return;
            }
            NearbySearchRequest.Radius = (Integer)radiusInt;


            string pageIndex = PageIndexInput.Text;
            int pageIndexInt;
            if (!(Int32.TryParse(pageIndex, out pageIndexInt)) || pageIndexInt < 1 || pageIndexInt > 60)
            {
                ResultTextView.Text = "Error : PageIndex Must be between 1 and 60!";
                return;
            }
            NearbySearchRequest.PageIndex = (Integer)pageIndexInt;

            string pageSize = PageSizeInput.Text;
            int pageSizeInt;
            if (!(Int32.TryParse(pageSize, out pageSizeInt)) || pageSizeInt < 1 || pageSizeInt > 20)
            {
                ResultTextView.Text = "Error : PageSize Must be between 1 and 20!";
                return;
            }
            NearbySearchRequest.PageSize = (Integer)pageSizeInt;

            LocationType poiType = PoiTypeSpinner.Enabled ? (LocationType)PoiTypeSpinner.SelectedItem : null;
            if (poiType != null)
            {
                NearbySearchRequest.PoiType = poiType;
            }

            NearbySearchRequest.StrictBounds = (Java.Lang.Boolean)BoundStrict.Checked;

            NearbySearchResultListener nearbySearchResultListener = new NearbySearchResultListener();

            // Call the search API.
            SearchService.NearbySearch(NearbySearchRequest, nearbySearchResultListener);

        }

        override protected void OnDestroy()
        {
            base.OnDestroy();
            SearchService = null;
        }

        private static void LogOnSearchError(SearchStatus searchStatus)
        {
            ResultTextView.Text = "Error : " + searchStatus.ErrorCode + " " + searchStatus.ErrorMessage;
        }

        private static void AddSitesToResultTextView(IList<Site> SiteList)
        {
            if (SiteList == null)
            {
                ResultTextView.Text = "Result is Empty!";
                return;
            }
            StringBuilder ResultText = new StringBuilder();
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
                                + "southwest{lat=" + viewport.Southwest.Lat + ", lng=" + viewport.Southwest.Lng + "}\n")));
                
            }
            ResultTextView.Text = ResultText.ToString();
            Log.Debug(KeywordSearchActivity.TAG, "NearbySearchResult: " + ResultText.ToString());
        }

        private class NearbySearchResultListener : Java.Lang.Object, ISearchResultListener
        {
            public void OnSearchError(SearchStatus status)
            {
                LogOnSearchError(status);
            }

            public void OnSearchResult(Java.Lang.Object resultObject)
            {
                NearbySearchResponse nearbySearchResponse = (NearbySearchResponse)resultObject;
                AddSitesToResultTextView(nearbySearchResponse.Sites);
            }
        }
    }
}