namespace FriendOfAward_Laubi_viek
{
   
        public class AuthServiceSimple
        {
    private readonly DbWrapperMySqlV2 db = DbWrapperMySqlV2.Wrapper;

            public bool ValidateLogin(string email, string password)
            {
        string sql = $@"
            SELECT COUNT(*) 
            FROM admins 
            WHERE Email = '{email}' AND Password = '{password}'
        ";

        object? result = db.RunQueryScalar(sql);

        if (result == null || result == DBNull.Value)
                    return false;

        return Convert.ToInt32(result) == 1;
        }
    
}
