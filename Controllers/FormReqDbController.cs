
using Azure.Core;
using FormRequest.Data;
using FormRequest.Models;
using FormRequest.ViewModel;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
//using MigraDoc.Rendering;
using Mono.TextTemplating;
using Newtonsoft.Json.Linq;
//using SelectPdf;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
//using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
namespace FormRequest.Controllers
{
    public class FormReqDbController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
       
        private int atNewRequest = 0;
        private int atAccepted = 1;
        private int atTransitRequest = 2;
        private int atOnsiteRequest = 3;
        private int atTransitting = 4;
        private int atReport = 5;
        private int atPendingRequest = 6;
        private int atStartRepairing = 7;
        private int atFinalRequest = 8;
        private int atReturn = 9;

        public FormReqDbController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
         
        }

    

public override void OnActionExecuting(ActionExecutingContext context)
    {
        base.OnActionExecuting(context);

        if (User?.Identity?.IsAuthenticated == true)
        {
            var notifications = _context.FormReqDb
                .OrderByDescending(m => m.RequestDate)
                .Take(20) // Limit to last 20 requests
                .ToList();

            ViewData["NotificationsModel"] = new RequestViewModel
            {
                FormReqDbs = notifications
            };
        }
    }


    public async Task<IActionResult> Index()
        {
            var model = new RequestViewModel  
            {
                FormReqDbs = await _context.FormReqDb.Include(m => m.Equipments).Include(m => m.ITTReports).ToListAsync(),
                AllUsers = _userManager.Users.ToList()
            };
            var username = User.Identity.Name;
            if (username == null)
            {
                return View(model);
            }
            var user = await _userManager.FindByEmailAsync(username);
            string? type = user.Type;
            if (type.Equals("Supervisor"))
            {
                return RedirectToAction("SupervisorForm");
            }
            else if (type.Equals("Registry"))
            {
                return RedirectToAction("Registry");
            }
            else if (type.Equals("Technician"))
            {
                return RedirectToAction("TechnicianForm");
            }
            else if (type.Equals("Admin"))
            {
                return View(model);
            }
            else if (type.Equals("ITO"))
            {
                return RedirectToAction("ITOform");
            }
            return View(model);
        }
        public async Task<IActionResult> Details(int? id)
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
                AllUsers = allUsers
            };
            if (formReqDb == null)
            {
                return NotFound();
            }

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

        public async Task<IActionResult> MyRequestForm()
        {
            var userId = _userManager.GetUserId(User);
            var model = new RequestViewModel
            {
                FormReqDbs = await _context.FormReqDb.Where(j => j.UserId == userId).ToListAsync(),
                AllUsers = _userManager.Users.ToList()
            };
            return View(model);
        }                                                  //USER//
        public async Task<IActionResult> Create()
        {
            var userName = User.Identity?.Name;
            if (string.IsNullOrEmpty(userName))
                return RedirectToAction("Index");

            var currentUser = await _context.Alluser.FirstOrDefaultAsync(m => m.UserName == userName);
            var users = await _context.Users.ToListAsync();

            var viewModel = new RequestViewModel
            {
                FormReqDb = new FormReqDb(),
                AllUsers = users,
                User = currentUser
            };
            return View("Create", viewModel);
        }

        // POST: Create a new form request
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RequestViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.AllUsers = await _context.Users.ToListAsync();
                return View(model);
            }

            var userName = User.Identity?.Name;
            if (string.IsNullOrEmpty(userName))
                return RedirectToAction("Index");

            var user = await _context.Alluser.FirstOrDefaultAsync(m => m.UserName == userName);
            if (user == null)
                return RedirectToAction("Index");

            var newForm = model.FormReqDb;
            var equipment = await _context.Equipment.FirstOrDefaultAsync(m => m.SerialNumber == newForm.SerialNumber);

            newForm.RequestDate = DateTime.Now;
            newForm.Equipments = equipment;
            newForm.UserId = user.Id;
            newForm.ResponsibleOfficer = userName;

            // ✅ Set pointer based on user type
            if (user.Type == "User")
            {
                newForm.Pointer = 0;
                newForm.status = "Pending";
            }
            else if (user.Type == "Supervisor")
            {
                newForm.Pointer = 1;
                newForm.status = "Accepted";
            }
            else
            {
                newForm.Pointer = 0;
                newForm.status = "Pending";
            }

            _context.FormReqDb.Add(newForm);
            await _context.SaveChangesAsync();
            await GenerateNotifications(newForm);

            return RedirectToAction("Index");
        }


        public async Task<IActionResult> EquipmentList()
        {
            var model = new RequestViewModel
            {
                Inventories = await _context.Equipment.ToListAsync()
            };

            return View("EquipmentList", model);
        }

    


        public IActionResult CreateEquipment()
        {
            var viewModel = new RequestViewModel
            {
                FormReqDb = new FormReqDb()
            };
            return View(viewModel);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateEquipment(RequestViewModel model)
        {
            if (ModelState.IsValid)
            {
                _context.Equipment.Add(model.Inventory);
                await _context.SaveChangesAsync();
                return RedirectToAction("EquipmentList");
            }

           
            return View(model);
        }






        // GET: List all forms (Admin or general view)
        public async Task<IActionResult> UserForm()
        {
            var model = new RequestViewModel
            {
                FormReqDbs = await _context.FormReqDb.ToListAsync(),
                RegistryList = await _context.Registry.ToListAsync(),
                AllUsers = _userManager.Users.ToList()
            };
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
                var duration = (DateTime.Now.Date - request.RequestDate.Date).Days;

                if (duration > limit)
                {
                    request.status = "closed";
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
                _context.Update(request);
                _context.SaveChanges();
            }

            return RedirectToAction("Feedback");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SetFeedbackLimit(int days)
        {
            var setting = _context.UserSettings.FirstOrDefault();

            if (setting == null)            
            {
                setting = new UserSettings
                {
                    FeedbackLimitDays = days,
                   
                };
                _context.UserSettings.Add(setting);
            }
            else
            {
                setting.FeedbackLimitDays = days;
              
                _context.UserSettings.Update(setting);
            }

            _context.SaveChanges();

            TempData["Message"] = "Feedback day limit updated successfully.";


            return RedirectToAction("AdminLimitDay"); // Change 
        }

        public async Task<IActionResult> AdminLimitDay()
        {
            var name = User.Identity.Name;
            if (name == null)
            {
                return RedirectToAction("Index");
            }

            var user = await _context.Alluser.FirstOrDefaultAsync(m => m.UserName == name);
            if (user == null || user.Type != "Admin")
            {
                return RedirectToAction("Index");
            }

            return View(AdminLimitDay); 
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
            var model = new RequestViewModel
            {

                FormReqDbs = await _context.FormReqDb.Where(j => j.Supervisor.Contains(User.Identity.Name) && j.status == "pending").ToListAsync(),
                AllUsers = _userManager.Users.ToList()
            };
            return View(model);
        }



        public async Task<IActionResult> DetailsSupervisorForm(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var formReqDb = await _context.FormReqDb
                .FirstOrDefaultAsync(m => m.Id == id);
            var AllUsers = _userManager.Users.ToList();
            var viewModel = new RequestViewModel
            {
                FormReqDb = formReqDb,
                AllUsers = AllUsers
            };
            if (formReqDb == null)
            {
                return NotFound();
            }

            return View(viewModel);
        }
        public async Task<IActionResult> SAllRequest()
        {
            var name = User.Identity.Name;
            if (name == null)
            {
                return RedirectToAction("TechnicianForm");
            }
            var model = new RequestViewModel
            {
                FormReqDbs = await _context.FormReqDb.Where(j => j.Supervisor == name).ToListAsync(),
                RegistryList = await _context.Registry.ToListAsync(),
                AllUsers = _userManager.Users.ToList()
            };
            return View(model);
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
        public async Task<IActionResult> ChangeStatus(int id, int Status, string? Remarks)
        {
            var formReqDb = await _context.FormReqDb.FindAsync(id);
            if (formReqDb == null)
            {
                return NotFound();
            }

            switch (Status)
            {
                case 1:
                    formReqDb.status = "Accepted";
                    formReqDb.Pointer = 1;
                    break;

                case 2:
                    formReqDb.status = "Rejected";
                    formReqDb.Pointer = 0;
                    break;

                case 3:
                    formReqDb.status = "Onsite request";
                    formReqDb.Pointer = 3;
                    formReqDb.remarks = Remarks ?? "";
                    await _context.SaveChangesAsync();
                    await GenerateNotifications(formReqDb);
                    return RedirectToAction(nameof(TechnicianForm));

                case 4:
                    formReqDb.status = "Transit request";
                    formReqDb.Pointer = 2;
                    formReqDb.remarks = Remarks ?? "";
                    await _context.SaveChangesAsync();
                    await GenerateNotifications(formReqDb);
                    return RedirectToAction(nameof(TechnicianForm));
            }

            try
            {
                await _context.SaveChangesAsync();
                await GenerateNotifications(formReqDb);
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
            var username = User.Identity.Name;
            if (username == null)
            {
                return RedirectToAction("Index");
            }
            var user = await _userManager.FindByEmailAsync(username);
            var Site = user.Site;
            var model = new RequestViewModel
            {
                FormReqDbs = await _context.FormReqDb.Include(m => m.Equipments).Where(j => (j.status.Contains("Transitting") || j.status.Contains("Send back") || j.status.Contains("Return")) && j.Site.Contains(Site)).ToListAsync(),
                AllUsers = _userManager.Users.ToList()
            };
            return View(model);
        }
      
        public async Task<IActionResult> RegistryDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var formReqDb = await _context.FormReqDb
                .FirstOrDefaultAsync(m => m.Id == id);
            var AllUsers = _userManager.Users.ToList();

            var viewModel = new RequestViewModel
            {
                FormReqDb = formReqDb,
                Registry = new Models.Registry(),
                AllUsers = AllUsers
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
            await GenerateNotifications(formReqDb);
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
            var username = User.Identity.Name;
            if (username == null)
            {
                return RedirectToAction("Index");
            }
            var user = await _userManager.FindByEmailAsync(username);
            var Site = user.Site;
            var model = new RequestViewModel
            {

                FormReqDb = await _context.FormReqDb.ToListAsync(),
                RegistryList = await _context.Registry.Include(m => m.Equipment).Where(j => j.From == Site || j.To == Site).ToListAsync(),
                AllUsers = _userManager.Users.ToList()
            };
            return View(model);
        }
        public async Task<IActionResult> DetailsTransiteForm(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var Registry = await _context.Registry.FindAsync(id);
            var formReqDb = await _context.FormReqDb
               .FirstOrDefaultAsync(m => m.Id == Registry.FormReqDbId);


            var AllUsers = _userManager.Users.ToList();

            var viewModel = new RequestViewModel
            {
                FormReqDb = formReqDb,
                Registry = Registry,
                AllUsers = AllUsers
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
                Registry = f.Registries.FirstOrDefault() ?? new Models.Registry()
                {
                    
                }

            }).ToList();

            return View("NewComponent", viewModelList);

        }


        public async Task<IActionResult> ITOform()
        {
            var model = new RequestViewModel
            {
                FormReqDbs = await _context.FormReqDb.Include(m => m.Equipments).Include(m => m.ITTReports).ToListAsync(),
                AllUsers = _userManager.Users.ToList()
            };
            return View(model);
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
        public async Task<IActionResult> ITOstatus(int id, string Status, string Remarks)
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
            await GenerateNotifications(formReqDb);
            return RedirectToAction("ITOpage");

        }


        [HttpPost]
        public async Task<IActionResult> UpdateStatusAsync(int id, string actionType)
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
                    request.Pointer = 0;
                    _context.SaveChanges();
                    return Json(new { success = true, newStatus = request.status });
                }
                else
                {
                    request.status = "rejected";
                    request.Pointer = 0;
                    _context.SaveChanges();
                    return Json(new { success = true, newStatus = request.status });
                }
            }
            else if (actionType == "accept")
            {
                if (request.Pointer == atNewRequest)
                {
                    request.status = "Accepted";
                    request.Pointer += 1;
                    _context.SaveChanges();
                    return Json(new { success = true, newStatus = request.status });
                }
                else if (request.Pointer == atOnsiteRequest)
                {
                    request.status = "repairing";
                    request.Pointer += 2;
                    _context.SaveChanges();
                    return Json(new { success = true, newStatus = request.status });
                }
                else if (request.Pointer == atTransitRequest)
                {
                    request.status = "Transitting";
                    request.Pointer += 2;
                    _context.SaveChanges();
                    return Json(new { success = true, newStatus = request.status });
                }
                else if (request.Pointer == atPendingRequest)
                {
                    request.status = "Start repairing";
                    request.Pointer += 1;
                    _context.SaveChanges();
                    return Json(new { success = true, newStatus = request.status });
                }
                else if (request.Pointer == atFinalRequest)
                {
                    if (checkRegistry)
                    {
                        request.status = "Return";
                        request.Pointer += 1;
                        _context.SaveChanges();
                        return Json(new { success = true, newStatus = request.status });
                    }
                    else
                    {
                        request.status = "Complete";
                        request.Pointer += 1;
                        _context.SaveChanges();
                     
                        return Json(new { success = true, newStatus = request.status });
                    }

                }
            }
            _context.SaveChanges();
            await GenerateNotifications(request);

            return Json(new { success = true, newStatus = request.status });
        }
        //IT TECHNICIAN
        public async Task<IActionResult> TechnicianForm()
        {
            var model = new RequestViewModel
            {
                FormReqDbs = await _context.FormReqDb.ToListAsync(),
                RegistryList = await _context.Registry.ToListAsync(),
                AllUsers = _userManager.Users.ToList()
            };
            return View(model);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Report(int id, string SerialNumber, string Status, string Remarks, RequestViewModel model)
        {
            var formReqDb = await _context.FormReqDb.FindAsync(id);
            if (formReqDb == null)
                return NotFound();

            if (formReqDb.Pointer == 4)
            {
                var ITTreport = await _context.ITTreport.FirstOrDefaultAsync(f => f.FormReqDb == id);
                if (ITTreport != null) ITTreport.Report += " " + Remarks;
                formReqDb.status = "Final request";
                formReqDb.Pointer = 5;
            }
            else if (formReqDb.Pointer == 3)
            {
                _context.ITTreport.Add(new ITTreport
                {
                    FormReqDb = id,
                    SerialNumber = SerialNumber,
                    Report = Remarks
                });
                formReqDb.status = "Pending request";
                formReqDb.Pointer = 4;
            }
            else
            {
                formReqDb.status = "Pending request";
                formReqDb.remarks = Remarks;
            }

            try
            {
                await _context.SaveChangesAsync();
                await GenerateNotifications(formReqDb);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FormReqDbExists(id))
                    return NotFound();
                else
                    throw;
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
            var equipment = await _context.Equipment.FirstOrDefaultAsync(m => m.SerialNumber == formReqDb.SerialNumber);
            var allUsers = _context.Users.ToList();
            var viewModel = new RequestViewModel
            {
                FormReqDb = formReqDb,
                AllUsers = allUsers,
                Inventory = equipment
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
                AllUsers = _userManager.Users.ToList()
            };
            return View(model);
        }

        
        public async Task<IActionResult> MovementConfirmation()
        {
            var model = new RequestViewModel
            {
                FormReqDbs = await _context.FormReqDb.Where(j => j.status == "Accepted").ToListAsync(),
                RegistryList = await _context.Registry.ToListAsync(),
                AllUsers = _userManager.Users.ToList()
            };
            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> GetFormRequests(string site, string department, string type)
        {
            var records = await _context.Equipment.Where(f =>
            f.Site == site &&
            f.Department == department &&
            f.EquipmentType == type).ToListAsync();

            return Json(records);
        }



        [HttpGet("FormReqDbs/EditUser/{UserName}")]
        public async Task<IActionResult> EditUser(string? UserName)
        {
            if (UserName == null)
            {
                return NotFound();
            }
            var allUsers = _context.Users.ToList();
            var Alluser = await _context.Alluser.FirstOrDefaultAsync(m => m.UserName == UserName);
            if (Alluser == null)
            {
                return NotFound();
            }
            var viewModel = new RequestViewModel

            {
                AllUsers = allUsers,
                User = Alluser
            };

            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> editUser(string userName, string Supervisor, string Site, string dept)
        {
            Console.WriteLine($"Searching for username: {userName}");
            var Alluser = await _context.Alluser.FirstOrDefaultAsync(m => m.UserName == userName);
            Alluser.Supervisor = Supervisor;
            Alluser.Site = Site;
            Alluser.Dept = dept;
            await _context.SaveChangesAsync();
            return RedirectToAction("UserForm");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FilterByDate(DateTime Date)
        {
            var Request = await _context.FormReqDb.Where(m => m.RequestDate > Date).Include(j => j.Equipments).ToListAsync();
            var view = new RequestViewModel
            {
                FormReqDb = Request,
                AllUsers = await _context.Alluser.ToListAsync(),
            };
            return View("SReportForm", view);
        }
                     
        public async Task<IActionResult> SReportForm()
        {

            var name = User.Identity.Name;
            if (name == null)
            {
                return RedirectToAction("Index");
            }
            var user = await _context.Alluser.FirstOrDefaultAsync(m => m.UserName == name);
            if (user.Type == "User")
            {
                var User = new RequestViewModel
                {
                    FormReqDbs = await _context.FormReqDb.Include(m => m.Equipments).Where(j => j.ResponsibleOfficer == name).ToListAsync(),
                    RegistryList = await _context.Registry.ToListAsync(),
                    AllUsers = _userManager.Users.ToList()
                };
                return View(User);
            }
            else if (user.Type == "Supervisor")
            {
                var Supervisor = new RequestViewModel
                {
                    FormReqDbs = await _context.FormReqDb.Include(m => m.Equipments).Where(j => j.Supervisor == name || j.ResponsibleOfficer == name).ToListAsync(),
                    RegistryList = await _context.Registry.ToListAsync(),
                    AllUsers = _userManager.Users.ToList()
                };
                return View(Supervisor);
            }
            else if (user.Type == "Technician" || user.Type == "ITO" || user.Type == "Admin")
            {
                var model = new RequestViewModel
                {
                    FormReqDbs = await _context.FormReqDb.Include(m => m.Equipments).ToListAsync(),
                    RegistryList = await _context.Registry.ToListAsync(),
                    AllUsers = _userManager.Users.ToList()
                };
                return View(model);
            }
            else if (user.Type == "Registry")
            {
                var Registry = new RequestViewModel
                {
                    FormReqDbs = await _context.FormReqDb.Include(m => m.Equipments).Where(m => m.Site == user.Site).ToListAsync(),
                    RegistryList = await _context.Registry.ToListAsync(),
                    AllUsers = _userManager.Users.ToList()
                };
                return View(Registry);
            }
            return View();
        }


        public async Task<IActionResult> ReportDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var formReqDb = await _context.FormReqDb
                .Include(m => m.Equipments)
                .Include(m => m.Registries) 
                .Where(m => m.Id == id)
                .ToListAsync();

            if (formReqDb == null || !formReqDb.Any())
            {
                return NotFound();
            }

            var allUsers = _userManager.Users.ToList();

            var viewModel = new RequestViewModel
            {
                FormReqDbs = formReqDb,
                AllUsers = allUsers
            };

            return View(viewModel);
        }




        [HttpPost]
        public async Task<ActionResult> GeneratePdfAsync(DateTime date)
        {
            var Request = await _context.FormReqDb.Where(m => m.RequestDate < date).Include(j => j.Equipments).ToListAsync();
            using (var memoryStream = new MemoryStream())
            {
                var writer = new iText.Kernel.Pdf.PdfWriter(memoryStream);
                var pdf = new iText.Kernel.Pdf.PdfDocument(writer);
                var document = new iText.Layout.Document(pdf);

                document.Add(new iText.Layout.Element.Paragraph("Product List")
                    .SetFontSize(16)
                    .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER));

                // Create table with 3 columns
                var table = new iText.Layout.Element.Table(6, true);

                // Header row
                table.AddHeaderCell("Request date");
                table.AddHeaderCell("Responsible Officer");
                table.AddHeaderCell("Supervisor");
                table.AddHeaderCell("Equipment Name");
                table.AddHeaderCell("Equipment type");
                table.AddHeaderCell("Serial number");

                foreach (var item in Request)
                {

                    table.AddCell(item.RequestDate.ToString());
                    table.AddCell(item.ResponsibleOfficer);
                    table.AddCell(item?.Supervisor ?? "");
                    table.AddCell(item.Equipments.EquipmentName);
                    table.AddCell(item.Equipments.EquipmentType);
                    table.AddCell(item.Equipments.SerialNumber);
                }
                // Add table to document
                document.Add(table);
                document.Close();

                return File(memoryStream.ToArray(), "application/pdf", "static-table.pdf");
            }
        }


        private async Task GenerateNotifications(FormReqDb request)
        {
            List<Notifications> newNotifications = new();

            if (request.Pointer == 0)
            {
                // Notify supervisors
                var supervisors = await _context.Alluser
                    .Where(u => u.Type == "Supervisor" && u.Site == request.Site && u.Dept == request.Department)
                    .ToListAsync();

                foreach (var sup in supervisors)
                {
                    newNotifications.Add(new Notifications
                    {
                        UserId = sup.Id,
                        Message = $"📩 New request #{request.Id} submitted by {request.ResponsibleOfficer}"
                    });
                }
            }
            else if (request.Pointer == 1)
            {
                // Notify technician
                var techs = await _context.Alluser
                    .Where(u => u.Type == "Technician" && u.Site == request.Site && u.Dept == request.Department)
                    .ToListAsync();

                foreach (var tech in techs)
                {
                    newNotifications.Add(new Notifications
                    {
                        UserId = tech.Id,
                        Message = $"✅ Supervisor accepted request #{request.Id}."
                    });
                }
            }
            else if (request.Pointer == 9)
            {
                // Notify user
                var user = await _context.Alluser.FirstOrDefaultAsync(u => u.Id == request.UserId);
                if (user != null)
                {
                    newNotifications.Add(new Notifications
                    {
                        UserId = user.Id,
                        Message = $"✔ Your request #{request.Id} has been closed."
                    });
                }
            }

            if (newNotifications.Any())
            {
                _context.Notifications.AddRange(newNotifications);
                await _context.SaveChangesAsync();
            }
        }


      


    }
}
