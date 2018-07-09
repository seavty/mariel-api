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
    }
}