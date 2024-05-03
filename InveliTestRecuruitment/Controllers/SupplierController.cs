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
    public class SupplierController : Controller
    {

        private readonly IConfiguration _configuration;

        public SupplierController(IConfiguration configuration)
        {
            this._configuration = configuration;
        }

        public IActionResult Index()
        {
            DataTable dtbl = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("DevConnection")))
            {
                sqlConnection.Open();
                SqlDataAdapter sqlDa = new SqlDataAdapter("SupplierAll", sqlConnection);
                sqlDa.SelectCommand.CommandType = CommandType.StoredProcedure;
                sqlDa.Fill(dtbl);
            }
            return View(dtbl);
        }

        // GET: Supplier/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            SupplierModel supplierModel = GetSupplier(id);

            return View(supplierModel);
        }

        // GET: Supplier/Create
        public IActionResult Create(int? id)
        {
            SupplierModel supplierModel = new SupplierModel();
            if (id > 0)
            {
                supplierModel = GetSupplier(id);
            }
            return View(supplierModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(int id, [Bind("Id,Code,Name,City")] SupplierModel supplierModel)
        {
            if (ModelState.IsValid)
            {
                using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("DevConnection")))
                {
                    sqlConnection.Open();
                    SqlCommand sqlCmd = new SqlCommand("CreateAndUpdate", sqlConnection);
                    sqlCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    sqlCmd.Parameters.AddWithValue("Id", supplierModel.Id);
                    sqlCmd.Parameters.AddWithValue("Code", supplierModel.Code);
                    sqlCmd.Parameters.AddWithValue("Name", supplierModel.Name);
                    sqlCmd.Parameters.AddWithValue("City", supplierModel.City);
                    sqlCmd.ExecuteNonQuery();
                }
                return RedirectToAction(nameof(Index));
            }
            return View(supplierModel);
        }

        // GET: Supplier/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            SupplierModel supplierModel = GetSupplier(id);
            return View(supplierModel);
        }

        // POST: Supplier/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("DevConnection")))
            {
                sqlConnection.Open();
                SqlCommand sqlCmd = new SqlCommand("DeleteSupplierByID", sqlConnection);
                sqlCmd.CommandType = System.Data.CommandType.StoredProcedure;
                sqlCmd.Parameters.AddWithValue("Id", id );
                sqlCmd.ExecuteNonQuery();
            }
            return RedirectToAction(nameof(Index));
        }


        public SupplierModel GetSupplier(int? id)
        {
            SupplierModel supplierModel = new SupplierModel();
            
            using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("DevConnection")))
            {
                DataTable dtbl = new DataTable();
                sqlConnection.Open();
                SqlDataAdapter sqlDa = new SqlDataAdapter("GetSupplierByID", sqlConnection);
                sqlDa.SelectCommand.CommandType = CommandType.StoredProcedure;
                sqlDa.SelectCommand.Parameters.AddWithValue("Id", id );
                sqlDa.Fill(dtbl);
                if(dtbl.Rows.Count  == 1) {
                    supplierModel.Id =Convert.ToInt32(dtbl.Rows[0]["Id"].ToString());
                    supplierModel.Code = dtbl.Rows[0]["Code"].ToString();
                    supplierModel.Name = dtbl.Rows[0]["Name"].ToString();
                    supplierModel.City = dtbl.Rows[0]["City"].ToString();
                }
                return supplierModel;
            }
        }
    }
}
