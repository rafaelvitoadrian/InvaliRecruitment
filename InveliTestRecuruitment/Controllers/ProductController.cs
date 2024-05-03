using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using InveliTestRecuruitment.Data;
using InveliTestRecuruitment.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace InveliTestRecuruitment.Controllers
{
    public class ProductController : Controller
    {
        private readonly IConfiguration _configuration;

        public ProductController(IConfiguration configuration)
        {
            this._configuration = configuration;
        }

        // GET: Product
        public IActionResult Index()
        {
            DataTable dtbl = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("DevConnection")))
            {
                sqlConnection.Open();
                SqlDataAdapter sqlDa = new SqlDataAdapter("ProductAll", sqlConnection);
                sqlDa.SelectCommand.CommandType = CommandType.StoredProcedure;
                sqlDa.Fill(dtbl);
            }
            return View(dtbl);
        }

        public IActionResult Create(int? id)
        {
            ProductModel product = new ProductModel();
            if (id > 0)
            {
                product = GetProduct(id);
            }
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(int id, [Bind("Id,Code,Name")] ProductModel supplierModel)
        {
            if (ModelState.IsValid)
            {
                using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("DevConnection")))
                {
                    sqlConnection.Open();
                    SqlCommand sqlCmd = new SqlCommand("CreateAndUpdateProduct", sqlConnection);
                    sqlCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    sqlCmd.Parameters.AddWithValue("Id", supplierModel.Id);
                    sqlCmd.Parameters.AddWithValue("Code", supplierModel.Code);
                    sqlCmd.Parameters.AddWithValue("Name", supplierModel.Name);
                    sqlCmd.ExecuteNonQuery();
                }
                return RedirectToAction(nameof(Index));
            }
            return View(supplierModel);
        }

        public async Task<IActionResult> Details(int? id)
        {
            ProductModel productModel = GetProduct(id);

            return View(productModel);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            ProductModel productModel = GetProduct(id);
            return View(productModel);
        }

        // POST: Supplier/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("DevConnection")))
            {
                sqlConnection.Open();
                SqlCommand sqlCmd = new SqlCommand("DeleteProductByID", sqlConnection);
                sqlCmd.CommandType = System.Data.CommandType.StoredProcedure;
                sqlCmd.Parameters.AddWithValue("Id", id);
                sqlCmd.ExecuteNonQuery();
            }
            return RedirectToAction(nameof(Index));
        }




        public ProductModel GetProduct(int? id)
        {
            ProductModel productModel = new ProductModel();

            using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("DevConnection")))
            {
                DataTable dtbl = new DataTable();
                sqlConnection.Open();
                SqlDataAdapter sqlDa = new SqlDataAdapter("GetProductByID", sqlConnection);
                sqlDa.SelectCommand.CommandType = CommandType.StoredProcedure;
                sqlDa.SelectCommand.Parameters.AddWithValue("Id", id);
                sqlDa.Fill(dtbl);
                if (dtbl.Rows.Count == 1)
                {
                    productModel.Id = Convert.ToInt32(dtbl.Rows[0]["Id"].ToString());
                    productModel.Code = dtbl.Rows[0]["Code"].ToString();
                    productModel.Name = dtbl.Rows[0]["Name"].ToString();
                }
                return productModel;
            }
        }




        /*
                // GET: Product/Details/5
                public async Task<IActionResult> Details(int? id)
                {
                    if (id == null)
                    {
                        return NotFound();
                    }

                    var productModel = await _context.ProductModel
                        .FirstOrDefaultAsync(m => m.Id == id);
                    if (productModel == null)
                    {
                        return NotFound();
                    }

                    return View(productModel);
                }

                // GET: Product/Create
                public IActionResult Create()
                {
                    return View();
                }

                // POST: Product/Create
                // To protect from overposting attacks, enable the specific properties you want to bind to.
                // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
                [HttpPost]
                [ValidateAntiForgeryToken]
                public async Task<IActionResult> Create([Bind("Id,Code,Name")] ProductModel productModel)
                {
                    if (ModelState.IsValid)
                    {
                        _context.Add(productModel);
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    }
                    return View(productModel);
                }

                // GET: Product/Edit/5
                public async Task<IActionResult> Edit(int? id)
                {
                    if (id == null)
                    {
                        return NotFound();
                    }

                    var productModel = await _context.ProductModel.FindAsync(id);
                    if (productModel == null)
                    {
                        return NotFound();
                    }
                    return View(productModel);
                }

                // POST: Product/Edit/5
                // To protect from overposting attacks, enable the specific properties you want to bind to.
                // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
                [HttpPost]
                [ValidateAntiForgeryToken]
                public async Task<IActionResult> Edit(int id, [Bind("Id,Code,Name")] ProductModel productModel)
                {
                    if (id != productModel.Id)
                    {
                        return NotFound();
                    }

                    if (ModelState.IsValid)
                    {
                        try
                        {
                            _context.Update(productModel);
                            await _context.SaveChangesAsync();
                        }
                        catch (DbUpdateConcurrencyException)
                        {
                            if (!ProductModelExists(productModel.Id))
                            {
                                return NotFound();
                            }
                            else
                            {
                                throw;
                            }
                        }
                        return RedirectToAction(nameof(Index));
                    }
                    return View(productModel);
                }

                // GET: Product/Delete/5
                public async Task<IActionResult> Delete(int? id)
                {
                    if (id == null)
                    {
                        return NotFound();
                    }

                    var productModel = await _context.ProductModel
                        .FirstOrDefaultAsync(m => m.Id == id);
                    if (productModel == null)
                    {
                        return NotFound();
                    }

                    return View(productModel);
                }

                // POST: Product/Delete/5
                [HttpPost, ActionName("Delete")]
                [ValidateAntiForgeryToken]
                public async Task<IActionResult> DeleteConfirmed(int id)
                {
                    var productModel = await _context.ProductModel.FindAsync(id);
                    if (productModel != null)
                    {
                        _context.ProductModel.Remove(productModel);
                    }

                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }

                private bool ProductModelExists(int id)
                {
                    return _context.ProductModel.Any(e => e.Id == id);
                }*/
    }
}
