//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;
//using workReport.Models;

//namespace workReport.Controllers
//{
//    public class GraphController : Controller
//    {
//        // GET: Graph
//        public ActionResult Index()
//        {
//            return View();
//        }

//        public ActionResult GetGraphData(string id, bool did)
//        {
//            var xyz = 0;
//            if (did == true)
//            {
//                xyz = -7;
//            }
//            else
//            {
//                xyz = -30;

//            }
//            var todaysdate = DateTime.Now.ToShortDateString();
//            var thirtydaysbefore = DateTime.Now.AddDays(xyz);
//            using (var ent = new workReportEntities())
//            {
//                var SumScore = ent.workList.Where(x => x.users ==Convert.ToInt32( id) && x.date_Eng >= thirtydaysbefore && x.date_Eng <= DateTime.Now)
//           .Select(x => new WorkListModel()
//           {
//              var call=
//               users = x.users,
//               date_Eng = x.date_Eng,
//               totalCountCall = x.workListType.Count(),
//               AddedDateString = x.date_Eng.ToString(),
//               LastTradedPrice = x.workListType,
//               AddedDate = x.date_Eng


//           })
//               .OrderBy(a => a.AddedDate)
//               .ToList();

//                return Json(SumScore, JsonRequestBehavior.AllowGet);
//            }


//        }



//        public ActionResult LineGraph()
//        {
//            return View();
//        }

//    }
//}