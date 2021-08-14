namespace heitech.efXt
{
    ///<summary>
    /// Use as Entry to UnitOfWork with or without a transaction in place
    ///</summary>
    public interface ICommands
    {
         ICommands WithTransaction();
         IUnitOfWork GetUnitOfWork();
    }
}