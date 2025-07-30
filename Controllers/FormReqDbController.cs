
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

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var form = await _context.FormReqDb.FindAsync(id);
            if (form == null) return NotFound();

            return View(form);

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

                                                                     //USER//
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
                try
                {
                    model.RequestDate = DateTime.Now.Date;

                    model.status = "pending";
                

                    _context.FormReqDb.Add(model);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Form submitted successfully.";
                    return RedirectToAction("Create");
                }
                catch (Exception)
                {
                    TempData["ErrorMessage"] = "An error occurred while submitting the form. Please try again.";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Please fill in all required fields correctly.";
            }

            return View(model);
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
   
       
        public async Task<IActionResult> Feedback()
        {
            
            var limit = _context.UserSettings.FirstOrDefault()?.FeedbackLimitDays;

            
            var formDBReq = await _context.FormReqDb
                .Where(j => (j.status.ToLower() == "complete" || j.status.ToLower() == "return") && !j.IsClosed)
                .ToListAsync();

            foreach (var request in formDBReq)
            {
                var duration = (DateTime.Now - request.RequestDate).Days;

                if (duration > limit)
                {
                    request.status = "Closed";
                    request.IsClosed = true;
                }
            }

            
            await _context.SaveChangesAsync();

            return View(formDBReq);
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
                
                _context.SaveChanges();
            }

            return RedirectToAction("Feedback");
        }
       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SetFeedbackLimit(int days)
        {
            var setting = _context.UserSettings.FirstOrDefault();

            if (setting == null)             //if empty creates a new record else updates existing ones
            {
                setting = new UserSettings
                {
                    FeedbackLimitDays = days,
                    LastUpdated = DateTime.Now
                };
                _context.UserSettings.Add(setting);
            }
            else
            {
                setting.FeedbackLimitDays = days;
                setting.LastUpdated = DateTime.Now;
               
            }

            _context.SaveChanges();

            TempData["Message"] = "Feedback day limit updated successfully.";

           
            return RedirectToAction("Index"); // Change 
        }
        public async Task<IActionResult> RequestMovement()
        {
            var requests = await _context.FormReqDb
                .Include(f => f.Registries)
                .Where(f => f.status == "OnSite" || f.status == "InTransit")
                .ToListAsync();

            return View(requests);
        }

        //SUPERVISOR
        public async Task<IActionResult> SupervisorForm()
        {
            var requests = await _context.FormReqDb
                 
                .ToListAsync();
               
               

            var viewModel = new RequestViewModel
            {
                FormReqDbs = requests
            };

            return View(viewModel);
        }

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
        public async Task<IActionResult> SubmitRemarks(int id, string remarks)
        {
            var request = await _context.FormReqDb.FindAsync(id);
            if (request == null)
            {
                return NotFound();
            }

            request.remarks = remarks;
            request.RequestDate = DateTime.Now; 

            await _context.SaveChangesAsync();

            // Redirect back to the same details page
            return RedirectToAction(nameof(DetailsSupervisorForm), new { id = id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeStatus(int id, int Status)
        {
            var formReqDb = await _context.FormReqDb.FindAsync(id);
            if (formReqDb == null)
            {
                return NotFound();
            }

            if (Status == 1)
            {
                formReqDb.status = "Accepted";
                formReqDb.Pointer += 1;//1
            }
            else if (Status == 2)
            {
                formReqDb.status = "Rejected";
            }
            else if (Status == 3)
            {
                formReqDb.status = "Onsite request";
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(TechnicianForm));
            }
            else if (Status == 4)
            {
                formReqDb.status = "Transit request";
                formReqDb.Pointer += 1;//2
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(TechnicianForm));
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
            return RedirectToAction(nameof(SupervisorForm));
        }
        private bool FormReqDbExists(int id)
        {
            throw new NotImplementedException();
        }
                                                                      //DELETE
        
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
                .Where(f => f.status == "transitting"|| f.status == "send back" || f.status =="return")
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
        public async Task<IActionResult> Arrived(int id, string Remarks)
        {
            var Registry = await _context.Registry.FindAsync(id);
            if (Registry == null)
            {
                return NotFound();
            }

            var formReqDb = await _context.FormReqDb.FindAsync(Registry.FormReqDbId);
            if (formReqDb == null)
            {
                return NotFound();
            }
            
            if (formReqDb.Pointer == 0)
            {
                formReqDb.status = "rejected";
                Registry.Remarks = Remarks;
            }
            else if (formReqDb.Pointer == 2)
            {
                formReqDb.status = "Repairing";
                Registry.IsValid = true;
                Registry.Remarks = Remarks;
                formReqDb.Pointer += 1;//3
            }
            else if (formReqDb.Pointer == 5)
            {
                formReqDb.status = "Complete";
                formReqDb.RequestDate = DateTime.Now;
                Registry.IsValid = !Registry.IsValid;
                Registry.Remarks = Remarks;
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
                    return RedirectToAction("Registry");
                }
                else
                {
                    newform.From = formReqDb.Site;
                    newform.To = "Reduit";
                    _context.Registry.Add(newform);
                    _context.SaveChanges();
                    return RedirectToAction("Registry");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("DB error: " + ex.Message);
            }

            return RedirectToAction("Registry");
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


        //Equipment Movement
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

        public IActionResult allrequest2()
        {
            var requests = _context.FormReqDb.ToList();

            var viewModels = requests.Select(req => new RequestViewModel
            {
                FormReqDb = req,
                ITTreport = _context.ITTreport.FirstOrDefault(r => r.FormReqDb == req.Id)
            }).ToList();

            return View("allrequest2", viewModels);
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ITOstatus(int id, String Status, String Remarks)
        {
            var formReqDb = await _context.FormReqDb.FindAsync(id);
            var registry = _context.Registry.Any(k => k.FormReqDbId == id);
            if (formReqDb == null)
            {
                return NotFound();
            }
            if (Status == "rejects")
            {
                if (registry)
                {
                    formReqDb.status = "send back";
                    formReqDb.remarks = Remarks;
                    formReqDb.Pointer = 0;
                }
                else
                {
                    formReqDb.status = Status;
                    formReqDb.remarks = Remarks;
                }
            }
            else if (formReqDb.Pointer == 2)
            {
                formReqDb.status = "Transitting";
            }
            else if (formReqDb.Pointer == 1)
            {
                formReqDb.status = "Repairing";
                formReqDb.Pointer += 2;
            }
            else if (formReqDb.Pointer == 4)
            {
                formReqDb.status = "Start repairing";
            }
            else if (formReqDb.Pointer == 5)
            {
                if (registry)
                {
                    formReqDb.status = "Return";
                }
                else
                {
                    formReqDb.status = "Complete";
                }
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
            return RedirectToAction("allrequest2");
        }





        [HttpPost]
        public IActionResult UpdateStatus(int id, string actionType)
        {
            var request = _context.FormReqDb.FirstOrDefault(r => r.Id == id);
            var checkRegistry = _context.Registry.Any(j => j.FormReqDbId == id);
            if (request == null)
                return Json(new { success = false, message = "Request not found." });

            if (actionType == "reject")
            {
                if (checkRegistry)
                {
                    request.status = "send back";
                    _context.SaveChanges();
                    return Json(new { success = true, newStatus = request.status });
                }
                else
                {
                    request.status = "rejected";
                    _context.SaveChanges();
                    return Json(new { success = true, newStatus = request.status });
                }
            }
            else if (actionType == "accept")
            {
                if (request.Pointer == 0)
                {
                    request.status = "Accepted";
                    request.Pointer += 1;
                    _context.SaveChanges();
                    return Json(new { success = true, newStatus = request.status });
                }
                else if (request.Pointer == 1)
                {
                    request.status = "repairing";
                    request.Pointer += 2;
                    _context.SaveChanges();
                    return Json(new { success = true, newStatus = request.status });
                }
                else if (request.Pointer == 2)
                {
                    request.status = "Transitting";
                    _context.SaveChanges();
                    return Json(new { success = true, newStatus = request.status });
                }
                else if (request.Pointer == 4)
                {
                    request.status = "Start repairing";
                    _context.SaveChanges();
                    return Json(new { success = true, newStatus = request.status });
                }
                else if (request.Pointer == 5)
                {
                    if (checkRegistry)
                    {
                        request.status = "Return";
                        _context.SaveChanges();
                        return Json(new { success = true, newStatus = request.status });
                    }
                    else
                    {
                        request.status = "Complete";
                        _context.SaveChanges();
                        return Json(new { success = true, newStatus = request.status });
                    }

                }
            }


            /*
            string currentStatus = request.status?.ToLower();
            if (currentStatus == "transit" || currentStatus == "accept transit" || currentStatus == "reject transit")
                request.status = actionType == "accept" ? "accept transit" : "reject transit";
            else if (currentStatus == "onsite" || currentStatus == "accept onsite" || currentStatus == "reject onsite")
                request.status = actionType == "accept" ? "accept onsite" : "reject onsite";
            else
                return Json(new { success = false, message = "Invalid status for action." });
            */
            _context.SaveChanges();
            return Json(new { success = true, newStatus = request.status });
        }


                                                                      //IT TECHNICIAN

        public async Task<IActionResult> TechnicianForm()
        {
            var model = new RequestViewModel
            {
                FormReqDbs = await _context.FormReqDb.ToListAsync(),
                RegistryList = await _context.Registry.ToListAsync(),
                
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Report(int id, String SerialNumber, String Status, String Remarks, RequestViewModel model)
        {
            var formReqDb = await _context.FormReqDb.FindAsync(id);
           

            if (formReqDb == null)
            {
                return NotFound();
            }
            if (formReqDb.Pointer == 4)
            {
                var ITTreport = await _context.ITTreport.FirstOrDefaultAsync(f => f.FormReqDb == id);
                ITTreport.Report += " " + Remarks;
                formReqDb.status = "Final request";
                formReqDb.Pointer += 1;//5

            }
            else if (formReqDb.Pointer == 3)
            {

                var newForm = new ITTreport
                {
                    FormReqDb = id,
                    SerialNumber = SerialNumber,
                    Report = Remarks
                };
                _context.ITTreport.Add(newForm);
                _context.SaveChanges();
                formReqDb.status = "Pending request";
                formReqDb.Pointer += 1;//4
            }
            else
            {
                formReqDb.status = "Pending request";
                formReqDb.remarks = Remarks;
              
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
            return RedirectToAction("TechnicianForm");
        }

        public async Task<IActionResult> TechnicianDetailsForm(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var formReqDb = await _context.FormReqDb?.FirstOrDefaultAsync(m => m.Id == id);
            var allUsers = _context.Users.ToList();
            var viewModel = new RequestViewModel
            {
                FormReqDb = formReqDb,
             
            };
            if (formReqDb == null)
            {
                return NotFound();
            }

            return View(viewModel);
        }
        public async Task<IActionResult> Movement()
        {
            var model = new RequestViewModel
            {
                FormReqDbs = await _context.FormReqDb.Where(j => j.status == "Accepted").ToListAsync(),
                RegistryList = await _context.Registry.ToListAsync(),
               
            };
            return View(model);
        }
        public async Task<IActionResult> MovementConfirmation()
        {
            var model = new RequestViewModel
            {
                FormReqDbs = await _context.FormReqDb.Where(j => j.status == "Accepted").ToListAsync(),
                RegistryList = await _context.Registry.ToListAsync(),

            };
            return View(model);
        }
    }
}
