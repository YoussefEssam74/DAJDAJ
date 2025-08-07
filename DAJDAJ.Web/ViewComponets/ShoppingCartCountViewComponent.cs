using DAJDAJ.Entities.Repositories;
using DAJDAJ.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DAJDAJ.Web.ViewComponents
{
    public class ShoppingCartCountViewComponent : ViewComponent
    {
        private readonly IUntiOfWork _unitOfWork;

        public ShoppingCartCountViewComponent(IUntiOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var claimsIdentity = (ClaimsIdentity?)User.Identity;
            var claim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);

            int count = 0;
            if (claim != null)
            {
                count = _unitOfWork.ShoppingCart.GetAll(
                    u => u.ApplicationUserId == claim.Value).Sum(x => x.Count);
                HttpContext.Session.SetInt32(SD.SessionKey, count);
            }
            else
            {
                HttpContext.Session.Clear();
            }
            return View(count);
        }
    }
}
