namespace FriendOfAward_Laubi_viek
{
    public class QRCode
    {
        public int Id { get; set; }
        public string Code { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

}
