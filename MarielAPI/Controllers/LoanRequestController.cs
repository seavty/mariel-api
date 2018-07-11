using MarielAPI.Helper;
using MarielAPI.Models.DTO.LoanRequest;
using MarielAPI.Utils.Handler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

namespace MarielAPI.Controllers
{
    public class LoanRequestController : ApiController
    {
        private LoanRequestHandler handler = null;

        public LoanRequestController()
        {
            handler = new LoanRequestHandler();
        }

        //-> Create
        [HttpPost]
        [Route(ConstantHelper.loanReuqestEndPoint)]
        [ResponseType(typeof(LoanRequestViewDTO))]
        public async Task<IHttpActionResult> Create(LoanRequestNewDTO newDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                return Ok(await handler.CreateLoanRequest(newDTO));
            }
            catch (HttpException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
