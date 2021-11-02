using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VieTrails_Web.Models;
using VieTrails_Web.Service.IService;

namespace VieTrails_Web.Controllers
{
    [Authorize]
    public class NationalParksController : Controller
    {
        private readonly INationalParkService _npService;

        public NationalParksController(INationalParkService npService)
        {
            _npService = npService;
        }
        public IActionResult Index()
        {
            return View(new NationalPark() { });
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Upsert(int? id)
        {
            var obj = new NationalPark();

            if (id == null) //that means create
            {
                return View(obj);
            }
            obj = await _npService.GetAsync(Constant.NationalParkAPIPath, id.GetValueOrDefault(), HttpContext.Session.GetString("JWTToken")); //Update
            if (obj == null)
            {
                return NotFound();
            }
            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(NationalPark obj)
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                if (files.Count > 0)
                {
                    byte[] p = null;
                    using (var fs1 = files[0].OpenReadStream())
                    {
                        using (var ms1 = new MemoryStream())
                        {
                            fs1.CopyTo(ms1);
                            p = ms1.ToArray();
                        }
                        obj.Picture = p;
                    }
                }
                else
                {
                    var objFromDb = await _npService.GetAsync(Constant.NationalParkAPIPath, obj.Id, HttpContext.Session.GetString("JWTToken"));
                    obj.Picture = objFromDb.Picture;
                }

                if (obj.Id == 0)
                {
                    await _npService.CreateAsync(Constant.NationalParkAPIPath, obj, HttpContext.Session.GetString("JWTToken"));
                }
                else
                {
                    await _npService.UpdateAsync(Constant.NationalParkAPIPath+obj.Id, obj, HttpContext.Session.GetString("JWTToken"));
                }

                return RedirectToAction(nameof(Index));
            }
            else
            {
                return View(obj);
            }
        }

        public async Task<IActionResult> GetAllNationalPark()
        {
            return Json(new { data = await _npService.GetAllAsync(Constant.NationalParkAPIPath, HttpContext.Session.GetString("JWTToken")) });
        }

        [HttpDelete]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var status = await _npService.DeleteAsync(Constant.NationalParkAPIPath, id, HttpContext.Session.GetString("JWTToken"));
            if (status)
            {
                return Json(new { success = true, message = "Delete Successful!" });
            }    
            return Json(new { success = false, message = "Delete Not Successful!" });
        }
    }
}
