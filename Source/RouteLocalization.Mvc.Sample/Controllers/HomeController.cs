namespace RouteLocalization.Mvc.Sample.Controllers
{
    using System.Web.Mvc;

    public partial class HomeController : Controller
    {
        [Route("Index")]
        [LocalizedRoute("Welcome", "en")]
        [LocalizedRoute("Willkommen", "de")]
        public virtual ActionResult Index()
        {
            return View();
        }
    }
}