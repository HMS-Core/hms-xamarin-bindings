/*
*       Copyright 2020-2021. Huawei Technologies Co., Ltd. All rights reserved.

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
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using AndroidX.AppCompat.App;
using Huawei.Hms.Maps;
using Huawei.Hms.Maps.Model;
using Huawei.Hms.Maps.Model.Animation;
using static Huawei.Hms.Maps.Model.Animation.Animation;
using AlphaAnimation = Huawei.Hms.Maps.Model.Animation.AlphaAnimation;
using AnimationSet = Huawei.Hms.Maps.Model.Animation.AnimationSet;

namespace XHms_Map_Kit_Demo_Project.MapKitActivities
{
    /// <summary>
    /// Tile overlay related activity.
    /// </summary>
    [Activity(Label = "TileOverlayDemoActivity")]
    public class TileOverlayDemoActivity : AppCompatActivity, IOnMapReadyCallback
    {
        private const string TAG = "TileOverlayDemoActivity";

        private HuaweiMap hMap;

        private MapFragment mMapFragment;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Log.Debug(TAG, "OnCreate: ");
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_tileoverlay_demo);
            mMapFragment = (MapFragment)FragmentManager.FindFragmentById(Resource.Id.mapFragment);
            mMapFragment.GetMapAsync(this);
        }

        public void OnMapReady(HuaweiMap map)
        {
            Log.Debug(TAG, "OnMapReady: ");
            hMap = map;
            hMap.UiSettings.MyLocationButtonEnabled = false;
            hMap.MoveCamera(CameraUpdateFactory.NewLatLngZoom(new LatLng(48.893478, 2.334595), 10));
            TileProvider mTileProvider = new TileProvider();
            TileOverlayOptions options =
                new TileOverlayOptions().InvokeTileProvider(mTileProvider).InvokeTransparency(0.5f).InvokeFadeIn(true);


            hMap.AddTileOverlay(options);
        }
    }

    public class TileProvider : Java.Lang.Object, ITileProvider
    {
        // Set the tile size to 256 x 256.
        public const int mTileSize = 256;
        public const int mScale = 1;
        public const int mDimension = mScale * mTileSize;

        public Tile GetTile(int x, int y, int zoom)
        {
            Matrix matrix = new Matrix();
            float scale = (float)Math.Pow(2, zoom) * mScale;
            matrix.PostScale(scale, scale);
            matrix.PostTranslate(-x * mDimension, -y * mDimension);

            // Generate a bitmap image.
            Bitmap bitmap = Bitmap.CreateBitmap(mDimension, mDimension, Bitmap.Config.Rgb565);
            bitmap.EraseColor(Color.ParseColor("#024CFF"));
            MemoryStream stream = new MemoryStream();
            bitmap.Compress(Bitmap.CompressFormat.Png, 100, stream);
            return new Tile(mDimension, mDimension, stream.ToArray());
        }
    }
}