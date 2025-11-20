using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WINIT.Models;

namespace WINIT.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _db;

        public HomeController(ILogger<HomeController> logger, AppDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        public async Task<IActionResult> Index(string domain = "all")
        {
            ViewBag.Domain = domain;

            var customerLogs = _db.LogCustomerIngestions.AsQueryable();
            var itemLogs = _db.LogItemIngestions.AsQueryable();

            var allCustomer = await customerLogs.ToListAsync();
            var allItem = await itemLogs.ToListAsync();

            // Combined metrics
            ViewBag.TotalRequests = allCustomer.Count + allItem.Count;
            ViewBag.Success202 = allCustomer.Count(x => x.HttpStatus == 202) + allItem.Count(x => x.HttpStatus == 202);
            ViewBag.Failure400 = allCustomer.Count(x => x.HttpStatus == 400) + allItem.Count(x => x.HttpStatus == 400);
            ViewBag.Processed = allCustomer.Count(x => x.ProcessStatus == "PROCESSED") + allItem.Count(x => x.ProcessStatus == "PROCESSED");
            ViewBag.Pending = allCustomer.Count(x => x.ProcessStatus == "PENDING") + allItem.Count(x => x.ProcessStatus == "PENDING");
            ViewBag.Errors = allCustomer.Count(x => x.ProcessStatus == "ERROR") + allItem.Count(x => x.ProcessStatus == "ERROR");

            ViewBag.ApiSuccessRate = ViewBag.TotalRequests > 0 ? Math.Round((double)ViewBag.Success202 / ViewBag.TotalRequests * 100, 1) : 0;
            ViewBag.ProcessingSuccessRate = ViewBag.Success202 > 0 ? Math.Round((double)ViewBag.Processed / ViewBag.Success202 * 100, 1) : 0;

            // Domain-specific
            ViewBag.CustomerTotal = allCustomer.Count;
            ViewBag.CustomerSuccessRate = allCustomer.Count > 0 ? Math.Round((double)allCustomer.Count(x => x.HttpStatus == 202) / allCustomer.Count * 100, 2) : 0;
            ViewBag.CustomerProcessingRate = allCustomer.Count(x => x.HttpStatus == 202) > 0 ? Math.Round((double)allCustomer.Count(x => x.ProcessStatus == "PROCESSED") / allCustomer.Count(x => x.HttpStatus == 202) * 100, 1) : 0;
            ViewBag.CustomerPending = allCustomer.Count(x => x.ProcessStatus == "PENDING");

            ViewBag.ItemTotal = allItem.Count;
            ViewBag.ItemSuccessRate = allItem.Count > 0 ? Math.Round((double)allItem.Count(x => x.HttpStatus == 202) / allItem.Count * 100, 2) : 0;
            ViewBag.ItemProcessingRate = allItem.Count(x => x.HttpStatus == 202) > 0 ? Math.Round((double)allItem.Count(x => x.ProcessStatus == "PROCESSED") / allItem.Count(x => x.HttpStatus == 202) * 100, 1) : 0;
            ViewBag.ItemPending = allItem.Count(x => x.ProcessStatus == "PENDING");

            // Logs for table
            var logs = new List<LogViewModel>();

            if (domain == "all" || domain == "customer")
                logs.AddRange(allCustomer.Select(x => new LogViewModel
                {
                    ReferenceId = x.LogId.ToString(),
                    Domain = "Customer",
                    ReceivedTime = x.RequestTime,
                    ApiStatus = x.HttpStatus,
                    ProcessingStatus = x.ProcessStatus,
                    ValidationErrors = x.ValidatonDetails,
                    RawPayload = x.RawPayload
                }));

            if (domain == "all" || domain == "item")
                logs.AddRange(allItem.Select(x => new LogViewModel
                {
                    ReferenceId = x.LogId.ToString(),
                    Domain = "Item",
                    ReceivedTime = x.RequestTime,
                    ApiStatus = x.HttpStatus,
                    ProcessingStatus = x.ProcessStatus,
                    ValidationErrors = x.ValidatonDetails,
                    RawPayload = x.RawPayload
                }));

            ViewBag.Logs = logs.OrderByDescending(x => x.ReceivedTime).Take(100).ToList();

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
