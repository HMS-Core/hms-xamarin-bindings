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
using System.Threading;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Huawei.Hms.Maps.Model;
using Java.IO;
using Java.Lang;
using Org.Json;
using Square.OkHttp3;

namespace XHms_Map_Kit_Demo_Project.Utils
{
    /// <summary>
    /// NetworkRequestManager
    /// </summary>
    public class NetworkRequestManager
    {
        private const string TAG = "NetworkRequestManager";

        // Maximum number of retries.
        private static readonly int MAX_TIMES = 10;

        /// <summary>
        /// GetWalkingRoutePlanningResult
        /// </summary>
        /// <param name="latLng1">origin latitude and longitude</param>
        /// <param name="latLng2">destination latitude and longitude</param>
        /// <param name="listener">network listener</param>
        public static void GetWalkingRoutePlanningResult(LatLng latLng1, LatLng latLng2,
     IOnNetworkListener listener)
        {
            GetWalkingRoutePlanningResult(latLng1, latLng2, listener, 0, false);
        }

        /// <summary>
        /// GetWalkingRoutePlanningResult
        /// </summary>
        /// <param name="latLng1">origin latitude and longitude</param>
        /// <param name="latLng2">destination latitude and longitude</param>
        /// <param name="listener">network listener</param>
        /// <param name="count">last number of retries</param>
        /// <param name="needEncode">dose the api key need to be encoded</param>
        private static void GetWalkingRoutePlanningResult(LatLng latLng1, LatLng latLng2,
 IOnNetworkListener listener, int count, bool needEncode)
        {
            int curCount = ++count;
            Log.Info(TAG, "current count: " + curCount);
            //creating thread
            System.Threading.Thread t = new System.Threading.Thread(() =>
            {
                try
                {
                    Response response =
                    NetClient.GetNetClient().GetBicyclingRoutePlanningResult(latLng1, latLng2, needEncode);
                    if (null != response && null != response.Body() && response.IsSuccessful)
                    {
                        try
                        {
                            string result = response.Body().String();
                            if (null != listener)
                            {
                                listener.RequestSuccess(result);
                            }
                            return;
                        }
                        catch (IOException e)
                        {
                            Log.Error(TAG, e.Message);
                        }
                    }

                    string returnCode = "";
                    string returnDesc = "";
                    bool need = needEncode;

                    try
                    {
                        string result = response.Body().String();
                        JSONObject jsonObject = new JSONObject(result);
                        returnCode = jsonObject.OptString("returnCode");
                        returnDesc = jsonObject.OptString("returnDesc");
                    }
                    catch (NullPointerException e)
                    {
                        returnDesc = "Request Fail!";
                    }
                    catch (IOException e)
                    {
                        returnDesc = "Request Fail!";
                    }
                    catch (JSONException e)
                    {
                        Log.Error(TAG, e.Message);
                    }
                    if (curCount >= MAX_TIMES)
                    {
                        if (null != listener)
                        {
                            listener.RequestFail(returnDesc);
                        }
                        return;
                    }

                    if (returnCode.Equals("6"))
                    {
                        need = true;
                    }
                    GetWalkingRoutePlanningResult(latLng1, latLng2, listener, curCount, need);
                }
                finally
                {
                    //finished executing.
                }
            });
            //t.IsBackground = true;
            t.Start();

        }

        /// <summary>
        /// GetBicyclingRoutePlanningResult
        /// </summary>
        /// <param name="latLng1">origin latitude and longitude</param>
        /// <param name="latLng2">destination latitude and longitude</param>
        /// <param name="listener">network listener</param>
        public static void GetBicyclingRoutePlanningResult(LatLng latLng1, LatLng latLng2,
    IOnNetworkListener listener)
        {
            GetBicyclingRoutePlanningResult(latLng1, latLng2, listener, 0, false);
        }

        /// <summary>
        /// GetBicyclingRoutePlanningResult
        /// </summary>
        /// <param name="latLng1">origin latitude and longitude</param>
        /// <param name="latLng2">destination latitude and longitude</param>
        /// <param name="listener">network listener</param>
        /// <param name="count">last number of retries</param>
        /// <param name="needEncode">dose the api key need to be encoded</param>
        private static void GetBicyclingRoutePlanningResult( LatLng latLng1,  LatLng latLng2,
         IOnNetworkListener listener, int count,  bool needEncode)
        {
            int curCount = ++count;
            Log.Info(TAG, "current count: " + curCount);
            //creating thread
            System.Threading.Thread t = new System.Threading.Thread(() =>
            {               
                try
                {
                    Response response =
                    NetClient.GetNetClient().GetBicyclingRoutePlanningResult(latLng1, latLng2, needEncode);
                    if (null != response && null != response.Body() && response.IsSuccessful)
                    {
                        try
                        {
                            string result = response.Body().String();
                            if (null != listener)
                            {
                                listener.RequestSuccess(result);
                            }
                            return;
                        }
                        catch (IOException e)
                        {
                            Log.Error(TAG, e.Message);
                        }
                    }

                    string returnCode = "";
                    string returnDesc = "";
                    bool need = needEncode;

                    try
                    {
                        string result = response.Body().String();
                        JSONObject jsonObject = new JSONObject(result);
                        returnCode = jsonObject.OptString("returnCode");
                        returnDesc = jsonObject.OptString("returnDesc");
                    }
                    catch (NullPointerException e)
                    {
                        returnDesc = "Request Fail!";
                    }
                    catch (IOException e)
                    {
                        returnDesc = "Request Fail!";
                    }
                    catch (JSONException e)
                    {
                        Log.Error(TAG, e.Message);
                    }
                    if (curCount >= MAX_TIMES)
                    {
                        if (null != listener)
                        {
                            listener.RequestFail(returnDesc);
                        }
                        return;
                    }

                    if (returnCode.Equals("6"))
                    {
                        need = true;
                    }
                    GetBicyclingRoutePlanningResult(latLng1, latLng2, listener, curCount, need);
                }
                finally
                {
                    //finished executing.
                }
            });
            //t.IsBackground = true;
            t.Start();

        }

        /// <summary>
        /// GetDrivingRoutePlanningResult
        /// </summary>
        /// <param name="latLng1">origin latitude and longitude</param>
        /// <param name="latLng2">destination latitude and longitude</param>
        /// <param name="listener">network listener</param>
        public static void GetDrivingRoutePlanningResult(LatLng latLng1, LatLng latLng2,
    IOnNetworkListener listener)
        {
            GetDrivingRoutePlanningResult(latLng1, latLng2, listener, 0, false);
        }

        /// <summary>
        /// GetDrivingRoutePlanningResult
        /// </summary>
        /// <param name="latLng1">origin latitude and longitude</param>
        /// <param name="latLng2">destination latitude and longitude</param>
        /// <param name="listener">network listener</param>
        /// <param name="count">last number of retries</param>
        /// <param name="needEncode">dose the api key need to be encoded</param>
        private static void GetDrivingRoutePlanningResult(LatLng latLng1, LatLng latLng2,
    IOnNetworkListener listener, int count, bool needEncode)
        {
            int curCount = ++count;
            Log.Info(TAG, "current count: " + curCount);
            //creating thread
            System.Threading.Thread t = new System.Threading.Thread(() =>
            {
                try
                {
                    Response response =
                    NetClient.GetNetClient().GetDrivingRoutePlanningResult(latLng1, latLng2, needEncode);
                    if (null != response && null != response.Body() && response.IsSuccessful)
                    {
                        try
                        {
                            string result = response.Body().String();
                            if (null != listener)
                            {
                                listener.RequestSuccess(result);
                            }
                            return;
                        }
                        catch (IOException e)
                        {
                            Log.Error(TAG, e.Message);
                        }
                    }

                    string returnCode = "";
                    string returnDesc = "";
                    bool need = needEncode;

                    try
                    {
                        string result = response.Body().String();
                        JSONObject jsonObject = new JSONObject(result);
                        returnCode = jsonObject.OptString("returnCode");
                        returnDesc = jsonObject.OptString("returnDesc");
                    }
                    catch (NullPointerException e)
                    {
                        returnDesc = "Request Fail!";
                    }
                    catch (IOException e)
                    {
                        returnDesc = "Request Fail!";
                    }
                    catch (JSONException e)
                    {
                        Log.Error(TAG, e.Message);
                    }
                    if (curCount >= MAX_TIMES)
                    {
                        if (null != listener)
                        {
                            listener.RequestFail(returnDesc);
                        }
                        return;
                    }

                    if (returnCode.Equals("6"))
                    {
                        need = true;
                    }
                    GetDrivingRoutePlanningResult(latLng1, latLng2, listener, curCount, need);
                }
                finally
                {
                    //finished executing.
                }
            });
            //t.IsBackground = true;
            t.Start();
        }

    public interface IOnNetworkListener
        {
            void RequestSuccess(string result);

            void RequestFail(string errorMsg);
        }
    }
}