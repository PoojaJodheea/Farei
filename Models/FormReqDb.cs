using System.ComponentModel.DataAnnotations;

namespace FormRequest.Models
{
    public class FormReqDb
    {
        [Key]
        public int Id { get; set; }

        // General Form Details
      
        [DataType(DataType.Date)]
        public DateTime RequestDate { get; set; }

     
        public string Site { get; set; }

       
        public string Department { get; set; }

        
        public string ResponsibleOfficer { get; set; }

        [Phone]
        public string ContactPhone { get; set; }

        // Equipment Section 
        public string EquipmentType { get; set; }

        public string ProblemDescription { get; set; }

        public string SerialNumber { get; set; }
        public bool? Verification { get; set; }
        public string status  { get; set; }
        public List<Registry> Registries { get; set; } = new List<Registry>();
    }
}