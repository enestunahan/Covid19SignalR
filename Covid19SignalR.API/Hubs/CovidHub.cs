using Covid19SignalR.API.Models;
using Microsoft.AspNetCore.SignalR;

namespace Covid19SignalR.API.Hubs
{
    public class CovidHub : Hub
    {
        private readonly CovidService _covidService;

        public CovidHub(CovidService covidService)
        {
            _covidService = covidService;
        }

        public async Task GetCovidListAsync()
        {
            await Clients.All.SendAsync("ReceiveCovidList", _covidService.GetCovidChartList());
        }
    }
}
