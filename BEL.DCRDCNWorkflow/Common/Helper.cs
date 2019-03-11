namespace System.Web.Mvc.Html
{
    using BEL.DCRDCNWorkflow;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Text;
    using System.Web.Mvc;
    using System.Linq;
    using System.Security.Cryptography;
    using BEL.DCRDCNWorkflow.Models.Master;
    using BEL.CommonDataContract;


    /// <summary>
    /// helper Class
    /// </summary>
    public static class Helper
    {
        /// <summary>
        /// RadioButtons the list.
        /// </summary>
        /// <param name="helper">The helper.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="selectList">The select list.</param>
        /// <param name="directionHorizontal">if set to <c>true</c> [direction horizontal].</param>
        /// <param name="cssClass">The CSS class.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>
        /// html radio button list
        /// </returns>
        public static MvcHtmlString RadioButtonList(this HtmlHelper helper, string id, object[] selectList, bool directionHorizontal = false, string cssClass = null, string defaultValue = null)
        {
            return helper.RadioButtonCheckBoxList(false, id, selectList, directionHorizontal, cssClass, defaultValue);
        }

        /// <summary>
        /// CheckBoxes the list.
        /// </summary>
        /// <param name="helper">The helper.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="selectList">The select list.</param>
        /// <param name="directionHorizontal">if set to <c>true</c> [direction horizontal].</param>
        /// <param name="cssClass">The CSS class.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>
        /// html check box list
        /// </returns>
        public static MvcHtmlString CheckBoxList(this HtmlHelper helper, string id, object[] selectList, bool directionHorizontal = false, string cssClass = null, string defaultValue = null)
        {
            return helper.RadioButtonCheckBoxList(true, id, selectList, directionHorizontal, cssClass, defaultValue);
        }

        /// <summary>
        /// Resources the value.
        /// </summary>
        /// <param name="html">The HTML.</param>
        /// <param name="key">The key.</param>
        /// <param name="resourceName">Name of the resource.</param>
        /// <returns>Resource Value</returns>
        public static string ResourceValue(this HtmlHelper html, string key, ResourceNames resourceName)
        {
            if (html != null)
            {
                return Convert.ToString(html.ViewContext.HttpContext.GetGlobalResourceObject(resourceName.ToString(), key, System.Threading.Thread.CurrentThread.CurrentUICulture));
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Labels for.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="html">The HTML.</param>
        /// <param name="expression">The expression.</param>
        /// <param name="resourceName">Name of the resource.</param>
        /// <returns>Label String</returns>
        public static MvcHtmlString LabelFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, ResourceNames resourceName)
        {
            if (expression != null && html != null)
            {
                var memberInfo = Helper.GetMemberInfo(expression);
                if (memberInfo != null)
                {
                    string key = "Label_" + memberInfo.Member.Name;
                    string value = Convert.ToString(html.ViewContext.HttpContext.GetGlobalResourceObject(resourceName.ToString(), key, System.Threading.Thread.CurrentThread.CurrentUICulture));
                    return html.LabelFor<TModel, TValue>(expression, value);
                }
            }
            return new MvcHtmlString(string.Empty);
        }

        /// <summary>
        /// Labels for.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="html">The HTML.</param>
        /// <param name="expression">The expression.</param>
        /// <param name="resourceName">Name of the resource.</param>
        /// <returns>Label String</returns>
        public static MvcHtmlString Label(this HtmlHelper html, string expression, ResourceNames resourceName)
        {
            if (expression != null && html != null)
            {
                //var memberInfo = Helper.GetMemberInfo(expression);
                //if (memberInfo != null)
                //{
                string key = "Label_" + expression;
                string value = Convert.ToString(html.ViewContext.HttpContext.GetGlobalResourceObject(resourceName.ToString(), key, System.Threading.Thread.CurrentThread.CurrentUICulture));
                if (string.IsNullOrEmpty(value.Trim()))
                {
                    value = expression;
                }
                return html.Label(expression, value);
                //}
            }
            return new MvcHtmlString(string.Empty);
        }


        /// <summary>
        /// Validations the message for.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="html">The HTML.</param>
        /// <param name="expression">The expression.</param>
        /// <param name="resourceName">Name of the resource.</param>
        /// <returns>Span tag with error text</returns>
        public static MvcHtmlString ValidationMessageFor<TModel, TProperty>(this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression, ResourceNames resourceName)
        {
            if (expression != null && html != null)
            {
                var memberInfo = Helper.GetMemberInfo(expression);
                if (memberInfo != null)
                {
                    string key = "Error_" + memberInfo.Member.Name;
                    string value = Convert.ToString(html.ViewContext.HttpContext.GetGlobalResourceObject(resourceName.ToString(), key, System.Threading.Thread.CurrentThread.CurrentUICulture));
                    return html.ValidationMessageFor<TModel, TProperty>(expression, value);
                }
            }
            return new MvcHtmlString(string.Empty);
        }

        /// <summary>
        /// Validations the message for.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="html">The HTML.</param>
        /// <param name="expression">The expression.</param>
        /// <param name="resourceKey">The resource key.</param>
        /// <param name="resourceName">Name of the resource.</param>
        /// <returns>Span tag with error text</returns>
        public static MvcHtmlString ValidationMessageFor<TModel, TProperty>(this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression, string resourceKey, ResourceNames resourceName)
        {
            if (expression != null && html != null)
            {
                string value = Convert.ToString(html.ViewContext.HttpContext.GetGlobalResourceObject(resourceName.ToString(), resourceKey, System.Threading.Thread.CurrentThread.CurrentUICulture));
                return html.ValidationMessageFor<TModel, TProperty>(expression, value);
            }
            return new MvcHtmlString(string.Empty);
        }

        /// <summary>
        /// Radioes the CheckBox button list.
        /// </summary>
        /// <param name="helper">The helper.</param>
        /// <param name="isCheckbox">if set to <c>true</c> [is checkbox].</param>
        /// <param name="id">The identifier.</param>
        /// <param name="selectList">The select list.</param>
        /// <param name="directionHorizontal">if set to <c>true</c> [direction horizontal].</param>
        /// <param name="cssClass">The CSS class.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>
        /// list of checkbox or radio button
        /// </returns>
        /// <exception cref="System.NullReferenceException">SelectList instance is null to create RadioButtonList MvcHtml control.</exception>
        private static MvcHtmlString RadioButtonCheckBoxList(this HtmlHelper helper, bool isCheckbox, string id, object[] selectList, bool directionHorizontal = false, string cssClass = null, string defaultValue = null)
        {
            if (selectList == null)
            {
                throw new NullReferenceException("SelectList instance is null to create RadioButtonList MvcHtml control.");
            }
            else
            {
                StringBuilder result = new StringBuilder();
                TagBuilder dvRadioList = new TagBuilder("div");
                dvRadioList.MergeAttribute("id", id);
                for (int idx = 0; idx < selectList.Length; idx++)
                {
                    string objectValue = Convert.ToString(selectList[idx].GetType().GetProperty("Value").GetValue(selectList[idx]));
                    string objectText = Convert.ToString(selectList[idx].GetType().GetProperty("Title").GetValue(selectList[idx]));
                    TagBuilder inputRadioTag = new TagBuilder("input");
                    inputRadioTag.MergeAttribute("type", isCheckbox ? "checkbox" : "radio");
                    inputRadioTag.MergeAttribute("id", id + "_" + idx);
                    inputRadioTag.MergeAttribute("name", id);
                    inputRadioTag.MergeAttribute("value", objectValue);
                    if (!string.IsNullOrEmpty(defaultValue) && defaultValue.ToUpper().Trim() == objectValue.ToUpper().Trim())
                    {
                        inputRadioTag.MergeAttribute("checked", "checked");
                    }
                    TagBuilder dvRadio;
                    if (directionHorizontal)
                    {
                        dvRadio = new TagBuilder("span");
                    }
                    else
                    {
                        dvRadio = new TagBuilder("div");
                    }
                    dvRadio.AddCssClass(cssClass);
                    dvRadio.InnerHtml += inputRadioTag.ToString();
                    dvRadio.InnerHtml += "<label for='" + id + "_" + idx + "'>" + objectText + "</span>";
                    dvRadioList.InnerHtml += dvRadio.ToString();
                }
                result.Append(dvRadioList.ToString());
                return MvcHtmlString.Create(result.ToString());
            }
        }

        /// <summary>
        /// Gets the member information.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns>Member Expression</returns>
        private static MemberExpression GetMemberInfo(Expression method)
        {
            LambdaExpression lambda = method as LambdaExpression;
            if (lambda == null)
            {
                return null;
            }
            MemberExpression memberExpr = null;
            if (lambda.Body.NodeType == ExpressionType.Convert)
            {
                memberExpr = ((UnaryExpression)lambda.Body).Operand as MemberExpression;
            }
            else if (lambda.Body.NodeType == ExpressionType.MemberAccess)
            {
                memberExpr = lambda.Body as MemberExpression;
            }
            if (memberExpr == null)
            {
                return null;
            }
            return memberExpr;
        }

        /// <summary>
        /// To the select list.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="selectedItem">The selected item.</param>
        /// <param name="textFieldName">Name of the text field.</param>
        /// <param name="valueFieldName">Name of the value field.</param>
        /// <returns>return select list</returns>
        public static SelectList ToSelectList(this IEnumerable list, string selectedItem, string textFieldName, string valueFieldName)
        {
            if (list != null)
            {
                SelectList selectList = new SelectList(list, valueFieldName, textFieldName, selectedItem);
                if (!string.IsNullOrEmpty(selectedItem) && selectList.SelectedValue == null)
                {
                    SelectListItem item = selectList.ToList().FirstOrDefault(x => x.Value.ToLower() == selectedItem.ToLower());
                    if (item != null)
                    {
                        item.Selected = true;
                    }
                }

                return selectList;
            }
            return null;
        }

        public static List<ApproverMasterListItem> splitUser(List<ApproverMasterListItem> items)
        {
            List<ApproverMasterListItem> tempDataList = new List<ApproverMasterListItem>();
            if (items != null)
            {
                foreach (ApproverMasterListItem item in items)
                {
                    if (item.UserSelection == true && !string.IsNullOrEmpty(item.UserID) && item.UserID.Contains(','))
                    {
                        string[] strUserName = item.UserName.Split(',');
                        string[] strUserID = item.UserID.Split(',');
                        for (int i = 0; i < strUserID.Length; i++)
                        {
                            ApproverMasterListItem temprow = new ApproverMasterListItem();
                            temprow.Title = item.Title;
                            temprow.UserName = Convert.ToString(strUserName[i]).Trim();
                            temprow.UserID = strUserID[i];
                            temprow.BusinessUnit = item.BusinessUnit;
                            temprow.Role = item.Role;
                            tempDataList.Add(temprow);
                        }
                    }
                    else
                    {
                        tempDataList.Add(item);
                    }
                }
            }
            return tempDataList.OrderBy(p => p.UserName).ToList();
        }

        //Security Fixes        
        /// <summary>
        /// Generates the hash key.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public static string GenerateHashKey(HttpContext context)
        {
            StringBuilder myStr = new StringBuilder();
            if (context != null)
            {
                myStr.Append(context.Request.Browser.Browser);
                myStr.Append(context.Request.Browser.Platform);
                myStr.Append(context.Request.Browser.MajorVersion);
                myStr.Append(context.Request.Browser.MinorVersion);
                myStr.Append(context.Request.UserHostName);
                using (SHA1 sha = new SHA1CryptoServiceProvider())
                {
                    byte[] hashdata = sha.ComputeHash(Encoding.UTF8.GetBytes(myStr.ToString()));
                    return Convert.ToBase64String(hashdata);
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// List of Valid File Extensions
        /// </summary>
        private static readonly string[] _validExtensions = { "avi", "pdf", "doc", "docx", "msg", "xls", "xlsx", "csv", "txt", "rtf", "html", "zip", "rar", "mp3", "wma", "mpg", "flv", "jpg", "jpeg", "png", "gif", "ppt", "pptx", "txt","tif","tiff","bmp" }; //  etc

        /// <summary>
        /// Validate File Extension on sever side.
        /// </summary>
        /// <param name="ext"></param>
        /// <returns></returns>
        public static bool IsValidFileExtension(string ext)
        {
            if (!string.IsNullOrEmpty(ext))
            {
                return _validExtensions.Contains(ext.ToLower());
            }
            else
            {
                return false;
            }
        }


    }
}