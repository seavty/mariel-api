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

                    /*
                    await DocumentHelper.SaveUploadImage(db, ConstantHelper.document_ItemTableID, record.id, newDTO.idCardBase64);
                    await DocumentHelper.SaveUploadImage(db, ConstantHelper.document_ItemTableID, record.id, newDTO.employmentLetterBase64);
                    await DocumentHelper.SaveUploadImage(db, ConstantHelper.document_ItemTableID, record.id, newDTO.bankAccountBase64);
                    */
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
        }


        public async Task<tblLoanRequest> SaveToLoanRequest(LoanRequestBaseDTO loanRequestDTO, int accountID)
        {
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
        }
    }
}