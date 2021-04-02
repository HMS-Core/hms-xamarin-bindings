/*
*       Copyright 2020-2021 Huawei Technologies Co., Ltd. All rights reserved.

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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Huawei.Hiar;

namespace XamarinAREngineDemo.AREngineActivities.Body3d.Rendering
{
    /// <summary>
    /// Rendering body AR type related data.
    /// </summary>
    interface BodyRelatedDisplay
    {
        /// <summary>
        /// Init render.
        /// </summary>
        void Init();

        /// <summary>
        /// Render objects, call per frame.
        /// </summary>
        /// <param name="bodies">ARBodies.</param>
        /// <param name="projectionMatrix">Camera projection matrix.</param>
        void OnDrawFrame(ICollection bodies, float[] projectionMatrix);
    }
}