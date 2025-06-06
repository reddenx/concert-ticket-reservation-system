using System.Drawing;

namespace ConcertoReservoApi.Infrastructure.Dtos;

public class AvailableEventSeatsView
{
    public SectionView[] Sections { get; set; }

    public class SectionView
    {
        public string Label { get; set; }
        public SeatView[] Seats { get; set; }

        //display info
        public Point[] DisplayPolygon { get; set; }
        public Point SeatOrigin { get; set; }

        public class SeatView
        {
            public string Id { get; set; }
            public string Label { get; set; }
            public SeatStates State { get; set; }
            public int? AvailableCount { get; set; } //if applicable for seat configuration
            public PriceLineItemView[] Price { get; set; }
            public SeatAddonView[] Addons { get; set; }

            //display info
            public Point Position;

            public enum SeatStates { Available, Reserved, Purchased } //maybe hide reserved as purchased for shopping audiences

            public class SeatAddonView
            {
                public string Id { get; set; }
                public string Label { get; set; }
                public PriceLineItemView[] Price { get; set; }
            }
        }
    }
}
