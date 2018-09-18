using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KaibaReduxAPI.Models
{
    public class Priceline
        // represents a quantity-price combination for a specific item
        // Ex: Ribs-
        //          full rack $20
        //          half rack $12
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public double Position { get; set; }
        public int ItemID { get; set; }

        public Priceline()
            // empty constructor
        {

        }

        public Priceline(int id, string description, decimal price, float position)
        {
            Id = id;
            Description = description;
            Price = price;
            Position = position;
        }

        public Priceline(string description, decimal price, float position, int itemID)
            // constructor without a PricelineID but with an ItemID
        {
            Description = description;
            Price = price;
            Position = position;
            ItemID = itemID;
        }

        public Priceline(int id, string description, decimal price, float position, int itemId)
            // This constructor overloads the first one, allowing you to also specifiy the ItemID
            // Notice the syntax to call a different constructor is : this(id, description, price, position)
            // Unlike Java this is done before the bracketed code body
            : this(id, description, price, position)
        {
            ItemID = itemId;
        }
    }
}
