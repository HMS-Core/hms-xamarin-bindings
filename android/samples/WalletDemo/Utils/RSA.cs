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
using System.Security.Cryptography;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Security;
using Java.Security.Spec;
using Javax.Crypto;

namespace XamarinHmsWalletDemo.Utils
{
    public class RSA
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

        /*
         Signature algorithm.
         */
        private static string SignAlgorithms256 = "SHA256WithRSA";
        private static string RsaEcbOaepSha256Algorithm = "RSA/ECB/OAEPwithSHA-256andMGF1Padding";

        /// <summary>
        /// Sign content.
        /// </summary>
        /// <param name="content">data to be signed.</param>
        /// <param name="privateKey">merchant's private key.</param>
        /// <returns> Signed value.</returns>
        public static string SignContent(string content,string privateKey)
        {
            try
            {
                PKCS8EncodedKeySpec privatePKCS8 = new PKCS8EncodedKeySpec(Base64.Decode(privateKey, Base64Flags.Default));
                KeyFactory keyf = KeyFactory.GetInstance("RSA");
                IPrivateKey priKey = keyf.GeneratePrivate(privatePKCS8);
                Java.Security.Signature signatureObj = Java.Security.Signature.GetInstance(SignAlgorithms256);
                signatureObj.InitSign(priKey);
                signatureObj.Update(Encoding.UTF8.GetBytes(content));
                sbyte[] signed = (sbyte[])(Array)signatureObj.Sign();
                return Base64.EncodeToString((byte[])(Array)signed, Base64Flags.Default);
            }
            catch (Exception ex)
            {
                Log.Info("RSA", "Sign fail: " + ex.Message);
                throw;
            } 
        }
        /// <summary>
        /// Verify signature.
        /// </summary>
        /// <param name="content">content to be signed.</param>
        /// <param name="sign">the signature to verified.</param>
        /// <param name="publicKey">public key.</param>
        /// <returns>if verifying signature succeed.</returns>
        public static bool DoCheck(sbyte[] content,string sign,string publicKey)
        {
            try
            {
                KeyFactory keyFactory = KeyFactory.GetInstance("RSA");
                sbyte[] encodedKey = (sbyte[])(Array)Base64.Decode(publicKey, Base64Flags.Default);
                IPublicKey pubKey = keyFactory.GeneratePublic(new X509EncodedKeySpec((byte[])(Array)encodedKey));
                Java.Security.Signature signature = Java.Security.Signature.GetInstance(SignAlgorithms256);
                signature.InitVerify(pubKey);
                signature.Update((byte[])(Array)content);
                return signature.Verify(Base64.Decode(sign, Base64Flags.Default));
            }
            catch (Exception ex)
            {
                Log.Info("RSA", "DoCheck fail: " + ex.Message);
                throw;
            }
        }

        public static string Encrypt(string source,string publicKey,string algorithm,string inputCharset)
        {
            IKey key = GetPublicKey(publicKey);
            Cipher cipher = Cipher.GetInstance(algorithm);
            cipher.Init(Javax.Crypto.CipherMode.EncryptMode, key);
            if (CommonUtil.IsNull(inputCharset))
            {
                inputCharset = "utf-8";
            }
            sbyte[] bytes = Hex2Byte(source);
            byte[] b1 = cipher.DoFinal((byte[])(Array)bytes);
            sbyte[] sb1 = (sbyte[])(Array)b1;
            string encoded = Base64Encode(sb1);
            return encoded;
        }
        /// <summary>
        /// encrypt bytes： src data
        /// </summary>
        /// <param name="bytes">bytes</param>
        /// <param name="publicKey">publicKey</param>
        /// <param name="algorithm">algorithm</param>
        /// <param name="inputCharset">inputCharset</param>
        /// <returns></returns>
        public static string Encrypt(sbyte[] bytes, string publicKey, string algorithm, string inputCharset)
        {
            IKey key = GetPublicKey(publicKey);
            Cipher cipher = Cipher.GetInstance(algorithm);
            cipher.Init(Javax.Crypto.CipherMode.EncryptMode, key);
            if (CommonUtil.IsNull(inputCharset))
            {
                inputCharset = "UTF-8";
            }
            byte[] b1 = cipher.DoFinal((byte[])(Array)bytes);
            sbyte[] sbyteArray = (sbyte[])(Array)b1;
            string encoded = Base64Hw.Encode(sbyteArray);
            return encoded;
        }

        public static sbyte[] Hex2Byte(string hex)
        {
            return HexStr2Byte(hex);
        }

        private static sbyte[] HexStr2Byte(string hex)
        {
            return hex == null ? new sbyte[0] : HwHex.DecodeHex(hex.ToCharArray());
        }
        public static string Base64Encode(sbyte[] binaryData)
        {
            if (CommonUtil.IsNull(binaryData))
            {
                return null;
            }
            int lengthDataBits = binaryData.Length * EightBit;
            if (lengthDataBits == 0)
            {
                return "";
            }

            int fewerThan24Bits = lengthDataBits % TwentyFourBitGroup;
            int numberTriplets = lengthDataBits / TwentyFourBitGroup;
            int numberQuartet = fewerThan24Bits != 0 ? numberTriplets + 1 : numberTriplets;
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
            AssembleInteger(binaryData, fewerThan24Bits, encodedData, encodedIndex, dataIndex);

            return new string(encodedData);
        }

        private static void AssembleInteger(sbyte[] binaryData, int fewerThan24Bits, char[] encodedData, int encodedIndex, int dataIndex)
        {
            sbyte b1;
            sbyte k1;
            sbyte b2;
            sbyte l1;
            sbyte val4 = 0;
            sbyte val5 = 0;
            if (fewerThan24Bits == EightBit)
            {
                b1 = binaryData[dataIndex];
                k1 = (sbyte)(b1 & 0x03);

                val4 = ((b1 & Sign) == 0) ? (sbyte)(b1 >> 2) : (sbyte)((b1) >> 2 ^ 0xc0);
                encodedData[encodedIndex++] = LookUpBase64Alphabet[val4];
                encodedData[encodedIndex++] = LookUpBase64Alphabet[k1 << 4];
                encodedData[encodedIndex++] = Pad;
                encodedData[encodedIndex++] = Pad;
            }
            else if (fewerThan24Bits == SixteenBit)
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
        /// Get PublicKey
        /// </summary>
        /// <param name="key">key(by Base64)</param>
        /// <returns></returns>
        private static IPublicKey GetPublicKey(string key)
        {
            X509EncodedKeySpec keySpec = new X509EncodedKeySpec(Base64Hw.Decode(key));
            KeyFactory keyFactory = KeyFactory.GetInstance("RSA");
            IPublicKey publicKey = keyFactory.GeneratePublic(keySpec);
            return publicKey;
        }
        /// <summary>
        /// remove WhiteSpace from MIME containing encoded Base64Hw data.
        /// </summary>
        /// <param name="data">the sbyte array of base64 data (with WS)</param>
        /// <returns>the new length</returns>
        private static int RemoveWhiteSpace(char[] data)
        {
            if (CommonUtil.IsNull(data))
            {
                return 0;
            }

            // count characters that's not whitespace
            int newSize = 0;
            int len = data.Length;
            for (int i = 0; i < len; i++)
            {
                if (!IsWhiteSpace(data[i]))
                {
                    data[newSize++] = data[i];
                }
            }
            return newSize;
        }

        private static bool IsWhiteSpace(char octect)
        {
            return (octect == 0x20 || octect == 0xd || octect == 0xa || octect == 0x9);
        }

        private static bool IsData(char octect)
        {
            return (octect < BaseLength && Base64Alphabet[octect] != -1);
        }

        private static sbyte[] CheckCharacters(char[] base64Data, char d1,char d2, int idx, int encodedIndex, int dataIndex, sbyte[] decodedData)
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
                    Java.Lang.JavaSystem.Arraycopy((byte[])(Array)decodedData, 0, (byte[])(Array)tmp, 0, idx * 3);
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
                    Java.Lang.JavaSystem.Arraycopy((byte[])(Array)decodedData, 0, (byte[])(Array)tmp, 0, idx * 3);
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

        private static bool IsNotPadD3AndPadD4(char d3, char d4)
        {
            return !IsPad(d3) && IsPad(d4);
        }

        private static bool IsPadD3AndD4(char d3, char d4)
        {
            return IsPad(d3) && IsPad(d4);
        }

        private static bool IsNotD3OrD4(char d3, char d4)
        {
            return !IsData(d3) || !IsData(d4);
        }

        private static bool IsPad(char octect) { return (octect == Pad); }

    }
}