using MarielAPI.Helper;
using MarielAPI.Models.DB;
using MarielAPI.Models.DTO.LoanRequest;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using X_Admin_API.Helper;

namespace MarielAPI.Utils.Handler
{
    public class LoanRequestHandler
    {
        private marielEntities db = null;

        public LoanRequestHandler()
        {
            db = new marielEntities();
        }

        //-> SelectByID
        public async Task<LoanRequestViewDTO> SelectByID(int id)
        {
            var record = await db.tblLoanRequests.FirstOrDefaultAsync(x => x.deleted == null && x.id == id);
            if (record == null)
                throw new HttpException((int)HttpStatusCode.NotFound, "NotFound");
            return MappingHelper.MapDBClassToDTO<tblLoanRequest, LoanRequestViewDTO>(record);
        }

        /*
        //-> Create
        public async Task<LoanRequestViewDTO> Create(LoanRequestNewDTO newDTO)
        {
            var record = await db.tblAccounts.FirstOrDefaultAsync(x => x.deleted == null && x.id == newDTO.accountID);
            if (record == null)
                throw new HttpException((int)HttpStatusCode.BadRequest, "This account has been deleted or does not exsit");

            if (record.status == "Pending")
                throw new HttpException((int)HttpStatusCode.BadRequest, ConstantHelper.ALREADY_REQUEST_LOAN);

            newDTO = StringHelper.TrimStringProperties(newDTO);
            var loanRequest = await new AccountHandler().SaveToLoanRequest(newDTO, newDTO.accountID);
            return await SelectByID(loanRequest.id);
        }
        */

        public async Task<LoanRequestViewDTO> CreateLoanRequest(LoanRequestNewDTO loanRequestDTO)
        {
            var account = await db.tblAccounts.FirstOrDefaultAsync(a => a.deleted == null && a.id == loanRequestDTO.accountID);
            if (account == null)
                throw new HttpException((int)HttpStatusCode.NotFound, "NotFound");

            var loan = db.tblLoanRequests.FirstOrDefault(x => x.accountID == loanRequestDTO.accountID &&
            x.status != "rejected" && x.status != "approve" && x.loan_Balance > 0);
            if (loan != null)
                throw new HttpException((int)HttpStatusCode.BadRequest, ConstantHelper.ALREADY_REQUEST_LOAN);

            var tblPin = db.tblPins.FirstOrDefault(x => x.accountID == account.id &&
            x.isUsed == null && x.name == loanRequestDTO.pin);
            if (tblPin == null)
            {
                throw new HttpException((int)HttpStatusCode.BadRequest, ConstantHelper.INVALID_PIN);
            }
            else
            {
                if (DateTime.Now.Subtract(tblPin.date.Value).Minutes > 3)
                {
                    throw new HttpException((int)HttpStatusCode.BadRequest, ConstantHelper.PIN_EXPIRED);
                }
            }

            tblPin.isUsed = "Y";

            loanRequestDTO = StringHelper.TrimStringProperties(loanRequestDTO);
            var loanRequest = await SaveToLoanRequest(
                loanRequestDTO.payday,
                loanRequestDTO.amount,
                loanRequestDTO.accountID,
                loanRequestDTO.purpose);
            return await SelectByID(int.Parse(loanRequest.id.ToString()));

        }

        /// private function 
        //-> 
        private async Task<tblLoanRequest> SaveToLoanRequest(int payDay, double amount, int accountID, string purpose)
        {
            var loanRequest = new tblLoanRequest();
            loanRequest.createdDate = DateTime.Now;
            //loanRequest.payDate = DateTime.Now;

            var today = DateTime.Now;
            var payDate = DateTime.Parse(DateTime.Now.AddDays(payDay).ToString("yyyy-MM-dd HH':'mm':'ss"));

            loanRequest.payDate = payDate;


            var interestRate = 0;
            switch (payDay)
            {
                case 10:
                    interestRate = 10;
                    break;
                case 15:
                    interestRate = 15;
                    break;
                case 30:
                    interestRate = 30;
                    break;
                default:
                    interestRate = 0;
                    break;
            }
            loanRequest.accountID = accountID;
            loanRequest.payDay = payDay;
            loanRequest.amount = Decimal.Parse(amount.ToString());
            loanRequest.interestRate = interestRate;
            loanRequest.interestAmount = Decimal.Parse((amount * interestRate / 100).ToString());
            loanRequest.loanAmount = Decimal.Parse((Decimal.Parse(amount.ToString()) + loanRequest.interestAmount).ToString());
            loanRequest.purpose = purpose;
            loanRequest.status = "Pending";
            loanRequest.loan_Balance = loanRequest.loanAmount;
            loanRequest.loan_PaidAmount = 0;
            db.tblLoanRequests.Add(loanRequest);
            await db.SaveChangesAsync();
            return loanRequest;
        }
    }
}