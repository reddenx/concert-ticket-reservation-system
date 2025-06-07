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


# Api Design Notes
I would liked to have diagramed this but ran out of time :/

## General
readonly objects have the View suffix, working (input/output) objects have the Dto suffix.  
I went with puts over patches for personal convenience, object patching can get a little messy and I wanted to keep things clean and simple for this prototype.

## Shopping
the primary shopping object is a readonly view with writable locations for updating fields.  
this was chosen over a more disassembled multi-endpoint solution to limit the amount of back and forth as these endpoints would likely be in high demand and ideally served with low latency. 
payment integration is assumed to be a front-end vendor widget or redirect to vendor that provides us with an id/token to use for capture once the user has given them their information.  
the general lifecycle is a user starts a shopping session for a given Event, proceeds to seat selection once they've cleared the queue (if present) starting the expiration timer, a user can then select seats up the the maximum allowed for the event, once selected, those seats/counts are reserved until their shopping session expires. the user then submits their data, resolving the validations present on their shopping session until they are ready for purchase. Hitting the purchase endpoint starts the checkout transaction during which all actions are locked. On a success they receive a receipt of their purchase with redemption information. on failure, if it's user resolvable, a validation issue is added that would need to be resolved before checkout is attempted again.

## Venues
endpoints were designed from the point of a view of an individual managing a venue, setting up section configurations available for events with a few helpful endpoints for management and planning. 
Venues have sections that have seats, multiple sections can be configured for the same location as alternatives for event organizers to select during event creation. 

## Events
events were design from the point of view of an event organizer. allowing the scheduling of sales date, drafting/publishing visibility, closing sales temporarily with handling of people already in the shopping process, and selecting a configuration for the venue.

# Design and Implementation

## Design
I aimed for a simplifed DDD architecture with room to split domains and layers to separate projects or apis in the future.
The main domains are Events, Venues, and Shopping.
There may be room for a Seating domain but it exists currently as a bit of an awkward subdomain of Shopping, with dependencies on the other two Domains.

### The API reflects the separation of domains
- `/events/public` has a few anonymous endpoints for searching events as an unprivilaged user
- `/events` is for event management and for users with the `Events` privilage
- `/venues/public` similarly has a few anonymous endpoitns for looking up venue information as an unprivilaged user
- `/venues` is for venue management to create and updating seating and section configurations, users must have the `Venues` privilage for access
- `/shopping` handles the purchasing workflow

### Current Feature Set
- venue section and seat arrangements, allows for multiple options over the same location (concert vs basketball game)
- event management, selecting the arrangement of seating sections and pricing, dates, visibility etc.
- shopping workflow, selecting an event, entering info, temporarily reserving seats while shopping, mock payment integration, checkout payment capture and receipt artifact, post checkout seat reference for redemtion

### Features that didn't quite make the time cut but could be added
- the venue workflow is incomplete
- floor/pit seating is a hack and would need to be just a bunch of individual seats
- shopping queue, intended to only allow a certain amount of users in the selection process at once to prevent a run on tickers and poor shopping experience

### Features considered during design/implementation
- promotions and discounts, creating a domain service that feeds the avilable seating service method based on shopping context, when calculating receipt, discounts would be looked up and applied to the price as modifiers in the shopping session's BuildReceipt method, or as modifiers to the price in Selected Seats during hydration

### Things that SHOULD be here for a production ready API
- UNIT TESTING, I just didn't get to this, and as a result it's likely full of bugs
  + Service and Core functionality should be 100% under tests
  + outbound infrastructure can be tested with integration testing in an isolated environment
  + inbound infrastructure can be tested with api tests in an isolated environment, I've used gatlingio, soapui, junit, and plain ol curl scripts before with automated deployments
- LOGGING is for the most part absent and should be present and along side error handling and detection
- domain object versioning, not fully implemented in a way that preserves transaction acidity on all objects
- domain objects for all managed objects, only managed to have time for a shopping object
- exception handling and rollback of transactions
  + anything with a cross domain mutation shuold have catch and rollback handling
- proper configuration abstraction (I usually like it to be able to merge execution args, appsettings, and runtime configuration changes together)
- many MANY invalid states are not checked, the patterns on the shopping object would be extended to cover that domain's cases

### Assumptions being made about hosting environment
- https terminating reverse proxy protecting the api
