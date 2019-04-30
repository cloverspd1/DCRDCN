namespace BEL.DCRDCNWorkflow
{
    using BEL.CommonDataContract;
    using BEL.DCRDCNWorkflow.BusinessLayer;
    using BEL.DCRDCNWorkflow.Common;
    using Microsoft.SharePoint.Client;
    using Microsoft.SharePoint.Client.UserProfiles;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Reflection;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Mvc.Html;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Drawing.Drawing2D;
    using System.Drawing.Text;


    /// <summary>
    /// Base Controller
    /// </summary>
    public class BaseController : Controller
    {
        /// <summary>
        /// Gets or sets the name of the SP user.
        /// </summary>
        /// <value>
        /// The name of the SP user.
        /// </value>
        private string SPUser
        {
            get
            {

                if (this.Session["SPUser"] != null)
                {
                    return Convert.ToString(this.Session["SPUser"]);
                }
                return null;
            }

            set
            {
                this.Session["SPUser"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the SP user.
        /// </summary>
        /// <value>
        /// The name of the SP user.
        /// </value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "spContext")]
        private UserDetails SPCurrentUser
        {
            get
            {
                if (!this.EnvironmentLive)
                {
                    if (this.Session["SPCurrentUser"] != null)
                    {
                        return (UserDetails)this.Session["SPCurrentUser"];
                    }
                    return null;
                }
                else
                {
                    var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
                    HttpCookie aCookie = Request.Cookies["SPCacheKey"];
                    Logger.Info(" SPCacheKey as Session Key " + aCookie.Value);
                    if (!string.IsNullOrEmpty(aCookie.Value))
                    {
                        if (this.Session[aCookie.Value] != null)
                        {
                            return (UserDetails)this.Session[aCookie.Value];
                        }
                    }
                    return null;
                }
                //if (this.Session["SPCurrentUser"] != null)
                //{
                //    return (UserDetails)this.Session["SPCurrentUser"];
                //}

            }

            set
            {
                if (!this.EnvironmentLive)
                {
                    this.Session["SPCurrentUser"] = value;
                }
                else
                {
                    var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
                    HttpCookie aCookie = Request.Cookies["SPCacheKey"];
                    Logger.Info(" SPCacheKey as Session Key " + aCookie.Value);
                    if (!string.IsNullOrEmpty(aCookie.Value))
                    {
                        this.Session[aCookie.Value] = value;
                    }
                    aCookie.Expires.AddDays(1);
                }
            }
        }

        public UserDetails CurrentUser
        {
            get
            {
                if (this.SPCurrentUser == null)
                {
                    if (!this.EnvironmentLive)
                    {
                        this.SPCurrentUser = new UserDetails();
                        //this.SPCurrentUser.UserId = "49";
                        // this.SPCurrentUser.UserId = "6"; //cloverspd1
                        //this.SPCurrentUser.UserId = "33"; //Dhanraj Mane
                         this.SPCurrentUser.UserId = "32"; //dcrdcnuser2
                        //this.SPCurrentUser.UserId = "24"; //dcrdcnuser1 sharepoint notificaiton
                        //this.SPCurrentUser.UserId = "37"; //collabappsupport3
                        // this.SPCurrentUser.UserId = "26"; //collabappsupport-2
                        //this.SPCurrentUser.UserId = "25"; //collabappsupport-1
                        //this.SPCurrentUser.UserId = "27";// cloverspd
                        // this.SPCurrentUser.UserId = "38";// murudkara tushar
                        //this.SPCurrentUser.UserId = "29";// patil bhushan
                        // this.SPCurrentUser.UserId = "28";// Ramdas Kuthal
                         // this.SPCurrentUser.UserId = "36"; //tmsuser1
                        //this.SPCurrentUser.UserId = "35"; //tmsuser2
                        //this.SPCurrentUser.UserId = "31"; //dcrdcnuser3
                        //this.SPCurrentUser.UserId = "46"; //deepak kamble
                        // this.SPCurrentUser.UserId = "153"; //jyotishkumar mohit
                        //this.SPCurrentUser.UserId = "179"; //jyotishkumar mohit
                        // this.SPCurrentUser.UserId = "48"; //Arup dey
                        //this.SPCurrentUser.UserId = "53"; //devinder behal
                        // this.SPCurrentUser.UserId = "49"; //Chandra Singh
                        // this.SPCurrentUser.UserId = "55"; //Ambikesh
                        //this.SPCurrentUser.UserId = "47"; //deepak mumbaiker
                        //this.SPCurrentUser.UserId = "167"; //Shailesh saawant

                       // this.SPCurrentUser.UserId = "60"; //Arshad Shaikh
                        //this.SPCurrentUser.UserId = "79"; //Amit Shetty

                        // this.SPCurrentUser.UserId = "24";
                        // this.SPCurrentUser.UserId = "220";
                        //this.SPCurrentUser.UserId = "24";
                        this.SPCurrentUser = DCRBusinessLayer.Instance.getUSerDetail(Convert.ToInt32(this.SPCurrentUser.UserId));   ///CommonBusinessLayer.Instance.GetLoginUserDetail(this.SPCurrentUser.UserId);
                        ////User user = CommonBusinessLayer.Instance.getCurrentUser(this.SPCurrentUser.UserId);

                        //this.SPCurrentUser.UserEmail = user.Email;
                        //this.SPCurrentUser.LoginName = user.LoginName;
                        //this.SPUser = "dcrdcnuser2@bajajelectricals.com";
                        //this.SPUser = "dcrdcnuser3@bajajelectricals.com";
                        //this.SPUser = "tmsuser1@bajajelectricals.com";
                        //this.SPUser = "tmsuser2@bajajelectricals.com";

                    }
                    else
                    {
                        try
                        {
                            User spUser = null;
                            var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
                            using (var clientContext = spContext.CreateUserClientContextForSPHost())
                            {
                                if (clientContext != null)
                                {
                                    this.SPCurrentUser = new UserDetails();
                                    spUser = clientContext.Web.CurrentUser;
                                    var peopleManager = new PeopleManager(clientContext);
                                    PersonProperties personProperties = peopleManager.GetMyProperties();

                                    clientContext.Load(spUser, user => user.Id, user => user.LoginName);
                                    clientContext.Load(personProperties);
                                    clientContext.ExecuteQuery();
                                    //this.SPCurrentUser = CommonBusinessLayer.Instance.GetLoginUserDetail(spUser.Id.ToString());
                                    this.SPCurrentUser.UserId = spUser.Id.ToString();
                                    this.SPCurrentUser.LoginName = spUser.LoginName;
                                    this.SPCurrentUser.FullName = personProperties.DisplayName;
                                    this.SPCurrentUser.UserEmail = personProperties.Email;
                                    this.SPCurrentUser.Department = personProperties.UserProfileProperties["Department"];
                                    //this.SPCurrentUser.UserId = spUser.Id.ToString();
                                    ////this.SPCurrentUser.CurrentSPUser = spUser;
                                    //this.SPCurrentUser.LoginName = spUser.LoginName;

                                    Logger.Info("Logged in user Name : " + spUser.LoginName);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Error("Invaild Login User");
                            Logger.Error(ex);
                            System.Web.HttpContext.Current.ClearError();
                            System.Web.HttpContext.Current.Response.Clear();
                            //System.Web.HttpContext.Current.Response.Redirect("~/Master/SessionTimeOutError", true);
                            System.Web.HttpContext.Current.Response.End();
                        }

                    }
                }
                return this.SPCurrentUser;
            }
        }

        /// <summary>
        /// Gets the current email identifier.
        /// </summary>
        /// <value>
        /// The current email identifier.
        /// </value>
        public string CurrentEmailId
        {
            get
            {
                if (string.IsNullOrEmpty(this.SPUser))
                {
                    if (!this.EnvironmentLive)
                    {
                        this.SPUser = "dcrdcnuser1@bajajelectricals.com";
                    }
                    this.SPUser = string.Empty;
                    ViewBag.UserEmail = this.SPUser;

                }
                return this.SPUser;
            }
        }

        /// <summary>
        /// Gets the sp host.
        /// </summary>
        /// <value>
        /// The sp host.
        /// </value>
        public string SPHost
        {
            get
            {
                try
                {
                    Uri spHostUrl = SharePointContext.GetSPHostUrl(System.Web.HttpContext.Current.Request);
                    if (spHostUrl != null)
                    {
                        return string.Format("{0}://{1}{2}", spHostUrl.Scheme, spHostUrl.Authority, Url.Content("~"));
                    }
                }
                catch
                {
                }
                return string.Empty;
            }
        }

        /// <summary>
        /// Gets the sp host URL.
        /// </summary>
        /// <value>
        /// The sp host URL.
        /// </value>
        public string SPHostUrl
        {
            get
            {
                try
                {
                    Uri spHostUrl = SharePointContext.GetSPHostUrl(System.Web.HttpContext.Current.Request);
                    if (spHostUrl != null)
                    {
                        return spHostUrl.ToString();
                    }
                }
                catch
                {
                }
                return string.Empty;
            }
        }

        /// <summary>
        /// Gets the Application Base Url.
        /// </summary>
        /// <value>
        /// The Application Base Url.
        /// </value>
        public string ApplicatinBaseUrl
        {
            get
            {
                var request = HttpContext.Request;
                string baseUrl = string.Format("{0}://{1}{2}", request.Url.Scheme, request.Url.Authority, HttpRuntime.AppDomainAppVirtualPath == "/" ? string.Empty : HttpRuntime.AppDomainAppVirtualPath);
                return baseUrl;
            }
        }

        /// <summary>
        /// Gets a value indicating whether [environment live].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [environment live]; otherwise, <c>false</c>.
        /// </value>
        public bool EnvironmentLive
        {
            get
            {
                bool environmentLive = false;
                bool.TryParse(Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["EnvironmentLive"]), out environmentLive);
                return environmentLive;
            }
        }

        /// <summary>
        /// Gets the base URL.
        /// </summary>
        /// <value>
        /// The base URL.
        /// </value>
        public string BaseUrl
        {
            get
            {
                return string.Format("{0}://{1}{2}", Request.Url.Scheme, Request.Url.Authority, Url.Content("~"));
            }
        }

        /// <summary>
        /// Actions the name of the status.
        /// </summary>
        /// <param name="status">The status.</param>
        /// <returns>Button Status</returns>
        public string ActionStatusName(ButtonActionStatus status)
        {
            return status.ToString();
        }

        /// <summary>
        /// Uploads the file.
        /// </summary>
        /// <param name="qqfile">The qqfile.</param>
        /// <returns>
        /// Upload File Status
        /// </returns>
        public JsonResult UploadFile(string qqfile)
        {
            var stream = this.Request.InputStream;
            //Security Testing Fixes start
            //check server side valid file extension
            if (!Helper.IsValidFileExtension(System.IO.Path.GetExtension(qqfile).Replace(".", "")))
            {
                return this.Json(new ActionStatus() { IsSucceed = false, Message = System.IO.Path.GetFileName(qqfile) + " type of not allowed." });
            }
            //Security Testing Fixes End

            string id = Guid.NewGuid().ToString() + System.IO.Path.GetExtension(qqfile);
            string fileName = System.IO.Path.GetFileNameWithoutExtension(qqfile) + System.IO.Path.GetExtension(qqfile);
            if (string.IsNullOrEmpty(this.Request["qqfile"]))
            {
                // IE Fix
                HttpPostedFileBase postedFile = this.Request.Files[0];
                stream = postedFile.InputStream;
            }
            byte[] fileData = null;
            using (var binaryReader = new BinaryReader(stream))
            {
                fileData = binaryReader.ReadBytes((int)stream.Length);
            }
            //Stamping Logic start

            //Stamping Logic End
            System.IO.File.WriteAllBytes(Server.MapPath("~/Uploads/" + id), fileData);
            //return this.Json(new FileDetails() { FileId = id, FileName = fileName, FileURL = this.BaseUrl.Trim('/') + Url.Content("~/Uploads/" + id), Status = FileStatus.New });
            return this.Json(new FileDetails() { FileId = id, FileName = fileName, FileURL = this.ApplicatinBaseUrl.Trim('/') + Url.Content("/Uploads/" + id), Status = FileStatus.New });
        }

        /// <summary>
        /// Uploads the file.
        /// </summary>
        /// <param name="qqfile">The qqfile.</param>
        /// <returns>
        /// Upload File Status
        /// </returns>
        public JsonResult UploadFileWithStamp(string qqfile)
        {
            var stream = this.Request.InputStream;
            //Security Testing Fixes start
            //check server side valid file extension
            if (!Helper.IsValidFileExtension(System.IO.Path.GetExtension(qqfile).Replace(".", "")))
            {
                return this.Json(new ActionStatus() { IsSucceed = false, Message = System.IO.Path.GetFileName(qqfile) + " type of not allowed." });
            }
            //Security Testing Fixes End

            string id = Guid.NewGuid().ToString() + System.IO.Path.GetExtension(qqfile);
            string fileName = System.IO.Path.GetFileNameWithoutExtension(qqfile) + System.IO.Path.GetExtension(qqfile);
            if (string.IsNullOrEmpty(this.Request["qqfile"]))
            {
                // IE Fix
                HttpPostedFileBase postedFile = this.Request.Files[0];
                stream = postedFile.InputStream;
            }
            byte[] fileData = null;
            using (var binaryReader = new BinaryReader(stream))
            {
                fileData = binaryReader.ReadBytes((int)stream.Length);
            }

            //Stamping Logic start

            string extension = System.IO.Path.GetExtension(qqfile).ToLower();
            if (!string.IsNullOrEmpty(extension) && (extension.Equals(".jpg") || extension.Equals(".jpeg") || extension.Equals(".png") || extension.Equals(".gif") || extension.Equals(".tif") || extension.Equals(".tiff") || extension.Equals(".bmp")))
            {
                using (MemoryStream ms = new MemoryStream(fileData))
                {
                    Image sourceImg = Image.FromStream(ms);
                    ////Create Image by passing image path 
                    byte[] imageBytes = MergeImages(sourceImg, Server.MapPath(Url.Content("~/images/BajajStamp.png")));
                    Logger.Info("Two Images has been merged");
                    try
                    {
                        if (imageBytes != null && imageBytes.Length >0)
                        {
                            ////Add Date And Time on Merged Image
                            Bitmap bitmap = new System.Drawing.Bitmap(new MemoryStream(imageBytes));
                            byte[] output = createImage(bitmap);
                            fileData = output;
                            bitmap.Dispose();
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.Error("Error while attch stamp on image :" + e.StackTrace);
                    }
                }
            }

            //Stamping Logic End
            System.IO.File.WriteAllBytes(Server.MapPath("~/Uploads/" + id), fileData);
            //return this.Json(new FileDetails() { FileId = id, FileName = fileName, FileURL = this.BaseUrl.Trim('/') + Url.Content("~/Uploads/" + id), Status = FileStatus.New });
            return this.Json(new FileDetails() { FileId = id, FileName = fileName, FileURL = this.ApplicatinBaseUrl.Trim('/') + Url.Content("/Uploads/" + id), Status = FileStatus.New });
        }

        public byte[] createImage(Bitmap bitMapImage)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                if (bitMapImage != null)
                {
                    Graphics graphicImage = Graphics.FromImage(bitMapImage);

                    ////Smooth graphics is nice.
                    graphicImage.SmoothingMode = SmoothingMode.AntiAlias;

                    ////set Font style for string to be wriiten on image
                    Font font = new Font("Arial", 10, FontStyle.Bold);
                    graphicImage.DrawString(DateTime.Now.ToString("dd-MM-yy"), font, Brushes.Red, new PointF(90, 96));                //add date
                    graphicImage.DrawString(DateTime.Now.ToString("H:mm:ss"), font, Brushes.Red, new PointF(90, 111));                //add time

                    bitMapImage.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                    bitMapImage.Dispose();
                    font.Dispose();
                }
                return stream.ToArray();
            }
        }

        /// <summary>
        /// Merges two images
        /// </summary>
        /// <param name="inputImage1">filename with complete path of the first image file.</param>
        /// <param name="inputImage2">filname with complete path of the second image file.</param>
        private byte[] MergeImages(Image sourceImagePath, string stampImagePath)
        {
            Image sourceImage = sourceImagePath;
            Image stampImage = Image.FromFile(stampImagePath);
            using (MemoryStream stream = new MemoryStream())
            {
                if (sourceImage != null && stampImage != null)
                {
                    ////get max width and max height of both images
                    int width = Math.Max(sourceImage.Width, stampImage.Width);
                    int height = Math.Max(sourceImage.Height, stampImage.Height);

                    ////set width and height for output image
                    using (Bitmap outputImage = new Bitmap(width, height))
                    {
                        Graphics graphics = Graphics.FromImage(outputImage);

                        ////first draw the source image on base
                        graphics.DrawImage(sourceImage, new Point(0, 0));

                        ////draw 2nd image over first image at 0,0 position on image
                        Point stampPoint = new Point(0, 0);
                        graphics.DrawImage(stampImage, stampPoint);

                        graphics.Dispose();
                        sourceImage.Dispose();
                        stampImage.Dispose();

                        outputImage.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                    }
                }

                return stream.ToArray();
            }
        }

        /// <summary>
        /// Removes the upload file.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>
        /// Upload File Status
        /// </returns>
        public JsonResult RemoveUploadFile(string id)
        {
            if (System.IO.File.Exists(Server.MapPath("~/Uploads/" + id)))
            {
                try
                {
                    System.IO.File.Delete(Server.MapPath("~/Uploads/" + id));
                }
                catch
                {
                }
            }
            return this.Json(new FileDetails() { FileId = id });
        }


        /// <summary>
        /// Downloads the file.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="currentLocation">The current location.</param>
        /// <param name="applicationName">Name of the application.</param>
        /// <returns>
        /// file result
        /// </returns>
        public ActionResult DownloadFile(string url, string currentLocation, string applicationName)
        {
            try
            {
                byte[] fileData = null;
                if (url != null)
                {
                    if (url.Contains("/Uploads/") || url.Contains("/Sample/"))
                    {
                        url = url.Replace(this.ApplicatinBaseUrl, string.Empty);
                        url = url.Replace("~", string.Empty);
                        fileData = System.IO.File.ReadAllBytes(Server.MapPath("~/" + url));
                    }
                    else
                    {
                        fileData = CommonBusinessLayer.Instance.DownloadFile(url, applicationName);
                    }
                }
                return this.File(fileData, "application/octet-stream", Path.GetFileName(url));
            }
            catch
            {
                return this.Redirect(currentLocation);
            }
        }

        /// <summary>
        /// Verifies the download file.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="applicationName">Name of the application.</param>
        /// <returns>
        /// Verify file true/false
        /// </returns>
        public JsonResult VerifyDownloadFile(string url, string applicationName)
        {
            try
            {
                if (url != null)
                {
                    if (url.Contains("/Uploads/") || url.Contains("/Sample/"))
                    {
                        Logger.Info("File URL:" + url);
                        Logger.Info("ApplicatinBaseUrl :" + this.ApplicatinBaseUrl);
                        url = url.Replace(this.ApplicatinBaseUrl, string.Empty);
                        url = url.Replace("~", string.Empty);
                        Logger.Info("File URL after remove:" + url);
                        System.IO.File.ReadAllBytes(Server.MapPath("~/" + url));
                        return this.Json(new { Status = true });
                    }
                    else
                    {

                        CommonBusinessLayer.Instance.DownloadFile(url, applicationName);
                    }
                }
                return this.Json(new { Status = true });
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return this.Json(new { Status = false });
            }
        }

        /// <summary>
        /// Downloads the CSV.
        /// </summary>
        /// <param name="dt">The dt.</param>
        /// <param name="filename">The filename.</param>
        /// <returns>File Result</returns>
        protected FileResult DownloadCSV(DataTable dt, string filename)
        {
            StringBuilder sb = new StringBuilder();
            if (dt != null)
            {
                foreach (DataColumn col in dt.Columns)
                {
                    sb.Append(col.ColumnName + ',');
                }
                sb.Remove(sb.Length - 1, 1);
                sb.Append(Environment.NewLine);
                foreach (DataRow row in dt.Rows)
                {
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        sb.Append(Regex.Replace(Convert.ToString(row[i]).Replace(",", ";"), "<.*?>", string.Empty) + ",");
                    }

                    sb.Append(Environment.NewLine);
                }
            }
            return this.File(Encoding.UTF8.GetBytes(sb.ToString()), "text/csv", filename);
        }

        /// <summary>
        /// Gets the temporary data.
        /// </summary>
        /// <typeparam name="T">type of object</typeparam>
        /// <param name="key">The key.</param>
        /// <returns>get from tempdata</returns>
        public T GetTempData<T>(TempKeys key) where T : new()
        {
            if (this.TempData[key.ToString()] == null)
            {
                this.TempData[key.ToString()] = new T();
            }
            this.TempData.Keep(key.ToString());
            return (T)this.TempData[key.ToString()];
        }

        /// <summary>
        /// Gets the temporary data.
        /// </summary>
        /// <typeparam name="T">type of object</typeparam>
        /// <param name="key">The key.</param>
        /// <returns>get from tempdata</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        public T GetTempData<T>(string key) where T : new()
        {
            if (!string.IsNullOrEmpty(key) && this.TempData[key.ToString()] == null)
            {
                this.TempData[key.ToString()] = new T();
            }
            this.TempData.Keep(key.ToString());
            return (T)this.TempData[key.ToString()];
        }

        /// <summary>
        /// Sets the temporary data.
        /// </summary>
        /// <typeparam name="T">type of object</typeparam>
        /// <param name="key">The key.</param>
        /// <param name="obj">The object.</param>
        public void SetTempData<T>(TempKeys key, T obj)
        {
            this.TempData[key.ToString()] = obj;
            this.TempData.Keep(key.ToString());
        }

        /// <summary>
        /// Sets the temporary data.
        /// </summary>
        /// <typeparam name="T">type of object</typeparam>
        /// <param name="key">The key.</param>
        /// <param name="obj">The object.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        public void SetTempData<T>(string key, T obj)
        {
            this.TempData[key.ToString()] = obj;
            this.TempData.Keep(key.ToString());
        }

        /// <summary>
        /// Processes the CSV.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <returns>DataTable of CSV</returns>
        public DataTable ProcessCSV(byte[] array)
        {
            string line = string.Empty;
            string[] strArray;
            DataTable dt = new DataTable();
            DataRow row;
            Regex r = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
            using (MemoryStream ms = new MemoryStream(array))
            {
                StreamReader sr = new StreamReader(ms);
                line = sr.ReadLine();
                strArray = r.Split(line);
                Array.ForEach(strArray, s => dt.Columns.Add(new DataColumn()));
                while ((line = sr.ReadLine()) != null)
                {
                    row = dt.NewRow();
                    row.ItemArray = r.Split(line);
                    dt.Rows.Add(row);
                }
                sr.Dispose();
                return dt;
            }
        }


        /// <summary>
        /// Gets the error message.
        /// </summary>
        /// <param name="resourceFile">The resource file.</param>
        /// <returns>
        /// message lsit
        /// </returns>
        protected List<string> GetErrorMessage(ResourceNames resourceFile)
        {
            List<string> errorList = this.GetErrorKeys();
            List<string> messageList = new List<string>();
            foreach (var v in errorList)
            {
                string errorText = this.GetResourceValue("Error_" + v, resourceFile);
                if (!string.IsNullOrEmpty(errorText))
                    messageList.Add(errorText);
                else
                    messageList.Add(v + " is required.");
            }
            return messageList;
        }

        /// <summary>
        /// Gets the file name list.
        /// </summary>
        /// <param name="sectionDetails">The section details.</param>
        /// <param name="type">The type.</param>
        /// <returns>ISection Detail</returns>
        protected ISection GetFileNameList(ISection sectionDetails, Type type)
        {
            if (sectionDetails == null)
            {
                return null;
            }
            dynamic meetingsectionDetails = Convert.ChangeType(sectionDetails, type);
            meetingsectionDetails.FileNameList = string.Empty;
            if (meetingsectionDetails.Files != null && meetingsectionDetails.Files.Count > 0)
            {
                meetingsectionDetails.FileNameList = JsonConvert.SerializeObject(meetingsectionDetails.Files);
            }
            return meetingsectionDetails;
        }


        /// <summary>
        /// Gets the task list.
        /// </summary>
        /// <typeparam name="T">type of tempdata list</typeparam>
        /// <param name="sectionDetails">The section details.</param>
        /// <param name="type">The type.</param>
        /// <param name="key">The key.</param>
        protected void SetTaskList<T>(ISection sectionDetails, Type type, TempKeys key) where T : new()
        {
            if (sectionDetails == null)
            {
                return;
            }
            dynamic meetingsectionDetails = Convert.ChangeType(sectionDetails, type);
            List<T> list = new List<T>();
            foreach (var task in meetingsectionDetails.Tasks)
            {
                list.Add((T)task);
            }
            this.SetTempData(key, list);
        }

        /// <summary>
        /// Sets the task list.
        /// </summary>
        /// <typeparam name="T">type of tempdata list</typeparam>
        /// <param name="sectionDetails">The section details.</param>
        /// <param name="type">The type.</param>
        /// <param name="key">The key.</param>
        /// <param name="typeofTask">The typeof task.</param>
        protected void SetTaskList<T>(ISection sectionDetails, Type type, TempKeys key, string typeofTask) where T : new()
        {
            if (sectionDetails == null)
            {
                return;
            }
            dynamic meetingsectionDetails = Convert.ChangeType(sectionDetails, type);
            List<T> list = new List<T>();

            foreach (var task in meetingsectionDetails.Tasks)
            {
                if (task.TypeofTask == typeofTask)
                {
                    list.Add((T)task);
                }
            }
            this.SetTempData(key, list);
        }

        /// <summary>
        /// Determines whether [is valid date] [the specified dt from].
        /// </summary>
        /// <param name="dtFrom">The dt from.</param>
        /// <param name="dtTo">The dt to.</param>
        /// <returns>return true/false based on condition</returns>
        protected bool IsValidDate(DateTime dtFrom, DateTime dtTo)
        {
            return dtFrom.Date > dtTo.Date;
        }

        /// <summary>
        /// Validates the users exist.
        /// </summary>
        /// <param name="emailList">The email list.</param>
        /// <returns>Invalid User List</returns>
        protected List<string> ValidateUsersExist(List<string> emailList)
        {
            List<string> tmpEmailList = new List<string>();
            if (emailList != null && emailList.Count > 0)
            {
                //to cater issue of multiple value in actionBy field BY Rushit start
                foreach (string email in emailList)
                {
                    if (email.Contains(","))
                    {
                        tmpEmailList.AddRange(email.Split(','));
                    }
                    else
                    {
                        tmpEmailList.Add(email);
                    }
                }
                //to cater issue of multiple value in actionBy field BY Rushit end
                List<string> invalidUsers = CommonBusinessLayer.Instance.ValidateUsers(tmpEmailList.Distinct().ToList());
                if (invalidUsers != null)
                {
                    return invalidUsers;
                }

            }
            return new List<string>();
        }

        /// <summary>
        /// Gets the save data items.
        /// </summary>
        /// <param name="userID">The user email.</param>
        /// <param name="actionStatus">The action status.</param>
        /// <param name="buttonCaption">The button caption.</param>
        /// <returns>
        /// Dictionary Items
        /// </returns>
        protected Dictionary<string, string> GetSaveDataDictionary(string userID, string actionStatus, string buttonCaption)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param[Parameter.USEREID] = userID;
            if (!string.IsNullOrEmpty(buttonCaption))
            {
                param[Parameter.ACTIONPER] = buttonCaption + "|" + actionStatus;
            }
            else
            {
                param[Parameter.ACTIONPER] = actionStatus;
            }
            return param;
        }

        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <param name="status">The status.</param>
        /// <param name="resourFilName">Name of the resour fil.</param>
        /// <returns>Action Status</returns>
        public ActionStatus GetMessage(ActionStatus status, ResourceNames resourFilName)
        {
            if (status != null)
            {
                for (int i = 0; i < status.Messages.Count; i++)
                {
                    string value = this.GetResourceValue(status.Messages[i], resourFilName);
                    if (!string.IsNullOrEmpty(value))
                    {
                        status.Messages[i] = value;
                    }
                }
            }
            return status;
        }

        /// <summary>
        /// Gets the resource value.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="resourFilName">Name of the resour fil.</param>
        /// <returns>Resource Value</returns>
        public string GetResourceValue(string key, ResourceNames resourFilName)
        {
            return Convert.ToString(HttpContext.GetGlobalResourceObject(resourFilName.ToString(), key, System.Threading.Thread.CurrentThread.CurrentUICulture));
        }

        /// <summary>
        /// Remove Unused Files.
        /// </summary>
        private void RemoveUnusedFiles()
        {
            Task.Factory.StartNew(async () =>
            {
                await Task.Delay(1000);
                try
                {
                    DirectoryInfo info = new DirectoryInfo(Server.MapPath("~/Uploads/"));
                    FileInfo[] files = info.GetFiles().Where(p => DateTime.Now.Subtract(p.LastAccessTime).Minutes >= 1440).ToArray();
                    foreach (FileInfo file in files)
                    {
                        try
                        {
                            file.Delete();
                        }
                        catch
                        {
                        }
                    }
                }
                catch
                {
                }
            });
        }

        /// <summary>
        /// Gets the file name list from current approver.
        /// </summary>
        /// <param name="sectionDetails">The section details.</param>
        /// <param name="type">The type.</param>
        /// <returns>I Section</returns>
        protected ISection GetFileNameListFromCurrentApprover(ISection sectionDetails, Type type)
        {
            if (sectionDetails == null)
            {
                return null;
            }
            dynamic meetingsectionDetails = Convert.ChangeType(sectionDetails, type);
            meetingsectionDetails.FileNameList = string.Empty;
            if (meetingsectionDetails.CurrentApprover != null && meetingsectionDetails.CurrentApprover.Files != null && meetingsectionDetails.CurrentApprover.Files.Count > 0)
            {
                meetingsectionDetails.FileNameList = JsonConvert.SerializeObject(meetingsectionDetails.CurrentApprover.Files);
            }
            return meetingsectionDetails;
        }

        /// <summary>
        /// Validates the state of the model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>true or false</returns>
        protected bool ValidateModelState(ISection model)
        {
            if (model != null && model.ActionStatus == ButtonActionStatus.SaveAsDraft)
            {
                List<string> listOfError = this.GetErrorKeys();
                ModelState.Clear();
                foreach (PropertyInfo prop in model.GetType().GetProperties())
                {
                    if (prop.GetCustomAttribute<RequiredOnDraft>() != null && listOfError.Contains(prop.Name))
                    {
                        ModelState.AddModelError(prop.Name, prop.Name);
                    }
                }
            }
            else if (model != null && model.ActionStatus == ButtonActionStatus.SendBack)
            {
                //List<string> listOfError = this.GetErrorKeys();
                ModelState.Clear();
                foreach (PropertyInfo prop in model.GetType().GetProperties())
                {
                    // Here, check if required on send back attribute is true and the respective field is null then throw validation.
                    if (prop.GetCustomAttribute<RequiredOnSendBack>() != null && prop.GetValue(model) == null)
                    {
                        ModelState.AddModelError(prop.Name, prop.Name);
                    }
                }
            }
            else if (model != null && model.ActionStatus == ButtonActionStatus.Delegate)
            {
                //List<string> listOfError = this.GetErrorKeys();
                ModelState.Clear();
                foreach (PropertyInfo prop in model.GetType().GetProperties())
                {
                    // Here, check if required on delegate attribute is true and the respective field is null then throw validation.
                    if (prop.GetCustomAttribute<RequiredOnDelegate>() != null && prop.GetValue(model) == null)
                    {
                        ModelState.AddModelError(prop.Name, prop.Name);
                    }
                }
            }
            else if (model != null && model.ActionStatus == ButtonActionStatus.SaveAndNoStatusUpdate)
            {
                //List<string> listOfError = this.GetErrorKeys();
                ModelState.Clear();
                foreach (PropertyInfo prop in model.GetType().GetProperties())
                {
                    ModelState.Remove(prop.Name);
                }
            }
            return ModelState.IsValid;
        }

        /// <summary>
        /// Gets the error keys.
        /// </summary>
        /// <returns>List of error keys</returns>
        private List<string> GetErrorKeys()
        {
            List<string> listOfError = new List<string>();
            var listOfKeys = ModelState.Keys == null ? null : ModelState.Keys.ToArray();
            var listOfValues = ModelState.Values == null ? null : ModelState.Values.ToArray();
            if (listOfValues != null && listOfKeys != null && listOfKeys.Length == listOfValues.Length)
            {
                for (int i = 0; i < listOfKeys.Length; i++)
                {
                    var value = listOfValues[i];
                    if (value != null && value.Errors != null && value.Errors.Count > 0)
                    {
                        string[] k = listOfKeys[i].Split('.');
                        listOfError.Add(k[k.Length - 1]);
                    }
                }
            }
            return listOfError;
        }

        /// <summary>
        /// Sets the tran listinto temporary data.
        /// </summary>
        /// <typeparam name="T">type of tran object</typeparam>
        /// <param name="transitems">The transitems.</param>
        /// <param name="key">The key.</param>
        protected void SetTranListintoTempData<T>(List<ITrans> transitems, TempKeys key) where T : new()
        {
            if (transitems == null)
            {
                return;
            }

            List<T> list = new List<T>();
            foreach (var task in transitems)
            {
                list.Add((T)task);
            }
            this.SetTempData(key, list);
        }

        /// <summary>
        /// Sets the tran listinto temporary data.
        /// </summary>
        /// <typeparam name="T">type of tran object</typeparam>
        /// <param name="transitems">The transitems.</param>
        /// <param name="key">The key.</param>
        protected void SetTranListintoTempData<T>(List<ITrans> transitems, string key) where T : new()
        {
            if (transitems == null)
            {
                return;
            }

            List<T> list = new List<T>();
            foreach (var task in transitems)
            {
                list.Add((T)task);
            }
            this.SetTempData(key, list);
        }

        /// <summary>
        /// Get Form Id From Url Method
        /// </summary>
        /// <returns>Number value</returns>
        public int GetFormIdFromUrl()
        {
            try
            {
                string url = Request.UrlReferrer != null ? Request.UrlReferrer.ToString() : string.Empty;
                if (url.IndexOf('?') > -1)
                {
                    string querystring = url.Substring(url.IndexOf('?'));
                    System.Collections.Specialized.NameValueCollection parameters =
                       System.Web.HttpUtility.ParseQueryString(querystring);
                    if (parameters.HasKeys())
                    {
                        if (!string.IsNullOrEmpty(parameters.Get("id")))
                        {
                            return Convert.ToInt32(parameters.Get("id"));
                        }
                    }
                }
            }
            catch
            {
            }
            return 0;
        }

        [System.Web.Mvc.HttpPost]
        public JsonResult KeepSessionAlive()
        {
            try
            {
                Logger.Info("System Name: " + Dns.GetHostEntry(Request.ServerVariables["REMOTE_ADDR"]).HostName);
                return new JsonResult { Data = "Success" };
            }
            catch
            {
                return new JsonResult { Data = "Success" };
            }
        }

        /// <summary>
        /// Called when an action executing
        /// </summary>
        /// <param name="filterContext">Information about the current request and action.</param>
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!Request.IsAjaxRequest())
            {
                this.SPUser = null;
            }
            base.OnActionExecuting(filterContext);
            this.ViewData["BasePath"] = this.ApplicatinBaseUrl;
            this.ViewData["SPHostUrl"] = this.SPHostUrl;
            this.ViewData["SPHost"] = this.SPHost;
            this.RemoveUnusedFiles();

            ////ViewBag.UserEmail = this.CurrentUser.UserEmail;
            ////Security Fixes
            ////string hashKey = Helper.GenerateHashKey(System.Web.HttpContext.Current);
            ////****
            //HttpCookie cookie = new HttpCookie("HS");
            //cookie.Value = hashKey;
            //Response.Cookies.Add(cookie);
            ////****
            ////if (!this.EnvironmentLive)
            ////{
            ////    if (this.Session["SPCurrentUser"] == null && !Request.RawUrl.Contains("/Error"))
            ////    {
            ////        if (filterContext != null)
            ////        {
            ////            RedirectToAction("Error", "Master", new { msg = "Request Time Out. Please open request again" });
            ////            //below line commented on 19/12/2016 as error of timeout was coming
            ////            //  filterContext.Result = new RedirectResult("/Master/Error?msg='Request Time Out. Please open request again'");
            ////            return;
            ////        }

            ////    }
            ////}
            ////else
            ////{
            ////    Uri redirectUrl;

            ////    switch (SharePointContextProvider.CheckRedirectionStatus(filterContext.HttpContext, out redirectUrl))
            ////    {
            ////        case RedirectionStatus.Ok:
            ////            return;
            ////        case RedirectionStatus.ShouldRedirect:
            ////            filterContext.Result = new RedirectResult(redirectUrl.AbsoluteUri);
            ////            break;
            ////        case RedirectionStatus.CanNotRedirect:
            ////            RedirectToAction("Error", "Master", new { msg = "Request Time Out. Please open request again" });
            ////            break;
            ////    }
            ////}

        }
    }
}