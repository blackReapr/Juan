using Juan.Interfaces;

namespace Juan.Areas.Admin;

public class DefaultVM<T> where T : class, IDefaultEntity
{
    public IEnumerable<T> Items { get; set; }
}
