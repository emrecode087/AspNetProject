﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjectOneMil.Models;
using ProjectOneMil.ViewModels;

namespace ProjectOneMil.Controllers
{
    public class UsersController : Controller
    {
        private UserManager<AppUser> _userManager;


        public UsersController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;

        }

        public IActionResult Index()
        {
            return View(_userManager.Users);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new AppUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FullName = model.FullName
                };

                IdentityResult result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }

                foreach (IdentityError err in result.Errors) {
                    ModelState.AddModelError("", err.Description);
                }
                
            }
            return View(model);
        }

        public async Task<IActionResult> Edit(string id)
        {
            if(id == null)
            {
                return RedirectToAction("Index");
            }
            var user = await _userManager.FindByIdAsync(id);

            if(user != null)
            {
                return View(new EditViewModel
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email
                });
            }
            
                
            
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string id,EditViewModel model)
        {
           if (id != model.Id)
           {
            return RedirectToAction("Index");     
           }

           if (ModelState.IsValid)
              {
                var user = await _userManager.FindByIdAsync(id);
    
                if(user != null)
                {
                     user.FullName = model.FullName;
                     user.Email = model.Email;
    
                     var result = await _userManager.UpdateAsync(user);

                    if (result.Succeeded && !string.IsNullOrEmpty(model.Password))
                    {
                        await _userManager.RemovePasswordAsync(user);
                        await _userManager.AddPasswordAsync(user, model.Password);
                    }    
                     if (result.Succeeded)
                     {
                          return RedirectToAction("Index");
                     }
    
                     foreach (IdentityError err in result.Errors)
                     {
                          ModelState.AddModelError("", err.Description);
                     }
                }
                return RedirectToAction("Index");
              }
            
           return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if(user != null)
            {
                IdentityResult result = await _userManager.DeleteAsync(user);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "User could not be deleted");
                }
            }
            else
            {
                ModelState.AddModelError("", "User not found");
            }
            return View("Index", _userManager.Users);
        }   
    }
}
