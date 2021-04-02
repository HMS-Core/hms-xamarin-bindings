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
using Huawei.Hmf.Tasks;
using System;

namespace SafetyDetectDemo.HmsSample
{

    public class OnSuccessListener : Java.Lang.Object, IOnSuccessListener
    {
        private Action<object> action;
        public OnSuccessListener(Action<object> action)
        {
            this.action = action;
        }

        public void OnSuccess(Java.Lang.Object obj)
        {
            action(obj);
        }
    }
    public class OnFailureListener : Java.Lang.Object, IOnFailureListener
    {
        private Action<object> action;
        public OnFailureListener(Action<object> action)
        {
            this.action = action;
        }
        public void OnFailure(Java.Lang.Exception e)
        {
            action(e);
        }
    }
}