using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KaibaReduxAPI.Models
{
    public class Menu
        // This class represents a whole independent menu
        // As you might find at two different locations 
        // Or a lunch and a dinner menu (both of which have multiple sections, but different items & prices)
    {
        // Class variables
        // Note how they use { get; set; } to automatically create getter/setter functionality
        // Also note how all attributes are public, they must be public for the .NET JSON serializer to function properly
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        // This value represents this menu's position on the page (1st, 2nd, or 3rd)
        public double Position { get; set; }

        // This list contains this menu's various sections
        public List<Section> SectionList { get; set; }

        public Menu()
            // empty constructor, only initializes the list
        {
            SectionList = new List<Section>();
        }

        public Menu(int id, string name, string description, float position)
            // standard constructor- takes the menu's info and assigns it to the appriopriate variables
        {
            Id = id;
            Name = name;
            Description = description;
            Position = position;
            SectionList = new List<Section>();
        }

        public Menu (string name, string description, float position)
            // constructor without id attribute assignment
        {
            Name = name;
            Description = description;
            Position = position;
            SectionList = new List<Section>();
        }

    public void addSection(Section section)
        {
            SectionList.Add(section);
        }

        public List<Section> getSectionList()
        {
            return SectionList;
        }
    }
}
