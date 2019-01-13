using System.ComponentModel.DataAnnotations;

namespace DataAccessAPI.Models
{
    public class Country
    {
        [Key]
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public bool IsOfac { get; set; }
        public bool IsRestricted { get; set; }
    }
}
