using Megatech.NAFSC.DataExport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Megatech.FMS.WebAPI.Controllers
{
    public class ExportController : ApiController
    {
        [Route("api/export/{id?}")]
        public IHttpActionResult Post(int? id = null)
        {
            var exp =             new Exporter();
            if (id != null )
            {
                var ret = exp.Export((int)id);
                return Ok(ret);
            }
            else
            {

                var ret = exp.Export();
                return Ok(ret);
            }
        }
    }
}
