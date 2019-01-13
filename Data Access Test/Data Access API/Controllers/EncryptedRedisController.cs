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
    public class EncryptedRedisController : ControllerBase
    {
        private readonly IEncryptedRedisService _encryptedRedisService;
        private readonly CountryContext _countryContext;

        public EncryptedRedisController(IEncryptedRedisService encryptedRedisService, CountryContext countryContext)
        {
            _encryptedRedisService = encryptedRedisService;
            _countryContext = countryContext;
        }

        [HttpGet]
        public async Task<ActionResult<ResultDto>> Get()
        {
            string countriesString = JsonConvert.SerializeObject(_countryContext.Countries.ToList());
            var setStopwatch = new Stopwatch();
            setStopwatch.Start();
            var setRedisTime = await _encryptedRedisService.SetInCache("Countries", countriesString);
            setStopwatch.Stop();

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var result = await _encryptedRedisService.GetFromCache("Countries");
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