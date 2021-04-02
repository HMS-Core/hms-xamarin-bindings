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

namespace XamarinHmsNearbyDemo
{
   public  class MessageBean : Java.Lang.Object
    {
        public  const int TYPE_SEND_TEXT = 0;
        public const int TYPE_RECEIVE_TEXT = 1;

        public string Msg { get; set; }
        public int Type { get; set; }
        public string MyName { get; set; }
        public string FriendName { get; set; }
        public string FileName { get; set; }

    }
}