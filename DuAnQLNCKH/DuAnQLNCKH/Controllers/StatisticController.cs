using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using DuAnQLNCKH.Models;


namespace DuAnQLNCKH.Controllers
{
    public class StatisticController : Controller
    {
        // GET: Statistic
        DHTDTTDNEntities1 dHTDTTDNEntities1 = new DHTDTTDNEntities1();
        //[Authorize(Roles = "1")]
        public ActionResult Index()
        {
            Session["Acess"] = "1";
            Session["UserName"] = "phamquangthao";
                viewbag();
           
                return View();
            
             
        }
        [HttpPost]
        public void ExportExcel1(List<TopicLectureStatisticModel> list)
        {
            Session["listEx1"] = list;
        }
        public void ExportExcel2(List<TopicStudenStatisticModel> list1)
        {
            Session["listEx2"] = list1;
        }
        [Authorize(Roles = "1")]
        public ActionResult journal()
        {

            using (DHTDTTDNEntities1 db = new DHTDTTDNEntities1())
            {

                List<TopicOfLecture> topicOfLectures = db.TopicOfLectures.ToList();

                List<PointTable> pointTables = db.PointTables.ToList();

                List<Information> information = db.Information.ToList();
                List<TopicOfStudent> topicOfStudents = db.TopicOfStudents.ToList();
                List<Models.Type> types = db.Types.ToList();
                var topicOfLecture = (from t in topicOfLectures
                                      join p in pointTables on t.IdP equals p.IdP
                                      join ty in types on p.IdTy equals ty.IdTy
                                      join i in information on t.IdLe equals i.IdLe
                                      
                                      where t.Status == 1 && ty.Name=="Tạp chí"
                                      select new TopicOfLectureView
                                      {
                                          pointTable = p,
                                          topicOfLecture = t,
                                          information = i,
                                          type = ty
                                          
                                      }).ToList();
                ViewBag.listTopicOfLecture = topicOfLecture;
                var topicOfStudent = (from t in topicOfStudents
                                      join p in pointTables on t.IdP equals p.IdP
                                      join ty in types on p.IdTy equals ty.IdTy

                                      where t.Status == 1 && ty.Name == "Tạp chí"
                                      select new TopicOfStudentView
                                      {
                                          pointTable = p,
                                          topicOfStudent = t,
                                          type=ty

                                      }).ToList();
                ViewBag.topicOfStudents = topicOfStudent;
                var listJournal = (from t in types
                                   join p in pointTables on t.IdTy equals p.IdTy
                                   where t.Name == "Tạp chí"
                                   select new
                                   {
                                       p.NameP,
                                       p.IdP

                                   }).ToList();
                ViewBag.listJournal = new SelectList(listJournal, "IdP", "NameP");
            }

           

            return View();
        }
         
        public void viewbag()
        {
            using (DHTDTTDNEntities1 db = new DHTDTTDNEntities1())
            {

                List<TopicOfLecture> topicOfLectures = db.TopicOfLectures.ToList();
                List<Subject> subjects = db.Subjects.ToList();
                List<PointTable> pointTables = db.PointTables.ToList();
                List<Faculty> faculties = db.Faculties.ToList();
                List<Information> information = db.Information.ToList();
                List<TopicOfStudent> topicOfStudents = db.TopicOfStudents.ToList();
                List<ProgressLe> progressLes = db.ProgressLes.ToList();
                List<ProgressSt> progressSts = db.ProgressSts.ToList();
                List<Models.Type> types = db.Types.ToList();
                 var topicOfLecture = (from t in topicOfLectures
                                      join p in pointTables  on t.IdP equals p.IdP
                                      join ty in types on p.IdTy equals ty.IdTy
                                      join i in information on t.IdLe equals i.IdLe
                                      join f in faculties on i.IdKhoa equals f.IdFa
                                      join pr in progressLes on t.IdTp equals pr.IdTp
                                      where t.Status==1 
                                      select new TopicOfLectureView
                                      {
                                          type=ty,
                                          pointTable=p,
                                          topicOfLecture = t,
                                          information = i,
                                          faculty = f,
                                          progressLe=pr
                                      }).ToList();
                ViewBag.listTopicOfLecture = topicOfLecture;
                var topicOfStudent1 = (from t in topicOfStudents
                                       
                                       join p in pointTables on t.IdP equals p.IdP
                                       join ty in types on p.IdTy equals ty.IdTy 
                                       join s in subjects  on t.IdSu equals s.IdSu
                                       join pr in progressSts on t.IdTp equals pr.IdTp
                                       where t.Status == 1
                                       select new TopicOfStudentView
                                       {

                                           topicOfStudent = t,
                                           subject=s,
                                           type=ty,
                                           pointTable = p,
                                           progressSt=pr

                                       }).ToList();
                ViewBag.listtopicOfStudents = topicOfStudent1;
                var listJournal = (from t in types
                                   join p in pointTables on t.IdTy equals p.IdTy
                                   where t.Name == "Tạp chí"
                                   select new 
                                   {
                                      p.NameP,
                                      p.IdP

                                   }).ToList();
                ViewBag.listJournal = new SelectList(listJournal, "IdP", "NameP");
                ViewBag.listNameStu = new SelectList(topicOfStudents, "IdSV", "NameSt");
            }
            
            List<Models.Type> typelist = dHTDTTDNEntities1.Types.ToList();
            ViewBag.listtype = new SelectList(typelist, "IdTy", "Name");
            List<Faculty> faculties1 = dHTDTTDNEntities1.Faculties.ToList();
            ViewBag.listFacul = new SelectList(faculties1, "IdFa", "Name");
            List<Subject> subjects1 = dHTDTTDNEntities1.Subjects.ToList();
            ViewBag.subjects1 = new SelectList(subjects1, "IdSu", "NameCu");
            List<Information> information1 = dHTDTTDNEntities1.Information.ToList();
            ViewBag.listNameLe = new SelectList(information1, "IdLe", "NameLe");
            ViewBag.listProgress = new SelectList(new List<SelectListItem> {
                       new SelectListItem { Value = "1" , Text = "đang thực hiện" },
                       new SelectListItem { Value = "2" , Text = "quá hạn" },
                       new SelectListItem { Value = "3" , Text = "đã nghiệm thu" }
            }, "Value", "Text");
         }

        public ActionResult getTypeList(string IdTy)
        {
            dHTDTTDNEntities1.Configuration.ProxyCreationEnabled = false;
            List<PointTable> DetailList = dHTDTTDNEntities1.PointTables.Where(x => x.IdTy == IdTy).ToList();
            return Json(DetailList, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SearchLecture(string name)
        {
            List<TopicOfLecture> listTopicOfLecture = dHTDTTDNEntities1.TopicOfLectures.Where(x=>x.Name.Contains(name)).ToList();
            List<TopicOfStudent> listTopicOfStudent = dHTDTTDNEntities1.TopicOfStudents.ToList();
            ViewBag.listTopicOfStudent = listTopicOfStudent;
            ViewBag.listTopicOfLecture = listTopicOfLecture;
            return View("Index");
        }
     
        private SqlConnection con;
        public string IdP1;
        public void connection()
        {
            string constr = @"Data Source=DESKTOP-ECMGDNK\SQLEXPRESS;initial catalog=nckh_dhdn;integrated security=True";
            con = new SqlConnection(constr);

        }
        [HttpPost]
        public ActionResult ExportExcel()
        {
            var gv = new GridView();

            gv.DataSource = Session["listEx1"];
            gv.DataBind();
             
            gv.HeaderRow.Cells[0].Text = "Tên đề tài";
            gv.HeaderRow.Cells[1].Text = "Tên giảng viên";
            gv.HeaderRow.Cells[2].Text = "Tên khoa";
            gv.HeaderRow.Cells[3].Text = "Loại đề tài";           
            gv.HeaderRow.Cells[4].Text = "Ngày bắt đầu";
            gv.HeaderRow.Cells[5].Text = "Thời gian";
            gv.HeaderRow.Cells[6].Text = "Kết thúc";
            gv.HeaderRow.Cells[7].Text = "Kinh phí";
            gv.HeaderRow.Cells[8].Text = "Trạng thái";
            gv.HeaderRow.Cells[9].Text = "Điểm";
               
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=DemoExcel.xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            StringWriter objStringWriter = new StringWriter();
            HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);
            gv.RenderControl(objHtmlTextWriter);
            Response.Output.Write(objStringWriter.ToString());
            Response.Flush();
            Response.End();
            viewbag();
            return View("Index");
        }
        [HttpPost]
        public ActionResult ExportExcelStu()
        {
            var gv = new GridView();

            gv.DataSource = Session["listEx2"];
            gv.DataBind();
          
            gv.HeaderRow.Cells[0].Text = "Tên đề tài";
            gv.HeaderRow.Cells[1].Text = "Tên sinh viên";
            gv.HeaderRow.Cells[2].Text = "Chuyên sâu";
            gv.HeaderRow.Cells[3].Text = "Loại đề tài";
            gv.HeaderRow.Cells[4].Text = "GV hướng dẫn";
            gv.HeaderRow.Cells[5].Text = "Ngày bắt đầu";
            gv.HeaderRow.Cells[6].Text = "Thời gian";
            gv.HeaderRow.Cells[7].Text = "Kết thúc";
            gv.HeaderRow.Cells[8].Text = "Kinh phí";
            gv.HeaderRow.Cells[9].Text = "Trạng thái";             
            gv.HeaderRow.Cells[10].Text = "Điểm";
            
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=DemoExcel.xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            StringWriter objStringWriter = new StringWriter();
            HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);
            gv.RenderControl(objHtmlTextWriter);
            Response.Output.Write(objStringWriter.ToString());
            Response.Flush();
            Response.End();
            viewbag();
            return View("Index");
        }

        //[HttpPost]
        
        //public JsonResult ExportToExcel(string IdPa, DateTime dateEnd, DateTime dateSt)
        //{
        //    ViewBag.IdP = IdPa;
        //    IdP1 = IdPa;
        //    viewbag();
        //    return Json("Index");
        //}
        public string idp()
        {

            return ViewBag.IdP;
        }
    }
}