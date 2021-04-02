/*
       Copyright 2020. Huawei Technologies Co., Ltd. All rights reserved.

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
using Android.Content;
using Android.Text;
using Android.Util;
using Java.Util;

namespace XamarinHmsIAPDemo
{
    class DeliveryUtils
    {
    private const String PURCHASETOKEN_KEY = "purchasetokenSet";
    private const String GEMS_COUNT_KEY = "gemsCount";
    private const String DATA_NAME = "database";

    private static HashMap getNumOfGems()
        {
            HashMap map = new HashMap();

            map.Put("ProductCons1", 5);

            return map;
        }

        public static bool isDelivered(Context context, String purchasetoken)
{
            ISharedPreferences sharedPreferences = context.GetSharedPreferences(DATA_NAME, FileCreationMode.Private);
            ICollection<string> stringSet = sharedPreferences.GetStringSet(PURCHASETOKEN_KEY, null);
            if (stringSet != null && stringSet.Contains(purchasetoken))
            {
                return true;
            }
            return false;
        }

        public static bool DeliverProduct(Context context, String productId, String purchaseToken)
        {
            if (TextUtils.IsEmpty(productId) || TextUtils.IsEmpty(purchaseToken))
            {
                Log.Info("Delivery", "fail empty");
                return false;
            }
            if (!getNumOfGems().ContainsKey(productId))
            {
                Log.Info("Delivery", "fail product id not found");
                return false;
            }
            ISharedPreferences sharedPreferences = context.GetSharedPreferences(DATA_NAME, FileCreationMode.Private);
            ISharedPreferencesEditor editor = sharedPreferences.Edit();

            long count = sharedPreferences.GetLong(GEMS_COUNT_KEY, 0);
            count +=   (long) getNumOfGems().Get(productId);
            editor.PutLong(GEMS_COUNT_KEY, count);

            ICollection<String> stringSet = sharedPreferences.GetStringSet(PURCHASETOKEN_KEY, new HashSet<String>());
            stringSet.Add(purchaseToken);
            editor.PutStringSet(PURCHASETOKEN_KEY, stringSet);
            return editor.Commit();
        }

        
        public static long getCountOfGems(Context context)
{
            ISharedPreferences sharedPreferences = context.GetSharedPreferences(DATA_NAME, FileCreationMode.Private);
            long count = sharedPreferences.GetLong(GEMS_COUNT_KEY, 0);
            return count;
        }

    }
}