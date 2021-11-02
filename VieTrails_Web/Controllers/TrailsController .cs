using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VieTrails_Web.Models;
using VieTrails_Web.Models.ViewModels;
using VieTrails_Web.Service.IService;

namespace VieTrails_Web.Controllers
{
    [Authorize]
    public class TrailsController : Controller
    {
        private readonly INationalParkService _npService;
        private readonly ITrailService _trailService;

        public TrailsController(INationalParkService npService, ITrailService trailService)
        {
            _npService = npService;
            _trailService = trailService;
        }
        public IActionResult Index()
        {
            return View(new Trail() { });
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Upsert(int? id)
        {
            IEnumerable<NationalPark> npList = await _npService.GetAllAsync(Constant.NationalParkAPIPath, HttpContext.Session.GetString("JWTToken"));

            TrailsVM objVM = new TrailsVM()
            {
                NationalParkList = npList.Select(i => new SelectListItem {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }),
                Trail = new Trail()
            };

            if (id == null) //that means create
            {
                return View(objVM);
            }

            objVM.Trail = await _trailService.GetAsync(Constant.TrailAPIPath, id.GetValueOrDefault(), HttpContext.Session.GetString("JWTToken")); //Update
            if (objVM.Trail == null)
            {
                return NotFound();
            }
            return View(objVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(TrailsVM obj)
        {
            if (ModelState.IsValid)
            {
                if (obj.Trail.Id == 0)
                {
                    await _trailService.CreateAsync(Constant.TrailAPIPath, obj.Trail, HttpContext.Session.GetString("JWTToken"));
                }
                else
                {
                    await _trailService.UpdateAsync(Constant.TrailAPIPath + obj.Trail.Id, obj.Trail, HttpContext.Session.GetString("JWTToken"));
                }

                return RedirectToAction(nameof(Index));
            }
            else
            {
                //need re-populate 
                IEnumerable<NationalPark> npList = await _npService.GetAllAsync(Constant.NationalParkAPIPath, HttpContext.Session.GetString("JWTToken"));

                TrailsVM objVM = new TrailsVM()
                {
                    NationalParkList = npList.Select(i => new SelectListItem
                    {
                        Text = i.Name,
                        Value = i.Id.ToString()
                    }),
                    Trail = obj.Trail
                };

                return View(objVM);
            }
        }

        public async Task<IActionResult> GetAllTrail()
        {
            return Json(new { data = await _trailService.GetAllAsync(Constant.TrailAPIPath, HttpContext.Session.GetString("JWTToken")) });
        }

        [HttpDelete]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var status = await _trailService.DeleteAsync(Constant.TrailAPIPath, id, HttpContext.Session.GetString("JWTToken"));
            if (status)
            {
                return Json(new { success = true, message = "Delete Successful!" });
            }    
            return Json(new { success = false, message = "Delete Not Successful!" });
        }
    }
}
