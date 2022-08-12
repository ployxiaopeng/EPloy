using EPloy.Hotfix.Table;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EPloy.Hotfix
{
    public static class TableExtension
    {
        private static Table<DREntity> dREntitys = null;
        private static Table<DREntity> DREntitys
        {
            get
            {
                if (dREntitys == null)
                {
                    dREntitys = HotFixMudule.DataTable.GetDataTable<DREntity>();
                }
                return dREntitys;
            }
        }

        public static DREntity GetDtObj(this int id)
        {
            return DREntitys.GetDataRow(id);
        }
    }
}