using System.Web.Mvc;

namespace NfeToPdf.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "NfeToPdf";

            return View();
        }
    }
}
