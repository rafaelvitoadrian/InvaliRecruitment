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
using System.Runtime.ConstrainedExecution;

namespace InveliTestRecuruitment.Controllers
{
    public class PurchaseOrderController : Controller
    {
        private readonly IConfiguration _configuration;

        public PurchaseOrderController(IConfiguration configuration)
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
                SqlDataAdapter sqlDa = new SqlDataAdapter("PurchaseAll", sqlConnection);
                sqlDa.SelectCommand.CommandType = CommandType.StoredProcedure;
                sqlDa.Fill(dtbl);
            }
            return View(dtbl);
        }

        public IActionResult Create(int? id)
        {
            PurchaseOrderModel purchase = new PurchaseOrderModel();
            DataTable dtbl = new DataTable();
            List<SupplierModel> suppliers = new List<SupplierModel>();

            using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("DevConnection")))
            {
                sqlConnection.Open();
                string query = "SELECT ID, Name FROM Supplier";

                using (SqlCommand command = new SqlCommand(query, sqlConnection))
                {
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        suppliers.Add(new SupplierModel
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Name = reader["Name"].ToString()
                        });
                    }
                }
            }
            ViewBag.Supplier = suppliers;

            


            if (id > 0)
            {
                purchase = GetPurchase(id);
            }


            return View(purchase);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(int id, [Bind("Id,Code,PurchaseDate,SupplierID,Remarks")] PurchaseOrderModel purchaseOrderModel)
        {
            if (ModelState.IsValid)
            {
                using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("DevConnection")))
                {
                    sqlConnection.Open();
                    SqlCommand sqlCmd = new SqlCommand("CreateAndUpdatePurchase", sqlConnection);
                    sqlCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    sqlCmd.Parameters.AddWithValue("Id", purchaseOrderModel.Id);
                    sqlCmd.Parameters.AddWithValue("Code", purchaseOrderModel.Code);
                    sqlCmd.Parameters.AddWithValue("Date", purchaseOrderModel.PurchaseDate);
                    sqlCmd.Parameters.AddWithValue("SupplierID", purchaseOrderModel.SupplierID);
                    sqlCmd.Parameters.AddWithValue("Remarks", purchaseOrderModel.Remarks);
                    sqlCmd.ExecuteNonQuery();
                }
                return RedirectToAction(nameof(Index));
            }
            return View(purchaseOrderModel);
        }

        public async Task<IActionResult> Details(int? id)
        {
            PurchaseOrderModel purchaseOrderModel = GetPurchase(id);
            SupplierModel supplier = new SupplierModel();

            // Get Supplier Data
            using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("DevConnection")))
            {
                DataTable dtbl = new DataTable();
                sqlConnection.Open();
                SqlDataAdapter sqlDa = new SqlDataAdapter("GetSupplierByID", sqlConnection);
                sqlDa.SelectCommand.CommandType = CommandType.StoredProcedure;
                sqlDa.SelectCommand.Parameters.AddWithValue("Id", purchaseOrderModel.SupplierID);
                sqlDa.Fill(dtbl);
                if (dtbl.Rows.Count == 1)
                {
                    supplier.Id = Convert.ToInt32(dtbl.Rows[0]["Id"].ToString());
                    supplier.Code = dtbl.Rows[0]["Code"].ToString();
                    supplier.Name = dtbl.Rows[0]["Name"].ToString();
                    supplier.City = dtbl.Rows[0]["City"].ToString();
                }
            }

            ViewBag.Supplier = supplier;

            //Get Detail Order
            List<PurchaseOrderDetail> detail = new List<PurchaseOrderDetail>();
            using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("DevConnection")))
            {
                sqlConnection.Open();
                string query = "SELECT * FROM PurchaseOrderDetail WHERE PurchaseOrderID=@IdOrder";

                using (SqlCommand command = new SqlCommand(query, sqlConnection))
                {
                    command.Parameters.AddWithValue("@IdOrder", purchaseOrderModel.Id);
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        detail.Add(new PurchaseOrderDetail
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            ProductID = Convert.ToInt32(reader["ProductID"]),
                            Quantity = Convert.ToInt32(reader["Quantity"]),
                            UnitPrice = Convert.ToInt32(reader["UnitPrice"]),
                        });
                    }
                }
            }
            ViewBag.Detail = detail;

            return View(purchaseOrderModel);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            PurchaseOrderModel purchaseOrderModel = GetPurchase(id);

            // Get Supplier Data
            SupplierModel supplier = new SupplierModel();
            using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("DevConnection")))
            {
                DataTable dtbl = new DataTable();
                sqlConnection.Open();
                SqlDataAdapter sqlDa = new SqlDataAdapter("GetSupplierByID", sqlConnection);
                sqlDa.SelectCommand.CommandType = CommandType.StoredProcedure;
                sqlDa.SelectCommand.Parameters.AddWithValue("Id", purchaseOrderModel.SupplierID);
                sqlDa.Fill(dtbl);
                if (dtbl.Rows.Count == 1)
                {
                    supplier.Id = Convert.ToInt32(dtbl.Rows[0]["Id"].ToString());
                    supplier.Code = dtbl.Rows[0]["Code"].ToString();
                    supplier.Name = dtbl.Rows[0]["Name"].ToString();
                    supplier.City = dtbl.Rows[0]["City"].ToString();
                }
            }

            ViewBag.Supplier = supplier;

            return View(purchaseOrderModel);
        }

        // POST: Supplier/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("DevConnection")))
            {
                sqlConnection.Open();
                SqlCommand sqlCmd = new SqlCommand("DeletePurchaseOrder", sqlConnection);
                sqlCmd.CommandType = System.Data.CommandType.StoredProcedure;
                sqlCmd.Parameters.AddWithValue("Id", id);
                sqlCmd.ExecuteNonQuery();
            }
            return RedirectToAction(nameof(Index));
        }




        public PurchaseOrderModel GetPurchase(int? id)
        {
            PurchaseOrderModel purchaseOrderModel = new PurchaseOrderModel();

            using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("DevConnection")))
            {
                DataTable dtbl = new DataTable();
                sqlConnection.Open();
                SqlDataAdapter sqlDa = new SqlDataAdapter("GetPurchaseByID", sqlConnection);
                sqlDa.SelectCommand.CommandType = CommandType.StoredProcedure;
                sqlDa.SelectCommand.Parameters.AddWithValue("Id", id);
                sqlDa.Fill(dtbl);
                if (dtbl.Rows.Count == 1)
                {
                    purchaseOrderModel.Id = Convert.ToInt32(dtbl.Rows[0]["Id"].ToString());
                    purchaseOrderModel.Code = dtbl.Rows[0]["Code"].ToString();
                    purchaseOrderModel.PurchaseDate = Convert.ToDateTime((dtbl.Rows[0]["PurchaseDate"]));
                    purchaseOrderModel.SupplierID = Convert.ToInt32(dtbl.Rows[0]["SupplierID"].ToString());
                    purchaseOrderModel.Remarks = dtbl.Rows[0]["Remarks"].ToString();
                }
                return purchaseOrderModel;
            }
        }

        /*public PurchaseOrderModel GetSupplier(int? id, PurchaseOrderModel purchaseOrder)
        {
            using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("DevConnection")))
            {
                DataTable dtbl = new DataTable();
                sqlConnection.Open();
                SqlDataAdapter sqlDa = new SqlDataAdapter("GetSupplierByID", sqlConnection);
                sqlDa.SelectCommand.CommandType = CommandType.StoredProcedure;
                sqlDa.SelectCommand.Parameters.AddWithValue("Id", id);
                sqlDa.Fill(dtbl);
                purchaseOrder.Supplier = new SupplierModel();
                if (dtbl.Rows.Count == 1)
                { 
                    purchaseOrder.Supplier.Id = Convert.ToInt32(dtbl.Rows[0]["Id"].ToString());
                    purchaseOrder.Supplier.Code = dtbl.Rows[0]["Code"].ToString();
                    purchaseOrder.Supplier.Name = dtbl.Rows[0]["Name"].ToString();
                    purchaseOrder.Supplier.City = dtbl.Rows[0]["City"].ToString();
                }
                return purchaseOrder;
            }
        }*/
    }
}
