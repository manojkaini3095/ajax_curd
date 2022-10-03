using ajax_curd.Context;
using ajax_curd.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage;
using Newtonsoft.Json;
using System.Diagnostics;
using Azure.Core;
using System.Text;
using System.Net;

namespace ajax_curd.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly StudentDbContext _context;
        private readonly IConfiguration _configuration;

        public static List<StudentModel> students = new List<StudentModel>();

        public HomeController(StudentDbContext studentDbContext, ILogger<HomeController> logger, IConfiguration configuration)
        {
            _context = studentDbContext;
            _logger = logger;
            _configuration = configuration;

        }
        public IActionResult Index()
        {
            var data = _context.Student.ToList();
            students = data;
            return View(students);
        } 
        [HttpGet]
        public JsonResult GetDetailsById(int id)
        {

            JsonResponseViewModel model = new JsonResponseViewModel();
            var student = _context.Student.Where(d => d.Id.Equals(id)).FirstOrDefault();

            if (student != null)
            {
                model.ResponseCode = 0;
                model.ResponseMessage = JsonConvert.SerializeObject(student);
            }
            else
            {
                model.ResponseCode = 1;
                model.ResponseMessage = "No record available";
            }
            return Json(model);
        }
        [HttpPost]
        public JsonResult InsertStudent(IFormCollection formcollection)
        {
            JsonResponseViewModel model = new JsonResponseViewModel();

            StudentModel student = new StudentModel();
            student.Email = formcollection["email"];
            student.Name = formcollection["name"];


            if (student != null && !string.IsNullOrEmpty(student.Name) && !string.IsNullOrEmpty(student.Email))
            {
                //MAKE DB CALL and handle the response
                _context.Add(student);
                _context.SaveChanges();

                var data = Moldingdata(student, "Create");


                model.ResponseCode = 0;
                model.ResponseMessage = JsonConvert.SerializeObject(student);
            }
            else
            {
                model.ResponseCode = 1;
                model.ResponseMessage = "No record available";
            }
            return Json(model);
        }
        [HttpPut]
        public JsonResult UpdateStudent(IFormCollection formcollection)
        {
            StudentModel student = new StudentModel();
            student.Id = int.Parse(formcollection["id"]);
            student.Email = formcollection["email"];
            student.Name = formcollection["name"];
            JsonResponseViewModel model = new JsonResponseViewModel();

            if (student != null && !string.IsNullOrEmpty(student.Name) && !string.IsNullOrEmpty(student.Email))
            {

                //MAKE DB CALL and handle the response
                _context.Update(student);
                _context.SaveChanges();

                var data = Moldingdata(student, "Update");

                model.ResponseCode = 0;
                model.ResponseMessage = JsonConvert.SerializeObject(student);

            }
            else
            {
                model.ResponseCode = 1;
                model.ResponseMessage = "No record available";
            }
            return Json(model);
        }
        [HttpDelete]
        public JsonResult DeleteStudent(IFormCollection formcollection)
        {
            StudentModel student = new StudentModel();
            student.Id = int.Parse(formcollection["id"]);
            JsonResponseViewModel model = new JsonResponseViewModel();

            if (student != null && student.Id > 0)
            {
                //MAKE DB CALL and handle the response
                var std = _context.Student.Find(student.Id);
                _context.Student.Remove(std);
                _context.SaveChanges();

                var data = Moldingdata(student, "Delete");

                model.ResponseCode = 0;
                model.ResponseMessage = JsonConvert.SerializeObject(student);
            }
            else
            {
                model.ResponseCode = 1;
                model.ResponseMessage = "No record available";
            }
            return Json(model);
        }


        public Task Moldingdata(StudentModel std, string action)
        {
            List<CustomModel> cmodel = new List<CustomModel>();
            if (action == "Update")
            {
                cmodel.Add(new CustomModel { Student = std, Action = "Update" });
            }
            else if (action == "Create")
            {
                cmodel.Add(new CustomModel { Student = std, Action = "Create" });
            }
            else
            {
                cmodel.Add(new CustomModel { Student = std, Action = "Delete" });
            }

            var data = PostApi(cmodel.FirstOrDefault());
            return Task.CompletedTask;
        }

        public string PostApi(CustomModel postData)
        {
            try
            {
                //var ApiUrl = "https://studentdatareadfunction20220930133806.azurewebsites.net";
                //var ApiUrl = "http://localhost:7134/api/StudentDataReadFunction";
                var ApiUrl = "https://studentdatareadfunction20220930133806.azurewebsites.net/api/StudentDataReadFunction?";

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ApiUrl);

                var data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(postData));

                request.Method = "post";
                request.ContentType = "application/json; charset=utf-8";
                request.ContentLength = data.Length;
                using (var stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
                var response = (HttpWebResponse)request.GetResponse();
                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                return responseString;
            }
            catch(Exception ex)
            {
                return ex.Message;
            }
        }
    }
}