using System;
using System.Drawing;
using System.Linq;
using static ConcertoReservoApi.Infrastructure.Dtos.Shopping.AvailableEventSeatsView.SectionView.SeatView;
using static ConcertoReservoApi.Infrastructure.DataRepositories.ISeatingRepository;

namespace ConcertoReservoApi.Infrastructure.Dtos.Shopping;

public class AvailableEventSeatsView
{
    public SectionView[] Sections { get; set; }

    public class SectionView
    {
        public string Label { get; set; }
        public SeatView[] Seats { get; set; }

        //display info
        public Point[] DisplayPolygon { get; set; }
        public Point Position { get; set; }

        public class SeatView
        {
            public string Id { get; set; }
            public string Label { get; set; }
            public SeatStatesDto State { get; set; }
            public PriceLineItemView[] Price { get; set; }
            public SeatAddonView[] Addons { get; set; }

            //display info
            public Point Position { get; set; }

            public enum SeatStatesDto { Available, Reserved, Purchased } //maybe hide reserved as purchased for shopping audiences

            public class SeatAddonView
            {
                public string Id { get; set; }
                public string Label { get; set; }
                public PriceLineItemView[] Price { get; set; }
            }
        }
    }

    public static AvailableEventSeatsView FromCore(EventSectionData[] sections)
    {
        return new AvailableEventSeatsView
        {
            Sections = sections.Select(a => new SectionView
            {
                Label = a.Label,
                DisplayPolygon = a.DisplayPolygon,
                Position = a.DisplayPosition,
                Seats = a.Seats.Select(s => new SectionView.SeatView
                {
                    Id = s.SeatId,
                    Label = s.Label,
                    Addons = s.AvailableAddons.Select(a => new SeatAddonView
                    {
                        Id = a.AddonId,
                        Label = a.Label,
                        Price = [new PriceLineItemView { Label = null, Amount = a.Price }],
                    }).ToArray(),
                    Price = [new PriceLineItemView { Label = null, Amount = s.Price }],
                    State = Map(s.Status),
                    Position = s.DisplayPosition,
                }).ToArray()
            }).ToArray(),
        };
    }
    private static SeatStatesDto Map(SeatStatuses status) => status switch
    {
        SeatStatuses.Available => SeatStatesDto.Available,
        SeatStatuses.Reserved => SeatStatesDto.Reserved,
        SeatStatuses.Purchased => SeatStatesDto.Purchased,
        _ => throw new ArgumentOutOfRangeException()
    };
}
