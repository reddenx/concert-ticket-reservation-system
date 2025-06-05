# Concert Ticker Management System

# Received Requirements
## core feature set
- A event management
- B ticket reservations and sales
- C venue capacity managemnet

## domain features
- 1 events
  + 1.1 create/update concert events
  + 1.2 set ticket types and pricing
  + 1.3 manage available capacity
  + 1.4 basic event details
- 2 tickets
  + 2.1 reserve tickets for a time window
  + 2.2 purchase tickets
  + 2.3 cancel reservations
  + 2.4 view ticket availability

## Some assumptions and definitions:
- **reserving a ticket:** holding a seat open during the shopping process so a user does not get notified the venue is sold out during checkout. these are short time windows generally under an hour. (apologies if this isn't the intended definition, I could also see this being some sort of pre-reservation based on interest months in advance, or some priorty purchasing sort of concept as well)
- **the shopping queue:** there are two bad outcomes during ticket purchasing, overselling, and selling out while people believe they are still shopping. the ticket reservation mostly prevents the latter, but allowing too many users in to select reservations may produce a similar effect upstream of reserving a ticket. Limiting the amount of users into seating selection as well as open reservations should alleviate this. (explained further in user stories)
- **venue configuration:** venues are often modular and can cater to a concert, things like open floor, separate pit, removal of first stadium seating, etc.

# Let's talk some user stories (briefly)

## Event Management
*setting up a new concert:*
> as an Event Manager, I want to set up the basic details of an event (descriptive fields), date/time of event, selected venue (and venue configuration for the event), time sales open, visibility (public/hidden/date to go public), maximum ticket purchase per person. validation is a must.

*closing ticket sales for a concert*
> sometimes they close early if they're planning on adding a second event, or at least temporarily to ensure second event gets enough purchases.

*cancelling a concert and dealing with purchases*
> options like vendor credit, refund, or voucher for similar ticket at rescheduled event.

*(stretch) adding another concert of the same type (common use case)*
> common thing, a gig sells out and another needs to be added, convient for a manager to have this feature


## Ticket Reservations and Sales

*user working their way through the queue, selecting seats*
> user lands on site, entered into the queue, during this time they could walk away or wait to receive an email or text (stretch), once notified and landed onto the selection page they are shown a 30m countdown for checkout, the user then selects seats from the seating selector, first by section, then by specific seat (if applicable), selecting seats synchronously moves their state to reserved for all other users. after filling in the rest of the necessary shopping information the user moves to checkout. if the user passes all validation their order turns to a purchase, the reserved tickets become sold, and the user is given ticketing information.

## Venue Management

*setting up a venue*  

*modifying a venue*  
> would have to be versioned, and dated. would notify of conflicting configurations for existing events.