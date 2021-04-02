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

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Huawei.Hms.Mlsdk.Text;
using Java.Util.Regex;

namespace HmsXamarinMLDemo.MLKitActivities.TextRelated.GeneralCard
{
    /// <summary>
    /// Post-processing plug-in for Exit-Entry Permit for Travelling to and from Hong Kong and Macao.
    /// </summary>
    public class PassCardProcess
    {
        private const string Tag = "PassCardProcess";

        private readonly MLText text;
        public PassCardProcess(MLText text)
        {
            this.text = text;
        }

        // Re-encapsulate the return result of OCR in Block.
        class BlockItem
        {
            public readonly string text;
            public readonly Rect rect;

            public BlockItem(string text, Rect rect)
                {
                    this.text = text;
                    this.rect = rect;
                }
        }

        public UniversalCardResult GetResult()
        {
            IList<MLText.Block> blocks = this.text.Blocks;
            if (blocks.Count == 0)
            {
                Log.Info(PassCardProcess.Tag, "PassCardProcess::getResult blocks is empty");
                return null;
            }
            IList<BlockItem> originItems = this.GetOriginItems(blocks);
            string valid = "";
            string number = "";
            bool validFlag = false;
            bool numberFlag = false;

            foreach (BlockItem item in originItems)
            {
                string tempStr = item.text;
                if (!validFlag)
                {
                    string result = this.TryGetValidDate(tempStr);
                    if (!string.IsNullOrEmpty(result))
                    {
                        valid = result;
                        validFlag = true;
                    }
                }
                if (!numberFlag)
                {
                    string result = this.TryGetCardNumber(tempStr);
                    if (!string.IsNullOrEmpty(result))
                    {
                        number = result;
                        numberFlag = true;
                    }
                }
            }
            return new UniversalCardResult(valid, number);
        }

        private string TryGetValidDate(string originStr)
        {
            return originStr;
        }

        private string TryGetCardNumber(string originStr)
        {
            return originStr;
        }

        /// <summary>
        /// Transform the detected text into BlockItem structure.
        /// </summary>
        /// <param name="blocks"></param>
        /// <returns></returns>
        private IList<BlockItem> GetOriginItems(IList<MLText.Block> blocks)
        {
            IList<BlockItem> originItems = new List<BlockItem>();

            foreach (MLText.Block block in blocks)
            {
                IList<MLText.Base> lines = block.Contents;
                foreach (MLText.TextLine line in lines)
                {
                    string text = line.StringValue;
                    text = FilterString(text, "[^a-zA-Z0-9\\.\\-,<\\(\\)\\s]");
                    Log.Info("PassCardProcess", "PassCardProcess text: " + text);
                    Point[] points = line.GetVertexes();
                    Rect rect = new Rect(points[0].X, points[0].Y, points[2].X, points[2].Y);
                    BlockItem item = new BlockItem(text, rect);
                    originItems.Add(item);
                }
            }
            return originItems;
        }

        /// <summary>
        /// Filter strings based on regular expressions.
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="filterStr"></param>
        /// <returns></returns>
        public static string FilterString(string origin, string filterStr)
        {
            if (string.IsNullOrEmpty(origin))
            {
                return "";
            }
            if (string.IsNullOrEmpty(filterStr))
            {
                return origin;
            }

            Java.Util.Regex.Pattern pattern = Java.Util.Regex.Pattern.Compile(filterStr);
            Matcher matcher = pattern.Matcher(origin);
            return matcher.ReplaceAll("").Trim();
        }
    }
}