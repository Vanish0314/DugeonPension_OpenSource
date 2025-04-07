
namespace GameFramework.Data
{
    internal sealed partial class DataManager : IDataManager
    {
        public enum DataStatus
        {
            None,
            Inited,
            Preloaded,
            Loaded,
            Unloaded,
        }
    }
}

