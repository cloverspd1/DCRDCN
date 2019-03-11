using System.Web;
using System.Web.Mvc;

namespace BEL.DCRDCNSPAppWeb
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
