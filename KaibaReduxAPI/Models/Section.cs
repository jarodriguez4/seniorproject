using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KaibaReduxAPI.Models
{
    public class Section
        // Represents a menu section, like appetizers or desserts
    {
        // class variables
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        // this number represents the position for this section within the menu
        // ie. Is this section first, fifth, or seventh
        public double Position { get; set; }

        // this holds the path to the picture for this section
        public string PicturePath { get; set; }

        // the ID of the menu that this section is on 
        public int MenuID { get; set; }

        // the list of items in this section
        public List<Item> ItemList { get; set; }

        public Section(int id, string name, string description, float position, string picturePath)
        // Standard constructor- takes and assigns the class variables
        {
            Id = id;
            Name = name;
            Description = description;
            Position = position;
            PicturePath = picturePath;
            ItemList = new List<Item>();
        }

        public Section(string name, string description, float position, string picturePath, int menuID)
        // constructor without specifying a Section Id, but with a MenuID
        {
            Name = name;
            Description = description;
            Position = position;
            PicturePath = picturePath;
            MenuID = menuID;
            ItemList = new List<Item>();
        }

        public Section()
            // empty constructor, only initializes the list
        {
            ItemList = new List<Item>();
        }

        public Section(int id, string name, string description, float position, string picturePath, int menuID)
            // This constructor overloads the first one, allowing you to also specifiy the MenuID
            // Notice the syntax to call a different constructor is : this(id, name, description, position, picturePath)
            // Unlike Java this is done after the method declaration but before the bracketed code body
            : this(id, name, description, position, picturePath)
        {
            MenuID = menuID;
        }

        public void addItem(Item item)
            // adds an item to this section's item list
            // also assigns this section's ID to the item's sectionID field
        {
            item.SectionID = Id;
            ItemList.Add(item);
        }

        public List<Item> getItemList()
        {
            return ItemList;
        }
    }
}
