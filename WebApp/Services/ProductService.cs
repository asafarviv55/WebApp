﻿using Microsoft.Extensions.Primitives;
using Org.BouncyCastle.Math;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.ConstrainedExecution;
using WebApp.Models;

namespace WebApp.Services
{
    public class ProductService
    {
        private const string ASCENDING_ORDER = "asc";
        private const string DESCENDING_ORDER = "desc";
        private static String connectionString = "Server=localhost,1433;Database=storedb;User ID=admin;Password=Wvyf3691!";

        public static void FileUpload(IFormCollection formData)
        {
            saveFileOnServer(formData);
            saveFilePathInDB(formData);
        }




        private static void saveFileOnServer(IFormCollection uploadedfile)
        {
            var files = uploadedfile.Files;
            try
            {
                foreach (var file in files)
                {
                    string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images");

                    if (file.Length > 0)
                    {
                        FileInfo fileInfo = new FileInfo(file.FileName);

                        string fileName = file.FileName;

                        using (var fileStream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
                        {
                            long size = file.Length;
                            if (Array.IndexOf(new string[] { ".jpeg", ".jpg", ".png" }, Path.GetExtension(file.FileName).ToString().ToLower()) < 0)
                            {
                                return;
                            }
                            file.CopyToAsync(fileStream);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("{0} Exception caught.", e);

                throw new Exception(e.Message, e);
            }
        }

        private static void saveFilePathInDB(IFormCollection formData)
        {
            //to get the connection string 
           // var connectionstring = "Server=localhost,1433;Database=storedb;User ID=admin;Password=Wvyf3691!";

            var files = formData.Files;

            foreach (var file in files)
            {
                string path = "Images/";

                FileInfo fileInfo = new FileInfo(file.FileName);

                string fileName = file.FileName;

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    try
                    {
                        conn.Open();

                        string commandtext = "savePathOfFile '" + path + fileName + "' , " + formData["id"].ToString();

                        Console.WriteLine(commandtext);

                        SqlCommand cmd = new SqlCommand(commandtext, conn);


                        cmd.ExecuteNonQuery();

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("{0} Exception caught.", e);

                        throw new Exception(e.Message, e);
                    }
                    finally
                    {
                        conn.Close();

                    }

                }
            }
        }




        public static void RecreateDB()
        {
            //to get the connection string 
         //   var connectionstring = "Server=localhost,1433;Database=storedb;User ID=admin;Password=Wvyf3691!";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string commandtext = "RecreateProducts";
                    SqlCommand cmd = new SqlCommand(commandtext, conn);
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Console.WriteLine("{0} Exception caught.", e);

                    throw new Exception(e.Message, e);
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        public static List<Product> InitialDB()
        {
            var products = new List<Product>();
            //to get the connection string 
         //   var connectionstring = "Server=localhost,1433;Database=storedb;User ID=admin;Password=Wvyf3691!";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    DataTable dt = new DataTable();
                    dt.Columns.Add(new DataColumn("code", typeof(int)));
                    dt.Columns.Add(new DataColumn("name", typeof(string)));
                    dt.Columns.Add(new DataColumn("description", typeof(string)));
                    dt.Columns.Add(new DataColumn("sell_date", typeof(DateTime)));

                    for (int i = 1; i <= 250; i++)
                    {
                        DataRow dr = dt.NewRow();
                        dr["code"] = i;
                        dr["name"] = i + " name";
                        dr["description"] = "product description " + i;
                        dr["sell_date"] = DateTime.Now;

                        dt.Rows.Add(dr);
                    }
                    //create object of SqlBulkCopy which help to insert  
                    SqlBulkCopy objbulk = new SqlBulkCopy(conn);


                    //assign Destination table name  
                    objbulk.DestinationTableName = "products";


                    objbulk.ColumnMappings.Add("code", "code");
                    objbulk.ColumnMappings.Add("name", "name");
                    objbulk.ColumnMappings.Add("description", "description");
                    objbulk.ColumnMappings.Add("sell_date", "sell_date");


                    //insert bulk Records into DataBase.  
                    objbulk.WriteToServer(dt);
                }
                catch (Exception e)
                {
                    Console.WriteLine("{0} Exception caught.", e);

                    throw new Exception(e.Message, e);
                }
                finally
                {
                    conn.Close();
                }
            }
            return products;
        }





        public static void AddNewProduct(int code, string name, string description, String imagePath)
        {
            var products = new List<Product>();
            //to get the connection string 
           // var connectionstring = "Server=localhost,1433;Database=storedb;User ID=admin;Password=Wvyf3691!";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string commandtext = "AddNewProduct " + code + " , '" + name + "' , '" + description + "' ,'" + DateTime.Now + "' , '" + imagePath + "' ";

                    SqlCommand cmd = new SqlCommand(commandtext, conn);

                    cmd.ExecuteNonQuery();

                }
                catch (Exception e)
                {
                    Console.WriteLine("{0} Exception caught.", e);

                    throw new Exception(e.Message, e);
                }
                finally
                {
                    conn.Close();
                }
            }

        }

        public static void UpdateProduct(int id, int code, string name, string description, string imagePath)
        {
            var products = new List<Product>();
            //to get the connection string 
           // var connectionstring = "Server=localhost,1433;Database=storedb;User ID=admin;Password=Wvyf3691!";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string commandtext = "UpdateProduct " + id + " , " + code + " , " + name + " , " + description + " , '" + DateTime.Now + "' , " + imagePath + " ";

                    Console.WriteLine(commandtext);

                    SqlCommand cmd = new SqlCommand(commandtext, conn);

                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Console.WriteLine("{0} Exception caught.", e);

                    throw new Exception(e.Message, e);
                }
                finally
                {
                    conn.Close();
                }
            }
        }


        public static AProducts GetAllProducts()
        {
            var aproducts = new AProducts();
            var products = new List<Product>();
            //to get the connection string 
           // var connectionstring = "Server=localhost,1433;Database=storedb;User ID=admin;Password=Wvyf3691!";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string commandtext = "GetAllProducts";

                    SqlCommand cmd = new SqlCommand(commandtext, conn);

                    var reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        var product = new Product()
                        {
                            id = Convert.ToInt32(reader["id"]),
                            code = Convert.ToInt32(reader["code"]),
                            name = reader["name"].ToString(),
                            description = reader["description"].ToString(),
                            sell_date = reader["sell_date"].ToString(),
                            imagePath = reader["path"].ToString()
                        };
                        products.Add(product);
                    }

                    aproducts.products = products;

                    reader.NextResult();

                    while (reader.Read())
                    {
                        aproducts.total = (int)reader["totalRows"];
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("{0} Exception caught.", e);
                    throw new Exception(e.Message, e);
                }
                finally
                {
                    conn.Close();
                }
            }
            return aproducts;
        }



        public static List<Product> Paging(int offset, int rowsPerPage)
        {
            var products = new List<Product>();
            //to get the connection string 
         //   var connectionstring = "Server=localhost,1433;Database=storedb;User ID=admin;Password=Wvyf3691!";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    string commandtext = "Paging " + offset + " , " + rowsPerPage;

                    SqlCommand cmd = new SqlCommand(commandtext, conn);

                    var reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        var product = new Product()
                        {
                            id = Convert.ToInt32(reader["id"]),
                            code = Convert.ToInt32(reader["code"]),
                            name = reader["name"].ToString(),
                            description = reader["description"].ToString(),
                            sell_date = reader["sell_date"].ToString(),
                            imagePath = reader["path"].ToString()
                        };
                        products.Add(product);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("{0} Exception caught.", e);

                    throw new Exception(e.Message, e);
                }
                finally
                {
                    conn.Close();
                }
            }
            return products;
        }






        public static void DeleteProducts(List<int> ids)
        {
            var products = new List<Product>();
            //to get the connection string 
           // var connectionstring = "Server=localhost,1433;Database=storedb;User ID=admin;Password=Wvyf3691!";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    var dt = new DataTable();

                    dt.Columns.Add("Id", typeof(int));

                    foreach (var id in ids)
                        dt.Rows.Add(id);

                    var delete = new SqlCommand("DeleteMultipleProducts ", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    delete.Parameters.AddWithValue("@IDs", dt).SqlDbType = SqlDbType.Structured;

                    delete.ExecuteNonQuery();

                }
                catch (Exception e)
                {
                    Console.WriteLine("{0} Exception caught.", e);

                    throw new Exception(e.Message, e);
                }
                finally
                {
                    conn.Close();
                }

            }

        }

        public static List<Product> GetAllProductsOrderBy(int orderCol, int orderDirection)
        {
            var products = new List<Product>();
           // var connectionstring = "Server=localhost,1433;Database=storedb;User ID=admin;Password=Wvyf3691!";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string commandtext = "";
                    commandtext = "GetProductsOrderByColumn " + "'" + getColumnNameByNumber(orderCol) + "' , '" + getOrderDirectionByNumber(orderDirection) + "' ";



                    SqlCommand cmd = new SqlCommand(commandtext, conn);
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        var product = new Product()
                        {
                            id = Convert.ToInt32(reader["id"]),
                            code = Convert.ToInt32(reader["code"]),
                            name = reader["name"].ToString(),
                            description = reader["description"].ToString(),
                            sell_date = reader["sell_date"].ToString(),
                            imagePath = reader["path"].ToString()
                        };
                        products.Add(product);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("{0} Exception caught.", e);

                    throw new Exception(e.Message, e);
                }
                finally
                {
                    conn.Close();
                }
            }
            return products;
        }

        public static List<Product> searchProduct(string searchStr)
        {
            var products = new List<Product>();
            //to get the connection string 
      //      var connectionstring = "Server=localhost,1433;Database=storedb;User ID=admin;Password=Wvyf3691!";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    using (SqlCommand cmd = new SqlCommand("SearchByNameOrCode", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@str", searchStr);
                        conn.Open();
                        var reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            var product = new Product()
                            {
                                id = Convert.ToInt32(reader["id"]),
                                code = Convert.ToInt32(reader["code"]),
                                name = reader["name"].ToString(),
                                description = reader["description"].ToString(),
                                sell_date = reader["sell_date"].ToString(),
                                imagePath = reader["path"].ToString()
                            };
                            products.Add(product);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("{0} Exception caught.", e);

                    throw new Exception(e.Message, e);
                }
                finally
                {
                    conn.Close();
                }
            }
            return products;
        }

        private static string getOrderDirectionByNumber(int col)
        {
            return col == 0 ? ASCENDING_ORDER : DESCENDING_ORDER;
        }

        private static string getColumnNameByNumber(int col)
        {
            switch (col)
            {
                case 1:
                    return "code";
                case 2:
                    return "name";
                case 3:
                    return "description";
                default:
                    return "sell_date";
            }
        }


    }
}
