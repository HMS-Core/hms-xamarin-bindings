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
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Huawei.Hms.Maps.Model;

namespace XHms_Map_Kit_Demo_Project.Utils
{
    public class MapUtils
    {
        public static readonly LatLng HUAWEI_CENTER = new LatLng(31.98559, 118.76613);

        public static readonly LatLng APARTMENT_CENTER = new LatLng(31.97480, 118.75682);

        public static readonly LatLng EPARK_CENTER = new LatLng(31.97846, 118.76454);

        public static readonly LatLng FRANCE = new LatLng(47.893478, 2.334595);

        public static readonly LatLng FRANCE1 = new LatLng(48.993478, 3.434595);

        public static readonly LatLng FRANCE2 = new LatLng(48.693478, 2.134595);

        public static readonly LatLng FRANCE3 = new LatLng(48.793478, 2.334595);

        public static IList<LatLng> CreateRectangle(LatLng center, double halfWidth, double halfHeight)
        {
            return new List<LatLng>(new LatLng[] { new LatLng(center.Latitude - halfHeight, center.Longitude - halfWidth),
                    new LatLng(center.Latitude - halfHeight, center.Longitude + halfWidth),
                    new LatLng(center.Latitude + halfHeight, center.Longitude + halfWidth),
                    new LatLng(center.Latitude + halfHeight, center.Longitude - halfWidth),
                    new LatLng(center.Latitude - halfHeight, center.Longitude - halfWidth)});
        }

        public static readonly int MAP_TYPE_NONE = 0;

        public static readonly int MAP_TYPE_NORMAL = 1;

        public static readonly float MAX_ZOOM_LEVEL = 20;

        public static readonly float MIN_ZOOM_LEVEL = 3;
    }
}