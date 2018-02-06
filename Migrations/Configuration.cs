namespace PCBookWebApp.Migrations
{
    using PCBookWebApp.Models;
    using PCBookWebApp.Models.BookModule;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<PCBookWebApp.DAL.PCBookWebAppContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(PCBookWebApp.DAL.PCBookWebAppContext context)
        {
            context.Projects.AddOrUpdate(
                  p => p.ProjectName,
                  new Project
                  {
                      ProjectName = "Pakiza Collection",
                      Address = "House #97, Road 11/A, Dhanmondi, Dhaka",
                      Email = "info@pakizagroup.com",
                      Phone = "01755645752",
                      Website = "www.pakizagroup.com",
                      DateCreated = DateTime.Now,
                      DateUpdated = DateTime.Now,
                      CreatedBy = "admin@pakizagroup.com"
                  }
                );

            context.TransctionTypes.AddOrUpdate(
              p => p.TransctionTypeName,
              new TransctionType
              {
                  TransctionTypeName = "Dr",
                  DateCreated = DateTime.Now,
                  DateUpdated = DateTime.Now,
                  CreatedBy = "admin@pakizagroup.com"
              },
              new TransctionType
              {
                  TransctionTypeName = "Cr",
                  DateCreated = DateTime.Now,
                  DateUpdated = DateTime.Now,
                  CreatedBy = "admin@pakizagroup.com"
              }
            );
            context.Groups.AddOrUpdate(
              p => p.GroupName,
              new Group
              {
                  GroupName = "Primary",
                  PrimaryId = null,
                  ParentId = 1,
                  GroupIdStr = "1",
                  DateCreated = DateTime.Now,
                  DateUpdated = DateTime.Now,
                  CreatedBy = "admin@pakizagroup.com"
              }
            );
            context.Primaries.AddOrUpdate(
              p => p.PrimaryName,
                      new Primary
                      {
                          PrimaryName = "Assets",
                          DateCreated = DateTime.Now,
                          DateUpdated = DateTime.Now,
                          CreatedBy = "admin@pakizagroup.com"
                      },
                      new Primary
                      {
                          PrimaryName = "Liability",
                          DateCreated = DateTime.Now,
                          DateUpdated = DateTime.Now,
                          CreatedBy = "admin@pakizagroup.com"
                      }, new Primary
                      {
                          PrimaryName = "Income",
                          DateCreated = DateTime.Now,
                          DateUpdated = DateTime.Now,
                          CreatedBy = "admin@pakizagroup.com"
                      }, new Primary
                      {
                          PrimaryName = "Expencess",
                          DateCreated = DateTime.Now,
                          DateUpdated = DateTime.Now,
                          CreatedBy = "admin@pakizagroup.com"
                      });
            context.VoucherTypes.AddOrUpdate(
              p => p.VoucherTypeName,
              new VoucherType
              {
                  VoucherTypeName = "Payment",
                  DateCreated = DateTime.Now,
                  DateUpdated = DateTime.Now,
                  CreatedBy = "admin@pakizagroup.com"
              },
              new VoucherType
              {
                  VoucherTypeName = "Receive",
                  DateCreated = DateTime.Now,
                  DateUpdated = DateTime.Now,
                  CreatedBy = "admin@pakizagroup.com"
              }, new VoucherType
              {
                  VoucherTypeName = "Contra",
                  DateCreated = DateTime.Now,
                  DateUpdated = DateTime.Now,
                  CreatedBy = "admin@pakizagroup.com"
              }, new VoucherType
              {
                  VoucherTypeName = "Bank Payment",
                  DateCreated = DateTime.Now,
                  DateUpdated = DateTime.Now,
                  CreatedBy = "admin@pakizagroup.com"
              }, new VoucherType
              {
                  VoucherTypeName = "Bank Receive",
                  DateCreated = DateTime.Now,
                  DateUpdated = DateTime.Now,
                  CreatedBy = "admin@pakizagroup.com"
              }, new VoucherType
              {
                  VoucherTypeName = "Jounal",
                  DateCreated = DateTime.Now,
                  DateUpdated = DateTime.Now,
                  CreatedBy = "admin@pakizagroup.com"
              }
            );
        }
    }
}
