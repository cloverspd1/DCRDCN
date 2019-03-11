namespace BEL.DCRDCNWorkflow.Common
{
    using BEL.CommonDataContract;
    using BEL.DCRDCNWorkflow.BusinessLayer;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Web;

    /// <summary>
    /// File List Helper
    /// </summary>
    public static class FileListHelper
    {
        /// <summary>
        /// Gets the base URL.
        /// </summary>
        /// <value>
        /// The base URL.
        /// </value>
        public static string BaseUrl
        {
            get
            {
                return string.Format("{0}://{1}", System.Web.HttpContext.Current.Request.Url.Scheme, System.Web.HttpContext.Current.Request.Url.Authority);
            }
        }

        /// <summary>
        /// Gets the Application Base Url.
        /// </summary>
        /// <value>
        /// The Application Base Url.
        /// </value>
        public static string ApplicatinBaseUrl
        {
            get
            {
                var request = System.Web.HttpContext.Current.Request;
                string baseUrl = string.Format("{0}://{1}{2}", request.Url.Scheme, request.Url.Authority, HttpRuntime.AppDomainAppVirtualPath == "/" ? string.Empty : HttpRuntime.AppDomainAppVirtualPath);
                return baseUrl;
            }
        }

        /// <summary>
        /// Generates the file bytes.
        /// </summary>
        /// <param name="fileNameList">The file name list.</param>
        /// <returns>file list</returns>
        public static List<FileDetails> GenerateFileBytes(string fileNameList)
        {
            if (fileNameList != null)
            {
                List<FileDetails> fileList = JsonConvert.DeserializeObject<List<FileDetails>>(fileNameList);
                if (fileList != null)
                {
                    fileList = fileList.Where(f => f.Status == FileStatus.New || f.Status == FileStatus.Delete).ToList();
                    for (int i = 0; i < fileList.Count; i++)
                    {
                        if (fileList[i].FileContent == null || fileList[i].FileContent.Length == 0)
                        {
                            //if (fileList[i].FileURL.StartsWith(FileListHelper.BaseUrl))
                            //{
                            //    fileList[i].FileURL = "~/" + fileList[i].FileURL.Replace(FileListHelper.BaseUrl, string.Empty).Trim('/');
                            //}
                            if (fileList[i].FileURL.StartsWith(FileListHelper.ApplicatinBaseUrl))
                            {
                                fileList[i].FileURL = "~/" + fileList[i].FileURL.Replace(FileListHelper.ApplicatinBaseUrl, string.Empty).Trim('/');
                            }

                            if (fileList[i].FileURL.Contains("/Uploads/") || fileList[i].FileURL.Contains("/Sample/"))
                            {
                                fileList[i].FileContent = FileListHelper.DownloadFileBytes(fileList[i].FileURL);
                            }
                            else
                            {
                                fileList[i].FileContent = CommonBusinessLayer.Instance.DownloadFile(fileList[i].FileURL, "NPD");
                            }


                        }
                    }
                    fileList.RemoveAll(f => f.FileContent == null && f.Status != FileStatus.Delete);
                }
                return fileList;
            }
            else
            {
                return new List<FileDetails>();
            }
        }

        /// <summary>
        /// Gets the file names.
        /// </summary>
        /// <param name="fileNameList">The file name list.</param>
        /// <returns>List of filenames</returns>
        public static List<string> GetFileNames(string fileNameList)
        {
            if (fileNameList != null)
            {
                List<FileDetails> fileList = JsonConvert.DeserializeObject<List<FileDetails>>(fileNameList);
                if (fileList != null)
                {
                    return fileList.Where(p => p.Status != FileStatus.Delete).Select(x => x.FileName).ToList();
                }
            }
            return new List<string>();
        }

        /// <summary>
        /// Downloads the file bytes.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>File Bytes</returns>
        public static byte[] DownloadFileBytes(string url)
        {
            try
            {
                return System.IO.File.ReadAllBytes(System.Web.HttpContext.Current.Server.MapPath(url));
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}