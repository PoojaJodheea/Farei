using FormRequest.Models;
using System.Collections.Generic;

namespace FormRequest.ViewModel
{
    public class RegistryViewModel
    {
        public FormReqDb? FormReqDb { get; set; }
        public List <FormReqDb>? FormReqDbs { get; set; }

        public Registry Registry { get; set; } = new Registry();
        public List<Registry> RegistryList { get; set; } = new List<Registry>(); 

      
}
}
