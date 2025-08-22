using ControllerExample.Filters;
using ControllerExample.Models;
using ControllerExample.Services;
using CsvHelper;
using Microsoft.AspNetCore.Mvc;
using Rotativa.AspNetCore;
using System.Diagnostics;
using System.Text.Json;

namespace ControllerExample.Controllers;

[TypeFilter(typeof(ResponseHeaderFilter), Arguments = new object[] { "X-Server-Info", "Prashant" })]
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

    [TypeFilter(typeof(PersonListActionFilter))]
    [TypeFilter(typeof(ResponseHeaderFilter), Arguments = new object[] { "X-Platform-Info", "Linux" })]
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
        return await Task.FromResult(new ViewAsPdf("PersonPDF", persons)
        {
            FileName = "Persons.pdf",
            PageSize = Rotativa.AspNetCore.Options.Size.A4,
            PageMargins = new Rotativa.AspNetCore.Options.Margins(20, 20, 20, 20),
            PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait
        });
    }

    [HttpGet]
    public async Task<IActionResult> PersonCSV()
    {
        var persons = _personService.GetPersons();
        var memoryStream = await _personService.GenerateCsvAsync(persons);
        return await Task.FromResult(File(memoryStream, "application/octet-stream", "Persons.csv"));
    }

    [HttpGet]
    public async Task<IActionResult> PersonCustomCSV()
    {
        var persons = _personService.GetPersons();
        var memoryStream = await _personService.GenerateCustomCsvAsync(persons);
        return await Task.FromResult(File(memoryStream, "application/octet-stream", "Persons.csv"));
    }

    [HttpGet]
    public async Task<IActionResult> PersonExcel()
    {
        var memoryStream = await _personService.GenerateExcelAsync();
        return await Task.FromResult(File(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Persons.xlsx"));
    }

    [HttpGet]
    public async Task<IActionResult> UpdateCountries()
    {
        return await Task.FromResult(View());
    }

    [HttpPost]
    public async Task<IActionResult> UpdateCountries(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            ViewBag.ErrorMessage = "File is not selected or empty.";
            return View();
        }
        if (!Path.GetExtension(file.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
        {
            ViewBag.ErrorMessage = "Only excel files are allowed.";
            return View();
        }
        if (file.Length > 10485760) // 10 MB
        {
            ViewBag.ErrorMessage = "File size exceeds the limit of 10 MB.";
            return View();
        }

        // int updatedCount = await _personService.UpdateContriesAsync(file);
        var countries = await _personService.GetContriesAsync(file);
        if (countries.Length == 0)
        {
            ViewBag.ErrorMessage = "No records were updated.";
            return View();
        }

        ViewBag.SuccessMessage = $"Countries updated successfully. Total records updated: {countries.Length}";
        ViewBag.Countries = countries;
        return View();
    }
}