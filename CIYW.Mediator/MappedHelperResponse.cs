namespace CIYW.Mediator;

public class MappedHelperResponse<TMapped, TEntity>
{
    public MappedHelperResponse(TMapped mapped, TEntity entity)
    {
        MappedEntity = mapped;
        Entity = entity;
    }
    public TMapped MappedEntity { get; set; }
    public TEntity Entity { get; set; }
}