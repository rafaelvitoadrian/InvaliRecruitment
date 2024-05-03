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
    public class PurchaseOrderDetailController : Controller
    {
        private readonly IConfiguration _configuration;

        public PurchaseOrderDetailController(IConfiguration configuration)
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
                SqlDataAdapter sqlDa = new SqlDataAdapter("PurchaseDetailAll", sqlConnection);
                sqlDa.SelectCommand.CommandType = CommandType.StoredProcedure;
                sqlDa.Fill(dtbl);
            }
            return View(dtbl);
        }

        public IActionResult Create(int? id)
        {
            PurchaseOrderDetail OrderDetail = new PurchaseOrderDetail();
            if (id > 0)
            {
                OrderDetail = GetPurchaseDetail(id);
            }

            return View(OrderDetail);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(int id, [Bind("Id,PurchaseOrderID,ProductID,Quantity,UnitPrice")] PurchaseOrderDetail purchaseOrderDetailModel)
        {
            if (ModelState.IsValid)
            {
                using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("DevConnection")))
                {
                    sqlConnection.Open();
                    SqlCommand sqlCmd = new SqlCommand("CreateAndUpdatePurchaseDetail", sqlConnection);
                    sqlCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    sqlCmd.Parameters.AddWithValue("Id", purchaseOrderDetailModel.Id);
                    sqlCmd.Parameters.AddWithValue("PurchaseOrderID", purchaseOrderDetailModel.PurchaseOrderID);
                    sqlCmd.Parameters.AddWithValue("ProductID", purchaseOrderDetailModel.ProductID);
                    sqlCmd.Parameters.AddWithValue("Quantity", purchaseOrderDetailModel.Quantity);
                    sqlCmd.Parameters.AddWithValue("UnitPrice", purchaseOrderDetailModel.UnitPrice);
                    sqlCmd.ExecuteNonQuery();
                }
                return RedirectToAction(nameof(Index));
            }
            return View(purchaseOrderDetailModel);
        }

        public async Task<IActionResult> Details(int? id)
        {
            PurchaseOrderDetail purchaseOrderDetailModel = GetPurchaseDetail(id);

            return View(purchaseOrderDetailModel);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            PurchaseOrderDetail purchaseOrderDetailModel = GetPurchaseDetail(id);
            return View(purchaseOrderDetailModel);
        }

        // POST: Supplier/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("DevConnection")))
            {
                sqlConnection.Open();
                SqlCommand sqlCmd = new SqlCommand("DeletePurchaseOrderDetail", sqlConnection);
                sqlCmd.CommandType = System.Data.CommandType.StoredProcedure;
                sqlCmd.Parameters.AddWithValue("Id", id);
                sqlCmd.ExecuteNonQuery();
            }
            string returnUrl = Request.Headers["Referer"].ToString();

            if (!string.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
        }

        public PurchaseOrderDetail GetPurchaseDetail(int? id)
        {
            PurchaseOrderDetail purchaseOrderDetailModel = new PurchaseOrderDetail();

            using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("DevConnection")))
            {
                DataTable dtbl = new DataTable();
                sqlConnection.Open();
                SqlDataAdapter sqlDa = new SqlDataAdapter("GetPurhaseDetailByID", sqlConnection);
                sqlDa.SelectCommand.CommandType = CommandType.StoredProcedure;
                sqlDa.SelectCommand.Parameters.AddWithValue("Id", id);
                sqlDa.Fill(dtbl);
                if (dtbl.Rows.Count == 1)
                {
                    purchaseOrderDetailModel.Id = Convert.ToInt32(dtbl.Rows[0]["Id"].ToString());
                    purchaseOrderDetailModel.ProductID = Convert.ToInt32(dtbl.Rows[0]["ProductID"].ToString());
                    purchaseOrderDetailModel.PurchaseOrderID = Convert.ToInt32(dtbl.Rows[0]["PurchaseOrderID"].ToString());
                    purchaseOrderDetailModel.UnitPrice = Convert.ToInt32(dtbl.Rows[0]["UnitPrice"].ToString());
                    purchaseOrderDetailModel.Quantity = Convert.ToInt32(dtbl.Rows[0]["Quantity"].ToString());
                }
                return purchaseOrderDetailModel;
            }
        }
    }
}
