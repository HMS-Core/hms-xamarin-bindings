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
using System;
using System.Collections.Generic;
using System.Text;
using Android.Content;
using Android.OS;
using Android.Provider;
using Android.Util;
using Huawei.Hms.Ml.Scan;

namespace XamarinHmsScanKitDemo.Action
{
    class ContactInfoAction
    {
        private static SparseArray<int> addressMap = new SparseArray<int>();
        private static SparseArray<int> phoneMap = new SparseArray<int>();
        private static SparseArray<int> emailMap = new SparseArray<int>();
        public ContactInfoAction()
        {
            addressMap.Put((int)HmsScan.AddressInfo.ADDRESS_TYPE.OtherUseType, (int)Android.Provider.SipAddressDataKind.Other);
            addressMap.Put((int)HmsScan.AddressInfo.ADDRESS_TYPE.OfficeType, (int)Android.Provider.SipAddressDataKind.Work);
            addressMap.Put((int)HmsScan.AddressInfo.ADDRESS_TYPE.ResidentialUseType, (int)Android.Provider.SipAddressDataKind.Home);

            phoneMap.Put(HmsScan.TelPhoneNumber.OtherUseType, (int)PhoneDataKind.Other);
            phoneMap.Put(HmsScan.TelPhoneNumber.OfficeUseType, (int)PhoneDataKind.Work);
            phoneMap.Put(HmsScan.TelPhoneNumber.ResidentialUseType, (int)PhoneDataKind.Home);
            phoneMap.Put(HmsScan.TelPhoneNumber.FaxUseType, (int)PhoneDataKind.FaxHome);
            phoneMap.Put(HmsScan.TelPhoneNumber.CellphoneNumberUseType, (int)PhoneDataKind.Mobile);

            emailMap.Put(HmsScan.EmailContent.OtherUseType, (int)EmailDataKind.Other);
            emailMap.Put(HmsScan.EmailContent.OfficeUseType, (int)EmailDataKind.Work);
        }

        public static Intent GetContactInfoIntent(HmsScan.ContactDetail contactInfo)
        {
            Intent intent = new Intent(Intent.ActionInsert, ContactsContract.Contacts.ContentUri);
            try
            {
                intent.PutExtra(ContactsContract.Intents.Insert.Name, contactInfo.PeopleName.FullName);
                intent.PutExtra(ContactsContract.Intents.Insert.JobTitle, contactInfo.Title);
                intent.PutExtra(ContactsContract.Intents.Insert.Company, contactInfo.Company);
                List<ContentValues> data = new List<ContentValues>();
                data.AddRange(GetAddresses(contactInfo));
                data.AddRange(GetPhones(contactInfo));
                data.AddRange(GetEmails(contactInfo));
                data.AddRange(GetUrls(contactInfo));
                intent.PutParcelableArrayListExtra(ContactsContract.Intents.Insert.Data, (IList<IParcelable>)data);
            }
            catch (Exception e)
            {
                Console.WriteLine("getCalendarEventIntent", e);
            }
            return intent;
        }


        private static List<ContentValues> GetAddresses(HmsScan.ContactDetail contactInfo)
        {
            List<ContentValues> data = new List<ContentValues>();
            if ((contactInfo.AddressesInfos != null))
            {
                foreach (HmsScan.AddressInfo address in contactInfo.AddressesInfos)
                {
                    if (address.AddressDetails != null)
                    {
                        ContentValues contentValues = new ContentValues();
                        contentValues.Put(ContactsContract.Data.ContentType, ContactsContract.CommonDataKinds.StructuredPostal.ContentItemType);
                        StringBuilder addressBuilder = new StringBuilder();
                        foreach (String addressLine in address.AddressDetails)
                        {
                            addressBuilder.Append(addressLine);
                        }
                        contentValues.Put(ContactsContract.CommonDataKinds.StructuredPostal.FormattedAddress, addressBuilder.ToString());
                        int type = addressMap.Get(address.AddressType);
                        contentValues.Put(ContactsContract.CommonDataKinds.StructuredPostal.ContentType,
                                type != 0 ? type : (int)Android.Provider.SipAddressDataKind.Other);
                        data.Add(contentValues);
                    }
                }
            }
            return data;
        }

        private static List<ContentValues> GetPhones(HmsScan.ContactDetail contactInfo)
        {
            List<ContentValues> data = new List<ContentValues>();
            if ((contactInfo.TelPhoneNumbers != null))
            {
                foreach (HmsScan.TelPhoneNumber phone in contactInfo.TelPhoneNumbers)
                {
                    ContentValues contentValues = new ContentValues();
                    contentValues.Put(ContactsContract.Data.InterfaceConsts.Mimetype, ContactsContract.CommonDataKinds.Phone.ContentItemType);
                    contentValues.Put(ContactsContract.CommonDataKinds.Phone.Number, phone.PhoneNumber);
                    int type = phoneMap.Get(phone.UseType);
                    contentValues.Put(ContactsContract.CommonDataKinds.Phone.ContentItemType, type != 0 ? type : (int)PhoneDataKind.Other);
                    data.Add(contentValues);
                }
            }
            return data;
        }

        private static List<ContentValues> GetEmails(HmsScan.ContactDetail contactInfo)
        {
            List<ContentValues> data = new List<ContentValues>();
            if ((contactInfo.EmailContents != null))
            {
                foreach (HmsScan.EmailContent email in contactInfo.EmailContents)
                {
                    ContentValues contentValues = new ContentValues();
                    contentValues.Put(ContactsContract.Data.InterfaceConsts.Mimetype, ContactsContract.CommonDataKinds.Email.ContentItemType);
                    contentValues.Put(ContactsContract.CommonDataKinds.Email.Address, email.AddressInfo);
                    int type = emailMap.Get(email.AddressType);
                    contentValues.Put(ContactsContract.CommonDataKinds.Email.ContentItemType, type != null ? type : (int)EmailDataKind.Other);
                    data.Add(contentValues);
                }
            }
            return data;
        }

        private static List<ContentValues> GetUrls(HmsScan.ContactDetail contactInfo)
        {
            List<ContentValues> data = new List<ContentValues>();
            if (contactInfo.ContactLinks != null)
            {
                foreach (String url in contactInfo.ContactLinks)
                {
                    ContentValues contentValues = new ContentValues();
                    contentValues.Put(ContactsContract.Data.InterfaceConsts.Mimetype, ContactsContract.CommonDataKinds.Website.ContentItemType);
                    contentValues.Put(ContactsContract.CommonDataKinds.Website.Url, url);
                    data.Add(contentValues);
                }
            }
            return data;
        }
    }
}