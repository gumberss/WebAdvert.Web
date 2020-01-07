using System;
using System.Linq;
using System.Threading.Tasks;
using Amazon.AspNetCore.Identity.Cognito;
using Amazon.Extensions.CognitoAuthentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebAdvert.Web.Models.Accounts;

namespace WebAdvert.Web.Controllers
{
    public class Accounts : Controller
    {
        private readonly SignInManager<CognitoUser> _singInManager;
        private readonly UserManager<CognitoUser> _userManager;
        private readonly CognitoUserPool _pool;

        public Accounts(SignInManager<CognitoUser> singInManager, UserManager<CognitoUser> userManager, CognitoUserPool pool)
        {
            _singInManager = singInManager;
            _userManager = userManager;
            _pool = pool;
        }

        public async Task<IActionResult> Signup()
        {
            var model = new SignupModel();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Signup(SignupModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _pool.GetUser(model.Email);

                if(user.Status != null)
                {
                    ModelState.AddModelError("UserExists", "User with this email already exists");
                    return View(model);
                }

                user.Attributes.Add(CognitoAttribute.Name.AttributeName, model.Email);

                var createdUser = await _userManager.CreateAsync(user, model.Password);

                if (createdUser.Succeeded)
                {
                    return RedirectToAction("Confirm");
                }
            }

            return View();
        }

        public async Task<IActionResult> Confirm(ConfirmModel model)
        {
            return View(model);
        }

        [HttpPost]
        [ActionName("Confirm")]
        public async Task<IActionResult> ConfirmPost(ConfirmModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email).ConfigureAwait(false);

                if(user == null)
                {
                    ModelState.AddModelError("NotFound", "A user with the given email address was not found");
                    return View(model);
                }

                var result = await (_userManager as CognitoUserManager<CognitoUser>).ConfirmSignUpAsync(user, model.Code, true).ConfigureAwait(false);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (var item in result.Errors)
                    {
                        ModelState.AddModelError(item.Code, item.Description);
                    }

                    return View(model);
                }
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Login(LoginModel model)
        {
            return View(model);
        }

        [HttpPost]
        [ActionName("Login")]
        public async Task<IActionResult> LoginPost(LoginModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _singInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);

            if(result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("LoginError", "Email and password do not match");
            }

            return View("Login", model);
        }

        [HttpGet]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordModel model)
        {
            return View(model);
        }

        [HttpPost]
        [ActionName("ForgotPassword")]
        public async Task<IActionResult> ForgotPasswordPost(ForgotPasswordModel model)
        {
            if (!ModelState.IsValid) View(model);

            var user = _pool.GetUser(model.Email);

            var response = await (_userManager as CognitoUserManager<CognitoUser>).ResetPasswordAsync(user).ConfigureAwait(false);

            if (response.Succeeded)
            {
                return RedirectToAction("ConfirmForgotPasswordCode", "Accounts");
            }
            else
            {
                ModelState.AddModelError("Error", "Ooups!!");

                foreach (var error in response.Errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }

                return View(model);
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmForgotPasswordCode(ConfirmForgotPassword model)
        {
            return View(model);
        }

        [HttpPost]
        [ActionName("ConfirmForgotPasswordCode")]
        public async Task<IActionResult> ConfirmForgotPasswordPost(ConfirmForgotPassword model)
        {
            if (!ModelState.IsValid) View(model);

            var user = _pool.GetUser(model.Email);

            var response = await _userManager.ResetPasswordAsync(user, model.Code, model.NewPasword).ConfigureAwait(false);

            if (!response.Succeeded)
            {
                ModelState.AddModelError("Error", "Ooups!!");

                foreach (var error in response.Errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }
             
                return View(model);
            }
            else
            {
                return View("Login");
            }
        }
    }
}