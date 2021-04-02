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
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics.Drawables;
using Android.Icu.Text;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Views;
using Android.Widget;
using Huawei.Cloud.Base.Http;
using Huawei.Cloud.Base.Media;
using Huawei.Cloud.Base.Util;
using Huawei.Cloud.Services.Drive;
using Huawei.Cloud.Services.Drive.Model;
using Java.Util;
using XHms_Drive_Kit_Demo_Project.DriveKit.Common;
using XHms_Drive_Kit_Demo_Project.DriveKit.Constants;
using XHms_Drive_Kit_Demo_Project.DriveKit.Hms;
using XHms_Drive_Kit_Demo_Project.DriveKit.Task;
using XHms_Drive_Kit_Demo_Project.DriveKit.Utils.Thumbnail;
using XHms_Drive_Kit_Demo_Project.DriveKit.Views.Activities;
using File = Huawei.Cloud.Services.Drive.Model.File;
using TimeUtils = XHms_Drive_Kit_Demo_Project.DriveKit.Utils.TimeUtils;
using Logger = XHms_Drive_Kit_Demo_Project.DriveKit.Log.Logger;

namespace XHms_Drive_Kit_Demo_Project.DriveKit.Fragment
{
    /// <summary>
    /// Interface Fragment, test for Drive interfaces.
    /// </summary>
    public class InterfaceFragment : Android.App.Fragment, View.IOnClickListener
    {
        private const string Tag = "InterfaceFragment";

        private static readonly string FileName = "IMG_20190712_155412.jpg";

        private static readonly string DocxFile = "test.docx";

        //Exit the webview page and refresh the file list page when you preview or edit a document online.
        public static readonly int WebViewBackRefresh = 5201;

        private static readonly long DirectUploadMaxSize = 20 * 1024 * 1024;

        private static readonly long DirectDownloadMaxSize = 20 * 1024 * 1024;

        // Successful result
        private static readonly int Success = 0;

        // Failure result
        private static readonly int Fail = 1;

        // Margin space
        private static readonly int ZoomOut = 30;

        // Main view
        private static View mView;

        // Context
        private Context context;

        private HistoryVersion mHistoryVersion;

        private HistoryVersion deleteHistoryVersions;

        // Used to cache metadata information after the folder is created successfully.
        private File mDirectory;

        // Used to cache metadata information after successful file creation
        private File mFile;

        private File mBackupFile;

        // Used to cache metadata information after the Comment is created successfully.
        private Comment mComment;

        //  Used to cache metadata information after the Reply is created successfully.
        private Reply mReply;

        // Used to cache channel token
        private string watchListPageToken;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            mView = inflater.Inflate(Resource.Layout.recent_fragment, container, false);
            context = Context;
            SetHasOptionsMenu(true);
            InitView();
            PrepareTestFile();
            return mView;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
        }

        public void OnClick(View v)
        {
            switch (v.Id)
            {
                case Resource.Id.drive_about_button_get:
                    ExecuteAboutGet();
                    break;
                case Resource.Id.drive_files_button_list:
                    ExecuteFilesList();
                    break;
                case Resource.Id.drive_files_button_create:
                    ExecuteFilesCreate();
                    break;
                case Resource.Id.drive_files_button_update:
                    ExecuteFilesUpdate();
                    break;
                case Resource.Id.drive_files_button_createfile:
                    ExecuteFilesCreateFile();
                    break;
                case Resource.Id.drive_files_button_get:
                    ExecuteFilesGet();
                    break;
                case Resource.Id.drive_files_button_copy:
                    ExecuteFilesCopy();
                    break;
                default:
                    OnViewClick(v.Id);
                    break;
            }
        }

        private void OnViewClick(int id)
        {
            switch (id)
            {
                case Resource.Id.drive_files_button_delete:
                    ExecuteFilesDelete();
                    break;
                case Resource.Id.drive_files_button_emptyRecycle:
                    ExecuteFilesEmptyRecycle();
                    break;
                case Resource.Id.drive_files_subscribe_button:
                    ExecuteFilesSubscribe();
                    break;
                case Resource.Id.drive_changes_subscribe_button:
                    ExecuteChangesSubscribe();
                    break;
                case Resource.Id.drive_changes_getstartcursor_button:
                    ExecuteChangesGetStartCursor();
                    break;
                case Resource.Id.drive_channels_stop:
                    ExecuteChannelsStop();
                    break;
                case Resource.Id.drive_changes_list_button:
                    ExecuteChangesList();
                    break;
                default:
                    OnOtherClick(id);
                    break;
            }
        }

        private void OnOtherClick(int id)
        {
            switch (id)
            {
                case Resource.Id.drive_files_update_content_button:
                    ExecuteFilesUpdateContent();
                    break;
                case Resource.Id.drive_replies_create:
                    ExecuteRepliesCreate();
                    break;
                case Resource.Id.drive_replies_list:
                    ExecuteRepliesList();
                    break;
                case Resource.Id.drive_replies_get:
                    ExecuteRepliesGet();
                    break;
                case Resource.Id.drive_replies_update:
                    ExecuteRepliesUpdate();
                    break;
                case Resource.Id.drive_replies_delete:
                    ExecuteRepliesDelete();
                    break;
                case Resource.Id.drive_comments_create:
                    ExecuteCommentsCreate();
                    break;
                case Resource.Id.drive_comments_list:
                    ExecuteCommentsList();
                    break;
                case Resource.Id.drive_comments_get:
                    ExecuteCommentsGet();
                    break;
                case Resource.Id.drive_comments_update:
                    ExecuteCommentsUpdate();
                    break;
                case Resource.Id.drive_comments_delete:
                    ExecuteCommentsDelete();
                    break;
                case Resource.Id.drive_historyversions_delete:
                    ExecuteHistoryVersionsDelete();
                    break;
                case Resource.Id.drive_historyversions_get:
                    ExecuteHistoryVersionsGet();
                    break;
                case Resource.Id.drive_historyversions_list:
                    ExecuteHistoryVersionsList();
                    break;
                case Resource.Id.drive_historyversions_update:
                    ExecuteHistoryVersionsUpdate();
                    break;
                case Resource.Id.drive_backup_btn:
                    ExecuteDataBackUp();
                    break;
                case Resource.Id.drive_restore_btn:
                    ExecuteDataRestore();
                    break;
                case Resource.Id.drive_online_open_btn:
                    ExecuteOnlineOpen();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Copy image to cache directory.
        /// </summary>
        private void PrepareTestFile()
        {
            try
            {
                Stream inputStream = context.Assets.Open(FileName);
                string cachePath = context.ExternalCacheDir.AbsolutePath;
                FileStream outputStream = new FileStream(cachePath + "/cache.jpg", FileMode.Create);
                byte[] bytesre = ReadFully(inputStream);
                outputStream.Write(bytesre, 0, bytesre.Length);
                byte[] buffer = new byte[1024];
                int byteCount = 0;

                outputStream.Flush();
                outputStream.Close();
                inputStream.Close();

                inputStream = context.Assets.Open(DocxFile);
                outputStream = new FileStream(cachePath + "/test.docx", FileMode.Create);
                byte[] bytesre2 = ReadFully(inputStream);
                outputStream.Write(bytesre, 0, bytesre.Length);

                outputStream.Flush();
                outputStream.Close();
                inputStream.Close();

                string BackFileName = "AppDataBackUpFileName.jpg";
                inputStream = context.Assets.Open(FileName);
                outputStream = new FileStream("/sdcard/" + BackFileName, FileMode.Create);
                byte[] bytesre3 = ReadFully(inputStream);
                outputStream.Write(bytesre, 0, bytesre.Length);

                outputStream.Flush();
                outputStream.Close();
                inputStream.Close();

                string accountFile = "account.json";
                inputStream = context.Assets.Open(accountFile);
                outputStream = new FileStream(cachePath + "/" + accountFile, FileMode.Create);
                byte[] bytesre4 = ReadFully(inputStream);
                outputStream.Write(bytesre, 0, bytesre.Length);
                
                outputStream.Flush();
                outputStream.Close();
                inputStream.Close();
            }
            catch (Exception e)
            {
                Logger.Error(Tag, "prepare file error: " + e.Message);
                return;
            }
        }

        public static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        //Handle UI refresh message.
        private Handler handler;

        /// <summary>
        /// Init UI view.
        /// </summary>
        private void InitView()
        {
            if (mView == null)
            {
                return;
            }

            handler = new Handler(
                delegate (Message msg)
                {
                    UpdateButtonUi(msg.Arg1, msg.What);
                }
            );

            Button driveAboutButton = (Button)mView.FindViewById(Resource.Id.drive_about_button_get);
            driveAboutButton.SetOnClickListener(this);

            Button deleteButton = (Button)mView.FindViewById(Resource.Id.drive_files_button_delete);
            deleteButton.SetOnClickListener(this);

            Button emptyRecycleButton = (Button)mView.FindViewById(Resource.Id.drive_files_button_emptyRecycle);
            emptyRecycleButton.SetOnClickListener(this);

            Button copyButton = (Button)mView.FindViewById(Resource.Id.drive_files_button_copy);
            copyButton.SetOnClickListener(this);

            Button generateIdButton = (Button)mView.FindViewById(Resource.Id.drive_files_button_createfile);
            generateIdButton.SetOnClickListener(this);

            Button getButton = (Button)mView.FindViewById(Resource.Id.drive_files_button_get);
            getButton.SetOnClickListener(this);

            Button listButton = (Button)mView.FindViewById(Resource.Id.drive_files_button_list);
            listButton.SetOnClickListener(this);

            Button subscribeButton = (Button)mView.FindViewById(Resource.Id.drive_files_subscribe_button);
            subscribeButton.SetOnClickListener(this);

            Button changesButton = (Button)mView.FindViewById(Resource.Id.drive_changes_subscribe_button);
            changesButton.SetOnClickListener(this);

            Button getCursorButton = (Button)mView.FindViewById(Resource.Id.drive_changes_getstartcursor_button);
            getCursorButton.SetOnClickListener(this);

            Button channelsStopButton = (Button)mView.FindViewById(Resource.Id.drive_channels_stop);
            channelsStopButton.SetOnClickListener(this);

            Button changesListButton = (Button)mView.FindViewById(Resource.Id.drive_changes_list_button);
            changesListButton.SetOnClickListener(this);

            Button createButton = (Button)mView.FindViewById(Resource.Id.drive_files_button_create);
            createButton.SetOnClickListener(this);

            Button updateButton = (Button)mView.FindViewById(Resource.Id.drive_files_button_update);
            updateButton.SetOnClickListener(this);

            Button updateContentButton = (Button)mView.FindViewById(Resource.Id.drive_files_update_content_button);
            updateContentButton.SetOnClickListener(this);

            Button repliesCreateButton = (Button)mView.FindViewById(Resource.Id.drive_replies_create);
            repliesCreateButton.SetOnClickListener(this);

            Button repliesListButton = (Button)mView.FindViewById(Resource.Id.drive_replies_list);
            repliesListButton.SetOnClickListener(this);

            Button repliesGetButton = (Button)mView.FindViewById(Resource.Id.drive_replies_get);
            repliesGetButton.SetOnClickListener(this);

            Button repliesUpdateButton = (Button)mView.FindViewById(Resource.Id.drive_replies_update);
            repliesUpdateButton.SetOnClickListener(this);

            Button repliesDeleteButton = (Button)mView.FindViewById(Resource.Id.drive_replies_delete);
            repliesDeleteButton.SetOnClickListener(this);

            Button commentsCreateButton = (Button)mView.FindViewById(Resource.Id.drive_comments_create);
            commentsCreateButton.SetOnClickListener(this);

            Button commentsListButton = (Button)mView.FindViewById(Resource.Id.drive_comments_list);
            commentsListButton.SetOnClickListener(this);

            Button commentsGetButton = (Button)mView.FindViewById(Resource.Id.drive_comments_get);
            commentsGetButton.SetOnClickListener(this);

            Button commentsUpdateButton = (Button)mView.FindViewById(Resource.Id.drive_comments_update);
            commentsUpdateButton.SetOnClickListener(this);

            Button commentsDeleteButton = (Button)mView.FindViewById(Resource.Id.drive_comments_delete);
            commentsDeleteButton.SetOnClickListener(this);

            Button historyVersionsDeleteButton = (Button)mView.FindViewById(Resource.Id.drive_historyversions_delete);
            historyVersionsDeleteButton.SetOnClickListener(this);

            Button historyVersionsGetButton = (Button)mView.FindViewById(Resource.Id.drive_historyversions_get);
            historyVersionsGetButton.SetOnClickListener(this);

            Button historyVersionsListButton = (Button)mView.FindViewById(Resource.Id.drive_historyversions_list);
            historyVersionsListButton.SetOnClickListener(this);

            Button historyVersionsUpdateButton = (Button)mView.FindViewById(Resource.Id.drive_historyversions_update);
            historyVersionsUpdateButton.SetOnClickListener(this);

            Button backUpButton = (Button)mView.FindViewById(Resource.Id.drive_backup_btn);
            backUpButton.SetOnClickListener(this);

            Button restoreButton = (Button)mView.FindViewById(Resource.Id.drive_restore_btn);
            restoreButton.SetOnClickListener(this);

            Button onlineOpenButton = (Button)mView.FindViewById(Resource.Id.drive_online_open_btn);
            onlineOpenButton.SetOnClickListener(this);
        }

        /// <summary>
        /// Update button background color and right icon.
        /// </summary>
        /// <param name="buttonId">buttonId</param>
        /// <param name="code">code</param>
        private void UpdateButtonUi(int buttonId, int code)
        {
            if (mView == null || Activity == null)
            {
                return;
            }
            Button button = (Button)mView.FindViewById(buttonId);
            Drawable drawable = null;
            Resources resources = Activity.Resources;
            if (code == Success)
            {
                drawable = Activity.GetDrawable(Resource.Mipmap.ic_success);
                if (resources != null && button != null)
                {
                    button.Background = Activity.GetDrawable(Resource.Drawable.button_circle_shape_green);
                }
            }
            else if (code == Fail)
            {
                if (resources != null && button != null)
                {
                    button.Background = Activity.GetDrawable(Resource.Drawable.button_circle_shape_red);
                }
                drawable = Activity.GetDrawable(Resource.Mipmap.ic_fail);
            }
            else
            {
                Logger.Info(Tag, "invalid result code");
            }
            if (drawable != null)
            {
                drawable.SetBounds(0, 0, drawable.MinimumWidth - ZoomOut, drawable.MinimumHeight - ZoomOut);
                if (button != null)
                {
                    button.SetCompoundDrawables(null, null, drawable, null);
                }
            }
        }

        /// <summary>
        /// Build a Drive instance.
        /// </summary>
        private Drive BuildDrive()
        {
            Drive service = (Drive)new Drive.Builder(CredentialManager.Instance.Credential, context).InvokeBuild();
            return service;
        }

        /// <summary>
        /// Execute the About.get interface test task.
        /// </summary>
        private void ExecuteAboutGet()
        {
            TaskManager.Instance.Execute(new DriveTaskImplementor
                (
                    delegate
                    {
                        DoAbout();
                    }
                ));
        }

        /// <summary>
        /// Test the About.get interface
        /// </summary>
        private void DoAbout()
        {
            try
            {
                Drive drive = BuildDrive();
                Drive.About about = drive.InvokeAbout();
                var aboutGet = (Drive.About.Get)about.InvokeGet().Set("fields", "*");
                About response = (About)aboutGet.Execute();
                CheckUpdateProtocol(response);
                SendHandleMessage(Resource.Id.drive_about_button_get, Success);
            }
            catch (Exception e)
            {
                SendHandleMessage(Resource.Id.drive_about_button_get, Fail);
                Logger.Error(Tag, "getAboutInfo error: " + e.ToString());
            }
        }

        /// <summary>
        /// Determine if you want to pop up the update page.
        /// </summary>
        /// <param name="about">Returned response.</param>
        private void CheckUpdateProtocol(About about)
        {
            if (about == null)
            {
                return;
            }
            Android.Util.Log.Debug(Tag, "checkUpdate: " + about.ToString());

            object updateValue = about.Get("needUpdate");
            bool isNeedUpdate = false;
            if (updateValue is bool)
            {
                isNeedUpdate = (bool)updateValue;
            }
            if (!isNeedUpdate)
            {
                return;
            }

            object urlValue = about.Get("updateUrl");
            string url = "";
            if (urlValue is string)
            {
                url = (string)urlValue;
            }
            if (TextUtils.IsEmpty(url))
            {
                return;
            }

            Android.Net.Uri uri = Android.Net.Uri.Parse(url);
            if (!"https".Equals(uri.Scheme))
            {
                return;
            }
            Intent intent = new Intent(Intent.ActionView, uri);
            try
            {
                StartActivity(intent);
            }
            catch (Exception e)
            {
                Logger.Error(Tag, "Activity Not found");
            }
        }

        /// <summary>
        /// Update the button style based on the returned result.
        /// </summary>
        /// <param name="buttonId">button id</param>
        /// <param name="result">Interface test result 0 success 1 failure</param>
        private void SendHandleMessage(int buttonId, int result)
        {
            Message message = handler.ObtainMessage();
            message.Arg1 = buttonId;
            message.What = result;
            handler.SendMessage(message);
        }

        /// <summary>
        /// Execute the Files.list interface test task.
        /// </summary>
        private void ExecuteFilesList()
        {
            TaskManager.Instance.Execute(new DriveTaskImplementor
                                    (
                                        delegate
                                        {
                                            DoFilesList();
                                        }
                                    ));
        }

        /// <summary>
        /// Test the Files.list interface.
        /// </summary>
        private void DoFilesList()
        {
            try
            {
                IList<File> folders = GetFileList("mimeType = 'application/vnd.huawei-apps.folder'", "fileName", 10, "*");
                Logger.Info(Tag, "executeFilesList: directory size =  " + folders.Count);
                if (folders.Count == 0)
                {
                    SendHandleMessage(Resource.Id.drive_files_button_list, Success);
                    return;
                }
                // get child files of a folder
                string directoryId = folders.ElementAt(0).Id;
                string queryStr = "'" + directoryId + "' in parentFolder and mimeType != 'application/vnd.huawei-apps.folder'";
                IList<File> files = GetFileList(queryStr, "fileName", 10, "*");
                Logger.Info(Tag, "executeFilesList: files size = " + files.Count);
                SendHandleMessage(Resource.Id.drive_files_button_list, Success);
            }
            catch (Exception e)
            {
                SendHandleMessage(Resource.Id.drive_files_button_list, Fail);
                Logger.Error(Tag, "executeFilesList exception: " + e.ToString());
            }
        }

        /// <summary>
        /// Traverse to get all files.
        /// </summary>
        /// <param name="query">Query conditions</param>
        /// <param name="orderBy">Sort conditions</param>
        /// <param name="pageSize">page Size</param>
        /// <param name="fields">fields</param>
        private IList<File> GetFileList(string query, string orderBy, int pageSize, string fields)
        {

            Drive drive = BuildDrive();
            Drive.Files.List request = drive.InvokeFiles().InvokeList();
            string pageToken = null;
            IList<File> fileList = new List<File>();
            do
            {
                FileList result = (FileList)request
                                    .SetQueryParam(query)
                                    .SetOrderBy(orderBy)
                                    .SetPageSize((Java.Lang.Integer)pageSize)
                                    .SetFields(fields)
                                    .Execute();
                foreach (File file in result.Files)
                {
                    fileList.Add(file);
                }
                pageToken = result.NextCursor;
                request.SetCursor(pageToken);
            } while (!String.IsNullOrEmpty(pageToken));
            Logger.Info(Tag, "getFileList: get files counts = " + fileList.Count);
            return fileList;
        }

        /// <summary>
        /// Get parent dir for copy files.
        /// </summary>
        /// <param name="fileList">files list</param>
        /// <returns>file ID of parent dir</returns>
        private IList<string> GetParentsId(FileList fileList)
        {
            if (fileList == null)
            {
                return null;
            }
            IList<File> files = fileList.Files;
            if (files == null || files.Count <= 0)
            {
                return null;
            }
            int size = files.Count;
            File file = files.ElementAt(size - 1);
            if (file == null)
            {
                return null;
            }
            // get the first one for test
            string parentDir = file.ParentFolder.ElementAt(0);
            IList<string> list = new List<string>();
            list.Add(parentDir);
            return list;
        }

        /// <summary>
        /// Execute the Files.create interface test task.
        /// </summary>
        private void ExecuteFilesCreate()
        {
            TaskManager.Instance.Execute(new DriveTaskImplementor
                        (
                            delegate
                            {
                                mDirectory = CreateDirectory();
                            }
                        ));
        }

        /// <summary>
        /// Create a directory.
        /// </summary>
        private File CreateDirectory()
        {
            try
            {
                Drive drive = BuildDrive();
                IDictionary<string, string> appProperties = new Dictionary<string, string>();
                appProperties.Add("appProperties", "property");
                SimpleDateFormat formatter = new SimpleDateFormat("yyyyMMdd_HHmmss_SSS");
                string dirName = formatter.Format(new Date());
                Logger.Info(Tag, "executeFilesCreate: " + dirName);

                File file = new File();
                file.SetFileName(dirName).SetAppSettings(appProperties).SetMimeType("application/vnd.huawei-apps.folder");
                File directory = (File)drive.InvokeFiles().InvokeCreate(file).Execute();
                SendHandleMessage(Resource.Id.drive_files_button_create, Success);
                return directory;
            }
            catch (Exception e)
            {
                SendHandleMessage(Resource.Id.drive_files_button_create, Fail);
                Logger.Error(Tag, "createDirectory error: " + e.ToString());
                return null;
            }
        }

        /// <summary>
        /// Execute the Files.update interface test task.
        /// </summary>
        private void ExecuteFilesUpdate()
        {
            TaskManager.Instance.Execute(new DriveTaskImplementor
            (
                delegate
                {
                    UpdateFile(mDirectory);
                }
            ));
        }

        /// <summary>
        /// Modify the file (directory) metaData, distinguish whether it is a file or a directory by MIMEType.
        /// </summary>
        /// <param name="file">File to be modified (directory)</param>
        private void UpdateFile(File file)
        {
            try
            {
                if (file == null)
                {
                    Logger.Error(Tag, "updateFile error, need to create file.");
                    SendHandleMessage(Resource.Id.drive_files_button_update, Fail);
                    return;
                }

                Drive drive = BuildDrive();
                File updateFile = new File();
                updateFile
                    .SetFileName(file.FileName + "_update")
                    .SetMimeType("application/vnd.huawei-apps.folder")
                    .SetDescription("update folder")
                    .SetFavorite((Java.Lang.Boolean)true);

                file = (File)drive.InvokeFiles().InvokeUpdate(file.Id, updateFile).Execute();
                Logger.Info(Tag, "updateFile result: " + file.ToString());
                SendHandleMessage(Resource.Id.drive_files_button_update, Success);
            }
            catch (Exception e)
            {
                SendHandleMessage(Resource.Id.drive_files_button_update, Fail);
                Logger.Error(Tag, "updateFile error: " + e.ToString());
            }
        }

        private void ExecuteFilesCreateFile()
        {
            TaskManager.Instance.Execute(new DriveTaskImplementor
                        (
                            delegate
                            {
                                string fileName = context.ExternalCacheDir.AbsolutePath + "/cache.jpg";
                                byte[] thumbnailImageBuffer = GetThumbnailImage(fileName);
                                string type = MimeType.GetMimeType(".jpg");
                                if (mDirectory == null)
                                {
                                    Logger.Error(Tag, "executeFilesCreateFile error, need to create Directory.");
                                    SendHandleMessage(Resource.Id.drive_files_button_createfile, Fail);
                                    return;
                                }
                                CreateFile(fileName, mDirectory.Id, thumbnailImageBuffer, type);
                            }
                        ));
        }

        /// <summary>
        /// create a image file by Files.create interface..
        /// </summary>
        /// <param name="filePath">Specifies the file to be uploaded.</param>
        /// <param name="parentId">Specifies the directory ID for uploading files.</param>
        /// <param name="thumbnailImageBuffer">thumbnail Image Data.</param>
        /// <param name="thumbnailMimeType">image mime type.</param>
        private void CreateFile(string filePath, string parentId, byte[] thumbnailImageBuffer, string thumbnailMimeType)
        {
            try
            {
                if (filePath == null)
                {
                    SendHandleMessage(Resource.Id.drive_files_button_createfile, Fail);
                    return;
                }

                Java.IO.File io = new Java.IO.File(filePath);
                FileContent fileContent = new FileContent(MimeType.GetMimeType(io), io);

                // set thumbnail , If it is not a media file, you do not need a thumbnail.
                File.ContentExtras contentPlus = new File.ContentExtras();
                File.ContentExtras.Thumbnail thumbnail = new File.ContentExtras.Thumbnail();
                thumbnail.SetContent(Huawei.Cloud.Base.Util.Base64.Base64.EncodeBase64String(thumbnailImageBuffer));
                thumbnail.SetMimeType(thumbnailMimeType);
                contentPlus.SetThumbnail(thumbnail);

                File content = new File().SetFileName(io.Name)
                    .SetMimeType(MimeType.GetMimeType(io))
                    .SetParentFolder(new SingletonList<string>(parentId))
                    .SetContentExtras(contentPlus);
                Drive drive = BuildDrive();
                Drive.Files.Create rquest = drive.InvokeFiles().InvokeCreate(content, fileContent);
                // default: resume, If the file Size is less than 20M, use directly upload.
                bool isDirectUpload = false;
                if (io.Length() < DirectUploadMaxSize)
                {
                    isDirectUpload = true;
                }
                rquest.MediaHttpUploader.SetDirectUploadEnabled(isDirectUpload);
                mFile = (File)rquest.Execute();
                Logger.Info(Tag, "executeFilesCreateFile:" + mFile.ToString());
                SendHandleMessage(Resource.Id.drive_files_button_createfile, Success);
            }
            catch (Exception e)
            {
                SendHandleMessage(Resource.Id.drive_files_button_createfile, Fail);
                Logger.Error(Tag, "executeFilesCreateFile exception: " + e.ToString());
            }
        }

        /// <summary>
        /// Generate and obtain the base64 code of the thumbnail.
        /// </summary>
        /// <returns>base64 code of the thumbnail</returns>
        private byte[] GetThumbnailImage(string imageFileName)
        {
            //imagePath: path to store thumbnail image
            string imagePath = "/storage/emulated/0/DCIM/Camera/";
            ThumbnailUtilsImage.GenerateImageThumbnail(imageFileName, imagePath + "imageThumbnail.jpg", 250, 150, 0);
            try
            {
                FileStream ins = new FileStream(imagePath + "imageThumbnail.jpg", FileMode.Open);
                var mem = new MemoryStream();
                ins.CopyTo(mem);
                byte[] buffer = mem.GetBuffer();

                return buffer;

            } 
            catch (Exception ex)
            {
                Logger.Error(Tag, ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Execute the Files.get interface test task.
        /// </summary>
        private void ExecuteFilesGet()
        {
            TaskManager.Instance.Execute(new DriveTaskImplementor
            (
                delegate
                {
                    DownLoadFile(mFile.Id);
                }
            ));
        }

        private class MediaHttpDownloaderProgressClass : Java.Lang.Object, IMediaHttpDownloaderProgressListener
        {
            public void ProgressChanged(MediaHttpDownloader mediaHttpDownloader)
            {
                // The download subthread invokes this method to process the download progress.
                double progress = mediaHttpDownloader.Progress;
            }
        }

        /// <summary>
        /// Test Files.get interface.
        /// </summary>
        /// <param name="fileId">Specifies the file to be obtained.</param>
        private void DownLoadFile(string fileId)
        {
            try
            {
                if (fileId == null)
                {
                    Logger.Error(Tag, "executeFilesGet error, need to create file.");
                    SendHandleMessage(Resource.Id.drive_files_button_get, Fail);
                    return;
                }
                string imagePath = "/storage/emulated/0/DCIM/Camera/";
                Drive drive = BuildDrive();
                // Get File metaData
                Drive.Files.Get request = drive.InvokeFiles().InvokeGet(fileId);
                request.SetFields("id,size");
                File res = (File)request.Execute();
                // Download File
                long size = res.Size();
                Drive.Files.Get get = drive.InvokeFiles().InvokeGet(fileId);
                get.SetForm("media");
                MediaHttpDownloader downloader = get.MediaHttpDownloader;

                bool isDirectDownload = false;
                if (size < DirectDownloadMaxSize)
                {
                    isDirectDownload = true;
                }
                downloader.SetContentRange(0, size - 1).SetDirectDownloadEnabled(isDirectDownload);
                downloader.SetProgressListener( new MediaHttpDownloaderProgressClass());
                Java.IO.File f = new Java.IO.File(imagePath + "download.jpg");
                FileStream fs = new FileStream(imagePath + "download.jpg", FileMode.OpenOrCreate);
                get.ExecuteContentAndDownloadTo(fs);
                
                Logger.Info(Tag, "executeFilesGetMedia success.");
                SendHandleMessage(Resource.Id.drive_files_button_get, Success);
            } catch (Exception e) {
                SendHandleMessage(Resource.Id.drive_files_button_get, Fail);
                Logger.Error(Tag, "executeFilesGet exception: " + e.ToString());
            }
        }

        /// <summary>
        /// Execute the Files.copy interface test task.
        /// </summary>
        private void ExecuteFilesCopy()
        {
            TaskManager.Instance.Execute(new DriveTaskImplementor
                        (
                            delegate
                            {
                                try
                                {
                                    Drive drive = BuildDrive();
                                    Drive.Files.List fileListReq = drive.InvokeFiles().InvokeList();
                                    fileListReq.SetQueryParam("mimeType = 'application/vnd.huawei-apps.folder'")
                                               .SetOrderBy("name")
                                               .SetPageSize((Java.Lang.Integer)100)
                                               .SetFields("*");
                                    FileList fileList = (FileList)fileListReq.Execute();
                                    IList<string> dstDir = GetParentsId(fileList);
                                    Logger.Error(Tag, "copyFile Source File Sharded Status: " + mFile.HasShared);
                                    CopyFile(mFile, dstDir);
                                }
                                catch (Exception e)
                                {
                                    Logger.Error(Tag, "copyFile -- list file error: " + e.ToString());
                                    SendHandleMessage(Resource.Id.drive_files_button_copy, Fail);
                                }
                            }
                        ));
        }

        /// <summary>
        /// Copy file.
        /// </summary>
        /// <param name="file">copy file</param>
        /// <param name="dstDir">Specifies the destination directory of the file to be copied.</param>
        private void CopyFile(File file, IList<string> dstDir)
        {
            try
            {

                // Copy operation, copy to the first created directory
                File copyFile = new File();
                if (file == null || file.FileName == null || dstDir == null)
                {
                    Logger.Error(Tag, "copyFile arguments error");
                    SendHandleMessage(Resource.Id.drive_files_button_copy, Fail);
                    return;
                }

                SimpleDateFormat formatter = new SimpleDateFormat("yyyyMMdd_HHmmss");
                string suffix = formatter.Format(new Date());
                copyFile.SetFileName(file.FileName + "_copy" + "_" + suffix);
                copyFile.SetDescription("copyFile");
                copyFile.SetParentFolder(dstDir);
                copyFile.SetFavorite((Java.Lang.Boolean)true);
                copyFile.SetEditedTime(new Huawei.Cloud.Base.Util.DateTime(System.DateTime.Now.Millisecond));

                Drive drive = BuildDrive();
                Drive.Files.Copy copyFileReq = drive.InvokeFiles().InvokeCopy(file.Id, copyFile);
                copyFileReq.SetFields("*");
                File result = (File)copyFileReq.Execute();
                Logger.Info(Tag, "copyFile: " + result.ToString());
                SendHandleMessage(Resource.Id.drive_files_button_copy, Success);
            }
            catch (Exception ex)
            {
                Logger.Error(Tag, "copyFile error: " + ex.ToString());
                SendHandleMessage(Resource.Id.drive_files_button_copy, Fail);
            }
        }

        /// <summary>
        /// Create a directory to test deleteFile.
        /// </summary>
        /// <returns>file</returns>
        private File GetDirectory()
        {
            File uploadFile = null;
            // Newly created directory
            Drive drive = BuildDrive();
            IDictionary<string, string> appProperties = new Dictionary<string, string>();
            appProperties.Add("test", "property");
            SimpleDateFormat formatter = new SimpleDateFormat("yyyyMMdd_HHmmss_SSS");
            string dir = formatter.Format(new Date());
            File file = new File();
            file.SetFileName(dir).SetAppSettings(appProperties).SetMimeType("application/vnd.huawei-apps.folder");
            try
            {
                uploadFile = (File)drive.InvokeFiles().InvokeCreate(file).Execute();
            }
            catch (Exception e)
            {
                Logger.Error(Tag, e.ToString());
            }
            return uploadFile;
        }

        /// <summary>
        /// Delete files (directories) from the recycle bin.
        /// </summary>
        /// <param name="fileId">file ID</param>
        private void DeleteFile(string fileId)
        {
            if (fileId == null)
            {
                Logger.Info(Tag, "deleteFile error, need to create file");
                SendHandleMessage(Resource.Id.drive_files_button_delete, Fail);
            }
            try
            {
                Drive drive = BuildDrive();
                Drive.Files.Delete deleteFileReq = drive.InvokeFiles().InvokeDelete(fileId);
                deleteFileReq.Execute();
                Logger.Info(Tag, "deleteFile result: " + deleteFileReq.ToString());
                SendHandleMessage(Resource.Id.drive_files_button_delete, Success);
            }
            catch (Exception ex)
            {
                SendHandleMessage(Resource.Id.drive_files_button_delete, Fail);
                Logger.Error(Tag, "deleteFile error: " + ex.ToString());
            }
        }

        /// <summary>
        /// Execute the Files.update test task.
        /// </summary>
        private void ExecuteFilesUpdateContent()
        {
            TaskManager.Instance.Execute(new DriveTaskImplementor
            (
                delegate
                {
                    string newFilePath = context.ExternalCacheDir.AbsolutePath + "/cache.jpg";
                    UpdateFile(mFile, newFilePath);
                }
            ));
        }

        /// <summary>
        /// Update the metadata and content of the file.
        /// </summary>
        /// <param name="oldFile">Specifies the old file to be updated.</param>
        /// <param name="newFilePath">new File.</param>
        void UpdateFile(File oldFile, string newFilePath)
        {
            try
            {
                if (oldFile == null || TextUtils.IsEmpty(newFilePath))
                {
                    Logger.Error(Tag, "updateFileContent error, need to create file.");
                    SendHandleMessage(Resource.Id.drive_files_update_content_button, Fail);
                    return;
                }

                Drive drive = BuildDrive();
                File content = new File();

                content.SetFileName(oldFile.FileName + "_update").SetMimeType(MimeType.GetMimeType(".jpg")).SetDescription("update image").SetFavorite((Java.Lang.Boolean)true);

                Java.IO.File io = new Java.IO.File(newFilePath);
                FileContent fileContent = new FileContent(MimeType.GetMimeType(io), io);
                Drive.Files.Update request = drive.InvokeFiles().InvokeUpdate(oldFile.Id, content, fileContent);
                bool isDirectUpload = false;
                if (io.Length() < DirectUploadMaxSize)
                {
                    isDirectUpload = true;
                }

                request.MediaHttpUploader.SetDirectUploadEnabled(isDirectUpload);
                mFile = (File)request.Execute();

                Logger.Info(Tag, "updateFileContent result: " + mFile.ToString());
                SendHandleMessage(Resource.Id.drive_files_update_content_button, Success);
            }
            catch (Exception e)
            {
                SendHandleMessage(Resource.Id.drive_files_update_content_button, Fail);
                Logger.Error(Tag, "updateFile error: " + e.ToString());
            }
        }

        /// <summary>
        /// Execute the Files.delete interface test task.
        /// </summary>
        private void ExecuteFilesDelete()
        {
            TaskManager.Instance.Execute(new DriveTaskImplementor
                            (
                                delegate
                                {
                                    //Create a folder and delete it
                                    File dir = GetDirectory();
                                    DeleteFile(dir.Id);
                                }
                            ));
        }

        /// <summary>
        /// Execute the Files.emptyRecycle interface test task.
        /// </summary>
        private void ExecuteFilesEmptyRecycle()
        {
            TaskManager.Instance.Execute(new DriveTaskImplementor
                (
                    delegate
                    {
                        DoFilesEmptyRecycle();
                    }
                ));
        }

        /// <summary>
        /// Empty recycle bin.
        /// </summary>
        private void DoFilesEmptyRecycle()
        {
            Drive drive = BuildDrive();
            try
            {
                // Create a new folder
                IDictionary<string, string> appProperties = new Dictionary<string, string>();
                appProperties.Add("property", "user_defined");
                SimpleDateFormat formatter = new SimpleDateFormat("yyyyMMdd_HHmmss_SSS");
                string dir = formatter.Format(new Date());
                File file = new File();
                file.SetFileName(dir).SetAppSettings(appProperties).SetMimeType("application/vnd.huawei-apps.folder");
                File uploadFile = (File)drive.InvokeFiles().InvokeCreate(file).Execute();

                // Call update to the recycle bin
                File trashFile = new File();
                trashFile.SetRecycled((Java.Lang.Boolean)true);
                drive.InvokeFiles().InvokeUpdate(uploadFile.Id, trashFile).Execute();
                // Empty the recycle bin
                Drive.Files.EmptyRecycle response = drive.InvokeFiles().InvokeEmptyRecycle();
                response.Execute();
                string value = response.ToString();
                Logger.Info(Tag, "executeFilesEmptyRecycle" + value);
                SendHandleMessage(Resource.Id.drive_files_button_emptyRecycle, Success);
            }
            catch (Exception e)
            {
                Logger.Error(Tag, "executeFilesEmptyRecycle error: " + e.ToString());
                SendHandleMessage(Resource.Id.drive_files_button_emptyRecycle, Fail);
            }
        }

        /// <summary>
        /// Execute the Files.subscribe interface test task.
        /// </summary>
        private void ExecuteFilesSubscribe()
        {
            TaskManager.Instance.Execute(new DriveTaskImplementor
                        (
                            delegate
                            {
                                FilesWatch(mFile.Id);
                            }
                        ));
        }

        /// <summary>
        /// Watching for changes to a file.
        /// </summary>
        /// <param name="fileId">file ID</param>
        private void FilesWatch(string fileId)
        {
            try
            {
                Drive drive = BuildDrive();
                Channel content = new Channel();
                content.SetId("id" + System.DateTime.Now.Millisecond);
                content.SetType("web_hook");
                content.SetUrl("https://www.huawei.com/path/to/webhook");
                Drive.Files.Subscribe request = drive.InvokeFiles().InvokeSubscribe(fileId, content);
                Channel channel = (Channel)request.Execute();
                //Object channel is used in other places.
                Logger.Info(Tag, "channel: " + channel.ToPrettyString());
                SendHandleMessage(Resource.Id.drive_files_subscribe_button, Success);
            }
            catch (Exception e)
            {
                SendHandleMessage(Resource.Id.drive_files_subscribe_button, Fail);
                Logger.Error(Tag, "Exception" + e.Message);
            }
        }

        /// <summary>
        /// Execute the Changes.startCursor interface test task.
        /// </summary>
        private void ExecuteChangesGetStartCursor()
        {
            TaskManager.Instance.Execute(new DriveTaskImplementor
                        (
                            delegate
                            {
                                DoGetStartCursor();
                            }
                        ));
        }

        /// <summary>
        /// In the future, the file will be changed. This gets the starting cursor of the changes.
        /// </summary>
        private void DoGetStartCursor()
        {
            try
            {
                Drive drive = BuildDrive();
                Drive.Changes.GetStartCursor request = drive.InvokeChanges().StartCursor;
                request.SetFields("*");
                StartCursor startPageToken = (StartCursor)request.Execute();
                Logger.Info(Tag, "GetStartCursor: " + startPageToken.ToString());
                SendHandleMessage(Resource.Id.drive_changes_getstartcursor_button, Success);
            }
            catch (Exception e)
            {
                SendHandleMessage(Resource.Id.drive_changes_getstartcursor_button, Fail);
                Logger.Error(Tag, "Exception" + e.Message);
            }
        }

        /// <summary>
        /// Execute the Changes.list interface test task.
        /// </summary>
        private void ExecuteChangesList()
        {
            TaskManager.Instance.Execute(new DriveTaskImplementor
                        (
                            delegate
                            {
                                GetChangesList(watchListPageToken);
                            }
                        ));
        }

        /// <summary>
        /// Lists the changes.
        /// </summary>
        /// <param name="cursor">
        /// The token to continue the previous list request on the next page.
        /// It must be the value of nextCursor in the previous response or in the getStartCursor method.
        /// </param>
        private IList<Change> GetChangesList(string cursor)
        {
            if (cursor == null)
            {
                SendHandleMessage(Resource.Id.drive_changes_list_button, Fail);
                Logger.Error(Tag, "getChangesList error: pageToken is null");
                return null;
            }
            try
            {
                Drive drive = BuildDrive();
                Drive.Changes.List listRequest = drive.InvokeChanges().InvokeList(cursor);
                listRequest.SetFields("*");
                ChangeList changeList = (ChangeList)listRequest.Execute();
                IList<Change> changes = changeList.Changes;
                SendHandleMessage(Resource.Id.drive_changes_list_button, Success);
                return changes;
            }
            catch (Exception e)
            {
                SendHandleMessage(Resource.Id.drive_changes_list_button, Fail);
                Logger.Error(Tag, "getChangesList error: " + e.ToString());
                return null;
            }
        }

        /// <summary>
        /// Execute the Changes.subscribe interface test task.
        /// </summary>
        private void ExecuteChangesSubscribe()
        {
            TaskManager.Instance.Execute(new DriveTaskImplementor
            (
                delegate
                {
                    string deviceId = HmsProxyImpl.Instance.DeviceId.Substring(0, 5);
                    WatchChanges(context, deviceId);
                }
            ));
        }

        /// <summary>
        /// Subscribe to file changes.
        /// </summary>
        /// <param name="context">current context</param>
        /// <param name="id">identifies a channel with a UUID or unique string. user_defined</param>
        private void WatchChanges(Context context, String id)
        {
            try
            {
                Drive drive = BuildDrive();
                StartCursor StartCursor = (StartCursor)drive.InvokeChanges().StartCursor.Execute();
                string startCursor = StartCursor.GetStartCursor();
                Channel content = new Channel();
                content.SetId(id);
                content.SetUserToken("1");
                content.SetType("web_hook");
                content.SetUrl("https://www.huawei.com/path/to/webhook");
                Java.Lang.Long currentTimeMillis = (Java.Lang.Long)TimeUtils.CurrentTimeMillis(); 
                content.SetExpirationTime(currentTimeMillis);
                Channel channel = (Channel)drive.InvokeChanges().InvokeSubscribe(startCursor, content).Execute();
                Logger.Info(Tag, "execute Watch channel" + channel.ToString());

                Android.Content.ISharedPreferences prefs = context.GetSharedPreferences("channel_config", FileCreationMode.Private);
                Android.Content.ISharedPreferencesEditor editor = prefs.Edit();
                editor.PutString("startPageVersion", startCursor);
                editor.PutString("resourceId", channel.ResourceId);
                editor.Commit();
                watchListPageToken = startCursor;
                SendHandleMessage(Resource.Id.drive_changes_subscribe_button, Success);
            }
            catch (Exception e)
            {
                SendHandleMessage(Resource.Id.drive_changes_subscribe_button, Fail);
                Logger.Error(Tag, "do Changes Subscribe error: " + e.ToString());
            }
        }

        /// <summary>
        /// Execute the Channels.stop interface test task.
        /// </summary>
        private void ExecuteChannelsStop()
        {
            TaskManager.Instance.Execute(new DriveTaskImplementor
            (
                delegate
                {
                    string id = HmsProxyImpl.Instance.DeviceId.Substring(0, 5);
                    StopChannel(context, id);
                }
            ));
        }

        /// <summary>
        /// Close the specified Channel.
        /// </summary>
        /// <param name="context">Current context</param>
        /// <param name="id">the same as channel.id returned by changes:watch.</param>
        private void StopChannel(Context context, String id)
        {
            try
            {
                Drive drive = BuildDrive();
                ISharedPreferences preferences = context.GetSharedPreferences("channel_config", FileCreationMode.Private);
                string resourceId = preferences.GetString("resourceId", "");
                Channel channel = new Channel();
                channel.SetId(id);
                channel.SetCategory("api#channel");
                channel.SetResourceId(resourceId);
                Drive.Channels.Stop stopReq = drive.InvokeChannels().InvokeStop(channel);
                stopReq.Execute();
                SendHandleMessage(Resource.Id.drive_channels_stop, Success);
            }
            catch (Exception e)
            {
                SendHandleMessage(Resource.Id.drive_channels_stop, Fail);
                Logger.Error(Tag, "stopChannel error: " + e.ToString());
            }
        }

        /// <summary>
        /// Execute the Replies Create interface test task.
        /// </summary>
        private void ExecuteRepliesCreate()
        {
            TaskManager.Instance.Execute(new DriveTaskImplementor
            (
                delegate
                {
                    if (mComment != null && mFile != null)
                    {
                        mReply = CreateReplies(mFile.Id, mComment.Id);
                    }
                    else
                    {
                        SendHandleMessage(Resource.Id.drive_replies_create, Fail);
                        Logger.Error(Tag, "replies create error: args wrong");
                    }
                }
            ));
        }

        /// <summary>
        /// Replying to a comment.
        /// </summary>
        /// <param name="fileId">file ID</param>
        /// <param name="commentId">comment ID</param>
        /// <returns>a Reply details.</returns>
        private Reply CreateReplies(string fileId, string commentId)
        {
            Drive drive = BuildDrive();
            Reply content = new Reply();
            content.SetDescription("a comment reply");
            try
            {
                Drive.Replies.Create request = (Drive.Replies.Create)drive.InvokeReplies().InvokeCreate(fileId, commentId, content).SetFields("*");
                Reply reply = (Reply)request.Execute();
                SendHandleMessage(Resource.Id.drive_replies_create, Success);
                return reply;
            }
            catch (Exception e)
            {
                SendHandleMessage(Resource.Id.drive_replies_create, Fail);
                Logger.Error(Tag, "replies create error: " + e.ToString());
                return null;
            }
        }

        /// <summary>
        /// Execute the Replies List interface test task.
        /// </summary>
        private void ExecuteRepliesList()
        {
            TaskManager.Instance.Execute(new DriveTaskImplementor
            (
                delegate
                {
                    if (mComment != null && mFile != null)
                    {
                        ListReplies(mFile.Id, mComment.Id);
                    }
                    else
                    {
                        SendHandleMessage(Resource.Id.drive_replies_list, Fail);
                        Logger.Error(Tag, "replies list error: args wrong");
                    }
                }
            ));
        }

        /// <summary>
        /// Get all replies to a comment.
        /// </summary>
        /// <param name="fileId">file ID</param>
        /// <param name="commentId">comment ID</param>
        /// <returns>a list that has All replies on comments.</returns>
        private IList<Reply> ListReplies(string fileId, string commentId)
        {
            Drive drive = BuildDrive();
            List<Reply> replyArrayList = new List<Reply>();
            string nextCursor = null;
            try
            {
                Drive.Replies.List request = (Drive.Replies.List)drive.InvokeReplies().InvokeList(fileId, commentId).SetFields("*");
                do
                {
                    if (nextCursor != null)
                    {
                        request.SetCursor(nextCursor);
                    }
                    ReplyList rlist = (ReplyList)request.SetPageSize((Java.Lang.Integer)100).Execute();
                    IList<Reply> replies = rlist.Replies;
                    if (replies == null)
                    {
                        break;
                    }
                    replyArrayList.AddRange(replies);
                    nextCursor = rlist.NextCursor;
                } while (!StringUtils.IsNullOrEmpty(nextCursor));
                Logger.Info(Tag, "replies size " + replyArrayList.Count);
                SendHandleMessage(Resource.Id.drive_replies_list, Success);
                return replyArrayList;
            }
            catch (Exception e)
            {
                SendHandleMessage(Resource.Id.drive_replies_list, Fail);
                Logger.Error(Tag, "permission list error: " + e.ToString());
                return null;
            }
        }

        /// <summary>
        /// Execute the Replies Get interface test task.
        /// </summary>
        private void ExecuteRepliesGet()
        {
            TaskManager.Instance.Execute(new DriveTaskImplementor
            (
                delegate
                {
                    if (mComment != null && mFile != null && mReply != null)
                    {
                        mReply = GetReplies(mFile.Id, mComment.Id, mReply.Id);
                    }
                    else
                    {
                        SendHandleMessage(Resource.Id.drive_replies_get, Fail);
                        Logger.Error(Tag, "replies get error: args wrong");
                    }
                }
            ));
        }

        /// <summary>
        /// Get a reply.
        /// </summary>
        /// <param name="fileId">file ID</param>
        /// <param name="commentId">comment ID</param>
        /// <param name="replyId">reply ID</param>
        /// <returns>reply details</returns>
        private Reply GetReplies(string fileId, string commentId, string replyId)
        {
            Drive drive = BuildDrive();
            try
            {
                Drive.Replies.Get request = (Drive.Replies.Get) drive.InvokeReplies().InvokeGet(fileId, commentId, replyId).SetFields("*");
                Reply reply = (Reply)request.Execute();
                Logger.Info(Tag, "get reply :" + reply.Description + ", " + reply.CreatedTime + ", " + reply.Creator);
                SendHandleMessage(Resource.Id.drive_replies_get, Success);
                return reply;
            }
            catch (Exception e)
            {
                SendHandleMessage(Resource.Id.drive_replies_get, Fail);
                Logger.Error(Tag, "replies get error: " + e.ToString());
                return null;
            }
        }

        /// <summary>
        /// Execute the Replies Update interface test task.
        /// </summary>
        private void ExecuteRepliesUpdate()
        {
            TaskManager.Instance.Execute(new DriveTaskImplementor
            (
                delegate
                {
                    if (mComment != null && mFile != null && mReply != null)
                    {
                        UpdateReplies(mFile.Id, mComment.Id, mReply.Id);
                    }
                    else
                    {
                        SendHandleMessage(Resource.Id.drive_replies_update, Fail);
                        Logger.Error(Tag, "replies update error: args wrong");
                    }
                }
            ));
        }

        /// <summary>
        /// Update a reply.
        /// </summary>
        /// <param name="fileId">file ID</param>
        /// <param name="commentId">comment ID</param>
        /// <param name="replyId">reply ID</param>
        private void UpdateReplies(string fileId, string commentId, string replyId)
        {
            Drive drive = BuildDrive();
            try
            {
                Reply latestReply = new Reply();
                latestReply.SetDescription("update a reply");
                // Update reply
                Drive.Replies.Update update_request = (Drive.Replies.Update)drive.InvokeReplies().InvokeUpdate(fileId, commentId, replyId, latestReply).SetFields("*");
                mReply = (Reply) update_request.Execute();
                Logger.Info(Tag, "get reply :" + mReply.Description + ", " + mReply.EditedTime + ", " + mReply.Creator);

                SendHandleMessage(Resource.Id.drive_replies_update, Success);
            }
            catch (Exception e)
            {
                SendHandleMessage(Resource.Id.drive_replies_update, Fail);
                Logger.Error(Tag, "replies update error: " + e.ToString());
            }
        }

        /// <summary>
        /// Execute the Replies Delete interface test task.
        /// </summary>
        private void ExecuteRepliesDelete()
        {
            TaskManager.Instance.Execute(new DriveTaskImplementor
            (
                delegate
                {
                    if (mComment != null && mFile != null && mReply != null)
                    {
                        DeleteReplies(mFile.Id, mComment.Id, mReply.Id);
                    }
                    else
                    {
                        SendHandleMessage(Resource.Id.drive_replies_delete, Fail);
                        Logger.Error(Tag, "replies delete error: args wrong");
                    }
                }
            ));
        }

        /// <summary>
        /// Deleting a reply.
        /// </summary>
        /// <param name="fileId">file ID.</param>
        /// <param name="commentId">comment ID.</param>
        /// <param name="replyId">reply ID.</param>
        private void DeleteReplies(string fileId, string commentId, string replyId)
        {
            Drive drive = BuildDrive();
            try
            {
                Drive.Replies.Delete request = drive.InvokeReplies().InvokeDelete(fileId, commentId, replyId);
                request.Execute();
                SendHandleMessage(Resource.Id.drive_replies_delete, Success);
            }
            catch (Exception e)
            {
                SendHandleMessage(Resource.Id.drive_replies_delete, Fail);
                Logger.Error(Tag, "replies delete error: " + e.ToString());
            }
        }

        /// <summary>
        /// Execute the Comments Create interface test task.
        /// </summary>
        private void ExecuteCommentsCreate()
        {
            TaskManager.Instance.Execute(new DriveTaskImplementor
            (
                delegate
                {
                    if (mFile != null)
                    {
                        mComment = CreateComments(mFile.Id);
                    }
                    else
                    {
                        SendHandleMessage(Resource.Id.drive_comments_create, Fail);
                        Logger.Error(Tag, "comment create error: args wrong");
                    }
                }
            ));
        }

        /// <summary>
        /// Add a comment below the file.
        /// </summary>
        /// <param name="fileId">file ID</param>
        /// <returns>comment details</returns>
        private Comment CreateComments(string fileId)
        {
            Drive drive = BuildDrive();
            Comment content = new Comment();
            content.SetDescription("test description");
            try
            {
                Drive.Comments.Create request = (Drive.Comments.Create)drive.InvokeComments().InvokeCreate(fileId, content).SetFields("*");
                Comment comment = (Comment)request.Execute();
                SendHandleMessage(Resource.Id.drive_comments_create, Success);
                return comment;
            }
            catch (Exception e)
            {
                SendHandleMessage(Resource.Id.drive_comments_create, Fail);
                Logger.Error(Tag, "comment create error: " + e.ToString());
                return null;
            }
        }

        /// <summary>
        /// Execute the Comments List interface test task.
        /// </summary>
        private void ExecuteCommentsList()
        {
            TaskManager.Instance.Execute(new DriveTaskImplementor
            (
                delegate
                {
                    if (mFile != null)
                    {
                        IList<Comment> list = ListComments(mFile.Id);
                        if (list != null)
                        {
                            SendHandleMessage(Resource.Id.drive_comments_list, Success);
                        }
                        else
                        {
                            SendHandleMessage(Resource.Id.drive_comments_list, Fail);
                        }
                    }
                    else
                    {
                        SendHandleMessage(Resource.Id.drive_comments_list, Fail);
                        Logger.Error(Tag, "comment list error: args wrong");
                    }
                }
            ));
        }

        /// <summary>
        /// List all comments on file.
        /// </summary>
        /// <param name="fileId">file ID.</param>
        /// <returns>a list that has All Comments on file.</returns>
        private IList<Comment> ListComments(string fileId)
        {
            Drive drive = BuildDrive();
            List<Comment> commentArrayList = new List<Comment>();
            string nextCursor = null;
            try
            {
                Drive.Comments.List request = drive.InvokeComments().InvokeList(fileId);
                do
                {
                    if (nextCursor != null)
                    {
                        request.SetCursor(nextCursor);
                    }
                    CommentList commentList = (CommentList)request.SetPageSize((Java.Lang.Integer)100).SetFields("*").Execute();
                    IList<Comment> comments = commentList.Comments;
                    if (comments == null)
                    {
                        break;
                    }
                    commentArrayList.AddRange(comments);
                    nextCursor = commentList.NextCursor;
                } while (!StringUtils.IsNullOrEmpty(nextCursor));
                Logger.Info(Tag, "comments size " + commentArrayList.Count);
                return commentArrayList;
            }
            catch (Exception e)
            {
                Logger.Error(Tag, "comments list error: " + e.ToString());
                return null;
            }
        }

        /// <summary>
        /// Execute the Comments Get interface test task.
        /// </summary>
        private void ExecuteCommentsGet()
        {
            TaskManager.Instance.Execute(new DriveTaskImplementor
            (
                delegate
                {
                    if (mFile != null && mComment != null)
                    {
                        mComment = GetComments(mFile.Id, mComment.Id);
                    }
                    else
                    {
                        SendHandleMessage(Resource.Id.drive_comments_get, Fail);
                        Logger.Error(Tag, "comment get error: args wrong");
                    }
                }
            ));
        }

        /// <summary>
        /// Get a comment.
        /// </summary>
        /// <param name="fileId">file ID</param>
        /// <param name="commentId">comment ID</param>
        /// <returns>comment details.</returns>
        private Comment GetComments(string fileId, string commentId)
        {
            Drive drive = BuildDrive();
            try
            {
                Drive.Comments.Get request = (Drive.Comments.Get)drive.InvokeComments().InvokeGet(fileId, commentId).SetFields("*");
                Comment latestComment = (Comment)request.Execute();
                Logger.Info(Tag, latestComment.Description + "," + latestComment.CreatedTime);
                SendHandleMessage(Resource.Id.drive_comments_get, Success);
                return latestComment;
            }
            catch (Exception e)
            {
                SendHandleMessage(Resource.Id.drive_comments_get, Fail);
                Logger.Error(Tag, "comment get error: " + e.ToString());
                return null;
            }
        }

        /// <summary>
        /// Execute the Comments Update interface test task.
        /// </summary>
        private void ExecuteCommentsUpdate()
        {
            TaskManager.Instance.Execute(new DriveTaskImplementor
            (
                delegate
                {
                    if (mFile != null && mComment != null)
                    {
                        UpdateComments(mFile.Id, mComment.Id);
                    }
                    else
                    {
                        SendHandleMessage(Resource.Id.drive_comments_update, Fail);
                        Logger.Error(Tag, "comment update error: args wrong");
                    }
                }
            ));
        }
        
        /// <summary>
        /// Update a comment on file.
        /// </summary>
        /// <param name="fileId">file ID</param>
        /// <param name="commentId">comment ID</param>
        private void UpdateComments(string fileId, string commentId)
        {
            Drive drive = BuildDrive();
            try
            {
                Comment comment = new Comment();
                comment.SetDescription("update a comment");

                Drive.Comments.Update update_request = (Drive.Comments.Update)drive.InvokeComments().InvokeUpdate(fileId, commentId, comment).SetFields("*");
                mComment = (Comment)update_request.Execute();
                Logger.Info(Tag, mComment.Description + ", " + mComment.Creator + ", " + mComment.EditedTime);
                SendHandleMessage(Resource.Id.drive_comments_update, Success);
            }
            catch (Exception e)
            {
                SendHandleMessage(Resource.Id.drive_comments_update, Fail);
                Logger.Error(Tag, "comment update error: " + e.ToString());
            }
        }

        /// <summary>
        /// Execute the Comments Delete interface test task.
        /// </summary>
        private void ExecuteCommentsDelete()
        {
            TaskManager.Instance.Execute(new DriveTaskImplementor
            (
                delegate
                {
                    if (mFile == null)
                    {
                        SendHandleMessage(Resource.Id.drive_comments_delete, Fail);
                        Logger.Error(Tag, "comment delete error: args wrong");
                    }
                    // Create a comment
                    Comment content = new Comment();
                    content.SetDescription("a tmp description");

                    Drive drive = BuildDrive();
                    try
                    {
                        Drive.Comments.Create request = (Drive.Comments.Create)drive.InvokeComments().InvokeCreate(mFile.Id, content).SetFields("*");
                        Comment comment = (Comment)request.Execute();
                        DeleteComments(mFile.Id, comment.Id);
                    }
                    catch (Exception e)
                    {
                        SendHandleMessage(Resource.Id.drive_comments_delete, Fail);
                        Logger.Error(Tag, "comment create error: " + e.ToString());
                    }
                }
            ));
        }

        /// <summary>
        /// Delete a comment.
        /// </summary>
        /// <param name="fileId">file ID</param>
        /// <param name="commentId">comment ID</param>
        private void DeleteComments(string fileId, string commentId)
        {
            Drive drive = BuildDrive();
            try
            {
                Drive.Comments.Delete request = drive.InvokeComments().InvokeDelete(fileId, commentId);
                request.Execute();
                SendHandleMessage(Resource.Id.drive_comments_delete, Success);
            }
            catch (Exception e)
            {
                SendHandleMessage(Resource.Id.drive_comments_delete, Fail);
                Logger.Error(Tag, "comment delete error: " + e.ToString());
            }
        }

        /// <summary>
        /// Delete a history version.
        /// </summary>
        private void ExecuteHistoryVersionsDelete()
        {
            TaskManager.Instance.Execute(new DriveTaskImplementor
            (
                delegate
                {
                    if (mFile != null && deleteHistoryVersions != null)
                    {
                        DeleteHistoryVersions(mFile.Id, deleteHistoryVersions.Id);
                    }
                    else
                    {
                        Logger.Error(Tag, "mHistoryVersion is null or mFile is null.");
                        SendHandleMessage(Resource.Id.drive_historyversions_delete, Fail);
                    }
                }
            ));
        }

        private void DeleteHistoryVersions(string fileId, string historyVersionId)
        {
            Drive drive = BuildDrive();
            if (drive == null)
            {
                Logger.Error(Tag, "deleteHistoryVersions error: drive is null.");
                SendHandleMessage(Resource.Id.drive_historyversions_delete, Fail);
                return;
            }
            try
            {
                drive.InvokeHistoryVersions().InvokeDelete(fileId, historyVersionId).Execute();
                SendHandleMessage(Resource.Id.drive_historyversions_delete, Success);
            }
            catch (Exception ex)
            {
                SendHandleMessage(Resource.Id.drive_historyversions_delete, Fail);
                Logger.Error(Tag, "deleteHistoryVersions error: " + ex.ToString());
            }
        }

        /// <summary>
        /// Get a history version.
        /// </summary>
        private void ExecuteHistoryVersionsGet()
        {
            TaskManager.Instance.Execute(new DriveTaskImplementor
            (
                delegate
                {
                    HistoryVersion temp;
                    if (mFile != null && mHistoryVersion != null)
                    {
                        temp = GetHistoryVersions(mFile.Id, mHistoryVersion.Id);
                    }
                    else
                    {
                        Logger.Error(Tag, "mHistoryVersion is null or mFile is null.");
                        SendHandleMessage(Resource.Id.drive_historyversions_get, Fail);
                        return;
                    }
                    if (temp == null)
                    {
                        SendHandleMessage(Resource.Id.drive_historyversions_get, Fail);
                        return;
                    }
                    SendHandleMessage(Resource.Id.drive_historyversions_get, Success);
                }
            ));
        }

        private HistoryVersion GetHistoryVersions(string fileId, string historyVersionId)
        {
            Drive drive = BuildDrive();
            if (drive == null)
            {
                Logger.Error(Tag, "getHistoryVersions error: drive is null.");
                return null;
            }
            HistoryVersion historyVersion = null;
            try
            {
                historyVersion = (HistoryVersion) drive.InvokeHistoryVersions().InvokeGet(fileId, historyVersionId).Execute();
            }
            catch (Exception ex)
            {
                Logger.Error(Tag, "getHistoryVersions error: " + ex.ToString());
            }
            return historyVersion;
        }

        private void ExecuteHistoryVersionsList()
        {
            TaskManager.Instance.Execute(new DriveTaskImplementor
            (
                delegate
                {
                    HistoryVersionList historyVersionList;
                    if (mFile != null)
                    {
                        historyVersionList = ListHistoryVersions(mFile.Id);
                    }
                    else
                    {
                        Logger.Error(Tag, "mFile is null.");
                        SendHandleMessage(Resource.Id.drive_historyversions_list, Fail);
                        return;
                    }
                    if (historyVersionList != null)
                    {
                        Logger.Error(Tag, "historyVersionList size " + historyVersionList.HistoryVersions.Count);
                        if (historyVersionList.HistoryVersions.Count > 0)
                        {
                            mHistoryVersion = historyVersionList.HistoryVersions.ElementAt(0);
                        }
                        if (historyVersionList.HistoryVersions.Count > 1)
                        {
                            deleteHistoryVersions = historyVersionList.HistoryVersions.ElementAt(1); ;
                        }
                        SendHandleMessage(Resource.Id.drive_historyversions_list, Success);
                    }
                    else
                    {
                        SendHandleMessage(Resource.Id.drive_historyversions_list, Fail);
                    }
                }
            ));
        }

        /// <summary>
        /// List all history versions.
        /// </summary>
        /// <param name="fileId">file ID</param>
        /// <returns>HistoryVersionList</returns>
        private HistoryVersionList ListHistoryVersions(string fileId)
        {
            Drive drive = BuildDrive();
            HistoryVersionList historyVersionList = null;
            if (drive == null)
            {
                Logger.Error(Tag, "listHistoryVersions error: drive is null.");
                return null;
            }
            try
            {
                historyVersionList = (HistoryVersionList)drive.InvokeHistoryVersions().InvokeList(fileId).SetFields("*").Execute();
            }
            catch (Exception ex)
            {
                Logger.Error(Tag, "listHistoryVersions error: " + ex.ToString());
            }
            return historyVersionList;
        }

        private void ExecuteHistoryVersionsUpdate()
        {
            TaskManager.Instance.Execute(new DriveTaskImplementor
            (
                delegate
                {
                    if (mFile != null && mHistoryVersion != null)
                    {
                        UpdateHistoryVersions(mFile.Id, mHistoryVersion);
                    }
                    else
                    {
                        Logger.Error(Tag, "mFile is null or mHistoryVersion is null");
                        SendHandleMessage(Resource.Id.drive_historyversions_update, Fail);
                    }
                }
            ));
        }

        /// <summary>
        /// Update one history versions
        /// </summary>
        /// <param name="fileId"></param>
        /// <param name="oldHistoryVersion"></param>
        private void UpdateHistoryVersions(string fileId, HistoryVersion oldHistoryVersion)
        {
            Drive drive = BuildDrive();
            if (drive == null)
            {
                Android.Util.Log.Error(Tag, "updateHistoryVersions error: drive is null.");
                SendHandleMessage(Resource.Id.drive_historyversions_update, Fail);
                return;
            }
            if (oldHistoryVersion == null)
            {
                Android.Util.Log.Error(Tag, "updateHistoryVersions error: oldHistoryVersion is null.");
                SendHandleMessage(Resource.Id.drive_historyversions_update, Fail);
                return;
            }
            try
            {
                // Modify whatever you want
                HistoryVersion historyVersion = new HistoryVersion();
                var reverseKeepPermanent = !(bool)oldHistoryVersion.KeepPermanent;
                historyVersion.SetKeepPermanent((Java.Lang.Boolean)reverseKeepPermanent);
                drive.InvokeHistoryVersions().InvokeUpdate(fileId, oldHistoryVersion.Id, historyVersion).Execute();
                SendHandleMessage(Resource.Id.drive_historyversions_update, Success);
            }
            catch (Exception ex)
            {
                SendHandleMessage(Resource.Id.drive_historyversions_update, Fail);
                Android.Util.Log.Error(Tag, "updateHistoryVersions error: " + ex.ToString());
            }
        }

        private void ExecuteDataBackUp()
        {
            TaskManager.Instance.Execute(new DriveTaskImplementor
            (
                delegate
                {
                    DoBackUpData();
                }
            ));
        }

        /// <summary>
        /// Assume that the application data file is AppDataBackUpFileName.jpg.
        /// Application data backup is to upload the AppDataBackUpFileName.jpg file to the application data directory on the cloud disk.
        /// </summary>
        private void DoBackUpData()
        {
            try
            {
                Drive drive = BuildDrive();
                string BackFileName = "AppDataBackUpFileName.jpg";
                string personalBackUpFolder = "AppName";
                Java.IO.File fileObject = new Java.IO.File("/sdcard/" + BackFileName);
                IDictionary<string, string> appProperties = new Dictionary<string, string>();
                // 1. create backup folder under folder "applicationData" first
                appProperties.Add("appProperties", "property");

                File file = new File();
                file.SetFileName(personalBackUpFolder + System.DateTime.Now.Millisecond.ToString())
                    .SetMimeType("application/vnd.huawei-apps.folder")
                    .SetAppSettings(appProperties)
                    .SetParentFolder(new SingletonList<string>("applicationData"));
                File directoryCreated = (File)drive.InvokeFiles().InvokeCreate(file).Execute();

                // 2. backUp: upload file "AppDatabackUpFileName"
                string mimeType = MimeType.GetMimeType(fileObject);
                File content = new File();
                content.SetFileName(fileObject.Name).SetMimeType(mimeType).SetParentFolder(new SingletonList<string>(directoryCreated.Id));
                mBackupFile = (File)drive.InvokeFiles().InvokeCreate(content, new FileContent(mimeType, fileObject)).SetFields("*").Execute();
                SendHandleMessage(Resource.Id.drive_backup_btn, Success);
            }
            catch (Exception e)
            {
                SendHandleMessage(Resource.Id.drive_backup_btn, Fail);
                Logger.Error(Tag, "BackUpData error: " + e.ToString());
            }
        }

        private void ExecuteDataRestore()
        {
            TaskManager.Instance.Execute(new DriveTaskImplementor
            (
                delegate
                {
                    if (mBackupFile != null)
                    {
                        DoRestoreData();
                    }
                    else
                    {
                        Logger.Error(Tag, "mBackUpFile is null");
                        SendHandleMessage(Resource.Id.drive_backup_btn, Fail);
                    }
                }
            ));
        }

        /// <summary>
        /// Restore file named "AppDataBackUpFileName".
        /// </summary>
        private void DoRestoreData()
        {
            Drive drive = BuildDrive();
            string searchFileName = "AppDataBackUpFileName.jpg";
            // must set containers as "applicationData" if we want list the backUp files
            string containers = "applicationData";
            string queryFile = "fileName = '" + searchFileName + "' and mimeType != 'application/vnd.huawei-apps.folder'";
            try
            {
                // 1. list all back files
                Drive.Files.List request = drive.InvokeFiles().InvokeList();
                FileList files;
                List<File> backupFiles = new List<File>();
                while (true)
                {
                    files = (FileList)request.SetQueryParam(queryFile).SetPageSize((Java.Lang.Integer)10).SetOrderBy("fileName").SetContainers(containers).SetFields("category,nextCursor,files/id,files/fileName,files/size").Execute();
                    if (files == null || files.Files.Count > 0)
                    {
                        break;
                    }

                    backupFiles.AddRange(files.Files);
                    string nextCursor = files.NextCursor;
                    if (!StringUtils.IsNullOrEmpty(nextCursor))
                    {
                        request.SetCursor(files.NextCursor);
                    }
                    else
                    {
                        break;
                    }
                }

                // 2. we can download "AppDataBackUpFileName" to restore local file
                long size = (long)mBackupFile.Size;
                Drive.Files.Get get = drive.InvokeFiles().InvokeGet(mBackupFile.Id);
                MediaHttpDownloader downloader = get.MediaHttpDownloader;

                bool isDirectDownload = false;
                if (size < DirectDownloadMaxSize)
                {
                    isDirectDownload = true;
                }
                downloader.SetContentRange(0, size - 1).SetDirectDownloadEnabled(isDirectDownload);
                string restoreFileName = "restoreFileName-"+ System.DateTime.Now.Millisecond +".jpg";
                Java.IO.File f = new Java.IO.File("/sdcard/" + restoreFileName);
                FileStream fs = new FileStream("/sdcard/" + restoreFileName, FileMode.OpenOrCreate);
                get.ExecuteContentAndDownloadTo(fs);
                SendHandleMessage(Resource.Id.drive_restore_btn, Success);
            }
            catch (Exception e)
            {
                SendHandleMessage(Resource.Id.drive_restore_btn, Fail);
                Logger.Error(Tag, "RestoreData error: " + e.ToString());
            }
        }

        private void ExecuteOnlineOpen()
        {
            TaskManager.Instance.Execute(new DriveTaskImplementor
            (
                delegate
                {
                    // 1. create folder
                    File dir = CreateDirectory();
                    // 2. upload a docx
                    Drive drive = BuildDrive();
                    File temp = null;
                    string filePath = context.ExternalCacheDir.AbsolutePath + "/test.docx";
                    Java.IO.File io = new Java.IO.File(filePath);
                    FileContent fileContent = new FileContent(MimeType.GetMimeType(io), io);
                    File content = new File().SetFileName(io.Name).SetMimeType(MimeType.GetMimeType(io)).SetParentFolder( new SingletonList<string>(dir.Id));
                    try
                    {
                        Drive.Files.Create rquest = drive.InvokeFiles().InvokeCreate(content, fileContent);
                        rquest.MediaHttpUploader.SetDirectUploadEnabled(true);
                        temp = (File)rquest.Execute();
                    }
                    catch (Exception e)
                    {
                        Logger.Error(Tag, "upload file error: " + e.ToString());
                        SendHandleMessage(Resource.Id.drive_online_open_btn, Fail);
                        return;
                    }
                    OpenOnlineFile(temp);
                }
            ));
        }

        private void OpenOnlineFile(File file)
        {
            string onlineViewLink = file.OnLineViewLink;
            if (TextUtils.IsEmpty(onlineViewLink))
            {
                Logger.Error(Tag, "File onLineViewLink is empty!");
                SendHandleMessage(Resource.Id.drive_online_open_btn, Fail);
                return;
            }
            Intent intent = new Intent(Activity, typeof(WebViewActivity));
            intent.PutExtra("url", onlineViewLink);
            Logger.Debug(Tag, "open file: " + file.FileName + " in WebView, url is " + onlineViewLink);
        }

        public override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == WebViewBackRefresh)
            {
                SendHandleMessage(Resource.Id.drive_online_open_btn, Success);
            }
        }
    }

    public class SingletonList<T> : IList<T>
    {
        private readonly T _item;

        public SingletonList(T item)
        {
            _item = item;
        }

        public IEnumerator<T> GetEnumerator()
        {
            yield return _item;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(T item)
        {
            throw new NotSupportedException("Add not supported.");
        }

        public void Clear()
        {
            throw new NotSupportedException("Clear not supported.");
        }

        public bool Contains(T item)
        {
            if (item == null) return _item == null;

            return item.Equals(_item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null) throw new ArgumentNullException("array");

            array[arrayIndex] = _item;
        }

        public bool Remove(T item)
        {
            throw new NotSupportedException("Remove not supported.");
        }

        public int Count
        {
            get { return 1; }
        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        public int IndexOf(T item)
        {
            return Contains(item) ? 0 : -1;
        }

        public void Insert(int index, T item)
        {
            throw new NotSupportedException("Insert not supported.");
        }

        public void RemoveAt(int index)
        {
            throw new NotSupportedException("RemoveAt not supported.");
        }

        public T this[int index]
        {
            get
            {
                if (index == 0) return _item;

                throw new IndexOutOfRangeException();
            }
            set { throw new NotSupportedException("Set not supported."); }
        }
    }
}