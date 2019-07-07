using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Common.Models
{
    /*
    public class TestModel : IValidatableObject
    {
        [Required]
       // [MaximumLength(50, ErrorMessage = "The length must be less than 50.")]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Old password Required" )]
       //[Remote("CheckCorrectPassword", "Account", HttpMethod = "POST", AdditionalFields = "OldPassword", ErrorMessage = DisplayMsgs.NotCorrectOldPassword)]
        [Display(Name = "Old password")]
        public string OldPassword { get; set; }

        // Controller:
        //[HttpGet]
        //[Authorize]
        //[ValidateAntiForgeryToken]
        //[HttpPost]
        //public JsonResult CheckSamePassword(string oldPassword, string newPassword)
        //{
        //    return Json(oldPassword != newPassword);
        //}

        //        <div class="changePsd_controll">
        //    @Html.PasswordFor(m => m.NewPassword, new { minlength = "7", maxlength = "20" })<div class="inputTip_change_psd"> &lowast; please enter your new password</div>
        //    @Html.ValidationMessageFor(m => m.NewPassword)
        //</div>      

        [Required(ErrorMessage = "New password Required" )]
        //[MaxLength(20, ErrorMessage = DisplayMsgs.InvalidLengthPassword)]
        //[MinLength(7, ErrorMessage = DisplayMsgs.InvalidLengthPassword)]
        [RegularExpression(@"^((?=.*\d)(?=.*[A-Z])(?=.*[\-\+\?\*\$\[\]\^\.\(\)\|`!@#%&amp;_=:;',/])|(?=.*\d)(?=.*[a-z])(?=.*[A-Z])|(?=.*\d)(?=.*[a-z])(?=.*[\-\+\?\*\$\[\]\^\.\(\)\|`!@#%&amp;_=:;',/])|(?=.*[a-z])(?=.*[A-Z])(?=.*[\-\+\?\*\$\[\]\^\.\(\)\|`!@#%&amp;_=:;',/])).+$", ErrorMessage = "InvalidPassword")]
        //[Remote("CheckSamePassword", "Account", HttpMethod = "POST", AdditionalFields = "OldPassword,NewPassword", ErrorMessage = DisplayMsgs.SamePassword)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        //Controller:
        //if (ModelState.IsValid)
        //{}
        //else
        //{
        // ModelState.AddModelError("Name", "Error Message");
        //}  
        //Submit后地址变为~/Home/Submit,解决方法
        //1.@using (Html.BeginForm("Index", "Home", FormMethod.Post, new { id = "submitForm" }))
        //            return View("Index",hm);
        //           // return RedirectToAction("Index", "Home",);
        //           // return Index(hm);
        //2.@using (Html.BeginForm("SwitchUser", "Home", FormMethod.Get, new { id = "switchUserForm" }))
        //public ActionResult Index()
        //        {  
        //            HomeModel hm = new HomeModel(WebHelper.CurrentTm);
        //            return View(hm);
        //        }
        
        //public ActionResult SwitchUser(string ad)
        //        {...
        //        return RedirectToAction("Index", "Home");  
        //        }

        public TestModel()
        {

        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (this.OldPassword == this.NewPassword)
            {
                yield return new ValidationResult("SamePassword", new[] { "NewPassword" });
            }

        }

        //@Html.TextBoxFor(m => m.UserName, new { @class = "userName" })
        //@Html.ValidationMessageFor(m => m.UserName)
        //<div class="error">@Html.ValidationSummary(true)</div>
        //class : input-validation-error
        //ModelState["EncryKey"]!=null && ModelState["EncryKey"].Errors.Count > 0
        //ModelState.AddModelError("", ex.Message);

        //JQuery
        //        1.加以下代碼自動在submit之前進行檢驗
        //            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
        //                        "~/Scripts/jquery.unobtrusive*",
        //                        "~/Scripts/jquery.validate*"));
                        

        //@section Scripts {
        //    @Scripts.Render("~/bundles/jqueryval")
        //    <script type="text/javascript">
        //    $(function () {
        //......
        //    });
        //     </script>
        //}   

        // 2.$('#submitForm').valid()手動檢驗
        //         function submitRequest() {
        //            if ($('#submitForm').valid() == true && $('#Key').val() == $('#ConfirmKey').val()) {
        //                if ($("#dialog_go a").prop("disabled"))
        //                    return false;
        //                $("#dialog_go a").prop("disabled", true);
        //                $('#dialog_go').attr("style", "display: block; z-index: 90;");
        //                $('#submitForm').submit();
        //            }
        //            else {
        //                $('.input-validation-error:first').focus();
        //            }
        //        }                    

    }
     * */
}
