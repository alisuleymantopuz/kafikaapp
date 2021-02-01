namespace api.orders.persistence.Entities
{
    public enum OrderStatus
    {
        Created = 1,
        InProgress = 2,
        Cancelled = 3,
        ShipperAssigned = 4,
        DeliveryInProgress = 5,
        Delivered = 6,
        CancelRequested = 7
    }
}
