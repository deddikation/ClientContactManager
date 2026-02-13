using Microsoft.AspNetCore.Mvc;

namespace ClientContactManager.WebUI.Controllers;

public class HomeController : Controller
{
    public IActionResult Index() => View();
}
