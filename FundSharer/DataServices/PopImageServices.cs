using FundSharer.Models;
using System;
using System.Collections.Generic;
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
                    using (ApplicationDbContext db = new ApplicationDbContext())
                    {
                        db.POPImages.Add(NewImg);
                        db.SaveChanges();
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
                    using (ApplicationDbContext db = new ApplicationDbContext())
                    {
                        db.POPImages.Remove(DeleteImg);
                        db.SaveChanges();
                    }
                }
            }
        }

        public static void UpdateImage(POPImage UpdateImg)
        {
            if (IsNotNull(UpdateImg))
            {
                if (ExistInRecord(UpdateImg))
                {
                    using (ApplicationDbContext db = new ApplicationDbContext())
                    {
                        db.Entry(UpdateImg).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            }
        }

        public static POPImage GetPopImageById(string ImageId)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                return db.POPImages.Find(ImageId);
            }
        }

        public static POPImage GetPaymentPopImage(Payment pay)
        {
            POPImage img = null;

            if (PaymentServices.IsNotNull(pay))
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    var imgs = (from i in db.POPImages where i.PaymentId == pay.Id select i).ToList();
                    if (imgs.Count() > 0)
                    {
                        img = imgs.First();
                    }
                }
            }
            return img;
        }

        public static List<POPImage> GetPopImages()
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                return db.POPImages.ToList();
            }
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
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var test = db.POPImages.Find(image.Id);
                return IsNotNull(test);
            }

        }
        #endregion


    }
}