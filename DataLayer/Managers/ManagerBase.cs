using DataLayer.Context;


namespace DataLayer.Managers
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class ManagerBase
    {
        protected MyselfContext Context;

        protected ManagerBase(MyselfContext context)
        {
            Context = context;
        }
    }
}
