/*
        Copyright 2021. Huawei Technologies Co., Ltd. All rights reserved.

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

namespace XHms_Drive_Kit_Demo_Project.DriveKit.Common
{
    public class OnFailureListener : Java.Lang.Object, Huawei.Hmf.Tasks.IOnFailureListener
    {
        //Action when task is failure
        private Action failureAction;
        //Action with parameter when task is failure
        private Action<Java.Lang.Exception> failureActionWithParam;

        public OnFailureListener(Action FailureAction)
        {
            this.failureAction = FailureAction;
        }

        public OnFailureListener(Action<Java.Lang.Exception> FailureActionWithParam)
        {
            this.failureActionWithParam = FailureActionWithParam;
        }

        public void OnFailure(Java.Lang.Exception exception)
        {
            if (failureAction != null)
                this.failureAction();
            else
            {
                this.failureActionWithParam(exception);
            }
        }
    }

}