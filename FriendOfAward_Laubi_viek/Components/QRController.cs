using Microsoft.AspNetCore.Mvc;
using System;

using Microsoft.AspNetCore.Mvc;
using System.Data;

[ApiController]
[Route("api/qr")]
public class QRController : ControllerBase
{
    private readonly DbWrapperMySqlV2 _db = DbWrapperMySqlV2.Wrapper;

    [HttpGet("next")]
    public IActionResult GetNext()
    {
        string code;

        do
        {
            code = Guid.NewGuid().ToString("N");

            var result = _db.RunQueryScalar(
                $"SELECT COUNT(*) FROM QRCodes WHERE Code = '{code}'");
        }
        while (Convert.ToInt32(
            _db.RunQueryScalar(
                $"SELECT COUNT(*) FROM QRCodes WHERE Code = '{code}'")) > 0);

        _db.RunNonQuery(
            $"INSERT INTO QRCodes (Code) VALUES ('{code}')");

        return Ok(code);
    }
}
