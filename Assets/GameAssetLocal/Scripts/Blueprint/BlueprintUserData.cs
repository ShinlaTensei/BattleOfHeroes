using Base.Data.Structure;
using Base.Module;

namespace PaidRubik
{
    [BlueprintReader("Data", DataFormat.Json)]
    public class BlueprintUserData : BaseBlueprint<PlayerProto.Types.UserData>
    {
        public override void Load()
        {
            if (Data is not null)
            {
                
            }
        }

        public override void LoadDummyData()
        {
            
        }

        public void AddData(PlayerProto.Types.UserData data)
        {
            Data = data;
        }


    }
}