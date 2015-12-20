using System.Web.Mvc;
using AmazonQuery.Models;
using System.Collections.Generic;

namespace AmazonQuery.Controllers
{
    public class HomeController : Controller
    {
        static AmazonFinder afinder=null;   // object for items retrieval from amazon server
        static List<SelectListItem> curlist = null; // currency names pulled from currency server
        static string cur_cur = "USD";  //default currency us server
        // Get: Index
        public ActionResult Index()
        {
            if(curlist==null)
                curlist = CurrencyList.populateList(cur_cur);
            ViewBag.currencyList = curlist;
            return View();
        }
        // GET: Search
        [HttpPost]
        public JsonResult Search(SearchReq model)   //called on keyword submittion 
        {

            var req = model.Query.Trim();
            if (req.Length > 0)
            {
                afinder = new AmazonFinder(model.Query);
                var result = afinder.getNext(AmazonFinder.STEP13);
                return Json(result);
            }
           
           return Json(null); 
           
        }
        [HttpPost]
        public JsonResult Next()        //called to pull next 13 items page
        {

            if (afinder == null) return Json(null);
           
            var list = afinder.getNext(AmazonFinder.STEP13);

            return Json(list);
        }

        [HttpPost]
        public ActionResult GetRate(Currency model) // called to get currency rate conversion from USD, based on EUR
        {

            var rates=CurrencyList.getCurrencyRate(model.to_cur,"EUR", cur_cur);
            var convert = (rates[1]/rates[2]) * rates[0];            

            return Content(convert.ToString(),"text/plain");
        }
    }
}