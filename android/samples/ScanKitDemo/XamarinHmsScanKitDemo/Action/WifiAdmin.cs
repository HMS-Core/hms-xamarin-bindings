/**
 * Copyright 2020. Huawei Technologies Co., Ltd. All rights reserved.
 *
 *    Licensed under the Apache License, Version 2.0 (the "License");
 *    you may not use this file except in compliance with the License.
 *    You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 *    Unless required by applicable law or agreed to in writing, software
 *    distributed under the License is distributed on an "AS IS" BASIS,
 *    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *    See the License for the specific language governing permissions and
 *    limitations under the License.
 */

using Android.Content;
using Android.Net.Wifi;
using Huawei.Hms.Ml.Scan;
using Java.Lang;
using System.Collections.Generic;
using System.Linq;

namespace XamarinHmsScanKitDemo.Action
{
    class WifiAdmin
    {
        private WifiManager mWifiManager;

        public WifiAdmin(Context context)
        {
            mWifiManager = (WifiManager)context.GetSystemService(Context.WifiService);
        }

        public bool OpenWifi()
        {
            bool bRet = true;
            if (!mWifiManager.IsWifiEnabled)
            {
                bRet = mWifiManager.SetWifiEnabled(true);
            }
            return bRet;
        }

        public bool Connect(string ssid, string password, int type)
        {
            if (!this.OpenWifi())
            {
                return false;
            }
            while (mWifiManager.WifiState == Android.Net.WifiState.Enabled)
            {
                try
                {
                    Thread.CurrentThread();
                    Thread.Sleep(100);
                }
                catch (InterruptedException ie)
                {
                }
            }

            int netID = this.CreateWifiInfo(ssid, password, type);
            bool bRet = mWifiManager.EnableNetwork(netID, true);
            mWifiManager.SaveConfiguration();
            return bRet;
        }

        public int CreateWifiInfo(string ssid, string password, int type)
        {
            WifiConfiguration config = new WifiConfiguration();
            config.AllowedAuthAlgorithms.Clear();
            config.AllowedGroupCiphers.Clear();
            config.AllowedKeyManagement.Clear();
            config.AllowedPairwiseCiphers.Clear();
            config.AllowedProtocols.Clear();
            config.Ssid = "\"" + ssid + "\"";

            WifiConfiguration tempConfig = this.IsExist(ssid);
            if (tempConfig != null)
            {
                if (!mWifiManager.RemoveNetwork(tempConfig.NetworkId))
                {
                    return tempConfig.NetworkId;
                }
            }

            if (type == HmsScan.WiFiConnectionInfo.NoPasswordModeType) //WIFICIPHER_NOPASS
            {
                config.WepKeys[0] = "";
                config.AllowedKeyManagement.Set((int)KeyManagementType.None);
                config.WepTxKeyIndex = 0;
            }
            if (type == HmsScan.WiFiConnectionInfo.WepModeType) //WIFICIPHER_WEP
            {
                config.HiddenSSID = true;
                config.WepKeys[0] = "\"" + password + "\"";
                config.AllowedAuthAlgorithms.Set((int)AuthAlgorithmType.Shared);
                config.AllowedGroupCiphers.Set((int)GroupCipherType.Ccmp);
                config.AllowedGroupCiphers.Set((int)GroupCipherType.Tkip);
                config.AllowedGroupCiphers.Set((int)GroupCipherType.Wep40);
                config.AllowedGroupCiphers.Set((int)GroupCipherType.Wep104);
                config.AllowedKeyManagement.Set((int)KeyManagementType.None);
                config.WepTxKeyIndex = 0;
            }
            if (type == HmsScan.WiFiConnectionInfo.WpaModeType) //WIFICIPHER_WPA
            {
                config.PreSharedKey = "\"" + password + "\"";
                config.HiddenSSID = true;
                config.AllowedAuthAlgorithms.Set((int)AuthAlgorithmType.Open);
                config.AllowedGroupCiphers.Set((int)GroupCipherType.Tkip);
                config.AllowedKeyManagement.Set((int)KeyManagementType.WpaPsk);
                config.AllowedPairwiseCiphers.Set((int)PairwiseCipherType.Tkip);
                config.AllowedGroupCiphers.Set((int)GroupCipherType.Ccmp);
                config.AllowedPairwiseCiphers.Set((int)PairwiseCipherType.Ccmp);
                config.StatusField = WifiStatus.Enabled;
            }
            return mWifiManager.AddNetwork(config);
        }

        private WifiConfiguration IsExist(string SSID)
        {
            List<WifiConfiguration> existingConfigs = mWifiManager.ConfiguredNetworks.ToList();
            foreach (WifiConfiguration existingConfig in existingConfigs)
            {
                if (existingConfig.Ssid.Equals("\"" + SSID + "\""))
                {
                    return existingConfig;
                }
            }
            return null;
        }
    }
}