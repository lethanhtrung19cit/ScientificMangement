using DuAnQLNCKH.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DuAnQLNCKH.Controllers
{
    public class UpdatePointController : Controller
    {
        // GET: UpdatePoint
        DHTDTTDNEntities1 qLNCKHDHTDTD = new DHTDTTDNEntities1();
       // [Authorize(Roles = "1")]
        public ActionResult Index()
        {
            viewbag();
            return View();
        }
        public void viewbag()
        {
            List<Models.Type> types = qLNCKHDHTDTD.Types.ToList();
            List<PointTable> pointTables = qLNCKHDHTDTD.PointTables.ToList();
            var listPoi = (from t in types
                           join p in pointTables on t.IdTy equals p.IdTy
                           select new TopicOfLectureView
                           {
                               type = t,
                               pointTable = p
                           }).ToList();
            ViewBag.listPoint = listPoi;
            ViewBag.listType = qLNCKHDHTDTD.Types.ToList();
            List<Models.Type> typelist = qLNCKHDHTDTD.Types.ToList();
            ViewBag.listtypes = new SelectList(typelist, "IdTy", "Name");
        }
        public ActionResult Update(int IdP, double Value, int Time)
        {
            
            qLNCKHDHTDTD.Database.ExecuteSqlCommand("update PointTable set Value="+Value+", Time="+Time+" where IdP="+IdP);
            qLNCKHDHTDTD.SaveChanges();
            viewbag();
            return View("Index"); 
        }
        public ActionResult DeleteType(string IdTy)
        {

            qLNCKHDHTDTD.Database.ExecuteSqlCommand("delete from Type where IdTy='" + IdTy+"'");
            qLNCKHDHTDTD.SaveChanges();
            viewbag();
            return View("Index");
        }
        public ActionResult CreateType(string IdTy, string Name)
        {
            viewbag();

            qLNCKHDHTDTD.Database.ExecuteSqlCommand("insert into Type(IdTy, Name) values('" + IdTy + "', N'" + Name + "')");
            qLNCKHDHTDTD.SaveChanges();

            return RedirectToAction("Index", "UpdatePoint");


        }
        [HttpPost]
        public ActionResult CreatePoint(string IdTy, string NameP, double Value)
        {
            viewbag();
            
            qLNCKHDHTDTD.Database.ExecuteSqlCommand("insert into PointTable(IdTy, NameP, Value) values('" + IdTy + "', N'" + NameP + "', " + Value + ")");
            qLNCKHDHTDTD.SaveChanges();

            return RedirectToAction("Index", "UpdatePoint");


        }
    }
}