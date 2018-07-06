using MarielAPI.Helper;
using MarielAPI.Models.DTO.Account;
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
    public class AccountController : ApiController
    {
        private AccountHandler handler = null;

        public AccountController()
        {
            handler = new AccountHandler();
        }

        //-> CheckPhoneNumber
        [HttpGet]
        [Route(ConstantHelper.accountEndPoint)]
        [ResponseType(typeof(AccountViewDTO))]
        public async Task<IHttpActionResult> CheckPhoneNumber([FromUri] string phoneNumber)
        {
            var record = await handler.CheckPhoneNumber(phoneNumber);
            if (record == null)
                return NotFound();
            else
                return Ok(record);
        }

        //-> Create
        [HttpPost]
        [Route(ConstantHelper.accountEndPoint)]
        [ResponseType(typeof(AccountViewDTO))]
        public async Task<IHttpActionResult> Create(AccountNewDTO newDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                return Ok(await handler.Create(newDTO));
            }
            catch (HttpException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
