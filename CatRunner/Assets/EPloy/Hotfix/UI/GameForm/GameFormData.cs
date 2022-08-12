
using EPloy.Table;
using System.Collections.Generic;

public class GameFormData : DataStoreBase
{
    public DRRoleData playerData { get; private set; }
    public DRSkillData AttData { get; private set; }
    public List<DRSkillData> Skills { get; private set; }
    public Dictionary<int, float> skillCDs { get; private set; }

    public override void Create()
    {
        Skills = new List<DRSkillData>();
        skillCDs = new Dictionary<int, float>();
    }

    public void SetPlayer(int id)
    {
        Table<DRSkillData> DTSkill = GameModule.Table.GetDataTable<DRSkillData>();
        playerData = GameModule.Table.GetDataTable<DRRoleData>().GetDataRow(id);

        AttData = DTSkill.GetDataRow(playerData.RoleATT);
        skillCDs.Add(AttData.Id, 0);

        for (int i = 0; i < playerData.RoleSkills.Count; i++)
        {
            DRSkillData data = DTSkill.GetDataRow(playerData.RoleSkills[i]);
            Skills.Add(data);
            skillCDs.Add(data.Id, 0);
        }
    }
}