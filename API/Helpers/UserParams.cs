namespace API.Helpers
{
    public class UserParams : PaginationParams
    {
        public string CurrentUsername { get; set; }
        public string Gender { get; set; }
        public int MinAge { get; set; } = 18;
        public int MaxAge { get; set; } = 110;
        public string NativeLanguage { get; set; }
        public string TargetLanguage { get; set; }
        public string OrderBy { get; set; } = "lastActive";
    }
}
