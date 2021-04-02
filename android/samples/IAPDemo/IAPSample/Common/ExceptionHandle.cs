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

using Android.App;
using Android.Util;
using Android.Widget;
using Huawei.Hms.Iap;
using Huawei.Hms.Iap.Entity;


namespace XamarinHmsIAPDemo
{
    class ExceptionHandle
    {
        
        public const int SOLVED = 0;

        public static int handle(Activity activity, Java.Lang.Exception e)
        {

            if ((e.GetType()==typeof(IapApiException))) {
                IapApiException iapApiException = (IapApiException)e;
                int StatusCode= iapApiException.Status.StatusCode;
                Log.Debug("Error", "returnCode: " + StatusCode);
                switch (StatusCode)
                {
                    case OrderStatusCode.OrderStateCancel:
                        Toast.MakeText(activity, "Order has been canceled!", ToastLength.Short).Show();
                        return SOLVED;
                    case OrderStatusCode.OrderStateParamError:
                        Toast.MakeText(activity, "Order state param error!", ToastLength.Short).Show();
                        return SOLVED;
                    case OrderStatusCode.OrderStateNetError:
                        Toast.MakeText(activity, "Order state net error!", ToastLength.Short).Show();
                        return SOLVED;
                    case OrderStatusCode.OrderVrUninstallError:
                        Toast.MakeText(activity, "Order vr uninstall error!", ToastLength.Short).Show();
                        return SOLVED;
                    case OrderStatusCode.OrderProductOwned:
                        Toast.MakeText(activity, "Product already owned error!", ToastLength.Short).Show();
                        return OrderStatusCode.OrderProductOwned;
                    case OrderStatusCode.OrderProductNotOwned:
                        Toast.MakeText(activity, "Product not owned error!", ToastLength.Short).Show();
                        return SOLVED;
                    case OrderStatusCode.OrderProductConsumed:
                        Toast.MakeText(activity, "Product consumed error!", ToastLength.Short).Show();
                        return SOLVED;
                    case OrderStatusCode.OrderAccountAreaNotSupported:
                        Toast.MakeText(activity, "Order account area not supported error!", ToastLength.Short).Show();
                        return SOLVED;
                    case OrderStatusCode.OrderNotAcceptAgreement:
                        Toast.MakeText(activity, "User does not agree the agreement", ToastLength.Short).Show();
                        return SOLVED;
                    default:
                        // handle other error scenarios
                        Toast.MakeText(activity, "Order unknown error!", ToastLength.Short).Show();
                        return SOLVED;
                }
            } else
            {
                Toast.MakeText(activity, "external error", ToastLength.Short).Show();
                Log.Debug("Error", e.Message);
                return SOLVED;
            }
        }
    }
}