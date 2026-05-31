using Chapeau.Enums;

namespace Chapeau.Models
{
    public class Table
    {
        public int TableId { get; set; }

        public int TableCapacity { get; set; }

        public TableStatus TableStatus { get; set; }
    }

}