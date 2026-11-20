using System.Net.Mail;
using WypozyczalniaSportowa.Domain.Exceptions;
namespace WypozyczalniaSportowa.Domain;

public sealed class Customer : Entity { private string _name = "", _email = ""; public Customer(string name, string email) { FullName = name; Email = email; } internal Customer(int id, string name, string email) : base(id) { FullName = name; Email = email; } public string FullName { get => _name; private set => _name = string.IsNullOrWhiteSpace(value) ? throw new DomainValidationException("Imię i nazwisko są wymagane.") : value.Trim(); } public string Email { get => _email; private set { try { _email = new MailAddress(value).Address; } catch (Exception ex) when (ex is ArgumentException or FormatException) { throw new DomainValidationException("Podaj poprawny e-mail."); } } } public override Entity Clone(bool deep) => new Customer(Id, FullName, Email); public override string ToString() => $"{FullName} <{Email}> (#{Id})"; }
