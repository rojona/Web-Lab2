namespace Web_Lab2.Repos;

public interface IUnitOfWork
{
    IProductRep Products { get; set; }
    ICustomerRep Customers { get; set; }
    IOrderRep Orders { get; set; }
    IOrderItemRep OrderItems { get; set; }
    
    Task<int> CompleteAsync();
}