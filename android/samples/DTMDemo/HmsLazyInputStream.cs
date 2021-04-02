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
using System.IO;
using Android.Content;
using Android.Util;

using Huawei.Agconnect.Config;

namespace XamarinHmsDTMDemo
{
    public class HmsLazyInputStream : LazyInputStream
    {
        public HmsLazyInputStream(Context context)
            : base(context)
        {
        }

        public override Stream Get(Context context)
        {
            try
            {
                return context.Assets.Open("agconnect-services.json");
            }
            catch (Exception e)
            {
                Log.Error("Hms", $"Failed to get input stream" + e.Message);
                return null;
            }
        }
    }
}