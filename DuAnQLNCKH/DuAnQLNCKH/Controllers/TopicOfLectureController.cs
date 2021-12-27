using DuAnQLNCKH.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using Type = DuAnQLNCKH.Models.Type;

namespace DuAnQLNCKH.Controllers
{
    public class TopicOfLectureController : Controller
    {
        // GET: DeTaiGV
        DHTDTTDNEntities1 qLNCKHDHTDTD = new DHTDTTDNEntities1();
        TopicOfLectureModel dtgv = new TopicOfLectureModel();
        List<TopicOfStudent> studentList = new List<TopicOfStudent>();
        List<Subject> subjects = new DHTDTTDNEntities1().Subjects.ToList();
        List<Extension> extensions = new DHTDTTDNEntities1().Extensions.ToList();
        List<TopicOfLecture> topicOfLectures = new DHTDTTDNEntities1().TopicOfLectures.ToList();
        List<TopicOfStudent> topicOfStudents = new DHTDTTDNEntities1().TopicOfStudents.ToList();
        List<Information> information = new DHTDTTDNEntities1().Information.ToList();
        List<PointTable> pointTables = new DHTDTTDNEntities1().PointTables.ToList();
        List<Faculty> faculties = new DHTDTTDNEntities1().Faculties.ToList();
        List<ProgressLe> progressLes = new DHTDTTDNEntities1().ProgressLes.ToList();
        List<ProgressSt> progressSts = new DHTDTTDNEntities1().ProgressSts.ToList();
        public ActionResult ListType()
        {
            List<Type> typelist = qLNCKHDHTDTD.Types.ToList();
            ViewBag.listtype = new SelectList(typelist, "IdTy", "Name");
            return View();
        }
        public ActionResult getTypeList(string IdTy)
        {
            qLNCKHDHTDTD.Configuration.ProxyCreationEnabled = false;
            List<PointTable> DetailList = qLNCKHDHTDTD.PointTables.Where(x => x.IdTy == IdTy).ToList();
            return Json(DetailList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Index()
        {
                
                var topicofLecture = (from t in topicOfLectures
                                      join i in information on t.IdLe equals i.IdLe
                                      join f in faculties on i.IdKhoa equals f.IdFa
                                      join p in pointTables on t.IdP equals p.IdP
                                      join e in extensions on t.IdTp equals e.IdTp
                                      join pr in progressLes on t.IdTp equals pr.IdTp
                                      select new TopicOfLectureView
                                      {
                                          pointTable = p,
                                          topicOfLecture = t,
                                          information = i,
                                          extension = e,
                                          faculty=f,
                                          progressLe=pr
                                      }).ToList();
                ViewBag.TopicOfLecture = topicofLecture;
                var topics = (from t in topicOfStudents
                              join p in pointTables on t.IdP equals p.IdP
                              join s in subjects on t.IdSu equals s.IdSu
                              join pr in progressSts on t.IdTp equals pr.IdTp
                              where t.Status == 1
                              select new TopicOfStudentView
                              {

                                  topicOfStudent = t,
                                  subject=s,
                                  pointTable = p,
                                  progressSt=pr
                              }).ToList();
                ViewBag.listTopicOfStudent = topics;
            
          
            return View();

        }

        public ActionResult myTopicLecture()
        {

          
            using (DHTDTTDNEntities1 db = new DHTDTTDNEntities1())
            {

                List<TopicOfLecture> topicOfLectures = db.TopicOfLectures.ToList();

                List<Information> information = db.Information.ToList();
                List<PointTable> pointTables = db.PointTables.ToList();
                List<Extension> extensions = db.Extensions.ToList();

                var topicofLecture = (from t in topicOfLectures
                                      join i in information on t.IdLe equals i.IdLe
                                      join p in pointTables on t.IdP equals p.IdP
                                      where i.UserName == Session["UserName"].ToString() && t.Status==0                                    
                                      select new TopicOfLectureView
                                      {
                                          pointTable=p,
                                          topicOfLecture = t,
                                          information = i
                                      }).ToList();
                ViewBag.topicExtension = topicofLecture;
                var topic = (from t in topicOfLectures
                                      join i in information on t.IdLe equals i.IdLe
                                      join p in pointTables on t.IdP equals p.IdP
                                      join e in extensions on t.IdTp equals e.IdTp
                                      join pr in progressLes on t.IdTp equals pr.IdTp
                                      where i.UserName == Session["UserName"].ToString() && t.Status == 1 && new TopicOfLectureModel().dateLec(t.IdTp)==pr.Date
                                      select new TopicOfLectureView
                                      {
                                          pointTable=p,
                                          topicOfLecture = t,
                                          extension=e,
                                          information = i,
                                          progressLe = pr
                                      }).ToList();
                ViewBag.topicProgress = topic;
                var topic1 = (from t in topicOfLectures
                             join i in information on t.IdLe equals i.IdLe
                             join p in pointTables on t.IdP equals p.IdP
                             where i.UserName == Session["UserName"].ToString() && t.Status == 2
                             select new TopicOfLectureView
                             {
                                 pointTable = p,
                                 topicOfLecture = t,
                                 information = i
                             }).ToList();
                ViewBag.topicEnd = topic1;
                return View();
            }
        }
        //To Handle connection related activities
        [HttpPost]
        public void ShowIdP(string id)
        {
            ViewBag.idp = id;
            
        }
        [HttpPost]
        public ActionResult CreateTopicOfLecture(HttpPostedFileBase FileDemo1, TopicOfLecture topicOfLecture)
        {
            //        [Required(ErrorMessage = "Nhập tên đề tài")]
            if (ModelState.IsValid)
            {
                string id = dtgv.IdTp();
                string username = Session["UserName"].ToString();
                TopicOfLectureModel topic = new TopicOfLectureModel();
                string idle = dtgv.IdLe(username);

                if (FileDemo1 != null)
                {
                    var Extension = Path.GetExtension(FileDemo1.FileName);
                    var fileName = "fileTopic-" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + Extension;
                    string path = Path.Combine(Server.MapPath("~/File/FileTopic"), fileName);
                    topicOfLecture.FileDemo = Url.Content(Path.Combine("~/File/FileTopic/", fileName));
                    if (topic.AddTopicLecture(topicOfLecture, id, idle))
                    {
                        ViewBag.Message = "Đăng ký đề tài thành công";
                        FileDemo1.SaveAs(path);
                        List<Type> typelist1 = qLNCKHDHTDTD.Types.ToList();
                        ViewBag.listtype = new SelectList(typelist1, "IdTy", "Name");
                        return View("ViewCreateTopicOfLecture");
                    }
                }
                else
                {
                    qLNCKHDHTDTD.Database.ExecuteSqlCommand("set dateformat dmy Insert into TopicOfLecture(IdTp, Name, IdLe, IdP, DateSt, Times, Expense, Status, CountAuthor) values ('" + id + "', N'" + topicOfLecture.Name + "', '" + idle + "', " + topicOfLecture.IdP + ", '" + topicOfLecture.DateSt + "', " + topicOfLecture.Times + ", " + topicOfLecture.Expense + ", 0, " + topicOfLecture.CountAuthor + ")");
                    ViewBag.Message = "Đăng ký đề tài thành công";
                    List<Type> typelist1 = qLNCKHDHTDTD.Types.ToList();
                    ViewBag.listtype = new SelectList(typelist1, "IdTy", "Name");
                    return View("ViewCreateTopicOfLecture");
                } 
                        

             }

            
             
            List<Type> typelist = qLNCKHDHTDTD.Types.ToList();
            ViewBag.listtype = new SelectList(typelist, "IdTy", "Name");
            return View("ViewCreateTopicOfLecture", topicOfLecture);
           
        }
       
        public ActionResult ViewCreateTopicOfLecture()
        {
            
            ViewBag.Message = "";
            List<Type> typelist = qLNCKHDHTDTD.Types.ToList();
            ViewBag.listtype = new SelectList(typelist, "IdTy", "Name");
            
            return View();
        }
        
        public ActionResult chuaduyet()
        {
            using (DHTDTTDNEntities1 db = new DHTDTTDNEntities1())
            {

                List<TopicOfLecture> topicOfLectures = db.TopicOfLectures.ToList();
                List<TopicOfStudent> topicOfStudents = db.TopicOfStudents.ToList();
                List<Information> information = db.Information.ToList();
                List<PointTable> pointTables = db.PointTables.ToList();
                List<Faculty> faculties = db.Faculties.ToList();
                List<Subject> subjects = qLNCKHDHTDTD.Subjects.ToList();
                var topicofLecture = (from t in topicOfLectures
                                      join i in information on t.IdLe equals i.IdLe
                                      join f in faculties on i.IdKhoa equals f.IdFa
                                      join p in pointTables on t.IdP equals p.IdP
                                       where t.Status == 0
                                      select new TopicOfLectureView
                                      {
                                          pointTable = p,
                                          topicOfLecture = t,
                                          information = i,
                                          faculty=f
                                      }).ToList();
                ViewBag.TopicOfLecture = topicofLecture;
                var topicofStudent = (from t in topicOfStudents
                                     join s in subjects on t.IdSu equals s.IdSu
                                      join p in pointTables on t.IdP equals p.IdP
                                      where t.Status == 0
                                      select new TopicOfLectureView
                                      {
                                          pointTable = p,
                                          topicOfStudent = t,
                                          subject=s
                                      }).ToList();
                ViewBag.TopicOfStudent = topicofStudent;
            }
           
            
            return View();
         
        }
         
        public ActionResult detailTopicLecture(string IdTp)
        {
            var listDetail = (from t in topicOfLectures
                              join i in information on t.IdLe equals i.IdLe
                              join p in pointTables on t.IdP equals p.IdP
                              join f in faculties on i.IdKhoa equals f.IdFa
                              where t.IdTp==IdTp
                              select new TopicOfLectureView
                              {
                                  topicOfLecture = t,
                                  information = i,
                                  faculty = f,
                                  pointTable = p
                              }
                            ).ToList();
            ViewBag.listDetail = listDetail;
            return View();
        }
        public void rejectTopic(string IdTp, string Reason)
        {
            var email = (from t in topicOfLectures
                            join i in information on t.IdLe equals i.IdLe
                            where t.IdTp == IdTp
                            select new
                            {
                                i.Email
                            }).FirstOrDefault().Email;
            TopicOfLecture topicOfLecture = new TopicOfLecture();
            var topic = qLNCKHDHTDTD.TopicOfLectures.Find(IdTp);
            topic.Status = 2;            
            qLNCKHDHTDTD.Entry(topic).State = EntityState.Modified;
            qLNCKHDHTDTD.SaveChanges();
             
            var nameTopic = qLNCKHDHTDTD.TopicOfLectures.Where(x => x.IdTp == IdTp).Select(x => x.Name).FirstOrDefault();
            string from1 = qLNCKHDHTDTD.Emails.Select(x => x.NameEmail).FirstOrDefault();
            string pass = qLNCKHDHTDTD.Emails.Where(x => x.NameEmail == from1).Select(x => x.PassWord).FirstOrDefault();

            using (MailMessage mail = new MailMessage())
            {
                // Define your senders
                mail.From = new MailAddress(from1);
                mail.Body = "Thông báo về đề tài " + nameTopic + " không được duyệt, Lí do : "+Reason;
                mail.Subject = "Thông báo đề tài không được duyệt";

                mail.To.Add(email.ToString());

                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.EnableSsl = true;
                NetworkCredential networkCredential = new NetworkCredential(from1, pass);
                smtp.UseDefaultCredentials = true;
                smtp.Credentials = networkCredential;
                smtp.Port = 587;
                smtp.Send(mail);
            }
        }
        public ActionResult detailTopicSt(string IdTp)
        {
            var listDetail = (from t in topicOfStudents
                              join p in pointTables on t.IdP equals p.IdP
                              join s in subjects on t.IdSu equals s.IdSu
                              where t.IdTp==IdTp
                              select new TopicOfLectureView
                              {
                                  topicOfStudent = t,                                  
                                  subject = s,
                                  pointTable = p
                              }
                            ).ToList();
            ViewBag.listDetail = listDetail;
            return View();
        }
        public void rejectTopicSt(string IdTp, string Reason)
        {
            var email = (from t in topicOfStudents
                            
                            where t.IdTp == IdTp
                            select new
                            {
                                t.Email
                            }).FirstOrDefault().Email;
            TopicOfStudent topicOfStudent = new TopicOfStudent();
            var topic = qLNCKHDHTDTD.TopicOfStudents.Find(IdTp);
            topic.Status = 2;            
            qLNCKHDHTDTD.Entry(topic).State = EntityState.Modified;
            qLNCKHDHTDTD.SaveChanges();
             
            var nameTopic = qLNCKHDHTDTD.TopicOfStudents.Where(x => x.IdTp == IdTp).Select(x => x.Name).FirstOrDefault();
            string from1 = qLNCKHDHTDTD.Emails.Select(x => x.NameEmail).FirstOrDefault();
            string pass = qLNCKHDHTDTD.Emails.Where(x => x.NameEmail == from1).Select(x => x.PassWord).FirstOrDefault();

            using (MailMessage mail = new MailMessage())
            {
                // Define your senders
                mail.From = new MailAddress(from1);
                mail.Body = "Thông báo về đề tài " + nameTopic + " không được duyệt, Lí do : "+Reason;
                mail.Subject = "Thông báo đề tài không được duyệt";

                mail.To.Add(email.ToString());

                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.EnableSsl = true;
                NetworkCredential networkCredential = new NetworkCredential(from1, pass);
                smtp.UseDefaultCredentials = true;
                smtp.Credentials = networkCredential;
                smtp.Port = 587;
                smtp.Send(mail);
            }
        }
        public void extension(int IdEx, string NameTo, string Email)
        {

            using (DHTDTTDNEntities1 entities = new DHTDTTDNEntities1())
            {
                entities.Database.ExecuteSqlCommand("update Extension set Status = N'Đã duyệt' where IdEx=" + IdEx);
                entities.SaveChanges();
                string from1 = qLNCKHDHTDTD.Emails.Select(x => x.NameEmail).FirstOrDefault();
                string pass = qLNCKHDHTDTD.Emails.Where(x => x.NameEmail == from1).Select(x => x.PassWord).FirstOrDefault();
                // By using a Message with no constructors, you can define your To recipients below
                using (MailMessage mail = new MailMessage())
                {
                    // Define your senders
                    mail.From = new MailAddress(from1);
                    mail.Body = "Thông báo đề tài " + NameTo + " đã được gia hạn";
                    mail.Subject = "Thông báo gia hạn đề tài";

                    mail.To.Add(Email);

                    mail.IsBodyHtml = true;
                    SmtpClient smtp = new SmtpClient();
                    smtp.Host = "smtp.gmail.com";
                    smtp.EnableSsl = true;
                    NetworkCredential networkCredential = new NetworkCredential(from1, pass);
                    smtp.UseDefaultCredentials = true;
                    smtp.Credentials = networkCredential;
                    smtp.Port = 587;
                    smtp.Send(mail);
                }

            }

        }
        public ActionResult registerExtension(string IdTp, DateTime Times, string Reason)
        {
            
            using (DHTDTTDNEntities1 db = new DHTDTTDNEntities1())
            {
                db.Database.ExecuteSqlCommand("update Extension set Times='" + Times + "', Reason=N'" + Reason + "', Status=N'chưa duyệt' where IdTp='"+IdTp+"'");
                List<TopicOfLecture> topicOfLectures = db.TopicOfLectures.ToList();
                List<Extension> extensions = db.Extensions.ToList();
                List<Information> information = db.Information.ToList();

                var topicextension = (from t in topicOfLectures
                                      join i in information on t.IdLe equals i.IdLe
                                      join e in extensions on t.IdTp equals e.IdTp
                                      where i.UserName == Session["UserName"].ToString()

                                      select new TopicOfLectureView
                                      {
                                          extension = e,
                                          topicOfLecture = t,
                                          information = i
                                      }).ToList();
                ViewBag.topicextension = topicextension;

            }
            return View("viewRegisterExtension");
        }
        public ActionResult viewRegisterExtension()
        {     
            using (DHTDTTDNEntities1 db = new DHTDTTDNEntities1())
            {

                List<TopicOfLecture> topicOfLectures = db.TopicOfLectures.ToList();
                List<Extension> extensions = db.Extensions.ToList();
                List<Information> information = db.Information.ToList();
                List<Faculty> faculties = db.Faculties.ToList();
                var topicextension = (from t in topicOfLectures 
                                     join i in information on t.IdLe equals i.IdLe
                                     join f in faculties on i.IdKhoa equals f.IdFa
                                     join e in extensions on t.IdTp equals e.IdTp
                                     where i.UserName== Session["UserName"].ToString()
                                     
                                      select new TopicOfLectureView
                                     {
                                         extension=e,
                                         topicOfLecture = t,
                                         information=i,
                                         faculty=f
                                     }).ToList();
                ViewBag.topicextension = topicextension;
                return View();
            }
        }
        public ActionResult listextension()
        {
            using (DHTDTTDNEntities1 db = new DHTDTTDNEntities1())
            {
                List<Extension> extension = db.Extensions.ToList();
                
                List<TopicOfLecture> topicOfLectures = db.TopicOfLectures.ToList();
                List<Information> informations = db.Information.ToList();
                List<PointTable> pointTables = db.PointTables.ToList();
                List<Faculty> faculties = db.Faculties.ToList();
                List<ProgressLe> progressLes = db.ProgressLes.ToList();

                var topicextension =(from t in topicOfLectures 
                                     join e in extension on t.IdTp equals e.IdTp
                                     join i in informations on t.IdLe equals i.IdLe
                                     join f in faculties on i.IdKhoa equals f.IdFa
                                     join p in pointTables on t.IdP equals p.IdP
                                     join pr in progressLes on t.IdTp equals pr.IdTp
                                     where e.Status == "chưa duyệt"
                                     select new TopicOfLectureView
                                     {
                                         extension = e,
                                         topicOfLecture = t,
                                         information=i,
                                         pointTable=p,
                                         faculty=f,
                                         progressLe=pr
                                         
                                     }).ToList();
                ViewBag.listextension = topicextension;
                return View();
            }
          

        }
        
        public ActionResult chuaduyetsv()
        {            
            
             var model = dtgv.listchuaduyetsv();
            return View(model);
        }
        [HttpPost]
        [Route("/TopicOfLecture/extension")]
        
        public ActionResult updateExtension(string IdTp, string DateEx, string Reason)
        {
            using (DHTDTTDNEntities1 entities = new DHTDTTDNEntities1())
            {
                List<TopicOfLecture> topicOfLectures = entities.TopicOfLectures.ToList();
                List<Extension> extensions = entities.Extensions.ToList();
                List<Information> information = entities.Information.ToList();

                var topicextension = (from t in topicOfLectures
                                      join i in information on t.IdLe equals i.IdLe
                                      join e in extensions on t.IdTp equals e.IdTp
                                      where i.UserName == Session["UserName"].ToString()

                                      select new TopicOfLectureView
                                      {
                                          extension = e,
                                          topicOfLecture = t,
                                          information = i
                                      }).ToList();
                ViewBag.topicextension = topicextension;
                DateTime date = Convert.ToDateTime(DateEx);
                entities.Database.ExecuteSqlCommand("set dateformat dmy  update Extension set Times = '"+date+"', Reason=N'"+Reason+"' where IdTp='" + IdTp+"'");
                entities.SaveChanges();
            }
            return RedirectToAction("viewRegisterExtension");
        }
        [HttpPost]       
        public void xetduyet2(string IdTp, string Times, string NameTo, string Email)
        {

            using (DHTDTTDNEntities1 entities = new DHTDTTDNEntities1())
            {
                string a = IdTp;
                DateTime Time = Convert.ToDateTime(Times);
                entities.Database.ExecuteSqlCommand("set dateformat dmy update TopicOfLecture set Status=1 where IdTp='" + IdTp +"' set dateformat dmy insert into ProgressLe values('"+IdTp+"', '"+DateTime.Now.ToString("d")+"', 1)"+ " insert into Extension(IdTp, Times, Status) values('" + IdTp + "', '" + Time + "', 'NULL')");
                entities.SaveChanges();
                string from1 = qLNCKHDHTDTD.Emails.Select(x => x.NameEmail).FirstOrDefault();
                string pass = qLNCKHDHTDTD.Emails.Where(x => x.NameEmail == from1).Select(x => x.PassWord).FirstOrDefault();

                string s = Session["UserName"].ToString();

                // By using a Message with no constructors, you can define your To recipients below
                using (MailMessage mail = new MailMessage())
                {
                    // Define your senders
                    mail.From = new MailAddress(from1);
                    mail.Body = "Thông báo đề tài " + NameTo + " đã được xét duyệt";
                    mail.Subject = "Thông báo xét duyệt đề tài";
                    
                   mail.To.Add(Email);
                     
                    mail.IsBodyHtml = true;
                    SmtpClient smtp = new SmtpClient();
                    smtp.Host = "smtp.gmail.com";
                    smtp.EnableSsl = true;
                    NetworkCredential networkCredential = new NetworkCredential(from1, pass);
                    smtp.UseDefaultCredentials = true;
                    smtp.Credentials = networkCredential;
                    smtp.Port = 587;
                    smtp.Send(mail);
                }
            }
        }
        [HttpPost]
        
        public void xetduyetsv(TopicOfStudent topicOfStudent)
        {
            using (DHTDTTDNEntities1 entities = new DHTDTTDNEntities1())
            {
                TopicOfStudent topic = (from c in entities.TopicOfStudents
                                        where c.IdTp == topicOfStudent.IdTp
                                        select c).FirstOrDefault();
                entities.Database.ExecuteSqlCommand("update TopicOfStudent set Status=N'đã duyệt' where IdTp='" + topic.IdTp + "'");
                entities.SaveChanges();
            }
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