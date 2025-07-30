namespace FormRequest.Models
{
    public class EquipmentInventory
    {
        public int ID { get; set; }
        public String EquipmentName { get; set; }
        public String SerialNumber { get; set; }
        public String EquipmentType { get; set; }
        public String Site { get; set; }
        public String Department { get; set; }
        public String? Remarks { get; set; }

    }
}
