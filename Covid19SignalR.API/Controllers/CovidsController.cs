using Covid19SignalR.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace Covid19SignalR.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CovidsController : ControllerBase
    {
        private readonly CovidService _covidService;

        public CovidsController(CovidService covidService)
        {
            _covidService = covidService;   
        }


        [HttpPost]
        public async Task<IActionResult> SaveCovid(Covid covid)
        {
            await _covidService.SaveCovid(covid);
            //IQueryable<Covid> covidList = _covidService.GetList();
            return Ok(_covidService.GetCovidChartList());
        }

        [HttpGet]
        public  IActionResult InitializeCovid()
        {

            Random random = new Random();
            Enumerable.Range(1, 10).ToList().ForEach(x =>
            {
               
                foreach(ECity item in Enum.GetValues(typeof(ECity)))
                {
                    var newCovid = new Covid { City = item, Count = random.Next(100, 1000), CovidDate = DateTime.Now.AddDays(x) };
                    _covidService.SaveCovid(newCovid).Wait();  // işlemi burada blokla devam etmesine izin verme demek
                    Thread.Sleep(1000);
                }
            
            });

            return Ok("Datalar başarıyla db ye kaydedildi");
        }
    }
}
