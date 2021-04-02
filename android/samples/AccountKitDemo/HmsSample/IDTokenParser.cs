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
using Org.Json;
using System.Net;
using System.IO;
using Android.Util;

namespace XamarinAccountKitDemo.HmsSample
{
    public class IDTokenParser
    {

        //Log tag
        private static readonly string TAG = "IDTokenParser";

        //Expected respond from 
        //id Token verification Api
        private static readonly string VerifiedResponseFromApi = "OK";

        /// <summary>
        /// Constructor
        /// </summary>
        public IDTokenParser()
        {

        }

        /// <summary>
        ///  The method calls the Verify ID Token API to send an ID token verification request to the HUAWEI Account Server.
        ///  This method use Server verification
        ///  In a commercial environment, please use the local verification method.
        /// </summary>
        public void Verify(string idToken)
        {
            // Create a request using a URL that can receive a post.
            string postURl = Constant.TokenVerificationUrl + idToken;
            WebRequest request = WebRequest.Create(postURl);
            // If required by the server, set the credentials.
            request.Credentials = CredentialCache.DefaultCredentials;

            // Get the response.
            WebResponse response = request.GetResponse();
            // Display the status.
            Console.WriteLine(((HttpWebResponse)response).StatusDescription);

            // Get the stream containing content returned by the server.
            // The using block ensures the stream is automatically closed.
            using (Stream dataStream = response.GetResponseStream())
            {
                // Open the stream using a StreamReader for easy access.
                StreamReader reader = new StreamReader(dataStream);
                // Read the content.
                string responseFromServer = reader.ReadToEnd();
                // Display the content.
                JSONObject jsonObject = new JSONObject(responseFromServer);

                if (((HttpWebResponse)response).StatusDescription.Equals(VerifiedResponseFromApi))
                    Log.Info(TAG, "id Token Validate Success, verify signature: " + responseFromServer);
                else
                    Log.Info(TAG, "id Token Validate failed");
            }

            // Close the response.
            response.Close();

        }
    }
}