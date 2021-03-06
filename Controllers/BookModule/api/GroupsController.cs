﻿using System;
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
using PCBookWebApp.Models;
using PCBookWebApp.Models.ViewModels;
using System.Configuration;
using System.Data.SqlClient;
using Microsoft.AspNet.Identity;
using System.Data.Entity.Migrations;
using PCBookWebApp.Models.BookModule;
using PCBookWebApp.Models.BookModule.BookViewModel;

namespace PCBookWebApp.Controllers.BookModule.api
{
    [Authorize]
    public class GroupsController : ApiController
    {
        private PCBookWebAppContext db = new PCBookWebAppContext();

        //[Route("api/Groups/RolesTree")]
        //public IHttpActionResult GetRolesTree()
        //{
        //    object[] objRole = null;
        //    try
        //    {
        //        objRole = (
        //                from rl in db.Groups
        //                where rl.ParentId == 1 && rl.Childrens == null
        //                select new
        //                {
        //                    GroupId = rl.GroupId,
        //                    GroupName = rl.GroupName,
        //                    ParentId = rl.ParentId,
        //                    Child = rl.Childrens,
        //                    collapsed = true,
        //                    children = from cl in db.Groups
        //                               where cl.ParentId == rl.GroupId && cl.Child == 1
        //                               select new
        //                               {
        //                                   GroupId = cl.GroupId,
        //                                   GroupName = cl.GroupName,
        //                                   ParentId = cl.ParentId,
        //                                   Child = cl.Child,
        //                                   collapsed = true,
        //                                   children = from cld in db.Groups
        //                                              where cld.ParentId == cl.GroupId && cl.Child == 1
        //                                              select new
        //                                              {
        //                                                  GroupId = cld.GroupId,
        //                                                  GroupName = cld.GroupName,
        //                                                  ParentId = cld.ParentId,
        //                                                  Child = cld.Child,
        //                                                  collapsed = true
        //                                              }
        //                               }
        //                }).ToArray();
        //    }
        //    catch (Exception e)
        //    {
        //        e.ToString();
        //    }
        //    return Json(new
        //    {
        //        objRole
        //    });
        //}

        [Route("api/Groups/GroupsMultiSelectList")]
        [HttpGet]
        public IHttpActionResult GroupsMultiSelectList()
        {
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers
                .Where(a => a.Id == userId)
                .Select(a => a.ShowRoomId)
                .FirstOrDefault();

            List<MultiSelectView> unitList = new List<MultiSelectView>();
            MultiSelectView unitObj = new MultiSelectView();

            string connectionString = ConfigurationManager.ConnectionStrings["PCBookWebAppContext"].ConnectionString;
            //string queryString = @"SELECT GroupId, GroupName FROM dbo.Groups WHERE ShowRoomId=@showRoomId";
            string queryString = @"SELECT GroupId, GroupName FROM dbo.Groups";
            using (System.Data.SqlClient.SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                connection.Open();
                //command.Parameters.Add(new SqlParameter("@showRoomId", showRoomId));
                SqlDataReader reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        int id = (int)reader["GroupId"];
                        string name = (string)reader["GroupName"];
                        //int categoryId = (int)reader["MainMaterialId"];
                        unitObj = new MultiSelectView();
                        unitObj.id = id;
                        unitObj.label = name;
                        //unitObj.gender = categoryId;
                        unitList.Add(unitObj);
                    }
                }
                finally
                {
                    reader.Close();
                }
            }
            return Ok(unitList);
        }
        [Route("api/Groups/GroupListMultiSelect")]
        [HttpGet]
        [ResponseType(typeof(Ledger))]
        public IHttpActionResult GetGroupListMultiSelect()
        {
            string userName = User.Identity.GetUserName();
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers
                .Where(a => a.Id == userId)
                .Select(a => a.ShowRoomId)
                .FirstOrDefault();
            var list = db.Groups
                .Where(a => a.GroupName != "Primary" && a.IsParent == false && a.PrimaryId == null)
                .Select(e => new { id = e.GroupId, label = e.GroupName })
                .OrderBy(e => e.label);
            if (list == null)
            {
                return NotFound();
            }
            return Ok(list);
        }

        [Route("api/Groups/GetGroupList")]
        [HttpGet]
        [ResponseType(typeof(XEditGroupView))]
        public IHttpActionResult GetGroupList()
        {
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers
                .Where(a => a.Id == userId)
                .Select(a => a.ShowRoomId)
                .FirstOrDefault();

            List<XEditGroupView> ImportProductList = new List<XEditGroupView>();
            XEditGroupView importProduct = new XEditGroupView();

            string connectionString = ConfigurationManager.ConnectionStrings["PCBookWebAppContext"].ConnectionString;
            //string queryString = @"SELECT        
            //                        dbo.Groups.GroupId AS id, dbo.Groups.GroupName AS name, dbo.Groups.ParentId, dbo.Groups.GroupIdStr, dbo.Groups.PrimaryId, Groups_1.GroupName AS UnderGroup, dbo.Primaries.PrimaryName, 
            //                        dbo.Groups.ShowRoomId, dbo.Groups.TrialBalance, dbo.Groups.Provision
            //                        FROM            
            //                        dbo.Groups LEFT OUTER JOIN
            //                        dbo.Primaries ON dbo.Groups.PrimaryId = dbo.Primaries.PrimaryId LEFT OUTER JOIN
            //                        dbo.Groups AS Groups_1 ON dbo.Groups.ParentId = Groups_1.GroupIdStr WHERE dbo.Groups.ShowRoomId=@showRoomId";
            string queryString = @"SELECT        
                                    dbo.Groups.GroupId, dbo.Groups.GroupName, dbo.Groups.PrimaryId, dbo.Groups.ParentId, dbo.Groups.GroupIdStr, dbo.Groups.TrialBalance, dbo.Groups.Provision, dbo.Groups.IsParent, 
                                    Groups_1.GroupName AS UnderGroup, dbo.Primaries.PrimaryName
                                    FROM            
                                    dbo.Groups LEFT OUTER JOIN
                                    dbo.Primaries ON dbo.Groups.PrimaryId = dbo.Primaries.PrimaryId LEFT OUTER JOIN
                                    dbo.Groups AS Groups_1 ON dbo.Groups.ParentId = Groups_1.GroupId";
            using (System.Data.SqlClient.SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                connection.Open();
                //command.Parameters.Add(new SqlParameter("@showRoomId", showRoomId));
                SqlDataReader reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        int id = (int)reader["GroupId"];
                        string name = (string)reader["GroupName"];
                        importProduct = new XEditGroupView();
                        importProduct.id = id;
                        importProduct.name = name;
                        if (reader["UnderGroup"] != DBNull.Value) {
                            importProduct.UnderGroup = (string) reader["UnderGroup"];
                        }
                        if (reader["PrimaryName"] != DBNull.Value)
                        {
                            importProduct.GeneralLedgerName = (string)reader["PrimaryName"];
                        }
                        importProduct.TrialBalance = (bool)reader["TrialBalance"];
                        importProduct.Provision = (bool)reader["Provision"];

                        //if (reader["PrimaryName"] != DBNull.Value) {
                        //    importProduct.PrimaryName = (string)reader["PrimaryName"];
                        //}
                        ImportProductList.Add(importProduct);
                    }
                }
                finally
                {
                    reader.Close();
                }
            }
            //ViewBag.AccountUserList = BankAccounts;
            return Ok(ImportProductList);
        }

        [Route("api/Groups/GetDropDownListXedit")]
        [HttpGet]
        [ResponseType(typeof(Group))]
        public IHttpActionResult GetDropDownListXedit()
        {
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers
                .Where(a => a.Id == userId)
                .Select(a => a.ShowRoomId)
                .FirstOrDefault();

            var list = db.Groups
                //.Where(a=> a.ShowRoomId==showRoomId)
                .Select(e => new { id = e.GroupId, text = e.GroupName });

            if (list == null)
            {
                return NotFound();
            }
            return Ok(list);
        }

        [Route("api/Groups/GetTypeHeadList")]
        [HttpGet]
        [ResponseType(typeof(Primary))]
        public IHttpActionResult GetTypeHeadList()
        {
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers
                .Where(a => a.Id == userId)
                .Select(a => a.ShowRoomId)
                .FirstOrDefault();

            var list = db.Groups
                //.Where(a => a.ShowRoomId == showRoomId)
                .Select(e => new { GroupId = e.GroupId, GroupName = e.GroupName, GroupIdStr = e.GroupIdStr });
            if (list == null)
            {
                return NotFound();
            }
            return Ok(list);
        }

        [Route("api/Groups/GetGroupListTree")]
        [HttpGet]
        [ResponseType(typeof(ChartOfAccountView))]
        public IHttpActionResult GetGroupListTree()
        {
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers
                .Where(a => a.Id == userId)
                .Select(a => a.ShowRoomId)
                .FirstOrDefault();

            List<ChartOfAccountView> list = new List<ChartOfAccountView>();
            ChartOfAccountView aObj = new ChartOfAccountView();

            string connectionString = ConfigurationManager.ConnectionStrings["PCBookWebAppContext"].ConnectionString;
            string queryString = @"SELECT        
                                    t1.GroupName AS lev1, dbo.Primaries.PrimaryName, t2.GroupName AS lev2, t3.GroupName AS lev3, t4.GroupName AS lev4, t5.GroupName AS lev5
                                    FROM            
                                    dbo.Groups AS t1 FULL OUTER JOIN
                                    dbo.Primaries LEFT OUTER JOIN
                                    dbo.Groups AS t2 ON dbo.Primaries.PrimaryId = t2.PrimaryId ON t1.GroupId = t2.ParentId FULL OUTER JOIN
                                    dbo.Groups AS t5 FULL OUTER JOIN
                                    dbo.Groups AS t4 ON t5.ParentId = t4.GroupId FULL OUTER JOIN
                                    dbo.Groups AS t3 ON t4.ParentId = t3.GroupId ON t2.GroupId = t3.ParentId
                                    WHERE        
                                    (t1.GroupName = 'Primary')
                                    ORDER BY dbo.Primaries.PrimaryName";
            using (System.Data.SqlClient.SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                connection.Open();
                //command.Parameters.Add(new SqlParameter("@showRoomId", showRoomId));
                SqlDataReader reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        //int id = (int) reader["GroupId"];  
                        aObj = new ChartOfAccountView();
                        //aObj.GroupId = id;
                        aObj.lev2 = reader["lev2"].ToString();
                        aObj.lev3 = reader["lev3"].ToString();
                        aObj.lev4 = (string)reader["lev4"].ToString();
                        aObj.lev5 = (string)reader["lev5"].ToString();
                        if (reader["PrimaryName"] != DBNull.Value)
                        {
                            aObj.PrimaryName = reader["PrimaryName"].ToString();
                        }
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






        // GET: api/Groups
        public IQueryable<Group> GetGroups()
        {
            return db.Groups;
        }

        // GET: api/Groups/5
        [ResponseType(typeof(Group))]
        public async Task<IHttpActionResult> GetGroup(int id)
        {
            Group group = await db.Groups.FindAsync(id);
            if (group == null)
            {
                return NotFound();
            }

            return Ok(group);
        }

        // PUT: api/Groups/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutGroup(int id, Group group)
        {
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers
                .Where(a => a.Id == userId)
                .Select(a => a.ShowRoomId)
                .FirstOrDefault();

            string userName = User.Identity.GetUserName();
            DateTime updateAt = DateTime.Now;
            int PrimaryId = 0;
            string GroupIdStr = null;
            string PreviousGroupName = "";
            bool Active = false;
            string CreatedBy = null;
            DateTime DateCreated = DateTime.Now;
            int ParentId = 0;

            // Get All Record
            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["PCBookWebAppContext"].ConnectionString);
            connection.Open();
            try
            {
                SqlDataReader reader = null;
                string sql = @"SELECT dbo.Groups.* FROM   dbo.Groups WHERE GroupId=@groupId";
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.Add("@groupId", SqlDbType.Int).Value = id;
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    PreviousGroupName = (string)reader["GroupName"];
                    GroupIdStr = (string)reader["GroupIdStr"];
                    Active = (bool)reader["Active"];
                    CreatedBy = (string) reader["CreatedBy"];
                    DateCreated = (DateTime) reader["DateCreated"];
                    ParentId = (int)reader["ParentId"];
                    if (reader["PrimaryId"] != DBNull.Value)
                    {
                        PrimaryId = (int)reader["PrimaryId"];
                    }
                }
                reader.Close();
                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            // End 



            if (PrimaryId > 0)
            {
                group.PrimaryId = PrimaryId;
            }
            
            group.GroupIdStr = GroupIdStr;
            group.Active = Active;
            group.DateCreated = DateCreated;
            group.ParentId = ParentId;
            group.DateUpdated = updateAt;
            group.CreatedBy = CreatedBy;
            //group.ShowRoomId = showRoomId;

            if (PreviousGroupName == "Primary" )
            {
                ModelState.AddModelError("GroupName", "Primary is not a editable item!");
            }


            //if (db.Groups.Any(m => m.GroupName == group.GroupName && m.CreatedBy == userName))
            //{
            //    ModelState.AddModelError("GroupName", "Group Name Already Exists!");
            //}

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != group.GroupId)
            {
                return BadRequest();
            }



            db.Entry(group).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GroupExists(id))
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

        // POST: api/Groups
        [ResponseType(typeof(Group))]
        public async Task<IHttpActionResult> PostGroup(Group group)
        {
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers
                .Where(a => a.Id == userId)
                .Select(a => a.ShowRoomId)
                .FirstOrDefault();

            // group.ShowRoomId = showRoomId;

            var ParentId = group.ParentId;

            string userName = User.Identity.GetUserName();
            DateTime createdAt = DateTime.Now;
            group.CreatedBy = userName;
            group.DateCreated = createdAt;
            group.DateUpdated = createdAt;




            //if (db.Groups.Any(m => m.GroupName == group.GroupName && m.ShowRoomId == showRoomId))
            //{
            //    ModelState.AddModelError("GroupName", "Group Name Already Exists!");
            //}
            if (db.Groups.Any(m => m.GroupName == group.GroupName))
            {
                ModelState.AddModelError("GroupName", "Group Name Already Exists!");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Groups.Add(group);
            await db.SaveChangesAsync();

            var groupIdStrParent = "";
            //............IsParent Update............................
            var parentGroup = db.Groups.Where(x => x.GroupId == group.ParentId).FirstOrDefault();
            groupIdStrParent = parentGroup.GroupIdStr;
            if (parentGroup != null && !parentGroup.IsParent == true && parentGroup.GroupName != "Primary")
            {
                parentGroup.IsParent = true;
                db.Groups.AddOrUpdate(parentGroup);
                db.SaveChanges();
            }

            var groupId = group.GroupId;
            string groupIdStr = (string) (groupIdStrParent + "-"+ groupId);

            // Only for 1st time creare primary group
            if (ParentId == 0)
            {
                string groupIdStr1 =  groupId.ToString();
                //Update Balance Table
                SqlConnection sqlUpdateCon = new SqlConnection(ConfigurationManager.ConnectionStrings["PCBookWebAppContext"].ConnectionString);
                SqlCommand cmdUpdate = new SqlCommand();
                cmdUpdate.CommandType = CommandType.Text;
                cmdUpdate.CommandText = "UPDATE dbo.Groups SET [ParentId ] = @ParentId , [GroupIdStr] = @groupIdStr1 WHERE [GroupId] = @groupId1";
                cmdUpdate.Parameters.AddWithValue("@groupIdStr1", groupIdStr1);
                cmdUpdate.Parameters.AddWithValue("@ParentId", (int)groupId);
                cmdUpdate.Parameters.AddWithValue("@groupId1", (int)groupId);

                cmdUpdate.Connection = sqlUpdateCon;

                sqlUpdateCon.Open();
                cmdUpdate.ExecuteNonQuery();
                sqlUpdateCon.Close();
                //Only for 1st time creare primary group
            }
            else {
                //Update GroupIdStr 
                SqlConnection sqlBUpdateCon = new SqlConnection(ConfigurationManager.ConnectionStrings["PCBookWebAppContext"].ConnectionString);
                SqlCommand cmdBUpdate = new SqlCommand();
                cmdBUpdate.CommandType = CommandType.Text;
                cmdBUpdate.CommandText = "UPDATE dbo.Groups SET [GroupIdStr] = @groupIdStr WHERE [GroupId] = @groupId";
                cmdBUpdate.Parameters.AddWithValue("@groupIdStr", groupIdStr);
                cmdBUpdate.Parameters.AddWithValue("@groupId", (int)groupId);
                cmdBUpdate.Connection = sqlBUpdateCon;

                sqlBUpdateCon.Open();
                cmdBUpdate.ExecuteNonQuery();
                sqlBUpdateCon.Close();
                // End Update GroupIdStr 

            }

            return CreatedAtRoute("DefaultApi", new { id = group.GroupId }, group);
        }


        // DELETE: api/Groups/5
        [ResponseType(typeof(Group))]
        public async Task<IHttpActionResult> DeleteGroup(int id)
        {
            Group group = await db.Groups.FindAsync(id);
            if (group == null)
            {
                return NotFound();
            }

            db.Groups.Remove(group);
            await db.SaveChangesAsync();

            return Ok(group);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool GroupExists(int id)
        {
            return db.Groups.Count(e => e.GroupId == id) > 0;
        }
    }
}