using WypozyczalniaSportowa.Domain;
namespace WypozyczalniaSportowa.Application; public sealed class RentalCreatedEventArgs(RentalReceipt receipt) : EventArgs { public RentalReceipt Receipt { get; } = receipt; public override string ToString() => $"Utworzono rezerwację #{Receipt.RentalId} na {Receipt.Total:C}."; }
