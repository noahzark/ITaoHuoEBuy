using System.Web;
using System.Web.Mvc;

namespace EBuy
{
    /// <summary>
    /// 过滤器配置
    /// </summary>
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}