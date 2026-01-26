namespace FriendOfAward_Laubi_viek
{
   
        public class AuthServiceSimple
        {
            public bool ValidateLogin(string email, string password)
            {
                string sql = $"SELECT COUNT(*) FROM Admins " +
                             $"WHERE Email = '{email}' AND Password = '{password}'";

                try
                {
                    object? result = DbWrapperMySqlV2.Wrapper.RunQueryScalar(sql);

                    int count = Convert.ToInt32(result);
                    return count == 1;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("DB-Fehler: " + ex.Message);
                    return false;
                }
            }
        }
    
}
