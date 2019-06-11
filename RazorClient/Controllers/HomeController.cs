using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ConsulService.Models;
using Microsoft.AspNetCore.Mvc;
using RazorClient.Models;
using Newtonsoft.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

namespace RazorClient.Controllers
{
    public class HomeController : Controller
    {
        //const string apiUri = "https://localhost:44391/";
        const string apiUri = "https://localhost:44394/api/values";
        public IActionResult Index()
        {
            IEnumerable<Transaction> messages = JsonConvert.DeserializeObject<IEnumerable<Transaction>>(DoRequest( "/transactions"));
            return View("Index",messages);
        }

        public IActionResult Currencies()
        {
            IEnumerable<Currency> messages = JsonConvert.DeserializeObject<IEnumerable<Currency>>(DoRequest( "/currencies"));
            return View(messages);
        }

        public IActionResult Categories()
        {
            IEnumerable<Category> messages = JsonConvert.DeserializeObject<IEnumerable<Category>>(DoRequest("/categories"));
            return View(messages);
        }

        public IActionResult Transaction(int id)
        {
            if (id == 0) return View(new Transaction());
            var message = JsonConvert.DeserializeObject<Transaction>(DoRequest( "/transactions/" + id.ToString()));
            return View(message);
        }

        public IActionResult Currency(int id)
        {
            if (id == 0) return View(new Currency());
            var message = JsonConvert.DeserializeObject<Currency>(DoRequest( "/currencies/" + id.ToString()));
            return View(message);
        }

        public IActionResult Category(int id)
        {
            if (id == 0) return View(new Category());
            var message = JsonConvert.DeserializeObject<Category>(DoRequest("/categories/"+id.ToString()));
            return View(message);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeCategory(Category model)
        {
            DoRequest($"/categories/{model.Id}", "POST", model.ToString());
           
            return RedirectToAction("Categories", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> ChangeCurrency(Currency model)
        {
            DoRequest($"/currencies/{model.Id}", "POST", model.ToString());

            return RedirectToAction("Currencies", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> ChangeTransaction(Transaction model)
        {
            DoRequest($"/transactions/{model.Id}", "POST", model.ToString());

            return RedirectToAction("Index", "Home");
        }


        public IActionResult Delete(string what)
        {
            DoRequest( "/" + what,"DELETE");
            return Index();
        }

        public IActionResult Rabbit()
        {
            IEnumerable<LogMessage> messages = JsonConvert.DeserializeObject<IEnumerable<LogMessage>>(DoRequest( "/rabbit"));
            return View("Rabbit",messages);
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                
                if (JsonConvert.DeserializeObject<bool>(DoRequest("/login", "POST",
                    model.ToString())))
                {
                    await Authenticate(model.Name); 

                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Incorrect credentials");
            }
            return View(model);
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                if (
                    JsonConvert.DeserializeObject<bool>(DoRequest("/register", "POST",
                    model.ToString()
                    )))
                {

                    await Authenticate(model.Name);

                    return RedirectToAction("Index", "Home");
                }
                else
                    ModelState.AddModelError("", "Incorrect credentials");
            }
            return View(model);
        }

        private async Task Authenticate(string userName)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, userName)
            };
            if (userName.ToLower() == "admin")
                claims.Add(new Claim(ClaimsIdentity.DefaultRoleClaimType, "Admin"));
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        string DoRequest(string uri, string method = "GET", string data = "", string contentType = "application/json")
        {
            byte[] dataBytes = Encoding.UTF8.GetBytes(data);

            uri = apiUri + /*method.ToLower() +*/ uri;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            request.ContentLength = dataBytes.Length;
            request.ContentType = contentType;
            request.Method = method;

            if (method!= "GET")
            using (Stream requestBody = request.GetRequestStream())
            {
                requestBody.Write(dataBytes, 0, dataBytes.Length);
            }
            //try
            //{
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
        //    }
        //    catch {
        //        return "{}";
        //    }
        }
    }
}
