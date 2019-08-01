using Microsoft.AspNetCore.Identity;
using security_demo_angular.Infrastructure.Entities;
using security_demo_angular.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace security_demo_angular.Infrastructure.Services
{
    public interface IAuthenticationService
    {
        Task<SignInResult> LogIntoSystem(LoginModel loginModel);
        Task<IdentityResult> RegisterNewUser(RegisterModel registerModel);
        Task SignOut();
        Task<ConfirmationModel> ForgotPassword(ForgotPasswordModel forgotPasswordViewModel);
        Task<IdentityResult> ResetPassword(ResetPasswordModel forgotPasswordViewModel);
    }

    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AuthenticationService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<SignInResult> LogIntoSystem(LoginModel loginModel)
        {
            var applicationUser = await _userManager.FindByEmailAsync(loginModel.Login) ?? await _userManager.FindByNameAsync(loginModel.Login);
            if (applicationUser != null)
            {
                var result = await _signInManager.PasswordSignInAsync(applicationUser, loginModel.Password, isPersistent: false, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    await UpdateUser(applicationUser);
                }

                return result;
            }
            return SignInResult.Failed;
        }

        private async Task<IdentityResult> UpdateUser(ApplicationUser applicationUser)
        {
            applicationUser.LoginsCount++;
            applicationUser.LastLogin = DateTime.Now;

            var result = await _userManager.UpdateAsync(applicationUser);
            return result;
        }

        public async Task<IdentityResult> RegisterNewUser(RegisterModel registerModel)
        {
            var result = await _userManager.CreateAsync(new ApplicationUser
            {
                Email = registerModel.Email,
                UserName = registerModel.UserName
            },
            registerModel.Password);

            if (result.Succeeded)
            {
                var applicationUser = await _userManager.FindByEmailAsync(registerModel.Email);
                result = await _userManager.AddToRoleAsync(applicationUser, "admin"); // UserGroup.admin.ToString());
            }

            return result;
        }

        public async Task SignOut()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<ConfirmationModel> ForgotPassword(ForgotPasswordModel forgotPasswordViewModel)
        {
            ConfirmationModel confirmationModel;
            var user = await _userManager.FindByEmailAsync(forgotPasswordViewModel.Email);
            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
            {
                confirmationModel = new ConfirmationModel { Error = true };
                return confirmationModel;
            }

            var code = await _userManager.GeneratePasswordResetTokenAsync(user);

            confirmationModel = new ConfirmationModel
            {
                Code = code,
                UserId = user.Id
            };

            return confirmationModel;
        }

        public async Task<IdentityResult> ResetPassword(ResetPasswordModel resetPasswordViewModel)
        {
            var user = await _userManager.FindByEmailAsync(resetPasswordViewModel.Email);
            if (user == null)
            {
                return IdentityResult.Failed();
            }

            var result = await _userManager.ResetPasswordAsync(user, resetPasswordViewModel.Code, resetPasswordViewModel.Password);

            return result;
        }
    }
}
