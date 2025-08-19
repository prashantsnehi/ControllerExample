using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ControllerExample.Models;
using System.Text.Json;
using ControllerExample.Services;
using Rotativa.AspNetCore;
using CsvHelper;
using System.Threading.Tasks;

namespace ControllerExample.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IPersonService _personService;
    private readonly IPersonService _personService1;
    private readonly IPersonService _personService2;

    public HomeController(ILogger<HomeController> logger, IPersonService personService,
        IPersonService personService1, IPersonService personService2)
    {
        _logger = logger;
        _personService = personService;
        _personService1 = personService1;
        _personService2 = personService2;
    }

    public IActionResult Index()
    {
        // ViewBag.Source = Request.Path;
        // ViewBag.Accept = Request.Headers.Accept;
        // ViewBag.Browser = Request.Headers.UserAgent;
        // ViewBag.ContentType = Request.ContentType;
        // ViewBag.IPAddress = Request.HttpContext.Connection.RemoteIpAddress;

        // ViewBag.Status = Response.StatusCode;
        // ViewBag.ResponseContentType = Response.ContentType;

        // ViewData["Instance1"] = _personService.ServiceInstanceId;
        // ViewData["Instance2"] = _personService1.ServiceInstanceId;
        // ViewData["Instance3"] = _personService2.ServiceInstanceId;

        return View();
    }

    public IActionResult Privacy()
    {
        ViewBag.Source = Request.Path;
        ViewBag.Accept = Request.Headers.Accept;
        ViewBag.Browser = Request.Headers.UserAgent;
        ViewBag.ContentType = Request.ContentType;
        ViewBag.IPAddress = Request.HttpContext.Connection.RemoteIpAddress;

        ViewBag.Status = Response.StatusCode;
        ViewBag.ResponseContentType = Response.ContentType;
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    [HttpPost]
    public IActionResult GetPerson(Person person)
    {
        if (!ModelState.IsValid)
        {
            // List<string> errorList = new List<string>();
            // foreach (var value in ModelState.Values)
            // {
            //     foreach (var error in value.Errors)
            //     {
            //         errorList.Add(error.ErrorMessage);
            //     }
            // }
            // var errorList = ModelState.Values.SelectMany(value => value.Errors)
            //     .SelectMany(err => err.ErrorMessage)
            //     .ToList();
            //  return BadRequest(string.Join("/n", errorList));
            return BadRequest(string.Join("/n",
                ModelState.Values.SelectMany(value => value.Errors)
                    .SelectMany(err => err.ErrorMessage)
                    .ToList())
            );

        }
        return Content(JsonSerializer.Serialize(person));
    }

    [HttpGet]
    public IActionResult GetPersons()
    {
        var persons = _personService.GetPersons();
        return PartialView("_PersonsPartial", persons);
    }

    [HttpGet]
    public IActionResult GetPersonGrid()
    {
        return ViewComponent("Grid", "From Controller");
    }

    [HttpPost]
    public IActionResult AddPerson(Person person)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(string.Join("/n",
                ModelState.Values.SelectMany(value => value.Errors)
                    .SelectMany(err => err.ErrorMessage)
                    .ToList())
            );
        }

        _personService.AddPerson(person);
        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> PersonPDF()
    {
        var persons = _personService.GetPersons();
        // var pdfBytes = await PdfGenerator.GeneratePdfAsync(persons);
        // return File(pdfBytes, "application/pdf", "Persons.pdf");
        return new ViewAsPdf("PersonPDF", persons)
        {
            FileName = "Persons.pdf",
            PageSize = Rotativa.AspNetCore.Options.Size.A4,
            PageMargins = new Rotativa.AspNetCore.Options.Margins(20, 20, 20, 20),
            PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait
        };
    }

    [HttpGet]
    public async Task<IActionResult> PersonCSV()
    {
        var persons = _personService.GetPersons();
        var memoryStream = await _personService.GenerateCsvAsync(persons);
        return File(memoryStream, "application/octet-stream", "Persons.csv");
    }
    
    [HttpGet]
    public async Task<IActionResult> PersonCustomCSV()
    {
        var persons = _personService.GetPersons();
        var memoryStream = await _personService.GenerateCustomCsvAsync(persons);  
        return File(memoryStream, "application/octet-stream","Persons.csv");
    }
}
