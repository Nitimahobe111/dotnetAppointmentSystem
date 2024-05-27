using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Numerics;
using System.Security.Claims;
using System.Text;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class AuthController : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly AppointmentSystemDbContext _dbContext;
        private readonly DoctorCredentials _doctorCredentials;


        private readonly IConfiguration _configuration;

        public AuthController(AppointmentSystemDbContext appointmentSystemDbContext,IConfiguration configuration)
        {
            
            _configuration = configuration;
            _dbContext = appointmentSystemDbContext;
        }

        [HttpGet]
        [Route("login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(Login model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                var role = "patient";
                var user = _dbContext.Registration.FirstOrDefault(i => i.Email == model.Email);
                if (user==null && model.Password== "123456" && model.Email == "doctor@demo.com")
                {
                    role = "doctor";
                    var a = new PatientModel()
                    {
                        Email= "doctor@demo.com"
                    };

                    var token = GenerateJwtToken(a, role);
                    HttpContext.Session.SetString("BearerToken", token);

                    return Redirect(returnUrl ?? "get");
                }
                else if(user != null)
                {
                    var token = GenerateJwtToken(user, role);
                    ViewBag.Token = token;
                    return Redirect("getpatientdetail/" +  model.Email );
                }
            }
            return View(model);
        }


        [Route("")]
        public async Task<IActionResult> RegistrationForm()
        {
            return View();
        }

        [HttpPost]
        [Route("")]
        public async Task<IActionResult> Registration([FromForm] PatientModel patientModel, IFormFile profilePic)
        {
            byte[] profilePictureBytes = null;
            if (patientModel.pic != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await patientModel.pic.CopyToAsync(memoryStream);
                    profilePictureBytes = memoryStream.ToArray();
                }
            }
            patientModel.ProfilePicture = profilePictureBytes;
            patientModel.pic=null;
            _dbContext.Registration.Add(patientModel);
            _dbContext.SaveChanges();
            return View(patientModel);
        }
       
        [HttpGet]
        [Route("get")]
        public IActionResult GetAll()
        {
            var patients = _dbContext.Registration.ToList();
            return View(patients);
        }
       
        [HttpPut]
        [Authorize(Roles = "doctor")]
        [Route("changepatientstatus/{patientId}")]
        public IActionResult ChangePatientStatus(string patientId)
        {
            var patientDetails = _dbContext.Registration.Find(patientId);
            if (patientDetails != null)
            {
                patientDetails.IsActive = !patientDetails.IsActive;

            }
            _dbContext.SaveChanges();
            return View("GetPatientDetails", patientDetails);
        }

        [HttpGet]
        [Authorize(Roles = "patient")]
        [Route("getappointment/{patientId}")]
        public IActionResult GetAppointment(string patientId)
        {
            _dbContext.Appointment.Find(patientId);
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "patient")]
        [Route("getallappointment")]
        public IActionResult GetAllAppointment()
        {
            _dbContext.Appointment.Find();
            return View();
        }

        [Authorize(Roles = "patient")]
        [Route("getallappointment")]
        public IActionResult UpdateAppointmentForm()
        {
            return View();
        }

        [HttpPut]
        [Authorize(Roles = "patient")]
        [Route("updateappointment")]
        public IActionResult UpdateAppointment([FromBody] AppointmentModel appointmentModel)
        {

           var appointment= _dbContext.Appointment.Find(appointmentModel.Id);
            appointment.StartTime = appointmentModel.StartTime;
            appointment.EndTime = appointmentModel.EndTime;
            _dbContext.SaveChanges();

            return View();
        }

        [Route("updatepatientdetailsform")]
        public IActionResult UpdatePatientDetailsForm()
        {
            return View();
        }
        [HttpPut]
        [Authorize(Roles = "patient")]
        [Route("updatepatientdetails")]
        public async Task<IActionResult> UpdatePatientDetailsAsync([FromBody] PatientModel patientModel)
        {

            var patient = _dbContext.Registration.Find(patientModel.Id);

            patient.FirstName = patientModel?.FirstName;
            patient.LastName = patientModel?.LastName;
            patient.PhoneNumber = patientModel?.PhoneNumber;
            patient.DateOfBirth = patientModel.DateOfBirth;
            byte[] profilePictureBytes = null;
            if (patientModel.pic != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await patientModel.pic.CopyToAsync(memoryStream);
                    profilePictureBytes = memoryStream.ToArray();
                }
            }
            patient.ProfilePicture = profilePictureBytes;
            _dbContext.SaveChanges();

            return View();
        }

        [HttpGet]
        [Authorize(Roles = "patient")]
        [Route("createappointment/{patientId}")]
        public IActionResult CreateAppointment(string patientId)
        {
            ViewBag.PatientId = patientId;
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "patient")]
        [Route("createappointment")]
        public IActionResult CreateAppointment([FromBody] AppointmentModel appointment)
       {
           _dbContext.Appointment.Any(a=>a.StartTime < appointment.EndTime &&
                             a.EndTime > appointment.StartTime);
            _dbContext.Appointment.Add(appointment);
            _dbContext.SaveChanges();
           return View();
        }

        [HttpPut]
        [Route("editpatientdetails/{patientId}")]
        public IActionResult EditPatientDetails(string patientId)
        {
            var patientDetails = _dbContext.Registration.Find(patientId);
            if (patientDetails != null)
            {
                patientDetails.IsActive = !patientDetails.IsActive;

            }
            _dbContext.SaveChanges();
            return View("GetPatientDetails", patientDetails);
        }

        [HttpGet]
        [Route("gets")]
        [Authorize(Roles = "doctor")]
        public IActionResult GetAlls()
        {
            var patients = _dbContext.Registration.ToList();
            return View(patients);
        }
        [HttpGet]
        [Authorize(Roles = "doctor")]
        [Route("getpatientdetails/{patientId}")]
        public IActionResult GetPatientDetails(string patientId)
        {
            var patientDetails = _dbContext.Registration.Find(patientId);
            if (patientDetails == null)
            {
                return NotFound();
            }

            return View("GetPatientDetails", patientDetails);
        }
        [HttpGet]
        //[Authorize(Roles = "patient")]
        [Route("getpatientdetail/{email}")]
        public IActionResult GetPatientDetail(string email)
        {
            var patientDetails = _dbContext.Registration.FirstOrDefault(i => i.Email == email);
            if (patientDetails == null)
            {
                return NotFound();
            }

            return View("GetPatientDetail", patientDetails);
        }

        private string GenerateJwtToken(PatientModel user,string role)
        {
            var mail = "doctor@demo.com";
            var id = "a";
            if (role=="patient")
            {
                 mail = user.Email;
                 id = user.Id;       
            }
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, mail),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, "id"),
                new Claim(ClaimTypes.Role, role)

            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(500),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);

        }
    }
}
