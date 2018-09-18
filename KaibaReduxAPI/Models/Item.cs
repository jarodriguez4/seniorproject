using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KaibaReduxAPI.Models
{
    public class Item
        // repersents a single menu item
    {
        // class variables
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        // this number represents the position within the section for this item
        // ie. Is this item first, fifth, or seventh
        public double Position { get; set; }

        // this holds the path to the picture for this item
        public string PicturePath { get; set; }

        // the ID of it's parent section
        public int SectionID { get; set; }

        // the list of prices for this item
        public List<Priceline> PriceLineList { get; set; }

        public Item(int id, string name, string description, float position, string picturePath)
            // Standard constructor- takes and assigns the class variables
        {
            Id = id;
            Name = name;
            Description = description;
            Position = position;
            PicturePath = picturePath;
            PriceLineList = new List<Priceline>();
        }

        public Item(string name, string description, float position, string picturePath, int sectionID)
        // constructor without giving an itemID but with a SectionID
        {
            Name = name;
            Description = description;
            Position = position;
            PicturePath = picturePath;
            SectionID = sectionID;
            PriceLineList = new List<Priceline>();
        }

        public Item()
            // empty constructor, only initializes the list
        {
            PriceLineList = new List<Priceline>();
        }

        public Item(int id, string name, string description, float position, string picturePath, int sectionID)
            // This constructor overloads the first one, allowing you to also specifiy the SectionID
            // Notice the syntax to call a different constructor is : this (id, name, description, position, picturePath)
            // Unlike Java this is done before the bracketed code body
            : this (id, name, description, position, picturePath)
        {
            SectionID = sectionID;
        }

        public void addPriceline(Priceline price)
        // adds a prceline to this item
        // also assigns this item's ID to the priceline's ItemID field
        {
            price.ItemID = Id;
            PriceLineList.Add(price);
        }

        public List<Priceline> getPricelineList()
        {
            return PriceLineList;
        }
    }
}
