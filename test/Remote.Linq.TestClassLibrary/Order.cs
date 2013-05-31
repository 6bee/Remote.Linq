using System.Collections.Generic;
using System.Linq;

namespace Remote.Linq.TestClassLibrary
{
    public class Order
    {
        public long Id { get; set; }

        public IList<OrderItem> Items
        {
            get { return _items ?? (_items = new List<OrderItem>()); }
        }
        private IList<OrderItem> _items;

        public decimal TotalAmount
        {
            get { return Items.Sum(i => i.TotalAmount); }
        }

        public override string ToString()
        {
            return string.Format("Order: {0} Item{2}  Total {1:C}", Items.Count, TotalAmount, Items.Count > 1 ? "s" : null);
        }
    }
}
