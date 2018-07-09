using MarielAPI.Helper;
using MarielAPI.Models.DB;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace MarielAPI.Utils.Helper
{
    public class DocumentHelper
    {
        public static async Task<List<sm_doc>> SaveUploadImage(marielEntities db, int tableID, int recordID, List<string> base64)
        {
            List<sm_doc> documents = new List<sm_doc>();
            if (base64 != null)
            {
                var i = 0;
                foreach (var image in base64)
                {
                    var document = new sm_doc();
                    using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(image)))
                    {
                        string pathForSavingToDB = "", imageNameForSavingToDB = "";
                        using (Bitmap bm = new Bitmap(ms))
                        {
                            string path = "";
                            string year = DateTime.Now.Year.ToString();
                            string month = DateTime.Now.Month > 9 ? DateTime.Now.Month.ToString() : "0" + DateTime.Now.Month;
                            path = ConstantHelper.UPLOAD_FOLDER + @"\" + year + @"\" + month;

                            path = HttpContext.Current.Server.MapPath(@"~\" + path);
                            if (!Directory.Exists(path))
                                Directory.CreateDirectory(path);

                            var createImageUniqueName = $"{tableID}_{recordID}_{DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss-fff")}_{i}.jpg"; //user "i" in order to prevent the same name
                            bm.Save(path + @"\" + createImageUniqueName);

                            
                            imageNameForSavingToDB = createImageUniqueName;
                            pathForSavingToDB = $"{ConstantHelper.UPLOAD_FOLDER}/{year}/{month}/{createImageUniqueName}";
                        }
                        document.name = imageNameForSavingToDB;
                        document.tableID = tableID;
                        document.createdDate = DateTime.Now;
                        document.value = recordID.ToString();
                        document.filePath = pathForSavingToDB;
                        db.sm_doc.Add(document);
                        await db.SaveChangesAsync();
                        documents.Add(document);
                    }
                    i++;
                }
            }
            return documents;
        }


        /*
        public static async Task<sm_doc> SaveUploadImage(marielEntities db, int tableID, int recordID, string base64)
        {
            var document = new sm_doc();
            using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(base64)))
            {
                string pathForSavingToDB = "", imageNameForSavingToDB = "";
                using (Bitmap bm = new Bitmap(ms))
                {
                    string path = "";
                    string year = DateTime.Now.Year.ToString();
                    string month = DateTime.Now.Month > 9 ? DateTime.Now.Month.ToString() : "0" + DateTime.Now.Month;
                    path = ConstantHelper.UPLOAD_FOLDER + @"\" + year + @"\" + month;

                    path = HttpContext.Current.Server.MapPath(@"~\" + path);
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);

                    Random random = new Random();
                    

                    var createImageUniqueName = $"{tableID}_{recordID}_{DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss-fff")}_{random.Next()}.jpg";
                    bm.Save(path + @"\" + createImageUniqueName);
                    imageNameForSavingToDB = createImageUniqueName;
                    pathForSavingToDB = $"{ConstantHelper.UPLOAD_FOLDER}/{year}/{month}/{createImageUniqueName}";
                }
                document.name = imageNameForSavingToDB;
                document.tableID = tableID;
                document.createdDate = DateTime.Now;
                document.value = recordID.ToString();
                document.filePath = pathForSavingToDB;
                db.sm_doc.Add(document);
                await db.SaveChangesAsync();
            }

            return document;
        }
        */
    }
}