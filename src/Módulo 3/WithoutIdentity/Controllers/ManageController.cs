using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using WithoutIdentity.Models;
using WithoutIdentity.Models.ManageViewModels;

namespace WithoutIdentity.Controllers
{
    public class ManageController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public ManageController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [TempData]
        public string StatusMessage { get; set; }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                throw new ApplicationException($"Não foi possível carregar o usuário com o ID '{_userManager.GetUserId(User)}'");
            }

            var model = new IndexViewModel
            {
                Username = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                IsEmailConfirmed = user.EmailConfirmed,
                StatusMeessage = StatusMessage
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index (IndexViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                throw new ApplicationException($"Não foi possível carregar o usuário com o ID '{_userManager.GetUserId(User)}'");
            }

            var email = user.Email;

            if (email != model.Email)
            {
                var setEmailResult = await _userManager.SetEmailAsync(user, model.Email);

                if (!setEmailResult.Succeeded)
                {
                    throw new ApplicationException($"Erro inesperado ao atribuir um email para o usuario com ID '{user.Id}'");
                }
            }

            var phoneNumber = user.PhoneNumber;

            if (phoneNumber != model.PhoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, model.PhoneNumber);

                if (!setPhoneResult.Succeeded)
                {
                    throw new ApplicationException($"Erro inesperado ao atribuir um telefone para o usuario com ID '{user.Id}'");
                }
            }

            StatusMessage = "Seu perfil foi atualizado";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> ChangePassword()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                throw new ApplicationException($"Não foi possivel carregar os dados do usuario com id {_userManager.GetUserId(User)}");
            }

            var model = new ChangePasswordViewModel { StatusMessage = StatusMessage };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                throw new ApplicationException($"Não foi possivel carregar os dados do usuario com id {_userManager.GetUserId(User)}");
            }

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

            if (!changePasswordResult.Succeeded)
            {
                foreach (var error in changePasswordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View(model);
            }

            await _signInManager.SignInAsync(user, isPersistent: false);

            StatusMessage = "Sua senha foi alterada com sucesso";

            return RedirectToAction(nameof(ChangePassword));
        }
    }
}