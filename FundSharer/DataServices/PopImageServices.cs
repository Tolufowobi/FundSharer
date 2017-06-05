using FundSharer.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace FundSharer.DataServices
{
    public class PopImageServices
    {

        public static void AddImage(POPImage NewImg)
        {
            if (IsNotNull(NewImg))
            {
                if (!ExistInRecord(NewImg))
                {
                    if(PaymentServices.IsNotNull(NewImg.Payment))
                    {
                        DbAccessHandler.DbContext.Entry(NewImg.Payment).State = EntityState.Unchanged;
                        DbAccessHandler.DbContext.POPImages.Add(NewImg);
                        DbAccessHandler.DbContext.SaveChanges();
                        
                    }
                }
            }
        }

        public static void RemoveImage(POPImage DeleteImg)
        {
            if (IsNotNull(DeleteImg))
            {
                if (ExistInRecord(DeleteImg))
                {
                    DbAccessHandler.DbContext.POPImages.Remove(DeleteImg);
                    DbAccessHandler.DbContext.SaveChanges();
                }
            }
        }

        public static void UpdateImage(POPImage UpdateImg)
        {
            if (IsNotNull(UpdateImg))
            {
                if (ExistInRecord(UpdateImg))
                {
                    DbAccessHandler.DbContext.Entry(UpdateImg).State = EntityState.Modified;
                    DbAccessHandler.DbContext.SaveChanges();
                }
            }
        }

        public static POPImage GetPopImageById(string ImageId)
        {
                return DbAccessHandler.DbContext.POPImages.Find(ImageId);
        }

        public static POPImage GetPaymentPopImage(Payment pay)
        {
            POPImage img = null;

            if (PaymentServices.IsNotNull(pay))
            {
                    var imgs = (from i in DbAccessHandler.DbContext.POPImages where i.PaymentId == pay.Id select i).ToList();
                    if (imgs.Count() > 0)
                    {
                        img = imgs.First();
                    }
                
            }
            return img;
        }

        public static List<POPImage> GetPopImages()
        {
                return DbAccessHandler.DbContext.POPImages.ToList();
            
        }

        #region Helpers
        public static bool IsNotNull(POPImage image)
        {
            if (image != null)
            { return true; }
            else
            { return false; }
        }

        public static bool ExistInRecord(POPImage image)
        {
            
                var test = DbAccessHandler.DbContext.POPImages.Find(image.Id);
                return IsNotNull(test);

        }
        #endregion


    }
}