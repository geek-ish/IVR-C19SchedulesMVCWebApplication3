using System.Web;
using System.Web.Mvc;

namespace IVR_C19SchedulesMVCWebApplication3
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
