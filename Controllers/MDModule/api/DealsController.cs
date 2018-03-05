using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using PCBookWebApp.DAL;
using PCBookWebApp.Models.MDModule;
using System.Web;
using System.IO;
using Newtonsoft.Json;
using System.Data.SqlClient;
using System.Configuration;

namespace PCBookWebApp.Controllers.MDModule.api
{
    public class DealsController : ApiController
    {
        private PCBookWebAppContext db = new PCBookWebAppContext();

        private string UploadFile(HttpPostedFile file, string mapPath)
        {

            string fileName = new FileInfo(file.FileName).Name;

            if (file.ContentLength > 0)
            {
                Guid id = Guid.NewGuid();

                var filePath = Path.GetFileName(id.ToString() + "_" + fileName);

                if (!File.Exists(mapPath + filePath))
                {

                    file.SaveAs(mapPath + filePath);
                    return filePath;
                }
                return null;
            }
            return null;

        }


        [Route("api/Deals/DealsLists")]
        [HttpGet]
        [ResponseType(typeof(DealsView))]
        public IHttpActionResult GetDealsLists()
        {

            List<DealsView> list = new List<DealsView>();
            DealsView aObj = new DealsView();

            string connectionString = ConfigurationManager.ConnectionStrings["PCBookWebAppContext"].ConnectionString;
            string queryString = @"SELECT Id, Name, Description FROM Deals";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        Guid id = (Guid)reader["Id"];
                        string name = (string)reader["Name"];
                        string description = (string)reader["Description"];
                        // Images
                        List<Deal_Image> listImg = new List<Deal_Image>();
                        Deal_Image aImgObj = new Deal_Image();

                        string queryStringImage = @"SELECT  Id, ImageUrl, DealId FROM Deal_Image WHERE (DealId = @DealId)";
                        using (SqlConnection connectionImage = new SqlConnection(connectionString))
                        {
                            SqlCommand commandImage = new SqlCommand(queryStringImage, connectionImage);
                            connectionImage.Open();
                            commandImage.Parameters.Add(new SqlParameter("@DealId", id));
                            SqlDataReader readerImage = commandImage.ExecuteReader();
                            try
                            {
                                while (readerImage.Read())
                                {
                                    Guid imageId = (Guid)readerImage["Id"];
                                    string imagePath = (string)readerImage["ImageUrl"];
                                    aImgObj = new Deal_Image();
                                    aImgObj.Id = imageId;
                                    aImgObj.ImageUrl = imagePath;
                                    listImg.Add(aImgObj);
                                }
                            }
                            finally
                            {
                                readerImage.Close();
                            }
                        }
                        // Production
                        List<DealProduction> listProduction = new List<DealProduction>();
                        DealProduction aProductionObj = new DealProduction();
                        double totalProduction = 0;
                        DateTime lastProduction = DateTime.Now;
                        string queryStringProduction = @"SELECT        
                                                            DealId, FactoryName, Quantity, DealProductionDate, ReProduction, DateCreated, DealProductionId
                                                            FROM            
                                                            dbo.DealProductions WHERE (DealId = @DealId) ORDER BY DealProductionDate ASC";
                        using (SqlConnection connectionProduction = new SqlConnection(connectionString))
                        {
                            SqlCommand commandProduction = new SqlCommand(queryStringProduction, connectionProduction);
                            connectionProduction.Open();
                            commandProduction.Parameters.Add(new SqlParameter("@DealId", id));
                            SqlDataReader readerProduction = commandProduction.ExecuteReader();
                            try
                            {
                                while (readerProduction.Read())
                                {
                                    totalProduction = totalProduction + (double)readerProduction["Quantity"];
                                    lastProduction = (DateTime)readerProduction["DealProductionDate"];
                                    int DealProductionId = (int)readerProduction["DealProductionId"];
                                    aProductionObj = new DealProduction();
                                    aProductionObj.DealProductionId = DealProductionId;
                                    aProductionObj.Quantity = (double)readerProduction["Quantity"];
                                    aProductionObj.FactoryName = (string)readerProduction["FactoryName"];
                                    aProductionObj.DealProductionDate = (DateTime)readerProduction["DealProductionDate"];
                                    listProduction.Add(aProductionObj);
                                }
                            }
                            finally
                            {
                                readerProduction.Close();
                            }
                        }
                        aObj = new DealsView();
                        aObj.Id = id;
                        aObj.Name = name;
                        aObj.Description = description;
                        aObj.TotalProduction = totalProduction;
                        aObj.Deal_Images = listImg;
                        aObj.DealProductions = listProduction;
                        aObj.LastProduction = lastProduction;
                        list.Add(aObj);
                    }
                }
                finally
                {
                    reader.Close();
                }
            }
            return Ok(list);
        }

        [Route("api/Deals/DealsList")]
        [HttpGet]
        public IHttpActionResult GetDealsList()
        {
            var list = db.Deals
                        .ToList()
                        .Select(x => new { x.Id, x.Name, x.Description })
                        .ToList();
            return Ok(list);

        }
        // GET: api/Deals
        public IQueryable<Deal> GetDeals()
        {

            return db.Deals;
        }

        // GET: api/Deals/5
        [ResponseType(typeof(Deal))]
        public async Task<IHttpActionResult> GetDeal(Guid id)
        {
            Deal deal = await db.Deals.FindAsync(id);
            if (deal == null)
            {
                return NotFound();
            }

            return Ok(deal);
        }

        // PUT: api/Deals/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutDeal(Guid id, Deal deal)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != deal.Id)
            {
                return BadRequest();
            }

            db.Entry(deal).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DealExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Deals
        [ResponseType(typeof(Deal))]
        //public async Task<IHttpActionResult> PostDeal(Deal deal)
        public IHttpActionResult Post()
        {
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}

            //db.Deals.Add(deal);

            //try
            //{
            //    await db.SaveChangesAsync();
            //}
            //catch (DbUpdateException)
            //{
            //    if (DealExists(deal.Id))
            //    {
            //        return Conflict();
            //    }
            //    else
            //    {
            //        throw;
            //    }
            //}

            //return CreatedAtRoute("DefaultApi", new { id = deal.Id }, deal);
            string path = HttpContext.Current.Server.MapPath("../Upload//FinishedGoodImages//");

            var model = HttpContext.Current.Request.Form["DealModel"];

            var deal = JsonConvert.DeserializeObject<Deal>(model);

            if (deal == null)
            {
                return BadRequest();
            }
            // This is for demo. So I want to make this simple.
            deal.Id = Guid.NewGuid();

            var retval = db.Deals.Add(deal);


            var files = HttpContext.Current.Request.Files;

            if (retval != null)
            {
                if (files.Count > 0)
                {
                    for (int i = 0; i < files.Count; i++)
                    {
                        HttpPostedFile file = files[i];
                        string fileName = UploadFile(file, path);
                        db.Deal_Image.Add(new Deal_Image { Id = Guid.NewGuid(), DealId = deal.Id, ImageUrl = "/Upload/FinishedGoodImages/" + fileName });
                    }
                }
                db.SaveChanges();

            }
            return Ok();
        }

        // DELETE: api/Deals/5
        [ResponseType(typeof(Deal))]
        public async Task<IHttpActionResult> DeleteDeal(Guid id)
        {
            Deal deal = await db.Deals.FindAsync(id);
            if (deal == null)
            {
                return NotFound();
            }

            db.Deals.Remove(deal);
            await db.SaveChangesAsync();

            return Ok(deal);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool DealExists(Guid id)
        {
            return db.Deals.Count(e => e.Id == id) > 0;
        }
    }
}