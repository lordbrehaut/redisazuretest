using DataAccessAPI.Models;
using System.Collections.Generic;

namespace DataAccessAPI.DataTransferObjects
{
    public class ResultDto
    {
        public List<Country> Countries { get; set; }
        public long SetTiming { get; set; }
        public long SetRedisTiming { get; set; }
        public long GetTiming { get; set; }
        public long GetRedisTiming { get; set; }        
    }
}
