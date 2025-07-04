using AspNetCoreGeneratedDocument;
using FormRequest.Data;
using FormRequest.Models;
using FormRequest.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FormRequest.Controllers
{
    public class FormReqDbController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FormReqDbController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var forms = await _context.FormReqDb.ToListAsync();
            return View(forms);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var form = await _context.FormReqDb
                .Include(f => f.Registries)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (form == null) return NotFound();

            var viewModel = new RequestViewModel
            {
                FormReqDb = form,
                RegistryList = form.Registries.ToList() 
            };

            return View(viewModel);
        }
        public IActionResult Create()  //display new form
        {
            return View(new FormReqDb());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
     
            public async Task<IActionResult> Create(FormReqDb model)
        {
            if (ModelState.IsValid)
            {
                _context.FormReqDb.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(model);
        }
        

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var form = await _context.FormReqDb.FindAsync(id);
            if (form == null) return NotFound();

            return View(form);

        }
        [HttpGet]
        public JsonResult GetSerialNumbers(string site, string department, string equipmentType) //displays list of SN according to conditions
        {
            var serials = _context.FormReqDb
                .Where(e => e.Site == site && e.Department == department && e.EquipmentType == equipmentType)
                .Select(e => e.SerialNumber)
                .Distinct()
                .ToList();

            return Json(serials);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, FormReqDb formReqDb)
        {
            if (id != formReqDb.Id) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(formReqDb);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(formReqDb);
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var form = await _context.FormReqDb.FirstOrDefaultAsync(m => m.Id == id);
            if (form == null) return NotFound();

            return View(form);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var form = await _context.FormReqDb.FindAsync(id);
            if (form != null)
            {
                _context.FormReqDb.Remove(form);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
        //REGISTRY SECTION
        public IActionResult Registry()
        {
            var requests = _context.FormReqDb
                .Include(f => f.Registries)
                .Where(f => !f.Registries.Any(r => r.IsValid && (r.IsOnSite || r.IsInTransit)))
                .ToList();

            return View(requests);
        }
        public IActionResult RegistryDetails(int id)
        {
            var formReq = _context.FormReqDb
                .Include(f => f.Registries)
                .FirstOrDefault(x => x.Id == id);

            if (formReq == null) return NotFound();

            var viewModel = new RequestViewModel
            {
                FormReqDb = formReq,
                Registry = new Registry
                {
                    FormReqDbId = id,
                    
                },
             RegistryList = formReq.Registries.ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SubmitRegistry(RequestViewModel model)
        {
            ModelState.Remove("Registry.FormReqDb");

            if (!ModelState.IsValid)
            {
                model.FormReqDb = _context.FormReqDb

                    .Include(f => f.Registries)
                    .FirstOrDefault(f => f.Id == model.Registry.FormReqDbId);

                model.RegistryList = model.FormReqDb?.Registries.ToList();
               

                return View("RegistryDetails", model);
            }

            //Adding registry
            model.Registry.MovementDate = DateTime.Now.Date;//local date only
            model.Registry.IsValid = true;
          _context.Registry.Add(model.Registry);
           
          _context.SaveChanges();
          
         
            return RedirectToAction("RegistryDetails", new { id = model.Registry.FormReqDbId });
        }
        public IActionResult OnSite()
        {
            var forms = _context.FormReqDb
                .Include(f => f.Registries)
                .Where(f => f.Registries.Any(r => ( r.IsOnSite)))
                .ToList();
            return View("IsOnSite", forms);
        }

        public IActionResult InTransit()
        {
            var forms = _context.FormReqDb
                .Include(f => f.Registries)
                .Where(f => f.Registries.Any(r => ( r.IsInTransit)))
                .ToList();
            return View("IsInTransit", forms);
        }
        //Delete request after acknowlegdement

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteRegistry(int registryId)
        {
            var registry = _context.Registry.FirstOrDefault(r => r.RegistryId == registryId);

            if (registry != null)
            {
                _context.Registry.Remove(registry);
                _context.SaveChanges();
            }

            
            return RedirectToAction("OnSite");
        }
        //ITO PAGE
        public IActionResult Repaired() //loads repaired request 
        {

            var repairedForms = _context.FormReqDb
                .Include(f => f.Registries)
              
                .ToList();

            return View("Repaired", repairedForms);
        }
        public IActionResult NewComponent()
        {
            var newComponents = _context.FormReqDb
                 
                .Include(f => f.Registries)
                .ToList();

            var viewModelList = newComponents.Select(f => new RequestViewModel
            {
                FormReqDb = f,
                Registry = f.Registries.FirstOrDefault() ?? new Registry()
                {
                    MovementDate = DateTime.Now.Date
                }

            }).ToList();

            return View("NewComponent", viewModelList);
        }
        public IActionResult ThirdParty(SearchFilter filter, string? SortOrder)
        {
            var query = _context.FormReqDb
                .Include(f => f.Registries)
                .Select(f => new RequestViewModel
                {
                    FormReqDb = f,
                    ThirdParty = _context.ThirdParties.FirstOrDefault(tp => tp.FormReqDbId == f.Id)
                })
                .AsQueryable();

            if (!string.IsNullOrEmpty(filter.SearchKey))
            {
                query = query.Where(x =>
                    x.FormReqDb.ResponsibleOfficer.Contains(filter.SearchKey) ||
                    
                    x.FormReqDb.SerialNumber.Contains(filter.SearchKey));
            }

            if (!string.IsNullOrEmpty(SortOrder))
            {
                if (SortOrder == "asc")
                {
                    query = query.OrderBy(x => x.ThirdParty.DateSent);
                }
                else if (SortOrder == "desc")
                {
                    query = query.OrderByDescending(x => x.ThirdParty.DateSent);
                }
            }

            return View(query.ToList());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitThirdPartyForm(ThirdParty model, IFormFile? Attachment) //IFormFile->optional attachment
        {
            string? filePath = null;

            if (Attachment != null && Attachment.Length > 0)  //
            {
                var uploadsFolder = Path.Combine("wwwroot", "uploads");
                Directory.CreateDirectory(uploadsFolder);

                var fileName = Guid.NewGuid() + Path.GetExtension(Attachment.FileName);//creates a filename
                var fullPath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await Attachment.CopyToAsync(stream);
                }

                filePath = Path.Combine("uploads", fileName); // stores file path for db
            }

            var existing = _context.ThirdParties
                .FirstOrDefault(tp => tp.FormReqDbId == model.FormReqDbId);  //existing fields from form

            if (existing != null)//if records exist->remark+attachment can be updated 
            {
                existing.ThirdPartyRemarks = model.ThirdPartyRemarks;
                if (filePath != null) existing.AttachmentPath = filePath;
                _context.Update(existing);
            }
            else
            {
                model.FormReqDb = null; //new entry->insert new record
                if (filePath != null) model.AttachmentPath = filePath;
                _context.ThirdParties.Add(model);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("ThirdParty");
       
        
        }

        public IActionResult Feedback()
        {
            var requests = _context.FormReqDb
                .Where(r => !r.IsClosed) //hides closed requests
                .ToList();

            return View(requests);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SubmitFeedback(int id, string feedback, bool confirmRepaired)
        {
            var request = _context.FormReqDb.FirstOrDefault(r => r.Id == id);  //request by id
            if (request != null)  //request found->save feedback+if checkbox ticked->IsClosed=True
            {
                request.UserFeedback = feedback;
                request.IsClosed = confirmRepaired;
                _context.Update(request);
                _context.SaveChanges();
            }

            return RedirectToAction("Feedback");
        }









    }
}
