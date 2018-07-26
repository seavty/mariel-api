using MarielAPI.Helper;
using MarielAPI.Models.DB;
using MarielAPI.Models.DTO.Account;
using MarielAPI.Models.DTO.LoanRequest;
using MarielAPI.Utils.Helper;
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
    public class AccountHandler
    {
        private marielEntities db = null;

        public AccountHandler()
        {
            db = new marielEntities();
        }

        //-> SelectByID
        public async Task<AccountViewDTO> SelectByID(int id)
        {
            var record = await db.tblAccounts.FirstOrDefaultAsync(x => x.deleted == null && x.id == id);
            if (record == null)
                throw new HttpException((int)HttpStatusCode.NotFound, "NotFound");
            return MappingHelper.MapDBClassToDTO<tblAccount, AccountViewDTO>(record);
        }

        //-> CheckPhoneNumber
        public async Task<AccountViewDTO> CheckPhoneNumber(string phoneNumber)
        {
            var record = await db.tblAccounts.FirstOrDefaultAsync(x => x.deleted == null && x.phoneNumber == phoneNumber);
            if (record == null)
                return null;
            return await SelectByID(record.id);
        }

        //-> Create
        public async Task<AccountViewDTO> Create(AccountNewDTO newDTO)
        {
            /*
            using (var transaction = db.Database.BeginTransaction())
            {
                var checkRecord = await db.tblAccounts.FirstOrDefaultAsync(x => x.deleted == null && x.status == "Pending" && x.phoneNumber == newDTO.phoneNumber); // check whether itemgroup name exist or not
                if (checkRecord != null)
                    throw new HttpException((int)HttpStatusCode.BadRequest, ConstantHelper.ALREADY_REQUEST_LOAN);

                try
                {
                    newDTO = StringHelper.TrimStringProperties(newDTO);
                    var record = (tblAccount)MappingHelper.MapDTOToDBClass<AccountNewDTO, tblAccount>(newDTO, new tblAccount());
                    record.createdDate = DateTime.Now;
                    record.status = "Pending";
                    db.tblAccounts.Add(record);
                    await db.SaveChangesAsync();

                    var base64s = new List<string>();
                    base64s.Add(newDTO.idCardBase64);
                    base64s.Add(newDTO.employmentLetterBase64);
                    base64s.Add(newDTO.bankAccountBase64);

                    var doc = await DocumentHelper.SaveUploadImage(db, ConstantHelper.document_ItemTableID, record.id, base64s);
                    
                    await SaveToLoanRequest(newDTO.loanRequest, record.id);

                    transaction.Commit();
                    return await SelectByID(record.id);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception(ex.Message);
                }
            }
            */
            using (var transaction = db.Database.BeginTransaction())
            {
                var tblPin = db.tblPins.FirstOrDefault(x => x.phoneNumber == newDTO.phoneNumber &&
                    x.isUsed == null && x.name == newDTO.pin);
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
                try
                {
                    /*
                    var tblPin = db.tblPins.FirstOrDefault(x => x.phoneNumber == newDTO.phoneNumber &&
                    x.isUsed == null && x.name == newDTO.pin);
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
                    */
                    tblPin.isUsed = "Y";

                    newDTO = StringHelper.TrimStringProperties(newDTO);
                    var account = (tblAccount)MappingHelper.MapDTOToDBClass<AccountNewDTO, tblAccount>(newDTO, new tblAccount());
                    account.createdDate = DateTime.Now;
                    account.status = "Pending";

                    db.tblAccounts.Add(account);
                    await db.SaveChangesAsync();
                    var base64s = new List<string>();
                    base64s.Add(newDTO.idCardBase64);
                    base64s.Add(newDTO.employmentLetterBase64);
                    base64s.Add(newDTO.bankAccountBase64);

                    var doc = await DocumentHelper.SaveUploadImage(db, ConstantHelper.TABLE_ACCOUNT_ID, account.id, base64s);
                    await SaveToLoanRequest(newDTO.loanRequest, account.id);
                    transaction.Commit();
                    return await SelectByID(account.id);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception(ex.Message);
                }
            }

        }

        
        public async Task<tblLoanRequest> SaveToLoanRequest(LoanRequestBaseDTO loanRequestDTO, int accountID)
        {
            /*
            var loanRequest = new tblLoanRequest();
            //db.tblLoanRequests.Add(loanRequest);
            loanRequest.createdDate = DateTime.Now;
            loanRequest.payDate = DateTime.Now;
            var interestRate = 0;
            switch (loanRequestDTO.payday)
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
            loanRequest.payDay = loanRequestDTO.payday;
            loanRequest.amount = Decimal.Parse(loanRequestDTO.amount.ToString());
            loanRequest.interestRate = interestRate;
            loanRequest.interestAmount = Decimal.Parse((loanRequestDTO.amount * interestRate / 100).ToString());
            loanRequest.loanAmount = Decimal.Parse((Decimal.Parse(loanRequestDTO.amount.ToString()) + loanRequest.interestAmount).ToString());
            db.tblLoanRequests.Add(loanRequest);
            await db.SaveChangesAsync();
            return loanRequest;
            */

            var loanRequest = new tblLoanRequest();
            //db.tblLoanRequests.Add(loanRequest);
            loanRequest.createdDate = DateTime.Now;

            var today = DateTime.Now;
            var payDate = DateTime.Parse(DateTime.Now.AddDays(loanRequestDTO.payday).ToString("yyyy-MM-dd HH':'mm':'ss"));

            loanRequest.payDate = payDate;
            var interestRate = 0;
            switch (loanRequestDTO.payday)
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
            loanRequest.payDay = loanRequestDTO.payday;
            loanRequest.loanAmount = Decimal.Parse(loanRequestDTO.amount.ToString());
            loanRequest.interestRate = interestRate;
            loanRequest.interestAmount = Decimal.Parse((loanRequestDTO.amount * interestRate / 100).ToString());
            loanRequest.loanAmount = Decimal.Parse((Decimal.Parse(loanRequestDTO.amount.ToString()) + loanRequest.interestAmount).ToString());
            loanRequest.purpose = loanRequestDTO.purpose;
            loanRequest.status = "Pending";
            loanRequest.loan_Balance = loanRequest.loanAmount;
            loanRequest.loan_PaidAmount = 0;
            db.tblLoanRequests.Add(loanRequest);
            await db.SaveChangesAsync();
            return loanRequest;

        }
        

        //----- *** ---//
        // Create PIN
        public async Task<bool> createPin(int id, string phone)
        {
            bool re = false;
            if (id != 0)
            {
                var account = await db.tblAccounts.FirstOrDefaultAsync(a => a.deleted == null && a.id == id);
                if (account == null)
                    throw new HttpException((int)HttpStatusCode.NotFound, "NotFound");

                if (account.phoneNumber != phone)
                    throw new HttpException((int)HttpStatusCode.BadRequest, ConstantHelper.INVALID_PHONE);
            }

            var loan = db.tblLoanRequests.FirstOrDefault(x => x.id == id &&
                x.status != "rejected" && x.status != "approve" && x.loan_Balance > 0);
            if (loan != null)
                throw new HttpException((int)HttpStatusCode.BadRequest, ConstantHelper.ALREADY_REQUEST_LOAN);

            //var ePin = db.tblPins.Where(x => x.deleted == null).Take(2).OrderByDescending(o => o.date);
            var ePin = db.tblPins.Where(x => x.deleted == null &&
                id != 0 ? x.accountID == id : x.phoneNumber == phone
            ).OrderByDescending(o => o.date).Take(2);
            if (ePin != null)
            {
                DateTime dt1 = DateTime.Now;
                DateTime dt2 = DateTime.Now;
                int i = 0;
                foreach (var _pin in ePin)
                {
                    if (i == 0)
                    {
                        dt1 = _pin.date.Value;
                        var tt = DateTime.Now.Subtract(dt1).Minutes + (DateTime.Now.Subtract(dt1).Hours * 60);
                        if (tt > 30)
                            break;
                    }
                    if (i == 1)
                    {
                        dt2 = _pin.date.Value;
                    }
                    i++;
                }
                if (i > 1)
                    if (dt1.Subtract(dt2).Minutes + (dt1.Subtract(dt1).Hours * 60) < 30)
                    {
                        throw new HttpException((int)HttpStatusCode.BadRequest, ConstantHelper.PENDING_SMS);
                    }
            }
            tblPin pin = new tblPin();
            pin.date = DateTime.Now;
            if (id != 0)
                pin.accountID = id;
            else
                pin.phoneNumber = phone;
            pin.name = GeneratePIN();
            db.tblPins.Add(pin);
            db.SaveChanges();
            re = true;
            return re;

        }

        private string GeneratePIN()
        {
            Random _random = new Random();
            return _random.Next(0, 9999).ToString("D4");
        }
    }
}