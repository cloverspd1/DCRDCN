namespace BEL.DCRDCNWorkflow
{
    using BEL.DCRDCNWorkflow.Common;
    using System.Web;
    using System.Web.Mvc;

    /// <summary>
    /// The Filter Config
    /// </summary>
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            if (filters != null)
            {
                filters.Add(new HandleErrorAttribute());
                filters.Add(new SessionExpiration());
            }
        }
    }
}
