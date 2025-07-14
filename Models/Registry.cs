using System.ComponentModel.DataAnnotations;

namespace FormRequest.Models
{
    public class Registry
    {
        [Key]
        public int RegistryId { get; set; }

        public string? From { get; set; }

        public string? To { get; set; }

        
        [DataType(DataType.Date)]
        public DateTime DateReceived { get; set; }

        public string Remarks { get; set; }

        public bool IsValid { get; set; }

        public bool IsOnSite { get; set; }
        public bool IsInTransit { get; set; }

        public int FormReqDbId { get; set; }

        [Display(Name = "Driver Name")]
        public string? Driver { get; set; }

        public FormReqDb FormReqDb { get; set; }
    }

   
}
