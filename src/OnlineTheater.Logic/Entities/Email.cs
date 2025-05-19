using CSharpFunctionalExtensions;

namespace OnlineTheater.Logic.Entities
{
    public class Email
    {
        private readonly string valor;

        private Email(string valor)
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

        public static Result<Email> Create(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
                return Result.Failure<Email>("Email cannot be null or empty");

            // Validación de formato de email usando expresión regular
            if (!System.Text.RegularExpressions.Regex.IsMatch(valor, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                return Result.Failure<Email>("Email is invalid");

            return Result.Success(new Email(valor));
        }
    }
}