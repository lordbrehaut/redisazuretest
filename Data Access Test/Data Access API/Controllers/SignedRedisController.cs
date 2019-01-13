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
    [Route("api/signedredis")]
    [ApiController]
    public class SignedRedisController : ControllerBase
    {
        private readonly ISignedRedisService _signedRedisService;
        private readonly CountryContext _countryContext;

        public SignedRedisController(ISignedRedisService signedRedisService, CountryContext countryContext)
        {
            _signedRedisService = signedRedisService;
            _countryContext = countryContext;
        }

        [HttpGet]
        public async Task<ActionResult<ResultDto>> Get()
        {
            string countriesString = JsonConvert.SerializeObject(_countryContext.Countries.ToList());
            var setStopwatch = new Stopwatch();
            setStopwatch.Start();
            var setRedisTime = await _signedRedisService.SetInCache("Countries", countriesString);
            setStopwatch.Stop();

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var result = await _signedRedisService.GetFromCache("Countries");
            stopwatch.Stop();
            return new ResultDto
            {
                Countries = JsonConvert.DeserializeObject<List<Country>>(result.Item1),
                GetTiming = stopwatch.ElapsedTicks,
                GetRedisTiming = result.Item2,
                SetTiming = setStopwatch.ElapsedTicks,
                SetRedisTiming = setRedisTime
            };
        }
    }
}