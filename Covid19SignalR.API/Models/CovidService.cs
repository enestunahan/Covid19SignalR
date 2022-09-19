using Covid19SignalR.API.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Covid19SignalR.API.Models
{
    public class CovidService
    {
        private readonly AppDbContext _context;
        private readonly IHubContext<CovidHub> _hubContext;

        public CovidService(AppDbContext context, IHubContext<CovidHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        public  IQueryable<Covid> GetList()
        {
            return _context.Covids.AsQueryable();
        }

        public async Task SaveCovid (Covid covid)
        {
            await _context.Covids.AddAsync(covid);
            await _context.SaveChangesAsync();

            // Db'ye yeni kayıt geldiğine client'ı haberdar ediyoruz
            await _hubContext.Clients.All.SendAsync("ReceiveCovidList", GetCovidChartList());
        }

        public List<CovidChart> GetCovidChartList()
        {
            List<CovidChart> charts = new List<CovidChart>();

            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandType = System.Data.CommandType.Text;

                command.CommandText = @"select tarih , [1] ,[2],[3],[4],[5] from 
                                        (select [City] , [Count] , CAST ([CovidDate] as date) as tarih from Covids) as covidT
                                        PIVOT
                                        (SUM(COUNT) FOR City IN ( [1] ,[2],[3],[4],[5] )) AS ptable
                                        order by tarih desc";

                _context.Database.OpenConnection();

                using(var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        CovidChart chart = new CovidChart();

                        chart.CovidDate = reader.GetDateTime(0).ToShortDateString();

                        Enumerable.Range(1, 5).ToList().ForEach(x =>
                        {
                            if (System.DBNull.Value.Equals(reader[x]))
                            {
                                chart.Counts.Add(0);
                            }
                            else
                            {
                                chart.Counts.Add(reader.GetInt32(x));
                            }


                        });
                        charts.Add(chart);
                    }
                }

                _context.Database.CloseConnection();

            }
            return charts;
        }


    }
}
