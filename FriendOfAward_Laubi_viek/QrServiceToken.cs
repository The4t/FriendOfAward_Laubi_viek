using QRCoder;

public class QrServiceToken
{
    private readonly DbWrapperMySqlV2 db = DbWrapperMySqlV2.Wrapper;

    public string CreateAndStoreToken()
    {
        string token = Guid.NewGuid().ToString("N");

        string sql = $"INSERT INTO QrTokens (Token, IsUsed) VALUES ('{token}', 0)";
        db.RunNonQuery(sql);

        return token;
    }

    public bool ValidateToken(string token)
    {
        string sql = $"SELECT IsUsed FROM QrTokens WHERE Token = '{token}'";
        object? result = db.RunQueryScalar(sql);

        if (result == null || result == DBNull.Value)
            return false;

        bool used = Convert.ToInt32(result) == 1;
        if (used)
            return false;

        string update = $"UPDATE QrTokens SET IsUsed = 1 WHERE Token = '{token}'";
        db.RunNonQuery(update);

        return true;
    }

    public string GenerateQrBase64(string token)
    {
        string url = $"http://172.17.7.60:5432/scan?token={token}";


        var gen = new QRCodeGenerator();
        var data = gen.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
        var qr = new PngByteQRCode(data);
        var bytes = qr.GetGraphic(20);

        return Convert.ToBase64String(bytes);
    }
    
}
