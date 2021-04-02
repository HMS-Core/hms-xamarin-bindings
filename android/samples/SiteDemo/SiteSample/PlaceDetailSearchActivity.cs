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
using System.Text;
using Android.Net;
using GoogleGson;
using System.Linq;

namespace XamarinHmsSiteDemo
{
    [Activity(Label = "PlaceDetailSearchActivity")]
    public class PlaceDetailSearchActivity : AppCompatActivity
    {
        private static readonly string TAG = "PlaceDetailSearchActivity";

        private string ApiKey;

        private static TextView ResultTextView;

        // Declare a SearchService object.
        private ISearchService SearchService;

        private EditText SiteIdInput;

        private EditText LanguageInput;

        private Button PlaceDetailsButton;

        private Switch Childern;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_place_detail_search);

            ApiKey = Intent.Extras.GetString("apiKey");
            SearchService = SearchServiceFactory.Create(this, Uri.Encode(ApiKey));

            ResultTextView = FindViewById<TextView>(Resource.Id.textview_place_detail_results);
            SiteIdInput = FindViewById<EditText>(Resource.Id.edittext_site_id);
            LanguageInput = FindViewById<EditText>(Resource.Id.edittext_language_pds);
            PlaceDetailsButton = FindViewById<Button>(Resource.Id.btn_get_place_details);

            Childern = FindViewById<Switch>(Resource.Id.switch_detail_search_children);
            PlaceDetailsButton.Click += delegate { ExecPlaceDetailSearch(); };
        }

        private void ExecPlaceDetailSearch()
        {
            DetailSearchRequest DetailSearchRequest = new DetailSearchRequest();

            if (SiteIdInput.Text.Length == 0)
            {
                ResultTextView.Text = "Error : Site ID is empty!";
                return;
            }
            DetailSearchRequest.SiteId = SiteIdInput.Text;

            if (!string.IsNullOrEmpty(LanguageInput.Text))
            {
                DetailSearchRequest.Language = LanguageInput.Text;
            }

            DetailSearchRequest.Children = Childern.Checked;

            DetailSearchResultListener detailSearchResultListener = new DetailSearchResultListener();

            // Call the search API.
            SearchService.DetailSearch(DetailSearchRequest, detailSearchResultListener);

        }

        override protected void OnDestroy()
        {
            base.OnDestroy();
            SearchService = null;
        }
        private class DetailSearchResultListener : Java.Lang.Object, ISearchResultListener
        {
            public void OnSearchError(SearchStatus searchStatus)
            {
                ResultTextView.Text = "Error : " + searchStatus.ErrorCode + " " + searchStatus.ErrorMessage;
            }

            public void OnSearchResult(Java.Lang.Object resultObject)
            {
                DetailSearchResponse detailSearchResponse = (DetailSearchResponse)resultObject;
                StringBuilder resultText = new StringBuilder();
                Site site = detailSearchResponse.Site;

                if (site ==null)
                {
                    ResultTextView.Text = "Result is Empty!";
                    return;
                }

                System.Text.StringBuilder ResultText = new System.Text.StringBuilder();
                ResultText.AppendLine("Success");
                AddressDetail addressDetail = site.Address;
                Coordinate location = site.Location;
                Poi poi = site.Poi;
                CoordinateBounds viewport = site.Viewport;
                    
                    string item = "siteId: '{0}', name: {1}, formatAddress: {2}, country: {3}, countryCode: {4}, location: {5}, poiTypes: {6}, viewport: {7} ";
                    ResultText.Append(string.Format(item,
                            site.SiteId, site.Name, site.FormatAddress,
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

                ResultTextView.Text = ResultText.ToString();
                Log.Debug(KeywordSearchActivity.TAG, "OnDetailSearchResult: " + ResultText.ToString());
            }
        }
    }
}