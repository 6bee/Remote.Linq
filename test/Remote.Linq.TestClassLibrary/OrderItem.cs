
namespace Remote.Linq.TestClassLibrary
{
    public class OrderItem
    {
        public long ProductId { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal TotalAmount
        {
            get { return Quantity * UnitPrice; }
        }

        public override string ToString()
        {
            return string.Format("Prod #{0}: {1} * {2:C} = {3:C}", ProductId, Quantity, UnitPrice, TotalAmount);
        }
    }
}
