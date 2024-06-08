namespace FitVital.DAL.Pojos
{
    // Objeto auxiliar para formato de peticiones del API
    public class userRequest
    {
        public string? Username { get; set; }

        public string? Password { get; set; }

        public string? Email { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? PhoneNumber { get; set; }

        public DateTime? BirthDate { get; set; }

        public string? Gender { get; set; }
    }
}


