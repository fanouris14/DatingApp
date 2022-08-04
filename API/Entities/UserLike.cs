namespace API.Entities
{
    public class UserLike
    {
        public AppUser SourceUser { get; set; } //user that is liking the other user
        public int SourceUserId { get; set; }


        public AppUser LikedUser { get; set; } //user that has been liked
        public int LikedUserId { get; set; }

    }
}
