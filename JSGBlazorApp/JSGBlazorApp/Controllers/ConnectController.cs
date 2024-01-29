using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JSGBlazorApp.Controllers
{
    [Route("api/connect")]
    [ApiController]
    public class ConnectController : ControllerBase
    {
        [HttpGet]
        public ActionResult<string> Get()
        {
            return "Hello from MyController!";
        }

        [HttpPost]
        public ActionResult<string> Post([FromBody] string data)
        {
            // 클라이언트로부터 받은 데이터를 처리하는 로직을 작성합니다.
            return "Received data: " + data;
        }
    }
}
