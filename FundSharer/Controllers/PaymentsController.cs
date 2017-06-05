using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using FundSharer.Models;
using Microsoft.AspNet.Identity;
using System.Web;
using FundSharer.DataServices;

namespace FundShare.Controllers
{
    [Authorize]
    public class PaymentsController : Controller
    {
        private ApplicationDbContext db = DbAccessHandler.DbContext;

        // GET: Payments
        public ActionResult Index()
        {
            return View(db.Payments.ToList());
        }

        // GET: Payments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Payment payment = db.Payments.Find(id);
            if (payment == null)
            {
                return HttpNotFound();
            }
            return View(payment);
        }

        // GET: Payments/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Payments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Amount,CreationDate,Confirmed")] Payment payment)
        {
            if (ModelState.IsValid)
            {
                db.Payments.Add(payment);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(payment);
        }

        // GET: Payments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Payment payment = db.Payments.Find(id);
            if (payment == null)
            {
                return HttpNotFound();
            }
            return View(payment);
        }

        // POST: Payments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Amount,CreationDate,Confirmed")] Payment payment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(payment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(payment);
        }

        // GET: Payments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Payment payment = db.Payments.Find(id);
            if (payment == null)
            {
                return HttpNotFound();
            }
            return View(payment);
        }

        // POST: Payments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Payment payment = db.Payments.Find(id);
            db.Payments.Remove(payment);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult UploadPaymentInfo(string Id)
        {
            Donation donation = DonationServices.GetDonationById(Id);
                if (donation != null)
                {
                    PaymentDetails details = new PaymentDetails
                    {
                        DonationId = donation.Id,
                        DonorBankName = donation.Donor.Bank,
                        DonorAccountNumber = donation.Donor.AccountNumber,
                        DonorFullName = donation.Donor.AccountTitle,
                        RecipientAccountNumber = donation.Ticket.TicketHolder.AccountNumber,
                        RecipientBankName = donation.Ticket.TicketHolder.Bank,
                        RecipientFullName = donation.Ticket.TicketHolder.AccountTitle
                    };
                    return View(details);
                }
            
            else
            {
                return HttpNotFound("We could not find the information you are looking for.");
            }

        }

        [HttpPost]
        public ActionResult UploadPaymentInfo(HttpPostedFileBase image)
        {
            var AppUser = new ApplicationUser() { Id = User.Identity.GetUserId() };
            BankAccount UserBankAccount = BankAccountServices.GetUserBankAccount(AppUser);

            var outgoingdonations = DonationServices.GetOutgoingAccountDonations(UserBankAccount).Where(m => m.IsOpen == false).ToList();
            Donation donation = null;
            if (outgoingdonations.Count() == 1)
            {
                donation = outgoingdonations.First();
            }

            Payment NewPay = new Payment
            {
                DonationPack = donation,
                CreationDate = DateTime.Now,
                Confirmed = false,
            };
            //Add a newly created payment for the uploaded file
            PaymentServices.AddPayment(NewPay);
            //Save it to the database
            //Create an image file associated with the payment from the HttpPostedFileBase object returned from the form
            POPImage PopImg = new POPImage();
            PopImg.Payment = NewPay;
            PopImg.Image = new byte[image.ContentLength];
            image.InputStream.Read(PopImg.Image, 0, image.ContentLength);
            //Add it to the images dataset
            PopImageServices.AddImage(PopImg);
            //add the donor account to the waiting list
            WaitingTicket ticket = new WaitingTicket { TicketHolder = donation.Donor, EntryDate = DateTime.Now };
            TicketServices.AddTicket(ticket);

            return RedirectToAction("HomePage", "Home");
        }

        //Get
        [ValidateAntiForgeryToken]
        public ActionResult ConfirmPayment(String Id)
        {
            Payment pay = PaymentServices.GetPaymentById(PaymentId);

            POPImage img = PopImageServices.GetPaymentPopImage(pay);
            ConfirmPaymentViewModel details = new ConfirmPaymentViewModel
            {
                PaymentId = pay.Id,
                Amount = pay.Amount,
                DonorName = pay.DonationPack.Donor.Owner.FullName,
                RecipientAccountNumber = pay.DonationPack.Ticket.TicketHolder.AccountNumber,
                Date = pay.CreationDate.ToShortDateString(),
                POPimage = img.Image
            };

            return View(details);
        }

        //POST
        [HttpPost]
        [ActionName("ConfirmPayment")]
        public ActionResult PaymentConfirmed(string Id)
        {
            var pay = PaymentServices.GetPaymentById(Id);
            //confirm the payment on the back end
            pay.Confirmed = true;
            PaymentServices.UpdatePayment(pay);
            //Open donation
            pay.DonationPack.IsOpen = true;
            DonationServices.UpdateDonations(pay.DonationPack);
            //make the donor account available to receive donations
            pay.DonationPack.Donor.IsReciever = true;
            //Update donor account on the backend
            BankAccountServices.UpdateBankAccount(pay.DonationPack.Donor);
            // create a waiting ticket for the donor account
            WaitingTicket donorTicket = new WaitingTicket { TicketHolder = pay.DonationPack.Donor, EntryDate = DateTime.Now, Donations = new System.Collections.Generic.List<Donation>() };
            //Add ticket to the back end
            TicketServices.AddTicket(donorTicket);

            //Check to see if the recipient ticket has no unopened packages
            if (pay.DonationPack.Ticket.Donations.Where(m => m.IsOpen == false).ToList().Count() == 0)// if not, make the ticket holder a reciever
            {
                pay.DonationPack.Ticket.TicketHolder.IsReciever = true;
                BankAccountServices.UpdateBankAccount(pay.DonationPack.Ticket.TicketHolder);
            }

            return RedirectToAction("HomePage", "Home");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
