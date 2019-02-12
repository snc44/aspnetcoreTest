using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BooksCatalog.Models;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;

namespace BooksCatalog.Controllers
{
    public class HomeController : Controller
    {
        ProductContext dbProduct;
        IHostingEnvironment appEnvironment;

        public HomeController(ProductContext context, IHostingEnvironment appEnv)
        {
            dbProduct = context;
            appEnvironment = appEnv;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Catalog(string product_name, string manufacturer, string price)
        {
            //int pageSize = 10;

            IQueryable<Product> products = dbProduct.Products;
            var count = await products.CountAsync();
            ProductListViewModel.Quantity = count;  // количество строк в базе
            decimal priceDec;

            //PageViewModel pageView = new PageViewModel(count, page, pageSize);

            if (!String.IsNullOrEmpty(product_name))
            {
                products = products.Where(b => b.Product_name.Contains(product_name));
            }
            if (!String.IsNullOrEmpty(manufacturer))
            {
                products = products.Where(b => b.Manufacturer.Contains(manufacturer));
                //products = products.Where(b => b.Author.Contains(manufacturer));
            }
            if (!String.IsNullOrEmpty(price))
            {
                priceDec = Decimal.Parse(price);
                products = products.Where(b => b.Product_price <= priceDec);
            }
            else
            {
                priceDec = 9999999;
            }
            
            //var items = await products.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            //IQueryable<Product> productItems = items.AsQueryable();
            //int qtyAfterSearch = productItems.Count();

            ProductListViewModel productListView = new ProductListViewModel
            {
                Products = products.ToList(),
                //Products = productItems,
                //pageViewModel = pageView,
                Product_name = product_name,
                Manufacturer = manufacturer,
                Product_price = priceDec
                //CountAfterSearch = qtyAfterSearch
            };
            return View(productListView);
        }

        public IActionResult Clients()
        {
            return View(dbProduct.Clients.ToList());
        }

        public IActionResult Sold()
        {
            return View(dbProduct.SoldProducts.ToList());
        }

        public IActionResult Upload()
        {
            return View();
        }

        public IActionResult ChangePrices()
        {
            return View(dbProduct.Categories.ToList());
        }

        [HttpPost]
        public async Task<IActionResult> Upload(Product product, string categoryName)
        {
            //string path;
            //if (uploadedImage != null)
            //{
            //    string newFileName = uploadedImage.FileName.Replace(" ", "x");
            //    path = "/client_images/" + newFileName;
            //    using(var fileStream = new FileStream(appEnvironment.WebRootPath + path, FileMode.Create))
            //    {
            //        await uploadedImage.CopyToAsync(fileStream);
            //    }
            //}
            //else
            //{
            //    path = "/images/default_image.jpg";
            //}
            Category category = await dbProduct.Categories.FirstOrDefaultAsync(c => c.Category_name == categoryName);
            int categId;
            if(category != null)
            {
                categId = category.Id;
            }
            else
            {
                dbProduct.Categories.Add(new Category()
                {
                    Category_name = categoryName,
                    Products_count = 0
                });
                await dbProduct.SaveChangesAsync();
                Category newCategory = await dbProduct.Categories.FirstOrDefaultAsync(c => c.Category_name == categoryName);
                categId = newCategory.Id;
            }

            dbProduct.Products.Add(new Product()
            {
                Product_name = product.Product_name,
                Manufacturer = product.Manufacturer,
                Product_price = product.Product_price,
                Product_count = product.Product_count,
                Category_id = categId
            });
            await dbProduct.SaveChangesAsync();
            return RedirectToAction("Catalog");
        }

        [HttpGet]
        [Route("delete/{id}")]
        public IActionResult Delete(int id)
        {
            dbProduct.Remove(dbProduct.Products.Find(id));
            dbProduct.SaveChanges();
            return RedirectToAction("Catalog");
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id != null)
            {
                Product product = await dbProduct.Products.FirstOrDefaultAsync(p => p.Id == id);
                if(product != null)
                {
                    return View(product);
                }
            }
            return NotFound();
        }

        public async Task<IActionResult> Selling(int? id)
        {
            if (id != null)
            {
                Product product = await dbProduct.Products.FirstOrDefaultAsync(p => p.Id == id);
                if (product != null)
                {
                    return View(product);
                }
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Selling(int prod_id, string clientName, string clientMail, string clientPhone, string saleDate)
        {
            DateTime dateOfSale = DateTime.Parse(saleDate);
            Product product = await dbProduct.Products.FirstOrDefaultAsync(p => p.Id == prod_id);
            Client client = await dbProduct.Clients.FirstOrDefaultAsync(c => c.Name == clientName);
            int clientId;
            if (client != null)
            {
                clientId = client.Id;
                client.AmountOfPurchases += product.Product_price;
                client.Email = clientMail;
                client.Phone = clientPhone;
                await dbProduct.SaveChangesAsync();
            }
            else
            {
                dbProduct.Clients.Add(new Client()
                {
                    Name = clientName,
                    Email = clientMail,
                    Phone = clientPhone,
                    AmountOfPurchases = product.Product_price
                });
                await dbProduct.SaveChangesAsync();
                Client newClient = await dbProduct.Clients.FirstOrDefaultAsync(c => c.Name == clientName);
                clientId = newClient.Id;
            }

            dbProduct.SoldProducts.Add(new SoldProduct()
            {
                Product_name = product.Product_name,
                Manufacturer = product.Manufacturer,
                Product_price = product.Product_price,
                Product_count = product.Product_count,
                Category_id = product.Category_id,
                Sale_date = dateOfSale,
                Client_id = clientId
            });
            await dbProduct.SaveChangesAsync();
            
            return RedirectToAction("Clients");
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Product product)
        {
            // ОТСАНОВИЛСЯ НА ЭТОМ МЕТОДЕ
            //string path;
            //if (uploadedImage != null)
            //{
            //    System.IO.File.Delete(product.Image);
            //    string newFileName = uploadedImage.FileName.Replace(" ", "x");
            //    path = "/client_images/" + newFileName;
            //    using (var fileStream = new FileStream(appEnvironment.WebRootPath + path, FileMode.Create))
            //    {
            //        await uploadedImage.CopyToAsync(fileStream);
            //    }
            //}
            
            dbProduct.Products.Update(product);
            await dbProduct.SaveChangesAsync();
            return RedirectToAction("Catalog");
        }
        
        [HttpPost]
        public async Task<IActionResult> ChangePrices(int categId, string changeRadio, string percentsCount)
        {
            double percents = Double.Parse(percentsCount);
            if(changeRadio == "Up")
            {
                percents = percents / 100 + 1;
            }
            else
            {
                percents = 1 - percents / 100;
            }
            //var c_id = new SqlParameter("@categId", categId);
            //var p_count = new SqlParameter("@percents", percents);
            string per = "" + percents;
            per = per.Replace(",", ".");
            string procExec = "exec Change_prices_proc " + categId + "," + per;
            dbProduct.Database.ExecuteSqlCommand(procExec);
            return RedirectToAction("Catalog");
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
