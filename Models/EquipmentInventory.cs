using System.ComponentModel.DataAnnotations;

namespace FormRequest.Models
{
    public class EquipmentInventory
    {
        public int ID { get; set; }
        public String EquipmentName { get; set; }
        public String SerialNumber { get; set; }
        public String EquipmentType { get; set; }
        public String EquipmentMake { get; set; }
        public String EquipmentModel { get; set; }
        public String EquipmentDrive { get; set; }
        public String CpuModel{ get; set; }
      
        public int StorageCapacity { get; set; }
        public int MemoryCapacity { get; set; }
        public String OperatingSys { get; set; }
        public String OS_Key { get; set; }
        public String OfficeName { get; set; }
        public String OfficeKey { get; set; }
        public String OfficeLogin { get; set; }
        public String OfficePassword { get; set; }
        public String AntiVirusName{ get; set; }
        public String AntiVirusLicense { get; set; }
       
        [DataType(DataType.Date)]
        public DateTime AntiVirusExpiryDatee { get; set; }
        public String Supplier { get; set; }

        [DataType(DataType.Date)]
        public DateTime DateOfPurchase { get; set; }
        public String User { get; set; }
        public Decimal Amount{ get; set; }
        public String OneDriveEmail { get; set; }
        public String OneDrivePassword { get; set; }
        public String Brcode { get; set; }
        public String Site { get; set; }
        public String Department { get; set; }
        public String? Remarks { get; set; }

    }
}
