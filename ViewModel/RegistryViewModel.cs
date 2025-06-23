using FormRequest.Models;
using System.Collections.Generic;

namespace FormRequest.ViewModel
{
    public class RegistryViewModel
    {
        public FormReqDb? FormReqDb { get; set; }

        public Registry Registry { get; set; }
        public List<Registry> RegistryList { get; set; } = new List<Registry>(); 

      
}
}
