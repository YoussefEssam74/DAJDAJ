using DAJDAJ.Entities.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DAJDAJ.Web.ViewComponents
{
    public class UserOrderCountViewComponent : ViewComponent
    {
        private readonly IUntiOfWork _unitOfWork;

        public UserOrderCountViewComponent(IUntiOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IViewComponentResult Invoke()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);

            if (claim != null)
            {
                var orderCount = _unitOfWork.OrderHeader
                    .GetAll(x => x.ApplicationUserId == claim.Value)
                    .Count();
                return View(orderCount);
            }

            return View(0);
        }
    }
}
