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

namespace XamarinHmsWalletDemo.Utils
{
    public class Base64Hw
    {
        private static int BaseLength = 128;
        private static int LookupLength = 64;
        private static int TwentyFourBitGroup = 24;
        private static int EightBit = 8;
        private static int SixteenBit = 16;
        private static int FourByte = 4;
        private static int Sign = -128;
        private static char Pad = '=';
        private static sbyte[] Base64Alphabet = new sbyte[BaseLength];
        private static char[] LookUpBase64Alphabet = new char[LookupLength];
        private static bool IsNotD3OrD4(char d3, char d4)
        {
            return !IsData((d3)) || !IsData((d4));
        }

        private static bool IsNotPadD3AndPadD4(char d3, char d4)
        {
            return !IsPad(d3) && IsPad(d4);
        }

        private static bool IsPadD3AndD4(char d3, char d4)
        {
            return IsPad(d3) && IsPad(d4);
        }
        static Base64Hw()
        {
            for (int idx = 0; idx < BaseLength; idx++)
            {
                    Base64Alphabet[idx] = (sbyte)(-1);
            }
            for (int idx = 'Z'; idx >= 'A'; idx--)
            {
                Base64Alphabet[idx] = (sbyte)(idx - 'A');
            }
            for (int idx = 'z'; idx >= 'a'; idx--)
            {
                Base64Alphabet[idx] = (sbyte)(idx - 'a' + 26);
            }

            for (int idx = '9'; idx >= '0'; idx--)
            {
                Base64Alphabet[idx] = (sbyte)(idx - '0' + 52);
            }

            Base64Alphabet['+'] = 62;
            Base64Alphabet['/'] = 63;

            for (int i = 0; i <= 25; i++)
            {
                LookUpBase64Alphabet[i] = (char)('A' + i);
            }

            for (int i = 26, j = 0; i <= 51; i++, j++)
            {
                LookUpBase64Alphabet[i] = (char)('a' + j);
            }

            for (int i = 52, j = 0; i <= 61; i++, j++)
            {
                LookUpBase64Alphabet[i] = (char)('0' + j);
            }
            LookUpBase64Alphabet[62] = '+';
            LookUpBase64Alphabet[63] = '/';
        }

        private static bool IsWhiteSpace(char octect)
        {
            return (octect == 0x20 || octect == 0xd || octect == 0xa || octect == 0x9);
        }

        private static bool IsPad(char octect) { return (octect == Pad); }

        private static bool IsData(char octect)
        {
#pragma warning disable CS0652 // Comparison to integral constant is useless; the constant is outside the range of the type
            return octect < BaseLength && Base64Alphabet[octect] != -1;
#pragma warning restore CS0652 // Comparison to integral constant is useless; the constant is outside the range of the type
        }
        /// <summary>
        /// Encodes hex octects into Base64Hw
        /// </summary>
        /// <param name="binaryData">Array containing binaryData</param>
        /// <returns>Encoded Base64Hw array</returns>
        public static string Encode(sbyte[] binaryData)
        {
            if (CommonUtil.IsNull(binaryData))
            {
                return null;
            }

            int lengthDataBits = binaryData.Length * EightBit;
            if(lengthDataBits == 0)
            {
                return "";
            }

            int fewerThan24bits = lengthDataBits % TwentyFourBitGroup;
            int numberTriplets = lengthDataBits / TwentyFourBitGroup;
            int numberQuartet = fewerThan24bits != 0 ? numberTriplets + 1 : numberTriplets;
            char[] encodedData = new char[numberQuartet * 4];

            sbyte k1 = 0;
            sbyte l1 = 0;
            sbyte b1 = 0;
            sbyte b2 = 0;
            sbyte b3 = 0;

            int encodedIndex = 0;
            int dataIndex = 0;
            sbyte val1 = 0;
            sbyte val2 = 0;
            sbyte val3 = 0;
            for (int i = 0; i < numberTriplets; i++)
            {
                b1 = binaryData[dataIndex++];
                b2 = binaryData[dataIndex++];
                b3 = binaryData[dataIndex++];

                l1 = (sbyte)(b2 & 0x0f);
                k1 = (sbyte)(b1 & 0x03);

                val1 = ((b1 & Sign) == 0) ? (sbyte)(b1 >> 2) : (sbyte)((b1) >> 2 ^ 0xc0);
                val2 = ((b2 & Sign) == 0) ? (sbyte)(b2 >> 4) : (sbyte)((b2) >> 4 ^ 0xf0);
                val3 = ((b3 & Sign) == 0) ? (sbyte)(b3 >> 6) : (sbyte)((b3) >> 6 ^ 0xfc);

                encodedData[encodedIndex++] = LookUpBase64Alphabet[val1];
                encodedData[encodedIndex++] = LookUpBase64Alphabet[val2 | (k1 << 4)];
                encodedData[encodedIndex++] = LookUpBase64Alphabet[(l1 << 2) | val3];
                encodedData[encodedIndex++] = LookUpBase64Alphabet[b3 & 0x3f];
            }

            // form integral number of 6-bit groups
            AssembleInteger(binaryData, fewerThan24bits, encodedData, encodedIndex, dataIndex);

            return new string(encodedData);
        }

        private static void AssembleInteger(sbyte[] binaryData,int fewerThan24bits,char[] encodedData,int encodedIndex,int dataIndex)
        {
            sbyte b1;
            sbyte k1;
            sbyte b2;
            sbyte l1;
            sbyte val4 = 0;
            sbyte val5 = 0;
            if (fewerThan24bits == EightBit)
            {
                b1 = binaryData[dataIndex];
                k1 = (sbyte)(b1 & 0x03);

                val4 = ((b1 & Sign) == 0) ? (sbyte)(b1 >> 2) : (sbyte)((b1) >> 2 ^ 0xc0);
                encodedData[encodedIndex++] = LookUpBase64Alphabet[val4];
                encodedData[encodedIndex++] = LookUpBase64Alphabet[k1 << 4];
                encodedData[encodedIndex++] = Pad;
                encodedData[encodedIndex++] = Pad;
            }
            else if (fewerThan24bits == SixteenBit)
            {
                b1 = binaryData[dataIndex];
                b2 = binaryData[dataIndex + 1];
                l1 = (sbyte)(b2 & 0x0f);
                k1 = (sbyte)(b1 & 0x03);

                val4 = ((b1 & Sign) == 0) ? (sbyte)(b1 >> 2) : (sbyte)((b1) >> 2 ^ 0xc0);
                val5 = ((b2 & Sign) == 0) ? (sbyte)(b2 >> 4) : (sbyte)((b2) >> 4 ^ 0xf0);

                encodedData[encodedIndex++] = LookUpBase64Alphabet[val4];
                encodedData[encodedIndex++] = LookUpBase64Alphabet[val5 | (k1 << 4)];
                encodedData[encodedIndex++] = LookUpBase64Alphabet[l1 << 2];
                encodedData[encodedIndex++] = Pad;
            }
        }
        /// <summary>
        /// Decodes Base64Hw data into octects
        /// </summary>
        /// <param name="encoded">string containing Base64Hw data</param>
        /// <returns>Array containind decoded data.</returns>
        public static byte[] Decode(string encoded)
        {
            if (CommonUtil.IsNull(encoded))
            {
                return new byte[0];
            }
            char[] base64Data = encoded.ToCharArray();
            // remove white spaces
            int len = RemoveWhiteSpace(base64Data);

            if (len % FourByte != 0)
            {
                return new byte[0];// should be divisible by four
            }

            int numberQuadruple = (len / FourByte);

            if (numberQuadruple == 0)
            {
                return new byte[0];
            }

            return (byte[])(Array)Decode(base64Data, numberQuadruple);
        }

        private static sbyte[] Decode(char[] base64Data, int numberQuadruple)
        {
            sbyte b1 = 0;
            sbyte b2 = 0;
            sbyte b3 = 0;
            sbyte b4 = 0;
            char d1 = '0';
            char d2 = '0';
            char d3 = '0';
            char d4 = '0';

            int idx = 0;
            int encodedIndex = 0;
            int dataIndex = 0;
            sbyte[] decodedData = new sbyte[(numberQuadruple) * 3];

            for (; idx < numberQuadruple - 1; idx++)
            {
                if (!IsData((d1 = base64Data[dataIndex++])) || !IsData((d2 = base64Data[dataIndex++]))
                    || !IsData((d3 = base64Data[dataIndex++])) || !IsData((d4 = base64Data[dataIndex++])))
                {
                    return new sbyte[0];
                } // if found "no data" just return null

                b1 = Base64Alphabet[d1];
                b2 = Base64Alphabet[d2];
                b3 = Base64Alphabet[d3];
                b4 = Base64Alphabet[d4];

                decodedData[encodedIndex++] = (sbyte)(b1 << 2 | b2 >> 4);
                decodedData[encodedIndex++] = (sbyte)(((b2 & 0xf) << 4) | ((b3 >> 2) & 0xf));
                decodedData[encodedIndex++] = (sbyte)(b3 << 6 | b4);
            }

            if (!IsData((d1 = base64Data[dataIndex++])) || !IsData((d2 = base64Data[dataIndex++])))
            {
                return new sbyte[0];// if found "no data" just return null
            }

            sbyte[] tmp = CheckCharacters(base64Data, d1, d2, idx, encodedIndex, dataIndex, decodedData);
            if (tmp != null)
            {
                return tmp;
            }
            return decodedData;
        }

        private static sbyte[] CheckCharacters(char[] base64Data, char d1, char d2, int idx, int encodedIndex, int dataIndex, sbyte[] decodedData)
        {
            sbyte b1;
            sbyte b2;
            char d3;
            char d4;
            sbyte b3;
            sbyte b4;
            b1 = Base64Alphabet[d1];
            b2 = Base64Alphabet[d2];

            d3 = base64Data[dataIndex++];
            d4 = base64Data[dataIndex++];
            if (IsNotD3OrD4(d3, d4))
            { // Check if they are PAD characters
                if (IsPadD3AndD4(d3, d4))
                {
                    if ((b2 & 0xf) != 0)
                    {
                        // last 4 bits should be zero
                        return new sbyte[0];
                    }
                    sbyte[] tmp = new sbyte[idx * 3 + 1];
                    Array.Copy(decodedData, 0, tmp, 0, idx * 3);
                    //Java.Lang.JavaSystem.Arraycopy((byte[])(Array)decodedData, 0, (byte[])(Array)tmp, 0, idx * 3);
                    tmp[encodedIndex] = (sbyte)(b1 << 2 | b2 >> 4);
                    return tmp;
                }
                else if (IsNotPadD3AndPadD4(d3, d4))
                {
                    b3 = Base64Alphabet[d3];
                    if ((b3 & 0x3) != 0)
                    {
                        // last 2 bits should be zero
                        return new sbyte[0];
                    }
                    sbyte[] tmp = new sbyte[idx * 3 + 2];
                    Array.Copy(decodedData, 0, tmp, 0, idx * 3);
                    //Java.Lang.JavaSystem.Arraycopy((byte[])(Array)decodedData, 0, (byte[])(Array)tmp, 0, idx * 3);
                    tmp[encodedIndex++] = (sbyte)(b1 << 2 | b2 >> 4);
                    tmp[encodedIndex] = (sbyte)(((b2 & 0xf) << 4) | ((b3 >> 2) & 0xf));
                    return tmp;
                }
                else
                {
                    return new sbyte[0];
                }
            }
            else
            { // No PAD e.g 3cQl
                b3 = Base64Alphabet[d3];
                b4 = Base64Alphabet[d4];
                decodedData[encodedIndex++] = (sbyte)(b1 << 2 | b2 >> 4);
                decodedData[encodedIndex++] = (sbyte)(((b2 & 0xf) << 4) | ((b3 >> 2) & 0xf));
                decodedData[encodedIndex++] = (sbyte)(b3 << 6 | b4);
            }
            return null;
        }
        /// <summary>
        /// remove WhiteSpace from MIME containing encoded Base64Hw data.
        /// </summary>
        /// <param name="base64Data">the byte array of base64 data (with WS)</param>
        /// <returns>the new length</returns>
        private static int RemoveWhiteSpace(char[] base64Data)
        {
            if (CommonUtil.IsNull(base64Data))
            {
                return 0;
            }
            // count characters that's not whitespace
            int newSize = 0;
            int len = base64Data.Length;
            for (int i = 0; i < len; i++)
            {
                if (!IsWhiteSpace(base64Data[i]))
                {
                    base64Data[newSize++] = base64Data[i];
                }
            }
            return newSize;
        }
    }
}