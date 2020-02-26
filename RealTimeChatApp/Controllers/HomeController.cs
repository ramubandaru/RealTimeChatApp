using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealTimeChatApp.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home  
        public ActionResult Index()
        {
            return View();
        }
    }
}
