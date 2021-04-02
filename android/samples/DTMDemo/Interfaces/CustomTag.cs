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
using System.Collections.Generic;
using Android.Util;
using Huawei.Hms.Dtm;

namespace XamarinHmsDTMDemo.Interfaces
{
    public class CustomTag : Java.Lang.Object, ICustomTag
    {
        public void Call(IDictionary<string, Java.Lang.Object> obj)
        {
            Log.Debug("ICustomTag.Call", obj.Values.ToString());
        }
    }
}