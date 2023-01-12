using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using Pizza.DataAccess.Repository.IRepository;
using Pizza.Models;
using Pizza.Models.ViewModels;
using Pizza.Utility;
using System.Security.Claims;

namespace deliveryFood.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
		[BindProperty]
        public ShoppingCartVM ShoppingCartVM { get; set; }
        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

			ShoppingCartVM = new ShoppingCartVM()
			{
				ListCart = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value,
				includeProperties: "Product"),
				OrderHeader = new()
            };

			//не потрібна штука, але хай буде, бо лінь забрати, а шо як заплутаюсь? Я просто спати хочу 😫
			foreach (var cart in ShoppingCartVM.ListCart)
            {
                cart.Price = cart.Product.Prise;
                ShoppingCartVM.OrderHeader.OrderTotal+=(cart.Price * cart.Count); // ай та пофіг, най вже буде цей фор іч -_-  (неожидані сюжетні повороти у цього індуса) ЛІНЬ ДВИГУН ПРОГРЕСУ
            }                                                                                                                                              // свобода - це рабство    свободу попагаям...
			//:)   (deliveryFood\Areas\Customer\Views\Cart\Index.cshtml) || 152 серія                                                                         незнання - сила 

			return View(ShoppingCartVM);
        }

		
		public IActionResult Summary() //=====Summary
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

			ShoppingCartVM = new ShoppingCartVM()
			{
				ListCart = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value,
				includeProperties: "Product"),
				OrderHeader = new()
			};
			ShoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(
				u => u.Id == claim.Value);

			ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.ApplicationUser.Name;
			ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
			ShoppingCartVM.OrderHeader.StreetAddress = ShoppingCartVM.OrderHeader.ApplicationUser.StreetAddress;
			ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.ApplicationUser.City;
			ShoppingCartVM.OrderHeader.State = ShoppingCartVM.OrderHeader.ApplicationUser.State;
			ShoppingCartVM.OrderHeader.PostalCode = ShoppingCartVM.OrderHeader.ApplicationUser.PostalCode;
			//Та сама штука -_-
			foreach (var cart in ShoppingCartVM.ListCart)
			{
				cart.Price = cart.Product.Prise;
				ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count); 
			}                                                                                                                                              
																																						                                                                            

			return View(ShoppingCartVM);
		}
		//===============================

		[HttpPost]
		[ActionName("Summary")]
		[ValidateAntiForgeryToken]
		public IActionResult SummaryPost() //=====SummaryPOST
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

			ShoppingCartVM.ListCart = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value,
				includeProperties: "Product");

			ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusApproved;
			ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusApproved;
			ShoppingCartVM.OrderHeader.OrderDate= DateTime.Now;
			ShoppingCartVM.OrderHeader.ApplicationUserId = claim.Value;

			//Та сама штука -_- 2.0)
			foreach (var cart in ShoppingCartVM.ListCart)
			{
				cart.Price = cart.Product.Prise;
				ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
			}

			_unitOfWork.OrderHeader.Add(ShoppingCartVM.OrderHeader);
			_unitOfWork.Save();
			foreach (var cart in ShoppingCartVM.ListCart)
			{
				OrderDetail orderDetail = new()
				{
					ProductId = cart.ProductId,
					OrderId = ShoppingCartVM.OrderHeader.Id,
					Price = cart.Price,
					Count = cart.Count
				};
				_unitOfWork.OrderDetails.Add(orderDetail);
				_unitOfWork.Save();
			}
			_unitOfWork.ShoppingCart.RemoveRange(ShoppingCartVM.ListCart);
			_unitOfWork.Save();

			return RedirectToAction("Index", "Home");
		}

		

		//===============================

		public IActionResult Plus(int cartId) // +++
		{
			var cart = _unitOfWork.ShoppingCart.GetFirstOrDefault(u => u.Id == cartId);
            _unitOfWork.ShoppingCart.IncrementCount(cart, 1);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
		}

		public IActionResult Minus(int cartId) // ---
		{
			var cart = _unitOfWork.ShoppingCart.GetFirstOrDefault(u => u.Id == cartId);
            if (cart.Count <= 1)
            {
                _unitOfWork.ShoppingCart.Remove(cart);
            }
            else
            {
                _unitOfWork.ShoppingCart.DecrementCount(cart, 1);
            }
                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
		}

		public IActionResult Remove(int cartId) // remove
		{
			var cart = _unitOfWork.ShoppingCart.GetFirstOrDefault(u => u.Id == cartId);
			_unitOfWork.ShoppingCart.Remove(cart);
			_unitOfWork.Save();
			return RedirectToAction(nameof(Index));
		}
	}
}
