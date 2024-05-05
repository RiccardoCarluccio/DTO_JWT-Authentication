namespace Esercizio_DTO.Autenticazione
{
    public class Utenti
    {
        static private List<Utente> _utenti = new List<Utente>()
        {
            new Utente()
            {
                Username = "Adrian",
                Password = "adrian",
                Ruolo = "Admin",
            },

            new Utente()
            {
                Username = "Silviu",
                Password = "silviu",
                Ruolo = "User",
            },
        };

        static public Utente GetUser(String username)
        {
            return _utenti.FirstOrDefault(ut => ut.Username == username);
        }
    }
}
