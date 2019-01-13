using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using DataAccessAPI.DataTransferObjects;
using DataAccessAPI.Models;
using DataAccessAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DataAccessAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RedisController : ControllerBase
    {
        private readonly IRedisService _redisService;
        private readonly CountryContext _countryContext;

        public RedisController(IRedisService redisService, CountryContext countryContext)
        {
            _redisService = redisService;
            _countryContext = countryContext;
        }

        [HttpGet]
        public async Task<ActionResult<ResultDto>> Get()
        {
            string countriesString = JsonConvert.SerializeObject(_countryContext.Countries.ToList());
            var setStopwatch = new Stopwatch();
            setStopwatch.Start();
            var setRedisTime = await _redisService.SetInCache("Countries", countriesString);
            setStopwatch.Stop();

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var result = await _redisService.GetFromCache("Countries");
            stopwatch.Stop();
            return new ResultDto
            {
                Countries = JsonConvert.DeserializeObject<List<Country>>(result.Item1),
                GetTiming = stopwatch.ElapsedTicks,
                GetRedisTiming = stopwatch.ElapsedTicks,
                SetTiming = setStopwatch.ElapsedTicks,
                SetRedisTiming = setRedisTime
            };
        }
    }
}