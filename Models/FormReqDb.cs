using System;
using System.Collections.Generic;
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

        public int Pointer { get; set; } //for status
        public string Department { get; set; }

        public string ResponsibleOfficer { get; set; }

        [Phone]
        public string ContactPhone { get; set; }

        // Equipment Section 
        public string EquipmentType { get; set; }

        public string ProblemDescription { get; set; }
        public string SerialNumber { get; set; }
        public bool? Verification { get; set; }
        public string? status { get; set; } 
        public string? Supervisor { get; set; }
        public string? Technician { get; set; }
        public bool IsDraft { get; set; } = true;
        public bool IsClosed { get; set; } = false;
        public string? UserFeedback { get; set; }
        public String? remarks { get; set; }

        public List<Registry> Registries { get; set; } = new List<Registry>();

        public static implicit operator FormReqDb(List<FormReqDb> v)
        {
            throw new NotImplementedException();
        }
    }
}