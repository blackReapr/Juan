using Juan.Models;

namespace Juan.ViewModels;

public class AccountVM
{
    public IEnumerable<Order> Orders { get; set; }
}
