
namespace Remote.Linq.TestClassLibrary
{
    public class Product
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public override string ToString()
        {
            return string.Format("Product #{0} '{1}' ({2:C})", Id, Name, Price);
        }
    }
}
