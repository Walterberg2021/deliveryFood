using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pizza.DataAccess.Repository.IRepository;
using Pizza.Models;
using Pizza.Models.ViewModels;
using Pizza.Utility;
using System.Diagnostics;
using System.Security.Claims;

namespace deliveryFood.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize]
	public class OrderController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;
		[BindProperty]
		public OrderVM OrderVM { get; set; }

        public OrderController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public IActionResult Index()
		{
			return View();
		}

        public IActionResult Details(int orderId)
        {
			OrderVM = new()
			{
				OrderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == orderId, includeProperties: "ApplicationUser"),
				OrderDetail = _unitOfWork.OrderDetails.GetAll(u => u.OrderId == orderId, includeProperties: "Product")
			};
            return View(OrderVM);
        }

		[HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
		[ValidateAntiForgeryToken]
        public IActionResult UpdateOrderDetail()
        {
			var orderHeaderFromDb = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == OrderVM.OrderHeader.Id, tracked:false);
            
			orderHeaderFromDb.Name= OrderVM.OrderHeader.Name;
            orderHeaderFromDb.PhoneNumber= OrderVM.OrderHeader.PhoneNumber;
            orderHeaderFromDb.StreetAddress= OrderVM.OrderHeader.StreetAddress;
            orderHeaderFromDb.City= OrderVM.OrderHeader.City;
            orderHeaderFromDb.State= OrderVM.OrderHeader.State;
            orderHeaderFromDb.PostalCode= OrderVM.OrderHeader.PostalCode;
			_unitOfWork.OrderHeader.Update(orderHeaderFromDb);
			_unitOfWork.Save();

			TempData["Success"] = "Order Details Updated";
			return RedirectToAction("Details", "Order", new { orderId = orderHeaderFromDb.Id });
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        [ValidateAntiForgeryToken]
        public IActionResult StartProcessing()
        {
			_unitOfWork.OrderHeader.UpdateStatus(OrderVM.OrderHeader.Id, SD.StatusProcess);
            _unitOfWork.Save();

            TempData["Done"] = "Order Status Updated";
            return RedirectToAction("Details", "Order", new { orderId = OrderVM.OrderHeader.Id });
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        [ValidateAntiForgeryToken]
        public IActionResult ShipOrder()
        {
            //var orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == OrderVM.OrderHeader.Id, tracked: false);
            _unitOfWork.OrderHeader.UpdateStatus(OrderVM.OrderHeader.Id, SD.StatusShipped);
            //_unitOfWork.OrderHeader.Update(orderHeaderFromDb);
            _unitOfWork.Save();

            TempData["Done"] = "Order Shipped Updated";
            return RedirectToAction("Details", "Order", new { orderId = OrderVM.OrderHeader.Id });
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        [ValidateAntiForgeryToken]
        public IActionResult CancelOrder() //-=-=-=-=-=-=-
        {

            //var orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == OrderVM.OrderHeader.Id, tracked: false);
            _unitOfWork.OrderHeader.UpdateStatus(OrderVM.OrderHeader.Id, SD.StatusCancelled);

            _unitOfWork.Save();

            TempData["Done"] = "Order Cancelled";
            return RedirectToAction("Details", "Order", new { orderId = OrderVM.OrderHeader.Id });
        }

        #region API CALLS
        [HttpGet]
		public IActionResult GetAll(string status)
		{
			IEnumerable<OrderHeader> orderHeaders;
			if (User.IsInRole(SD.Role_Admin) || User.IsInRole(SD.Role_Employee))
			{
				orderHeaders = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser");
			}else
			{
				var claimsIdentity = (ClaimsIdentity)User.Identity;
				var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                orderHeaders = _unitOfWork.OrderHeader.GetAll(u=>u.ApplicationUserId==claim.Value, includeProperties: "ApplicationUser");
            }

			switch (status)
			{
				case "inprocess":
					orderHeaders = orderHeaders.Where(u => u.OrderStatus == SD.StatusProcess);
                    break;

                case "complited":
                    orderHeaders = orderHeaders.Where(u => u.OrderStatus == SD.StatusShipped);
                    break;

                case "approved":
                    orderHeaders = orderHeaders.Where(u => u.OrderStatus == SD.StatusApproved);
                    break;

                default:
                    break;
            }
			
			return Json(new { data = orderHeaders });
		}
		#endregion
	}
}
