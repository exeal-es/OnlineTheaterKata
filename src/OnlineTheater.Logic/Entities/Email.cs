
namespace OnlineTheater.Logic.Entities
{
    public class Email
    {
        private readonly string valor;

        public Email(string valor)
        {
            this.valor = valor;
        }

        public string Valor => valor;

        public override bool Equals(object? obj)
        {
            return obj is Email email &&
                   valor == email.valor;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(valor);
        }
    }
}