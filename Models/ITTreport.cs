using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;



namespace FormRequest.Models
{
    public class ITTreport
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int? FormReqDb { get; set; }
        public String? SerialNumber { get; set; }
        public String? Report { get; set; }
    }
}