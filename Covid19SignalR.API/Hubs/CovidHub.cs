using Microsoft.AspNetCore.SignalR;

namespace Covid19SignalR.API.Hubs
{
    public class CovidHub : Hub
    {
        public async Task GetCovidListAsync()
        {
            await Clients.All.SendAsync("ReceiveCovidList", "servisten covid 19 verilerini al");
        }
    }
}
