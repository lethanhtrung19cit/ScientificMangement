using DuAnQLNCKH.Models;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DuAnQLNCKH.Controllers
{
    public class TypeController : Controller
    {
        DHTDTTDNEntities1 qLNCKHDHTDTD = new DHTDTTDNEntities1();

        [Authorize(Roles = "1")]
        public ActionResult Index(FilePointTable model)
        {
            
            var list1 = (from c in qLNCKHDHTDTD.PointTables
                         select new FilePointTable
                         {
                             IdP = c.IdP,
                             IdTy = c.IdTy,
                             NameP = c.NameP,
                             File1=c.File1
                         }).ToList();
            //var list1 = demo.Database.SqlQuery<tblFileDetail>("select * from tblFileDetails").ToList();

            

            ViewBag.FileList = list1;
            model.ListFile = list1;
            ViewBag.listType = qLNCKHDHTDTD.Types.ToList();
            ViewBag.listPointTable = qLNCKHDHTDTD.PointTables.ToList();
            List<Models.Type> typelist = qLNCKHDHTDTD.Types.ToList();
            ViewBag.listtypes = new SelectList(typelist, "IdTy", "Name");
            //List<PointTable> point = new List<PointTable>();
            return View();
        }
       
        [HttpPost]
        public ActionResult CreateType(Models.Type type)
        {
            if (ModelState.IsValid)
            {
                TypeModel types = new TypeModel();
               
                if (types.AddTypes(type, types.IdTy()))
                    ViewBag.Message = "Employee details added successfully";

            }
            ViewBag.listPointTable = qLNCKHDHTDTD.PointTables.ToList();
            List<Models.Type> typelist = qLNCKHDHTDTD.Types.ToList();
            ViewBag.listtypes = new SelectList(typelist, "IdTy", "Name");
            ViewBag.listType = qLNCKHDHTDTD.Types.ToList();
            return RedirectToAction("Index", "Type");
            
        }
        
        [HttpPost]
        public ActionResult CreatePoint(HttpPostedFileBase files, FilePointTable model)
        {

            var list1 = (from c in qLNCKHDHTDTD.PointTables
                         select new FilePointTable
                         {
                             IdP = c.IdP,
                             IdTy = c.IdTy,
                             NameP = c.NameP,
                             File1 = c.File1
                         }).ToList();
            //var list1 = demo.Database.SqlQuery<tblFileDetail>("select * from tblFileDetails").ToList();


            model.ListFile = list1;
            ViewBag.FileList = list1;
            
            if (files != null)
            {
                var Extension = Path.GetExtension(files.FileName);
                var fileName = "my-file-" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + Extension;
                string path = Path.Combine(Server.MapPath("~/UpLoadFile"), fileName);
                model.File1 = Url.Content(Path.Combine("~/UpLoadFile/", fileName));
                TypeModel type = new TypeModel();

                if (type.AddPoint(model))
                {
                    files.SaveAs(path);
                    ViewBag.Message = "Employee details added successfully";
                }
                else
                {
                    ModelState.AddModelError("", "Error In Add File. Please Try Again !!!");
                }
            }
            else
            {
                ModelState.AddModelError("", "Please Choose Correct File Type !!");
               
            }

            
            ViewBag.listPointTable = qLNCKHDHTDTD.PointTables.ToList();
            List<Models.Type> typelist = qLNCKHDHTDTD.Types.ToList();
            ViewBag.listtypes = new SelectList(typelist, "IdTy", "Name");
            ViewBag.listType = qLNCKHDHTDTD.Types.ToList();
            return RedirectToAction("Index", "Type");
            
            
        }

        public ActionResult DownloadFile(string filePath)
        {
            string fullName = Server.MapPath("~" + filePath);

            byte[] fileBytes = GetFile(fullName);
            return File(
                fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, filePath);
        }

        byte[] GetFile(string s)
        {
            System.IO.FileStream fs = System.IO.File.OpenRead(s);
            byte[] data = new byte[fs.Length];
            int br = fs.Read(data, 0, data.Length);
            if (br != fs.Length)
                throw new System.IO.IOException(s);
            return data;
        }


    }
}