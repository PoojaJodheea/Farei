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
        // GET: Supervisor Form
        public async Task<IActionResult> SupervisorForm()
        {
            var requests = await _context.FormReqDb
                 .Where(f => f.status == "pending")
                .ToListAsync();
               
               

            var viewModel = new RequestViewModel
            {
                FormReqDbs = requests
            };

            return View(viewModel);
        }



        // GET: Supervisor Form Details
        public async Task<IActionResult> DetailsSupervisorForm(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var formReqDb = await _context.FormReqDb
                .FirstOrDefaultAsync(m => m.Id == id);

            if (formReqDb == null)
            {
                return NotFound();
            }

            var viewModel = new RequestViewModel
            {
               FormReqDb = formReqDb
            };

            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeStatus(int id, String Accepted)
        {
            var formReqDb = await _context.FormReqDb.FindAsync(id);
            if (formReqDb == null)
            {
                return NotFound();
            }
            formReqDb.status = Accepted;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FormReqDbExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(SupervisorForm));
        }

        private bool FormReqDbExists(int id)
        {
            throw new NotImplementedException();
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

        public async Task<IActionResult> Registry()
        {
            var requests = await _context.FormReqDb
                .Where(f => f.status == "accept transit"|| f.status == "send back" || f.status =="return")
                .ToListAsync();

            var viewModel = new RequestViewModel
            {
                FormReqDbs = requests
            };

            return View(viewModel);
        }
        public async Task<IActionResult> RegistryDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var formReqDb = await _context.FormReqDb
                .FirstOrDefaultAsync(m => m.Id == id);
          

            var viewModel = new RequestViewModel
            {
                FormReqDb = formReqDb,
                Registry = new Registry(),
             
            };
            if (formReqDb == null)
            {
                return NotFound();
            }

            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegistryConfirmation(int formReqId, string driver, DateTime dateReceived)
        {
            var form = await _context.FormReqDb.FindAsync(formReqId);
            if (form == null) return NotFound();

            // Create registry entry
            var registry = new Registry
            {
                FormReqDbId = formReqId,
                Driver = driver,
                DateReceived = dateReceived,
                IsValid = true
            };

            _context.Registry.Add(registry);

            // Update form status
            form.status = "TransitConfirmed";
            _context.FormReqDb.Update(form);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Registry));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Arrived(int id)
        {
            var Registry = await _context.Registry.FindAsync(id);
            var formReqDb = await _context.FormReqDb.FindAsync(Registry.FormReqDbId);
            var checkRegistry = _context.Registry.Any(j => j.FormReqDbId == formReqDb.Id);
            if (Registry == null)
            {
                return NotFound();
            }
            if (checkRegistry)
            {
                if (formReqDb.status == "sendback")
                {
                    formReqDb.status = "reject";
                }
                else if (formReqDb.status == "accept transit")
                {
                    formReqDb.status = "Repairing";
                    Registry.IsValid = true;
                }
            }
            else
            {
                Registry.IsValid = true;
                formReqDb.status = "Repairing";
            }
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FormReqDbExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction("Registry");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitRegistry(RequestViewModel model)
        {

            try
            {
                var formReqDb = await _context.FormReqDb.FindAsync(model.Registry?.FormReqDbId);
                var existRegistry = _context.Registry.Any(k => k.FormReqDbId == formReqDb.Id);
                var newform = model.Registry;
                if (formReqDb == null)
                {
                    return NotFound();
                }

                await _context.SaveChangesAsync();
                if (existRegistry)
                {
                    newform.To = formReqDb.Site;
                    newform.From = "Reduit";
                    _context.Registry.Add(newform);
                    _context.SaveChanges();
                    return RedirectToAction("RegistryForm");
                }
                else
                {
                    newform.From = formReqDb.Site;
                    newform.To = "Reduit";
                    _context.Registry.Add(newform);
                    _context.SaveChanges();
                    return RedirectToAction("RegistryForm");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("DB error: " + ex.Message);
            }

            return RedirectToAction("RegistryForm");
        }




        public IActionResult OnSite()
        {
            var forms = _context.FormReqDb
                .Include(f => f.Registries)
                .Where(f => f.Registries.Any(r => ( r.IsOnSite)))
                .ToList();
            return View("IsOnSite", forms);
        }

    

        public async Task<IActionResult> Transit()
        {
           

            var model = new RequestViewModel
            {
                FormReqDbs = await _context.FormReqDb.ToListAsync(),
                RegistryList = await _context.Registry
                                    .Include(r => r.FormReqDb)
                                   
                                    .ToListAsync(),
            };
            return View(model);
        }

        public async Task<IActionResult> DetailsTransiteForm(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var formReqDb = await _context.FormReqDb
   .FirstOrDefaultAsync(m => m.Id == id);
            var Registry = await _context.Registry
               .Include(r => r.FormReqDb)
               .FirstOrDefaultAsync(m => m.FormReqDbId == id);



            var viewModel = new RequestViewModel
            {
                FormReqDb = formReqDb,
                Registry = Registry,
            
            };
            if (formReqDb == null)
            {
                return NotFound();
            }

            return View(viewModel);
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
                    
                }

            }).ToList();

            return View("NewComponent", viewModelList);

        }

        public IActionResult allrequest2() //loads all request 
        {

            var allrequest = _context.FormReqDb
     
     .ToList();


            return View("allrequest2", allrequest);
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
        public async Task<IActionResult> RequestMovement()
        {
            var requests = await _context.FormReqDb
                .Include(f => f.Registries)
                .Where(f => f.status == "OnSite" || f.status == "InTransit")
                .ToListAsync();

            return View(requests);
        }




        [HttpPost]
        public IActionResult UpdateStatus(int id, string actionType)
        {
            var request = _context.FormReqDb.FirstOrDefault(r => r.Id == id);
            if (request == null)
                return Json(new { success = false, message = "Request not found." });

            string currentStatus = request.status?.ToLower();
            if (currentStatus == "transit" || currentStatus == "accept transit" || currentStatus == "reject transit")
                request.status = actionType == "accept" ? "accept transit" : "reject transit";
            else if (currentStatus == "onsite" || currentStatus == "accept onsite" || currentStatus == "reject onsite")
                request.status = actionType == "accept" ? "accept onsite" : "reject onsite";
            else if (currentStatus == "pending" || currentStatus == "accept" || currentStatus == "reject")
                request.status = actionType == "accept" ? "accept" : "reject";
            else return Json(new { success = false, message = "Invalid status for action." });

            _context.SaveChanges();
            return Json(new { success = true, newStatus = request.status });
        }








    }
}
