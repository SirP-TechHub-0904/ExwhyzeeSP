using ExwhyzeeSP.Web.Areas.Service;
using ExwhyzeeSP.Core.Models;
using ExwhyzeeSP.Data.Contexts;
using ExwhyzeeSP.Core.Dtos.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Text;
using RestSharp;
using Newtonsoft.Json;
using ExwhyzeeSP.Core.Dtos.Zoom;

namespace ExwhyzeeSP.Web.Controllers
{
    [Microsoft.AspNetCore.Mvc.Route("api/liveclass/[action]")]
    [Microsoft.AspNetCore.Mvc.ApiController]
    public class LiveClassApiController : ApiController
    {
        public ApplicationDbContext db = new ApplicationDbContext();
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        [Microsoft.Extensions.DependencyInjection.ActivatorUtilitiesConstructor]
        public LiveClassApiController()
        {
        }

        public LiveClassApiController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;

        }


        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpHelper.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpHelper.Current.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        //  [HttpGet]
        //[ResponseType(typeof(ApiSchoolInfoDto))]
        //[Route("api/schoolinfo")]
        public IActionResult GetUsers()
        {
            try
            {
                List<User> userdata = new List<User>();
                var contents = TokenManager.GetUsers();
                    //userdata = JsonConvert.DeserializeObject<List<ZoomUserDto>>(response.Content);
                    Rootobject datalist = JsonConvert.DeserializeObject<Rootobject>(contents);
                    userdata = datalist.users.ToList();
                   

        }
            catch(Exception c) { }
            return null;
        }


        public IActionResult GetUsersMeetings()
        {
            try
            {
                var token = TokenManager.GenerateToken();
                ///v2/accounts/128273138/users/{userId}/meetings
                /////https://api.zoom.us/v2/users
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)768 | (SecurityProtocolType)3072;
                var client = new RestClient("https://api.zoom.us/v2/users/exwhyzeetech@gmail.com/meetings"); var request = new RestRequest("", Method.Get);
                request.AddHeader("content-type", "application/json");
                request.AddHeader("authorization", "Bearer" + token);

                request.AddQueryParameter("type", "scheduled");
                request.AddQueryParameter("page_size", "30");
                request.AddQueryParameter("page_number", "1");
                RestResponse response = client.Execute(request);

                if (response.IsSuccessful)
                {
                    var contents = response.Content.ToString();
                    return Ok(contents);
                }

            }
            catch (Exception c) { }
            return null;
        }


    }
}


