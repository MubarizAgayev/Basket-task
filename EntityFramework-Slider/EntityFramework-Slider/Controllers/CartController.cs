using EntityFramework_Slider.Data;
using EntityFramework_Slider.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NuGet.ContentModel;

namespace EntityFramework_Slider.Controllers
{
    public class CartController : Controller
    {
        private readonly AppDbContext _context;
        public CartController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {


            List<BasketVM> basketProducts;

            if (Request.Cookies["basket"] != null)
            {
                basketProducts = JsonConvert.DeserializeObject<List<BasketVM>>(Request.Cookies["basket"]);
            }
            else
            {
                basketProducts = new List<BasketVM>();
            }

            List<BasketDetailVM> basketDetails = new();

            foreach (var product in basketProducts)
            {
                var dbProduct = await _context.Products.Include(m=>m.Images).Include(m=>m.Category).FirstOrDefaultAsync(m=>m.Id == product.Id);
                basketDetails.Add(new BasketDetailVM
                {
                    Id= dbProduct.Id,
                    Name = dbProduct.Name,
                    Description = dbProduct.Description,
                    Price= dbProduct.Price,
                    Image = dbProduct.Images.Where(m=>m.IsMain).FirstOrDefault().Image,
                    Count = product.Count,
                    Total = dbProduct.Price * product.Count,
                    CategoryName = dbProduct.Category.Name
                });
            }  

              return View(basketDetails);
        }


        //[ActionName("Delete")]
        public IActionResult DeleteProductFromBasket(int? id)
        {
            if (id is null) return BadRequest();

            List<BasketVM> basketProducts = JsonConvert.DeserializeObject<List<BasketVM>>(Request.Cookies["basket"]);

            var deletedProdcut = basketProducts.FirstOrDefault(m => m.Id == id);

            basketProducts.Remove(deletedProdcut);

            Response.Cookies.Append("basket", JsonConvert.SerializeObject(basketProducts));

            return Ok();
        }
    }
}
