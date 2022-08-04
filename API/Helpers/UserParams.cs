namespace API.Helpers
{
    public class UserParams : PaginationParams
    {

        //for filtering
        public string CurrentUsername { get; set; }
        public string Gender { get; set; }

        public int minAge { get; set; } = 18;
        public int maxAge { get; set; } = 150;

        //for shorting
        public string OrderBy { get; set; } = "lastActive";
    }
}
