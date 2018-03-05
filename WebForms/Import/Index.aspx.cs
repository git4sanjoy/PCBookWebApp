using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PCBookWebApp.WebForms.Import
{
    public partial class Index : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            string connectString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=D:\\PFCHBSData.mdb;";

            OleDbConnection cn = new OleDbConnection(connectString);
            cn.Open();
            string selectString = "SELECT Delivery.Date, Delivery.ChalanNo, Delivery.CID FROM Delivery GROUP BY Delivery.Date, Delivery.ChalanNo, Delivery.CID";
            OleDbCommand cmd = new OleDbCommand(selectString, cn);
            OleDbDataReader reader = cmd.ExecuteReader();
            //Access DB Loop Through Thansction Table
            while (reader.Read())
            {
                DateTime mdate = (DateTime) reader["Date"];
                string challanNo = reader["ChalanNo"].ToString();
                //int cid = (int) reader["CID"];
                double totalFinalDiscount = 0;
                double totalGatOther = 0;
                int cid =Convert.ToInt32(reader["CID"]);
                //Open Delivery Table
                OleDbConnection cn1 = new OleDbConnection(connectString);
                cn1.Open();
                string sqlDelivery = "SELECT Delivery.CID, Delivery.Date, Delivery.ChalanNo, Sum(Delivery.FDiscount) AS SumOfFDiscount, Sum(Delivery.GatOther) AS SumOfGatOther FROM Delivery GROUP BY Delivery.CID, Delivery.Date, Delivery.ChalanNo HAVING (((Delivery.Date)=#" + mdate + "#) AND ((Delivery.ChalanNo)='" + challanNo + "') AND ((Delivery.CID)=" + cid + "))";
                //string sqlDelivery = "SELECT Delivery.Date, Delivery.CID, Delivery.ChalanNo, Delivery.YardPiece, Delivery.Rate, Delivery.DRate, Delivery.FDiscount, Delivery.GatOther FROM Delivery WHERE Delivery.ChalanNo='"+ challanNo + "' AND Delivery.Date=#"+ mdate + "#";

                OleDbCommand cmd1 = new OleDbCommand(sqlDelivery, cn1);
                cmd1.Parameters.AddWithValue("@memoDate", DbType.DateTime).Value = mdate;
                cmd1.Parameters.AddWithValue("@memoNo", DbType.DateTime).Value = challanNo;
                OleDbDataReader reader1 = cmd1.ExecuteReader();
                //Access DB Loop
                while (reader1.Read())
                {
                    cid = Convert.ToInt32(reader1["CID"]);
                    if (cid == 0) {
                        cid = 16252;
                    }
                    //double qu = Convert.ToDouble(reader1["YardPiece"]);
                    //double rate = Convert.ToDouble(reader1["Rate"]);
                    //double discountRate = Convert.ToDouble(reader1["DRate"]);
                    double finalDiscount = Convert.ToDouble(reader1["SumOfFDiscount"]);
                    double gatOther = Convert.ToDouble(reader1["SumOfGatOther"]);
                    totalFinalDiscount = totalFinalDiscount + Convert.ToDouble(reader1["SumOfFDiscount"]);
                    totalGatOther = totalGatOther+ Convert.ToDouble(reader1["SumOfGatOther"]); 

                }// Access Loop Delivery table
                reader1.Close();
                cn1.Close();

                // Proceed to save for Memo Master Table
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["PCBookWebAppContext"].ConnectionString))
                {
                    con.Open();
                    string sql = @"INSERT INTO dbo.MemoMasters (MemoDate, CustomerId, ShowRoomId, MemoNo, MemoDiscount, GatOther, Active, CreatedBy, DateCreated, DateUpdated) 
                                                      VALUES(@MemoDate, @CustomerId, @ShowRoomId, @MemoNo, @MemoDiscount, @GatOther, @active, @createdBy, @createDate, @updatedate)";
                    SqlCommand cmdInsert = new SqlCommand(sql, con);
                    cmdInsert.Parameters.Add("@MemoDate", SqlDbType.DateTime).Value = mdate;
                    cmdInsert.Parameters.Add("@CustomerId", SqlDbType.Int).Value = cid;
                    cmdInsert.Parameters.Add("@ShowRoomId", SqlDbType.Int).Value = 18;
                    cmdInsert.Parameters.Add("@MemoNo", SqlDbType.VarChar, 145).Value = challanNo;
                    
                    cmdInsert.Parameters.Add("@MemoDiscount", SqlDbType.Decimal).Value = totalFinalDiscount;
                    cmdInsert.Parameters.Add("@GatOther", SqlDbType.Decimal).Value = totalGatOther;

                    cmdInsert.Parameters.Add(new SqlParameter("@createdBy", "tsr.002.sr@pakizagroup.com"));
                    cmdInsert.Parameters.Add("@active", SqlDbType.Bit).Value = false;
                    cmdInsert.Parameters.Add("@createDate", SqlDbType.DateTime).Value = DateTime.Now;
                    cmdInsert.Parameters.Add("@updatedate", SqlDbType.DateTime).Value = DateTime.Now;

                    cmdInsert.CommandType = CommandType.Text;
                    cmdInsert.ExecuteNonQuery();
                    con.Close();

                }

            }// Access Loop Transction Table
            reader.Close();
            cn.Close();

            lblStatus.Text = "Data Imported Successfully";

        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            lblStatus.Text = "";
            string connectString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=D:\\PFCHBSData.mdb;";
            
            //Open SQL Server
            SqlConnection myConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["PCBookWebAppContext"].ConnectionString);
            try
            {
                myConnection.Open();
                SqlDataReader memoReader = null;
                SqlCommand command = new SqlCommand("SELECT * FROM MemoMasters where showroomid=18", myConnection);
                memoReader = command.ExecuteReader();

                while (memoReader.Read())
                {
                    DateTime memoDate = (DateTime) memoReader["MemoDate"];
                    string memoNo = (string) memoReader["MemoNo"];
                    int memoMasterId = (int) memoReader["MemoMasterId"];
                    int cid = (int) memoReader["CustomerId"];

                    if (cid == 16252)
                    {
                        cid = 0;

                    }
                    //int cid = (int) memoReader["CustomerId"];

                    OleDbConnection cn = new OleDbConnection(connectString);
                    cn.Open();
                    string selectString = "SELECT Delivery.CID, Delivery.Date, Delivery.ChalanNo, Delivery.FDiscount, Delivery.GatOther, Delivery.YardPiece, Delivery.Rate, Delivery.DRate, Delivery.SSCI FROM Delivery WHERE(((Delivery.Date) =#" + memoDate + "#) AND ((Delivery.ChalanNo)='" + memoNo + "') AND ((Delivery.CID)=" + cid + "))";
                    OleDbCommand cmd = new OleDbCommand(selectString, cn);
                    OleDbDataReader reader = cmd.ExecuteReader();
                    //Access DB Loop Through Thansction Table
                    double totalPrice = 0;
                    double totalFinalDiscount = 0;
                    double totalGatOther = 0;
                    double netPaymentCash = 0;
                    

                    while (reader.Read())
                    {
                        cid = Convert.ToInt32(reader["CID"]);

                        int productId = Convert.ToInt32(reader["SSCI"]);
                        double qu = Convert.ToDouble(reader["YardPiece"]);
                        double rate = Convert.ToDouble(reader["Rate"]);
                        double discountRate = Convert.ToDouble(reader["DRate"]);
                        double finalDiscount = Convert.ToDouble(reader["FDiscount"]);
                        double gatOther = Convert.ToDouble(reader["GatOther"]);
                        totalPrice = totalPrice + (qu * (rate - discountRate));

                        totalFinalDiscount = totalFinalDiscount + Convert.ToDouble(reader["FDiscount"]);
                        totalGatOther = totalGatOther + Convert.ToDouble(reader["GatOther"]);

                        // Proceed to save for Memo Master Details Table
                        using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["PCBookWebAppContext"].ConnectionString))
                        {
                            con.Open();
                            string sql = @"INSERT INTO dbo.MemoDetails (MemoMasterId, ProductId, Than, Quantity, Rate, Discount, Active, CreatedBy, DateCreated, DateUpdated) 
                                                                 VALUES(@MemoMasterId, @ProductId, @Than, @Quantity, @Rate, @Discount, @active, @createdBy, @createDate, @updatedate)";
                            SqlCommand cmdInsert = new SqlCommand(sql, con);

                            cmdInsert.Parameters.Add("@MemoMasterId", SqlDbType.Int).Value = memoMasterId;
                            cmdInsert.Parameters.Add("@ProductId", SqlDbType.Int).Value = productId;
                            cmdInsert.Parameters.Add("@Than", SqlDbType.Decimal).Value = 0;
                            cmdInsert.Parameters.Add("@Quantity", SqlDbType.Decimal).Value = qu;
                            cmdInsert.Parameters.Add("@Rate", SqlDbType.Decimal).Value = rate;
                            cmdInsert.Parameters.Add("@Discount", SqlDbType.Decimal).Value = discountRate;

                            cmdInsert.Parameters.Add("@active", SqlDbType.Bit).Value = false;
                            cmdInsert.Parameters.Add(new SqlParameter("@createdBy", "tsr.002.sr@pakizagroup.com"));
                            cmdInsert.Parameters.Add("@createDate", SqlDbType.DateTime).Value = DateTime.Now;
                            cmdInsert.Parameters.Add("@updatedate", SqlDbType.DateTime).Value = DateTime.Now;
                            cmdInsert.CommandType = CommandType.Text;
                            cmdInsert.ExecuteNonQuery();
                            con.Close();

                        }
                    }// Access Loop Transction Table
                    reader.Close();
                    cn.Close();
                    //Add cash payments
                    netPaymentCash = totalPrice + totalGatOther - totalFinalDiscount;
                    if (cid == 0)
                    {
                        cid = 16252;
                        // Proceed to save for Cash sale payments 
                        using (SqlConnection conPayment = new SqlConnection(ConfigurationManager.ConnectionStrings["PCBookWebAppContext"].ConnectionString))
                        {
                            conPayment.Open();
                            string sqlPayment = @"INSERT INTO dbo.Payments (MemoMasterId, CustomerId, ShowRoomId, PaymentDate, SSAmount,  SCAmount,  SDiscount,  PaymentType, Active, CreatedBy, DateCreated, DateUpdated, AdjustmentBf) 
                                                                        VALUES(@MemoMasterId, @CustomerId, @ShowRoomId, @PaymentDate,@SSAmount, @SCAmount,@SDiscount, @PaymentType, @active, @createdBy, @createDate, @updatedate,@AdjustmentBf)";
                            SqlCommand cmdInsertPayment = new SqlCommand(sqlPayment, conPayment);

                            cmdInsertPayment.Parameters.Add("@MemoMasterId", SqlDbType.Int).Value = memoMasterId;
                            cmdInsertPayment.Parameters.Add("@CustomerId", SqlDbType.Int).Value = cid;
                            cmdInsertPayment.Parameters.Add("@ShowRoomId", SqlDbType.Decimal).Value = 18;
                            cmdInsertPayment.Parameters.Add("@PaymentDate", SqlDbType.DateTime).Value = memoDate;

                            cmdInsertPayment.Parameters.Add("@SSAmount", SqlDbType.Decimal).Value = 0;
                            //cmdInsertPayment.Parameters.Add("@TSAmount", SqlDbType.Decimal).Value = 0;
                            cmdInsertPayment.Parameters.Add("@SCAmount", SqlDbType.Decimal).Value = netPaymentCash;
                            //cmdInsertPayment.Parameters.Add("@TCAmount", SqlDbType.Decimal).Value = 0;
                            cmdInsertPayment.Parameters.Add("@SDiscount", SqlDbType.Decimal).Value = 0;
                            //cmdInsertPayment.Parameters.Add("@TDiscount", SqlDbType.Decimal).Value = 0;
                            
                            cmdInsertPayment.Parameters.Add("@PaymentType", SqlDbType.VarChar, 145).Value = "Cash Party";
                            cmdInsertPayment.Parameters.Add("@active", SqlDbType.Bit).Value = false;
                            cmdInsertPayment.Parameters.Add(new SqlParameter("@createdBy", "tsr.002.sr@pakizagroup.com"));
                            cmdInsertPayment.Parameters.Add("@createDate", SqlDbType.DateTime).Value = DateTime.Now;
                            cmdInsertPayment.Parameters.Add("@updatedate", SqlDbType.DateTime).Value = DateTime.Now;
                            cmdInsertPayment.Parameters.Add("@AdjustmentBf", SqlDbType.Bit).Value = false;
                            cmdInsertPayment.CommandType = CommandType.Text;
                            cmdInsertPayment.ExecuteNonQuery();
                            conPayment.Close();

                        }
                    }
                }
                memoReader.Close();
                myConnection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }


            lblStatus.Text = "Data Imported Successfully";
        }

        protected void UpdateMemoNo_Click(object sender, EventArgs e)
        {
            string connectString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=D:\\PFCHBSData.mdb;";

            OleDbConnection cn = new OleDbConnection(connectString);
            cn.Open();
            string selectString = "SELECT * FROM Table1";
            OleDbCommand cmd = new OleDbCommand(selectString, cn);
            OleDbDataReader reader = cmd.ExecuteReader();
            //Access DB Loop Through Thansction Table
            while (reader.Read())
            {
                int sl = Convert.ToInt32( reader["sl"]);
                string challanNo = reader["ch"].ToString();
                string updateChallanNo = reader["updatech"].ToString();

                //Update Memo Table
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["PCBookWebAppContext"].ConnectionString);
                SqlCommand cmdUpdate = new SqlCommand();
                cmdUpdate.CommandType = System.Data.CommandType.Text;
                cmdUpdate.CommandText = "UPDATE dbo.MemoMasters SET [MemoNo] = @MemoNo WHERE [MemoMasterId] = @MemoMasterId";
                cmdUpdate.Parameters.AddWithValue("@MemoNo", updateChallanNo);
                cmdUpdate.Parameters.AddWithValue("@MemoMasterId", sl);
                cmdUpdate.Connection = con;

                con.Open();
                cmdUpdate.ExecuteNonQuery();
                con.Close();
                //End Balance Update


            }// Access Loop Transction Table
            reader.Close();
            cn.Close();

            lblStatus.Text = "Memo No Updated Successfully";
        }
    }
}