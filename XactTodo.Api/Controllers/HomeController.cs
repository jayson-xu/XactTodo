using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace XactTodo.Api.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            /*var absoluteUri = string.Concat(Request.Scheme, "://", Request.Host.ToUriComponent(), Request.PathBase.ToUriComponent(), Request.Path.ToUriComponent());
            var swaggerUri = Path.Combine(absoluteUri, "swagger");
            return new RedirectResult(swaggerUri);*/
            return new RedirectResult("~/swagger");
        }
    }
}