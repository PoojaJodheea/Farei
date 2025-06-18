using System.ComponentModel.DataAnnotations;

namespace FormRequest.Models
{
    public class FormReqDb
    {
        [Key]
        public int Id { get; set; }

        // General Form Details
        [Required]
        [DataType(DataType.Date)]
        public DateTime RequestDate { get; set; }

        [Required]
        public string Site { get; set; }

        [Required]
        public string Department { get; set; }

        [Required]
        public string ResponsibleOfficer { get; set; }

        [Phone]
        public string ContactPhone { get; set; }

        // Equipment Section (1 entry for now — add related tables later if needed)
        public string EquipmentType { get; set; }

        public string ProblemDescription { get; set; }

        public string SerialNumber { get; set; }

        // Registry Section
        public string From { get; set; }

        public string To { get; set; }

        [DataType(DataType.Date)]
        public DateTime MovementDate { get; set; }

        public string Remarks { get; set; }

        public bool IsApproved { get; set; }
        public bool IsInvalid { get; set; }

        public bool ? Verification { get; set; }
    }
}
