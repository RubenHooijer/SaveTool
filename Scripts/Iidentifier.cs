using System.Collections.Generic;
namespace Common.SaveLoadSystem
{
    public interface Iidentifier
    {
        int id { get; set; }
        List<ComponentSave> componentSaves { get; set; }
        bool hasChanged { get; set; }
    }
}
