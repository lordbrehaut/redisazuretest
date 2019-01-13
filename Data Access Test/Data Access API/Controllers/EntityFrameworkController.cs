using DataAccessAPI.DataTransferObjects;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DataAccessAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EntityFrameworkController : ControllerBase
    {
        private readonly CountryContext _countryContext;

        public EntityFrameworkController(CountryContext countryContext) {
            _countryContext = countryContext;
        }

        [HttpGet]
        public ActionResult<ResultDto> Get()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var result = _countryContext.Countries.ToList();
            stopwatch.Stop();
            return new ResultDto
            {
                Countries = result,
                GetTiming = stopwatch.ElapsedMilliseconds
            };
        }
    }
}