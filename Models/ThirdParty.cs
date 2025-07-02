using System.ComponentModel.DataAnnotations;

namespace FormRequest.Models
{
    public class ThirdParty
    {
        [Key]
        public int ThirdPartyId { get; set; }

        public int FormReqDbId { get; set; }
        public FormReqDb FormReqDb { get; set; }
        public string CompanyName { get; set; }
        public string CompanyContact { get; set; }
        [DataType(DataType.Date)]
        public DateTime DateSent { get; set; }
        public string ThirdPartyRemarks { get; set; }
        public string? AttachmentPath { get; set; }


    }
}
