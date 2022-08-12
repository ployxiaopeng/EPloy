using EPloy.Table;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public static class TableExtension
{
    private static Table<DREntity> dREntitys = null;
    private static Table<DREntity> DREntitys
    {
        get
        {
            if (dREntitys == null)
            {
                dREntitys = GameModule.Table.GetDataTable<DREntity>();
            }
            return dREntitys;
        }
    }

    public static DREntity GetDtObj(this int id)
    {
        return DREntitys.GetDataRow(id);
    }
}