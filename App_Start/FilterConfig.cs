using System.Web;
using System.Web.Mvc;

namespace ElasticBeanstalk_pdf
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
