using FormRequest.Models;
using System.Collections.Generic;

namespace FormRequest.ViewModel
{
    public class RequestViewModel
    {
        public FormReqDb? FormReqDb { get; set; }
        public List<FormReqDb>? FormReqDbs { get; set; }
        
        public Registry Registry { get; set; } = new Registry();
        public List<Registry> RegistryList { get; set; } = new List<Registry>();
        public ThirdParty? ThirdParty { get; set; }
        public ITTreport? ITTreport { get; set; }
        public List<ApplicationUser> AllUsers { get; internal set; }
        public ApplicationUser? User { get; set; }

        public EquipmentInventory? Inventory { get; set; }

        public List<Notifications>? Notifications { get; set; } = new List<Notifications>();
    }
}