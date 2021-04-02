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
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace XamarinAREngineDemo.Common
{
    /// <summary>
    /// This class is used to display information on the screen. Before using this function,
    /// you need to set listening, where the information is processed. In this sample, a method
    /// for displaying information on the screen is created in the UI thread.
    /// </summary>
    public class TextDisplay
    {
        private OnTextInfoChangeListener mTextInfoListener;

        /// <summary>
        /// Display the string information. This method is called in each frame
        /// when Android.Opengl.GLSurfaceView.IRenderer's OnDrawFrame.
        /// </summary>
        /// <param name="sb">String builder.</param>
        public void OnDrawFrame(StringBuilder sb)
        {
            if (sb == null)
            {
                ShowTextInfo();
                return;
            }
            ShowTextInfo(sb.ToString());
        }

        /// <summary>
        /// Set the listener to display information in the UI thread. 
        /// This method is called when Android.Opengl.GLSurfaceView.IRenderer's OnSurfaceCreated.
        /// </summary>
        /// <param name="listener">OnTextInfoChangeListener.</param>
        public void SetListener(OnTextInfoChangeListener listener)
        {
            mTextInfoListener = listener;
        }

        /// <summary>
        /// Listen to the text change and execute corresponding methods.
        /// </summary>
        public interface OnTextInfoChangeListener
        {
            /// <summary>
            /// Display the given text.
            /// </summary>
            /// <param name="text">Text to be displayed.</param>
            /// <param name="positionX">X-coordinates of points</param>
            /// <param name="positionY">Y-coordinates of points</param>
            void TextInfoChanged(string text, float positionX, float positionY);
        }

        private void ShowTextInfo(string text)
        {
            if (mTextInfoListener != null)
            {
                mTextInfoListener.TextInfoChanged(text, 0, 0);
            }
        }

        private void ShowTextInfo()
        {
            if (mTextInfoListener != null)
            {
                mTextInfoListener.TextInfoChanged(null, 0, 0);
            }
        }

    }
}