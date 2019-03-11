namespace BEL.DataAccessLayer
{
    using Newtonsoft.Json;
    using BEL.CommonDataContract;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Web;
    using Microsoft.SharePoint.Client;

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
        /// Generates the file bytes.
        /// </summary>
        /// <param name="fileNameList">The file name list.</param>
        /// <param name="mvcBaseUrl">The MVC base URL.</param>
        /// <returns>return List of files</returns>
        public static List<FileDetails> GenerateFileBytes(string fileNameList, string mvcBaseUrl)
        {
            if (fileNameList != null)
            {
                List<FileDetails> fileList = JsonConvert.DeserializeObject<List<FileDetails>>(fileNameList);
                if (fileList != null)
                {
                    for (int i = 0; i < fileList.Count; i++)
                    {
                        if (fileList[i].FileContent == null || fileList[i].FileContent.Length == 0)
                        {
                            fileList[i].FileURL = "~/" + fileList[i].FileURL.Replace(mvcBaseUrl, string.Empty).Trim('/');
                            fileList[i].FileContent = FileListHelper.DownloadFileBytes(fileList[i].FileURL);
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
                    return fileList.Select(x => x.FileName).ToList();
                }
            }
            return new List<string>();
        }

        /// <summary>
        /// Downloads the file bytes.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>File Bytes</returns>
        private static byte[] DownloadFileBytes(string url)
        {
            try
            {
                return System.IO.File.ReadAllBytes(System.Web.HttpContext.Current.Server.MapPath(url));
            }
            catch
            {
                return null;
            }
        }
    }
}