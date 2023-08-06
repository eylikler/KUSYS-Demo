using KUSYS_Demo.WebApplication.Authorization;
using KUSYS_Demo.WebApplication.DTOs.Courses;
using KUSYS_Demo.WebApplication.DTOs.Students;
using KUSYS_Demo.WebApplication.DTOs.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace KUSYS_Demo.WebApplication.Controllers
{
    public class StudentController : BaseController
    {
        public StudentController(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
        {

        }

        public async Task<IActionResult> Index()
        {
            List<StudentDto> studentList = new List<StudentDto>();

            var isAdmin = await RoleCheck("Admin");
            ViewData["IsAdmin"] = isAdmin;

            var response = await GetStudents();
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                studentList = JsonSerializer.Deserialize<List<StudentDto>>(responseContent);
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                var responseMsg = JsonSerializer.Deserialize<MessageDto>(responseContent);

                ViewData["Message"] = responseMsg.message;
                return View(studentList);
            }

            return View(studentList);
        }

        public async Task<IActionResult> Details(int id)
        {
            var userDetails = await GetStudentById(id);

            if (userDetails == null)
            {
                //return NotFound();
                return RedirectToAction(nameof(Index));
            }

            return View(userDetails);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StudentDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ViewData["ErrorsList"] = GetModelStateErrors();

                    return View(model);
                }

                var response = await CreateStudent(model);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    ViewData["Message"] = "Kayıt başarılı.";
                    return RedirectToAction(nameof(Index));
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    var responseMsg = JsonSerializer.Deserialize<MessageDto>(responseContent);

                    ViewData["Message"] = responseMsg.message;
                    return View(model);
                }
            }
            catch
            {
                ViewData["Message"] = "Bir hata oluştu.";
                return View(model);
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            var user = await GetStudentById(id);

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, StudentDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ViewData["ErrorsList"] = GetModelStateErrors();

                    return View(model);
                }

                var response = await UpdateStudentById(id, model);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    ViewData["Message"] = "Güncelleme başarılı.";
                    return RedirectToAction(nameof(Index));
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    var responseMsg = JsonSerializer.Deserialize<MessageDto>(responseContent);

                    ViewData["Message"] = responseMsg.message;
                    return View(model);
                }
            }
            catch
            {
                ViewData["Message"] = "Bir hata oluştu.";
                return View(model);
            }
        }

        //public async Task<IActionResult> Delete(int id)
        //{
        //    ViewData["Title"] = "Öğrenci Sil";

        //    return View();
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var response = await DeleteStudent(id);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return Json(new { success = true, message = "Öğrenci başarılı şekilde silindi." });
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    return Json(new { success = false, message = "Yetkisiz İşlem!" });
                }
                else
                {
                    var responseMsg = JsonSerializer.Deserialize<MessageDto>(responseContent);

                    return Json(new { success = false, message = responseMsg.message });
                }
            }
            catch
            {
                return Json(new { success = false, message = "Bir hata oluştu." });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMatchedCourse(int id)
        {
            try
            {
                var response = await DeleteStudentCourse(id);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return Json(new { success = true, message = "Ders eşleştirmesi kaldırıldı." });
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    return Json(new { success = false, message = "Yetkisiz İşlem!" });
                }
                else
                {
                    var responseMsg = JsonSerializer.Deserialize<MessageDto>(responseContent);

                    return Json(new { success = false, message = responseMsg.message });
                }
            }
            catch
            {
                return Json(new { success = false, message = "Bir hata oluştu." });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmCourseMatching(int studentId, string courseId)
        {
            try
            {
                var studentCourseDto = new StudentCourseDto()
                {
                    studentId = studentId,
                    courseId = courseId
                };

                var response = await InsertStudentCourse(studentCourseDto);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return Json(new { success = true, message = "Ders eşleştirmesi yapıldı." });
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    return Json(new { success = false, message = "Yetkisiz İşlem!" });
                }
                else
                {
                    var responseMsg = JsonSerializer.Deserialize<MessageDto>(responseContent);

                    return Json(new { success = false, message = responseMsg.message });
                }
            }
            catch
            {
                return Json(new { success = false, message = "Bir hata oluştu." });
            }
        }

        public async Task<IActionResult> StudentCourses(int id)
        {
            List<StudentCourseDto> studentCoursesList = new List<StudentCourseDto>();

            ViewData["StudentId"] = id;

            var response = await GetStudentCourses(id);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                studentCoursesList = JsonSerializer.Deserialize<List<StudentCourseDto>>(responseContent);
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                var responseMsg = JsonSerializer.Deserialize<MessageDto>(responseContent);

                ViewData["Message"] = responseMsg.message;
                return View(studentCoursesList);
            }

            return View(studentCoursesList);
        }

        public async Task<IActionResult> CourseMatching(int id)
        {
            List<CourseDto> coursesList = new List<CourseDto>();

            ViewData["StudentId"] = id;

            var response = await GetUnmatchedCourses(id);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                coursesList = JsonSerializer.Deserialize<List<CourseDto>>(responseContent);
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                var responseMsg = JsonSerializer.Deserialize<MessageDto>(responseContent);

                ViewData["Message"] = responseMsg.message;
                return View(coursesList);
            }

            return View(coursesList);
        }

        // helper methods

        public async Task<UserDto> GetAuthUser()
        {
            var apiUrl = "http://localhost:4000/Users/auth";

            try
            {
                var httpClient = _httpClientFactory.CreateClient();

                var accessToken = HttpContext.Session.GetString("_JWToken");

                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
                var response = await httpClient.GetAsync(apiUrl);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var userDto = JsonSerializer.Deserialize<UserDto>(responseContent);

                    return userDto;
                }
            }
            catch (Exception)
            {

            }

            return null;
        }

        public async Task<bool> RoleCheck(string roleName)
        {
            var apiUrl = "http://localhost:4000/Users/rolecheck?roleName=" + roleName;

            bool isAdmin = false;

            try
            {
                var httpClient = _httpClientFactory.CreateClient();

                var accessToken = HttpContext.Session.GetString("_JWToken");

                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
                var response = await httpClient.GetAsync(apiUrl);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    isAdmin = JsonSerializer.Deserialize<bool>(responseContent);
                }
            }
            catch (Exception)
            {

            }

            return isAdmin;
        }

        public async Task<HttpResponseMessage> GetStudents()
        {
            var apiUrl = "http://localhost:4000/student";

            HttpResponseMessage response;

            try
            {
                var httpClient = _httpClientFactory.CreateClient();

                var accessToken = HttpContext.Session.GetString("_JWToken");

                if (accessToken == null)
                {
                    return new HttpResponseMessage(HttpStatusCode.Unauthorized);
                }

                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
                return await httpClient.GetAsync(apiUrl);
            }
            catch (Exception)
            {
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }

        public async Task<StudentDto> GetStudentById(int id)
        {
            var apiUrl = "http://localhost:4000/student/" + id;

            StudentDto student = new StudentDto();

            try
            {
                var httpClient = _httpClientFactory.CreateClient();

                var accessToken = HttpContext.Session.GetString("_JWToken");

                if (accessToken == null)
                {
                    return null;
                }

                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
                var studentListResponse = await httpClient.GetAsync(apiUrl);
                var studentListResponseContent = await studentListResponse.Content.ReadAsStringAsync();

                if (studentListResponse.StatusCode == HttpStatusCode.OK)
                {
                    return JsonSerializer.Deserialize<StudentDto>(studentListResponseContent);
                }
            }
            catch (Exception)
            {

            }

            return null;
        }

        public async Task<HttpResponseMessage> UpdateStudentById(int id, StudentDto studentDto)
        {
            var apiUrl = "http://localhost:4000/student/" + id;

            try
            {
                string json = JsonSerializer.Serialize(studentDto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var httpClient = _httpClientFactory.CreateClient();

                var accessToken = HttpContext.Session.GetString("_JWToken");

                if (accessToken == null)
                {
                    return new HttpResponseMessage(HttpStatusCode.Unauthorized);
                }

                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
                return await httpClient.PutAsync(apiUrl, content);
            }
            catch (Exception)
            {
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }

        public async Task<HttpResponseMessage> CreateStudent(StudentDto studentDto)
        {
            var apiUrl = "http://localhost:4000/student/insert";

            try
            {
                string json = JsonSerializer.Serialize(studentDto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var httpClient = _httpClientFactory.CreateClient();

                var accessToken = HttpContext.Session.GetString("_JWToken");

                if (accessToken == null)
                {
                    return new HttpResponseMessage(HttpStatusCode.Unauthorized);
                }

                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
                return await httpClient.PostAsync(apiUrl, content);
            }
            catch (Exception)
            {
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }

        public async Task<HttpResponseMessage> DeleteStudent(int id)
        {
            var apiUrl = "http://localhost:4000/student/" + id;

            try
            {
                var httpClient = _httpClientFactory.CreateClient();

                var accessToken = HttpContext.Session.GetString("_JWToken");

                if (accessToken == null)
                {
                    return new HttpResponseMessage(HttpStatusCode.Unauthorized);
                }

                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
                return await httpClient.DeleteAsync(apiUrl);
            }
            catch (Exception)
            {
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }

        public async Task<HttpResponseMessage> DeleteStudentCourse(int id)
        {
            var apiUrl = "http://localhost:4000/student/courses/" + id;

            try
            {
                var httpClient = _httpClientFactory.CreateClient();

                var accessToken = HttpContext.Session.GetString("_JWToken");

                if (accessToken == null)
                {
                    return new HttpResponseMessage(HttpStatusCode.Unauthorized);
                }

                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
                return await httpClient.DeleteAsync(apiUrl);
            }
            catch (Exception)
            {
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }

        public async Task<HttpResponseMessage> GetStudentCourses(int studentId)
        {
            var apiUrl = "http://localhost:4000/student/courses/" + studentId;

            try
            {
                var httpClient = _httpClientFactory.CreateClient();

                var accessToken = HttpContext.Session.GetString("_JWToken");

                if (accessToken == null)
                {
                    return new HttpResponseMessage(HttpStatusCode.Unauthorized);
                }

                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
                return await httpClient.GetAsync(apiUrl);
            }
            catch (Exception)
            {
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }

        public async Task<HttpResponseMessage> GetUnmatchedCourses(int studentId)
        {
            var apiUrl = "http://localhost:4000/student/unmatchedcourses/" + studentId;

            HttpResponseMessage response;

            try
            {
                var httpClient = _httpClientFactory.CreateClient();

                var accessToken = HttpContext.Session.GetString("_JWToken");

                if (accessToken == null)
                {
                    return new HttpResponseMessage(HttpStatusCode.Unauthorized);
                }

                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
                return await httpClient.GetAsync(apiUrl);
            }
            catch (Exception)
            {
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }

        public async Task<HttpResponseMessage> InsertStudentCourse(StudentCourseDto studentCourseDto)
        {
            var apiUrl = "http://localhost:4000/student/courses";

            try
            {
                string json = JsonSerializer.Serialize(studentCourseDto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var httpClient = _httpClientFactory.CreateClient();

                var accessToken = HttpContext.Session.GetString("_JWToken");

                if (accessToken == null)
                {
                    return new HttpResponseMessage(HttpStatusCode.Unauthorized);
                }

                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
                return await httpClient.PostAsync(apiUrl, content);
            }
            catch (Exception)
            {
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }

        private List<string> GetModelStateErrors()
        {
            return ModelState.Values
                .SelectMany(modelState => modelState.Errors)
                .Select(error => error.ErrorMessage)
                .ToList();
        }
    }
}
