using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using RestSharp;
using RestSharp.Authenticators;
using Newtonsoft.Json;
using System.Text.Json;
using System.Data;
using Microsoft.Extensions.Configuration;
using DotnetWebApiWithEFCodeFirst;
using RestApiSO.Models;
using System.Reflection.Emit;
using System.Collections.Generic;
using System.Transactions;
using System;

namespace RestApiSO
{
    public class DBContext : DbContext
    {
        static HttpClient client = new HttpClient();
        static SqlConnection sqlConnection = new SqlConnection();

        public DbSet<CollectiveExternalLink> CollectiveExternalLink { get; set; }
        public DbSet<Collective> Collective { get; set; }
        public DbSet<Tag> Tag { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity< LinkedList<Tag>>()
              // String.Join(String.Empty, Synonyms.ToArray());

            modelBuilder.Entity<Collective>()
                  .Property(p => p.Tags)
                  .HasConversion(
                  v => string.Join(',', v),
                  v => v.Split(',', StringSplitOptions.RemoveEmptyEntries));
        }



        public static async Task<Item> GetTags()
        {
            var options = new RestClientOptions("https://api.stackexchange.com")
            {
                Authenticator = new HttpBasicAuthenticator("key", "9Ke4vzAg4XJj45QL8N746Q((")
            };
            var client = new RestClient(options);
            var request = new RestRequest("/2.3/tags");
            request.AddParameter("order", "desc");
            request.AddParameter("sort", "popular");
            request.AddParameter("site", "stackoverflow");
            var response = await client.GetAsync(request);
            Item item = Newtonsoft.Json.JsonConvert.DeserializeObject<Item>(response.Content);
            


            DBContext c = new DBContext();
            c.CreateDatabase();
            c.SetTagsId(item);
            c.SaveToDatabase(item);


            return null;
        }


        public Item SetTagsId(Item item)
        {
            int collectiveExternalLinkID = 1;
            int collectiveID = 1;
            int tagID = 1;
            

            foreach(Tag tag in item.items)
            {
                tag.TagId = tagID;
                tagID++;
                if(tag.Collectives != null)
                {
                    foreach (Collective collective in tag.Collectives)
                    {
                        collective.CollectiveId = collectiveID;
                        collectiveID++;

                        if (collective.ExternalLinks != null)
                        {
                            foreach (CollectiveExternalLink collectiveExternalLink in collective.ExternalLinks)
                            {
                                collectiveExternalLink.CollectiveExternalLinkId = collectiveExternalLinkID;
                                collectiveExternalLinkID++;
                            }
                        }
                    }
                }
            }
            return item;
        }

        public void SaveToDatabase(Item item)
        {
            List<Tag> tags = item.items.ToList();

            try
            {
                using (var conn = new SqlConnection(Program.connectionString))
                {
                    conn.Open();
                    foreach (Tag tag in item.items)
                    {
                        // Tag
                        SqlCommand cmdTag = new SqlCommand("InsertTag", conn);
                        cmdTag.CommandType = CommandType.StoredProcedure;
                        cmdTag.Parameters.Add(new SqlParameter("Count", tag.Count));
                        cmdTag.Parameters.Add(new SqlParameter("HasSynonyms", tag.HasSynonyms));
                        cmdTag.Parameters.Add(new SqlParameter("IsModeratorOnly", tag.IsModeratorOnly));
                        cmdTag.Parameters.Add(new SqlParameter("IsRequired", tag.IsRequired));
                        if (tag.LastActivityDate != DateTime.MinValue)
                        {
                            cmdTag.Parameters.Add(new SqlParameter("LastActivityDate", tag.LastActivityDate.ToString("dd mon yyyy hh:mi:ss")));
                        }
                        else
                        {
                            cmdTag.Parameters.Add(new SqlParameter("LastActivityDate", DBNull.Value));
                        }
                        cmdTag.Parameters.Add(new SqlParameter("Name", tag.Name));
                        cmdTag.Parameters.Add(new SqlParameter("UserId", tag.UserId));
                        cmdTag.ExecuteNonQuery();


                        // Collective
                        if(tag.Collectives != null)
                        {
                            foreach (Collective collective in tag.Collectives)
                            {
                                SqlCommand cmdCol = new SqlCommand("InsertCollective", conn);
                                cmdCol.CommandType = CommandType.StoredProcedure;
                                cmdCol.Parameters.Add(new SqlParameter("Description", collective.Description));
                                cmdCol.Parameters.Add(new SqlParameter("TagFK", tag.TagId));
                                cmdCol.Parameters.Add(new SqlParameter("Link", collective.Link));
                                cmdCol.Parameters.Add(new SqlParameter("Name", collective.Name));
                                cmdCol.Parameters.Add(new SqlParameter("Slug", collective.Slug));
                                cmdCol.Parameters.Add(new SqlParameter("Tags", string.Join(";", collective.Tags)));
                                cmdCol.ExecuteNonQuery();

                                // CollectiveExternalLink
                                foreach (CollectiveExternalLink cek in collective.ExternalLinks)
                                {
                                    SqlCommand cmdExt = new SqlCommand("InsertCollectiveExternalLink", conn);
                                    cmdExt.CommandType = CommandType.StoredProcedure;
                                    cmdExt.Parameters.Add(new SqlParameter("CollectiveFK", collective.CollectiveId));
                                    cmdExt.Parameters.Add(new SqlParameter("Link", cek.Link));
                                    cmdExt.Parameters.Add(new SqlParameter("Type", cek.Type));
                                    cmdExt.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                    conn.Close();
                }
            }
            catch(Exception)
            {
                throw;
            }
        }

        public void CreateDatabase()
        {
            using (var conn = new SqlConnection(Program.connectionString))
            try
            {
                using (var command = new SqlCommand("createTagTables", conn)
                {
                    CommandType = CommandType.StoredProcedure
                })
                {
                    conn.Open();
                    command.ExecuteNonQuery();
                    conn.Close();
                }
            }
            catch(Exception ex)
            {
                    throw;
             }
        }

        public DataTable GetAllTags()
        {
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            using (var conn = new SqlConnection(Program.connectionString))
            {
                try
                {
                    using (SqlDataAdapter da = new SqlDataAdapter())
                    {
                        da.SelectCommand = new SqlCommand("GetAllTags", conn);
                        da.SelectCommand.CommandType = CommandType.StoredProcedure;
                        da.Fill(dt);
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
            return dt;
        }
    }
}
