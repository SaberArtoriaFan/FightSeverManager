using Saber.Base;
using Saber.ECS;
namespace XianXia.Unit
{
    //public interface IOrgan 
    //{ 
    //    //void SetEnable(bool enable);
    //}



    public abstract class OrganBase:ComponentBase
    {
        protected abstract ComponentType componentType { get; }
        public override ComponentType ComponentType => componentType;
        UnitBase unit;
        public UnitBase OwnerUnit => unit;
        public OrganBase() 
        {

        }
        protected override void InitComponent(IContainerEntity owner)
        {
            base.InitComponent(owner);
            this.unit = owner as UnitBase;
            InitComponent(unit);

        }
        protected virtual void InitComponent(EntityBase unit)
        {
            this.ClearEnable();
        }
        public override void Destory()
        {
            base.Destory();
            unit = null;
        }
    }
}
